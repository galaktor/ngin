/**************************************
 * FILE:          StateManager.cs
 * DATE:          05.01.2010 10:18:55
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
using NGin.Core.States.RHFSM;
using NGin.Core.Logging;
using NGin.Core.Configuration;
using NGin.Core.Tasks;

namespace NGin.Core.States
{
    public interface IStateManager: IDisposable
    {
        IState GetCurrentState();   
    }
    [Service( typeof( StateManager ), typeof( IStateManager ), true )]
    internal class StateManager: IStateManager
    {
        public bool IsDisposing
        {
            get;
            private set;
        }
		~StateManager()
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
        { }

        protected virtual void DisposeManaged()
        {
            this.LogManager = null;
            this.Core = null;
            this.LoopManager = null;
            this.PluginManager = null;

            lock ( this.stateMachineLock )
            {
                if ( this.stateMachine != null )
                {
                    this.stateMachine.Dispose();
                }
            }
        }

        private object stateMachineLock = new object();
        private IMachine stateMachine;
        internal IMachine StateMachine
        {
            get
            {
                lock ( this.stateMachineLock )
                {
                    return this.stateMachine;
                }
            }
            set
            {
                lock ( this.stateMachineLock )
                {
                    this.stateMachine = value;
                }
            }
        }

        internal ILogManager LogManager
        {
            get;
            set;
        }

        internal IPluginManager PluginManager
        {
            get;
            set;
        }

        internal INGinCore Core { get; set; }
        internal IMainLoopManager LoopManager { get; set; }

        public StateManager( ILogManager logManager, IPluginManager pluginManager, INGinCore core )
            : this( logManager, pluginManager, new Machine( logManager ), core )
        {
        }

        internal StateManager( ILogManager logManager, IPluginManager pluginManager, IMachine stateMachine, INGinCore core )
        {
            if ( logManager == null )
            {
                string message = "The log manager must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "logManager", message );
                throw argnEx;
            }

            if ( pluginManager == null )
            {
                string message = "The plugin manager must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "pluginManager", message );
                logManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            if ( stateMachine == null )
            {
                string message = "The state machine must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "stateMachine", message );
                logManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            if ( core == null )
            {
                string message = "The core must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "core", message );
                logManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            this.LogManager = logManager;
            this.PluginManager = pluginManager;
            this.StateMachine = stateMachine;
            this.Core = core;

            this.Core.RunStarted += new CoreStateDelegate( this.Core_RunStarted );
            this.Core.RunStopped += new CoreStateDelegate( this.Core_RunStopped );            
        }

        void Core_RunStopped( INGinCore senderCore )
        {
            this.Shutdown( false );
        }

        void Core_RunStarted( INGinCore senderCore )
        {
            this.Initialize( this.PluginManager, this.StateMachine, this.Core );
        }

        internal void Initialize( IPluginManager pluginManager, IMachine stateMachine, INGinCore core )
        {
            // ensure plugin manager is not null
            if ( pluginManager == null )
            {
                string message = "The plugin manager must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "logManager", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            // ensure state machine is not null
            if ( stateMachine == null )
            {
                string message = "The state machine must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "stateMachine", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            if ( core == null )
            {
                string message = "The core must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "core", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argnEx, message );
                throw argnEx;
            }

            IEnumerable<Attribute> statePlugins = pluginManager.GetPluginsForType( typeof( NGinRootStateAttribute ) );

            // ensure that only one root state is defined
            if ( statePlugins.Count<Attribute>() > 1 )
            {
                string message = "More than one root state plugin was found. There must be only 1.";
                InvalidOperationException invOpEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, invOpEx, message );
                throw invOpEx;
            }

            // get attribute
            NGinRootStateAttribute rootStateAtt = statePlugins.ElementAt<Attribute>( 0 ) as NGinRootStateAttribute;

            // make sure attribute is not null
            if ( rootStateAtt == null )
            {
                string message = "Error loading the root state plugin.";
                InvalidOperationException invOpEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, invOpEx, message );
                throw invOpEx;
            }
            
            // add type to machine root
            Type rootStateType = rootStateAtt.RootStateType;
            IState state = core.GetService( rootStateType ) as IState;

            if ( state == null )
            {
                string message = String.Format( System.Globalization.CultureInfo.InvariantCulture, "An error occurred while retrieving instance of {0}.", rootStateType.FullName );
                InvalidOperationException invopEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, invopEx, message );
                throw invopEx;
            }

            this.StateMachine.RootState.AddSubState( state );            

            stateMachine.Initialize( state.Name );
        }

        public void Shutdown( bool autoDispose )
        {
            this.StateMachine.Shutdown( autoDispose );
        }

        public IState GetCurrentState()
        {
            IState result = null;

            result = this.stateMachine.ActiveState;

            return result;
        }
    }
}
