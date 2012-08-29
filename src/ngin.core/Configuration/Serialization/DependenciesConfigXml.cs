/**************************************
 * FILE:          DependenciesConfigXml.cs
 * DATE:          05.01.2010 10:08:47
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
    [Serializable, XmlRoot( "dependencyconfig" ),
        ConfigDataType( typeof( DependenciesConfigXml ) )]
    public class DependenciesConfigXml
    {
        [XmlArray( "dependencies" ), XmlArrayItem( "directory" )]
        public Collection<DirectoryXml> Dependencies { get; set; }

        public bool IsInitialized
        {
            get
            {
                if ( this.Dependencies == null )
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            if ( this.Dependencies != null && this.Dependencies.Count > 0 )
            {
                int hashCode = Dependencies.Count.GetHashCode();

                foreach ( DirectoryXml dir in this.Dependencies )
                {
                    hashCode = hashCode | dir.GetHashCode();
                }

                return hashCode;
            }

            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            DependenciesConfigXml other = obj as DependenciesConfigXml;
            // check type
            if ( other == null )
            {
                return false;
            }

            if ( this.Dependencies != null )
            {
                // check amount of dependencies
                if ( this.Dependencies.Count != other.Dependencies.Count )
                {
                    return false;
                }

                // check each dependency
                for ( int i = 0; i < this.Dependencies.Count; i++ )
                {
                    if ( !this.Dependencies[ i ].Equals( other.Dependencies[ i ] ) )
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
