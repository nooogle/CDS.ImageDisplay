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
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport("user32.dll")]
    internal static partial short GetKeyState(int nVirtKey);


    /// <summary>
    /// Keycode for the space key
    /// </summary>
    internal const int VK_SPACE = 0x20;
}
