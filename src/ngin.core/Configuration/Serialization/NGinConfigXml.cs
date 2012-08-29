/**************************************
 * FILE:          NGinConfigXml.cs
 * DATE:          05.01.2010 10:11:47
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
/**************************************
 * FILE:          NGinConfigXml.cs
 * DATE:          05.01.2010 10:11:40
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
using System.Xml.Serialization;
using NGin.Core.Exceptions;

namespace NGin.Core.Configuration.Serialization
{
    //[Serializable]
    //public class NGinConfigModule
    //{
    //    [XmlAttribute( "type" )]
    //    public string Type { get; set; }

    //    [XmlArray( "properties" ), XmlArrayItem( "property" )]
    //    public Collection<ModulePropertyXml> Properties { get; set; }

    //    public bool IsInitialized
    //    {
    //        get
    //        {
    //            if ( String.IsNullOrEmpty( this.Type ) )
    //            {
    //                return false;
    //            }

    //            return true;
    //        }
    //    }

    //    public override int GetHashCode()
    //    {
    //        if ( this.Type != null )
    //        {
    //            return this.Type.GetHashCode();
    //        }

    //        return base.GetHashCode();
    //    }

    //    public override bool Equals( object obj )
    //    {
    //        NGinConfigModule other = obj as NGinConfigModule;
            
    //        if ( other == null )
    //        {
    //            return false;
    //        }

    //        if ( this.Type == null )
    //        {
    //            return other.Type == null;
    //        }

    //        // check typedef
    //        return this.Type.Equals( other.Type );
    //    }
    //}
    [Serializable, XmlRoot( "ngin" )]
    public class NGinConfigXml : INGinConfigXml
    {
        [XmlArray( "modules" ), XmlArrayItem( "module" )]
        public Collection<ModuleXml> Modules { get; set; }
        private Dictionary<string, ModuleXml> moduleBuffer = new Dictionary<string, ModuleXml>();

        public ModuleXml GetModule( string moduleName )
        {
            if ( this.moduleBuffer.Count != this.Modules.Count )
            {
                foreach ( ModuleXml module in this.Modules )
                {
                    this.moduleBuffer.Add( module.Name, module );
                }
            }

            ModuleXml result = null;

            if ( this.moduleBuffer.ContainsKey( moduleName ) )
            {
                result = this.moduleBuffer[ moduleName ];
            }
            else
            {
                throw new ModuleNotFoundException( "The requested module name does not exist: '" + moduleName + "'" );
            }

            return result;
        }

        public bool IsInitialized
        {
            get
            {
                if ( this.Modules == null )
                {
                    return false;
                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            if ( this.Modules != null && this.Modules.Count > 0 )
            {
                int hashCode = Modules.Count.GetHashCode();

                foreach ( ModuleXml module in this.Modules )
                {
                    hashCode = hashCode | module.GetHashCode();
                }

                return hashCode;
            }

            return base.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            NGinConfigXml other = obj as NGinConfigXml;

            // check type
            if ( other == null )
            {
                return false;
            }

            if ( this.Modules != null )
            {
                // check amount of ModulesCopy
                if ( this.Modules.Count != other.Modules.Count )
                {
                    return false;
                }

                // check each module
                for ( int i = 0; i < this.Modules.Count; i++ )
                {
                    if ( !this.Modules[ i ].Equals( other.Modules[ i ] ) )
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}