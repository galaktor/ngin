/**************************************
 * FILE:          PluggableAttributeTest.cs
 * DATE:          05.01.2010 10:24:11
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
using System.Text;
using NUnit.Framework;
using NGin.Core.Configuration;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class PluggableAttributeTest
    {
        [Test]
        public void PluggalbeAttribute_TypeAssignableAsAttribute_Success()
        {
            // arrange
            PluggableAttribute att;
            Type type = typeof( SerializableAttribute );

            // act
            att = new PluggableAttribute(type);

            // assert
            Assert.IsNotNull( att );
            Assert.IsInstanceOf<PluggableAttribute>( att );
            Assert.AreEqual( type, att.PluggableType );
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void PluggableAttribute_TypeNotAssignableFromAttribute_RaiseArgumentException()
        {
            // arrange
            PluggableAttribute att;
            Type type = typeof( object );

            // act
            att = new PluggableAttribute( type );

            // assert            
        }
    }
}
