/**************************************
 * FILE:          InputOutputManager.cs
 * DATE:          05.01.2010 10:16:29
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
using System.IO;
using System.Reflection;

namespace NGin.Core.Platform
{
    public class InputOutputManager
    {
        private static Dictionary<FileInfo, Stream> openStreams = new Dictionary<FileInfo, Stream>();

        public static Assembly CoreAssembly
        {
            get
            {
                return GetAssembly( typeof( NGinCore ) );
            }
        }

        public static FileStream OpenFile( string filePath, FileMode mode )
        {
            FileInfo file = new FileInfo( filePath );

            return OpenFile( file, mode );
        }

        public static FileStream OpenFile( FileInfo file, FileMode mode )
        {
            if ( file == null )
            {
                throw new ArgumentNullException( "file", "The given file must not be null." );
            }

            FileStream result = null;

            result = file.Open( mode );

            //// TODO: log exceptions
            //try
            //{
            //    result = file.Open( mode );
            //}
            //catch ( FileNotFoundException fnfEx )
            //{
            //    // TODO: LOG
            //    throw;
            //}
            //catch ( UnauthorizedAccessException uaEx )
            //{
            //    // TODO: LOG
            //    throw;
            //}
            //catch ( DirectoryNotFoundException dnfEx )
            //{
            //    // TODO: LOG
            //    throw;
            //}
            //catch ( IOException ioEx )
            //{
            //    // TODO: LOG
            //    throw;
            //}

            InputOutputManager.RegisterOpenStream( file, result );

            return result;
        }

        private static void RegisterOpenStream(string fileName, Stream stream)
        {
            if ( fileName == null )
            {
                throw new ArgumentNullException( "fileName" );
            }

            if ( stream == null )
            {
                throw new ArgumentNullException( "stream" );
            }

            FileInfo file = new FileInfo( fileName );

            RegisterOpenStream( file, stream );
        }

        private static void RegisterOpenStream( FileInfo file, Stream stream )
        {
            if ( file == null )
            {
                throw new ArgumentNullException( "file" );
            }

            if ( stream == null )
            {
                throw new ArgumentNullException( "stream" );
            }

            openStreams.Add( file, stream );
        }

        public static void CloseFile( string filePath )
        {
            if ( filePath == null )
            {
                throw new ArgumentNullException( "filePath" );
            }

            FileInfo fileToClose = new FileInfo( filePath );

            CloseFile( fileToClose );
        }

        public static void CloseFile( FileInfo fileToClose )
        {
            if ( fileToClose == null )
            {
                throw new ArgumentNullException( "fileToClose" );
            }

            if ( openStreams.ContainsKey( fileToClose ) )
            {
                Stream streamToClose = openStreams[ fileToClose ];
                streamToClose.Close();

                openStreams.Remove( fileToClose );
            }
        }

        public static Assembly LoadAssembly( FileInfo assemblyFile )
        {
            Assembly result = null;

            result = Assembly.LoadFrom( assemblyFile.FullName );

            //// TODO: Log exceptions
            //try
            //{
            //    result = Assembly.LoadFrom( assemblyFile.FullName );
            //}
            //catch ( Exception ex )
            //{
            //    // Log?
            //    throw;
            //}

            return result;
        }

        public static Assembly GetAssembly( Type containedType )
        {
            Assembly result = null;

            result = Assembly.GetAssembly( containedType );

            //// TODO: log exceptions
            //try
            //{
            //    result = Assembly.GetAssembly( containedType );
            //}
            //catch ( ArgumentNullException ex )
            //{
            //    // LOG
            //    throw;
            //}

            return result;
        }
    }
}
