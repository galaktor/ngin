/**************************************
 * FILE:          TestSystem.cs
 * DATE:          05.01.2010 10:27:03
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
using NGin.Core.Configuration;
using NGin.Core.Logging;
using NGin.Core.Systems;
using System;
using System.Threading;

namespace NGin.Core.Test.Performance
{
    [System( typeof( TestSystem ), null )]
    public class TestSystem : NGinSystem
    {
        public TestSystem( ILogManager logManager )
            : base( TimeSpan.FromSeconds( 1 / 50.0 ), logManager )
        { }        

        public override string Name
        {
            get { return "TestSystem"; }
        }

        public override void Update()
        {
            Thread.Sleep( TimeSpan.FromMilliseconds( 10 ) );
            //this.LogManager.Trace( NGin.Core.Test.Performance.Namespace.LoggerName, LogLevel.Information, this.GetType().FullName + ": Performing task...Time since last: {0} MaximumRate: {1}", this.TimeSinceLastTask.TotalMilliseconds, this.MaximumRate.TotalMilliseconds );
            //System.Console.WriteLine("TestSystsem: TASK");
        }
    }

    [System( typeof( TestSystem2 ), null )]
    public class TestSystem2 : NGinSystem
    {
        public TestSystem2( ILogManager logManager )
            : base( TimeSpan.Zero, logManager )
        { }

        public override string Name
        {
            get { return "TestSystem2"; }
        }

        public override void Update()
        {
            Thread.Sleep( 760 );            
            //this.LogManager.Trace( NGin.Core.Test.Performance.Namespace.LoggerName, LogLevel.Information, this.GetType().FullName + ": Performing task...Time since last: {0} MaximumRate: {1}", this.TimeSinceLastTask.TotalMilliseconds, this.MaximumRate.TotalMilliseconds );
            //System.Console.WriteLine("TestSystsem: TASK");
        }
    }
}
