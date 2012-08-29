/**************************************
 * FILE:          TaskManager.cs
 * DATE:          05.01.2010 10:20:16
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
using System.Linq;
using System.Threading;
using NGin.Core.Logging;
using NGin.Core.Systems;
using NGin.Core.Configuration;
using System.Collections.Generic;
using NGin.Core.Scene;
using NGin.Core.States;

namespace NGin.Core.Tasks
{
    public interface ITaskManager: IDisposable
    {
        void CallAllSystemTasksSequential();
        void CallAllSystemTasksAsynchronous();
        void CallSystemTasksAsynchronous( IEnumerable<string> systemNames );
        int MaxWorkerThreads { get; set; }
        int MinWorkerThreads { get; set; }
        int AvailableWorkerThreads { get; }

    }
    [Service( typeof( TaskManager ), typeof( ITaskManager ), true )]
    public class TaskManager: ITaskManager
    {
        internal ILogManager LogManager { get; set; }
        internal ISystemsManager SystemsManager { get; set; }
        internal IMainLoopManager MainLoopManager { get; set; }
        internal IStateManager StateManager { get; set; }

        public bool IsDisposing
        {
            get;
            private set;
        }
		~TaskManager()
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
        { }

        public int AvailableWorkerThreads
        {
            get
            {
                int workerThreads, compThreads;
                ThreadPool.GetAvailableThreads( out workerThreads, out compThreads );
                return workerThreads;
            }            
        }

        public int MaxWorkerThreads
        {
            get
            {
                int result, comp;
                ThreadPool.GetMaxThreads( out result, out comp );
                return result;
            }
            set
            {
                ThreadPool.SetMaxThreads( value, value );
            }
        }

        public int MinWorkerThreads
        {
            get
            {
                int result, comp;
                ThreadPool.GetMinThreads( out result, out comp );
                return result;
            }
            set
            {
                ThreadPool.SetMinThreads( value, value );
            }
        }

        public TaskManager( ILogManager logManager, ISystemsManager systemsManager, IMainLoopManager mainLoopManager, IStateManager stateManager )
        {
            if ( logManager == null )
            {
                string message = "The log manager must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "logManager", message );
                throw argEx;
            }

            this.LogManager = logManager;

            if ( systemsManager == null )
            {
                string message = "The systems manager must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "systemsManager", message );
                LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            if ( mainLoopManager == null )
            {
                string message = "The main loop manager must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "mainLoopManager", message );
                LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            if ( stateManager == null )
            {
                string message = "The state manager must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "stateManager", message );
                LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            this.SystemsManager = systemsManager;
            this.MainLoopManager = mainLoopManager;
            this.StateManager = stateManager;

            this.MainLoopManager.HeartbeatStarted += new HeartbeatDelegate( this.MainLoopManager_HeartbeatStarted );

            // set min and max to processor count
            int defaultThreadCount = Environment.ProcessorCount;
            ThreadPool.SetMaxThreads( defaultThreadCount, defaultThreadCount );
            ThreadPool.SetMinThreads( defaultThreadCount, defaultThreadCount );

            int minWorker, minComp, maxWorker, maxComp;
            ThreadPool.GetMinThreads( out minWorker, out minComp );
            ThreadPool.GetMaxThreads( out maxWorker, out maxComp );

            this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "ThreadPool WorkerThreads - MIN: {0}, MAX: {1}", minWorker, maxWorker );
        }

        void MainLoopManager_HeartbeatStarted( TimeSpan timeSinceLast )
        {
            //// method for only calling used systems
            //this.CallSystemTasksAsynchronous( this.StateManager.GetCurrentState().GetRequiredSystems( true ) );

            this.CallAllSystemTasksAsynchronous();
        }

        [Obsolete( "Use Asynchronous processing instead.", false )]
        public void CallAllSystemTasksSequential()
        {
            foreach ( ISystem system in this.SystemsManager.RegisteredSystems )
            {
                system.PerformTask();
            }
        }

        delegate void PerformTaskDelegate();
        public void CallAllSystemTasksAsynchronous()
        {
            ISystem[] systems = this.SystemsManager.RegisteredSystems.ToArray<ISystem>();
            IAsyncResult[] results = new IAsyncResult[ systems.Count<ISystem>() ];
            PerformTaskDelegate[] delegates = new PerformTaskDelegate[ systems.Count<ISystem>() ];

            for ( int i = 0; i < systems.Count<ISystem>(); i++ )
            {
                delegates[ i ] = systems[ i ].PerformTask;
                results[ i ] = delegates[ i ].BeginInvoke( null, null );
            }

            for ( int i = 0; i < delegates.Length; i++ )
            {
                delegates[i].EndInvoke( results[ i ] );
            }
        }

        public void CallSystemTasksAsynchronous(IEnumerable<string> systemNames )
        {
            if ( systemNames == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "systemNames" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            int length = systemNames.Count<string>();
            IAsyncResult[] results = new IAsyncResult[ length ];
            PerformTaskDelegate[] delegates = new PerformTaskDelegate[ length ];            

            for ( int i = 0; i < length; i++ )
            {
                ISystem system = this.SystemsManager.GetSystem( systemNames.ElementAt<string>( i ) );
                
                delegates[ i ] = system.PerformTask;
                results[ i ] = delegates[ i ].BeginInvoke( null, null );
            }

            for ( int i = 0; i < delegates.Length; i++ )
            {
                delegates[ i ].EndInvoke( results[ i ] );
            }
        }
    }
}
