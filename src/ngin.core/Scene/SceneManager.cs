/**************************************
 * FILE:          SceneManager.cs
 * DATE:          05.01.2010 10:18:34
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
using NGin.Core.Logging;

namespace NGin.Core.Scene
{   
    [Service( typeof( SceneManager ), typeof( ISceneManager ), true )]
    public class SceneManager : NGin.Core.Scene.ISceneManager
    {
        internal IEntityExtensionManager EntityExtensionManager { get; set; }
        internal ILogManager LogManager
        {
            get;
            set;
        }

        private object scenesLockObject = new object();
        private Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();
        public IEnumerable<IScene> Scenes 
        { 
            get 
            { 
                lock ( this.scenesLockObject ) 
                { 
                    return scenes.Values; 
                } 
            } 
        }

        public SceneManager( ILogManager logManager, IEntityExtensionManager extensionManager )
        {
            if ( logManager == null )
            {
                // ERROR
            }

            if ( extensionManager == null )
            {
                // ERROR
            }

            this.EntityExtensionManager = extensionManager;
            this.LogManager = logManager;
        }

        public IScene GetScene( string sceneName )
        {
            if ( String.IsNullOrEmpty( sceneName ) )
            {
                string message = "The given scene name must not be empty or null.";
                ArgumentException argEx = new ArgumentException( message, "sceneName" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            IScene result = null;

            lock ( this.scenesLockObject )
            {
                if ( !this.scenes.TryGetValue( sceneName, out result ) )
                {
                    // ERROR
                    // NOT FOUND - THROW EXCEPTION?
                    // alternative: ignore and return null
                }                
            }

            return result;
        }

        internal void AddScene( IScene scene, bool replaceExisting )
        {
            if ( scene == null )
            {
                ArgumentNullException argnEx = new ArgumentNullException( "scene" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Fatal, argnEx, argnEx.Message );
                throw argnEx;
            }

            lock ( this.scenesLockObject )
            {
                IScene existing;
                if ( !this.scenes.TryGetValue( scene.Name, out existing ) )
                {
                    this.scenes.Add( scene.Name, scene );
                }
                else
                {
                    if ( replaceExisting )
                    {
                        existing.Dispose();
                        this.scenes.Remove(scene.Name);
                    }

                    this.scenes.Add( scene.Name, scene );
                }
            }
        }

        public IScene CreateAndAddScene()
        {
            return this.CreateAndAddScene( "Scene_" + this.scenes.Count );
        }

        public IScene CreateAndAddScene( string sceneName )
        {
            if ( String.IsNullOrEmpty( sceneName ) )
            {
                string message = "The given scene name must not be empty or null.";
                ArgumentException argEx = new ArgumentException( message, "sceneName" );
                this.LogManager.Trace( Namespace.LoggerName, LogLevel.Error, argEx, message );
                throw argEx;
            }

            IScene result = null;

            result = new Scene( sceneName, this.EntityExtensionManager, this.LogManager );
            this.AddScene( result, true );

            return result;
        }        
    }
}
