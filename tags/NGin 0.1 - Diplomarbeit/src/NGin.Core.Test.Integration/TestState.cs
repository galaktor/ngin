/**************************************
 * FILE:          TestState.cs
 * DATE:          05.01.2010 10:26:10
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
using NGin.Core.Scene;
using NGin.Core.States;
using NGin.Core.States.RHFSM;
using NGin.Core.Systems;
using NGin.Core.Messaging;
using System;
using System.Diagnostics;

namespace NGin.Core.Test.Integration
{
    [NGinRootState( typeof( TestState ) ), Service( typeof( TestState ), null, false )]
    public class TestState : State
    {
        public ISystemsManager SystemsManager { get; set; }
        public TestState( ILogManager logManager, ISceneManager sceneManager, ISystemsManager systemsManager, IMessageManager messageManager )
            : base( "test", null, sceneManager, logManager, new ExitState( sceneManager, logManager, messageManager ) )
        {
            this.SystemsManager = systemsManager;
        }

        public override void BuildUp()
        {
            this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestState: BUILDUP" );

            ISystem system1 = SystemsManager.GetSystem( "TestSystem" );
            ISystem system2 = SystemsManager.GetSystem( "TestSystem2" );
            ISystem system3 = SystemsManager.GetSystem( "TestSystem3" );
            ISystem system4 = SystemsManager.GetSystem( "TestSystem4" );
            ISystem system5 = SystemsManager.GetSystem( "TestSystem5" );
            ISystem system6 = SystemsManager.GetSystem( "TestSystem6" );
            IEntity entity;

            for ( int i = 0; i < 500; i++ )
            {
                entity = this.Scene.CreateNewEntity( "test" + i.ToString() );
                IEntityExtension ext;
                ext = this.Scene.CreateNewEntityExtension<TestExtension>( "test" );
                entity.AddExtension( ext );
                ext = this.Scene.CreateNewEntityExtension<TestExtension2>( "test2" );
                entity.AddExtension( ext );
                ext = this.Scene.CreateNewEntityExtension<TestExtension3>( "test3" );
                entity.AddExtension( ext );
                ext = this.Scene.CreateNewEntityExtension<TestExtension4>( "test4" );
                entity.AddExtension( ext );
                ext = this.Scene.CreateNewEntityExtension<TestExtension5>( "test5" );
                entity.AddExtension( ext );
                ext = this.Scene.CreateNewEntityExtension<TestExtension6>( "test6" );
                entity.AddExtension( ext );

                entity.RequestAction( null, "test", 123, "abc" );
                entity.RequestAction( null, "test", 123, "abc" );
                entity.RequestAction( null, "test", 123, "abc" );
                entity.RequestAction( null, "test", 123, "abc" );
                entity.RequestAction( null, "test", 123, "abc" );

                this.Scene.AddEntity( entity );                
            }

        }

        public override void TearDown()
        {
            this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestState: TEARDOWN" );
            this.Scene.ClearEntities();
        }

        Stopwatch s = new Stopwatch();
        public override void Update()
        {
            if ( !s.IsRunning )
            {
                s.Start();
            }

            this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestState: UPDATE" );
            if ( s.Elapsed >= TimeSpan.FromSeconds (5) )
            {
                this.RequestTransitionTo( State.CombineStateNames( this.Name, "exit" ) );
                s.Reset();
            }
        }
    }
}
