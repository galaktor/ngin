#region Header

/**************************************
 * FILE:          TransitToStateEventArgs.cs
 * DATE:          13.08.2009 16:44:13
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// 
    /// </summary>
    public class TransitToStateEventArgs : EventArgs
    {
        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitToStateEventArgs"/> class.
        /// </summary>
        /// <param name="reportingState">The state requesting the transit.</param>
        /// <param name="targetAbsolutePath">Absolute path of the target state.</param>
        public TransitToStateEventArgs( State reportingState, string targetAbsolutePath )
        {
            this.ReportingState = reportingState;
            this.TargetStateAbsolutePath = targetAbsolutePath;
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the state requesting the transit.
        /// </summary>
        public State ReportingState
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the target state's absolute path.
        /// </summary>
        public string TargetStateAbsolutePath
        {
            get; private set;
        }

        #endregion Public Properties

        #endregion Properties
    }
}