/**************************************
 * FILE:          SectionXml.cs
 * DATE:          05.01.2010 10:12:04
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
    public class SectionXml : ISectionXml
    {
        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlElement( "data" )]
        public XmlElement XmlElement { get; set; }

        [XmlAttribute( "datatype" )]
        public string DataTypeName { get; set; }

        public Type DataType { get; set; }

        public object Data { get; set; }

        public string XmlRaw
        {
            get
            {
                return this.XmlElement.OuterXml;
            }
        }

        public byte[] GetXmlAsBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes( this.XmlRaw );
        }

        #region ICloneable Member

        public object Clone()
        {
            SectionXml clone = new SectionXml();
            clone.DataType = this.DataType;
            clone.DataTypeName = this.DataTypeName;
            clone.Name = this.Name;
            clone.XmlElement = this.XmlElement;

            return clone;
        }

        #endregion

        public override bool Equals( object obj )
        {
            ISectionXml other = obj as ISectionXml;
            if ( other == null )
            {
                return false;
            }

            return this.Equals( other as ISectionXml );
        }

        public override int GetHashCode()
        {
            // generate hash code from name, since this is usually the ID of a section,
            // even if others of the same type exist
            if ( this.Name != null )
            {
                return this.Name.GetHashCode();
            }

            return base.GetHashCode();
        }

        #region IEquatable<ISectionXml> Member

        public bool Equals( ISectionXml other )
        {
            bool dataTypeEqual = false;
            if ( this.DataType == null )
            {
                dataTypeEqual = other.DataType == null;
            }
            else
            {
                dataTypeEqual = this.DataType.Equals( other.DataType );
            }

            bool dataTypeNameEqual = false;
            if ( this.DataTypeName == null )
            {
                dataTypeNameEqual = other.DataTypeName == null;
            }
            else
            {
                dataTypeNameEqual = this.DataTypeName.Equals( other.DataTypeName );
            }

            bool nameEqual = false;
            if ( this.Name == null )
            {
                nameEqual = other.Name == null;
            }
            else
            {
                nameEqual = this.Name.Equals( other.Name );
            }

            bool xmlEqual = false;
            if ( this.XmlElement == null )
            {
                xmlEqual = other.XmlElement == null;
            }
            else
            {
                xmlEqual = this.XmlElement.Equals( other.XmlElement );
            }

            return dataTypeEqual &&
                   dataTypeNameEqual &&
                   nameEqual &&
                   xmlEqual;
        }

        #endregion
    }
}
