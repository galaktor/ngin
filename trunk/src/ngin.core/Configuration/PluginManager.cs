/**************************************
 * FILE:          PluginManager.cs
 * DATE:          05.01.2010 10:13:55
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NGin.Core.Configuration.Serialization;
using NGin.Core.Exceptions;
using NGin.Core.Logging;
using NGin.Core.Platform;

namespace NGin.Core.Configuration
{
    public class PluginManager: IPluginManager
    {
        protected internal ILogManager LogManager { get; private set; }
        protected internal PluginsConfigXml PluginsConfig { get; private set; }

        private IList<Assembly> pluginAssemblies = null;
        private IList<Type> pluggables = null;
        private Dictionary<Type, IList<Attribute>> plugins = null;

        public PluginManager( ILogManager logManager, PluginsConfigXml pluginsConfig )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "logManager", "The given log manager must not be null." );
            }

            this.LogManager = logManager;

            if ( pluginsConfig == null )
            {
                string message = "The given plugins configuration must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "pluginsConfig", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            this.PluginsConfig = pluginsConfig;

            this.pluginAssemblies = this.GetPluginAssemblies( pluginsConfig );
            this.pluggables = this.GetPluggables( this.pluginAssemblies );
            this.plugins = this.GetPlugins( this.pluggables, this.pluginAssemblies );
        }

        internal IList<Assembly> GetPluginAssemblies( PluginsConfigXml pluginsConfig )
        {
            if ( pluginsConfig == null )
            {
                string message = "The given plugins config must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "pluginsConfig", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            // check if configuration is valid
            if ( !pluginsConfig.IsInitialized )
            {
                string message = "The given configuration is invalid.";
                InvalidOperationException iopEx = new InvalidOperationException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, iopEx );
                throw iopEx;
            }

            List<Assembly> result = new List<Assembly>();

            foreach ( FileXml pluginDef in pluginsConfig.PlugIns )
            {
                FileInfo pluginFile = new FileInfo( Path.Combine( pluginDef.Location, pluginDef.FileName ) );
                Assembly pluginAssembly = InputOutputManager.LoadAssembly( pluginFile );

                // Idea: should duplicate assemblies be avoided to avoid redundant searching?
                result.Add( pluginAssembly );
            }

            return result;
        }

        internal IList<Type> GetPluggables( IList<Assembly> assemblyPlugins )
        {
            if ( assemblyPlugins == null )
            {
                string message = "The given assembly list must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "assemblyPlugins", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            IList<Type> result = new List<Type>();

            foreach ( Assembly pluginAssembly in assemblyPlugins )
            {
                if ( pluginAssembly == null )
                {
                    string message = "The plugin assembly must not be null.";
                    InvalidOperationException iopEx = new InvalidOperationException( message );
                    this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, iopEx );
                    throw iopEx;
                }

                var pluginTypes = from type in pluginAssembly.GetTypes()
                                  where type.GetCustomAttributes( typeof( PluggableAttribute ), true ).Length > 0
                                  select type;

                result = result.Concat<Type>( pluginTypes ).ToList<Type>();
            }

            return result;
        }

        internal Dictionary<Type,IList<Attribute>> GetPlugins( IList<Type> pluggables, IList<Assembly> pluginAssemblies )
        {
            if ( pluggables == null )
            {
                string message = "The given pluggables list must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "pluggables", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            if ( pluginAssemblies == null )
            {
                string message = "The given assembly list must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "pluginAssemblies", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            // initialize data structure
            Dictionary<Type,IList<Attribute>> result = new Dictionary<Type, IList<Attribute>>(); 

            foreach ( Assembly pluginAssembly in pluginAssemblies )
            {
                foreach ( Type t in pluginAssembly.GetTypes() )
                {
                    foreach( Type pluggable in pluggables)
                    {
                        var plugins = from att in t.GetCustomAttributes( pluggable, true )
                                      select att;

                        if ( plugins.Count<object>() > 0 )
                        {
                            // retrieve existing list or create a new one
                            IList<Attribute> list = null;
                            if ( result.TryGetValue( pluggable, out list ) )
                            {
                                list = list.Concat<Attribute>( plugins.Cast<Attribute>() ).ToList<Attribute>();
                            }
                            else
                            {
                                list = plugins.Cast<Attribute>().ToList<Attribute>();
                            }

                            result[ pluggable ] = list;
                        }
                    }                    
                }                                
            }

            return result;
        }

        public IEnumerable<Attribute> GetPluginsForType( Type pluggableType )
        {
            if ( pluggableType == null )
            {
                string message = "The given pluggable type must not be null.";
                ArgumentNullException argEx = new ArgumentNullException( "pluggableType", message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, message, argEx );
                throw argEx;
            }

            IList<Attribute> result = null;
            if ( this.plugins.TryGetValue( pluggableType, out result ) )
            {
                return result;
            }
            else
            {
                string message = "The given pluggable type could not be found: " + pluggableType.FullName + ".";
                PluginNotFoundException pnfEx = new PluginNotFoundException( message );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, message, pnfEx );
                throw pnfEx;
            }
        }
    }
}
