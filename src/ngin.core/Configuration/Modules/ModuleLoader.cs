/**************************************
 * FILE:          ModuleLoader.cs
 * DATE:          05.01.2010 10:08:06
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
using System.Reflection;
using Autofac;
using NGin.Core.Exceptions;
using NGin.Core.Platform;

namespace NGin.Core.Configuration.Modules
{
    internal class ModuleLoader
    {
        public INGinConfig CoreConfig { get; private set; }

        public ModuleLoader( INGinConfig coreConfig )
        {            
            this.CoreConfig = coreConfig;
        }

        public IEnumerable<IModule> Load()
        {
            Dictionary<Type, IModule> modules = this.GetModulesFromModuleConfig( this.CoreConfig.GetModulesDeepCopy() );
            return modules.Values.ToList<IModule>();
        }

        private Dictionary<Type,IModule> GetModulesFromModuleConfig( IList<INGinModuleConfig> modulesFromConfig )
        {
            if ( modulesFromConfig == null )
            {
                throw new ArgumentNullException( "The given list of modules from config must not be null." );
            }

            Dictionary<Type, IModule> result = new Dictionary<Type, IModule>();

            foreach ( INGinModuleConfig module in modulesFromConfig )
            {
                string moduleTypeName = module.TypeFullName;
                //Type moduleType = null;

                if ( !String.IsNullOrEmpty( moduleTypeName ) )
                {
                    Type moduleType = InputOutputManager.CoreAssembly.GetType( moduleTypeName, false );
                    bool isModule = typeof( IModule ).IsAssignableFrom( moduleType );

                    // check if no result came back or
                    // if given type is not compatible with IModule
                    if ( moduleType == null || !isModule )
                    {
                        throw new CoreConfigException( "The module type '" + moduleTypeName + "' is invalid or could not be loaded." );
                    }

                    IModule autofacModule;
                    try
                    {
                        object[] moduleParams = new object[] { module };
                        autofacModule = moduleType.InvokeMember( moduleType.Name, BindingFlags.CreateInstance, null, null, moduleParams ) as IModule;
                    }
                    catch ( MissingMethodException mmEx )
                    {
                        throw new CoreConfigException( "The module type '" + moduleType.FullName + "' could not be instanciated. Make sure it has a matching constructor.", mmEx );
                    }

                    if ( autofacModule == null )
                    {
                        throw new InvalidOperationException( "An error has occurred while instanciation module of type '" + moduleTypeName + "'." );
                    }

                    result.Add( moduleType, autofacModule );
                }                
            }

            return result;
        }
    }
}
