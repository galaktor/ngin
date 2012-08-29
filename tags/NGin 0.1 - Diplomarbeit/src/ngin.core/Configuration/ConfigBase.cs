/**************************************
 * FILE:          ConfigBase.cs
 * DATE:          05.01.2010 10:12:25
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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NGin.Core.Exceptions;
using NGin.Core.Platform;

namespace NGin.Core.Configuration
{
    public abstract class ConfigBase
    {
        protected Dictionary<string, Type> validConfigDataTypes = new Dictionary<string, Type>();

        public void AddValidConfigDataType( Type validConfigDataType )
        {
            // must not be null
            if ( validConfigDataType == null )
            {
                throw new ArgumentNullException();
            }

            // must have config datatype attribute
            if ( validConfigDataType.GetCustomAttributes( typeof( ConfigDataTypeAttribute ), true ).Length == 0 )
            {
                throw new ArgumentException();
            }

            this.validConfigDataTypes[ validConfigDataType.FullName ] = validConfigDataType;
        }

        public ConfigBase()
        {
            // register fallback data types as valid
            this.validConfigDataTypes.Add( typeof( XmlElement ).FullName, typeof( XmlElement ) );
            this.validConfigDataTypes.Add( typeof( string ).FullName, typeof( string ) );

            // register custom data types as valid
            // TODO: move this code to a special class
            Type[] types = null;
            try
            {
                types = InputOutputManager.CoreAssembly.GetTypes();
            }
            catch ( System.Reflection.ReflectionTypeLoadException rtlEx )
            {
                foreach ( Exception ex in rtlEx.LoaderExceptions )
                {
                    Console.WriteLine("LoaderException: " + ex.Message );
                }

                throw;
            }

            foreach ( Type assemblyType in types )
            {
                foreach ( object att in assemblyType.GetCustomAttributes( typeof( ConfigDataTypeAttribute ), true ) )
                {
                    ConfigDataTypeAttribute cdt = att as ConfigDataTypeAttribute;
                    if ( cdt != null )
                    {
                        this.AddValidConfigDataType( cdt.ConfigDataType );
                        //this.validConfigDataTypes.Add( cdt.ConfigDataType.FullName, cdt.ConfigDataType );
                    }
                }
            }
        }

        internal T DeserializeXmlAs<T>( string rawXml )
        {
            return ( T ) this.DeserializeXmlAs( typeof( T ), rawXml );
        }

        internal object DeserializeXmlAs( string typeFullName, string rawXml )
        {
            byte[] rawXmlAsBytes = System.Text.Encoding.UTF8.GetBytes( rawXml );
            return this.DeserializeXmlAs( typeFullName, rawXmlAsBytes );
        }

        internal object DeserializeXmlAs( Type type, string rawXml )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( "type", "The given type must not be null." );
            }

            return this.DeserializeXmlAs( type.FullName, rawXml );
        }

        internal T DeserializeXmlAs<T>( byte[] rawXmlAsBytes )
        {
            return ( T ) this.DeserializeXmlAs( typeof( T ), rawXmlAsBytes );
        }

        internal object DeserializeXmlAs( Type type, byte[] rawXmlAsBytes )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( "type", "The given type must not be null." );
            }

            return this.DeserializeXmlAs( type.FullName, rawXmlAsBytes );
        }

        internal object DeserializeXmlAs( string typeFullName, byte[] rawXmlAsBytes )
        {
            Type type = null;

            if ( String.IsNullOrEmpty( typeFullName ) )
            {
                throw new ArgumentNullException( "The given type name must not be null or empty.", "typeFullName" );
            }

            if ( rawXmlAsBytes == null )
            {
                throw new ArgumentNullException( "The given xml as bytes must not be null.", "rawXmlAsBytes" );
            }

            if ( this.validConfigDataTypes.ContainsKey( typeFullName ) )
            {
                type = this.validConfigDataTypes[ typeFullName ];
            }
            else
            {
                // TODO: This exception should be thrown wherever the check for registerred types is performed, e. g. PlugInManager
                throw new PluginNotFoundException( "The requested config data type '" + typeFullName + "' is not registerred." );
            }

            object result = null;

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

            XmlSerializer serializer = new XmlSerializer( type );

            try
            {
                result = serializer.Deserialize( reader );
            }
            catch ( InvalidOperationException ivoEx )
            {
                throw new CoreConfigException( "An error occurred during deserialization to type '" + type.FullName + "'.", ivoEx );
            }
            catch ( System.Text.DecoderFallbackException dfbEx )
            {
                throw new CoreConfigException( "The text encoding caused an error during deserialization of type '" + type.FullName + "'", dfbEx );
            }
            finally
            {
                reader.Close();
            }

            return result;
        }
    }
}
