/**************************************
 * FILE:          ModuleXml.cs
 * DATE:          05.01.2010 10:11:33
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
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using NGin.Core.Exceptions;

namespace NGin.Core.Configuration.Serialization
{
    [Serializable]
    public class ModuleXml : IModuleXml
    {
        public ModuleXml()
        {
            this.Sections = new Collection<SectionXml>();
            Console.WriteLine( this.Name );
        }

        [XmlAttribute( "name" )]
        public string Name { get; set; }

        [XmlAttribute( "type" )]
        public string Type { get; set; }

        [XmlArray( "sections" ), XmlArrayItem( "section" )]
        public Collection<SectionXml> Sections { get; set; }

        public IList<ISectionXml> GetSectionsCloned()
        {
            IList<ISectionXml> result = new List<ISectionXml>();

            foreach ( SectionXml section in this.Sections )
            {
                result.Add( section );
            }

            return result;
        }

        private Dictionary<string, SectionXml> sectionsBuffer = new Dictionary<string, SectionXml>();

        public ISectionXml GetSection( string sectionName )
        {
            if ( this.sectionsBuffer.Count != this.Sections.Count )
            {
                foreach ( SectionXml section in this.Sections )
                {
                    this.sectionsBuffer.Add( section.Name, section );
                }
            }

            SectionXml result = null;

            if ( this.sectionsBuffer.ContainsKey( sectionName ) )
            {
                result = this.sectionsBuffer[ sectionName ];
            }
            else
            {
                throw new SectionNotFoundException( "The requested section name does not exist: '" + sectionName + "'" );
            }

            return result;
        }

        public bool IsInitialized
        {
            get
            {
                if ( String.IsNullOrEmpty( this.Type ) )
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            if ( this.Name != null )
            {
                return this.Name.GetHashCode();
            }

            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            ModuleXml other = obj as ModuleXml;

            if ( other == null )
            {
                return false;
            }

            if ( this.Name == null )
            {
                return other.Type == null;
            }

            // check typedef
            return this.Name.Equals( other.Type );
        }

        #region ICloneable Member

        object ICloneable.Clone()
        {
            ModuleXml result = new ModuleXml();

            // copy properties
            result.Name = this.Name;
            result.Type = this.Type;

            // copy sections
            foreach ( SectionXml section in this.Sections )
            {
                result.Sections.Add( section.Clone() as SectionXml );
            }

            return result;
        }

        #endregion

        #region IEquatable<INGinConfigModule> Member

        public bool Equals( IModuleXml other )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
