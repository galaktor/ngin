/**************************************
 * FILE:          EntityPropertyStorageTest.cs
 * DATE:          05.01.2010 10:23:08
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
using NGin.Core.Scene;
using NMock2;
using System.Collections;
using System.Threading;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class EntityPropertyStorageTest
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
        public void EntityPropertyStorage_ValidOwnerEntity_Success()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage;

            // act
            storage = new EntityPropertyStorage( entityMock );

            // assert
            Assert.IsNotNull( storage );
            Assert.IsInstanceOf<EntityPropertyStorage>( storage );
            Assert.AreEqual( entityMock, storage.Owner );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void EntityPropertyStorage_OwnerEntityNull_RaiseArgumentNullException()
        {
            // arrange
            IEntity entityNull = null;
            EntityPropertyStorage storage;

            // act
            storage = new EntityPropertyStorage( entityNull );

            // assert
        }

        [Test]
        public void StableBufferEnumerator_1000ValuesInBuffer_1000ValuesReturned()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            int times = 1000;
            for ( int i = 0; i < times; i++ )
            {
                storage.StableBufferStorage.Add( "test_" + i, new object() );
            }
            int enumCount = 0;

            // act
            foreach ( KeyValuePair<string, object> kvp in storage.StableBuffer )
            {
                enumCount++;
            }

            // assert
            Assert.AreEqual( times, enumCount );
        }

        [Test]
        public void UnstableBufferEnumerator_1000ValuesInBuffer_1000ValuesReturned()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            int times = 1000;
            for ( int i = 0; i < times; i++ )
            {
                storage.UnstableBufferStorage.Add( "test_" + i, new object() );
            }
            int enumCount = 0;

            // act
            foreach ( KeyValuePair<string, object> kvp in storage.UnstableBuffer )
            {
                enumCount++;
            }

            // assert
            Assert.AreEqual( times, enumCount );
        }

        [Test]
        public void DataEnumerator_1000ValuesInData_1000ValuesReturned()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            int times = 1000;
            for ( int i = 0; i < times; i++ )
            {
                storage.MainDataStorage.Add( "test_" + i, new object() );
            }
            int enumCount = 0;

            // act
            foreach ( KeyValuePair<string, object> kvp in storage.Data )
            {
                enumCount++;
            }


            // assert
            Assert.AreEqual( times, enumCount );
        }

        [Test, ExpectedException(typeof(BufferPublicationException))]
        public void Publicize_2SameKeys_RaiseBufferPublicationException()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            string key = "testData";
            object data = new object();
            int times = 2;

            // act
            for ( int i = times; i > 0; i-- )
            {
                storage.Publicize( key, data );
            }

            // assert
        }

        [Test]
        public void Publicize_1000ValidKeyValidData_1000Stored()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            int times = 1000;

            // act
            for(int i = 0; i < times; i++)
            {
                storage.Publicize( "test_" + i, new object() );
            }

            // assert
            Assert.AreEqual( times, storage.UnstableBufferStorage.Count );
        }

        [Test]
        public void SwapAndFlush_100ThenFlushThen30PublicationsFromSingleThreads_100ValuesToMainDataThen30Updates()
        {
            // arrange
            IEntity entityMock = this.mocks.NewMock<IEntity>();
            EntityPropertyStorage storage = new EntityPropertyStorage( entityMock );
            int times = 100;
            IList<Thread> threads = new List<Thread>();
            for ( int i = times; i > 0; i-- )
            {
                string key = "test_" + i;
                Thread t = new Thread( x => { Thread.Sleep( i * 16 ); storage.Publicize( key, new object() ); } );
                threads.Add( t );
                t.Start();
            }
            foreach ( Thread t in threads )
            {
                t.Join();
            }
            threads.Clear();
            Assert.AreEqual( times, storage.UnstableBufferStorage.Count );

            // act
            storage.SwapAndFlush();

            Assert.AreEqual( 0, storage.UnstableBufferStorage.Count );
            Assert.AreEqual( times, storage.MainDataStorage.Count );
            
            times = 30;
            for ( int i = times; i > 0; i-- )
            {
                if ( i == 15 )
                {
                    Thread swapT = new Thread
                    ( 
                        (object x) => 
                        {
                            Thread.Sleep( (int)x );
                            storage.SwapAndFlush();
                        }
                    );
                    threads.Add( swapT );
                    swapT.Start(i);       
                }
                string key = "test_" + i;
                Thread t = new Thread( x => { Thread.Sleep( i * 56 ); storage.Publicize( key, new object() ); } );
                threads.Add( t );
                t.Start();
            }
            foreach ( Thread t in threads )
            {
                t.Join();
            }
            threads.Clear();
            Assert.AreEqual( times, storage.UnstableBufferStorage.Count + storage.StableBufferStorage.Count );

            storage.SwapAndFlush();

            // assert
            Assert.AreEqual( 0, storage.UnstableBufferStorage.Count );            
            Assert.AreEqual( 100, storage.MainDataStorage.Count );
            
        }
    }
}
