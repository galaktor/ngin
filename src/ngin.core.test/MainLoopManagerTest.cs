/**************************************
 * FILE:          MainLoopManagerTest.cs
 * DATE:          05.01.2010 10:23:34
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
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;
using NGin.Core.Tasks;
using NGin.Core.States;
using NGin.Core.Logging;

namespace NGin.Core.Test
{
    [TestFixture]
    public class MainLoopManagerTest
    {
        private Mockery mocks;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new Mockery();
        }

        [TearDown]
        public void TearDown()
        {
            this.mocks.VerifyAllExpectationsHaveBeenMet();
            this.mocks.Dispose();
            this.mocks = null;
        }

        [Test]
        public void MainLoopManager_ValidParams_CreateInstance()
        {
            // arrange
            MainLoopManager mlm;
            IStateManager sm = this.mocks.NewMock<IStateManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();

            // act
            mlm = new MainLoopManager( sm, lm );

            // assert
            Assert.IsNotNull( mlm );
            Assert.IsInstanceOf<MainLoopManager>( mlm );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void MainLoopManager_StateManagerNull_ThrowArgumentNullException()
        {
            // arrange
            MainLoopManager mlm;
            IStateManager sm = null;
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();

            // act
            mlm = new MainLoopManager( sm, lm );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void MainLoopManager_LogManagerNull_ThrowArgumentNullException()
        {
            // arrange
            MainLoopManager mlm;
            IStateManager sm = this.mocks.NewMock<IStateManager>();
            ILogManager lm = null;

            // act
            mlm = new MainLoopManager( sm, lm );

            // assert
        }

        /// <summary>
        /// Ignore any exceptions if this test passes - they are thrown because of the game loop thread and can be ignored here.
        /// </summary>
        [Test]
        public void Run_LoopNotRunningYet_IsRunningAfterwards()
        {
            // arrange
            IStateManager sm = this.mocks.NewMock<IStateManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            MainLoopManager mlm = new MainLoopManager( sm, lm );

            // act
            mlm.RunAsynchronous();

            // assert
            Assert.IsTrue( mlm.Running );
        }

        /// <summary>
        /// Ignore any exceptions if this test passes - they are thrown because of the game loop thread and can be ignored here.
        /// </summary>
        [Test, ExpectedException( typeof( InvalidOperationException ) )]
        public void Run_LoopAlreadyRunning_ThrowInvalidOperationExceptioin()
        {
            // arrange
            IStateManager sm = this.mocks.NewMock<IStateManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            MainLoopManager mlm = new MainLoopManager( sm, lm );
            mlm.RunAsynchronous();

            // act
            mlm.RunAsynchronous();

            // assert
        }

        [Test, ExpectedException( typeof( InvalidOperationException ) )]
        public void Stop_LoopNotRunning_ThrowInvalidOperationException()
        {
            // arrange
            IStateManager sm = this.mocks.NewMock<IStateManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            MainLoopManager mlm = new MainLoopManager( sm, lm );

            // act
            mlm.Stop();

            // assert
        }   
    }
}
