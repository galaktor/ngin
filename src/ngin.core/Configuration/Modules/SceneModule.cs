/**************************************
 * FILE:          SceneModule.cs
 * DATE:          05.01.2010 10:08:30
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
using NGin.Core.Logging;
using Autofac.Builder;
using Autofac;
using NGin.Core.Scene;

namespace NGin.Core.Configuration.Modules
{
    internal class SceneModule: NGinModule
    {
        public SceneModule( INGinModuleConfig moduleConfig )
            : base( moduleConfig )
        { }

        protected override void ConfigureContainer( IContainer container )
        {            
            ILogManager logManager = container.Resolve<ILogManager>();
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring module: {0}...", typeof(SceneModule).FullName );
            IPluginManager pluginManager = container.Resolve<IPluginManager>();

            ContainerBuilder builder = new ContainerBuilder();
            
            this.ConfigureEntityExtensions( builder, pluginManager, logManager );
            this.ConfigureScenes( builder, pluginManager, logManager );

            builder.Build( container );
            
            base.ConfigureContainer( container );
        }

        private void ConfigureScenes( ContainerBuilder builder, IPluginManager pluginManager, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring scenes..." );

            builder.Register<IScene>( ( context ) =>
            {
                ISceneManager sceneManager = context.Resolve<ISceneManager>();
                return sceneManager.CreateAndAddScene();
            } );
        }

        private void ConfigureEntityExtensions( ContainerBuilder builder, IPluginManager pluginManager, ILogManager logManager )
        {
            logManager.Trace( Namespace.LoggerName, LogLevel.Information, "Configuring entity extensions..." );

            try
            {
                IEnumerable<Attribute> plugins = pluginManager.GetPluginsForType( typeof( EntityExtensionAttribute ) );
                foreach ( Attribute att in plugins )
                {
                    EntityExtensionAttribute extAtt = att as EntityExtensionAttribute;

                    Type instanceType, interfaceType;
                    instanceType = extAtt.EntityExtensionType;
                    interfaceType = ( extAtt.InterfaceType == null ) ? extAtt.EntityExtensionType : extAtt.InterfaceType;

                    logManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "Registering entity extension {0} as {1}.", instanceType.FullName, interfaceType.FullName );
                    builder.Register( instanceType ).As( interfaceType).FactoryScoped();
                }
            }
            catch ( NGin.Core.Exceptions.PluginNotFoundException )
            {
                logManager.Trace( Namespace.LoggerName, LogLevel.Error, "No entity extensions registerred." );
            }
        }
    }
}
