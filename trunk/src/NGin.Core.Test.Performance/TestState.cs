/**************************************
 * FILE:          TestState.cs
 * DATE:          05.01.2010 10:26:59
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
using NGin.Core.Configuration;
using NGin.Core.Logging;
using NGin.Core.Scene;
using NGin.Core.States;
using NGin.Core.States.RHFSM;
using NGin.Core.Systems;

namespace NGin.Core.Test.Performance
{
    [NGinRootState( typeof( TestState ) ), Service( typeof( TestState ), null, false )]
    public class TestState : State
    {
        public ISystemsManager SystemsManager { get; set; }
        public TestState( ILogManager logManager, ISceneManager sceneManager, ISystemsManager systemsManager )
            : base( "test", null, sceneManager, logManager )
        {
            this.SystemsManager = systemsManager;
        }

        public override void BuildUp()
        {
            this.Scene.AddEntity( this.Scene.CreateNewEntity( "test1", this.Scene.CreateNewEntityExtension<TestExtension>( "test1" ),
                                                                       this.Scene.CreateNewEntityExtension<TestExtension1>( "test2" ) ) );
        }

        public override void TearDown()
        {
            this.Scene.ClearEntities();
        }
    }
}
