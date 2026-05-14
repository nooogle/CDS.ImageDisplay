using System.Runtime.InteropServices;

namespace CDS.ImageDisplay.Utils;

/// <summary>
/// Win32 API functions
/// </summary>
public static partial class Win32
{
    /// <summary>
    /// Get the state of a key
    /// </summary>
    [LibraryImport("user32.dll")]
    public static partial short GetKeyState(int nVirtKey);


    /// <summary>
    /// Keycode for the space key
    /// </summary>
    public const int VK_SPACE = 0x20;
}
