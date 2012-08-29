/**************************************
 * FILE:          StateManagerTest.cs
 * DATE:          05.01.2010 10:24:43
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
using NGin.Core.States;
using NGin.Core.Logging;
using NGin.Core.Configuration;
using NGin.Core.States.RHFSM;
using NGin.Core.Scene;

namespace NGin.Core.Test
{
    [NGinRootState( typeof( SmTestState ) ), Service( typeof( SmTestState ), null, false )]
    internal class SmTestState : State
    {
        public SmTestState( ISceneManager scm, ILogManager lm )
            : base( "test", "init", scm, lm )
        { }
    }

    [TestFixture]
    public class StateManagerTest
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
        public void StateManager_ValidLogManager_CreateInstance()
        {
            // arrange
            StateManager sm;
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On(lm).Method("Trace").WithAnyArguments();
            IPluginManager pm = this.mocks.NewMock<IPluginManager>();
            INGinCore c = this.mocks.NewMock<INGinCore>();
            Stub.On( c ).EventAdd( "RunStarted" );
            Stub.On( c ).EventRemove( "RunStarted" );
            Stub.On( c ).EventAdd( "RunStopped" );
            Stub.On( c ).EventRemove( "RunStopped" );

            // act
            sm = new StateManager( lm, pm, c );

            // assert
            Assert.IsNotNull( sm );
            Assert.IsInstanceOf<StateManager>( sm );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void StateManager_LogManagerNull_ThrowArgumentNullException()
        {
            // arrange
            StateManager sm;
            ILogManager lm = null;
            IPluginManager pm = this.mocks.NewMock<IPluginManager>();
            INGinCore c = this.mocks.NewMock<INGinCore>();
            Stub.On( c ).EventAdd( "RunStarted" );
            Stub.On( c ).EventRemove( "RunStarted" );
            Stub.On( c ).EventAdd( "RunStopped" );
            Stub.On( c ).EventRemove( "RunStopped" );

            // act
            sm = new StateManager( lm, pm, c );

            // assert
        }

        [Test, ExpectedException( typeof( InvalidOperationException ) )]
        public void Initialize_NoCustomRootStateLoaded_ThrowInvalidOperationException()
        {
            // arrange
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            IPluginManager pm = this.mocks.NewMock<IPluginManager>();
            Stub.On( pm ).Method( "GetPluginsForType" ).WithAnyArguments().Will( Return.Value( new List<Attribute>() { null } ) );
            INGinCore c = this.mocks.NewMock<INGinCore>();
            ISceneManager scm = this.mocks.NewMock<ISceneManager>();
            Stub.On( scm ).Method( "CreateAndAddScene" ).Will( Return.Value( null ) );
            Stub.On( c ).Method( "GetService" ).With( typeof( SmTestState ) ).Will( Return.Value( new SmTestState( scm, lm ) ) );
            Stub.On( c ).EventAdd( "RunStarted" );
            Stub.On( c ).EventRemove( "RunStarted" );
            Stub.On( c ).EventAdd( "RunStopped" );
            Stub.On( c ).EventRemove( "RunStopped" );
            StateManager sm = new StateManager( lm, pm, c );
            IMachine m = this.mocks.NewMock<IMachine>();
            Stub.On( m ).Method( "Initialize" ).WithAnyArguments();

            // act
            sm.Initialize( pm, m, c );

            // assert
        }

        //[Test, ExpectedException( typeof( InvalidOperationException ) )]
        //public void Initialize_AlreadyInitialized_ThrowInvalidOperationException()
        //{
        //    // arrange
        //    ILogManager lm = this.mocks.NewMock<ILogManager>();
        //    Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
        //    IPluginManager pm = this.mocks.NewMock<IPluginManager>();
        //    NGinRootStateAttribute rootStateAtt = new NGinRootStateAttribute( typeof( SmTestState ) );
        //    Stub.On( pm ).Method( "GetPluginsForType" ).WithAnyArguments().Will( Return.Value( new List<Attribute>() { rootStateAtt } ) );
        //    INGinCore c = this.mocks.NewMock<INGinCore>();
        //    ISceneManager scm = this.mocks.NewMock<ISceneManager>();
        //    Stub.On( scm ).Method( "CreateAndAddScene" ).Will( Return.Value( null ) );
        //    Stub.On( c ).Method( "GetService" ).With( typeof( SmTestState ) ).Will( Return.Value( new SmTestState( scm, lm ) ) );
        //    Stub.On( c ).EventAdd( "RunStarted" );
        //    Stub.On( c ).EventRemove( "RunStarted" );
        //    Stub.On( c ).EventAdd( "RunStopped" );
        //    Stub.On( c ).EventRemove( "RunStopped" );
        //    StateManager sm = new StateManager( lm, pm, c );
        //    IMachine m = this.mocks.NewMock<IMachine>();
        //    Stub.On( m ).Method( "Initialize" ).WithAnyArguments();

        //    // act
        //    sm.Initialize( pm, m, c );
        //    sm.Initialize( pm, m, c );

        //    // assert
        //}
    }
}
