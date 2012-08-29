/**************************************
 * FILE:          PluginsConfigXml.cs
 * DATE:          05.01.2010 10:11:57
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
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace NGin.Core.Configuration.Serialization
{
    [Serializable, XmlRoot( "pluginconfig" ),
        ConfigDataType( typeof( PluginsConfigXml ) )]
    public class PluginsConfigXml
    {
        [XmlArray( "plugins" ), XmlArrayItem( "file" )]
        public Collection<FileXml> PlugIns { get; set; }

        public bool IsInitialized
        {
            get
            {
                if ( this.PlugIns == null )
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            if ( this.PlugIns != null && this.PlugIns.Count > 0 )
            {
                int hashCode = PlugIns.Count.GetHashCode();

                foreach ( FileXml file in this.PlugIns )
                {                    
                    hashCode = hashCode ^ file.GetHashCode();
                }

                return hashCode;
            }

            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            PluginsConfigXml other = obj as PluginsConfigXml;
            // check type
            if ( other == null )
            {
                return false;
            }

            if ( this.PlugIns != null )
            {
                // check amount of dependencies
                if ( this.PlugIns.Count != other.PlugIns.Count )
                {
                    return false;
                }

                // check each dependency
                for ( int i = 0; i < this.PlugIns.Count; i++ )
                {
                    if ( !this.PlugIns[ i ].Equals( other.PlugIns[ i ] ) )
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
