/**************************************
 * FILE:          EntityTest.cs
 * DATE:          05.01.2010 10:23:14
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
using NGin.Core;
using NMock2;
using NGin.Core.Exceptions;
using System.Collections.ObjectModel;
using NGin.Core.Scene;
using System.Threading;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class EntityTest
    {
        private Mockery mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new Mockery();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAllExpectationsHaveBeenMet();
            mocks.Dispose();
            mocks = null;
        }

        private T GetObjectFromIEnumerable<T>( string key, IEnumerable<KeyValuePair<string,T>> e )
        {
            T result = default(T);

            foreach ( var kvp in e )
            {
                if ( kvp.Key == key )
                {
                    result = kvp.Value;
                }
            }

            return result;
        }

        private IEntityExtension GetExtensionMock( string name )
        {
            IEntityExtension mockExtension = this.mocks.NewMock<IEntityExtension>();
            Stub.On( mockExtension ).GetProperty( "Name" ).Will( Return.Value( name ) );
            Stub.On( mockExtension ).Method( "Attatch" ).WithAnyArguments();
            Stub.On( mockExtension ).Method( "Detatch" ).WithAnyArguments();
            return mockExtension;
        }

        [Test]
        public void Entity_CreateEntity_Success()
        {
            // arrange
            Entity entity;
            string name = "testEntity";

            // act
            entity = new Entity( name );

            // assert
            Assert.IsNotNull( entity );
            Assert.IsInstanceOf<Entity>( entity );
            Assert.AreEqual( name, entity.Name );
            Assert.AreEqual( 0, entity.ExtensionsCount );
        }

        [Test]
        public void AddExtension_AddOneExtension_ExtensionMembersExistent()
        {
            // arrange
            
            string extensionName = "testExtension";
            IEntityExtension mockExtension = this.GetExtensionMock( extensionName );
            string entityName = "testEntity";
            Entity entity = new Entity( entityName );

            // act
            entity.AddExtension( mockExtension );

            // assert
            Assert.IsTrue( entity.ExtensionsCount == 1 );
            Assert.AreEqual( mockExtension, entity.GetExtension( extensionName) );
        }

        [Test]
        public void RemoveExtension_RemoveAddedExtension_ExtensionsAreEmpty()
        {
            // arrange
            
            string extensionName = "testExtension";
            IEntityExtension mockExtension = this.GetExtensionMock( extensionName );            
            string entityName = "testEntity";
            Entity entity = new Entity( entityName );
            entity.AddExtension( mockExtension );
            IEntityExtension removedExtension;

            // act
            removedExtension = entity.RemoveExtension( extensionName );

            // assert
            Assert.IsTrue( entity.ExtensionsCount == 0 );
            Assert.AreEqual( mockExtension, removedExtension );
        }

        [Test, ExpectedException(typeof(ExtensionNotFoundException))]
        public void RemoveExtension_EntityHasNoExtensions_RaiseExtensionNotFoundException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );

            // act
            entity.RemoveExtension( "someExtension" );

            // assert
        }

        [Test, ExpectedException(typeof(DuplicateExtensionException))]
        public void AddExtension_AddSameExtensionTwice_RaiseDuplicateExtensionException()
        {
            // arrange
            string extensionName = "testExtension";
            IEntityExtension mockExtension = this.GetExtensionMock( extensionName );            
            Entity entity = new Entity( "testEntity" );

            // act
            entity.AddExtension( mockExtension );
            entity.AddExtension( mockExtension );

            // assert
        }

        [Test]
        public void RegisterActionHandler_OneValidHandler_HandlerCountEqualOne()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomething";
            EventHandler<ActionRequestEventArgs> handler = (sender,args) => {};

            // act
            entity.RegisterActionHandler( actionKey, handler );

            // assert
            Assert.AreEqual( 1, entity.ActionHandlersCount );
        }

        [Test]
        public void RegisterActionHandler_100ValidHandlers_HandlerCountEqualOneInvocationListHas100()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomething";
            EventHandler<ActionRequestEventArgs> handler = ( sender, args ) => { };
            int times = 100;
            EventHandler<ActionRequestEventArgs> handlerReturned = null;

            // act
            for ( int i = 0; i < times; i++ )
            {
                entity.RegisterActionHandler( actionKey, handler );
            }

            handlerReturned = this.GetObjectFromIEnumerable<EventHandler<ActionRequestEventArgs>>( actionKey, entity.ActionHandlers );

            // assert
            Assert.AreEqual( 1, entity.ActionHandlersCount );
            Assert.AreEqual( times, handlerReturned.GetInvocationList().Length );
        }

        [Test]
        public void RegisterActionHandler_100ValidHandlersEachDifferentThread_HandlerCountEqualOneInvocationListHas100()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomething";
            EventHandler<ActionRequestEventArgs> handler = ( sender, args ) => { };
            int times = 100;
            EventHandler<ActionRequestEventArgs> handlerReturned = null;

            // act
            List<Thread> threads = new List<Thread>();
            for ( int i = times; i > 0; i-- )
            {
                Thread t = new Thread( x => { Thread.Sleep( i * 15 );  entity.RegisterActionHandler( actionKey, handler ); } );
                threads.Add( t );
                t.Start();
            }

            foreach ( Thread t in threads )
            {
                t.Join();
            }

            // assert
            handlerReturned = this.GetObjectFromIEnumerable<EventHandler<ActionRequestEventArgs>>( actionKey, entity.ActionHandlers );
            Assert.AreEqual( 1, entity.ActionHandlersCount );
            Assert.AreEqual( times, handlerReturned.GetInvocationList().Length );
        }

        [Test,ExpectedException(typeof(ArgumentNullException))]
        public void RegisterActionHandler_ActionKeyNull_RaiseArgumentNullException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = null;
            EventHandler<ActionRequestEventArgs> handler = ( sender, e ) => { };

            // act
            entity.RegisterActionHandler( actionKey, handler );

            // assert
        }

        [Test]
        public void RegisterActionHandler_ActionKeyEmpty_Success()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = string.Empty;
            EventHandler<ActionRequestEventArgs> handler = ( sender, e ) => { };

            // act
            entity.RegisterActionHandler( actionKey, handler );

            // assert
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void RegisterActionHandler_HandlerNull_RaiseArgumentNullException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomethingWrong";
            EventHandler<ActionRequestEventArgs> handler = null;

            // act
            entity.RegisterActionHandler( actionKey, handler );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void UnregisterActionHandler_ActionKeyNull_RaiseArgumentNullException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKeyNull = null;
            EventHandler<ActionRequestEventArgs> handler = ( sender, e ) => { };
            bool removeAll = true;

            // act
            entity.UnregisterActionHandler( actionKeyNull, handler, removeAll );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void UnregisterActionHandler_HandlerNull_RaiseArgumentNullException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "testAction";
            EventHandler<ActionRequestEventArgs> handlerNull = null;
            bool removeAll = true;

            // act
            entity.UnregisterActionHandler( actionKey, handlerNull, removeAll );

            // assert
        }

        [Test]
        public void UnregisterActionHandler_HandlerAdded5TimesBeforeRemoveAll_NoneLeft()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomething";
            EventHandler<ActionRequestEventArgs> handler = ( sender, e ) => { };
            EventHandler<ActionRequestEventArgs> handlerReturned = null;
            int addTime = 5;
            for ( int i = 0; i < addTime; i++ )
            {
                entity.RegisterActionHandler( actionKey, handler );
            }
            handlerReturned = this.GetObjectFromIEnumerable<EventHandler<ActionRequestEventArgs>>( actionKey, entity.ActionHandlers );
            Assert.AreEqual( addTime, handlerReturned.GetInvocationList().Length );

            // act
            entity.UnregisterActionHandler( actionKey, handler, true );

            // assert
            Assert.AreEqual( 0, entity.ActionHandlersCount );
        }

        [Test]
        public void UnregisterActionHandler_HandlerAdded5TimesBeforeRemoveLastOne_4Left()
        {
            // arrange   
            Entity entity = new Entity( "testEntity" );
            string actionKey = "doSomething";
            EventHandler<ActionRequestEventArgs> handler = ( sender, e ) => { };
            EventHandler<ActionRequestEventArgs> handlerReturned = null;
            int addTime = 5;
            for ( int i = 0; i < addTime; i++ )
            {
                entity.RegisterActionHandler( actionKey, handler );
            }
            handlerReturned = this.GetObjectFromIEnumerable<EventHandler<ActionRequestEventArgs>>( actionKey, entity.ActionHandlers );
            Assert.AreEqual( addTime, handlerReturned.GetInvocationList().Length );

            // act
            entity.UnregisterActionHandler( actionKey, handler, false );
            handlerReturned = this.GetObjectFromIEnumerable<EventHandler<ActionRequestEventArgs>>( actionKey, entity.ActionHandlers );

            // assert
            Assert.AreEqual( 1, entity.ActionHandlersCount );
            Assert.AreEqual( addTime - 1, handlerReturned.GetInvocationList().Length );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void RequestAction_ActionKeyNull_RaiseArgumentNullException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = null;
            object sender = new object();
            object[] parameters = new object[] { };

            // act
            entity.RequestAction( sender, actionKey, parameters );

            // assert
        }

        [Test]
        public void RequestAction_ActionKeyEmpty_SuccessNoException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = string.Empty;
            object sender = new object();
            object[] parameters = new object[] { };

            // act
            entity.RequestAction( sender, actionKey, parameters );

            // assert
        }

        [Test]
        public void RequestAction_SenderNull_SuccessNoException()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = string.Empty;
            object sender = null;
            object[] parameters = new object[] { };

            // act
            entity.RequestAction( sender, actionKey, parameters );

            // assert
        }

        [Test]
        public void RequestAction_RequestFrom100DifferentThreads_ResultCorrect()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            int actualActionCount = 0;
            int expectedActionCount = 100;
            string actionKey = "SetActionPerfomedToTrue";
            entity.RegisterActionHandler( actionKey, ( sender, actionRequestEventArgs ) => { actualActionCount++; } );

            // act
            IList<System.Threading.Thread> threads = new List<System.Threading.Thread>( expectedActionCount );
            for ( int i = 0; i < expectedActionCount; i++ )
            {
                System.Threading.Thread t = new System.Threading.Thread( x => { System.Threading.Thread.Sleep( i * 15 ); entity.RequestAction( this, actionKey, "abc", 123 ); } );
                threads.Add( t );
                t.Start();
            }

            foreach ( System.Threading.Thread t in threads )
            {
                t.Join();
            }
            // request invalid action and this will not effect the test
            entity.RequestAction( this, "SomethingElse" );            

            // assert
            Assert.AreEqual( expectedActionCount, actualActionCount );
        }

        [Test, ExpectedException(typeof(ExtensionNotFoundException))]
        public void RequestAction_NoRegisteredHandler_SuccessNoReaction()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string actionKey = "DoSomething";

            // act
            entity.RequestAction( this, actionKey, "abc", 123 );

            // assert
            Assert.Pass( "No exceptions occurred, therefore: test passed." );
        }

        #region Not needed since setting attributes directly from the outside is not possible.
        //[Test]
        //public void SetAttribute_MemberIdExists_AttributeSetCorrectly()
        //{
        //    // arrange

        //    // act

        //    // assert
        //    Assert.Fail();
        //}

        //[Test, ExpectedException(typeof(ExtensionNotFoundException))]
        //public void SetAttribute_UnknownMemberId_RaiseExtensionNotFoundException()
        //{
        //    // arrange

        //    // act

        //    // assert
        //    Assert.Fail();
        //}

        //[Test, ExpectedException(typeof(ArgumentException))]
        //public void SetAttribute_InvalidParameters_RaiseArgumentException()
        //{
        //    // arrange

        //    // act

        //    // assert
        //    Assert.Fail();
        //} 
        #endregion

        [Test]
        public void GetAttribute_AttributeIdExists_ReturnValidData()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            // inject property
            string attributeId = "Position";
            object testObject = new object();
            IEntityExtensionPublicationStorage extView = entity.Properties as IEntityExtensionPublicationStorage;
            extView.Publicize( attributeId, testObject );
            entity.Properties.SwapAndFlush();
            object result = null;

            // act
            result = entity.GetAttribute( attributeId );

            // assert
            Assert.IsNotNull( result );
            Assert.AreEqual( testObject, result );
        }

        [Test]
        public void GetAttribute_UnknownAttributeId_ReturnsNull()
        {
            // arrange
            Entity entity = new Entity( "testEntity" );
            string attributeId = "Position";
            object result = null;

            // act
            result = entity.GetAttribute( attributeId );

            // assert
            Assert.IsNull( result );
        }

        [Test]
        public void Equals_EqualEntities_AreEqual()
        {
            // arrange
            string sameName = "test";
            Entity one = new Entity( sameName );
            Entity equalTwo = new Entity( sameName );
            bool areEqual = false;

            // act
            areEqual = one.Equals( equalTwo );

            // assert
            Assert.IsTrue( areEqual );
            Assert.AreEqual( one.GetHashCode(), equalTwo.GetHashCode() );
        }

        [Test]
        public void Equals_DifferentIds_AreNotEqual()
        {
            // arrange
            Entity one = new Entity( "one" );
            Entity equalTwo = new Entity( "two" );
            bool areEqual = false;

            // act
            areEqual = one.Equals( equalTwo );

            // assert
            Assert.IsFalse( areEqual );
            Assert.AreNotEqual( one.GetHashCode(), equalTwo.GetHashCode() );
        }
    }
}
