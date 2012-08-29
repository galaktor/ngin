/**************************************
 * FILE:          DependenciesConfigXmlTest.cs
 * DATE:          05.01.2010 10:22:38
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
using NGin.Core.Configuration.Serialization;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class DependenciesConfigXmlTest
    {
        [Test]
        public void DependenciesConfigXml_CreateInstance_Success()
        {
            // arrange
            DependenciesConfigXml config;

            // act
            config = new DependenciesConfigXml();

            // assert
            Assert.IsNotNull( config );
            Assert.IsInstanceOf<DependenciesConfigXml>( config );
        }

        [Test]
        public void Equals_EqualObject_AreEqual()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dir );
            bool areEqual;

            // act
            areEqual = configOne.Equals( configTwo );

            // assert
            Assert.IsTrue( areEqual );
        }

        [Test]
        public void Equals_DifferentDependenciesLength_AreNotEqual()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dir );
            configTwo.Dependencies.Add( dir );
            bool areEqual;

            // act
            areEqual = configOne.Equals( configTwo );

            // assert
            Assert.IsFalse( areEqual );
        }

        [Test]
        public void Equals_DifferentDependencies_AreNotEqual()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            DirectoryXml dirOther = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            dirOther.Location = System.IO.Path.GetPathRoot( dir.Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dirOther );
            bool areEqual;

            // act
            areEqual = configOne.Equals( configTwo );

            // assert
            Assert.IsFalse( areEqual );
        }

        [Test]
        public void GetHashCode_EqualObjects_SameHashCodes()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dir );
            int hashCodeOne, hashCodeTwo;

            // act
            hashCodeOne = configOne.GetHashCode();
            hashCodeTwo = configTwo.GetHashCode();

            // assert
            Assert.AreEqual( hashCodeOne, hashCodeTwo );
        }

        [Test]
        public void GetHashCode_DifferentDependenciesCount_DifferentHashCodes()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dir );
            configTwo.Dependencies.Add( dir );
            int hashCodeOne, hashCodeTwo;

            // act
            hashCodeOne = configOne.GetHashCode();
            hashCodeTwo = configTwo.GetHashCode();

            // assert
            Assert.AreNotEqual( hashCodeOne, hashCodeTwo );
        }

        [Test]
        public void GetHashCode_DifferentDependencies_DifferentHashCodes()
        {
            // arrange
            DependenciesConfigXml configOne = new DependenciesConfigXml();
            DependenciesConfigXml configTwo = new DependenciesConfigXml();
            DirectoryXml dir = new DirectoryXml();
            DirectoryXml dirOther = new DirectoryXml();
            dir.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            dirOther.Location = System.IO.Path.GetPathRoot( dir.Location );
            configOne.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configOne.Dependencies.Add( dir );
            configTwo.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();
            configTwo.Dependencies.Add( dir );
            configTwo.Dependencies.Add( dirOther );
            int hashCodeOne, hashCodeTwo;

            // act
            hashCodeOne = configOne.GetHashCode();
            hashCodeTwo = configTwo.GetHashCode();

            // assert
            Assert.AreNotEqual( hashCodeOne, hashCodeTwo );
        }

        [Test]
        public void Equals_OtherType_AreNotEqual()
        {
            // arrange
            DependenciesConfigXml config = new DependenciesConfigXml();
            string other = "notaconfig";
            bool areEqual;

            // act
            areEqual = config.Equals( other );

            // assert
            Assert.IsFalse( areEqual );
        }

        [Test]
        public void IsInitialized_DependenciesInitialized_ReturnTrue()
        {
            // arrange
            DependenciesConfigXml config = new DependenciesConfigXml();
            config.Dependencies = new System.Collections.ObjectModel.Collection<DirectoryXml>();

            // act

            // assert
            Assert.IsTrue( config.IsInitialized );
        }

        [Test]
        public void IsInitialized_DependenciesNull_ReturnFalse()
        {
            // arrange
            DependenciesConfigXml config = new DependenciesConfigXml();
            config.Dependencies = null;

            // act

            // assert
            Assert.IsFalse( config.IsInitialized );
        }
    }
}
