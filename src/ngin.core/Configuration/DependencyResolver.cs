/**************************************
 * FILE:          DependencyResolver.cs
 * DATE:          05.01.2010 10:12:42
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using NGin.Core.Configuration.Serialization;

namespace NGin.Core.Configuration
{
    public interface IDependencyResolver: INGinManager
    {
        
    }
    internal class DependencyResolver: IDependencyResolver
    {
        //protected internal ILogManager LogManager { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyResolver"/> class.
        /// </summary>
        public DependencyResolver( /* ILogManager logManager, */ DependenciesConfigXml dependenciesConfig )
        {
            //this.LogManager = logManager;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( this.CurrentDomain_AssemblyResolve );

            foreach ( DirectoryXml probingDirectory in dependenciesConfig.Dependencies )
            {
                this.AddProbingPath( probingDirectory.Location, probingDirectory.Recurse );
            }
        }



        private Collection<DirectoryInfo> probingPaths = new Collection<DirectoryInfo>();

        /// <summary>
        /// Gets the probing paths.
        /// </summary>
        /// <value>The probing paths.</value>
        protected internal StringCollection ProbingPaths
        {
            get
            {
                // create deep copy
                var result = new StringCollection();
                foreach ( DirectoryInfo probingPath in this.probingPaths )
                {
                    result.Add( probingPath.FullName );
                }

                return result;
            }
        }

        /// <summary>
        /// Handles the AssemblyResolve event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private System.Reflection.Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
        {
            return this.TryResolveAssemblyDependency( args.Name );
        }

        /// <summary>
        /// Tries the resolve assembly dependency.
        /// </summary>
        /// <param name="nameToResolve">The name to resolve.</param>
        /// <returns></returns>
        internal Assembly TryResolveAssemblyDependency( string nameToResolve )
        {
            Assembly result = null;

            if ( String.IsNullOrEmpty( nameToResolve ) )
            {
                throw new ArgumentNullException( "The name  to resolve must not be null.", "nameToResolve" );
            }

            if ( this.probingPaths.Count > 0 )
            {
                foreach ( DirectoryInfo probingDir in probingPaths )
                {
                    FileInfo[] dlls = probingDir.GetFiles( "*.dll" );
                    foreach ( FileInfo dll in dlls )
                    {
                        string extension = dll.Extension;
                        string name = dll.Name;
                        if ( dll.Name.Contains( extension ) )
                        {
                            // remove extension
                            name = name.Remove( name.IndexOf( extension ) );
                        }
                        if ( nameToResolve.Contains( name ) )
                        {
                            Assembly candidate = null;

                            candidate = Assembly.LoadFile( dll.FullName );

                            //// TODO: log exceptions
                            //try
                            //{
                            //    // LOG: trying assembly with name XxX
                            //    candidate = Assembly.LoadFile( dll.FullName );
                            //}
                            //catch ( BadImageFormatException biEx )
                            //{
                            //    // dll could not be loaded, it is probably not a managed assembly
                            //    // LOG: XxX has invalid format
                            //    throw;
                            //}

                            if ( candidate.FullName.Equals( nameToResolve ) )
                            {
                                result = candidate;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Adds a probing path.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        protected internal void AddProbingPath( string directoryPath, bool recurse )
        {
            if ( String.IsNullOrEmpty( directoryPath ) )
            {
                throw new ArgumentException( "The given directory path may not be null or empty: '" + directoryPath + "'", "directoryPath" );
            }

            DirectoryInfo directory = null;

            try
            {
                directory = new DirectoryInfo( directoryPath );
            }
            catch ( ArgumentException argEx )
            {             
                // LOG: XxX is not a valid directory path
                throw new ArgumentException( "The given directory path is not valid: '" + directoryPath + "'", "directoryPath", argEx );
            }

            this.AddProbingPath( directory, recurse );
        }

        /// <summary>
        /// Adds a probing path.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <exception cref="System.ArgumentException">Thrown when the specified <paramref name="directory"/>  is  <see langword="null"/> or is already contained.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown when the specified <paramref name="directory"/> does not exist.</exception>
        protected internal void AddProbingPath( DirectoryInfo directory, bool recurse)
        {
            if ( directory == null )
            {
                throw new ArgumentException( "The given directory may not be null.", "directory" );
            }

            if ( !directory.Exists )
            {
                throw new DirectoryNotFoundException( "The given directory does not exist: '" + directory.FullName + "'" );
            }

            if ( this.probingPaths.Contains( directory ) )
            {
                throw new ArgumentException( "The given directory already exists: '" + directory.FullName + "'", "directory" );
            }

            // add directory
            this.probingPaths.Add( directory );

            if(recurse)
            {
                DirectoryInfo[] subDirs = directory.GetDirectories();
                foreach ( DirectoryInfo subDir in subDirs )
                {
                    this.AddProbingPath( subDir, recurse );
                }
            }            
        }

        /// <summary>
        /// Clears the probing paths.
        /// </summary>
        protected internal void ClearProbingPaths()
        {
            this.probingPaths.Clear();
        }

        #region INGinManager Member

        private Guid id = new Guid();
        public Guid Id
        {
            get
            {
                return this.id;
            }
        }

        #endregion

        #region IEquatable<INGinManager> Member

        public bool Equals( INGinManager other )
        {
            return this.Id == other.Id;
        }

        #endregion
    }
}
