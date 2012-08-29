/**************************************
 * FILE:          NGinModule.cs
 * DATE:          05.01.2010 10:08:23
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
using Autofac;
using Autofac.Builder;

namespace NGin.Core.Configuration.Modules
{
    internal abstract class NGinModule : Module, IModule
    {
        public INGinModuleConfig ModuleConfig { get; private set; }

        public NGinModule( INGinModuleConfig moduleConfig )
        {
            this.ModuleConfig = moduleConfig;
        }

        protected override void Load( ContainerBuilder builder )
        {
            this.RegisterComponents( builder );
            base.Load( builder );
        }


        #region IModule Member

        void IModule.Configure( IContainer container )
        {
            this.ConfigureContainer( container );
        }

        #endregion

        protected virtual void RegisterComponents( ContainerBuilder builder )
        { }
        protected virtual void ConfigureContainer( IContainer container )
        { }
    }
}
