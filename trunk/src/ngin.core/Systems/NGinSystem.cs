/**************************************
 * FILE:          NGinSystem.cs
 * DATE:          05.01.2010 10:19:30
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
using NGin.Core.Logging;
using System.Diagnostics;
using System;

namespace NGin.Core.Systems
{
    //[NGinSystem]
    public interface ISystem: IDisposable
    {
        event TaskStateChangedDelegate TaskStarted;
        event TaskStateChangedDelegate TaskEnded;
        void PerformTask();
        string Name { get; }
        TimeSpan TimeSinceLastTask { get; }
        TimeSpan MaximumRate { get; }
        TimeSpan TargetRate { get; }
    }
    public abstract class NGinSystem: ISystem
    {
        public bool IsDisposing
        {
            get;
            private set;
        }

        ~NGinSystem()
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


        private int AvgDistanceToMaxRateBufferLimit = 2;

        private object AvgDistanceToMaxRateBufferCountLock = new object();
        private int AvgDistanceToMaxRateBufferCount = 0;

        private object AvgDistanceToMaxRateBufferLock = new object();
        private TimeSpan AvgDistanceToMaxRateBuffer = TimeSpan.Zero;

        private object AvgDistanceToMaxRateLock = new object();
        internal TimeSpan AvgDistanceToMaxRate = TimeSpan.Zero;

        private object timeSinceLastTaskLock = new object();
        private TimeSpan timeSinceLastTask;
        public TimeSpan TimeSinceLastTask
        {
            get
            {
                lock ( this.timeSinceLastTaskLock )
                {
                    return timeSinceLastTask;
                }
            }
            private set
            {
                lock ( this.timeSinceLastTaskLock )
                {
                    timeSinceLastTask = value;
                }
            }
        }

        private object taskTimeStopwatchLock = new object();
        Stopwatch taskTimeStopwatch = new Stopwatch();

        public TimeSpan TargetRate 
        {
            get
            {
                lock ( this.AvgDistanceToMaxRateLock )
                {
                    return this.MaximumRate - this.AvgDistanceToMaxRate;
                }
            }
        }
        public TimeSpan MaximumRate { get; internal set; }      

        protected internal ILogManager LogManager { get; internal set; }
        
        public NGinSystem( TimeSpan maximumRate, ILogManager logManager)
        {
            this.LogManager = logManager;            
            this.MaximumRate = maximumRate;

            this.taskTimeStopwatch.Start();
        }

        public abstract string Name { get; }          

        // event lock object
        private object taskStartedEventLock = new object();
        private TaskStateChangedDelegate taskStarted;
        public event TaskStateChangedDelegate TaskStarted
        {
            add
            {
                lock ( this.taskStartedEventLock )
                {
                    taskStarted += value;
                }
            }

            remove
            {
                lock ( this.taskStartedEventLock )
                {
                    taskStarted -= value;
                }
            }
        }

        // event lock object
        private object taskEndedEventLock = new object();
        private TaskStateChangedDelegate taskEnded;
        public event TaskStateChangedDelegate TaskEnded
        {
            add
            {
                lock ( this.taskEndedEventLock )
                {
                    taskEnded += value;
                }
            }

            remove
            {
                lock ( this.taskEndedEventLock )
                {
                    taskEnded -= value;
                }
            }
        }

        // evtl: TASK-ATTRIBUTE: min/max time?
        
        internal void FireTaskStartedEvent()
        {
            lock ( this.taskStartedEventLock )
            {
                if ( this.taskStarted != null )
                {
                    // synchrounous is still by far the fastest
                    this.taskStarted( this );

                    #region Asynchrounous Experimental
                    //Delegate[] delegates = this.taskStarted.GetInvocationList();
                    //IAsyncResult[] results = new IAsyncResult[ delegates.Count<Delegate>() ];

                    //for ( int i = 0; i < delegates.Count<Delegate>(); i++ )
                    //{
                    //    IAsyncResult result = ( ( TaskStateChangedDelegate ) delegates[ i ] ).BeginInvoke( this, null, null );
                    //    results[ i ] = result;
                    //}

                    //for ( int i = 0; i < delegates.Length; i++ )
                    //{
                    //    ( ( TaskStateChangedDelegate ) delegates[ i ] ).EndInvoke( results[ i ] );
                    //} 
                    #endregion
                }
            }
        }

        internal void FireTaskEndedEvent()
        {
            lock ( this.taskEndedEventLock )
            {
                if ( this.taskEnded != null )
                {
                    // synchrounous is still by far the fastest
                    this.taskEnded( this );

                    #region Asynchronous Experimental
                    //Delegate[] delegates = this.taskEnded.GetInvocationList();
                    //IAsyncResult[] results = new IAsyncResult[ delegates.Count<Delegate>() ];

                    //for ( int i = 0; i < delegates.Count<Delegate>(); i++ )
                    //{
                    //    IAsyncResult result = ( ( TaskStateChangedDelegate ) delegates[ i ] ).BeginInvoke( this, null, null );
                    //    results[ i ] = result;
                    //}

                    //for ( int i = 0; i < delegates.Length; i++ )
                    //{
                    //    ( ( TaskStateChangedDelegate ) delegates[ i ] ).EndInvoke( results[ i ] );
                    //} 
                    #endregion
                }
            }
        }

        public void PerformTask()
        {
            // TODO: IF core is running slow!!
            TimeSpan targetRate = this.TargetRate;
            if ( this.taskTimeStopwatch.Elapsed >= targetRate )
            {
                // fire task started
                this.FireTaskStartedEvent();

                this.Update();

                TimeSpan elapsed;
                lock ( this.taskTimeStopwatchLock )
                {
                    elapsed = this.taskTimeStopwatch.Elapsed;
                }
                if ( elapsed - this.MaximumRate <= this.MaximumRate )
                {
                    TimeSpan avgDist;
                    lock ( this.AvgDistanceToMaxRateLock )
                    {
                        avgDist = this.AvgDistanceToMaxRate;
                    }
                    lock ( this.AvgDistanceToMaxRateBufferLock )
                    {
                        this.AvgDistanceToMaxRateBuffer += elapsed - this.MaximumRate + avgDist;
                    }
                }
                else
                {
                    lock ( this.AvgDistanceToMaxRateBufferLock )
                    {
                        this.AvgDistanceToMaxRateBuffer += TimeSpan.Zero;
                    }
                }
                lock ( this.AvgDistanceToMaxRateBufferCountLock )
                {
                    this.AvgDistanceToMaxRateBufferCount++;
                }

                //#region DELETE ME
                //Console.WriteLine( "System {0}: MaxRate: {1} ms AvgDistance: {2} ms TargetRate: {3} ms", this.Name, this.MaximumRate.TotalMilliseconds, this.AvgDistanceToMaxRate.TotalMilliseconds, this.TargetRate.TotalMilliseconds );
                //#endregion

                // fire task ended
                this.FireTaskEndedEvent();

                lock ( this.taskTimeStopwatchLock )
                {
                    elapsed = this.taskTimeStopwatch.Elapsed;
                }
                lock ( this.timeSinceLastTaskLock )
                {
                    this.TimeSinceLastTask = elapsed;
                }

                //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "System: {0} TPS: {1} MaxTPS: {2}", this.Name, ( 1000 / this.TimeSinceLastTask.TotalMilliseconds ), ( 1000 / this.MaximumRate.TotalMilliseconds ) );

                lock ( this.taskTimeStopwatchLock )
                {
                    this.taskTimeStopwatch.Reset();
                    this.taskTimeStopwatch.Start();
                }
            }            

            // TODO: prevent TargetRate from getting to fast!
            int bufferCount;
            lock ( this.AvgDistanceToMaxRateBufferCountLock )
            {
                bufferCount = this.AvgDistanceToMaxRateBufferCount;
            }
            if ( bufferCount >= this.AvgDistanceToMaxRateBufferLimit )
            {
                long avgDistTicks;
                lock ( this.AvgDistanceToMaxRateBufferLock )
                {
                    avgDistTicks = this.AvgDistanceToMaxRateBuffer.Ticks;
                }
                TimeSpan newAvgDist;
                lock(this.AvgDistanceToMaxRateBufferCountLock)
                {
                    newAvgDist = TimeSpan.FromTicks( avgDistTicks / this.AvgDistanceToMaxRateBufferCount );
                }
                lock(this.AvgDistanceToMaxRateLock)
                {
                    this.AvgDistanceToMaxRate = newAvgDist;
                }
                lock ( this.AvgDistanceToMaxRateBufferCountLock )
                {
                    this.AvgDistanceToMaxRateBufferCount = 0;
                }
                lock ( this.AvgDistanceToMaxRateBufferLock )
                {
                    this.AvgDistanceToMaxRateBuffer = TimeSpan.Zero;
                }
            }

        }

        public virtual void Update()
        {
            // user implement logic
        }
    }
}
