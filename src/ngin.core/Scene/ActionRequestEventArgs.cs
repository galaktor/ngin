/**************************************
 * FILE:          ActionRequestEventArgs.cs
 * DATE:          05.01.2010 10:16:43
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
using System.Linq;

namespace NGin.Core.Scene
{
    [Serializable]
    public class ActionRequestEventArgs : EventArgs, IDisposable
    {
        public object RequestingSender { get; private set; }
        public string ActionKey { get; private set; }
        public DateTime RequestTime { get; private set; }
        public object[] ActionParameters { get; private set; }

        public ActionRequestEventArgs( object requestingSender, string actionKey, DateTime requestTime, params object[] actionParameters )
        {
            this.RequestingSender = requestingSender;
            this.ActionKey = actionKey;
            if ( requestTime == null )
            {
                this.RequestTime = DateTime.Now;
            }            
            else
            {
                this.RequestTime = requestTime;
            }
            this.ActionParameters = actionParameters;
        }

        #region IDisposable Member

        public void Dispose()
        {
            this.RequestingSender = null;
            this.ActionKey = null;
            this.RequestTime = default( DateTime );
            if ( this.ActionParameters != null )
            {
                for ( int i = 0; i < this.ActionParameters.Count<object>(); i++ )
                {
                    this.ActionParameters[ i ] = null;
                }
                this.ActionParameters = null;
            }
        }

        #endregion
    }
}
