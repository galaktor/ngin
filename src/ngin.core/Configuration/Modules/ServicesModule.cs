/**************************************
 * FILE:          ServicesModule.cs
 * DATE:          05.01.2010 10:08:39
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
using Autofac;
using Autofac.Builder;
using NGin.Core.Logging;
using System.Collections.Generic;
using NGin.Core.Scene;
using Autofac.Component;

namespace NGin.Core.Configuration.Modules
{
    internal class ServicesModule: NGinModule
    {
        public ServicesModule( INGinModuleConfig moduleConfig )
            : base( moduleConfig )
        { }

        protected override void ConfigureContainer( IContainer container )
        {            
            ILogManager logManager = container.Resolve<ILogManager>();
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring module: {0}...", typeof(ServicesModule).FullName );
            IPluginManager pluginManager = container.Resolve<IPluginManager>();

            ContainerBuilder builder = new ContainerBuilder();

            this.ConfigureServices( builder, pluginManager, logManager );
            this.ConfigureSystems( builder, pluginManager, logManager );            

            builder.Build( container );
            
            base.ConfigureContainer( container );
        }        

        private void ConfigureSystems( ContainerBuilder builder, IPluginManager pluginManager, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring systems..." );
            foreach ( Attribute att in pluginManager.GetPluginsForType( typeof( SystemAttribute ) ) )
            {
                SystemAttribute systemAtt = att as SystemAttribute;

                Type instanceType, interfaceType;
                instanceType = systemAtt.SystemType;
                interfaceType = ( systemAtt.InterfaceType == null ) ? systemAtt.SystemType : systemAtt.InterfaceType;

                logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering system {0} as {1}.", instanceType.FullName, interfaceType.FullName );
                builder.Register( instanceType ).As( interfaceType ).SingletonScoped();
            }
        }

        /// <summary>
        /// Retrieves all types marked as service and registers them with the builder.
        /// </summary>
        /// <param name="builder">The autofac builder to register the services with.</param>
        /// <param name="pluginManager">The plugin manager to retrieve the service plugin details from.</param>
        private void ConfigureServices( ContainerBuilder builder, IPluginManager pluginManager, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring services..." );
            foreach ( Attribute att in pluginManager.GetPluginsForType( typeof( ServiceAttribute ) ) )
            {
                ServiceAttribute serviceAtt = att as ServiceAttribute;

                Type instanceType, interfaceType;
                instanceType = serviceAtt.InstanceType;
                interfaceType = ( serviceAtt.InterfaceType == null ) ? serviceAtt.InstanceType : serviceAtt.InterfaceType;

                if ( serviceAtt.AsSingleton )
                {                    
                    logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering service {0} as {1}. Scope: {2}", instanceType.FullName, interfaceType.FullName, "Singleton" );
                    builder.Register( instanceType ).As( interfaceType ).SingletonScoped();
                }
                else
                {
                    logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering service {0} as {1}. Scope: {2}", instanceType.FullName, interfaceType.FullName, "Factory" );
                    builder.Register( instanceType ).As( interfaceType ).FactoryScoped();
                }
            }
        }        
    }
}
