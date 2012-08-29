/**************************************
 * FILE:          SceneTest.cs
 * DATE:          05.01.2010 10:24:36
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
using NGin.Core.Scene;
using System.Collections.ObjectModel;
using NGin.Core.Logging;
using NGin.Core.Systems;

namespace NGin.Core.Test
{
    [TestFixture]
    public class SceneTest
    {
        private Mockery mocks;

        private ISystem CreateSystemMock( string name )
        {
            ISystem sys = this.mocks.NewMock<ISystem>();
            Stub.On( sys ).EventAdd( "TaskStarted" );
            Stub.On( sys ).EventAdd( "TaskEnded" );
            Stub.On( sys ).GetProperty( "Name" ).Will( Return.Value( name) );

            return sys;
        }

        private IEntity CreateEntityMock( string name, params IEntityExtension[] extensions )
        {
            Dictionary<string, IEntityExtension> d = new Dictionary<string, IEntityExtension>();
            foreach ( IEntityExtension ext in extensions )
            {
                d.Add( ext.Name, ext );
            }

            IEntity e = this.mocks.NewMock<IEntity>();
            Stub.On( e ).GetProperty( "Name" ).Will( Return.Value( name ) );
            Stub.On( e ).GetProperty( "Extensions" ).Will( Return.Value( d ) );
            Stub.On( e ).EventAdd( "ExtensionAdded" );
            Stub.On( e ).EventRemove( "ExtensionAdded" );
            Stub.On( e ).EventAdd( "ExtensionRemoved" );
            Stub.On( e ).EventRemove( "ExtensionRemoved" );

            return e;
        }

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
        public void Scene_CreateInstance_Success()
        {
            // arrange
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            Scene.Scene s;

            // act
            s = new Scene.Scene( "test", eem, lm );

            // assert
            Assert.IsNotNull( s );
            Assert.IsInstanceOf<Scene.Scene>( s );
        }

        [Test]
        public void AddEntity_ValidEntity_EntityContained()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = this.CreateEntityMock( "testEntity" );

            // act
            s.AddEntity( e );

            // assert
            Assert.AreEqual( 1, s.Entities.Count<IEntity>() );
        }

        [Test]
        public void RemoveEntity_ValidEntity_EntitiesEmpty()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = this.CreateEntityMock( "testEntity" );
            s.AddEntity( e );

            // act
            s.RemoveEntity( e );

            // assert
            Assert.AreEqual( 0, s.Entities.Count<IEntity>() );
        }

        [Test]
        public void RemoveEntity_ValidEntityName_EntitiesEmtpy()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = this.CreateEntityMock( "testEntity" );
            s.AddEntity( e );

            // act


            // assert
            Assert.Fail("ToDo");
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void RemoveEntity_EntityNotFound_ThrowsArgumentException()
        {
            // arrange
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = this.CreateEntityMock( "testEntity" );

            // act
            s.RemoveEntity( e );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void AddEntity_EntityAlreadyContained_ThrowArgumentException()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = this.CreateEntityMock( "testEntity" );
            s.AddEntity( e );

            // act
            s.AddEntity( e );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void RemoveEntity_EntityNull_ThrowArgumentNullException()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = null;

            // act
            s.RemoveEntity( e );

            // assert            
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddEntity_EntityNull_ThrowArgumentNullException()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            IEntity e = null;

            // act
            s.AddEntity( e );

            // assert
        }

        [Test]
        public void AddEntityRequiredSystem_EntityExtensionsHave5UnknownSystems_5RequiredSystemsAdded()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            ISystem sys1 = this.CreateSystemMock( "sys1" );
            ISystem sys2 = this.CreateSystemMock( "sys2" );
            ISystem sys3 = this.CreateSystemMock( "sys3" );
            ISystem sys4 = this.CreateSystemMock( "sys4" );
            ISystem sys5 = this.CreateSystemMock( "sys5" );

            IEntity e = this.CreateEntityMock( "testEntity", new EntityExtension( "ext1", sys1, lm ),
                                                             new EntityExtension( "ext2", sys2, lm ),
                                                             new EntityExtension( "ext3", sys3, lm ),
                                                             new EntityExtension( "ext4", sys4, lm ),
                                                             new EntityExtension( "ext5", sys5, lm ) );


            // act
            s.AddEntityRequiredSystem( e );

            // assert
            Assert.AreEqual( 5, s.RequiredSystems.Count<string>() );
        }

        [Test]
        public void AddEntityRequiredSystem_EntityExtensionsHave5KnownSystems_RequiredSystemsCountUnchangedSpecificSystemsCountIncremented()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            ISystem sys1 = this.CreateSystemMock( "sys1" );
            ISystem sys2 = this.CreateSystemMock( "sys2" );
            ISystem sys3 = this.CreateSystemMock( "sys3" );
            ISystem sys4 = this.CreateSystemMock( "sys4" );
            ISystem sys5 = this.CreateSystemMock( "sys5" );

            IEntity e = this.CreateEntityMock( "testEntity", new EntityExtension( "ext1", sys1, lm ),
                                                             new EntityExtension( "ext2", sys2, lm ),
                                                             new EntityExtension( "ext3", sys3, lm ),
                                                             new EntityExtension( "ext4", sys4, lm ),
                                                             new EntityExtension( "ext5", sys5, lm ) );


            // act
            s.AddEntityRequiredSystem( e );
            s.AddEntityRequiredSystem( e );

            // assert
            Assert.AreEqual( 5, s.RequiredSystems.Count<string>() );
        }

        [Test]
        public void AddEntityRequiredSystems_EntityHasNoExtensions_RequiredSystemsCountUnchanged()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );           

            IEntity e = this.CreateEntityMock( "testEntity" );


            // act
            s.AddEntityRequiredSystem( e );

            // assert
            Assert.AreEqual( 0, s.RequiredSystems.Count<string>() );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void AddEntityRequiredSystems_EntityNull_ThrowArgumentNullException()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );

            IEntity e = null;


            // act
            s.AddEntityRequiredSystem( e );

            // assert
        }

        [Test]
        public void RemoveEntityRequiredSystem_EntityExtensionsHave5UnknownSystems_RequiredSystemsUnchanged()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            ISystem sys1 = this.CreateSystemMock( "sys1" );
            ISystem sys2 = this.CreateSystemMock( "sys2" );
            ISystem sys3 = this.CreateSystemMock( "sys3" );
            ISystem sys4 = this.CreateSystemMock( "sys4" );
            ISystem sys5 = this.CreateSystemMock( "sys5" );

            IEntity e = this.CreateEntityMock( "testEntity", new EntityExtension( "ext1", sys1, lm ),
                                                             new EntityExtension( "ext2", sys2, lm ),
                                                             new EntityExtension( "ext3", sys3, lm ),
                                                             new EntityExtension( "ext4", sys4, lm ),
                                                             new EntityExtension( "ext5", sys5, lm ) );
            s.AddEntityRequiredSystem( e );


            // act
            s.RemoveEntityRequiredSystem( e );

            // assert
            Assert.AreEqual( 0, s.RequiredSystems.Count<string>() );
        }

        [Test]
        public void RemoveEntityRequiredSystems_EntityHasNoExtensions_RequiredSystemsCountUnchanged()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );

            IEntity e = this.CreateEntityMock( "testEntity" );
            s.AddEntityRequiredSystem( e );


            // act
            s.RemoveEntityRequiredSystem( e );

            // assert
            Assert.AreEqual( 0, s.RequiredSystems.Count<string>() );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void RemoveEntityRequiredSystems_EntityNull_ThrowArgumentNullException()
        {
            // arrange
            IEntityExtensionManager eem = this.mocks.NewMock<IEntityExtensionManager>();
            ILogManager lm = this.mocks.NewMock<ILogManager>();
            Stub.On( lm ).Method( "Trace" ).WithAnyArguments();
            Scene.Scene s = new Scene.Scene( "test", eem, lm );
            ISystem sys1 = this.CreateSystemMock( "sys1" );
            ISystem sys2 = this.CreateSystemMock( "sys2" );
            ISystem sys3 = this.CreateSystemMock( "sys3" );
            ISystem sys4 = this.CreateSystemMock( "sys4" );
            ISystem sys5 = this.CreateSystemMock( "sys5" );

            IEntity e = null;

            // act
            s.RemoveEntityRequiredSystem( e );

            // assert
        }    
    }
}
