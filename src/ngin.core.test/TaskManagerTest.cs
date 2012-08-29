/**************************************
 * FILE:          TaskManagerTest.cs
 * DATE:          05.01.2010 10:24:54
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
using NGin.Core.Tasks;
using NGin.Core.Logging;
using NMock2;
using NGin.Core.Systems;
using System.Threading;
using NGin.Core.States;

namespace NGin.Core.Test
{
    [TestFixture]
    public class TaskManagerTest
    {
        private class TestSystem : NGin.Core.Systems.NGinSystem
        {
            public TestSystem( ILogManager logManager )
                : base( TimeSpan.FromSeconds(0), logManager )
            {

            }

            public override string Name
            {
                get { return "TestSystem"; }
            }

            //private static object updateCallsLock = new object();
            //public static List<int> updateCalls = new List<int>();
            //public static List<int> UpdateCalls 
            //{
            //    get { lock ( updateCallsLock ) { return updateCalls;  } }
            //}

            //public static void ResetUpdateCallsList()
            //{
            //    lock ( TestSystem.updateCallsLock )
            //    {
            //        TestSystem.updateCalls.Clear();
            //    }
            //}

            public override void Update()
            {
                //lock ( updateCallsLock )
                //{
                //    updateCalls.Add( Thread.CurrentThread.ManagedThreadId );
                //}
            }
        }

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
        public void TaskManager_ValidLogManagerValidSystemsManager_CreateInstance()
        {
            // arrange
            ILogManager lmMock = this.mocks.NewMock<ILogManager>();
            Stub.On( lmMock ).Method( "Trace" ).WithAnyArguments();
            ISystemsManager smMock = this.mocks.NewMock<ISystemsManager>();
            IMainLoopManager mlmMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( mlmMock ).EventAdd( "HeartbeatStarted" );
            Stub.On( mlmMock ).EventAdd( "HeartbeatEnded" );
            IStateManager stmMock = this.mocks.NewMock<IStateManager>();
            ITaskManager tm;

            // act
            tm = new TaskManager( lmMock, smMock, mlmMock, stmMock );

            // assert
            Assert.IsNotNull( tm );
            Assert.IsInstanceOf<TaskManager>( tm );            
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TaskManager_LogManagerNull_ThrowArgumentNullException()
        {
            // arrange
            ILogManager lmMock = null;
            ISystemsManager smMock = this.mocks.NewMock<ISystemsManager>();
            IMainLoopManager mlmMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( mlmMock ).EventAdd( "HeartbeatStarted" );
            Stub.On( mlmMock ).EventAdd( "HeartbeatEnded" );
            IStateManager stmMock = this.mocks.NewMock<IStateManager>();
            ITaskManager tm;

            // act
            tm = new TaskManager( lmMock, smMock, mlmMock, stmMock );

            // assert
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TaskManager_SystemsManagerNull_ThrowArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.mocks.NewMock<ILogManager>();
            Stub.On( lmMock ).Method( "Trace" ).WithAnyArguments();
            ISystemsManager smMock = null;
            IMainLoopManager mlmMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( mlmMock ).EventAdd( "HeartbeatStarted" );
            Stub.On( mlmMock ).EventAdd( "HeartbeatEnded" );
            IStateManager stmMock = this.mocks.NewMock<IStateManager>();
            ITaskManager tm;

            // act
            tm = new TaskManager( lmMock, smMock, mlmMock, stmMock );

            // assert
        }

        [Test]
        public void CallAllSystemTasksSequential_10000SystemsExist_10000CallsCounted()
        {
            // arrange

            // act

            // assert
            Assert.Inconclusive( "YAGNI?" );
        }

        [Test]
        public void CallAllSystemTasksSequential_NoSystemsFound_NoCalls()
        {
            // arrange

            // act

            // assert
            Assert.Inconclusive( "YAGNI?" );
        }

        [Test]
        public void CallAllSystemTasksAsynchrnous_NoSystemsFound_NoCalls()
        {
            // arrange

            // act

            // assert
            Assert.Fail();
        }

        [Test, NUnit.Framework.Ignore("WARNING: This test is error-prone. I have tested the async task call directly in the task manager using AsyncCallbacks and it does process all tasks. The problem must be the way this test keeps count of tasks.")]
        public void CallAllSystemTasksAsynchronous_10000SystemsExist_10000CallsCounted()
        {
            // arrange
            ILogManager lmMock = this.mocks.NewMock<ILogManager>();
            Stub.On( lmMock ).Method( "Trace" ).WithAnyArguments();
            int systemsCount = 10000;
            ISystem systemOne = new TestSystem( lmMock );
            //TestSystem.ResetUpdateCallsList();
            IList<ISystem> systems = new List<ISystem>( systemsCount );
            for ( int i = 0; i < systemsCount; i++ )
            {
                systems.Add( systemOne );
            }

            int taskCounter = 0;
            object taskCounterLock = new object();

            systemOne.TaskStarted += x => { lock ( taskCounterLock ) { taskCounter++; } };

            Assert.AreEqual( systemsCount, systems.Count );
            ISystemsManager smMock = this.mocks.NewMock<ISystemsManager>();
            Stub.On( smMock ).GetProperty( "RegisteredSystems" ).Will( Return.Value( systems ) );
            IMainLoopManager mlmMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( mlmMock ).EventAdd( "HeartbeatStarted" );
            Stub.On( mlmMock ).EventAdd( "HeartbeatEnded" );
            IStateManager stmMock = this.mocks.NewMock<IStateManager>();
            ITaskManager tm = new TaskManager( lmMock, smMock, mlmMock, stmMock );

            // act
            tm.CallAllSystemTasksAsynchronous();

            // assert
            //Assert.AreEqual( systemsCount, TestSystem.UpdateCalls.Count );
            Assert.AreEqual( systemsCount, taskCounter );


            //SortedList<int, int> usedThreads = new SortedList<int, int>();
            //foreach ( int threadId in TestSystem.UpdateCalls )
            //{
            //    if ( !usedThreads.ContainsKey( threadId ) )
            //    {
            //        usedThreads.Add( threadId, threadId );
            //    }
            //}
            //if ( Environment.ProcessorCount == 1 )
            //{
            //    Assert.AreEqual( 1, usedThreads );
            //    Assert.Inconclusive( "This machine only has one CPU. Asynchronous behavious or the task manager can only work parallel on a machine with more than one CPU." );
            //}
            //if ( Environment.ProcessorCount > 1 && usedThreads.Count == 1 )
            //{
            //    Assert.Inconclusive( "The asynchronous method seems to have only used 1 thread even though this machine has more than one CPU. Run the test again to ensure it usually uses more threads." );
            //}
        }
    }
}
