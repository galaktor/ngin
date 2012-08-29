/**************************************
 * FILE:          SettingsConfigXml.cs
 * DATE:          05.01.2010 10:12:11
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
namespace NGin.Core.Configuration.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using NGin.Core.Configuration;

    [Serializable, XmlRoot( "customsettings" ),
        ConfigDataType( typeof( SettingsConfigXml ) )]
    public class SettingsConfigXml
    {
        [XmlArray( "settings" ), XmlArrayItem( "setting" )]
        public Collection<SettingXml> Settings { get; set; }

        private Dictionary<string, string> settingValues;

        public bool TryGetSettingValue( string key, out string valueTarget )
        {
            if ( this.settingValues == null )
            {
                this.settingValues = new Dictionary<string, string>();
                foreach ( SettingXml setting in this.Settings )
                {
                    this.settingValues[ setting.Key ] = setting.Value;
                }
            }

            return this.settingValues.TryGetValue( key, out valueTarget );
        }

        public override bool Equals( object obj )
        {
            SettingsConfigXml other = obj as SettingsConfigXml;
            // check type
            if ( other == null )
            {
                return false;
            }

            if ( this.Settings != null )
            {
                // check amount of dependencies
                if ( this.Settings.Count != other.Settings.Count )
                {
                    return false;
                }

                // check each dependency
                for ( int i = 0; i < this.Settings.Count; i++ )
                {
                    if ( !this.Settings[ i ].Equals( other.Settings[ i ] ) )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if ( this.Settings != null && this.Settings.Count > 0 )
            {
                int hashCode = Settings.Count.GetHashCode();

                foreach ( SettingXml setting in this.Settings )
                {
                    hashCode = hashCode ^ setting.GetHashCode();
                }

                return hashCode;
            }

            return base.GetHashCode();
        }
    }
}
