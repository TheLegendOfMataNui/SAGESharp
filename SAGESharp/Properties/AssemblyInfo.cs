using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SAGESharp")]
[assembly: AssemblyDescription("Libraries to manipulate data for the Saffire Advanced Game Engine (SAGE)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Litestone Studios")]
[assembly: AssemblyProduct("SAGESharp")]
[assembly: AssemblyCopyright("Copyright © Litestone 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ca7ebe43-febb-49f4-ad76-04075fba3b13")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.2")]
[assembly: AssemblyFileVersion("1.1.2")]

// The following lines make internal classes visible to tests and also
// the mocking framework. We don't want the signed assembly to expose
// any internal classes so that's guarded behind a macro.
#if !SIGN
[assembly: InternalsVisibleTo("SAGESharp.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
