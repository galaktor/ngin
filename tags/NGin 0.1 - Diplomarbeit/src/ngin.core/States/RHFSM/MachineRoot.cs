#region Header

/**************************************
 * FILE:          MachineRoot.cs
 * DATE:          13.08.2009 16:42:12
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NGin.Core.Logging;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// The root element of a state tree within a <see cref="RHFSM.Machine"/>.
    /// </summary>
    /// <remarks>A class only used by the <see cref="RHFSM.Machine"/> for initialization of the state tree.</remarks>
    internal class MachineRoot : State
    {
        #region Fields

        #region Public Constant/Read-Only Fields

        public const string MACHINE_ROOT_NAME = "root";

        #endregion Public Constant/Read-Only Fields

        #endregion Fields

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Creates an instance of the root state element.
        /// </summary>
        /// <param name="machine">The machine owning the root element.</param>
        public MachineRoot( IMachine machine, ILogManager logManager )
            : base(MachineRoot.MACHINE_ROOT_NAME, null, null, logManager )
        {
            this.Machine = machine;
            this.subStatesList.Add( MachineRoot.MACHINE_ROOT_NAME, this );
        }

        //public void ClearSubStates()
        //{
        //    lock ( this.subStatesLock )
        //    {
        //        this.subStates.Clear();
        //        this.subStates.Add( MachineRoot.MACHINE_ROOT_NAME, this );
        //    }
        //}

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the machine that owns this root element.
        /// </summary>
        /// <value>The <see cref="RHFSM.Machine"/>.</value>
        public IMachine Machine
        {
            get;
            private set;
        }

        #endregion Public Properties

        #endregion Properties
    }
}