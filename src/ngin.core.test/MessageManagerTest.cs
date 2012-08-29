/**************************************
 * FILE:          MessageManagerTest.cs
 * DATE:          05.01.2010 10:23:39
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
using NGin.Core.Messaging;
using NGin.Core.Logging;
using NGin.Core.Tasks;
using NGin.Core.Scene;

namespace NGin.Core.Test
{
    [TestFixture]
    public class MessageManagerTest
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
        public void CreateInstance_ValidParameters_Success()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m; 

            // act
            m = new MessageManager( logManagerMock, loopManagerMock );

            // assert
            Assert.IsNotNull( m );
            Assert.IsInstanceOf<MessageManager>( m );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_LogManagerNull_ThrowArgumentNullException()
        {
            // arrange
            ILogManager logManagerMock = null;
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m;

            // act
            m = new MessageManager( logManagerMock, loopManagerMock );

            // assert
            Assert.IsNotNull( m );
            Assert.IsInstanceOf<MessageManager>( m );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_MainLoopManagerNull_ThrowArgumentNullException()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = null;
            MessageManager m;

            // act
            m = new MessageManager( logManagerMock, loopManagerMock );

            // assert
            Assert.IsNotNull( m );
            Assert.IsInstanceOf<MessageManager>( m );
        }

        [Test]
        public void RegisterMessageType_ValidParameters_MessageSuccessfullyRegisterred()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );

            // act
            m.RegisterMessageType( "test", ( sender, args ) => { } );

            // assert
            Assert.AreEqual( 1, m.MessageHandlers.Count<KeyValuePair<string,EventHandler<ActionRequestEventArgs>>>() );
        }

        [Test]
        public void RegisterMessageType_TypeNullSend10Messages_Receive10Messages()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int sendTimes = 10;
            int receiveCount = 0;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { receiveCount++; };

            // act
            m.RegisterMessageType( null, a );
            for ( int i = 0; i < sendTimes; i++ )
            {
                m.SendMessage( this, "blah" );
            }

            // assert
            Assert.AreEqual( sendTimes, receiveCount );
        }

        [Test]
        public void UnregisterMessageType_TypeNullSend10Messages_Receive0Messages()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int sendTimes = 10;
            int receiveCount = 0;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { receiveCount++; };
            m.RegisterMessageType( null, a );

            // act
            m.UnregisterMessageType( null, a, false );
            for ( int i = 0; i < sendTimes; i++ )
            {
                m.SendMessage( this, "blah" );
            }

            // assert
            Assert.AreEqual( 0, receiveCount );
        }

        [Test]
        public void RegisterMessageType10Times_ValidParameters_MessageRegistryHas10Invocations()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int addTimes = 10;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { };

            // act
            for ( int i = 0; i < addTimes; i++ )
            {
                m.RegisterMessageType( "test", a );
            }

            // assert
            Assert.AreEqual( addTimes, m.GetInvocationCountForType( "test" ) );
        }

        [Test]
        public void UnregisterMessageType10Times_ValidParametersDontRemoveAll_MessageRegistryHas9Left()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int addTimes = 10;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { };
            for ( int i = 0; i < addTimes; i++ )
            {
                m.RegisterMessageType( "test", a );
            }

            // act
            m.UnregisterMessageType( "test", a, false );

            // assert
            Assert.AreEqual( addTimes - 1, m.GetInvocationCountForType("test") );
        }

        [Test]
        public void UnregisterMessageType10Times_ValidParametersRemoveAll_MessageRegistryEmpty()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int addTimes = 10;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { };
            for ( int i = 0; i < addTimes; i++ )
            {
                m.RegisterMessageType( "test", a );
            }

            // act
            m.UnregisterMessageType( "test", a, true );

            // assert
            Assert.AreEqual( 0, m.GetInvocationCountForType( "test" ) );
        }

        [Test]
        public void SendMessage_MessageTypeRegistered_MessageHandledSuccessfully()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { };
            m.RegisterMessageType( "test", a );

            // act
            m.SendMessage( this, "test" );

            // assert
            Assert.AreEqual( 1, m.MessageBufferCount );
        }

        [Test]
        public void SendMessage_MessageTypeNotRegisterred_MessageBufferEmpty()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { };

            // act
            m.SendMessage( this, "test" );

            // assert
            Assert.AreEqual( 0, m.MessageBufferCount );
        }

        [Test]
        public void SendBufferedMessages_MessageTypeRegistered10Times_10MessagesDispatchedBufferEmpty()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int registerTimes = 10;
            int sendsCounter = 0;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { sendsCounter++; };
            for ( int i = 0; i < registerTimes; i++ )
            {
                m.RegisterMessageType( "test", a );
            }
            m.SendMessage( this, "test" );

            // act
            m.SendBufferedMessages();

            // assert
            Assert.AreEqual( registerTimes, sendsCounter );
            Assert.AreEqual( 0, m.MessageBufferCount );
        }

        [Test]
        public void SendBufferedMessages_NoMessagesRegistered_NothingHappensBufferEmpty()
        {
            // arrange
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            IMainLoopManager loopManagerMock = this.mocks.NewMock<IMainLoopManager>();
            Stub.On( loopManagerMock ).EventAdd( "HeartbeatEnded" );
            MessageManager m = new MessageManager( logManagerMock, loopManagerMock );
            int registerTimes = 10;
            int sendsCounter = 0;
            EventHandler<ActionRequestEventArgs> a = ( sender, args ) => { sendsCounter++; };
            m.SendMessage( this, "test" );

            // act
            m.SendBufferedMessages();

            // assert
            Assert.AreEqual( 0, sendsCounter );
            Assert.AreEqual( 0, m.MessageBufferCount );
        }


    }
}
