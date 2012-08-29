/**************************************
 * FILE:          ModulePropertyXml.cs
 * DATE:          05.01.2010 10:11:21
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
using System.Xml;
using System.Xml.Serialization;

namespace NGin.Core.Configuration.Serialization
{
    [Serializable]
    public class ModulePropertyXml
    {
        [XmlAttribute( "id" )]
        public string Id { get; set; }

        [XmlAttribute( "value" )]
        public string Value { get; set; }

        public bool IsInitialized
        {
            get
            {
                // name must not be empty or null while value can be empty in theory
                if ( String.IsNullOrEmpty( this.Id ) ||
                     this.Value == null )
                {
                    return false;
                }

                return true;
            }
        }

        public override bool Equals( object obj )
        {
            ModulePropertyXml other = obj as ModulePropertyXml;

            if ( other == null )
            {
                return false;
            }

            if ( this.Id == null )
            {
                return other.Id == null;
            }

            return this.Id.Equals( other.Id );
        }

        public override int GetHashCode()
        {
            if ( this.Id != null )
            {
                return this.Id.GetHashCode();
            }

            return base.GetHashCode();
        }
    }
}
