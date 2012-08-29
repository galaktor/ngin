/**************************************
 * FILE:          TestExtension5.cs
 * DATE:          05.01.2010 10:25:43
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
using NGin.Core.Logging;
using NGin.Core.Scene;
using NGin.Core.Systems;
using NGin.Core.Configuration;
using NGin.Core.Messaging;
using System;

namespace NGin.Core.Test.Integration
{
    [EntityExtension( typeof( TestExtension5 ), null )]
    public class TestExtension5 : EntityExtension
    {
        private object something = null;

        public TestExtension5( string name, TestSystem5 system, ILogManager logManager )
            : base( name, system, logManager )
        {
            this.AddActionHandler( "test", this.TestActionHandler );
        }

        protected override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
        {
            publicDataStorage.TryGetValue( this.Name, out this.something );
        }

        protected override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
        {
            publicDataStorage.Publicize( this.Name, this.something );
        }

        private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
        {
        }

        protected override void Process( IActionRequestable actionRequestTarget )
        {
        }
    }
}
