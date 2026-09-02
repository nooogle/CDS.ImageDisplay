namespace UITests;

/// <summary>
/// Locates the built <c>CDS.ImageDisplay.WinForms.TestHarness.exe</c> on disk so tests can launch it.
/// </summary>
internal static class HarnessLauncher
{
    private const string HarnessProjectName = "CDS.ImageDisplay.WinForms.TestHarness";

    /// <summary>
    /// Resolves the full path to the harness executable, built as a sibling project.
    /// </summary>
    /// <exception cref="FileNotFoundException">The harness has not been built for the current configuration.</exception>
    public static string ResolveExePath()
    {
        string configuration =
#if DEBUG
            "Debug";
#else
            "Release";
#endif

        string candidate = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            HarnessProjectName, "bin", configuration, "net10.0-windows",
            $"{HarnessProjectName}.exe"));

        if (!File.Exists(candidate))
        {
            throw new FileNotFoundException(
                $"Test harness exe not found at '{candidate}'. Build {HarnessProjectName} first.",
                candidate);
        }

        return candidate;
    }
}
