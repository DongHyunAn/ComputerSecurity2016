using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if NOXAML 
	[assembly: AssemblyTitle("Telerik.Windows.Themes.Windows8Touch (No XAML)")]
#else
	[assembly: AssemblyTitle("Telerik.Windows.Themes.Windows8Touch")]
#endif

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
#if WPF
[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]
#endif
// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("97c02beb-08a3-4f67-8bfc-33e6abd33bcc")]