#region Header

/**************************************
 * FILE:          Machine.cs
 * DATE:          13.08.2009 16:41:46
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
using NGin.Core.Scene;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// A hierarchical finite state machine that operates recursively on an n-ary state tree.
    /// <remarks>Automatically manages a root state of the type <see cref="RHFSM.MachineRoot"/>. Every other state will become a child of the root.</remarks>
    /// </summary>
    public class Machine : IDisposable, IMachine
    {
        #region Fields

        #region Private Fields

        private IState activeState;

        // locks for thread-safety
        private object activeStateLock = new object();
        private bool isRunning = false;
        private object isRunningLock = new object();
        private object transitionLock = new object();

        #endregion Private Fields

        #endregion Fields

        #region Constructors

        #region Public Constructors

        public ILogManager LogManager { get; internal set; }
        /// <summary>
        /// Creates a new machine. Automatically generates a root state of the type <see cref="RHFSM.MachineRoot"/>
        /// </summary>
        public Machine(ILogManager logManager )
        {
            this.RootState = new MachineRoot( this, logManager );
            this.LogManager = logManager;
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Events

        #region Public Events

        /// <summary>
        /// This event is fired every time the machine starts or stops running.
        /// </summary>
        public event IsRunningChangedDelegate IsRunningChanged;

        #endregion Public Events

        #endregion Events

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the most deepest active state within the whole tree.
        /// </summary>
        /// <remarks>By default this is the automatically generated <see cref="RHFSM.Machine.RootState"/>.
        /// Only if the machine every segues to another state, that will be the active state.</remarks>
        public IState ActiveState
        {
            get
            {
                lock ( this.activeStateLock )
                {
                    return activeState;
                }
            }
            private set
            {
                lock ( this.activeStateLock )
                {
                    IState oldState = null;

                    if ( this.activeState != null )
                    {
                        // save ref to old state
                        oldState = this.activeState;

                        // unregister event handler from old state
                        oldState.TransitToStateRequested -= this.ActiveState_SegueToStateRequested;
                    }

                    // save new state
                    this.activeState = value;

                    // register event handler for new state
                    this.activeState.TransitToStateRequested += new TransitToStateDelegate( this.ActiveState_SegueToStateRequested );
                }

                // if there is an initial substate for the active state, then transit to it
                if ( this.activeState != null && !String.IsNullOrEmpty( this.activeState.InitialSubStateName ) )
                {
                    this.TransitToState( State.CombineStateNames( this.activeState.AbsolutePath, this.activeState.InitialSubStateName ) );
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the machine is currently running or if it is deactivated.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock ( this.isRunningLock )
                {
                    return this.isRunning;
                }
            }
            set
            {
                lock ( this.isRunningLock )
                {
                    bool statusOld = this.isRunning;
                    this.isRunning = value;
                    this.FireIsRunningChangedEvent( statusOld, this.isRunning );
                }
            }
        }

        /// <summary>
        /// Gets the machines root state.
        /// </summary>
        /// <remarks>The root state is the only state that will have no parents (null). It also is the only state with a <see cref="RHFSM.State.Depth"/>of 0.</remarks>
        public IState RootState
        {
            get; private set;
        }

        #endregion Public Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Initializes the state machine. The given intial state id will be entered.
        /// </summary>
        /// <param name="initialStateName">Initial name of the state.</param>
        public void Initialize(string initialStateName)
        {
            // set active stat to root
            this.ActiveState = this.RootState;

            this.IsRunning = true;

            // if initial state was defined
            if ( !String.IsNullOrEmpty( initialStateName ) )
            {
                // transit to initial state
                this.TransitToState( initialStateName );
            }
        }

        /// <summary>
        /// Shutdowns the state machine. All active states will be exited and optionally disposed.
        /// </summary>
        /// <param name="autoDispose">If set to <c>true</c> the machine will dispose all substates as well as itself automatically.</param>
        /// <remarks>Set <paramref name="autoDispose"/> to <c>false</c> if you wish to dispose your states by yourself. Remember to dispose the machine later.</remarks>
        /// <seealso cref="RHFSM.Machine.Dispose()"/>
        public void Shutdown( bool autoDispose )
        {
            // translate to root so that all active substates can exit properly
            this.TransitToState( this.RootState.Name );
            this.IsRunning = false;

            this.RootState.ClearSubStates();

            if ( autoDispose )
            {
                // dispose
                this.Dispose();
            }
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Handles the event of the <see cref="RHFSM.Machine.ActiveState"/> requesting a transition to another state.
        /// </summary>
        /// <param name="sender">The state that requested a transition.</param>
        /// <param name="e">The event data.</param>
        internal void ActiveState_SegueToStateRequested( object sender, TransitToStateEventArgs e )
        {
            if ( e == null )
            {
                throw new ArgumentNullException( "e", "The received event arguments must not be null." );
            }

            this.TransitToState( e.TargetStateAbsolutePath );
        }

        /// <summary>
        /// Calls all <see cref="RHFSM.State.Enter"/> methods on all states from the ancestor to the target state in the correct order.
        /// </summary>
        /// <param name="targetState">The target state down to which <see cref="RHFSM.State.Enter"/> of all states will be called. This state will be entered, too.</param>
        /// <param name="startAncestor">The ancestor that will function as a starting point for the calls downwards through all states.
        /// This state will not be entered, since it already should be active.</param>
        /// <remarks>This algorithm is recursive. It moves from the <paramref name="targetState"/> up to the <paramref name="startAncestor"/>.
        /// When the ancestor is found, it drops all the way back, calling the <see cref="RHFSM.State.Enter"/> method in the correct order.</remarks>
        internal void EnterAncestors( IState targetState, IState startAncestor )
        {
            if ( targetState == null )
            {
                throw new ArgumentNullException( "targetState", "The given state must not be null." );
            }

            if ( startAncestor == null )
            {
                throw new ArgumentNullException( "startAncestor", "The given state must not be null." );
            }

            if ( startAncestor.Depth > targetState.Depth )
            {
                throw new InvalidOperationException( "The start ancestor must not be deeper than the target state." );
            }

            // IF current is nca, Enter() it and return.
            if ( targetState == startAncestor )
            {
                return;
            }

            IState nextParent = targetState.Parent == null ? targetState : targetState.Parent;
            this.EnterAncestors( nextParent, startAncestor );

            targetState.Enter();
        }

        /// <summary>
        /// Exits all states upwards, beginning with <paramref name="startState"/> and stopping right before <paramref name="targetAncestor"/>.
        /// </summary>
        /// <param name="startState">The starting point of the path upwards. This state will be exited, too.</param>
        /// <param name="targetAncestor">The end of the path of states that will be exited. This state will not be exited.</param>
        /// <remarks>This algorithm is not recursive. It moves from the <paramref name="startState"/> up to the <paramref name="targetAncestor"/>,
        /// calling Enter on each state in between.</remarks>
        internal void ExitAncestors( IState startState, IState targetAncestor )
        {
            if ( startState == null )
            {
                throw new ArgumentNullException( "startState", "The given state must not be null." );
            }

            if ( targetAncestor == null )
            {
                throw new ArgumentNullException( "targetAncestor", "The given state must not be null." );
            }

            if ( targetAncestor.Depth > startState.Depth )
            {
                throw new InvalidOperationException( "The target ancestor must not be deeper than the start state." );
            }

            while ( startState != targetAncestor )
            {
                startState.Exit();
                startState = startState.Parent;
            }
        }

        /// <summary>
        /// Finds the nearest common ancestor of two states in the tree.
        /// </summary>
        /// <param name="firstState">The first of two states to perform the search for.</param>
        /// <param name="secondState">The second of two states to perform the search for.</param>
        /// <returns>The state that is found to be the nearest common ancestor of the two given states.</returns>
        /// <remarks>This should never be used directly. It is only intended for tests. This algorithm is recursive. It compares
        /// all three combinations of the given state's two ancestor generations.
        /// If a parent is null, the next child is used instead, since the only state in the tree that can have null parents is
        /// the root. This method always returns a result, in the worst case the root itself. Runtime complexity is O(3*d), with 'd'
        /// being the depth steps between the NCA and the deeper of both given states.</remarks>
        internal IState FindNearestCommonAncestorState( IState firstState, IState secondState )
        {
            if ( firstState == null )
            {
                throw new ArgumentNullException( "firstState", "The given state must not be null." );
            }

            if ( secondState == null )
            {
                throw new ArgumentNullException( "secondState", "The given state must not be null." );
            }

            IState deeperState = this.GetDeepestState( firstState, secondState );
            IState higherState = ( firstState == deeperState ) ? secondState : firstState;

            if ( deeperState.Parent == higherState )
            {
                return higherState;
            }
            else if ( higherState.Parent == deeperState )
            {
                return deeperState;
            }

            IState deeperParent = deeperState.Parent == null ? deeperState : deeperState.Parent;
            IState higherParent = higherState.Parent == null ? higherState : higherState.Parent;

            // if input states are identical to next input states, then a stack overflow has been detected
            if ( deeperParent == firstState &&
               higherParent == secondState )
            {
                throw new InvalidOperationException( "A potential stack overflow was detected." );
            }

            if ( deeperParent == higherParent )
            {
                // no matter which one to return, both are equal
                return deeperParent;
            }
            else if ( deeperParent.Parent == higherParent )
            {
                return higherParent;
            }
            else if ( higherParent.Parent == deeperParent )
            {
                return deeperParent;
            }

            return FindNearestCommonAncestorState( deeperParent, higherParent );
        }

        /// <summary>
        /// Fires an event announcing that <see cref="RHFSM.Machine.IsRunning"/> property has changed.
        /// </summary>
        /// <param name="statusOld">The old value of <see cref="RHFSM.Machine.IsRunning"/>.</param>
        /// <param name="statusNew">The new value of <see cref="RHFSM.Machine.IsRunning"/>.</param>
        internal void FireIsRunningChangedEvent( bool statusOld, bool statusNew )
        {
            if ( this.IsRunningChanged != null )
            {
                this.IsRunningChanged.Invoke( this, new IsRunningChangedEventArgs( this, statusOld, statusNew ) );
            }
        }

        /// <summary>
        /// Compares the <see cref="RHFSM.State.Depth"/> property of two states and returns the deeper of both. If both
        /// are equal deep it just returns the first on.
        /// </summary>
        /// <param name="one">The first state.</param>
        /// <param name="two">The second state.</param>
        /// <returns>The deeper of both given states.</returns>
        /// <remarks>This should never be used directly. It is only intended for tests.</remarks>
        internal IState GetDeepestState( IState one, IState two )
        {
            if ( one == null )
            {
                throw new ArgumentNullException( "one", "The given state must not be null." );
            }

            if ( two == null )
            {
                throw new ArgumentNullException( "two", "The given state must not be null." );
            }

            if ( one.Depth >= two.Depth )
            {
                return one;
            }
            else
            {
                return two;
            }
        }

        /// <summary>
        /// Initializes the state machine. The machine will initialize at root state.
        /// </summary>
        /// <remarks>This should never be used directly. It is only intended for tests.</remarks>
        internal void Initialize()
        {
            this.Initialize( null );
        }

        /// <summary>
        /// Performs a transition to the given state identified by it's absolute path.
        /// </summary>
        /// <param name="targetStateAbsolutePath">The target state absolute path. An absolute path describes the state's location from the root state's point of view.
        /// The path consists of the states names separated by the <see cref="RHFSM.State.SUBSTATE_SEPARATOR_CHARACTER"/>.</param>
        /// <returns>The state that is active after the transition.</returns>
        /// <remarks>This should never be used directly. It is only intended for tests.</remarks>
        internal IState TransitToState( string targetStateAbsolutePath )
        {
            if ( String.IsNullOrEmpty( targetStateAbsolutePath ) )
            {
                throw new ArgumentNullException( "targetStateAbsolutePath", "The given target state path must not be empty or null." );
            }

            IState result = null;

            lock ( this.transitionLock )
            {
                // get current and target state
                IState currentState = this.ActiveState;

                if ( targetStateAbsolutePath == currentState.AbsolutePath )
                {
                    return currentState;
                }

                IState targetState = this.RootState[ targetStateAbsolutePath ];

                IState nca = this.FindNearestCommonAncestorState( currentState, targetState );

                // call all Exit() methods on the way up from current to nca
                // TODO: what if a parent turns out to be null before reaching ROOT?
                //       this would indicate a broken tree, since only ROOT may have null as a parent
                //       still, this should be handled properly by throwing e.g. an InvalidOperationException
                this.ExitAncestors( currentState, nca );

                // call all Enter() methods on the way down from nca to target
                this.EnterAncestors( targetState, nca );

                // set active state to new state
                this.ActiveState = targetState;

                ( ( State ) currentState ).IsCurrent = false;
                ( ( State ) this.ActiveState ).IsCurrent = true;

                // save return value
                result = targetState;
            }

            // return new state
            return result;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose( bool disposing )
        {
            // release unmanaged resources

            if ( disposing )
            {
                lock ( this.transitionLock )
                {
                    // release managed resources
                    this.RootState.Dispose();
                    this.RootState = null;
                }

                //// do not nullify to prevent NullPointerExceptions after disposal
                //this.transitionLock = null;
                //this.activeStateLock = null;
                //this.isRunningLock = null;
            }
        }

        #endregion Private Methods

        #endregion Methods
    }
}