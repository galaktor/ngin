/**************************************
 * FILE:          Log4NetControllerTest.cs
 * DATE:          05.01.2010 10:23:22
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
using System.Text;
using NUnit.Framework;
using NGin.Core.Logging;
using System.IO;
using System.Xml;
using System.Threading;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class Log4NetControllerTest
    {
        private string loggerName = "NGin.Core.Test";
        private XmlElement configXml;

        [TestFixtureSetUp]
        public void SetUp()
        {
            XmlDocument configDoc = new XmlDocument();
            configDoc.Load( "Log4NetControllerTest_config.xml" );
            this.configXml = configDoc.DocumentElement;
        }

        [SetUp]
        public void SetUpTest()
        {
            // delete log file if exists
            if ( File.Exists( "NGin.Core.Test.log" ))
            {
                File.Delete( "NGin.Core.Test.log" );
            }
        }

        [Test]
        public void Log_AllLevels_FileCreated()
        {
            // arrange
            Log4NetLogController controller = new Log4NetLogController( configXml );
            FileInfo resultFile;

            // act
            controller.Trace( this.loggerName, LogLevel.Debugging, "Debugging!" );
            controller.Trace( this.loggerName, LogLevel.Error, "Error!" );
            controller.Trace( this.loggerName, LogLevel.Fatal, "Fatal!" );
            controller.Trace( this.loggerName, LogLevel.Information, "Information!" );
            controller.Trace( this.loggerName, LogLevel.Warning, "Warning!" );
            controller.Shutdown();

            resultFile = new FileInfo("NGin.Core.Test.log");

            // assert
            Assert.True(resultFile.Exists);
            StreamReader reader = new StreamReader("NGin.Core.Test.log");
            string content = reader.ReadToEnd();
            Assert.IsTrue(content.Contains("DEBUG"));
            Assert.IsTrue(content.Contains("WARN"));
            Assert.IsTrue(content.Contains("INFO"));
            Assert.IsTrue(content.Contains("ERROR"));
            Assert.IsTrue(content.Contains("FATAL"));
            reader.Close();
        }

        [Test]
        public void Log_AllLevelsException_FileCreated()
        {
            // arrange
            Log4NetLogController controller = new Log4NetLogController( configXml );
            FileInfo resultFile;
            Exception exception = new Exception( "This is just a test! No exception has been thrown." );

            // act
            controller.Trace( this.loggerName, LogLevel.Debugging, "Debugging!", exception );
            controller.Trace( this.loggerName, LogLevel.Error, "Error!", exception );
            controller.Trace( this.loggerName, LogLevel.Fatal, "Fatal!", exception );
            controller.Trace( this.loggerName, LogLevel.Information, "Information!", exception );
            controller.Trace( this.loggerName, LogLevel.Warning, "Warning!", exception );
            controller.Shutdown();

            resultFile = new FileInfo( "NGin.Core.Test.log" );

            // assert
            Assert.True( resultFile.Exists );
            StreamReader reader = new StreamReader( "NGin.Core.Test.log" );
            string content = reader.ReadToEnd();
            Assert.IsTrue( content.Contains( "DEBUG" ) );
            Assert.IsTrue( content.Contains( "WARN" ) );
            Assert.IsTrue( content.Contains( "INFO" ) );
            Assert.IsTrue( content.Contains( "ERROR" ) );
            Assert.IsTrue( content.Contains( "FATAL" ) );
            reader.Close();
        }

        [Test]
        public void Log_AllLevelsSeveralThreads_FileCreated()
        {
            // arrange
            Log4NetLogController controller = new Log4NetLogController( configXml );
            FileInfo resultFile;
            Thread tOne = new Thread( delegate( object param ) { Thread.Sleep( 150 ); ( ( ILogController ) param ).Trace( this.loggerName, LogLevel.Debugging, "Debugging!" ); Thread.Sleep( 150 ); ( ( ILogController ) param ).Trace( this.loggerName, LogLevel.Information, "Information!" ); } );
            Thread tTwo = new Thread( delegate( object param ) { Thread.Sleep( 150 ); ( ( ILogController ) param ).Trace( this.loggerName, LogLevel.Error, "Error!" ); Thread.Sleep( 150 ); ( ( ILogController ) param ).Trace( this.loggerName, LogLevel.Warning, "Warning!" ); } );
            Thread tThree = new Thread( delegate( object param ) { Thread.Sleep( 150 ); ( ( ILogController ) param ).Trace( this.loggerName, LogLevel.Fatal, "Fatal!" ); } );

            // act
            tOne.Start( controller );
            tTwo.Start( controller );
            tThree.Start( controller );
            tOne.Join();
            tTwo.Join();
            tThree.Join();
            controller.Shutdown();

            resultFile = new FileInfo( "NGin.Core.Test.log" );

            // assert
            Assert.True( resultFile.Exists );
            StreamReader reader = new StreamReader( "NGin.Core.Test.log" );
            string content = reader.ReadToEnd();
            Assert.IsTrue( content.Contains( "DEBUG" ) );
            Assert.IsTrue( content.Contains( "WARN" ) );
            Assert.IsTrue( content.Contains( "INFO" ) );
            Assert.IsTrue( content.Contains( "ERROR" ) );
            Assert.IsTrue( content.Contains( "FATAL" ) );
            reader.Close();
        }
    }
}
