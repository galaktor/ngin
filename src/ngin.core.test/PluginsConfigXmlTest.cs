/**************************************
 * FILE:          PluginsConfigXmlTest.cs
 * DATE:          05.01.2010 10:24:23
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
    public class PluginsConfigXmlTest
    {
        [Test]
        public void PluginsConfigXml_CreateInstance_Success()
        {
            // arrange
            PluginsConfigXml config;

            // act
            config = new PluginsConfigXml();

            // assert
            Assert.IsNotNull( config );
            Assert.IsInstanceOf<PluginsConfigXml>( config );
        }

        [Test]
        public void Equals_EqualObject_AreEqual()
        {
            // arrange
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( file );
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
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( file );
            configTwo.PlugIns.Add( file );
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
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            FileXml dirOther = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            dirOther.Location = System.IO.Path.GetPathRoot( file.Location );
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( dirOther );
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
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            file.FileName = "abcdef";
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( file );
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
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            file.FileName = "abc";
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( file );
            configTwo.PlugIns.Add( file );
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
            PluginsConfigXml configOne = new PluginsConfigXml();
            PluginsConfigXml configTwo = new PluginsConfigXml();
            FileXml file = new FileXml();
            FileXml dirOther = new FileXml();
            file.Location = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            file.FileName = "abcd";
            dirOther.Location = System.IO.Path.GetPathRoot( file.Location );
            dirOther.FileName = "xyz";
            configOne.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configOne.PlugIns.Add( file );
            configTwo.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            configTwo.PlugIns.Add( file );
            configTwo.PlugIns.Add( dirOther );
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
            PluginsConfigXml config = new PluginsConfigXml();
            string other = "notaconfig";
            bool areEqual;

            // act
            areEqual = config.Equals( other );

            // assert
            Assert.IsFalse( areEqual );
        }

        [Test]
        public void IsInitialized_PlugInsInitialized_ReturnTrue()
        {
            // arrange
            PluginsConfigXml config = new PluginsConfigXml();
            config.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();

            // act

            // assert
            Assert.IsTrue( config.IsInitialized );
        }

        [Test]
        public void IsInitialized_PlugInsNull_ReturnFalse()
        {
            // arrange
            PluginsConfigXml config = new PluginsConfigXml();
            config.PlugIns = null;

            // act

            // assert
            Assert.IsFalse( config.IsInitialized );
        }
    }
}
