/**************************************
 * FILE:          EntityPropertyStorage.cs
 * DATE:          05.01.2010 10:17:10
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
using System.Collections;
using System.Collections.Generic;

namespace NGin.Core.Scene
{
    internal class EntityPropertyStorage: IEntityExtensionPublicationStorage, IEntityExtensionPublicAqquisitionStorage, IEntityBufferedExtensionData
    {
        // main data
        private object dataLock = new object();
        internal Dictionary<string, object> MainDataStorage { get; set; }

        // change buffers
        private object stableBufferLock = new object();
        internal Dictionary<string, object> StableBufferStorage { get; set; }
        private object unstableBufferLock = new object();
        internal Dictionary<string, object> UnstableBufferStorage { get; set; }

        public IEntity Owner { get; private set; }

        public EntityPropertyStorage( IEntity owner )
        {
            if ( owner == null )
            {
                throw new ArgumentNullException( "owner", "The given owner must not be null." );
            }

            // save ref to owning entity
            this.Owner = owner;

            // initialize data structures
            this.MainDataStorage = new Dictionary<string, object>();
            this.UnstableBufferStorage = new Dictionary<string, object>();
            this.StableBufferStorage = new Dictionary<string, object>();
        }

        #region IEntityExtensionPublicationStorage Member       

        public IEnumerable<KeyValuePair<string, object>> StableBuffer
        {
            get
            {
                lock ( this.stableBufferLock )
                {
                    return ( ( IEnumerable<KeyValuePair<string, object>> ) this.StableBufferStorage );
                }
            }
        }

        public IEnumerable<KeyValuePair<string, object>> Data
        {
            get
            {
                lock ( this.dataLock )
                {
                    return ( ( IEnumerable<KeyValuePair<string, object>> ) this.MainDataStorage );
                }
            }
        }

        public IEnumerable<KeyValuePair<string, object>> UnstableBuffer
        {
            get
            {
                lock ( this.unstableBufferLock )
                {
                    return ( ( IEnumerable<KeyValuePair<string, object>> ) this.UnstableBufferStorage );
                }
            }
        }

        public void Publicize( string key, object data )
        {
            lock ( this.unstableBufferLock )
            {
                try
                {
                    // use Add to provoke ArgumentException if key already was publicized
                    this.UnstableBufferStorage.Add( key, data );
                    // Alternative would be to store directly to index of 'key' which would overwrite if existant
                }
                catch ( ArgumentException argEx )
                {
                    // LOG?
                    throw new BufferPublicationException( "The key '" + key + "' was already publicized into the buffer. This may only happen once per tick.", argEx );
                }
            }     
        }

        #endregion       
    
        #region IEntityBufferedData Member

        public void SwapAndFlush()
        {
            // SWAP
            lock ( this.unstableBufferLock )
            {
                // store ref to stable buffer
                var temp = this.StableBufferStorage;

                // TODO: I dislike having nested locks
                // can't think of a way to swap without nested lock a the moment
                lock ( this.stableBufferLock )
                {
                    // replacte stable buffer
                    this.StableBufferStorage = this.UnstableBufferStorage;
                }

                // replace unstable buffer
                this.UnstableBufferStorage = temp;

                // clear unstable data before allowing access to it
                this.UnstableBufferStorage.Clear();
            }

            // FLUSH
            // move data from new stable buffer to entity data
            lock ( this.dataLock )
            {
                lock ( this.stableBufferLock )
                {
                    foreach ( KeyValuePair<string, object> data in this.StableBufferStorage )
                    {
                        // replace all updated data
                        this.MainDataStorage[ data.Key ] = data.Value;
                    }
                }
            }
        }

        #endregion

        public bool ContainsProperty( string key )
        {
            lock ( this.dataLock )
            {
                return this.MainDataStorage.ContainsKey( key );
            }
        }

        public bool ContainsData( object value )
        {
            lock ( this.dataLock )
            {
                return this.MainDataStorage.ContainsValue( value );
            }
        }
       
        public bool TryGetValue( string key, out object value )
        {
            lock ( this.dataLock )
            {
                return this.MainDataStorage.TryGetValue( key, out value );
            }
        }        

        public object this[ string key ]
        {
            get
            {    
                object result = null;

                // locking already managed within this.TryGetValue
                this.TryGetValue( key, out result );

                return result;   
            }
        }
       
        public bool Contains( KeyValuePair<string, object> item )
        {
            lock ( this.dataLock )
            {
                return ( ( ICollection<KeyValuePair<string, object>> ) this.MainDataStorage ).Contains( item );
            }
        }

        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            lock ( this.dataLock )
            {
                ( ( ICollection<KeyValuePair<string, object>> ) this.MainDataStorage ).CopyTo( array, arrayIndex );
            }
        }

        public int Count
        {
            get
            {
                lock ( this.dataLock )
                {
                    return this.MainDataStorage.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                return true;
            }
        }       

        #region IEnumerable Member

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock ( this.dataLock )
            {
                return ( ( IEnumerable ) this.MainDataStorage ).GetEnumerator();
            }
        }

        #endregion

        #region IDisposable Member

        public void Dispose()
        {
            lock ( this.stableBufferLock )
            {
                if ( this.StableBufferStorage != null )
                {
                    this.StableBufferStorage.Clear();
                    this.StableBufferStorage = null;
                }
            }

            lock ( this.unstableBufferLock )
            {
                if ( this.UnstableBufferStorage != null )
                {
                    this.UnstableBufferStorage.Clear();
                    this.UnstableBufferStorage = null;
                }
            }

            lock ( this.dataLock )
            {
                if ( this.MainDataStorage != null )
                {
                    this.MainDataStorage.Clear();
                    this.MainDataStorage = null;
                }
            }

            this.Owner = null;
        }

        #endregion
    }
}
