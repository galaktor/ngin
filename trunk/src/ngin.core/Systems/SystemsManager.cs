/**************************************
 * FILE:          SystemsManager.cs
 * DATE:          05.01.2010 10:19:35
 * AUTHOR:        Raphael B. Estrada
 * AUTHOR URL:    http://www.galaktor.net
 * AUTHOR E-MAIL: galaktor@gmx.de
 * 
 * The MIT License
 * 
 * Copyright (c) 2010 Raphael B. Estrada
 * Author URL:    http://www.galaktor.net
 * Author E-Mail: galaktor@gmx.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * ***********************************/
using System.Collections.Generic;
using NGin.Core.Configuration;
using NGin.Core.Logging;
using System.Reflection;
using NGin.Core.Tasks;
using System.Text;
using System;

namespace NGin.Core.Systems
{
    [Service(typeof(SystemsManager), typeof(ISystemsManager), true)]
    public class SystemsManager: ISystemsManager
    {
        internal ILogManager LogManager { get; private set; }
        private object registeredSystemsLock = new object();
        internal Dictionary<string, ISystem> registeredSystems = new Dictionary<string, ISystem>();

        public IEnumerable<ISystem> RegisteredSystems
        {
            get
            {
                lock ( this.registeredSystemsLock )
                {
                    return this.registeredSystems.Values;
                }
            }
        }

        public bool IsDisposing
        {
            get;
            private set;
        }
		~SystemsManager()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
		}
		private void Dispose(bool disposing)
		{
            this.IsDisposing = true;

			if (disposing)
			{                
                this.DisposeManaged();
			}
           
            this.DisposeUnmanaged();
		}

        protected virtual void DisposeUnmanaged()
        {
            lock ( this.registeredSystemsLock )
            {
                foreach ( ISystem system in this.registeredSystems.Values )
                {
                    system.Dispose();
                }
            }
        }

        protected virtual void DisposeManaged()
        { }

        public SystemsManager( IPluginManager pluginManager, ILogManager logManager, INGinCore core, IMainLoopManager mainLoopManager )
        {
            this.LogManager = logManager;

            foreach ( SystemAttribute plugin in pluginManager.GetPluginsForType( typeof( SystemAttribute ) ) )
            {
                // create and store system
                //ConstructorInfo constructor = plugin.SystemType.GetConstructor( plugin.ConstructorParameterTypes );
                //ISystem system = plugin.SystemType.InvokeMember( plugin.SystemType.Name, global::System.Reflection.BindingFlags.CreateInstance, null, null, new object[] { this.LogManager } ) as ISystem;
                ISystem system = core.GetService( plugin.SystemType ) as ISystem;                

                lock ( this.registeredSystemsLock )
                {
                    this.registeredSystems.Add( system.Name, system );
                }                
            }
        }        

        public ISystem GetSystem( string systemKey )
        {
            if ( systemKey == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "systemKey" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            ISystem result = null;
            lock ( this.registeredSystemsLock )
            {
                result = this.registeredSystems[ systemKey ];
            }
            
            return result;
        }
    }
}
