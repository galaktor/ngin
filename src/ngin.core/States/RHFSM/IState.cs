#region Header

/**************************************
 * FILE:          IState.cs
 * DATE:          13.08.2009 16:41:21
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
using NGin.Core.Scene;
using System.Collections.Generic;

namespace NGin.Core.States.RHFSM
{

    /// <summary>
    /// Interface to a state.
    /// </summary>
    public interface IState : IDisposable, IEquatable<IState>
    {
        #region Events

        #region Interface Events

        /// <summary>
        /// Occurs whenever the state wishes to transit to another state.
        /// </summary>
        event TransitToStateDelegate TransitToStateRequested;

        #endregion Interface Events

        #endregion Events

        #region Properties

        #region Interface Properties

        /// <summary>
        /// Gets the absolute path of this state within the machine. This value is put
        /// together using the parents path and this states name.
        /// </summary>
        string AbsolutePath
        {
            get;
        }

        /// <summary>
        /// Gets the depth of this state within the machine. This value is calculated from it's parents
        /// depth.
        /// </summary>
        int Depth
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating if this state has any substates.
        /// </summary>
        bool HasSubStates
        {
            get;
        }

        /// <summary>
        /// Gets the name of an initial substate. Can be null if no initial substate is set.
        /// </summary>
        string InitialSubStateName
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating if this state is currently an active state within the machine or not.
        /// </summary>
        bool IsActive
        {
            get;
        }

        bool IsCurrent
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating if this state is the root state within a machine.
        /// This value comes from checking if the state has any parents. Only the root
        /// state cannot have any parents.
        /// </summary>
        bool IsRootState
        {
            get;
        }

        /// <summary>
        /// Gets the name of the current state.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the parent of the current state.
        /// </summary>
        IState Parent
        {
            get; set;
        }

        /// <summary>
        /// Gets an array containing references to all substates of the
        /// current state.
        /// </summary>
        IState[] SubStates
        {
            get;
        }

        /// <summary>
        /// Retrieves a substate with the given name <paramref name="subStateName"/>.
        /// </summary>
        /// <param name="subStateName">The name of the substate to retrieve.</param>
        /// <returns>The substate with the given name <paramref name="subStateName"/>.</returns>
        /// <exception cref="StateNotFoundException">If the given state name is unknown to the current state.</exception>
        IState this[string subStateName]
        {
            get;
        }

        /// <summary>
        /// Retrieves the scene that belongs to the current state.
        /// </summary>
        IScene Scene { get; }

        #endregion Interface Properties

        #endregion Properties

        #region Methods

        #region Interface Methods

        /// <summary>
        /// Adds a substate of the given type to the current state. An instance of the type is created using reflection.
        /// </summary>
        /// <typeparam name="TStateType">The type of state that is to be created and added.</typeparam>
        /// <exception cref="ArgumentNullException">If the given <paramref name="stateType"/> is null.</exception>
        /// <exception cref="ArgumentException">If the given type does not explicitly implement <see cref="RHFSM.IState"/> or
        /// if it does not have a default constructor defined.</exception>
        /// <exception cref="InvalidOperationException">If for some other reason the instance could not be created.</exception>
        void AddSubState<TStateType>()
            where TStateType : IState;

        /// <summary>
        /// Adds a substate to the current state.
        /// </summary>
        /// <param name="subState">The state to add.</param>
        /// <exception cref="ArgumentNullException">If the given <paramref name="subState"/> is null.</exception>
        /// <exception cref="RHFSM.StateNameAlreadyInUseException">If a state with the given state's name already exists as a substate of the current state.</exception>
        void AddSubState( IState subState );

        /// <summary>
        /// Adds a substate of the given type to the current state.
        /// </summary>
        /// <param name="stateType">The type of state that is to be added.</param>
        /// <exception cref="ArgumentNullException">If the given <paramref name="stateType"/> is null.</exception>
        /// <exception cref="ArgumentException">If the given type does not explicitly implement <see cref="RHFSM.IState"/> or
        /// if it does not have a default constructor defined.</exception>
        /// <exception cref="InvalidOperationException">If for some other reason the instance could not be created.</exception>
        void AddSubState( Type stateType );

        void ClearSubStates();

        /// <summary>
        /// Called when the machine enters this state.
        /// </summary>
        /// <remarks>This method should never be called directly.</remarks>
        void Enter();

        /// <summary>
        /// When overriden, contains any code that the state needs to build itself up so that Update() can be called afterwards.
        /// </summary>
        void BuildUp();

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        bool Equals( object obj );

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();

        /// <summary>
        /// Retrieves a substate with the given path <paramref name="subStatePath"/>.
        /// </summary>
        /// <param name="subStatePath">The name of the substate to retrieve.</param>
        /// <returns>The substate with the given name <paramref name="subStateName"/>.</returns>
        /// <exception cref="StateNotFoundException">If the given state name is unknown to the current state.</exception>
        IState GetSubState( string subStatePath );

        /// <summary>
        /// Removes the state's substate with the given <paramref name="subStateName"/>.
        /// </summary>
        /// <param name="subStateName">The name of the substate to remove.</param>
        /// <returns>The state that has been removed.</returns>
        /// <exception cref="StateNotFoundException">If the given <paramref name="subStateName"/> could not be found within the current state.</exception>
        IState RemoveSubState( string subStateName );

        /// <summary>
        /// Called when the machine enters this state.
        /// </summary>
        /// <remarks>This method should never be called directly.</remarks>
        void Exit();

        /// <summary>
        /// When overriden, contains any code that the state needs to tear itself down.
        /// </summary>
        void TearDown();

        /// <summary>
        /// When overriden, contains any code that the state needs to update itself or perform
        /// any actions. This usually is where a state will check for certain conditions and
        /// request a transition to another state when it decides to.
        /// </summary>
        void Update();

        IEnumerable<string> GetRequiredSystems( bool recursive );

        #endregion Interface Methods

        #endregion Methods
    }
}