﻿/**************************************
 * FILE:          EntityExtensionAttribute.cs
 * DATE:          05.01.2010 10:12:49
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
using System.Linq;
using System.Text;

namespace NGin.Core.Configuration
{
    [Pluggable( typeof( EntityExtensionAttribute) ), AttributeUsage( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
    public class EntityExtensionAttribute: Attribute
    {
        public Type EntityExtensionType { get; private set; }
        public Type InterfaceType { get; private set; }

        public EntityExtensionAttribute( Type entityExtensionType, Type interfaceType )
        {
            if ( entityExtensionType == null )
            {
                throw new ArgumentNullException( "entityExtensionType" );
            }

            //if ( interfaceType == null )
            //{
            //    throw new ArgumentNullException( "interfaceType" );
            //}            

            this.EntityExtensionType = entityExtensionType;
            this.InterfaceType = interfaceType;
        }
    }
}
