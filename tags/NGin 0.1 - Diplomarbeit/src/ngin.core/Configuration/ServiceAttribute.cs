/**************************************
 * FILE:          ServiceAttribute.cs
 * DATE:          05.01.2010 10:14:01
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

namespace NGin.Core.Configuration
{
    [Pluggable(typeof(ServiceAttribute)), System.AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = true )]
    public sealed class ServiceAttribute : Attribute
    {
        public Type InstanceType { get; private set; }
        public Type InterfaceType { get; private set; }
        public bool AsSingleton { get; private set; }

        public ServiceAttribute( Type instanceType, Type interfaceType, bool asSingleton )
        {
            // check for null
            if ( instanceType == null )
            {
                throw new ArgumentNullException( "instanceType", "The given instance type must not be null." );
            }

            // NULL is not a problem for interface, then the instance type will be used as ID
            //// check for null
            //if ( interfaceType == null )
            //{
            //    throw new ArgumentNullException( "interfaceType", "The given interface type must not be null." );
            //}

            // ensure instance type is not an interface
            if ( instanceType.IsInterface )
            {
                throw new ArgumentException( "instanceType", "The given instance type must not be an interface." );
            }

            // IF an interface type is defined then it must really be an interface
            // ensure interface type is an interface
            if ( interfaceType != null && !interfaceType.IsInterface )
            {
                throw new ArgumentException( "interfaceType", "The given interface type must be an interface." );
            }

            this.InstanceType = instanceType;
            this.InterfaceType = interfaceType;
            this.AsSingleton = asSingleton;
        }        
    }
}
