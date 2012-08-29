/**************************************
 * FILE:          Entity.cs
 * DATE:          05.01.2010 10:16:52
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

using NGin.Core.Exceptions;

namespace NGin.Core.Scene
{
    internal class Entity : IEntity
    {
        #region Fields

        #region Private Fields

        private object extensionAddedLock = new object();
        private event ExtensionListChangedDelegate extensionAdded;
        public event ExtensionListChangedDelegate ExtensionAdded
        {
            add
            {
                lock ( this.extensionAddedLock )
                {
                    this.extensionAdded += value;
                }
            }
            remove
            {
                lock ( this.extensionAddedLock )
                {
                    this.extensionAdded -= value;
                }
            }
        }

        private void FireExtensionAddedEvent( IEntityExtension extension )
        {
            lock ( this.extensionAddedLock )
            {
                if ( this.extensionAdded != null )
                {
                    this.extensionAdded( this, extension );
                }
            }
        }

        private object extensionRemovedLock = new object();
        private event ExtensionListChangedDelegate extensionRemoved;
        public event ExtensionListChangedDelegate ExtensionRemoved
        {
            add
            {
                lock ( this.extensionRemovedLock )
                {
                    this.extensionRemoved += value;
                }
            }
            remove
            {
                lock ( this.extensionRemovedLock )
                {
                    this.extensionRemoved -= value;
                }
            }
        }

        private void FireExtensionRemovedEvent( IEntityExtension extension )
        {
            lock ( this.extensionRemovedLock )
            {
                if ( this.extensionRemoved != null )
                {
                    this.extensionRemoved( this, extension );
                }
            }
        }
        

        private volatile Dictionary<string, EventHandler<ActionRequestEventArgs>> actionHandlers = new Dictionary<string, EventHandler<ActionRequestEventArgs>>();
        private object actionHandlersLockObject = new object();
        private volatile Dictionary<string, IEntityExtension> extensions = new Dictionary<string, IEntityExtension>();
        private object extensionsLock = new object();
        private volatile IEntityBufferedExtensionData properties;
        private object propertiesLock = new object();

        #endregion Private Fields

        #endregion Fields

        #region Constructors

        #region Public Constructors

        internal Entity( string name, params IEntityExtension[] extensions )
        {
            this.properties = new EntityPropertyStorage(this);
            this.Name = name;

            foreach ( IEntityExtension extension in extensions )
            {
                this.AddExtension( extension );
            }
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Properties

        #region Public Properties



        public IEnumerable<KeyValuePair<string, EventHandler<ActionRequestEventArgs>>> ActionHandlers
        {
            get
            {
                lock ( actionHandlersLockObject )
                {
                    return ( ( IEnumerable<KeyValuePair<string, EventHandler<ActionRequestEventArgs>>> ) this.actionHandlers );
                }
            }
        }

        public int ActionHandlersCount
        {
            get
            {
                lock ( this.actionHandlersLockObject )
                {
                    return this.actionHandlers.Count;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, IEntityExtension>> Extensions
        {
            get
            {
                return ( ( IEnumerable<KeyValuePair<string, IEntityExtension>> ) this.extensions );
            }
        }

        public int ExtensionsCount
        {
            get
            {
                lock ( this.extensionsLock )
                {
                    return this.extensions.Count;
                }
            }
        }

        object nameLock = new object();
        private string name;
        public string Name
        {
            get
            {
                lock ( this.nameLock )
                {
                    return name;
                }                
            }
            set
            {
                lock ( this.nameLock )
                {
                    name = value;
                }                
            }
        }

        public IEntityBufferedExtensionData Properties
        {
            get
            {
                lock ( this.propertiesLock )
                {
                    return this.properties;
                }
            }
        }

        #endregion Public Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        private object updateRequestedLock = new object();        
        internal event UpdateDelegate updateRequested;
        public event UpdateDelegate UpdateRequested
        {
            add
            {
                lock ( this.updateRequestedLock )
                {
                    this.updateRequested += value;
                }
            }
            remove
            {
                lock ( this.updateRequestedLock )
                {
                    this.updateRequested -= value;
                }
            }            
        }
        protected internal void Flush()
        {
            //lock ( this.extensionsLock )
            //{
            //    foreach ( IEntityExtension ext in this.extensions.Values )
            //    {
            //        ( ( EntityExtension ) ext ).PublicizeEntityProperties( this.Properties as IEntityExtensionPublicationStorage );
            //    }
            //}            
            this.FireUpdateRequestedEvent( this.Properties as IEntityExtensionPublicationStorage );

            this.Properties.SwapAndFlush();
        }

        internal void FireUpdateRequestedEvent( IEntityExtensionPublicationStorage publicationStorage )
        {
            lock ( this.updateRequestedLock )
            {
                if ( this.updateRequested != null )
                {
                    this.updateRequested( this.Properties as IEntityExtensionPublicationStorage );
                }                
            }
        }

        public void AddExtension( IEntityExtension extension )
        {
            if ( extension == null )
            {
                throw new ArgumentNullException( "extension", "The given extension must not be null." );
            }

            try
            {
                lock ( this.extensionsLock )
                {
                    this.extensions.Add( extension.Name, extension );
                }

                //( ( EntityExtension ) extension ).PublicizeEntityProperties( this.Properties as IEntityExtensionPublicationStorage );
                //this.Properties.SwapAndFlush();
                //this.UpdateRequested += ( ( EntityExtension ) extension ).PublicizeEntityProperties;
            }
            catch ( ArgumentNullException )
            {
                // LOG?
                throw;
            }
            catch ( ArgumentException argEx )
            {
                throw new DuplicateExtensionException( "This entity already has an extension with the name '" + extension.Name + "'", argEx );
            }

            extension.Attatch( this, this.Properties as IEntityExtensionPublicationStorage, this as IActionRequestable, this );
            this.Properties.SwapAndFlush();

            this.FireExtensionAddedEvent( extension );
        }

        public bool IsDisposing
        {
            get;
            private set;
        }
		~Entity()
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
            lock ( this.actionHandlersLockObject )
            {
                if ( this.actionHandlers != null )
                {
                    this.actionHandlers.Clear();
                    this.actionHandlers = null;
                }
            }

            lock ( this.extensionsLock )
            {
                if ( this.extensions != null )
                {
                    foreach ( KeyValuePair<string,IEntityExtension> kvp in this.extensions )
                    {
                        kvp.Value.Detatch( this, this );

                        // TODO: is this necessary?
                        kvp.Value.Dispose();
                    }

                    this.extensions.Clear();
                    this.extensions = null;
                }
            }

            lock ( this.propertiesLock )
            {
                if ( this.properties != null )
                {
                    this.properties.Dispose();
                    this.properties = null;
                }
            }

            lock ( this.updateRequestedLock )
            {
                if ( this.updateRequested != null )
                {
                    this.updateRequested = null;
                }
            }

            lock ( this.extensionAddedLock )
            {
                if ( this.extensionAdded != null )
                {
                    this.extensionAdded = null;
                }
            }

            lock ( this.extensionRemovedLock )
            {
                if ( this.extensionRemoved != null )
                {
                    this.extensionRemoved = null;
                }
            }
        }

        public bool Equals( IEntity other )
        {            
            if ( other == null ) return false;

            bool nameEqual = ( this.Name == null ) ? other.Name == null : this.Name == other.Name;

            return nameEqual;
        }

        public override bool Equals( object obj )
        {
            return this.Equals( obj as IEntity );
        }

        public object GetAttribute( string attributeId )
        {
            object result = null;

            if ( !this.properties.TryGetValue( attributeId, out result ) )
            {
                // LOG?
                // attribute not found
                // ignore, don't throw exception
            }

            return result;
        }

        public IEntityExtension GetExtension( string extensionName )
        {
            IEntityExtension result = null;

            lock ( this.extensionsLock )
            {
                this.extensions.TryGetValue( extensionName, out result );
            }

            return result;
        }

        public override int GetHashCode()
        {
            return ( this.Name == null ) ? 0 : this.Name.GetHashCode();
        }

        public void RegisterActionHandler( string actionKey, EventHandler<ActionRequestEventArgs> actionHandler )
        {
            if ( actionHandler == null )
            {
                throw new ArgumentNullException( "actionHandler", "The given action handler must not be null." );
            }

            if ( actionKey == null )
            {
                throw new ArgumentNullException( "actionKey", "The given action key must not be null." );
            }

            lock ( this.actionHandlersLockObject )
            {
                if ( this.actionHandlers.ContainsKey( actionKey ) )
                {
                    this.actionHandlers[ actionKey ] += actionHandler;
                }
                else
                {
                    EventHandler<ActionRequestEventArgs> handler = Delegate.Combine(actionHandler) as EventHandler<ActionRequestEventArgs>;
                    this.actionHandlers.Add( actionKey, handler );
                }
            }
        }

        public IEntityExtension RemoveExtension( string extensionName )
        {
            IEntityExtension removed = null;

            lock ( this.extensionsLock )
            {
                if ( this.extensions.TryGetValue( extensionName, out removed ) )
                {
                    this.extensions.Remove( extensionName );
                    //this.UpdateRequested -= ( ( EntityExtension ) removed ).PublicizeEntityProperties;
                }
                else
                {
                    throw new ExtensionNotFoundException( "The extension with name '" + extensionName + "' could not be found." );
                }                
            }

            removed.Detatch( this, this );

            this.FireExtensionRemovedEvent( removed );

            return removed;
        }

        public void RequestAction( object requestingSender, string actionKey, params object[] actionParameters )
        {
            DateTime requestTime = DateTime.Now;
            EventHandler<ActionRequestEventArgs> handlerDelegate = null;

            lock ( this.actionHandlersLockObject )
            {
                try
                {
                    this.actionHandlers.TryGetValue( actionKey, out handlerDelegate );
                }
                catch ( ArgumentNullException )
                {
                    // TODO: ERROR
                    // LOG?
                    // actionKey was null
                    throw;
                }

            }

            if ( handlerDelegate != null )
            {
                ActionRequestEventArgs args = new ActionRequestEventArgs( requestingSender, actionKey, requestTime, actionParameters );
                handlerDelegate.Invoke( requestingSender, args );
            }
        }

        public void UnregisterActionHandler( string actionKey, EventHandler<ActionRequestEventArgs> actionHandler, bool removeAll )
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

            lock ( this.actionHandlersLockObject )
            {
                if ( this.actionHandlers.TryGetValue( actionKey, out handler ) )
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

                    if(handler != null)
                    {
                        this.actionHandlers[ actionKey ] = handler;
                    }
                    else
                    {
                        this.actionHandlers.Remove( actionKey );
                    }
                }
                else
                {
                    // LOG?
                    // ignore since the name does not exist
                }
            }
        }

        #endregion Public Methods

        #endregion Methods
    }
}