/**************************************
 * FILE:          PluginManagerTest.cs
 * DATE:          05.01.2010 10:24:17
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
using NMock2;
using NGin.Core.Logging;
using NGin.Core.Configuration;
using NGin.Core.Configuration.Serialization;
using System.IO;
using System.Collections;
using System.Reflection;

namespace NGin.Core.Test
{
    [TestFixture]
    public class PluginManagerTest
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

        private ILogManager CreateLogManagerMock()
        {
            ILogManager result = this.mocks.NewMock<ILogManager>();

            Stub.On( result ).Method( "Trace" ).WithAnyArguments();

            return result;
        }

        private PluginsConfigXml CreatePluginsConfigMock( params string[] pluginFilePaths )
        {
            PluginsConfigXml result = new PluginsConfigXml();

            result.PlugIns = new System.Collections.ObjectModel.Collection<FileXml>();
            foreach ( string filePath in pluginFilePaths )
            {
                FileXml fileXml = new FileXml();
                fileXml.FileName = Path.GetFileName( filePath );
                fileXml.Location = Path.GetDirectoryName( filePath );
                result.PlugIns.Add( fileXml );
            }

            return result;
        }

        [Test]
        public void PluginManager_ValidLogManagerValidConfigNoPlugins_Success()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml configMock = this.CreatePluginsConfigMock();
            PluginManager pm = null;

            // act
            pm = new PluginManager( lmMock, configMock );

            // assert
            Assert.IsNotNull( pm );
            Assert.IsInstanceOf<PluginManager>( pm );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void PluginManager_LogManagerNullValidConfigNoPlugins_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmNull = null;
            PluginsConfigXml configMock = this.CreatePluginsConfigMock();
            PluginManager pm = null;

            // act
            pm = new PluginManager( lmNull, configMock );

            // assert
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void PluginManager_ValidLogManagerConfigNull_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml configNull = null;
            PluginManager pm = null;

            // act
            pm = new PluginManager( lmMock, configNull );

            // assert
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void GetPluginAssemblies_ConfigPluginsListNull_RaiseInvalidOperationException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            PluginsConfigXml configInvalid = this.CreatePluginsConfigMock();
            configInvalid.PlugIns = null;

            // act
            pm.GetPluginAssemblies( configInvalid );

            // assert
        }

        [Test]
        public void GetPluginAssemblies_ValidConfigNoPlugins_0Assemblies()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Assembly> assemblies = null;

            // act
            assemblies = pm.GetPluginAssemblies( config );

            // assert
            Assert.IsNotNull(assemblies);
            Assert.AreEqual( 0, assemblies.Count );
        }

        [Test]
        public void GetPluginAssemblies_ValidConfig2Plugins_2Assemblies()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();            
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );            
            string pluginOnePath = Assembly.GetExecutingAssembly().Location;
            string pluginTwoPath = Assembly.GetExecutingAssembly().Location;
            PluginsConfigXml config2Plugins = this.CreatePluginsConfigMock( pluginOnePath, pluginTwoPath );
            IList<Assembly> assemblies = null;

            // act
            assemblies = pm.GetPluginAssemblies( config2Plugins );

            // assert
            Assert.IsNotNull( assemblies );
            Assert.AreEqual( 2, assemblies.Count );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetPluginAssemblies_ConfigNull_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            PluginsConfigXml configNull = null;

            // act
            pm.GetPluginAssemblies( configNull );

            // assert
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void GetPluggables_ValidListOneEntryIsNull_RaiseInvalidOperationException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            Assembly testAssembly = null;
            IList<Assembly> assemblyList = new List<Assembly>();
            assemblyList.Add( testAssembly );
            IList<Type> pluggables = null;

            // act
            pm.GetPluggables( assemblyList );

            // assert
        }

        [Test]
        public void GetPluggables_ValidListTestAssembly_2Pluggables()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            Assembly testAssembly = Assembly.GetExecutingAssembly();
            IList<Assembly> assemblyList = new List<Assembly>();
            assemblyList.Add( testAssembly );
            IList<Type> pluggables = null;

            // act
            pluggables = pm.GetPluggables( assemblyList );

            // assert
            Assert.IsNotNull( pluggables );
            Assert.AreEqual( 2, pluggables.Count );
            Assert.AreEqual( typeof(TestPluggableOne), pluggables[0] );
            Assert.AreEqual( typeof(TestPluggableTwo), pluggables[1] );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetPluggables_ListNull_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Assembly> assemblyList = null;

            // act
            pm.GetPluggables( assemblyList );

            // assert
        }

        [Test]
        public void GetPlugins_ValidPluggablesEmptyValidAssembliesEmpty_0Plugins()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Type> pluggables = new List<Type>();
            IList<Assembly> pluginAssemblies = new List<Assembly>();
            Dictionary<Type, IList<Attribute>> plugins = null;

            // act
            plugins = pm.GetPlugins( pluggables, pluginAssemblies );

            // assert
            Assert.IsNotNull( plugins );
            Assert.AreEqual( 0, plugins.Count );
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetPlugins_PluggablesNullValidAssembliesEmpty_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Type> pluggables = null;
            IList<Assembly> pluginAssemblies = new List<Assembly>();
            Dictionary<Type, IList<Attribute>> plugins = null;

            // act
            plugins = pm.GetPlugins( pluggables, pluginAssemblies );

            // assert
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void GetPlugins_ValidPluggablesEmptyAssembliesNull_RaiseArgumentNullException()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Type> pluggables = new List<Type>();
            IList<Assembly> pluginAssemblies = null;
            Dictionary<Type, IList<Attribute>> plugins = null;

            // act
            plugins = pm.GetPlugins( pluggables, pluginAssemblies );

            // assert
        }

        [Test]
        public void GetPlugins_TwoPluggablesOneAssembly_2PluginTypes2TestPluggableOne1TestPlugggableTwo()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();            
            PluginManager pm = new PluginManager( lmMock, config );
            IList<Type> pluggables = new List<Type>() { typeof( TestPluggableOne ), typeof( TestPluggableTwo ) };
            IList<Assembly> pluginAssemblies = new List<Assembly>() { Assembly.GetExecutingAssembly() };
            Dictionary<Type, IList<Attribute>> plugins = null;

            // act
            plugins = pm.GetPlugins( pluggables, pluginAssemblies );

            // assert
            Assert.IsNotNull( plugins );
            Assert.AreEqual( 2, plugins.Count );
            Assert.AreEqual( 2, plugins[ typeof( TestPluggableOne ) ].Count );
            Assert.AreEqual( 1, plugins[ typeof( TestPluggableTwo ) ].Count );
        }

        [Test]
        public void GetPluginsForType_ValidExistentPluginType2Results_Returns2Results()
        {
            // arrange
            ILogManager lmMock = this.CreateLogManagerMock();
            PluginsConfigXml config = this.CreatePluginsConfigMock();
            FileXml pluginFile = new FileXml();
            pluginFile.FileName = Path.GetFileName( Assembly.GetExecutingAssembly().Location );
            pluginFile.Location = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            config.PlugIns.Add( pluginFile );
            PluginManager pm = new PluginManager( lmMock, config );            
            IEnumerable<Attribute> pluginsOne = null;
            IEnumerable<Attribute> pluginsTwo = null;

            // act
            pluginsOne = pm.GetPluginsForType( typeof( TestPluggableOne ) );
            pluginsTwo = pm.GetPluginsForType( typeof( TestPluggableTwo ) );

            // assert
            Assert.IsNotNull( pluginsOne );
            Assert.AreEqual( 2, pluginsOne.Count<Attribute>() );
            Assert.IsNotNull( pluginsTwo );
            Assert.AreEqual( 1, pluginsTwo.Count<Attribute>() );
        }
        
    }

    [Pluggable( typeof( TestPluggableOne ) )]
    public class TestPluggableOne : Attribute
    { }

    [Pluggable( typeof( TestPluggableTwo ) )]
    public class TestPluggableTwo : Attribute
    { }

    [TestPluggableOne]
    class TestPluginOne
    { }

    [TestPluggableOne]
    class TestPluginTwo
    { }

    [TestPluggableTwo]
    class TestPluginThree
    { }
}
