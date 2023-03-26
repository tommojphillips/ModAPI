// VERSION 1.1


using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("ModApi")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyProduct("ModApi")]
[assembly: AssemblyCopyright("Tommo J. Productions Copyright © 2023")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("0.1.4.5")]
[assembly: AssemblyFileVersion("0.1.4.5")]

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents the version info
    /// </summary>
    public class VersionInfo
{
    /// <summary>
    /// Rerpresents the latest release date
    /// </summary>
    public const string lastestRelease = "16.04.2023 08:23 PM";
    /// <summary>
    /// Represents the current version
    /// </summary>
    public const string version = "0.1.4.5";

/// <summary>
/// Represents if the mod has been complied for x64
/// </summary>
#if x64
            internal const bool IS_64_BIT = true;
#else
internal const bool IS_64_BIT = false;
#endif
/// <summary>
/// Represents if the mod has been complied in Debug mode
/// </summary>
#if DEBUG
            internal const bool IS_DEBUG_CONFIG = true;
#else
internal const bool IS_DEBUG_CONFIG = false;
        #endif
    }
}
