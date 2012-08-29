/**************************************
 * FILE:          NGinConfigTest.cs
 * DATE:          05.01.2010 10:23:46
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
using NGin.Core.Configuration;
using System.IO;
using System.Xml;
using NGin.Core.Exceptions;
using NGin.Core.Configuration.Serialization;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class NGinConfigTest
    {      
        private FileInfo testConfigFile = new FileInfo( "ngin.test.config" );

        [Test]
        public void CoreConfig_CreateInstanceValidFile_Success()
        {
            // arrange
            NGinConfig config;

            // act
            config = new NGinConfig( this.testConfigFile.FullName );

            // assert
            Assert.IsNotNull( config );
            Assert.IsInstanceOf<NGinConfig>( config );
        }

        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void NGinConfig_CreateInstanceInvalidFile_RaiseFileNotFoundException()
        {
            // arrange
            NGinConfig config;
            string invalidFileName = Path.GetFileName( Path.GetTempFileName() );

            // act
            config = new NGinConfig( invalidFileName );

            // assert
        }

        [Test]
        public void DeserializeConfigFile_ValidFile_Success()
        {
            // arrange
            NGinConfig config = new NGinConfig( this.testConfigFile.FullName );
            INGinConfigXml configXml = null;

            // act
            configXml = config.DeserializeConfigFile( this.testConfigFile.FullName );

            // assert
            Assert.IsNotNull( configXml );
            Assert.IsInstanceOf<INGinConfigXml>( configXml );
            Assert.IsNotNull( configXml.Modules );
            Assert.AreEqual( 2, configXml.Modules.Count );
        }

        [Test, ExpectedException( typeof( FileNotFoundException ) )]
        public void DeserializeConfigFile_FileDoesNotExist_RaiseFileNotFoundException()
        {
            // arrange
            NGinConfig config = new NGinConfig( this.testConfigFile.FullName );
            INGinConfigXml configXml = null;
            FileInfo fileNotExist = new FileInfo( "filenotexist.test.config" );
            // make sure file REALLY does not exist :-)
            Assert.IsFalse( fileNotExist.Exists );

            // act
            configXml = config.DeserializeConfigFile( fileNotExist.FullName );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void DeserializeConfigFile_FileIsNull_RaiseArgumentNullException()
        {
            // arrange
            NGinConfig config = new NGinConfig( this.testConfigFile.FullName );
            INGinConfigXml configXml = null;
            string fileNull = null;

            // act
            configXml = config.DeserializeConfigFile( fileNull );

            // assert
        }

        

        
    }
}
