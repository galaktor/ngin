/**************************************
 * FILE:          LogManager.cs
 * DATE:          05.01.2010 10:16:04
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


using NGin.Core.Exceptions;

namespace NGin.Core.Logging
{
    internal class LogManager : ILogManager
    {
        #region Constructors

        #region Other Constructors

        public LogManager( /* IConfigManager configManager,*/ ILogController logController)
        {
            //System.Xml.XmlElement configXml = configManager.GetSectionXml( "log4net" );
            //this.LogController = new Log4NetLogController( "NGin.Core", configXml );

            if ( logController == null )
            {
                throw new ManagerInitializationException("Dependency ILogController was null.");
            }

            this.LogController = logController;
        }

        #endregion Other Constructors

        #endregion Constructors

        #region Properties

        #region Internal Properties

        private ILogController LogController{ get; set; }

        #endregion Internal Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        public void Trace( string loggerName, LogLevel level, string message, params object[] stringParameters )
        {
            this.LogController.Trace( loggerName, level, message, stringParameters );
        }

        public void Trace( string loggerName, LogLevel level, Exception exception, string message, params object[] stringParameters )
        {
            this.LogController.Trace( loggerName, level, exception, message, stringParameters );
        }

        #endregion Public Methods

        #endregion Methods
    }
}