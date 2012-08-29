/**************************************
 * FILE:          TestExtension1.cs
 * DATE:          05.01.2010 10:26:55
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

namespace NGin.Core.Test.Performance
{
    [EntityExtension( typeof( TestExtension1 ), null )]
    public class TestExtension1 : EntityExtension
    {
        private object something = null;

        public TestExtension1( string name, TestSystem system, ILogManager logManager )
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
            //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
            foreach ( object param in e.ActionParameters )
            {
                //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
            }
        }

        protected override void Process( IActionRequestable actionRequestTarget )
        {
            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
        }
    }

    //[EntityExtension( typeof( TestExtension2 ), null )]
    //public class TestExtension2 : EntityExtension
    //{
    //    private object something = null;

    //    public TestExtension2( string name, TestSystem2 system, ILogManager logManager )
    //        : base( name, system, logManager )
    //    {
    //        this.AddActionHandler( "test", this.TestActionHandler );
    //    }

    //    protected internal override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
    //    {
    //        publicDataStorage.TryGetValue( this.Name, out this.something );
    //    }

    //    protected internal override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
    //    {
    //        publicDataStorage.Publicize( this.Name, this.something );
    //    }

    //    private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
    //    {
    //        //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
    //        foreach ( object param in e.ActionParameters )
    //        {
    //            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
    //        }
    //    }

    //    protected override void Process( IActionRequestable actionRequestTarget )
    //    {
    //        //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
    //    }
    //}

    //[EntityExtension( typeof( TestExtension3 ), null )]
    //public class TestExtension3 : EntityExtension
    //{
    //    private object something = null;

    //    public TestExtension3( string name, TestSystem3 system, ILogManager logManager )
    //        : base( name, system, logManager )
    //    {
    //        this.AddActionHandler( "test", this.TestActionHandler );
    //    }

    //    protected internal override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
    //    {
    //        publicDataStorage.TryGetValue( this.Name, out this.something );
    //    }

    //    protected internal override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
    //    {
    //        publicDataStorage.Publicize( this.Name, this.something );
    //    }

    //    private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
    //    {
    //        //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
    //        foreach ( object param in e.ActionParameters )
    //        {
    //            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
    //        }
    //    }

    //    protected override void Process( IActionRequestable actionRequestTarget )
    //    {
    //        //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
    //    }
    //}

    //[EntityExtension( typeof( TestExtension4 ), null )]
    //public class TestExtension4 : EntityExtension
    //{
    //    private object something = null;

    //    public TestExtension4( string name, TestSystem4 system, ILogManager logManager )
    //        : base( name, system, logManager )
    //    {
    //        this.AddActionHandler( "test", this.TestActionHandler );
    //    }

    //    protected internal override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
    //    {
    //        publicDataStorage.TryGetValue( this.Name, out this.something );
    //    }

    //    protected internal override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
    //    {
    //        publicDataStorage.Publicize( this.Name, this.something );
    //    }

    //    private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
    //    {
    //        //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
    //        foreach ( object param in e.ActionParameters )
    //        {
    //            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
    //        }
    //    }

    //    protected override void Process( IActionRequestable actionRequestTarget )
    //    {
    //        //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
    //    }
    //}

    //[EntityExtension( typeof( TestExtension5 ), null )]
    //public class TestExtension5 : EntityExtension
    //{
    //    private object something = null;

    //    public TestExtension5( string name, TestSystem5 system, ILogManager logManager )
    //        : base( name, system, logManager )
    //    {
    //        this.AddActionHandler( "test", this.TestActionHandler );
    //    }

    //    protected internal override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
    //    {
    //        publicDataStorage.TryGetValue( this.Name, out this.something );
    //    }

    //    protected internal override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
    //    {
    //        publicDataStorage.Publicize( this.Name, this.something );
    //    }

    //    private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
    //    {
    //        //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
    //        foreach ( object param in e.ActionParameters )
    //        {
    //            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
    //        }
    //    }

    //    protected override void Process( IActionRequestable actionRequestTarget )
    //    {
    //        //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
    //    }
    //}

    //[EntityExtension( typeof( TestExtension6 ), null )]
    //public class TestExtension6 : EntityExtension
    //{
    //    private object something = null;

    //    public TestExtension6( string name, TestSystem6 system, ILogManager logManager )
    //        : base( name, system, logManager )
    //    {
    //        this.AddActionHandler( "test", this.TestActionHandler );
    //    }

    //    protected internal override void AqquirePublicEntityProperties( IEntityExtensionPublicAqquisitionStorage publicDataStorage )
    //    {
    //        publicDataStorage.TryGetValue( this.Name, out this.something );
    //    }

    //    protected internal override void PublicizeEntityProperties( IEntityExtensionPublicationStorage publicDataStorage )
    //    {
    //        publicDataStorage.Publicize( this.Name, this.something );
    //    }

    //    private void TestActionHandler( object requestingSender, ActionRequestEventArgs e )
    //    {
    //        //this.LogManager.Trace( Namespace.LoggerName, LogLevel.Debugging, "TestExtension: HANDLING action request: " + e.ActionKey );
    //        foreach ( object param in e.ActionParameters )
    //        {
    //            //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "Param: " + param.ToString() );
    //        }
    //    }

    //    protected override void Process( IActionRequestable actionRequestTarget )
    //    {
    //        //this.LogManager.Trace(Namespace.LoggerName, LogLevel.Debugging, "DO SOMETHING!" );
    //    }
    //}
}
