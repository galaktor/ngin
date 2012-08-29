#region Header

/**************************************
 * FILE:          ActiveStatusChangedEventArgs.cs
 * DATE:          13.08.2009 16:39:03
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
using System.Text;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// 
    /// </summary>
    public class IsRunningChangedEventArgs : EventArgs
    {
        #region Constructors

        #region Public Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="IsRunningChangedEventArgs"/> class.
        /// </summary>
        /// <param name="machine">The machine reporting the state change.</param>
        /// <param name="statusOld">The state that the machine had before the change.</param>
        /// <param name="statusNew">The state that the machine had after the change.</param>
        public IsRunningChangedEventArgs( Machine machine, bool statusOld, bool statusNew )
        {
            this.Machine = machine;
            this.StatusOld = statusOld;
            this.StatusNew = statusNew;
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the reporting machine of type <see cref="RHFSM.Machine"/>.
        /// </summary>
        public Machine Machine
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the machines running state before the state change.
        /// </summary>
        public bool StatusNew
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the machines running state after the state change.
        /// </summary>
        public bool StatusOld
        {
            get; private set;
        }

        #endregion Public Properties

        #endregion Properties
    }
}