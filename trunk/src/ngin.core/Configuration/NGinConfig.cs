/**************************************
 * FILE:          NGinConfig.cs
 * DATE:          05.01.2010 10:13:28
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
using System.Xml;
using System.Xml.Serialization;
using NGin.Core.Configuration.Serialization;
using NGin.Core.Exceptions;
using NGin.Core.Platform;

namespace NGin.Core.Configuration
{
    public class NGinConfig :INGinConfig
    {
        private Dictionary<string, INGinModuleConfig> modules = new Dictionary<string, INGinModuleConfig>();
        public IList<INGinModuleConfig> GetModulesDeepCopy() 
        {     
            IList<INGinModuleConfig> result = new List<INGinModuleConfig>( this.modules.Count );
            foreach ( INGinModuleConfig module in this.modules.Values )
            {
                result.Add( module.Clone() as INGinModuleConfig );
            }
            return result;
        }

        public void AddValidConfigDataTypeToAllModules( Type configDataType )
        {
            foreach ( INGinModuleConfig module in this.modules.Values )
            {
                module.AddValidConfigDataType( configDataType );
            }
        }

        private INGinConfigXml nginConfig = null;

        public IDependencyResolver DependencyResolver { get; internal set; }

        public NGinConfig( string configFile )
        {            
            this.nginConfig = this.DeserializeConfigFile( configFile );
                                 
            DependenciesConfigXml dependenciesConfig = this.DeserializeDependenciesConfig( nginConfig );
            this.DependencyResolver = new DependencyResolver( dependenciesConfig );

            foreach ( IModuleXml module in this.nginConfig.Modules )
            {
                INGinModuleConfig moduleConfig = new NGinModuleConfig( module );
                this.modules.Add( moduleConfig.Name, moduleConfig );
            }
        }

        internal INGinConfigXml DeserializeConfigFile( string configFilePath )
        {
            INGinConfigXml result = null;

            // deserialize general configuration object
            FileStream stream = InputOutputManager.OpenFile( configFilePath, FileMode.Open );
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.CloseInput = true;
            XmlReader reader = XmlReader.Create( stream, settings );
            XmlSerializer serializer = new XmlSerializer( typeof( NGinConfigXml ) );

            try
            {
                result = serializer.Deserialize( reader ) as INGinConfigXml;
            }
            catch ( ArgumentException )
            {
                // LOG?
                throw;
            }
            finally
            {
                reader.Close();
                InputOutputManager.CloseFile( configFilePath );
            }

            return result;
        }

        internal DependenciesConfigXml DeserializeDependenciesConfig( INGinConfigXml config )
        {
            DependenciesConfigXml result = null;

            ISectionXml dependenciesConfigSection = config.GetModule( GlobalConstants.XmlConfiguration.ModuleNames.CoreModule ).GetSection( GlobalConstants.XmlConfiguration.SectionNames.DependenciesSection );
            byte[] rawXmlAsBytes = System.Text.Encoding.UTF8.GetBytes( dependenciesConfigSection.XmlRaw );
            MemoryStream memStream = new MemoryStream( rawXmlAsBytes );
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.CloseInput = true;

            XmlReader reader = null;

            try
            {
                reader = XmlReader.Create( memStream, settings );
            }
            catch ( InvalidOperationException ivoEx )
            {
                throw new CoreConfigException( "An error occurred while processing the byte stream as xml.", ivoEx );
            }
            finally
            {
                if ( reader == null )
                {
                    memStream.Close();
                    memStream.Dispose();
                }
            }

            XmlSerializer serializer = new XmlSerializer( typeof(DependenciesConfigXml) );

            try
            {
                result = serializer.Deserialize( reader ) as DependenciesConfigXml;
            }
            catch ( InvalidOperationException ivoEx )
            {
                throw new CoreConfigException( "An error occurred during deserialization to type '" + typeof(DependenciesConfigXml).FullName + "'.", ivoEx );
            }
            catch ( System.Text.DecoderFallbackException dfbEx )
            {
                throw new CoreConfigException( "The text encoding caused an error during deserialization of type '" + typeof(DependenciesConfigXml).FullName + "'", dfbEx );
            }
            finally
            {
                reader.Close();
            }

            return result;
        }

        public INGinModuleConfig GetModule( string moduleName )
        {
            if ( String.IsNullOrEmpty( moduleName ) )
            {
                throw new ArgumentNullException( "moduleName", "The given module name must not be null or empty." );
            }

            INGinModuleConfig result = null;

            this.modules.TryGetValue( moduleName, out result );

            if ( result == null )
            {
                throw new ModuleNotFoundException( "The module with the name of '" + moduleName + "' could not be found." );
            }

            return result;
        }
    }
}
