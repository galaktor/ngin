/**************************************
 * FILE:          CoreModule.cs
 * DATE:          05.01.2010 10:07:56
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
using System.Xml;
using Autofac;
using Autofac.Builder;
using NGin.Core.Configuration.Serialization;
using NGin.Core.Logging;
using NGin.Core.Tasks;
using NGin.Core.States;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NGin.Core.Configuration.Modules
{
    internal class CoreModule: NGinModule
    {
        public INGinConfig CoreConfig { get; private set; }

        public CoreModule( INGinModuleConfig moduleConfig )
            : base( moduleConfig )
        { }        

        protected override void ConfigureContainer( IContainer container )
        {
            this.CoreConfig = container.Resolve<INGinConfig>();

            // create temporary builder for log manager
            ContainerBuilder builder = new ContainerBuilder();

            // register and build log manager first            
            this.ConfigureLogManager( builder );
            builder.Build( container );
            ILogManager logManager = container.Resolve<ILogManager>();

            // log message
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring module: {0}...", typeof( CoreModule ).FullName );

            // create new builder for other core components
            builder = new ContainerBuilder();

            // configure core services
            this.ConfigureConfigManager( builder, logManager );
            this.ConfigureDependencyResolver( builder, logManager );
            this.ConfigurePluginManager( builder, logManager );

            // build to container
            builder.Build( container );

            this.ConfigureCustomConfigDataTypes( container, logManager );

            base.ConfigureContainer( container );
        }

        private void ConfigureCustomConfigDataTypes( IContainer container, ILogManager logManager )
        {
            IPluginManager pluginManager = container.Resolve<IPluginManager>();
            IEnumerable<System.Attribute> plugins = pluginManager.GetPluginsForType( typeof( ConfigDataTypeAttribute ) );
            foreach ( Attribute att in plugins )
            {
                ConfigDataTypeAttribute configAtt = att as ConfigDataTypeAttribute;
                if(configAtt != null)
                {
                    this.CoreConfig.AddValidConfigDataTypeToAllModules( configAtt.ConfigDataType );
                }                
            }
            
        }        

        private void ConfigureDependencyResolver( ContainerBuilder builder, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering service {0} as {1}. Scope: {2}", typeof(DependencyResolver).FullName, typeof(IDependencyResolver).FullName, "Singleton" );
            builder.Register<IDependencyResolver>( context =>
                                                   {
                                                       INGinConfig coreConfig = context.Resolve<INGinConfig>();
                                                       return coreConfig.DependencyResolver;
                                                   }
                                                   ).SingletonScoped();
        }

        private void ConfigurePluginManager( ContainerBuilder builder, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering service {0} as {1}. Scope: {2}", typeof( PluginManager ).FullName, typeof( IPluginManager ).FullName, "Singleton" );
            builder.Register<IPluginManager>( context =>
                                              {
                                                  PluginsConfigXml pluginsConfig = this.ModuleConfig.GetSectionDataAs<PluginsConfigXml>( GlobalConstants.XmlConfiguration.SectionNames.PluginsSection );
                                                  ILogManager lm = context.Resolve<ILogManager>();
                                                  return new PluginManager( lm, pluginsConfig );
                                              }
                                             ).SingletonScoped();
        }

        private void ConfigureConfigManager( ContainerBuilder builder, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering service {0} as {1}. Scope: {2}", typeof( ConfigManager ).FullName, typeof( IConfigManager ).FullName, "Singleton" );
            builder.Register<ConfigManager>().As<IConfigManager>().SingletonScoped();
        }

        private void ConfigureLogManager( ContainerBuilder builder )
        {
            XmlElement log4netConfigXml = this.ModuleConfig.GetSectionDataAsXmlElement( "logging" );
            builder.Register<NGin.Core.Logging.Log4NetLogController>( context => { return new NGin.Core.Logging.Log4NetLogController( log4netConfigXml ); } ).As<NGin.Core.Logging.ILogController>().FactoryScoped();
            builder.Register<NGin.Core.Logging.LogManager>().As<NGin.Core.Logging.ILogManager>().SingletonScoped();
        }        
    }
}
