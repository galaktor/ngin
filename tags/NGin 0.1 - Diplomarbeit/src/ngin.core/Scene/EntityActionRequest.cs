/**************************************
 * FILE:          EntityActionRequest.cs
 * DATE:          05.01.2010 10:16:56
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

namespace NGin.Core.Scene
{
    internal struct EntityExtensionActionRequest
    {    
        /// <summary>
        /// The key of the action that was requested.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The sender that requested the action.
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The parameters that were passed to be used by the action handler.
        /// </summary>
        public object[] ActionParameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityExtensionActionRequest"/> struct.
        /// </summary>
        /// <param name="sender">The sender that requests the action.</param>
        /// <param name="key">The key of the action.</param>
        /// <param name="actionParameters">The parameters to be used by the action.</param>
        public EntityExtensionActionRequest( object sender, string key, params object[] actionParameters ):this()
        {
            this.Sender = sender;
            this.Key = key;
            this.ActionParameters = actionParameters;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int keyCode = ( this.Key == null ) ? 0 : this.Key.GetHashCode();
            int senderCode = ( this.Sender == null ) ? 0 : this.Sender.GetHashCode();
            return keyCode ^ senderCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals( object obj )
        {
            if ( obj == null ) return false;
            if ( !( obj is EntityExtensionActionRequest ) ) return false;
            EntityExtensionActionRequest other = ( EntityExtensionActionRequest ) obj;
            return this.Key == other.Key && this.Sender == other.Sender;
        }   
    }
}
