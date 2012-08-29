/**************************************
 * FILE:          EntityExtension.cs
 * DATE:          05.01.2010 10:17:01
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
using NGin.Core.Logging;
using NGin.Core.Systems;

namespace NGin.Core.Scene
{
    public class EntityExtension : IEntityExtension
    {
        #region Fields

        #region Private Fields

        public bool IsDisposing
        {
            get;
            private set;
        }
		~EntityExtension()
		{
			this.Dispose(false);
		}
		public void Dispose()
		{
			this.Dispose(true);
		}
		private void Dispose(bool disposing)
		{
            this.IsDisposing = true;

			if (disposing)
			{                
                this.DisposeManaged();
			}
           
            this.DisposeUnmanaged();
		}

        protected virtual void DisposeUnmanaged()
        { }

        protected virtual void DisposeManaged()
        {
            if ( this.actionRequestBuffer != null )
            {
                lock ( this.actionRequestBufferLock )
                {
                    this.actionRequestBuffer.Clear();
                    this.actionRequestBuffer = null;
                }
            }

            if ( this.actionRequestTarget != null )
            {
                lock ( this.actionRequestTargetLock )
                {
                    this.actionRequestTarget = null;
                }
            }

            if ( this.publicData != null )
            {
                lock ( this.publicDataLock )
                {
                    this.publicData = null;
                }
            }

            if ( this.handledActionKeys != null )
            {
                lock ( this.handledActionKeysLock )
                {
                    this.handledActionKeys.Clear();
                    this.handledActionKeys = null;
                }
            }

            if ( this.System != null )
            {
                this.System.TaskStarted -= this.System_TaskStarted;
                this.System.TaskEnded -= this.System_TaskEnded;

                this.System = null;
            }

            if ( this.LogManager != null )
            {
                this.LogManager = null;
            }

            if ( this.Name != null )
            {
                this.Name = null;
            }
        }

        private object handledActionKeysLock = new object();
        private IDictionary<string,EventHandler<ActionRequestEventArgs>> handledActionKeys = new Dictionary<string,EventHandler<ActionRequestEventArgs>>();
        protected internal IEnumerable<string> HandledActionKeys
        {
            get
            {
                lock ( this.handledActionKeysLock )
                {
                    return this.handledActionKeys.Keys;
                }
            }
        }

        protected void AddActionHandler( string actionKey, EventHandler<ActionRequestEventArgs> actionHandler )
        {
            if ( actionHandler == null )
            {
                throw new ArgumentNullException( "actionHandler", "The given action handler must not be null." );
            }

            if ( actionKey == null )
            {
                throw new ArgumentNullException( "actionKey", "The given action key must not be null." );
            }

            lock ( this.handledActionKeysLock )
            {
                if ( this.handledActionKeys.ContainsKey( actionKey ) )
                {
                    this.handledActionKeys[ actionKey ] += actionHandler;
                }
                else
                {
                    EventHandler<ActionRequestEventArgs> handler = Delegate.Combine( actionHandler ) as EventHandler<ActionRequestEventArgs>;
                    this.handledActionKeys.Add( actionKey, actionHandler );
                }
            }
        }

        protected internal bool TryGetActionHandler( string key, out EventHandler<ActionRequestEventArgs> handler )
        {
            lock ( this.handledActionKeysLock )
            {
                return this.handledActionKeys.TryGetValue( key, out handler );
            }
        }

        protected void RemoveActionHandler( string actionKey, EventHandler<ActionRequestEventArgs> actionHandler, bool removeAll )
        {
            if ( actionHandler == null )
            {
                throw new ArgumentNullException( "actionHandler", "The given action handler must not be null." );
            }

            if ( actionKey == null )
            {
                throw new ArgumentNullException( "actionKey", "The given action key must not be null." );
            }

            EventHandler<ActionRequestEventArgs> handler = null;

            lock ( this.handledActionKeysLock )
            {
                if ( this.handledActionKeys.TryGetValue( actionKey, out handler ) )
                {
                    if ( removeAll )
                    {
                        // will remove all occurances of handler
                        handler = Delegate.RemoveAll( handler, actionHandler ) as EventHandler<ActionRequestEventArgs>;
                    }
                    else
                    {
                        // will remove ONLY the last occurance of handler
                        handler -= actionHandler;
                    }

                    if ( handler != null )
                    {
                        this.handledActionKeys[ actionKey ] = handler;
                    }
                    else
                    {
                        this.handledActionKeys.Remove( actionKey );
                    }
                }
                else
                {
                    // LOG?
                    // ignore since the name does not exist
                }
            }
        }

        private object actionRequestBufferLock = new object();
        private Queue<ActionRequestEventArgs> actionRequestBuffer = new Queue<ActionRequestEventArgs>();

        private IEntityExtensionPublicationStorage publicData;
        private object publicDataLock = new object();
        protected internal ILogManager LogManager { get; set; }

        #endregion Private Fields

        #endregion Fields

        #region Constructors

        #region Public Constructors

        public EntityExtension( string name, ISystem system, ILogManager logManager )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "logManager", "The log manager must not be null." );
            }

            if ( String.IsNullOrEmpty( name ) )
            {
                string message = "The given extension name must not be empty or null.";
                ArgumentException argEx = new ArgumentException( message, "name" );
                logManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            this.Name = name;
            this.System = system;
            this.LogManager = logManager;

            // register system events
            this.System.TaskStarted += new TaskStateChangedDelegate( this.System_TaskStarted );
            this.System.TaskEnded += new TaskStateChangedDelegate( this.System_TaskEnded );
        }

        void System_TaskEnded( ISystem system )
        {
            if ( this.IsActive )
            {
                // TODO:
                // nothing happens here YET
                // could it be wise to have extensions flush their data here?
            }
        }

        void System_TaskStarted( ISystem system )
        {
            if ( this.IsActive )
            {
                this.Update();
            }
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties

        public ISystem System { get; protected internal set; }
        public bool IsActive
        {
            get
            {
                lock ( this.publicDataLock )
                {
                    return this.PublicData != null;
                }
            }
        }

        public string Name
        {
            get;
            protected set;
        }

        #endregion Public Properties

        #region Internal Properties

        internal IEntityExtensionPublicationStorage PublicData
        {
            get
            {
                lock ( this.publicDataLock )
                {
                    return publicData;
                }
            }
            set
            {
                lock ( this.publicDataLock )
                {
                    publicData = value;
                }
            }
        }

        #endregion Internal Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        private object actionRequestTargetLock = new object();
        private IActionRequestable actionRequestTarget;
        internal IActionRequestable ActionRequestTarget
        {
            get
            {
                lock ( this.actionRequestTargetLock )
                {
                    return this.actionRequestTarget;
                }
            }
            private set
            {
                lock ( this.actionRequestTargetLock )
                {
                    this.actionRequestTarget = value;
                }
            }
        }

        public void Attatch( IActionRequestRegistry handlerRegistry, IEntityExtensionPublicationStorage storage, IActionRequestable actionRequestTarget, IUpdateRequester updateRequester )
        {
            if ( storage == null )
            {
                throw new ArgumentNullException( "storage", "The given data storage must not be null." );
            }

            if ( handlerRegistry == null )
            {
                throw new ArgumentNullException( "handlerRegistry", "The handler registry must not be null." );
            }

            if ( actionRequestTarget == null )
            {
                throw new ArgumentNullException( "actionRequestTarget", "The action request target must not be null." );
            }

            this.PublicData = storage;
            this.RegisterActionHandlers( handlerRegistry );
            this.ActionRequestTarget = actionRequestTarget;

            this.PublicizeEntityProperties( storage );
            updateRequester.UpdateRequested += this.PublicizeEntityProperties;
        }

        public void Detatch( IActionRequestRegistry handlerRegistry, IUpdateRequester updateRequester )
        {
            this.PublicData = null;
            this.UnregisterActionHandlers( handlerRegistry );
            this.ActionRequestTarget = null;
            updateRequester.UpdateRequested -= this.PublicizeEntityProperties;
        }

        protected internal void Update()
        {
            this.ProcessBufferedActionRequests();
            this.AqquirePublicEntityProperties( this.PublicData as IEntityExtensionPublicAqquisitionStorage );
            this.Process( this.ActionRequestTarget );

            //// THIS SHOULD ONLY BE DONE WHEN HEARTBEAT ENDS
            //this.PublicizeEntityProperties( this.PublicData );
        }

        protected virtual void Process( IActionRequestable actionRequestTarget )
        {

        }

        #endregion Public Methods

        #region Internal Methods

        protected internal void ProcessBufferedActionRequests()
        {
            lock ( this.actionRequestBufferLock )
            {
                while (this.actionRequestBuffer.Count > 0)
                {
                    ActionRequestEventArgs args = this.actionRequestBuffer.Dequeue();
                    EventHandler<ActionRequestEventArgs> handler = null;
                    if ( this.TryGetActionHandler( args.ActionKey, out handler ) )
                    {
                        handler.Invoke( args.RequestingSender, args );
                    }
                    else
                    {
                        // LOG?
                        // ERROR: this extension received a a request that it must have registerred before
                        // but the handler could not be found
                        throw new InvalidOperationException( "This extension recieved an action request that it cannot handle." );
                    }
                }
            }
        }

        /// <summary>
        /// When overridden, an extension will have the chance to get any external entity data
        /// before proceeding with it's operations that are based on this data.
        /// </summary>
        /// <param name="publicDataStorage">The public storage from which data can be aqquired using the methods <see cref="IEntityExtensionPublicAqquisitionStorage.TryGetValue"/> or <see cref="IEntityExtensionPublicAqquisitionStorage.this[ string key ]"/>.</param>
        protected virtual void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
        {
        }

        /// <summary>
        /// When overridden, an extension will have the chance to publicize any data so that it can
        /// be accessed by other extensions or from outside of the owning entity.
        /// </summary>
        /// <param name="publicDataStorage">The public storage into which data can be publicized using <see cref="IEntityExtensionPublicationStorage.Publicize"/></param>
        protected virtual void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
        {
        }

        protected internal void RegisterActionHandlers( IActionRequestRegistry handlerRegistry )
        {
            lock ( this.handledActionKeysLock )
            {
                foreach ( string actionKey in this.handledActionKeys.Keys )
                {
                    handlerRegistry.RegisterActionHandler( actionKey, this.BufferActionRequest );
                }
            }
        }

        protected internal void UnregisterActionHandlers( IActionRequestRegistry handlerRegistry )
        {
            lock ( this.handledActionKeysLock )
            {
                foreach ( string actionKey in this.handledActionKeys.Keys )
                {
                    handlerRegistry.UnregisterActionHandler( actionKey, this.BufferActionRequest, true );
                }
            }
        }

        protected internal void BufferActionRequest( object sender, ActionRequestEventArgs e )
        {
            // exception handling not needed, this has to be fast
            // also buffer NULL
            // will be checked on dequeue later on

            lock ( this.actionRequestBufferLock )
            {
                this.actionRequestBuffer.Enqueue( e );
            }
        }

        #endregion Internal Methods

        #endregion Methods

        #region IEquatable<IEntityExtension> Member

        public bool Equals( IEntityExtension other )
        {
            if ( other == null ) return false;

            bool nameEqual = ( this.Name == null ) ? other.Name == null : this.Name == other.Name;
            bool typeEqual = this.GetType() == other.GetType();

            return typeEqual && nameEqual;
        }

        #endregion

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IEntityExtension );
        }

        public override int GetHashCode()
        {
            int nameCode = ( this.Name == null ) ? 0 : this.Name.GetHashCode();
            int typeCode = this.GetType().GetHashCode();

            return nameCode ^ typeCode;
        }
    }
}