#region Header

/**************************************
 * FILE:          StateNotFoundException.cs
 * DATE:          13.08.2009 16:43:32
 * AUTHOR:        Raphael B. Estrada
 * AUTHOR URL:    http://www.galaktor.net
 * AUTHOR E-MAIL: galaktor@gmx.de
 *
 * The MIT License
 *
 * Copyright (c) 2009 Raphael B. Estrada
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

#endregion Header

using System;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class StateNotFoundException : Exception
    {
        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        public StateNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        /// <param name="subStateName">Name of the substate causing the exception.</param>
        public StateNotFoundException( string subStateName )
            : this(subStateName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNotFoundException"/> class.
        /// </summary>
        /// <param name="subStateName">Name of the substate causing the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public StateNotFoundException( string subStateName, Exception innerException )
            : base("Could not find the given state: '" + subStateName + "'", innerException)
        {
        }

        #endregion Public Constructors

        #endregion Constructors
    }
}