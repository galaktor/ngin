/**************************************
 * FILE:          MessageManager.cs
 * DATE:          05.01.2010 10:16:19
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
using NGin.Core.Configuration;
using NGin.Core.Scene;
using NGin.Core.Logging;
using NGin.Core.Tasks;

namespace NGin.Core.Messaging
{
    [Service(typeof(MessageManager), typeof(IMessageManager), true)]
    public class MessageManager: IMessageManager
    {
        private object messageBufferLock = new object();
        private Queue<ActionRequestEventArgs> messageBuffer = new Queue<ActionRequestEventArgs>();

        internal int MessageBufferCount
        {
            get
            {
                lock ( this.messageBufferLock )
                {
                    return this.messageBuffer.Count;
                }
            }
        }

        private object messageHandlerAllMessagesLock = new object();
        private EventHandler<ActionRequestEventArgs> messageHandlerAllMessages;

        private Dictionary<string, EventHandler<ActionRequestEventArgs>> messageHandlers = new Dictionary<string, EventHandler<ActionRequestEventArgs>>();
        private object messageHandlersLockObject = new object();

        internal int GetInvocationCountForType(string messageKey)
        {
            if ( messageKey == null )
            {
                // handler for all messages
                lock ( this.messageHandlerAllMessagesLock )
                {
                    if ( this.messageHandlerAllMessages != null )
                    {
                        return messageHandlerAllMessages.GetInvocationList().Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                // other handlers
                lock ( this.messageHandlersLockObject )
                {
                    EventHandler<ActionRequestEventArgs> handler;
                    if ( this.messageHandlers.TryGetValue( messageKey, out handler ) )
                    {
                        return handler.GetInvocationList().Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<string, EventHandler<ActionRequestEventArgs>>> MessageHandlers
        {
            get
            {
                lock ( messageHandlersLockObject )
                {
                    return ( ( IEnumerable<KeyValuePair<string, EventHandler<ActionRequestEventArgs>>> ) this.messageHandlers );
                }
            }
        }

        private ILogManager LogManager { get; set; }

        public MessageManager( ILogManager logManager, IMainLoopManager mainLoopManager )
        {
            if ( logManager == null )
            {
                throw new ArgumentNullException( "logManager", "The log manager must not be null." );
            }

            if ( mainLoopManager == null )
            {
                throw new ArgumentNullException( "mainLoopManager", "The main loop manager must not be null." );
            }

            this.LogManager = logManager;
            mainLoopManager.HeartbeatEnded += new HeartbeatDelegate( this.MainLoopManager_HeartbeatEnded );
        }

        void MainLoopManager_HeartbeatEnded( TimeSpan timeSinceLast )
        {
            this.SendBufferedMessages();
        }

        #region IMessageManager Member

        public void RegisterMessageType( string messageKey, EventHandler<ActionRequestEventArgs> handler )
        {
            if ( handler == null )
            {
                throw new ArgumentNullException( "actionHandler", "The given action handler must not be null." );
            }

            if ( messageKey == null )
            {
                // handler for all messages
                lock ( this.messageHandlerAllMessagesLock )
                {
                    this.messageHandlerAllMessages += handler;
                }
            }
            else
            {
                // other handlers
                lock ( this.messageHandlersLockObject )
                {
                    if ( this.messageHandlers.ContainsKey( messageKey ) )
                    {
                        this.messageHandlers[ messageKey ] += handler;
                    }
                    else
                    {
                        this.messageHandlers.Add( messageKey, handler );
                    }
                }
            }
        }

        public void UnregisterMessageType( string messageKey, EventHandler<ActionRequestEventArgs> handler, bool removeAll )
        {
            if ( handler == null )
            {
                throw new ArgumentNullException( "actionHandler", "The given action handler must not be null." );
            }

            if ( messageKey == null )
            {
                lock ( this.messageHandlerAllMessagesLock )
                {
                    // handler for all messages
                    if ( this.messageHandlerAllMessages != null )
                    {
                        if ( removeAll )
                        {
                            // will remove all occurances of handler
                            Delegate.RemoveAll( messageHandlerAllMessages, handler );
                        }
                        else
                        {
                            // will remove ONLY the last occurance of handler
                            messageHandlerAllMessages -= handler;
                        }
                    }
                    else
                    {
                        // LOG?
                        // ignore since the name does not exist
                    }
                }
            }
            else
            {
                // other handlers
                EventHandler<ActionRequestEventArgs> registeredHandler = null;
                lock ( this.messageHandlersLockObject )
                {
                    if ( this.messageHandlers.TryGetValue( messageKey, out registeredHandler ) )
                    {
                        if ( removeAll )
                        {
                            // will remove all occurances of handler
                            registeredHandler = Delegate.RemoveAll( registeredHandler, handler ) as EventHandler<ActionRequestEventArgs>;
                        }
                        else
                        {
                            // will remove ONLY the last occurance of handler
                            registeredHandler -= handler;
                        }

                        if ( registeredHandler != null )
                        {
                            this.messageHandlers[ messageKey ] = registeredHandler;
                        }
                        else
                        {
                            this.messageHandlers.Remove( messageKey );
                        }
                    }
                    else
                    {
                        // LOG?
                        // ignore since the name does not exist
                    }
                }
            }
        }

        public void SendMessage( object requestingSender, string actionKey, params object[] actionParameters )
        {
            if ( actionKey == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "actionKey" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            DateTime requestTime = DateTime.Now;
            EventHandler<ActionRequestEventArgs> handlerDelegate = null;
            ActionRequestEventArgs args = new ActionRequestEventArgs( requestingSender, actionKey, requestTime, actionParameters );

            lock ( this.messageHandlersLockObject )
            {
                if ( this.messageHandlers.TryGetValue( actionKey, out handlerDelegate ) )
                {
                    this.AddMessageToBuffer( args );
                }
                else
                {
                    // handler unknown, ignore
                    this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "No handler for message type {0}.", actionKey );
                }            
            }

            lock ( this.messageHandlerAllMessagesLock )
            {
                if ( this.messageHandlerAllMessages != null )
                {
                    this.messageHandlerAllMessages( requestingSender, args );
                }
            }
        }

        internal void AddMessageToBuffer( ActionRequestEventArgs args )
        {
            if ( args == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "args", "The given event args must not be null." );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            lock ( this.messageBufferLock )
            {
                this.messageBuffer.Enqueue( args );
            }
        }

        public void SendBufferedMessages()
        {
            lock ( this.messageBufferLock )
            {
                while ( this.messageBuffer.Count > 0 )
                {
                    ActionRequestEventArgs args = this.messageBuffer.Dequeue();
                    EventHandler<ActionRequestEventArgs> handler = null;
                    if ( this.messageHandlers.TryGetValue( args.ActionKey, out handler ) )
                    {
                        handler.Invoke( args.RequestingSender, args );
                    }
                }
            }
        }

        #endregion

        public bool IsDisposing
        {
            get;
            private set;
        }
		~MessageManager()
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
            lock ( this.messageHandlersLockObject )
            {
                this.messageHandlers.Clear();
            }

            lock ( this.messageHandlerAllMessagesLock )
            {
                this.messageHandlerAllMessages = null;
            }

            lock ( this.messageBufferLock )
            {
                this.messageBuffer.Clear();
            }

            this.LogManager = null;
        }
    }
}
