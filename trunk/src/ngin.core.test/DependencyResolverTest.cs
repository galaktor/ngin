/**************************************
 * FILE:          DependencyResolverTest.cs
 * DATE:          05.01.2010 10:22:43
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
using System.Text;
using NUnit.Framework;
using NGin.Core.Configuration;
using System.IO;
using System.Collections.ObjectModel;
using System.Reflection;
using NGin.Core.Logging;
using NMock2;
using NGin.Core.Configuration.Serialization;

namespace NGin.Core.Test.Unit
{
    [TestFixture]
    public class DependencyResolverTest
    {
        private Mockery mocks;

        [SetUp]
        public void SetUp()
        {
            this.mocks = new Mockery();
        }

        [TearDown]
        public void TearDown()
        {
            this.mocks.VerifyAllExpectationsHaveBeenMet();
            this.mocks.Dispose();
            this.mocks = null;
        }

        private DependenciesConfigXml CreateMockDependenciesConfig( params string[] probingDirectories )
        {
            DependenciesConfigXml result = new DependenciesConfigXml();
            result.Dependencies = new Collection<DirectoryXml>();

            foreach ( string probingDir in probingDirectories )
            {
                DirectoryXml dirXml = new DirectoryXml();
                dirXml.Location = probingDir;
                result.Dependencies.Add( dirXml );
            }

            return result;
        }

        [Test]
        public void DependencyResolver_CreateInstance_Success()
        {
            // arrange
            DependencyResolver dr;

            // act
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            dr = new DependencyResolver( dcMock );

            // assert
            Assert.IsNotNull( dr );
            Assert.IsInstanceOf<DependencyResolver>( dr );
        }

        [Test]
        public void AddProbingPath_ValidPathString_Success()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string probingPath = ".";

            // act
            dr.AddProbingPath( probingPath, false );

            // assert
            dr.ProbingPaths.Contains( probingPath );
        }

        [Test]
        public void ProbingPaths_Get_Success()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string path1 = ".";
            string path2 = "..";
            dr.AddProbingPath( path1, false );
            dr.AddProbingPath( path2, false );

            // act
            var probingPaths = dr.ProbingPaths;

            // assert
            Assert.IsNotNull( probingPaths );
            Assert.IsNotEmpty( probingPaths );
            Assert.AreEqual( 2, probingPaths.Count );
            Assert.IsTrue( probingPaths.Contains( Path.GetFullPath( path1 ) ) );
            Assert.IsTrue( probingPaths.Contains( Path.GetFullPath( path2 ) ) );
        }

        [Test]
        public void AddProbingPath_ValidDirectoryInfo_Success()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string probingPath = ".";
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo( probingPath );

            // act
            dr.AddProbingPath( directory, false );

            // assert
            dr.ProbingPaths.Contains( directory.FullName );
        }

        [Test, ExpectedException( typeof( DirectoryNotFoundException ) )]
        public void AddProbingPath_InvalidPathString_RaiseDirectoryNotFoundException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string directoryPath = "pathdoesnotexist";

            // act
            dr.AddProbingPath( directoryPath, false );

            // assert
        }

        [Test, ExpectedException( typeof( DirectoryNotFoundException ) )]
        public void AddProbingPath_InvalidDirectoryInfo_RaiseDirectoryNotFoundException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string directoryPath = "pathdoesnotexist";
            DirectoryInfo directory = new DirectoryInfo( directoryPath );

            // act
            dr.AddProbingPath( directory, false );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void AddProbingPath_PathIsNull_RaiseArgumentException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string path = null;

            // act
            dr.AddProbingPath( path, false );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void AddProbingPath_DirectoryInfoIsNull_RaiseArgumentException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            DirectoryInfo directory = null;

            // act
            dr.AddProbingPath( directory, false );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void AddProbingPath_DirectoryPathInvalid_RaiseArgumentException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string invalidDir = Path.GetInvalidFileNameChars()[ 0 ] + @"§$%&/";

            // act
            dr.AddProbingPath( invalidDir, false );

            // assert
        }

        [Test, ExpectedException( typeof( ArgumentException ) )]
        public void AddProbingPath_ProbingPathAlreadyAdded_RaiseArgumentException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            string directoryPath = ".";
            DirectoryInfo directory = new DirectoryInfo( directoryPath );

            // act
            dr.AddProbingPath( directory, false );
            dr.AddProbingPath( directory, false );

            // assert
        }

        [Test]
        public void ClearProbingPaths_AddTwoThenClear_NoPathsLeft()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            dr.AddProbingPath( ".", false );
            dr.AddProbingPath( "..", false );

            // act
            dr.ClearProbingPaths();

            // assert
            Assert.IsEmpty( dr.ProbingPaths );
        }

        [Test]
        public void TryResolveAssemblyDependency_ResolveKnownAssembly_Success()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            Assembly expectedAssembly = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName( expectedAssembly.Location );
            string assemblyName = expectedAssembly.FullName;
            dr.AddProbingPath( path, false );
            Assembly resolvedAssembly = null;

            // act
            resolvedAssembly = dr.TryResolveAssemblyDependency( assemblyName );

            // assert
            Assert.IsNotNull( resolvedAssembly );
            Assert.AreEqual( expectedAssembly, resolvedAssembly );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TryResolveAssemblyDependency_TypeNameNull_RaiseArgumentNulLException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            Assembly expectedAssembly = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName( expectedAssembly.Location );
            string assemblyName = null;
            dr.AddProbingPath( path, false );
            Assembly resolvedAssembly = null;

            // act
            resolvedAssembly = dr.TryResolveAssemblyDependency( assemblyName );

            // assert
        }

        [Test, ExpectedException( typeof( BadImageFormatException ) )]
        public void TryResolveAssemblyDependency_ResolveKnownAssembly_RaiseBadImageFormatException()
        {
            // arrange
            
            DependenciesConfigXml dcMock = this.CreateMockDependenciesConfig();
            DependencyResolver dr = new DependencyResolver( dcMock );
            Assembly expectedAssembly = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName( expectedAssembly.Location );
            string assemblyName = "InvalidDLL";
            dr.AddProbingPath( path, false );
            Assembly resolvedAssembly = null;

            // act
            resolvedAssembly = dr.TryResolveAssemblyDependency( assemblyName );

            // assert            
        }
    }
}
