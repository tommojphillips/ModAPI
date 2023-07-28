// VERSION 1.4 | BUILD x.x.x with build excluded.
using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("ModApi")]
[assembly: AssemblyDescription("ModApi v0.1.4 BUILD 8")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyProduct("ModApi")]
[assembly: AssemblyCopyright("Tommo J. Productions Copyright © 2023")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("0.1.4")]
[assembly: AssemblyFileVersion("0.1.4.8")]

namespace TommoJProductions.ModApi
{
    
    /// <summary>
    /// Represents the version info for ModApi
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// Represents latest release version date. Format: dd:MM:yyyy hh:mm tt
        /// </summary>
	    public static readonly string lastestRelease = "28.07.2023 06:27 PM";
        /// <summary>
        /// Represents current version. (Excluding build number)
        /// </summary>
	    public static readonly string version = "0.1.4";
        /// <summary>
        /// Represents current full version . (including build number)
        /// </summary>
	    public static readonly string fullVersion = "0.1.4.8";
        /// <summary>
        /// Represents current build number. (excludes major, minor and revision numbers)
        /// </summary>
	    public static readonly string build = "8";

        /// <summary>
        /// Represents if the mod has been complied for x64
        /// </summary>
        #if x64
            internal static readonly bool IS_64_BIT = true;
        #else
            internal static readonly bool IS_64_BIT = false;
        #endif
        /// <summary>
        /// Represents if the mod has been complied in Debug mode
        /// </summary>
        #if DEBUG
            internal static readonly bool IS_DEBUG_CONFIG = true;
        #else
            internal const bool IS_DEBUG_CONFIG = false;
        #endif
    }
}
