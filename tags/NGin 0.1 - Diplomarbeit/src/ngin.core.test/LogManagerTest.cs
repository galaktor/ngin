/**************************************
 * FILE:          LogManagerTest.cs
 * DATE:          05.01.2010 10:23:28
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
using NMock2;
using NGin.Core.Logging;
using System.Xml;
using NGin.Core.Exceptions;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class LogManagerTest
    {
        private string loggerName = "NGin.Core.Test";
        Mockery mocks;

        [TestFixtureSetUp]
        public void SetUp()
        {
            this.mocks = new Mockery();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            this.mocks.VerifyAllExpectationsHaveBeenMet();
            this.mocks.Dispose();
            this.mocks = null;
        }

        [Test]
        public void LogManager_CreateInstance_Success()
        {
            // arrange
            ILogController logController = this.mocks.NewMock<ILogController>();            

            // act
            LogManager manager = new LogManager( logController );

            // assert
            Assert.IsNotNull( manager );
            Assert.IsInstanceOf<LogManager>( manager );
        }

        [Test, ExpectedException(typeof(ManagerInitializationException))]
        public void LogManager_LogControllerNull_RaiseManagerInitializationException()
        {
            // arrange
            ILogController controller = null;

            // act
            LogManager manager = new LogManager( controller );

            // assert
        }

        [Test]
        public void Trace_DebugLevel_LogControllerCalled()
        {
            // arrange
            string testMessage = "This is a Test.";
            object[] stringParameters = new object[] { };
            ILogController logController = this.mocks.NewMock<ILogController>();
            Expect.Once.On( logController ).Method( "Trace" ).With( this.loggerName, LogLevel.Debugging, testMessage, stringParameters );
            LogManager manager = new LogManager( logController );

            // act
            manager.Trace( this.loggerName, LogLevel.Debugging, testMessage, stringParameters );

            // assert
        }

        [Test]
        public void Trace_DebugLevelException_LogControllerCalled()
        {
            // arrange
            string testMessage = "This is a Test.";
            object[] stringParameters = new object[] { };
            ILogController logController = this.mocks.NewMock<ILogController>();
            Exception exception = new Exception( "This is just a test! No exception has been thrown." );
            Expect.Once.On( logController ).Method( "Trace" ).With( this.loggerName, LogLevel.Debugging, exception, testMessage, stringParameters );
            LogManager manager = new LogManager( logController );

            // act
            manager.Trace( this.loggerName, LogLevel.Debugging, exception, testMessage, stringParameters );

            // assert
        }
    }
}
