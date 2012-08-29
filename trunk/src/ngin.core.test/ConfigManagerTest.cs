/**************************************
 * FILE:          ConfigManagerTest.cs
 * DATE:          05.01.2010 10:22:32
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
using NMock2;
using NGin.Core.Logging;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class ConfigManagerTest
    {
        private Mockery mocks;

        private FileInfo testConfigFile = new FileInfo( "ngin.test.config" );

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
        public void ConfigManager_CreateInstance_Success()
        {
            // arrange
            ConfigManager manager;
            INGinConfig coreConfigMock = this.mocks.NewMock<INGinConfig>();
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();

            // act
            manager = new ConfigManager( coreConfigMock, logManagerMock );

            // assert
            Assert.IsNotNull( manager );
            Assert.IsInstanceOf<ConfigManager>( manager );
            Assert.AreSame( coreConfigMock, manager.Configuration );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void ConfigManager_ConfigNull_RaiseArgumentNullException()
        {
            // arrange
            ConfigManager manager;
            INGinConfig coreConfigNull = null;
            ILogManager logManagerMock = this.mocks.NewMock<ILogManager>();

            // act
            manager = new ConfigManager( coreConfigNull, logManagerMock );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void ConfigManager_LogManagerNull_RaiseArgumentNullExcpetion()
        {
            // arrange
            ConfigManager manager;
            INGinConfig coreConfigMock = this.mocks.NewMock<INGinConfig>(); ;
            ILogManager logManagerNull = null;

            // act
            manager = new ConfigManager( coreConfigMock, logManagerNull );

            // assert
        }
    }
}
