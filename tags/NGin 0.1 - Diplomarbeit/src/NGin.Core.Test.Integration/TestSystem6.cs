/**************************************
 * FILE:          TestSystem6.cs
 * DATE:          05.01.2010 10:26:35
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

namespace NGin.Core.Test.Integration
{
    [System( typeof( TestSystem6 ), null )]
    public class TestSystem6 : NGinSystem
    {
        public TestSystem6( ILogManager logManager )
            : base( TimeSpan.FromSeconds(1/65.0f), logManager )
        { }

        public override string Name
        {
            get { return "TestSystem6"; }
        }

        public override void Update()
        {
            this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, this.GetType().FullName + ": Performing task...Time since last: {0} MaximumRate: {1}", this.TimeSinceLastTask.TotalMilliseconds, this.MaximumRate.TotalMilliseconds );
        }
    }
}
