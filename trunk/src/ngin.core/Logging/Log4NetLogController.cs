/**************************************
 * FILE:          Log4NetLogController.cs
 * DATE:          05.01.2010 10:15:55
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
using System.Xml;
using log4net;

namespace NGin.Core.Logging
{
    internal class Log4NetLogController : ILogController
    {
        internal bool LoggingActive { get; set; }

        #region Constructors

        #region Public Constructors

        public Log4NetLogController( XmlElement configXml )
        {
            log4net.Config.XmlConfigurator.Configure( configXml );

            // deactivate logging if default logger is set to level 'OFF'
            log4net.Repository.ILoggerRepository repo = log4net.LogManager.GetRepository( "log4net-default-repository" );
            if ( repo.Threshold == log4net.Core.Level.Off )
            {
                this.LoggingActive = false;
            }
            else
            {
                this.LoggingActive = true;
            }
        }

        #endregion Public Constructors

        #endregion Constructors

        #region Methods

        #region Public Methods

        public void Trace( string loggerName, LogLevel level, string message, params object[] stringParameters )
        {
            if ( this.LoggingActive )
            {
                ILog logger = log4net.LogManager.GetLogger( loggerName );

                switch ( level )
                {
                    case LogLevel.Debugging:
                        if ( logger.IsDebugEnabled )
                        {
                            logger.DebugFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Error:
                        if ( logger.IsErrorEnabled )
                        {
                            logger.ErrorFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Fatal:
                        if ( logger.IsFatalEnabled )
                        {
                            logger.FatalFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Information:
                        if ( logger.IsInfoEnabled )
                        {
                            logger.InfoFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Warning:
                        if ( logger.IsWarnEnabled )
                        {
                            logger.WarnFormat( message, stringParameters );
                        }
                        break;
                    default:
                        break;
                } 
            }
        }

        public void Trace( string loggerName, LogLevel level, Exception exception, string message, params object[] stringParameters )
        {
            if ( this.LoggingActive )
            {
                ILog logger = log4net.LogManager.GetLogger( loggerName );

                switch ( level )
                {
                    case LogLevel.Debugging:
                        if ( logger.IsDebugEnabled )
                        {
                            logger.DebugFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Error:
                        if ( logger.IsErrorEnabled )
                        {
                            logger.ErrorFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Fatal:
                        if ( logger.IsFatalEnabled )
                        {
                            logger.FatalFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Information:
                        if ( logger.IsInfoEnabled )
                        {
                            logger.InfoFormat( message, stringParameters );
                        }
                        break;
                    case LogLevel.Warning:
                        if ( logger.IsWarnEnabled )
                        {
                            logger.WarnFormat( message, stringParameters );
                        }
                        break;
                    default:
                        break;
                } 
            }
        }

        public void Shutdown()
        {
            log4net.Core.LoggerManager.Shutdown();
        }

        #endregion Public Methods

        #endregion Methods
    }
}