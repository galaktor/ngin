/**************************************
 * FILE:          NGinModuleConfig.cs
 * DATE:          05.01.2010 10:13:42
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
using System.Diagnostics;
using System.Xml;
using NGin.Core.Configuration.Serialization;
using NGin.Core.Exceptions;
namespace NGin.Core.Configuration
{
    public class NGinModuleConfig : ConfigBase, INGinModuleConfig
    {
        public IModuleXml ModuleConfig { get; private set; }
        public string Name { get; private set; }
        public string TypeFullName { get; private set; }
        
        public NGinModuleConfig( IModuleXml moduleConfig )
        {
            this.ModuleConfig = this.PostProcessModuleConfig( moduleConfig );
            this.Name = this.ModuleConfig.Name;
            this.TypeFullName = this.ModuleConfig.Type;
        }

        internal IModuleXml PostProcessModuleConfig( IModuleXml moduleConfig )
        {
            ModuleXml result = moduleConfig as ModuleXml;

            if ( result == null )
            {
                throw new ArgumentException( "The given module config must be assignable to type ModuleXml.", "moduleConfig" );
            }

            foreach ( SectionXml section in result.Sections )
            {
                this.InsertDataIntoSectionXml( section );
            }

            return result;
        }

        internal ISectionXml GetSectionCloned( string sectionName )
        {
            ISectionXml result = null;

            if ( String.IsNullOrEmpty( sectionName ) )
            {
                throw new ArgumentNullException( "The requested sections name must not be null or empty.", "sectionName" );
            }

            ISectionXml originalSection = null;
            try
            {
                originalSection = this.ModuleConfig.GetSection( sectionName );
            }
            catch ( SectionNotFoundException snfEx )
            {
                throw new ArgumentException( "The given section name is not valid: '" + sectionName + "'", snfEx );
            }
            result = this.CloneSectionWithData( originalSection );

            return result;
        }
        internal ISectionXml CloneSectionWithData( ISectionXml originalSection )
        {
            ISectionXml result = null;

            if ( originalSection == null )
            {
                throw new ArgumentNullException( "The given section must not be null.", "originalSection" );
            }

            result = originalSection.Clone() as ISectionXml;
            this.InsertDataIntoSectionXml( result as SectionXml );

            return result;
        }

        public object GetSectionData( string sectionName )
        {
            object result = null;

            ISectionXml section = this.GetSectionCloned( sectionName );

            if ( section != null )
            {
                result = section.Data;
            }

            return result;
        }

        public string GetSectionDataAsRawXml( string sectionName )
        {
            string result = null;

            ISectionXml section = this.GetSectionCloned( sectionName );

            if ( section != null )
            {
                result = section.XmlRaw;
            }

            return result;
        }

        public XmlElement GetSectionDataAsXmlElement( string sectionName )
        {
            ISectionXml section = this.GetSectionCloned( sectionName );

            XmlElement result = null;

            if ( section != null )
            {
                result = section.XmlElement;
            }

            return result;
        }

        public T GetSectionDataAs<T>( string sectionName )
        {
            return ( T ) this.GetSectionDataAs( typeof( T ), sectionName );
        }

        public object GetSectionDataAs( Type type, string sectionName )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( "The given type must not be null.", "type" );
            }

            object result = null;
            ISectionXml section = this.GetSectionCloned( sectionName );

            if ( section != null )
            {
                switch ( type.FullName )
                {
                    case "System.Xml.XmlElement":
                        result = section.XmlElement;
                        break;
                    case "System.String":
                        result = section.XmlRaw;
                        break;
                    default:
                        {
                            result = this.DeserializeXmlAs( type, section.XmlRaw );
                            break;
                        }
                }
            }

            return result;
        }

        internal Type InsertDataIntoSectionXml( SectionXml section )
        {
            object data = null;

            if ( section == null )
            {
                throw new ArgumentNullException( "The given section must not be null.", "section" );
            }
            if ( String.IsNullOrEmpty( section.DataTypeName ) )
            {
                //throw new ArgumentException( "The given section '" + section.Id + "' does not have a valid data type name.", "section.DataTypeName" );
                section.DataTypeName = typeof( XmlElement ).FullName;
            }

            switch ( section.DataTypeName )
            {
                //case "System.String":
                case "System.Xml.XmlElement":
                    // fallback: save XmlElement as MainDataStorage
                    data = section.XmlElement.Clone();
                    break;
                default:
                    try
                    {
                        data = this.DeserializeXmlAs( section.DataTypeName, section.XmlRaw );
                    }
                    catch ( PluginNotFoundException )
                    {
                        data = section.XmlElement.Clone();
                    }
                    break;
            }

            Debug.Assert( data != null, "An error has occurred while preprocessing the sections data: '" + section.Name + "'." );

            section.Data = data;
            section.DataType = data.GetType();

            return section.DataType;
        }

        #region ICloneable Member

        public object Clone()
        {
            return new NGinModuleConfig( this.ModuleConfig.Clone() as IModuleXml);
        }

        #endregion
    }
}
