/**************************************
 * FILE:          Scene.cs
 * DATE:          05.01.2010 10:18:29
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
using System.Linq;
using System.Text;
using System.Collections;
using NGin.Core.Systems;
using NGin.Core.Configuration;
using NGin.Core.Logging;

namespace NGin.Core.Scene
{
    public sealed class Scene: IScene
    {
        public bool IsDisposing
        {
            get;
            private set;
        }
		~Scene()
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

        private void DisposeUnmanaged()
        { }

        private void DisposeManaged()
        {
            this.ClearEntities();
            lock ( this.requiredSystemsLock )
            {
                if ( this.requiredSystems != null )
                {
                    this.requiredSystems.Clear();
                    this.requiredSystems = null;
                }
            }

            this.SystemsManager = null;
            this.LogManager = null;
            this.EntityExtensionManager = null;
            this.Name = null;
        }

        private object requiredSystemsLock = new object();
        private Dictionary<string, int> requiredSystems = new Dictionary<string, int>();
        public IEnumerable<string> RequiredSystems
        {
            get
            {
                lock ( this.requiredSystemsLock )
                {
                    return this.requiredSystems.Keys;
                }
            }
        }


        private object entitiesLock = new object();
        internal Dictionary<string, IEntity> entities = new Dictionary<string, IEntity>();
        public IEnumerable<IEntity> Entities
        {
            get
            {
                lock ( this.entitiesLock )
                {
                    return this.entities.Values;
                }
            }
        }

        public string Name { get; private set; }

        internal IEntityExtensionManager EntityExtensionManager { get; private set; }
        internal ILogManager LogManager { get; private set; }
        internal ISystemsManager SystemsManager { get; private set; }

        
        public Scene( string name, IEntityExtensionManager extensionManager, ILogManager logManager )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "logManager" );
            }

            this.LogManager = logManager;

            if ( String.IsNullOrEmpty( name ) )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "name" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            if ( extensionManager == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "extensionManager" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            this.EntityExtensionManager = extensionManager;
            this.Name = name;
        }

        public void Update()
        {
            lock ( this.entitiesLock )
            {
                foreach ( IEntity entity in this.entities.Values )
                {
                    ( ( Entity ) entity ).Flush();
                }
            }            
        }

        public IEntity CreateNewEntity( string entityName, params IEntityExtension[] entityExtensions )
        {
            IEntity result = null;

            result = new Entity( entityName, entityExtensions );

            return result;
        }

        public IEntityExtension CreateNewEntityExtension<TEntityExtension>( params object[] parameters )
        {
            IEntityExtension result;
            
            result = this.EntityExtensionManager.CreateNewEntityExtension<TEntityExtension>( parameters );

            return result;
        }

        internal void AddEntityRequiredSystem( IEntity entity )
        {            
            if ( entity == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "entity" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            foreach ( KeyValuePair<string, IEntityExtension> kvpExt in entity.Extensions )
            {
                ISystem extSystem = kvpExt.Value.System;

                this.AddRequiredSystem( extSystem );
            }
        }

        private void AddRequiredSystem( ISystem extSystem )
        {
            lock ( this.requiredSystemsLock )
            {
                string systemId = extSystem.Name;
                int count = 0;
                this.requiredSystems.TryGetValue( systemId, out count );
                this.requiredSystems[ systemId ] = count + 1;
            }
        }

        internal void RemoveEntityRequiredSystem( IEntity entity )
        {
            if ( entity == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "entity" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            foreach ( KeyValuePair<string, IEntityExtension> kvpExt in entity.Extensions )
            {
                ISystem extSystem = kvpExt.Value.System;

                this.RemoveRequiredSystem( extSystem );
            }
        }

        private void RemoveRequiredSystem( ISystem extSystem )
        {
            lock ( this.requiredSystemsLock )
            {
                string systemId = extSystem.Name;
                int count = 0;
                if ( this.requiredSystems.TryGetValue( systemId, out count ) )
                {
                    // type found, so decrement counter
                    count--;
                    if ( count == 0 )
                    {
                        this.requiredSystems.Remove( systemId );
                    }
                    else
                    {
                        this.requiredSystems[ systemId ] = count;
                    }
                }
            }
        }

        private void Entity_ExtensionAdded( IEntity ent, IEntityExtension ext )
        {
            this.AddRequiredSystem( ext.System );
        }
        private void Entity_ExtensionRemoved( IEntity ent, IEntityExtension ext )
        {
            this.RemoveRequiredSystem( ext.System );
        }
        public void AddEntity( IEntity entity )
        {
            if ( entity == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "entity" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            lock ( this.entitiesLock )
            {
                this.entities.Add( entity.Name, entity );
            }

            this.AddEntityRequiredSystem( entity );

            entity.ExtensionAdded += this.Entity_ExtensionAdded;
            entity.ExtensionRemoved += this.Entity_ExtensionRemoved;
        }

        public void ClearEntities()
        {
            lock ( this.entitiesLock )
            {
                if ( this.entities != null )
                {
                    IList<IEntity> removeEntities = new List<IEntity>( this.entities.Count );
                    foreach ( KeyValuePair<string, IEntity> entity in this.entities )
                    {
                        removeEntities.Add( entity.Value );
                    }

                    foreach ( IEntity entity in removeEntities )
                    {
                        this.RemoveEntity( entity );
                    }

                    removeEntities.Clear();
                    removeEntities = null;

                    this.entities.Clear();
                    this.entities = null;
                }                
            }
        }

        public void RemoveEntity( IEntity entity )
        {
            if ( entity == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "entity" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            lock ( this.entitiesLock )
            {
                if ( !this.entities.Remove( entity.Name ) )
                {
                    ArgumentException argEx = new ArgumentException( "entity" );
                    this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argEx, "The given entity was not found in this scene." );
                    throw argEx;
                }
            }

            this.RemoveEntityRequiredSystem( entity );

            entity.ExtensionAdded -= this.Entity_ExtensionAdded;
            entity.ExtensionRemoved -= this.Entity_ExtensionRemoved;
        }

        public override bool Equals( object obj )
        {
            IScene other = obj as IScene;

            if ( other == null )
            {
                return false;
            }

            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public bool TryGetEntity( string entityName, out IEntity entity )
        {
            entity = null;
            bool result = false;

            lock ( this.entitiesLock )
            {
                result = this.entities.TryGetValue( entityName, out entity );
            }

            return result;
        }
    }
}
