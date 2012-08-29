#region Header

/**************************************
 * FILE:          State.cs
 * DATE:          13.08.2009 16:42:34
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
using NGin.Core.Scene;
using NGin.Core.Logging;

namespace NGin.Core.States.RHFSM
{
    /// <summary>
    /// A base class implementation of any state that will be used within a <see cref="RHFSM.Machine"/>.
    /// </summary>
    public abstract class State: IState
    {
        #region Fields

        #region Public Constant/Read-Only Fields

        /// <summary>
        /// Defines the character used to separate names within a state path.
        /// </summary>
        public const char SUBSTATE_SEPARATOR_CHARACTER = '/';

        #endregion Public Constant/Read-Only Fields

        #region Protected Fields

        /// <summary>
        /// Contains the substates of the state.
        /// </summary>
        protected Dictionary<string, IState> subStatesList = new Dictionary<string, IState>();

        #endregion Protected Fields

        #region Private Fields

        // locks for concurrent access
        private object parentAccessLock = new object();
        private IState parentState = null;
        protected object subStatesLock = new object();

        #endregion Private Fields

        #endregion Fields

        #region Constructors

        #region Public Constructors

        protected internal ILogManager LogManager { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="id">The id that will be assigned to this state.</param>
        /// <param name="initialSubStateName">The name of a the initiali substate. Can be 'null'
        /// for no initial substate.</param>
        /// <param name="subStates">Any state instances that are to be added as substates to the new state.</param>
        public State(string id, string initialSubStateName, ISceneManager sceneManager, ILogManager logManager, params IState[] subStates )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "The log manager must not be null." );
            }

            this.LogManager = logManager;

            if ( String.IsNullOrEmpty( id ) )
            {
                throw new ArgumentNullException( "The given id must not be null or empty." );
            }

            this.Name = id;

            if ( initialSubStateName == string.Empty )
            {
                throw new ArgumentException( "If an initial substate name is given, it must not be empty. Use 'null' if an initial substate is not required." );
            }

            this.InitialSubStateName = initialSubStateName;


            if ( !this.IsRootState )
            {
                if ( sceneManager == null )
                {
                    throw new ArgumentNullException( "The scene manager must not be null." );
                }
                else
                {
                    this.Scene = sceneManager.CreateAndAddScene( this.Name + "_scene" );
                }
            }

            foreach ( State state in subStates )
            {
                this.AddSubState( state );
            }
        }

        public void ClearSubStates()
        {
            lock ( this.subStatesLock )
            {
                this.subStatesList.Clear();
                if ( this.IsRootState )
                {
                    this.subStatesList.Add( MachineRoot.MACHINE_ROOT_NAME, this );
                }                
            }
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Events

        #region Public Events

        /// <summary>
        /// Occurs whenever the state wishes to transit to another state.
        /// </summary>
        public event TransitToStateDelegate TransitToStateRequested;

        #endregion Public Events

        #endregion Events

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the absolute path of this state within the machine. This value is put
        /// together using the parents path and this states name.
        /// </summary>
        public string AbsolutePath
        {
            get
            {
                lock ( this.parentAccessLock )
                {
                    if ( this.Parent != null )
                    {
                        return State.CombineStateNames( this.Parent.AbsolutePath, this.Name );
                    }
                    else
                    {
                        return this.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the depth of this state within the machine. This value is calculated from it's parents
        /// depth.
        /// </summary>
        public int Depth
        {
            get
            {
                lock ( this.parentAccessLock )
                {
                    if ( this.Parent != null )
                    {
                        return this.Parent.Depth + 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if this state has any substates.
        /// </summary>
        public bool HasSubStates
        {
            get
            {
                lock ( this.subStatesLock )
                {
                    return this.subStatesList.Count != 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of an initial substate. Can be null if no initial substate is set.
        /// </summary>
        public string InitialSubStateName
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets a value indicating if this state is currently an active state within the machine or not.
        /// </summary>
        public bool IsActive
        {
            get; private set;
        }

        public bool IsCurrent
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating if this state is the root state within a machine.
        /// This value comes from checking if the state has any parents. Only the root
        /// state cannot have any parents.
        /// </summary>
        public bool IsRootState
        {
            get
            {
                lock ( this.parentAccessLock )
                {
                    return this.Parent == null && this.Name.Equals(MachineRoot.MACHINE_ROOT_NAME);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the current state.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the parent of the current state.
        /// </summary>
        public IState Parent
        {
            get
            {
                lock ( this.parentAccessLock )
                {
                    return this.parentState;
                }
            }
            set
            {
                lock ( this.parentAccessLock )
                {
                    this.parentState = value;
                }
            }
        }

        /// <summary>
        /// Gets an array containing references to all substates of the
        /// current state.
        /// </summary>
        public IState[] SubStates
        {
            get
            {
                State[] result = null;

                lock ( this.subStatesLock )
                {
                    result = new State[ this.subStatesList.Count ];
                    this.subStatesList.Values.CopyTo( result, 0 );
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the scene associated with the current state.
        /// </summary>
        public IScene Scene
        {
            get;
            protected internal set;
        }

        #endregion Public Properties

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Retrieves a substate with the given name <paramref name="subStateName"/>.
        /// </summary>
        /// <param name="subStateName">The name of the substate to retrieve.</param>
        /// <returns>The substate with the given name <paramref name="subStateName"/>.</returns>
        /// <exception cref="StateNotFoundException">If the given state name is unknown to the current state.</exception>
        public IState this[string subStateName]
        {
            get
            {
                return this.GetSubState( subStateName );
            }
        }

        #endregion Indexers

        #region Methods

        #region Public Static Methods

        /// <summary>
        /// Combines several names to a valid path using the global path separator <see cref="RHFSM.State.SUBSTATE_SEPARATOR_CHARACTER"/>.
        /// </summary>
        /// <param name="subStateNames">A list of names that are to be combined to a path.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="ArgumentNullException">If any of the given <paramref name="subStateNames"/> is null.</exception>
        public static string CombineStateNames(params string[] subStateNames)
        {
            StringBuilder sb = new StringBuilder();

            for ( int i = 0; i < subStateNames.Length; i++ )
            {
                string id = subStateNames[ i ];

                if ( id == null )
                {
                    throw new ArgumentNullException( "The given sub state names must not be null." );
                }

                if ( id.StartsWith( MachineRoot.MACHINE_ROOT_NAME ) )
                {
                    id = id.Substring( MachineRoot.MACHINE_ROOT_NAME.Length );
                    if ( id.StartsWith( State.SUBSTATE_SEPARATOR_CHARACTER.ToString() ) )
                    {
                        id = id.Substring( 1 );
                    }
                }

                if ( !String.IsNullOrEmpty(id) )
                {
                    sb.Append( id );

                    // add separator if there are more to come
                    if ( i < subStateNames.Length - 1 )
                    {
                        sb.Append( State.SUBSTATE_SEPARATOR_CHARACTER );
                    }
                }
            }

            return sb.ToString();
        }

        #endregion Public Static Methods

        #region Public Methods


        /// <summary>
        /// Adds a substate of the given type to the current state. An instance of the type is created using reflection.
        /// </summary>
        /// <typeparam name="TStateType">The type of state that is to be created and added.</typeparam>
        /// <exception cref="ArgumentNullException">If the given <paramref name="stateType"/> is null.</exception>
        /// <exception cref="ArgumentException">If the given type does not explicitly implement <see cref="RHFSM.IState"/> or
        /// if it does not have a default constructor defined.</exception>
        /// <exception cref="InvalidOperationException">If for some other reason the instance could not be created.</exception>
        public void AddSubState<TStateType>()
            where TStateType : IState
        {
            this.AddSubState( typeof( TStateType ) );
        }

        /// <summary>
        /// Adds a substate of the given type to the current state.
        /// </summary>
        /// <param name="stateType">The type of state that is to be added.</param>
        /// <exception cref="ArgumentNullException">If the given <paramref name="stateType"/> is null.</exception>
        /// <exception cref="ArgumentException">If the given type does not explicitly implement <see cref="RHFSM.IState"/> or
        /// if it does not have a default constructor defined.</exception>
        /// <exception cref="InvalidOperationException">If for some other reason the instance could not be created.</exception>
        public void AddSubState( Type stateType )
        {
            if ( stateType == null )
            {
                throw new ArgumentNullException( "stateType", "The given state type must not be null." );
            }

            // TODO: wtf? why doesn't this work?
            ////if ( !(typeof(IState).IsAssignableFrom(stateType)) )
            //if ( !stateType.BaseType.Equals( typeof( State ) ) )
            //{
            //    throw new ArgumentException( "The given type must implement RHFSM.IState.", "stateType" );
            //}

            object newSubState;
            // create instance of TStateType
            try
            {
                newSubState = stateType.InvokeMember( stateType.Name, System.Reflection.BindingFlags.CreateInstance, null, null, new object[] { } );
            }
            catch ( MissingMethodException mmEx )
            {
                throw new ArgumentException( "The given type '" + stateType.Name + "' must have a default constructor to be created by type. Think about using AddSubState(IState subState) instead.", mmEx );
            }

            if ( newSubState == null )
            {
                throw new InvalidOperationException( "Could not create an instance of type " + stateType.FullName );
            }

            // add the subState
            this.AddSubState( newSubState as IState );
        }

        /// <summary>
        /// Adds a substate to the current state.
        /// </summary>
        /// <param name="subState">The state to add.</param>
        /// <exception cref="ArgumentNullException">If the given <paramref name="subState"/> is null.</exception>
        /// <exception cref="RHFSM.StateNameAlreadyInUseException">If a state with the given state's name already exists as a substate of the current state.</exception>
        public void AddSubState( IState subState )
        {
            if ( subState == null)
            {
                throw new ArgumentNullException( "subState", "The given substate to add must not be null." );
            }

            lock ( this.subStatesLock )
            {
                try
                {
                    this.subStatesList.Add( subState.Name, subState );
                }
                catch ( ArgumentException argEx )
                {
                    throw new StateNameAlreadyInUseException( subState.Name, argEx );
                }
            }

            subState.Parent = this;
        }        

        /// <summary>
        /// When overriden, contains any code that the state needs to build itself up so that Update() can be called afterwards.
        /// </summary>
        public virtual void BuildUp()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Called when the machine enters this state.
        /// </summary>
        /// <remarks>This method should never be called directly.</remarks>
        public void Enter()
        {
            this.BuildUp();
            this.IsActive = true;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals( IState other )
        {
            return ReferenceEquals( this, other );
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals( object obj )
        {
            // using address equality is sufficient
            return base.Equals( obj );
        }

        /// <summary>
        /// Called when the machine enters this state.
        /// </summary>
        /// <remarks>This method should never be called directly.</remarks>
        public void Exit()
        {
            this.TearDown();
            this.IsActive = false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // hash code based on address is ok
            return base.GetHashCode();
        }

        /// <summary>
        /// Retrieves a substate with the given path <paramref name="subStatePath"/>.
        /// </summary>
        /// <param name="subStatePath">The name of the substate to retrieve.</param>
        /// <returns>The substate with the given name <paramref name="subStateName"/>.</returns>
        /// <exception cref="StateNotFoundException">If the given state name is unknown to the current state.</exception>
        public IState GetSubState( string subStatePath )
        {
            if ( subStatePath == null )
            {
                throw new ArgumentNullException( "subStatePath", "The given path must not be null." );
            }

            string subStateName = string.Empty;
            string rest = string.Empty;

            int firstSeparator = subStatePath.IndexOf( State.SUBSTATE_SEPARATOR_CHARACTER );
            if ( firstSeparator < 0 )
            {
                subStateName = subStatePath;
            }
            else
            {
                subStateName = subStatePath.Substring( 0, firstSeparator );
                rest = subStatePath.Substring( firstSeparator + 1 );
            }

            IState subState = null;

            lock ( this.subStatesLock )
            {
                try
                {
                    subState = this.subStatesList[ subStateName ];
                }
                catch ( KeyNotFoundException knfEx )
                {
                    throw new StateNotFoundException( subStateName, knfEx );
                }

                if ( subState == null )
                {
                    throw new StateNotFoundException( subStateName );
                }

                if ( String.IsNullOrEmpty( rest ) )
                {
                    return subState;
                }
                else
                {
                    return this.subStatesList[ subStateName ][ rest ];
                }
            }
        }

        /// <summary>
        /// Removes the state's substate with the given <paramref name="subStateName"/>.
        /// </summary>
        /// <param name="subStateName">The name of the substate to remove.</param>
        /// <returns>The state that has been removed.</returns>
        /// <exception cref="StateNotFoundException">If the given <paramref name="subStateName"/> could not be found within the current state.</exception>
        public IState RemoveSubState( string subStateName )
        {
            // save reference to return later
            IState removedState = null;

            lock ( this.subStatesLock )
            {
                // attempt to retrieve the state
                try
                {
                    removedState = this.subStatesList[ subStateName ];
                }
                catch ( KeyNotFoundException knfEx )
                {
                    throw new StateNotFoundException( subStateName, knfEx );
                }

                // remove the state if it is contained
                if ( removedState != null )
                {
                    bool errorRemoving = !( this.subStatesList.Remove( subStateName ) );
                    removedState.Parent = null;

                    if ( errorRemoving )
                    {
                        // An error occurred while removing a state that should be contained
                        string message = String.Format( CultureInfo.InvariantCulture,
                                                       "The state with Id {0} could not be removed, even though it seems to be a valid substate.",
                                                       subStateName );
                        throw new InvalidOperationException( message );
                    }
                }
            }

            // return the state for potential further usage or disposal
            return removedState;
        }

        /// <summary>
        /// When overriden, contains any code that the state needs to tear itself down.
        /// </summary>
        public virtual void TearDown()
        {
        }

        /// <summary>
        /// When overriden, contains any code that the state needs to update itself or perform
        /// any actions. This usually is where a state will check for certain conditions and
        /// request a transition to another state when it decides to.
        /// </summary>
        public virtual void Update()
        {
            lock ( this.parentAccessLock )
            {
                if ( this.parentState != null )
                {
                    this.parentState.Update();
                }                
            }

            if ( this.Scene != null )
            {
                this.Scene.Update();
            }            
        }

        public IEnumerable<string> GetRequiredSystems( bool recursive )
        {
            List<string> result = new List<string>();

            if(this.Scene != null)
            {
                result.AddRange( this.Scene.RequiredSystems );
            }

            if(recursive)
            {
                lock ( this.parentAccessLock )
                {
                    if ( this.parentState != null )
                    {
                        result.AddRange( this.parentState.GetRequiredSystems( recursive ) );
                    }
                }
            }

            return result;
        }

        #endregion Public Methods

        #region Protected Methods


        /// <summary>
        /// Fires an event that indicates that this state wishes to transit to another state.
        /// </summary>
        /// <param name="targetStateAbsolutePath">The absolute path of the state to transit to.</param>
        /// <remarks>This method is never to be called directly.</remarks>
        protected void FireTransitToStateEvent( string targetStateAbsolutePath )
        {
            // inactive states cannot request transitions, the request would be ignored
            if ( this.TransitToStateRequested != null )
            {
                TransitToStateEventArgs e = new TransitToStateEventArgs( this, targetStateAbsolutePath );
                this.TransitToStateRequested.Invoke( this, e );
            }
        }

        /// <summary>
        /// When overriden, releases any managed resources. Called during object disposal.
        /// </summary>
        /// <seealso cref="RHFSM.State.Dispose()"/>
        protected virtual void ReleaseManagedResources()
        {
            // dispose of substates
            Collection<IState> statesToRemove = new Collection<IState>();
            lock ( this.subStatesLock )
            {
                foreach ( KeyValuePair<string, IState> kvp in this.subStatesList )
                {
                    // get the state that will be disposed
                    IState removedState = kvp.Value;

                    // mark to remove from collection afterwards
                    statesToRemove.Add( removedState );
                }
            }

            for ( int i = 0; i < statesToRemove.Count; i++ )
            {
                // holf local reference to control reference count
                IState removedState = statesToRemove[i];

                // remove the state from the collection
                this.subStatesList.Remove( removedState.Name );

                // dispose
                removedState.Dispose();

                // nullify reference to mark unused in GC
                removedState = null;
            }
        }

        /// <summary>
        /// When overriden, releases any unmanged resources. Called during object disposal.
        /// </summary>
        /// <seealso cref="RHFSM.State.Dispose()"/>
        protected virtual void ReleaseUnmanagedResources()
        {
        }

        /// <summary>
        /// Fires an event that indicates that the state wishes to transit to <paramref name="targetStateAbsolutePath"/>
        /// </summary>
        /// <param name="targetStateAbsolutePath">The absolute path of the state to transit to.</param>
        protected void RequestTransitionTo(string targetStateAbsolutePath)
        {
            // fire an event to signalize the request
            this.FireTransitToStateEvent( targetStateAbsolutePath );
        }

        #endregion Protected Methods

        #region Private Methods


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose( bool disposing )
        {
            this.ReleaseUnmanagedResources();
            if ( disposing )
            {
                this.ReleaseManagedResources();
            }
        }

        #endregion Private Methods

        #endregion Methods
    }
}