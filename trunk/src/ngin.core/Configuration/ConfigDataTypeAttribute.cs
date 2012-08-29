/**************************************
 * FILE:          ConfigDataTypeAttribute.cs
 * DATE:          05.01.2010 10:12:30
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

namespace NGin.Core.Configuration
{
    [Pluggable( typeof( ConfigDataTypeAttribute ) ), AttributeUsage( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
    public sealed class ConfigDataTypeAttribute : Attribute
    {
        public Type ConfigDataType { get; private set; }

        public ConfigDataTypeAttribute( Type configDataType )
        {
            if ( configDataType == null )
            {
                throw new ArgumentNullException( "configDataType", "The given config data type must not be null." );
            }

            bool isSerializable = false;
            bool isXmlRoot = false;
            bool overridesEquals = false;
            //bool isEquatable = false;

            // check if overrides object.Equals()
            foreach ( var info in configDataType.GetMethods() )
            {
                if ( info.Name.Equals( "Equals" ) && info.DeclaringType.Equals( configDataType ) && info.GetBaseDefinition().DeclaringType.Equals( typeof( object ) ) )
                {
                    overridesEquals = true;
                    break;
                }
            }

            // check if serializable
            if ( configDataType.GetCustomAttributes( typeof( SerializableAttribute ), false ).Length > 0 )
            {
                isSerializable = true;
            }

            // check if xmlroot
            if ( configDataType.GetCustomAttributes( typeof( System.Xml.Serialization.XmlRootAttribute ), false ).Length > 0 )
            {
                isXmlRoot = true;
            }

            // make sure config data type overrides object.Equals()
            if ( !overridesEquals )
            {
                throw new ArgumentException( "The given configuratioun data type '" + configDataType.FullName + "' must override 'object.Equals(object obj)'.", "configDataType" );
            }

            // make sure config data type is serializable
            if ( !isSerializable )
            {
                throw new ArgumentException( "The given configuratioun data type '" + configDataType.FullName + "' must be decorated with the 'System.SerializableAttribute'.", "configDataType" );
            }

            // make sure the config data type is an xml root
            if ( !isXmlRoot )
            {
                throw new ArgumentException( "The given configuration data type '" + configDataType.FullName + "' must be decorated with the 'System.Xml.Serialization.XmlRootAttribute'.", "configDataType" );
            }

            // store data type
            this.ConfigDataType = configDataType;
        }
    }
}
