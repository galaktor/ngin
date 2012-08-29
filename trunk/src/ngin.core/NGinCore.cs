/**************************************
 * FILE:          NGinCore.cs
 * DATE:          05.01.2010 10:21:58
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
using Autofac;
using Autofac.Builder;
using NGin.Core.Configuration;
using NGin.Core.Configuration.Modules;
using NGin.Core.Logging;
using NGin.Core.Tasks;

namespace NGin.Core
{
    public delegate void CoreStateDelegate( INGinCore senderCore );
    public class NGinCore : INGinCore
    {
        private IContainer container;
        private ContainerBuilder builder;

        internal INGinConfig CoreConfig { get; set; }

        #region Constructors

        #region Public Constructors

        public NGinCore( string coreConfigFilePath )
        {
            // create id
            this.Id = new Guid();

            // create core configuration first thing
            this.CoreConfig = new NGinConfig( coreConfigFilePath );

            // initialize core with config
            this.Initialize( this.CoreConfig );
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties

        public INGinConfig Config
        {
            get;
            private set;
        }

        public Guid Id
        {
            get;
            private set;
        }

        #endregion Public Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        private object runStoppedEventLock = new object();
        private event CoreStateDelegate runStopped;
        public event CoreStateDelegate RunStopped
        {
            add
            {
                lock ( this.runStoppedEventLock )
                {
                    this.runStopped += value;
                }
            }
            remove
            {
                lock ( this.runStoppedEventLock )
                {
                    this.runStopped -= value;
                }
            }
        }

        internal void FireRunStartedEvent()
        {
            lock ( this.runStartedEventLock )
            {
                if ( this.runStarted != null )
                {
                    this.runStarted( this );
                }
            }
        }

        internal void FireRunStoppedEvent()
        {
            lock ( this.runStoppedEventLock )
            {
                if ( this.runStopped != null )
                {
                    this.runStopped( this );
                }
            }
        }

        private object runStartedEventLock = new object();
        private event CoreStateDelegate runStarted;
        public event CoreStateDelegate RunStarted
        {
            add
            {
                lock ( this.runStartedEventLock )
                {
                    this.runStarted += value;
                }
            }
            remove
            {
                lock ( this.runStartedEventLock )
                {
                    this.runStarted -= value;
                }
            }
        }

        internal void Initialize( INGinConfig coreConfig )
        {
            // create empty container
            this.container = new Container();

            // create builder
            this.builder = new ContainerBuilder();

            // register self
            builder.Register( this ).As<INGinCore>().SingletonScoped();

            // register core config
            builder.Register( coreConfig ).As<INGinConfig>().SingletonScoped();

            // extract modules from config
            ModuleLoader loader = new ModuleLoader( coreConfig );

            // load and prepare modules
            // make sure core module gets registered first!
            IModule coreModule = null;
            IList<IModule> otherModules = new List<IModule>();
            foreach ( IModule module in loader.Load() )
            {
                if ( module.GetType() == typeof( CoreModule ) )
                {
                    coreModule = module;
                }
                else
                {
                    otherModules.Add( module );
                }                
            }

            // register core module manually
            builder.RegisterModule( coreModule );

            // register other modules
            foreach ( IModule module in otherModules )
            {
                builder.RegisterModule( module );
            }

            // build registered components
            builder.Build(this.container);

            ILogManager logManager = this.container.Resolve<ILogManager>();
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Core initialization complete." );

            // TEST: force initialize task manager
            ITaskManager taskManager = this.GetService<ITaskManager>();
        }

        public void Run()
        {            
            IMainLoopManager loopManager = this.GetService<IMainLoopManager>();
            this.FireRunStartedEvent();

            loopManager.RunAsynchronous();
        }

        public void Stop()
        {            
            IMainLoopManager loopManager = this.GetService<IMainLoopManager>();
            this.FireRunStoppedEvent();
            loopManager.Stop();
        }

        public void WaitForEnd()
        {
            IMainLoopManager loopManager = this.GetService<IMainLoopManager>();
            loopManager.WaitForLoopThreadEnd();
        }

        public void WaitForEnd( TimeSpan timeout )
        {
            IMainLoopManager loopManager = this.GetService<IMainLoopManager>();
            loopManager.WaitForLoopThreadEnd( timeout );
        }

        public bool IsDisposing
        {
            get;
            private set;
        }
		~NGinCore()
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
            if ( this.container != null )
            {
                this.container.Dispose();
            }

            this.container = null;

            this.builder = null;
        }

        public bool Equals( INGinManager other )
        {
            return this.Id.Equals( other );
        }

        public TService GetService<TService>()
        {
            return this.container.Resolve<TService>();
        }

        public TService GetService<TService>( params object[] parameters )
        {

            IList<Parameter> autofacParameters = new List<Parameter>( parameters.Length );
            for ( int i = 0; i < parameters.Length; i++ )
            {
                PositionalParameter p = new PositionalParameter( i, parameters[ i ] );
                autofacParameters.Add( p );
            }

            return this.container.Resolve<TService>( autofacParameters );
        }

        public object GetService( Type serviceType )
        {
            return this.container.Resolve( serviceType );
        }

        #endregion Public Methods

        #endregion Methods
    }
}