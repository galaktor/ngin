/**************************************
 * FILE:          LocationXml.cs
 * DATE:          05.01.2010 10:11:08
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
    public abstract class LocationXml
    {
        [XmlAttribute( "location" )]
        public string Location { get; set; }

        public virtual bool IsInitialized
        {
            get
            {
                if ( String.IsNullOrEmpty( this.Location ) )
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            if ( this.Location != null )
            {
                return this.Location.GetHashCode();
            }

            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            LocationXml other = obj as LocationXml;
            // check type
            if ( other == null )
            {
                return false;
            }

            // check location
            return this.Location.Equals( other.Location );
        }
    }
}
