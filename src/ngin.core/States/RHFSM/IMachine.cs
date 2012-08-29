#region Header

/**************************************
 * FILE:          IMachine.cs
 * DATE:          13.08.2009 16:39:24
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
    /// A hierarchical finite state machine that operates recursively on an n-ary state tree.
    /// <remarks>Automatically manages a root state of the type <see cref="RHFSM.MachineRoot"/>. Every other state will become a child of the root.</remarks>
    /// </summary>
    public interface IMachine : IDisposable
    {
        #region Events

        #region Interface Events

        /// <summary>
        /// Occurs when the state <see cref="RHFSM.Machine.IsRunning"/> changes.
        /// </summary>
        event IsRunningChangedDelegate IsRunningChanged;

        #endregion Interface Events

        #endregion Events

        #region Properties

        #region Interface Properties

        /// <summary>
        /// Gets the most deepest active state within the whole tree.
        /// </summary>
        /// <remarks>By default this is the automatically generated <see cref="RHFSM.IMachine.RootState"/>.
        /// Only if the machine every segues to another state, that will be the active state.</remarks>
        IState ActiveState
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating if the machine is currently running or if it is deactivated.
        /// </summary>
        bool IsRunning
        {
            get; set;
        }

        /// <summary>
        /// Gets the machines root state.
        /// </summary>
        /// <remarks>The root state is the only state that will have no parents (null). It also is the only state with a <see cref="RHFSM.State.Depth"/>of 0.</remarks>
        IState RootState
        {
            get;
        }

        #endregion Interface Properties

        #endregion Properties

        #region Methods

        #region Interface Methods

        /// <summary>
        /// Initializes the state machine. The given intial state id will be entered.
        /// </summary>
        /// <param name="initialStateName">Initial name of the state.</param>
        void Initialize( string initialStateName );

        //State TransitToState( string targetStateAbsolutePath );
        /// <summary>
        /// Shutdowns the state machine. All active states will be exited and optionally disposed.
        /// </summary>
        /// <param name="autoDispose">If set to <c>true</c> the machine will dispose all substates as well as itself automatically.</param>
        /// <remarks>Set <paramref name="autoDispose"/> to <c>false</c> if you wish to dispose your states by yourself. Remember to dispose the machine later.</remarks>
        /// <seealso cref="RHFSM.Machine.Dispose()"/>
        void Shutdown( bool autoDispose );

        #endregion Interface Methods

        #endregion Methods
    }
}