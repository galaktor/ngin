/**************************************
 * FILE:          EntityExtensionManager.cs
 * DATE:          05.01.2010 10:17:06
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGin.Core.Logging;
using NGin.Core.Configuration;

namespace NGin.Core.Scene
{
    public interface IEntityExtensionManager
    {
        IEntityExtension CreateNewEntityExtension<TEntityExtension>( params object[] parameters );
    }
    [Service( typeof(EntityExtensionManager), typeof(IEntityExtensionManager), true)]
    public class EntityExtensionManager: IEntityExtensionManager
    {
        internal ILogManager LogManager { get; set; }
        internal IPluginManager PluginManager { get; set; }
        internal INGinCore Core { get; set; }

        public EntityExtensionManager( ILogManager logManager, INGinCore core )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "logManager" );
            }

            if ( core == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "core" );
                logManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            this.Core = core;
            this.LogManager = logManager;
        }

        public IEntityExtension CreateNewEntityExtension<TEntityExtension>( params object[] parameters )
        {
            IEntityExtension result = this.Core.GetService<TEntityExtension>( parameters ) as IEntityExtension;

            if ( result == null )
            {
                InvalidOperationException invopEx = new InvalidOperationException( "Returned instance is not of type IEntityExtension" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, invopEx, invopEx.Message );
                throw invopEx;
            }

            return result;
        }
    }
}
