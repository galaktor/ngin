/**************************************
 * FILE:          EntityExtensionTest.cs
 * DATE:          05.01.2010 10:23:02
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
using NGin.Core.Scene;
using NGin.Core.Systems;
using NGin.Core.Logging;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class EntityExtensionTest
    {

        Mockery mocks;

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
        public void EntityExtensionTest_CreateExtension_JustName_Success()
        {
            // arrange
            EntityExtension ext;
            string name = "test";
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();

            // act
            ext = new EntityExtension( name, systemMock, logManagerMock );

            // assert
            Assert.IsNotNull( ext );
            Assert.IsInstanceOf<EntityExtension>( ext );
            Assert.IsNull( ext.PublicData );
            Assert.IsFalse( ext.IsActive );
            Assert.AreEqual( name, ext.Name );
        }

        [Test]
        public void Attatch_ValidRegistryValidStorage_Success()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension ext = new EntityExtension( "test", systemMock, logManagerMock );
            IEntityExtensionPublicationStorage storageMock = this.mocks.NewMock<IEntityExtensionPublicationStorage>();
            IActionRequestRegistry registry = this.mocks.NewMock<IActionRequestRegistry>();
            IActionRequestable requestTargetMock = this.mocks.NewMock<IActionRequestable>();
            IUpdateRequester updateRequesterMock = this.mocks.NewMock<IUpdateRequester>();
            Stub.On( storageMock ).Method( "Publicize" ).WithAnyArguments();
            Stub.On( updateRequesterMock ).EventAdd( "UpdateRequested" );

            // act
            ext.Attatch( registry, storageMock, requestTargetMock, updateRequesterMock );

            // assert
            Assert.AreEqual( storageMock, ext.PublicData );
            Assert.IsTrue( ext.IsActive );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void Attatch_ValidRegistryStorageNull_RaiseArgumentNullException()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension ext = new EntityExtension( "test", systemMock, logManagerMock );
            IEntityExtensionPublicationStorage storageMock = null;
            IActionRequestRegistry registry = this.mocks.NewMock<IActionRequestRegistry>();
            IActionRequestable requestTargetMock = this.mocks.NewMock<IActionRequestable>();
            IUpdateRequester updateRequesterMock = this.mocks.NewMock<IUpdateRequester>();            
            Stub.On( updateRequesterMock ).EventAdd( "UpdateRequested" );

            // act
            ext.Attatch( registry, storageMock, requestTargetMock, updateRequesterMock );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void Attatch_RegistryNullValidStorage_RaiseArgumentNullException()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension ext = new EntityExtension( "test", systemMock, logManagerMock );
            IEntityExtensionPublicationStorage storageMock = this.mocks.NewMock<IEntityExtensionPublicationStorage>();
            IActionRequestRegistry registry = null;
            IActionRequestable requestTargetMock = this.mocks.NewMock<IActionRequestable>();
            IUpdateRequester updateRequesterMock = this.mocks.NewMock<IUpdateRequester>();
            Stub.On( storageMock ).Method( "Publicize" ).WithAnyArguments();
            Stub.On( updateRequesterMock ).EventAdd( "UpdateRequested" );

            // act
            ext.Attatch( registry, storageMock, requestTargetMock, updateRequesterMock );

            // assert
        }

        [Test]
        public void Detatch_ValidRegistry_Success()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension ext = new EntityExtension( "test", systemMock, logManagerMock );
            IActionRequestRegistry registry = this.mocks.NewMock<IActionRequestRegistry>();
            IEntityExtensionPublicationStorage storageMock = this.mocks.NewMock<IEntityExtensionPublicationStorage>();
            IActionRequestable requestTargetMock = this.mocks.NewMock<IActionRequestable>();
            IUpdateRequester updateRequesterMock = this.mocks.NewMock<IUpdateRequester>();
            Stub.On( storageMock ).Method( "Publicize" ).WithAnyArguments();
            Stub.On( updateRequesterMock ).EventAdd( "UpdateRequested" );
            Stub.On( updateRequesterMock ).EventRemove( "UpdateRequested" );
            ext.Attatch( registry, storageMock, requestTargetMock, updateRequesterMock );
            Assert.AreEqual( storageMock, ext.PublicData );

            // act            
            ext.Detatch( registry, updateRequesterMock );

            // assert
            Assert.IsNull( ext.PublicData );
            Assert.IsFalse( ext.IsActive );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void Detatch_RegistryNull_RaiseArgumentNullException()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension ext = new EntityExtension( "test", systemMock, logManagerMock );
            IActionRequestRegistry registry = null;
            IEntityExtensionPublicationStorage storageMock = this.mocks.NewMock<IEntityExtensionPublicationStorage>();
            IActionRequestable requestTargetMock = this.mocks.NewMock<IActionRequestable>();
            IUpdateRequester updateRequesterMock = this.mocks.NewMock<IUpdateRequester>();
            Stub.On( storageMock ).Method( "Publicize" ).WithAnyArguments();
            Stub.On( updateRequesterMock ).EventAdd( "UpdateRequested" );
            Stub.On( updateRequesterMock ).EventRemove( "UpdateRequested" );
            ext.Attatch( registry, storageMock, requestTargetMock, updateRequesterMock );
            Assert.AreEqual( storageMock, ext.PublicData );

            // act
            ext.Detatch( registry, updateRequesterMock );

            // assert
        }

        class OtherExtensionTest : EntityExtension
        {
            public OtherExtensionTest( string name, ISystem system, ILogManager logManager )
                : base( name, system, logManager )
            { }

            public void SetNameToNull()
            {
                this.Name = null;
            }
        }

        [Test]
        public void Equals_IEquatable_SameNameAndType_AreEqual()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension one = new EntityExtension( "one", systemMock, logManagerMock );
            EntityExtension two = new EntityExtension( "one", systemMock, logManagerMock );
            bool areEqual;

            // act
            areEqual = one.Equals( two as IEntityExtension );

            // assert
            Assert.IsTrue( areEqual );
            Assert.AreEqual( one.GetHashCode(), two.GetHashCode() );
        }

        [Test]
        public void Equals_IEquatable_OtherNameSameType_AreNotEqual()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension one = new EntityExtension( "one", systemMock, logManagerMock );
            EntityExtension two = new EntityExtension( "two", systemMock, logManagerMock );
            bool areEqual;

            // act
            areEqual = one.Equals( two as IEntityExtension );

            // assert
            Assert.IsFalse( areEqual );
            Assert.AreNotEqual( one.GetHashCode(), two.GetHashCode() );
        }

        [Test]
        public void Equals_IEquatable_SameNameOtherType_AreNotEqual()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension one = new EntityExtension( "one", systemMock, logManagerMock );
            EntityExtension two = new OtherExtensionTest( "one", systemMock, logManagerMock );
            bool areEqual;

            // act
            areEqual = one.Equals( two as IEntityExtension );

            // assert
            Assert.IsFalse( areEqual );
            Assert.AreNotEqual( one.GetHashCode(), two.GetHashCode() );
        }

        [Test]
        public void Equals_IEquatable_OtherIsNull_AreNotEqual()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension one = new EntityExtension( "one", systemMock, logManagerMock );
            EntityExtension two = null;
            bool areEqual;

            // act
            areEqual = one.Equals( two as IEntityExtension );

            // assert
            Assert.IsFalse( areEqual );
        }

        [Test]
        public void Equals_IEquatable_NameNullOtherValid_AreNotEqual()
        {
            // arrange
            ISystem systemMock = this.mocks.NewMock<ISystem>(); 
            Stub.On( systemMock ).EventAdd( "TaskStarted" ); 
            Stub.On( systemMock ).EventAdd( "TaskEnded" );
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();
            Stub.On( logManagerMock ).Method( "Trace" ).WithAnyArguments();
            EntityExtension one = new EntityExtension( "one", systemMock, logManagerMock );
            OtherExtensionTest two = new OtherExtensionTest( "one", systemMock, logManagerMock );
            two.SetNameToNull();
            bool areEqual;

            // act
            areEqual = one.Equals( two as IEntityExtension );

            // assert
            Assert.IsFalse( areEqual );
            Assert.AreNotEqual( one.GetHashCode(), two.GetHashCode() );
        }
    }
}
