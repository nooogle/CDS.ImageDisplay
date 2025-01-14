using System.Runtime.InteropServices;

namespace CDS.Imaging.WinForms
{
    /// <summary>
    /// Win32 API functions
    /// </summary>
    public static class Win32
    {
        /// <summary>
        /// Get the state of a key
        /// </summary>
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);


        /// <summary>
        /// Keycode for the space key
        /// </summary>
        public const int VK_SPACE = 0x20;
    }
}
