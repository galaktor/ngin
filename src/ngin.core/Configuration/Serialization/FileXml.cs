/**************************************
 * FILE:          FileXml.cs
 * DATE:          05.01.2010 10:09:15
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
    public class FileXml : LocationXml
    {
        [XmlAttribute( "filename" )]
        public string FileName { get; set; }

        public override bool IsInitialized
        {
            get
            {
                if ( String.IsNullOrEmpty( this.FileName ) )
                {
                    return false;
                }

                return base.IsInitialized;
            }
        }

        public override bool Equals( object obj )
        {
            FileXml other = obj as FileXml;
            if ( other == null )
            {
                return false;
            }

            // check file
            bool fileEqual = false;
            if ( this.FileName == null )
            {
                fileEqual = other.FileName == null;
            }
            else
            {
                fileEqual = this.FileName.EndsWith( other.FileName );
            }

            // check location
            bool locationEqual = false;
            if ( this.Location == null )
            {
                locationEqual = other.Location == null;
            }
            else
            {
                locationEqual = this.Location.Equals( other.Location );
            }

            return fileEqual && locationEqual;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            if ( this.Location != null )
                hashCode |= this.Location.GetHashCode();

            if ( this.FileName != null )
                hashCode |= this.FileName.GetHashCode();

            if ( hashCode == 0 )
            {
                hashCode = base.GetHashCode();
            }

            return hashCode;
        }
    }
}
