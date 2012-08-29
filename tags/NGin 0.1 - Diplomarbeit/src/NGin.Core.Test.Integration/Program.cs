/**************************************
 * FILE:          Program.cs
 * DATE:          05.01.2010 10:25:15
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
using System.Threading;
using NGin.Core.Logging;
using NGin.Core.Scene;
using System.IO;
using NGin.Core.Messaging;

namespace NGin.Core.Test.Integration
{
    public class Program
    {        
        static void Main( string[] args )
        {
            if ( File.Exists( "debug.flg" ) )
            {
                Console.WriteLine("Attach remote debugger now. Press ENTER to continue.");
                Console.ReadLine();
            }

            // create core using config file
            INGinCore core = new NGinCore("ngin.config");

            ILogManager logManager = core.GetService<ILogManager>();   

            core.Run();
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Loop started." );

            IMessageManager messageManager = core.GetService<IMessageManager>();

            messageManager.RegisterMessageType( "End", ( s, a ) => 
            {
                core.Stop();
                logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Loop stopped." );
            } );

            core.WaitForEnd();

            core.Run();
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Loop started." );

            core.WaitForEnd();

            Console.WriteLine( "Press ENTER to exit." );
            Console.ReadLine();
        }
    }
}