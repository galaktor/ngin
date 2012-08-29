/**************************************
 * FILE:          ConfigBaseTest.cs
 * DATE:          05.01.2010 10:22:21
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
using NUnit.Framework;
using NGin.Core.Configuration;
using NGin.Core.Exceptions;
using NGin.Core.Configuration.Serialization;
using System.IO;
using System.Xml;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class ConfigBaseTest
    {
        private XmlElement xmlPluginsConfig;
        private XmlElement xmlDependenciesConfig;
        private XmlElement xmlRandom;
        private string xmlRawPluginsConfig = "<pluginconfig><plugins><file location='.' filename='ngin.core.dll' /></plugins></pluginconfig>";
        private string xmlRawDependenciesConfig = "<dependencyconfig><dependencies><directory location='dependencies' /></dependencies></dependencyconfig>";
        private string xmlRawRandomXml = "<something><went /><wrong />Bro</something>";

        private FileInfo testConfigFile = new FileInfo( "ngin.test.config" );

        private class ConfigBaseTestImpl : ConfigBase
        {

        }

        [Test]
        public void DeserializeXmlAs_ValidTypeNameValidXml_Success()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( PluginsConfigXml );
            string xmlRaw = this.xmlRawPluginsConfig;
            object result = null;

            // act
            result = config.DeserializeXmlAs( expectedType.FullName, xmlRaw );

            // assert
            Assert.IsNotNull( result );
            Assert.IsInstanceOf( expectedType, result );
        }

        [Test]
        public void DeserializeXmlAs_ValidTypeValidXmlBytes_Success()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( PluginsConfigXml );
            byte[] xmlRawBytes = System.Text.Encoding.UTF8.GetBytes( this.xmlRawPluginsConfig );
            object result = null;

            // act
            result = config.DeserializeXmlAs( expectedType.FullName, xmlRawBytes );

            // assert
            Assert.IsNotNull( result );
            Assert.IsInstanceOf( expectedType, result );
        }

        [Test, ExpectedException( typeof( CoreConfigException ) )]
        public void DeserializeXmlAs_ValidTypeInvalidBytes_RaiseCoreConfigException()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( PluginsConfigXml );
            byte[] xmlRawBytes = Guid.NewGuid().ToByteArray();
            object result = null;

            // act
            result = config.DeserializeXmlAs( expectedType.FullName, xmlRawBytes );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void DeserializeXmlAs_TypeNameNull_RaiseArgumentNulLException()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( PluginsConfigXml );
            string typeName = null;
            byte[] xmlRawBytes = System.Text.Encoding.UTF8.GetBytes( this.xmlRawPluginsConfig );
            object result = null;


            // act
            result = config.DeserializeXmlAs( typeName, xmlRawBytes );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void DeserializeXmlAs_ValidTypeBytesNull_RaiseArgumentNullException()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( PluginsConfigXml );
            byte[] xmlRawBytes = null;
            object result = null;

            // act
            result = config.DeserializeXmlAs( expectedType.FullName, xmlRawBytes );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void DeserializeXmlAs_TypeNullValidBytes_RaiseArgumentNullException()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type typeNull = null;
            byte[] xmlRawBytes = System.Text.Encoding.UTF8.GetBytes( this.xmlRawPluginsConfig );
            object result = null;

            // act
            result = config.DeserializeXmlAs( typeNull, xmlRawBytes );

            // assert
        }

        [Test, ExpectedException( typeof( PluginNotFoundException ) )]
        public void DeserializeXmlAs_InvalidTypeValidBytes_RaisePluginNotFoundException()
        {
            // arrange
            ConfigBase config = new ConfigBaseTestImpl();
            Type expectedType = typeof( ThreadStaticAttribute );
            byte[] xmlRawBytes = System.Text.Encoding.UTF8.GetBytes( this.xmlRawPluginsConfig );
            object result = null;

            // act
            result = config.DeserializeXmlAs( expectedType.FullName, xmlRawBytes );

            // assert
        }
    }
}
