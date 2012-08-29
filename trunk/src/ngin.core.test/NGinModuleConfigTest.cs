/**************************************
 * FILE:          NGinModuleConfigTest.cs
 * DATE:          05.01.2010 10:23:57
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
using NGin.Core.Exceptions;
using NGin.Core.Configuration;
using System.IO;
using System.Xml;
using NGin.Core.Configuration.Serialization;
using NMock2;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class NGinModuleConfigTest
    {
        private Mockery mocks;

        private XmlElement xmlPluginsConfig;
        private XmlElement xmlDependenciesConfig;
        private XmlElement xmlRandom;
        private string xmlRawPluginsConfig = "<pluginconfig><plugins><file location='.' filename='ngin.core.dll' /></plugins></pluginconfig>";
        private string xmlRawDependenciesConfig = "<dependencyconfig><dependencies><directory location='dependencies' /></dependencies></dependencyconfig>";
        private string xmlRawRandomXml = "<something><went /><wrong />Bro</something>";

        [SetUp]
        public void SetUp()
        {
            this.mocks = new Mockery();

            XmlDocument xmlDocPlugins = new XmlDocument();
            xmlDocPlugins.LoadXml( this.xmlRawPluginsConfig );
            this.xmlPluginsConfig = xmlDocPlugins.DocumentElement;

            XmlDocument xmlDocDependencies = new XmlDocument();
            xmlDocDependencies.LoadXml( this.xmlRawDependenciesConfig );
            this.xmlDependenciesConfig = xmlDocDependencies.DocumentElement;

            XmlDocument xmlDocRandom = new XmlDocument();
            xmlDocRandom.LoadXml( this.xmlRawRandomXml );
            this.xmlRandom = xmlDocRandom.DocumentElement;
        }

        [TearDown]
        public void TearDown()
        {
            this.mocks.VerifyAllExpectationsHaveBeenMet();
            this.mocks.Dispose();
            this.mocks = null;
        }

        private ISectionXml CreateSectionMock(string sectioName, XmlElement sectionDataXml, string sectionTypeName )
        {
            SectionXml result = new SectionXml();
            result.Name = sectioName;
            result.XmlElement = sectionDataXml;
            result.DataTypeName = sectionTypeName;

            return result;
        }

        private IModuleXml CreateModuleMock( string moduleName, string typeFullName, params ISectionXml[] sections )
        {
            ModuleXml result = new ModuleXml();

            // stub properties
            result.Name = moduleName;
            result.Type = typeFullName;
            result.Sections = new System.Collections.ObjectModel.Collection<SectionXml>();
           
            // stub sections
            foreach ( ISectionXml section in sections )
            {
                result.Sections.Add( section as SectionXml );
            }

            return result;
        }

        [Test]
        public void InsertDataSectionIntoSectionXml_DefaultDataTypeXmlElement_Success()
        {
            // arrange
            ISectionXml testSection = this.CreateSectionMock( "testSection", this.xmlPluginsConfig, typeof(XmlElement).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            Type expectedType = typeof( XmlElement );
            Type actualType = null;

            // act
            actualType = config.InsertDataIntoSectionXml( testSection as SectionXml );

            // assert
            Assert.AreEqual( expectedType, actualType );
            Assert.IsNotNull( testSection.Data );
            Assert.IsInstanceOf( expectedType, testSection.Data );
            Assert.AreEqual( testSection.XmlElement, testSection.Data );
            Assert.AreEqual( testSection.XmlRaw, ( ( XmlElement ) testSection.Data ).OuterXml );
        }

        [Test]
        public void InsertDataSectionIntoSectionXml_DataTypePluginsConfig_Success()
        {
            // arrange
            Type expectedType = typeof( PluginsConfigXml );
            Type actualType = null;
            SectionXml testSection = new SectionXml();
            testSection.Name = "testSection";
            testSection.XmlElement = this.xmlPluginsConfig;
            testSection.DataTypeName = expectedType.FullName;
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );

            // act
            actualType = config.InsertDataIntoSectionXml( testSection );

            // assert
            Assert.AreEqual( expectedType, actualType );
            Assert.IsNotNull( testSection.Data );
            Assert.IsInstanceOf( expectedType, testSection.Data );
            Assert.IsTrue( ( ( PluginsConfigXml ) testSection.Data ).IsInitialized );
        }

        [Test]
        public void InsertDataSectionIntoSectionXml_DataTypeDependenciesConfig_Success()
        {
            // arrange
            Type expectedType = typeof( DependenciesConfigXml );
            Type actualType = null;
            SectionXml testSection = new SectionXml();
            testSection.Name = "testSection";
            testSection.XmlElement = this.xmlDependenciesConfig;
            testSection.DataTypeName = expectedType.FullName;
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            
            // act
            actualType = config.InsertDataIntoSectionXml( testSection );

            // assert
            Assert.AreEqual( expectedType, actualType );
            Assert.IsNotNull( testSection.Data );
            Assert.IsInstanceOf( expectedType, testSection.Data );
            Assert.IsTrue( ( ( DependenciesConfigXml ) testSection.Data ).IsInitialized );
        }

        [Test, ExpectedException( typeof( PluginNotFoundException ) )]
        public void InsertDataSectionIntoSectionXml_DataTypeNotDefined_RaisePluginNotFoundException()
        {
            // arrange
            Type expectedType = typeof( DependenciesConfigXml );
            Type actualType = null;
            SectionXml testSection = new SectionXml();
            testSection.Name = "testSection";
            testSection.XmlElement = this.xmlDependenciesConfig;
            // define invalid type name
            testSection.DataTypeName = "Something.Went.Wrong.ConfigType";
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
           
            // act
            actualType = config.InsertDataIntoSectionXml( testSection );

            // assert
        }

        [Test, ExpectedException( typeof( CoreConfigException ) )]
        public void InsertDataSectionIntoSectionXml_XmlDoesNotMatchType_RaiseCoreConfigException()
        {
            // arrange
            Type expectedType = typeof( DependenciesConfigXml );
            Type actualType = null;
            SectionXml testSection = new SectionXml();
            testSection.Name = "testSection";
            // fill in xml that does not match type
            testSection.XmlElement = this.xmlRandom;
            testSection.DataTypeName = expectedType.FullName;
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );

            // act
            actualType = config.InsertDataIntoSectionXml( testSection );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void InsertDataSectionIntoSectionXml_SectionNull_RaiseArgumentNullException()
        {
            // arrange
            Type actualType = null;
            SectionXml testSection = null;
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );

            // act
            actualType = config.InsertDataIntoSectionXml( testSection );

            // assert
        }

        [Test]
        public void GetSectionCloned_GetSameSectionTwice_ResultNotSameRef()
        {
            // arrange
            Type expectedType = typeof( DependenciesConfigXml );
            SectionXml testSection = new SectionXml();
            testSection.Name = "testSection";
            // fill in xml that does not match type
            testSection.XmlElement = this.xmlDependenciesConfig;
            testSection.DataTypeName = expectedType.FullName;
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            ISectionXml first = null;
            ISectionXml second = null;

            // act
            first = config.GetSectionCloned( testSection.Name );
            second = config.GetSectionCloned( testSection.Name );

            // assert
            Assert.IsNotNull( first );
            Assert.IsNotNull( second );
            Assert.AreNotSame( first, second );
            Assert.AreEqual( first, second );
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void GetSectionCloned_SectionNotExist_RaiseArgumentException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string invalidSectionName = Guid.NewGuid().ToString();
            ISectionXml section = null;

            // act
            section = config.GetSectionCloned( invalidSectionName );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void GetSectionCloned_SectionNull_RaiseArgumentNullException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionNameNull = null;
            ISectionXml section = null;

            // act
            section = config.GetSectionCloned( sectionNameNull );

            // assert
        }

        [Test]
        public void CloneSectionWithData_ValidSection_DataValid()
        {
            // arrange
            ISectionXml testSection = this.CreateSectionMock( "plugins", this.xmlPluginsConfig, typeof( PluginsConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            ISectionXml section = config.GetSectionCloned( "plugins" );
            ISectionXml clone = null;

            // act
            clone = config.CloneSectionWithData( section );


            // assert
            Assert.IsNotNull( clone );
            Assert.IsInstanceOf<ISectionXml>( clone );
            Assert.AreNotSame( section, clone );
            Assert.AreEqual( section, clone );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void CloneSectionWithData_SectionNull_RaiseArgumentNullException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            ISectionXml sectionNull = null;

            // act
            config.CloneSectionWithData( sectionNull );

            // assert
        }

        [Test]
        public void GetSectionData_ValidSection_DataValid()
        {
            // arrange
            Type expectedType = typeof( PluginsConfigXml );
            ISectionXml testSection = this.CreateSectionMock( "plugins", this.xmlPluginsConfig, expectedType.FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = "plugins";
            ISectionXml expectedSection = config.GetSectionCloned( sectionName );
            object data = null;

            // act
            data = config.GetSectionData( sectionName );

            // assert
            Assert.IsNotNull( data );
            Assert.IsInstanceOf( expectedType, data );
            Assert.AreEqual( expectedSection.Data, data );
            Assert.AreEqual( expectedSection.DataType, data.GetType() );
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void GetSectionData_UnknownSection_RaiseArgumentException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = Guid.NewGuid().ToString();
            object data = null;

            // act
            data = config.GetSectionData( sectionName );

            // assert
        }

        [Test]
        public void GetSectionDataAsRawXml_ValidSection_Sucess()
        {
            // arrange
            ISectionXml testSection = this.CreateSectionMock( "plugins", this.xmlPluginsConfig, typeof( PluginsConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string expectedXml = config.GetSectionCloned( "plugins" ).XmlRaw;
            string resultXml = null;

            // act
            resultXml = config.GetSectionDataAsRawXml( "plugins" );

            // assert
            Assert.IsNotNull( resultXml );
            Assert.AreEqual( expectedXml, resultXml );
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void GetSectionDataAsRawXml_UnknownSection_RaiseArgumentException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = Guid.NewGuid().ToString();
            string resultXml = null;

            // act
            resultXml = config.GetSectionDataAsRawXml( sectionName );

            // assert            
        }

        [Test]
        public void GetSectionDataAsXmlElement_ValidSection_Success()
        {
            // arrange                       
            string sectionName = "dependencies";
            ISectionXml testSection = this.CreateSectionMock( sectionName, this.xmlDependenciesConfig, typeof( DependenciesConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            XmlElement expectedXml = config.GetSectionCloned( sectionName ).XmlElement;
            XmlElement resultXml = null;

            // act
            resultXml = config.GetSectionDataAsXmlElement( sectionName );

            // assert
            Assert.IsNotNull( resultXml );
            Assert.AreEqual( expectedXml, resultXml );
        }

        [Test]
        public void GetSectionDataAs_XmlElement_Success()
        {
            // arrange
            string sectionName = "dependencies";
            ISectionXml testSection = this.CreateSectionMock( sectionName, this.xmlDependenciesConfig, typeof( DependenciesConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            XmlElement expectedXml = config.GetSectionCloned( sectionName ).XmlElement;
            XmlElement resultXml = null;

            // act
            resultXml = config.GetSectionDataAs( typeof( XmlElement ), sectionName ) as XmlElement;

            // assert
            Assert.IsNotNull( resultXml );
            Assert.AreEqual( expectedXml, resultXml );
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void GetSectionDataAsXmlElement_InvalidSection_RaiseArgumentException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = Guid.NewGuid().ToString();
            XmlElement resultXml = null;

            // act
            resultXml = config.GetSectionDataAsXmlElement( sectionName );

            // assert
        }

        [Test]
        public void GetSectionDataAs_ValidSectionString_GetDataRawXml()
        {
            // arrange
            string sectionName = "plugins";
            ISectionXml testSection = this.CreateSectionMock( sectionName, this.xmlPluginsConfig, typeof( PluginsConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            
            string expectedXml = config.GetSectionCloned( sectionName ).XmlRaw;
            string resultData = null;

            // act
            resultData = config.GetSectionDataAs( typeof( string ), sectionName ) as string;

            // assert
            Assert.IsNotNull( resultData );
            Assert.AreEqual( expectedXml, resultData );
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void GetSectionDataAs_InvalidSectionString_RaiseArgumentException()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = Guid.NewGuid().ToString();
            string resultData = null;

            // act
            resultData = config.GetSectionDataAs( typeof( string ), sectionName ) as string;

            // assert            
        }

        [Test, ExpectedException( typeof( PluginNotFoundException ) )]
        public void GetSectionDataAs_InvalidConfigDataType_RaisePluginNotFoundException()
        {
            // arrange
            ISectionXml testSection = this.CreateSectionMock( "plugins", this.xmlPluginsConfig, typeof( PluginsConfigXml ).FullName );
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule", testSection );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = "plugins";
            Type invalidType = typeof( MemoryStream );

            // act
            config.GetSectionDataAs( invalidType, sectionName );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void GetSectionDataAs_ConfigDataTypeNull_RaiseArgumentNullExcpetion()
        {
            // arrange
            IModuleXml moduleMock = this.CreateModuleMock( "testModule", "NGin.Core.Test.NGinModuleConfigTest.TestModule" );
            NGinModuleConfig config = new NGinModuleConfig( moduleMock );
            string sectionName = "plugins";
            Type typeNull = null;

            // act
            config.GetSectionDataAs( typeNull, sectionName );

            // assert
        }
    }
}
