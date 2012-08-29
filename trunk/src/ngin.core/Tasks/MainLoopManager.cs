/**************************************
 * FILE:          MainLoopManager.cs
 * DATE:          05.01.2010 10:20:05
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
using System.Diagnostics;
using System.Threading;
using NGin.Core.Logging;
using NGin.Core.Configuration;
using NGin.Core.States;
using NGin.Core.Scene;
using NGin.Core.States.RHFSM;

namespace NGin.Core.Tasks
{
    [Service( typeof( MainLoopManager ), typeof( IMainLoopManager ), true )]
    internal class MainLoopManager: IMainLoopManager
    {
        private object timerLock = new object();
        private Stopwatch timer = new Stopwatch();
        private Stopwatch avgUpdateTime = new Stopwatch();

        internal TimeSpan durationSinceLastAvgUpdate;
        internal TimeSpan durationLastHeartbeat;
        internal TimeSpan durationAverageHeartbeat;
        // buffer for average values
        private long[] heartbeatBuffer;
        private int currentBufferIndex;
        private int bufferMaximum = 25;

        private object runningLock = new object();
        private volatile bool running = false;
        internal bool Running
        {
            get
            {
                lock ( this.runningLock )
                {
                    return this.running;
                }
            }
            set
            {
                lock ( this.runningLock )
                {
                    this.running = value;
                }
            }
        }

        private object heartbeatStartedEventLock = new object();
        private event HeartbeatDelegate heartbeatStarted;
        public event HeartbeatDelegate HeartbeatStarted
        {
            add
            {
                lock ( this.heartbeatStartedEventLock )
                {
                    this.heartbeatStarted += value;
                }
            }
            remove
            {
                lock ( this.heartbeatStartedEventLock )
                {
                    this.heartbeatStarted -= value;
                }
            }
        }

        private object heartbeatEndedEventLock = new object();
        private event HeartbeatDelegate heartbeatEnded;
        public event HeartbeatDelegate HeartbeatEnded
        {
            add
            {
                lock ( this.heartbeatEndedEventLock )
                {
                    this.heartbeatEnded += value;
                }
            }
            remove
            {
                lock ( this.heartbeatEndedEventLock )
                {
                    this.heartbeatEnded -= value;
                }
            }
        }
        private object loopEndedEventLock = new object();
        private event LoopStateDelegate loopEnded;
        public event LoopStateDelegate LoopEnded
        {
            add
            {
                lock ( this.loopEndedEventLock )
                {
                    this.loopEnded += value;
                }
            }
            remove
            {
                lock ( this.loopEndedEventLock )
                {
                    this.loopEnded -= value;
                }
            }
        }

        private object loopStartedEventLock = new object();
        private event LoopStateDelegate loopStarted;
        public event LoopStateDelegate LoopStarted
        {
            add
            {
                lock ( this.loopStartedEventLock )
                {
                    this.loopStarted += value;
                }
            }
            remove
            {
                lock ( this.loopStartedEventLock )
                {
                    this.loopStarted -= value;
                }
            }
        }

        internal Thread loopThread;

        internal ILogManager LogManager { get; set; }
        internal IStateManager StateManager { get; set; }

        public MainLoopManager( IStateManager stateManager, ILogManager logManager )
        {
            if(logManager == null)
            {
                throw new ArgumentNullException( "logManager" );
            }

            if ( stateManager == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "stateManager" );
                logManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            this.LogManager = logManager;
            this.StateManager = stateManager;

            this.heartbeatBuffer = new long[ this.bufferMaximum + 1 ];
            this.currentBufferIndex = 0;

            this.HeartbeatEnded += new HeartbeatDelegate( this.MainLoopManager_HeartbeatEnded );
        }

        internal void MainLoopManager_HeartbeatEnded( TimeSpan timeSinceLast )
        {
            IState currentState = this.StateManager.GetCurrentState();
            currentState.Update();         
        }

        internal void FireLoopStartedEvent()
        {
            lock ( this.loopStartedEventLock )
            {
                if ( this.loopStarted != null )
                {
                    this.loopStarted( this );
                }
            }
        }

        internal void FireLoopEndedEvent()
        {
            lock ( this.loopEndedEventLock )
            {
                if ( this.loopEnded != null )
                {
                    this.loopEnded( this );
                }
            }
        }

        public void RunAsynchronous()
        {
            if ( this.Running )
            {
                string message = "The loop is alreay running and thus cannot be started again.";
                InvalidOperationException invEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, invEx, message );
                throw invEx;
            }

            // initialize loop thread
            this.loopThread = new Thread( this.RunLoop );
            this.loopThread.Name = "Main loop";
            
            this.Running = true;

            // start loop
            this.loopThread.Start();

            this.FireLoopStartedEvent();
        }

        public void WaitForLoopThreadEnd()
        {
            if ( Thread.CurrentThread != loopThread )
            {
                this.loopThread.Join();
            }
        }

        public void WaitForLoopThreadEnd( TimeSpan timeout )
        {
            if ( Thread.CurrentThread != loopThread )
            {
                this.loopThread.Join( timeout );
            }     
        }
       
        public void Stop()
        {
            if ( !this.Running )
            {
                string message = "The loop is not running and thus cannot be stopped.";
                InvalidOperationException invEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, invEx, message );
                throw invEx;
            }

            this.Running = false;

            // wait for main loop to finish
            // IF this is not currently that loop
            if ( Thread.CurrentThread != loopThread )
            {
                this.loopThread.Join();
            }

            this.FireLoopEndedEvent();
        }

        internal void RunLoop()
        {
            while ( this.Running )
            {
                lock ( this.timerLock )
                {
                    timer.Reset();
                    timer.Start();

                    this.FireHeartbeatStartedEvent( this.durationLastHeartbeat );
                    this.FireHeartbeatEndedEvent( this.durationLastHeartbeat );

                    timer.Stop();
                    this.durationLastHeartbeat = timer.Elapsed;
                }

                #region Time updates
                if ( this.durationSinceLastAvgUpdate.TotalSeconds >= 1 )
                {
                    long sum = 0;
                    foreach ( long heartBeatDuration in this.heartbeatBuffer )
                    {
                        sum += heartBeatDuration;
                    }
                    this.durationAverageHeartbeat = TimeSpan.FromTicks( sum / this.bufferMaximum );

                    this.durationSinceLastAvgUpdate = TimeSpan.Zero;

                    #if DEBUG
                    this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "HEARTBEAT - Average HPS: {0}", (1000.0f / this.durationAverageHeartbeat.TotalMilliseconds) );
                    #endif

                    //Console.WriteLine( "HEARTBEAT - Average time (ms): {0}", this.durationAverageHeartbeat.TotalMilliseconds );
                }
                else
                {
                    this.durationSinceLastAvgUpdate = this.durationSinceLastAvgUpdate.Add( this.durationLastHeartbeat );
                }

                this.currentBufferIndex = ( this.currentBufferIndex >= this.bufferMaximum ) ? 0 : this.currentBufferIndex + 1;
                this.heartbeatBuffer[ this.currentBufferIndex ] = this.durationLastHeartbeat.Ticks;
                #endregion
            }
        }

        internal void FireHeartbeatStartedEvent( TimeSpan timeSinceLast )
        {
            lock ( this.heartbeatStartedEventLock )
            {
                if ( this.heartbeatStarted != null )
                {
                    this.heartbeatStarted( timeSinceLast );
                }
            }
        }

        internal void FireHeartbeatEndedEvent( TimeSpan timeSinceLast )
        {
            lock ( this.heartbeatEndedEventLock )
            {
                if ( this.heartbeatEnded != null )
                {
                    this.heartbeatEnded( timeSinceLast );
                }
            }
        }
    }
}
