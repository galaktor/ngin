/**************************************
 * FILE:          PerformanceTestRunner.cs
 * DATE:          05.01.2010 10:16:36
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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Microsoft.Win32;

using NGin.Core.Logging;
using NGin.Core.Systems;
using NGin.Core.Tasks;
using NGin.Core.Configuration;

namespace NGin.Core.Performance
{
    [Service(typeof(PerformanceTestRunner), typeof(IPerformanceTestRunner), false)]
    internal class PerformanceTestRunner : NGin.Core.Performance.IPerformanceTestRunner
    {
        internal INGinCore Core { get; private set; }
        public PerformanceTestRunner( INGinCore core, ILogManager logManager, ISystemsManager systemsManager )
        {
            this.Core = core;
            this.LogManager = logManager;
            this.SystemsManager = systemsManager;
        }

        #region Fields

        #region Private Fields

        private long frequency;
        
        private Stopwatch mainTimer = new Stopwatch();
        private Decimal multiplier = new Decimal( 1.0e9 );
        private Dictionary<ISystem, Dictionary<int, TimeSpan>> results = new Dictionary<ISystem, Dictionary<int, TimeSpan>>();
        private List<TestRecord> startRecords = new List<TestRecord>();
        private object startRecordsLock = new object();
        private List<TestRecord> stopRecords = new List<TestRecord>();
        private object stopRecordsLock = new object();
        private int taskCounter = 0;
        private object taskCounterLock = new object();

        private bool completed = false;

        #endregion Private Fields

        #endregion Fields        

        #region Properties

        #region Private Properties

        //public  ITaskManager TaskManager { get; set; }
        private ILogManager LogManager
        {
            get; set;
        }

        private ISystemsManager SystemsManager
        {
            get; set;
        }

        private int TaskCounter
        {
            get
            {
                lock ( taskCounterLock )
                {
                    return taskCounter;
                }
            }
            set
            {
                lock ( taskCounterLock )
                {
                    taskCounter = value;
                    FireTaskCounterChanged( taskCounter, mainTimer.Elapsed, TestDuration );
                }
            }
        }

        public bool BeepWhenFinished { get; set; }
        public bool PrintProgressInfo { get; set; }

        #endregion Private Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        private TimeSpan TestDuration;

        public string Test( TimeSpan testDuration, bool printProgressInfo, bool beepWhenFinished )
        {
            this.PrintProgressInfo = printProgressInfo;
            this.BeepWhenFinished = beepWhenFinished;
            this.TestDuration = testDuration;

            if ( this.PrintProgressInfo )
            {
                Console.CursorVisible = false;
            }

            this.PrintMachineData();

            #region Get internal clock frequency
            if ( QueryPerformanceFrequency( out frequency ) == false )
            {
                // Frequency not supported
                throw new InvalidOperationException();
            }
            #endregion

            #region Set up measurement of system tasks using TaskStarted and TaskEnded events
            foreach ( ISystem system in SystemsManager.RegisteredSystems )
            {
                system.TaskStarted += x =>
                {
                    int id = Thread.CurrentThread.ManagedThreadId;

                    long time;
                    QueryPerformanceCounter( out time );
                    lock ( startRecordsLock )
                    {

                        startRecords.Add( new TestRecord( TestMode.Start, x, id, time ) );
                    }
                };
                system.TaskEnded += x =>
                {
                    int id = Thread.CurrentThread.ManagedThreadId;

                    long time;
                    QueryPerformanceCounter( out time );
                    lock ( stopRecordsLock )
                    {
                        stopRecords.Add( new TestRecord( TestMode.Stop, x, id, time ) );
                    }
                    TaskCounter++;
                };
            }
            #endregion

            #region Run tasks and measure time
            mainTimer.Start();       
     
            // run core for defined time then stop
            this.Core.Run();
            Thread.Sleep( testDuration );
            this.Core.Stop();

            mainTimer.Stop();
            #endregion

            #region Prepare results
            var startSystemsDump = from record in startRecords
                                   select record.System;

            var stopSystemsDump = from record in stopRecords
                                  select record.System;

            var allSystemsDump = startSystemsDump.Concat<ISystem>( stopSystemsDump );

            var distictSystems = allSystemsDump.Distinct<ISystem>();

            var startThreadIdDump = from record in startRecords
                                    select record.ThreadId;

            var stopThreadIdDump = from record in stopRecords
                                   select record.ThreadId;

            var allThreadIdsDump = startThreadIdDump.Concat<int>( stopThreadIdDump );

            var distictThreadIds = allThreadIdsDump.Distinct<int>();

            Dictionary<ISystem, int> tasksRunPerSystem = new Dictionary<ISystem, int>();
            foreach(ISystem startedSystem in startSystemsDump)
            {
                int count;
                if ( !tasksRunPerSystem.TryGetValue( startedSystem, out count ) )
                {
                    count = 0;
                }

                tasksRunPerSystem[ startedSystem ] = count + 1;
            }

            // initialize results
            foreach ( ISystem system in distictSystems )
            {
                Dictionary<int, TimeSpan> threadData = new Dictionary<int, TimeSpan>();
                foreach ( int threadId in distictThreadIds )
                {
                    threadData.Add( threadId, TimeSpan.Zero );
                }
                results.Add( system, threadData );
            }
            #endregion

            #region Collect results
            foreach ( TestRecord record in startRecords )
            {
                TestRecord match = null;
                for ( int i = 0; i < stopRecords.Count; i++ )
                {
                    TestRecord stopRecord = stopRecords[ i ];
                    if ( stopRecord.TestMode == TestMode.Stop &&
                        stopRecord.System == record.System &&
                        stopRecord.ThreadId == record.ThreadId &&
                        stopRecord.Time > record.Time )
                    {
                        match = stopRecord;
                        stopRecords.Remove( stopRecord );
                        break;
                    }
                }

                double durationTicks = ( double ) ( match.Time - record.Time ) * ( double ) multiplier / ( double ) frequency;
                TimeSpan duration = TimeSpan.FromMilliseconds( durationTicks / 1000000 );

                TimeSpan total = results[ record.System ][ record.ThreadId ].Add( duration );
                results[ record.System ][ record.ThreadId ] = total;
            }
            #endregion

            #region Print results
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( "***** Results: *****" );            

            Dictionary<int,double> threadLoadRecord = new Dictionary<int,double>();

            sb.AppendLine();
            sb.AppendLine( "*************** SYSTEMS ***************" );
            double totalCpuTime = 0;
            foreach ( KeyValuePair<ISystem, Dictionary<int, TimeSpan>> kvp in results )
            {
                sb.AppendLine( "--------" );
                sb.AppendFormat( "> {0}", kvp.Key.Name ).AppendLine();
                double systemCpuTime = 0;
                foreach ( KeyValuePair<int, TimeSpan> threadData in kvp.Value )
                {
                    sb.AppendFormat( "Thread {0} duration: {1} ms", threadData.Key, threadData.Value.TotalMilliseconds ).AppendLine();
                    double sum;
                    threadLoadRecord.TryGetValue(threadData.Key, out sum);
                    threadLoadRecord[threadData.Key] = sum + threadData.Value.TotalMilliseconds;
                    systemCpuTime += threadData.Value.TotalMilliseconds;
                }
                int systemTaskCount = tasksRunPerSystem[ kvp.Key ];
                sb.AppendFormat( "System tasks run: {0}", systemTaskCount).AppendLine();
                sb.AppendFormat( "Average task duration: {0} ms", systemCpuTime / systemTaskCount ).AppendLine();
                sb.AppendFormat( "Tasks per second: {0} TPS", systemTaskCount / mainTimer.Elapsed.TotalSeconds ).AppendLine();
                sb.AppendFormat( "Defined MaximumRate: {0} TPS ({1} ms)",  1000 / kvp.Key.MaximumRate.TotalMilliseconds, kvp.Key.MaximumRate.TotalMilliseconds ).AppendLine();
                sb.AppendFormat( "Last calculated TargetRate: {0} TPS ({1} ms)", 1000 / kvp.Key.TargetRate.TotalMilliseconds, kvp.Key.TargetRate.TotalMilliseconds ).AppendLine();
                totalCpuTime += systemCpuTime;
                sb.AppendFormat( "Total CPU time: {0} ms", systemCpuTime ).AppendLine();
            }
            sb.AppendLine( "--------" );
            sb.AppendLine( "> TOTAL" );
            sb.AppendFormat( "Tasks run: {0}", this.TaskCounter ).AppendLine();
            sb.AppendFormat( "Tasks per second: {0} TPS", this.TaskCounter / mainTimer.Elapsed.TotalSeconds ).AppendLine();
            sb.AppendFormat( "Threads used: {0} IDs: ", distictThreadIds.Count<int>() );
            for ( int i = 0; i < distictThreadIds.Count<int>(); i++ )
            {
                sb.Append( distictThreadIds.ElementAt<int>( i ) );
                if ( i < distictThreadIds.Count<int>() - 1 )
                {
                    sb.Append( ", " );
                }
            }
            sb.AppendLine();
            sb.AppendFormat( "Total real time: {0} ms", this.mainTimer.Elapsed.TotalMilliseconds ).AppendLine();
            sb.AppendFormat( "Total CPU time (tasks): {0} ms", totalCpuTime ).AppendLine();
            //double maxAvailableCpuTime = mainTimer.Elapsed.TotalMilliseconds * Environment.ProcessorCount;
            //sb.AppendFormat( "MACT (Maximal available CPU time) (elapsed total time {0} ms X {1} threads) = {2} ms", mainTimer.ElapsedMilliseconds, distictThreadIds.Count<int>(),  maxAvailableCpuTime ).AppendLine();
            double cpuLoad = totalCpuTime / mainTimer.Elapsed.TotalMilliseconds;
            sb.AppendFormat( "CPU load factor (total cpu ms / total real ms): {0}", Math.Round( cpuLoad, 5 ) ).AppendLine();

            sb.AppendLine();
            sb.AppendLine( "************* THREAD LOAD *************" );
            
            foreach ( int threadId in distictThreadIds )
            {
                sb.AppendLine( "--------" );
                sb.AppendFormat( "> Thread: {0}", threadId ).AppendLine();
                double threadTotalCpuTime = threadLoadRecord[threadId];
                sb.AppendFormat("CPU time: {0} ms", threadTotalCpuTime ).AppendLine();
                sb.AppendFormat( "Percentage to total CPU time: {0}", Math.Round( ( threadTotalCpuTime * 100 ) / totalCpuTime, 2 ) ).AppendLine();
            }
            

            LogManager.Trace( Namespace.LoggerName, LogLevel.Information, sb.ToString() );
            #endregion

            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Static Methods

        [DllImport( "KERNEL32" )]
        private static extern bool QueryPerformanceCounter( out long lpPerformanceCount );

        [DllImport( "Kernel32.dll" )]
        private static extern bool QueryPerformanceFrequency( out long lpFrequency );

        #endregion Private Static Methods

        #region Private Methods

        private void FireTaskCounterChanged( int taskCount, TimeSpan elapsed, TimeSpan total  )
        {
            if ( this.PrintProgressInfo )
            {
                if ( !completed )
                {
                    string symbol = string.Empty;
                    switch ( taskCount % 4 )
                    {
                        case 0: symbol = "/"; break;
                        case 1: symbol = "-"; break;
                        case 2: symbol = "\\"; break;
                        case 3: symbol = "|"; break;
                    }                    
                    Console.SetCursorPosition( 0, Console.CursorTop - 2 );
                    double progress = ( elapsed.TotalMilliseconds * 100 ) / total.TotalMilliseconds;
                    progress = ( progress > 100.0 ) ? 100.0 : progress;
                    Console.WriteLine( "{0}\t{1}% completed   \t({2} tasks processed)             ", symbol, Math.Round( progress, 2 ), taskCount );

                    if ( elapsed >= total )
                    {
                        completed = true;
                        Console.SetCursorPosition( Console.CursorLeft - Console.CursorLeft, Console.CursorTop - 1 );
                        for ( int i = 0; i < Console.BufferWidth; i++ )
                        {
                            Console.Write( " " );
                        }
                        for ( int i = 0; i < Console.BufferWidth; i++ )
                        {
                            Console.Write( " " );
                        }

                        if ( this.BeepWhenFinished )
                        {
                            Console.Beep();
                        }
                    }
                    else
                    {
                        TimeSpan timeRemaining = total - elapsed;
                        Console.WriteLine( "\tTime remaining: {0}            ", timeRemaining.ToString() );
                    }
                }
                else
                {
                    Console.SetCursorPosition( Console.CursorLeft - Console.CursorLeft, Console.CursorTop - 2 );
                    Console.WriteLine( "                                                                                          " );
                    Console.WriteLine( "                                                                                          " );
                } 
            }
        }

        private void PrintMachineData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "Machine name.....{0}", Environment.MachineName ).AppendLine();
            sb.AppendFormat( "OS:..............{0}", Environment.OSVersion.ToString() ).AppendLine();
            sb.AppendFormat( "CLR:.............{0}", Environment.Version.ToString() ).AppendLine();
            #region Get systemCpuTime memory - not really required, removed since needs reference to 'Microsoft.VisualBasic.dll'
            //Microsoft.VisualBasic.Devices.ComputerInfo c = new Microsoft.VisualBasic.Devices.ComputerInfo();
            //sb.AppendFormat( "Memory:..........{0} MB", c.TotalPhysicalMemory / 1024 / 1024 ).AppendLine(); 
            #endregion
            RegistryKey RegKey = Registry.LocalMachine;
            RegKey = RegKey.OpenSubKey( "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0" );
            Object cpuSpeed = RegKey.GetValue( "~MHz" );
            Object cpuType = RegKey.GetValue( "ProcessorNameString" );
            sb.AppendFormat( "CPU:.............{0} ({1}) @ {2} MHz", cpuType, System.Environment.GetEnvironmentVariable( "PROCESSOR_ARCHITECTURE" ), cpuSpeed ).AppendLine();
            sb.AppendFormat( "CPU count:.......{0}", Environment.ProcessorCount ).AppendLine();
            LogManager.Trace( Namespace.LoggerName, LogLevel.Information, sb.ToString() );
            Console.WriteLine();
        }

        #endregion Private Methods

        #endregion Methods
    }
}