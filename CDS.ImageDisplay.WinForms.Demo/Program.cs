using System;
using System.Windows.Forms;

namespace CDS.ImageDisplay.WinForms.Demo;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
#if NET5_0_OR_GREATER
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var form = new FormTestLauncher();
        Application.Run(form);
    }
}
