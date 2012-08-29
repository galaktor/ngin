/**************************************
 * FILE:          DirectoryXmlTest.cs
 * DATE:          05.01.2010 10:22:56
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
using System.IO;
using System.Reflection;
using System.Xml;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class SectionXmlTest
    {
        private XmlElement xmlPlugins;
        private XmlElement xmlDependencies;
        private string xmlRawPluginsConfig = "<pluginconfig><plugins><file location='.' filename='ngin.core.dll' /></plugins></pluginconfig>";
        private string xmlRawDependenciesConfig = "<dependencyconfig><dependencies><directory location='dependencies' /></dependencies></dependencyconfig>";

        [SetUp]
        public void SetUp()
        {
            XmlDocument pluginsDoc = new XmlDocument();
            pluginsDoc.LoadXml( this.xmlRawPluginsConfig );
            this.xmlPlugins = pluginsDoc.DocumentElement;

            XmlDocument dependenciesDoc = new XmlDocument();
            dependenciesDoc.LoadXml( this.xmlRawDependenciesConfig );
            this.xmlDependencies = dependenciesDoc.DocumentElement;
        }

        [Test]
        public void SectionXml_CreateInstance_Success()
        {
            // arrange
            SectionXml section;

            // act
            section = new SectionXml();

            // assert
            Assert.IsNotNull( section );
            Assert.IsInstanceOf<SectionXml>( section );
        }

        [Test]
        public void Clone_CloneInstance_AreEqualExceptData()
        {
            // arrange
            SectionXml original = new SectionXml();
            original.Name = "plugins";
            original.XmlElement = this.xmlPlugins;
            original.DataTypeName = typeof( PluginsConfigXml ).FullName;
            original.Data = this.xmlPlugins;
            SectionXml clone = null;

            // act
            clone = original.Clone() as SectionXml;

            // assert
            Assert.IsNotNull(clone);
            Assert.IsNull(clone.Data);
            Assert.AreEqual( original, clone );
            Assert.AreNotSame( original, clone );
        }

        [Test]
        public void Equals_EqualSections_Success()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataTypeName = typeof( PluginsConfigXml ).FullName;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreEqual( original, other );
        }

        [Test]
        public void Equals_DifferentTypes_NotEqual()
        {
            // arrange
            SectionXml section = new SectionXml();
            object sectionInvalid = new object();

            // act

            // assert
            Assert.AreNotEqual( section, sectionInvalid );
        }
        [Test]
        public void Equals_DifferentDataTypes_NotEqual()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = original.DataType.FullName;
            other.DataType = typeof( object );
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreNotEqual( original, other );
        }
        [Test]
        public void Equals_TypesNull_Equal()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = null;
            original.DataTypeName = string.Empty;
            other.DataType = null;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreEqual( original, other );
        }
        [Test]
        public void Equals_DifferentTypeNames_NotEqual()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = Guid.NewGuid().ToString();
            other.DataType = original.DataType;
            other.DataTypeName = Guid.NewGuid().ToString();
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreNotEqual( original, other );
        }
        [Test]
        public void Equals_TypeNamesNull_Equal()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = null;
            other.DataType = original.DataType;
            other.DataTypeName = null;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreEqual( original, other );
        }
        [Test]
        public void Equals_DifferentName_NotEqual()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = "something else";
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = original.DataType.FullName;
            other.DataType = original.DataType;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreNotEqual( original, other );
        }
        [Test]
        public void Equals_NamesNull_Equal()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = null;
            other.Name = null;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = original.XmlElement;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = original.DataType.FullName;
            other.DataType = original.DataType;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreEqual( original, other );
        }
        [Test]
        public void Equals_DifferentXml_NotEqual()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = this.xmlPlugins;
            other.XmlElement = this.xmlDependencies;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = original.DataType.FullName;
            other.DataType = original.DataType;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreNotEqual( original, other );
        }
        [Test]
        public void Equals_XmlNull_AreEqual()
        {
            // arrange
            SectionXml original = new SectionXml();
            SectionXml other = new SectionXml();
            original.Name = "plugins";
            other.Name = original.Name;
            original.XmlElement = null;
            other.XmlElement = null;
            original.DataType = typeof( PluginsConfigXml );
            original.DataTypeName = original.DataType.FullName;
            other.DataType = original.DataType;
            other.DataTypeName = original.DataTypeName;
            original.Data = this.xmlPlugins;
            other.Data = original.Data;

            // act

            // assert
            Assert.AreEqual( original, other );
        }
        [Test]
        public void GetHashCode_EqualNames_SameHashCode()
        {
            // arrange
            string name = "rolf";
            SectionXml original = new SectionXml();
            original.Name = name;
            SectionXml other = new SectionXml();
            other.Name = name;

            // act

            // assert
            Assert.AreEqual( original.GetHashCode(), other.GetHashCode() );
        }
        [Test]
        public void GetHashCode_DifferentNames_DifferentHashCode()
        {
            // arrange
            SectionXml original = new SectionXml();
            original.Name = "rolf";
            SectionXml other = new SectionXml();
            other.Name = "bert";

            // act

            // assert
            Assert.AreNotEqual( original.GetHashCode(), other.GetHashCode() );
        }
    }

    [TestFixture]
    public class FileXmlTest
    {
        [Test]
        public void FileXml_CreateInstance_Success()
        {
            // arrange
            FileXml file;

            // act
            file = new FileXml();
            
            // assert
            Assert.IsNotNull( file );
            Assert.IsInstanceOf<FileXml>( file );
        }

        [Test]
        public void Equals_BothAreEqual_Success()
        {
            // arrange
            FileXml fileOne = new FileXml();
            FileXml fileTwo = new FileXml();
            fileOne.FileName = "something";
            fileTwo.FileName = fileOne.FileName;
            fileOne.Location = "";
            fileTwo.Location = fileOne.Location;

            // act

            // assert
            Assert.AreEqual( fileOne, fileTwo );
        }

        [Test]
        public void Equals_DifferentFileNames_NotEqual()
        {
            // arrange
            FileXml fileOne = new FileXml();
            FileXml fileTwo = new FileXml();
            fileOne.Location = "something";
            fileTwo.Location = "else";
            fileOne.FileName = "something";
            fileTwo.FileName = "else";

            // act

            // assert
            Assert.AreNotEqual( fileOne, fileTwo );
        }
        [Test]
        public void Equals_DifferentTypes_NotEqual()
        {
            // arrange
            FileXml fileOne = new FileXml();
            object fileTwoInvalid = new object();

            // act

            // assert
            Assert.AreNotEqual( fileOne, fileTwoInvalid );
        }

        [Test]
        public void Equals_BothFileNamesNull_AreEqual()
        {
            // arrange
            FileXml fileOne = new FileXml();
            FileXml fileTwo = new FileXml();
            fileOne.Location = "something";
            fileTwo.Location = "something";
            fileOne.FileName = null;
            fileTwo.FileName = null;

            // act

            // assert
            Assert.AreEqual( fileOne, fileTwo );
        }

        [Test]
        public void GetHashCode_EqualObjects_SameHashCode()
        {
            // arrange
            FileXml fileOne = new FileXml();
            FileXml fileTwo = new FileXml();
            fileOne.Location = "something";
            fileTwo.Location = "something";
            fileOne.FileName = "else";
            fileTwo.FileName= "else";

            // act

            // assert
            Assert.AreEqual( fileOne.GetHashCode(), fileTwo.GetHashCode() );
        }
        [Test]
        public void GetHashCode_DifferentFileNames_DifferentHashCode()
        {
            // arrange
            FileXml fileOne = new FileXml();
            FileXml fileTwo = new FileXml();
            fileOne.Location = "something";
            fileTwo.Location = "something";
            fileOne.FileName = "one";
            fileTwo.FileName = "two";

            // act

            // assert
            Assert.AreNotEqual( fileOne.GetHashCode(), fileTwo.GetHashCode() );
        }

        [Test]
        public void IsInitialized_FileNameLocationInitialized_Success()
        {
            // arrange
            FileXml file = new FileXml();
            file.FileName = "test";
            file.Location = "test";

            // act

            // assert
            Assert.IsTrue( file.IsInitialized );
        }

        [Test]
        public void IsInitialized_FileNameEmpty_NotInitialized()
        {
            // arrange
            FileXml file = new FileXml();
            file.FileName = string.Empty;
            file.Location = "test";

            // act

            // assert
            Assert.IsFalse( file.IsInitialized );
        }

        [Test]
        public void IsInitialized_FileNameNull_NotInitialized()
        {
            // arrange
            FileXml file = new FileXml();
            file.FileName = null;

            // act

            // assert
            Assert.IsFalse( file.IsInitialized );
        }
    }

    [TestFixture]
    public class DirectoryXmlTest
    {
        [Test]
        public void DirectoryXmlTest_CreateInstance_Success()
        {
            // arrange
            DirectoryXml directory;

            // act
            directory = new DirectoryXml();

            // assert
            Assert.IsNotNull( directory );
            Assert.IsInstanceOf<DirectoryXml>( directory );
        }

        [Test]
        public void IsInitialized_LocationInitialized_Success()
        {
            // arrange
            DirectoryXml directory = new DirectoryXml();
            directory.Location = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );

            // act

            // assert
            Assert.IsTrue( directory.IsInitialized );
        }

        [Test]
        public void IsInitialized_LocationNull_NotInitialized()
        {
            // arrange
            DirectoryXml directory = new DirectoryXml();
            directory.Location = null;

            // act

            // assert
            Assert.IsFalse( directory.IsInitialized );
        }

        [Test]
        public void GetHashCode_EqualObjects_SameHashcode()
        {
            // arrange
            string location = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            DirectoryXml directoryOne = new DirectoryXml();
            DirectoryXml directoryTwo = new DirectoryXml();
            directoryOne.Location = location;
            directoryTwo.Location = location;
            
            // act

            // assert
            Assert.IsTrue( directoryOne.Equals( directoryTwo ) );
        }

        [Test]
        public void GetHashCode_DifferentLocations_DIfferentHashcodes()
        {
            // arrange
            DirectoryXml dirOne = new DirectoryXml();
            DirectoryXml dirTwo = new DirectoryXml();
            dirOne.Location = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            dirTwo.Location = Path.GetPathRoot( dirOne.Location );

            // act

            // assert
            Assert.AreNotEqual( dirOne.GetHashCode(), dirTwo.GetHashCode() );
        }

        [Test]
        public void Equals_DifferentType_NotEqual()
        {
            // arrange
            DirectoryXml dirOne = new DirectoryXml();
            object dirTwoInvalid = new object();

            // act

            // assert
            Assert.AreNotEqual( dirOne, dirTwoInvalid );
        }

        [Test]
        public void Equals_AreEqual_Success()
        {
            // arrange
            DirectoryXml dirOne = new DirectoryXml();
            DirectoryXml dirTwo = new DirectoryXml();
            string location = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
            dirOne.Location = location;
            dirTwo.Location = location;

            // act

            // assert
            Assert.AreEqual( dirOne, dirTwo );
        }

        [Test]
        public void Equals_DifferentLocations_NotEqual()
       { 
            // arrange
            DirectoryXml dirOne = new DirectoryXml();
            DirectoryXml dirTwo = new DirectoryXml();
            dirOne.Location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            dirTwo.Location = Path.GetPathRoot( dirOne.Location );
           

            // act

            // assert
            Assert.AreNotEqual( dirOne, dirTwo );
        }
    }
}
