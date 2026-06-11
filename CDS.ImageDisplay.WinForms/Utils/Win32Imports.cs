using System.Runtime.InteropServices;

namespace CDS.ImageDisplay.WinForms.Utils;

/// <summary>
/// Win32 API functions
/// </summary>
public static partial class Win32Imports
{
    /// <summary>
    /// Get the state of a key
    /// </summary>
#if NET48
    [DllImport("user32.dll")]
    internal static extern short GetKeyState(int nVirtKey);
#else
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport("user32.dll")]
    internal static partial short GetKeyState(int nVirtKey);
#endif


    /// <summary>
    /// Keycode for the space key
    /// </summary>
    internal const int VK_SPACE = 0x20;
}
