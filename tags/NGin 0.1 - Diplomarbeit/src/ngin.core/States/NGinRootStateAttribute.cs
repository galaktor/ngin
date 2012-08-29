/**************************************
 * FILE:          NGinRootStateAttribute.cs
 * DATE:          05.01.2010 10:19:00
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
using NGin.Core.Configuration;

namespace NGin.Core.States
{
    [Pluggable( typeof( NGinRootStateAttribute ) ), AttributeUsage( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
    public class NGinRootStateAttribute: Attribute
    {
        public Type RootStateType { get; private set; }
        public Type[] ConstructorParameterTypes { get; set; }

        public NGinRootStateAttribute( Type rootStateType ):this(rootStateType, new Type[] {} )
        { }

        public NGinRootStateAttribute( Type rootStateType, params Type[] constructorParameterTypes )
        {
            // make sure type is not null
            if ( rootStateType == null )
            {
                string message = "The given root state type must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "rootStateType", message );
                throw argnEx;
            }

            // make sure defined state type inherits from abstract class RHFSM.State
            if ( !rootStateType.IsSubclassOf( typeof( NGin.Core.States.RHFSM.State ) ) )
            {
                string message = "The defined state type must be a subclass of " + typeof( NGin.Core.States.RHFSM.State ).FullName + ".";
                InvalidOperationException invopEx = new InvalidOperationException( message );
                throw invopEx;
            }

            if ( constructorParameterTypes == null )
            {
                string message = "The given constructor parameter types must not be null.";
                ArgumentNullException argnEx = new ArgumentNullException( "constructorParameterTypes", message );
                throw argnEx;
            }

            this.RootStateType = rootStateType;
            this.ConstructorParameterTypes = constructorParameterTypes;
        }
    }
}
