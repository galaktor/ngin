using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle( "NGin.Core" )]
[assembly: AssemblyDescription( "The Core library for the NGin game engine." )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Raphael Estrada" )]
[assembly: AssemblyProduct( "NGin.Core" )]
[assembly: AssemblyCopyright( "Copyright © Raphael Estrada 2009" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible( false )]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid( "e9f39327-63ba-411f-8d16-8590d4bf95a3" )]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:

[assembly: AssemblyInformationalVersion( "0.0.0.0" )]
[assembly: AssemblyVersion( "0.0.0.0" )]
[assembly: AssemblyFileVersion( "0.0.0.0" )]

[assembly: System.CLSCompliant( true )]

// When debugging, allow tests to access internal core types and members
//#if DEBUG
[assembly: InternalsVisibleTo( @"NGin.Core.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100bb6fe1c522e0e45c75e3b1320e7a029a111a538650ec417a9704928eeb3f7ac7ead3fc98fcf70ec8c6318a3783a9db2910bf2cec3c0181fb5d1daa0f7c9ca638536206c02a11ca2ec59878aa229c123e26313462016607d5eac7b45a73755ad66d8eb43bb629d41592eddd8a44e31f2a15ce49543b5e139b0cea90433156c0a8" )]
[assembly: InternalsVisibleTo( @"NGin.Core.Test.Integration, PublicKey=0024000004800000940000000602000000240000525341310004000001000100bb6fe1c522e0e45c75e3b1320e7a029a111a538650ec417a9704928eeb3f7ac7ead3fc98fcf70ec8c6318a3783a9db2910bf2cec3c0181fb5d1daa0f7c9ca638536206c02a11ca2ec59878aa229c123e26313462016607d5eac7b45a73755ad66d8eb43bb629d41592eddd8a44e31f2a15ce49543b5e139b0cea90433156c0a8" )]
//[assembly: InternalsVisibleTo( @"NGin.Core.Test" )]
//[assembly: InternalsVisibleTo( @"NGin.Core.Test.Integration" )]
//#endif