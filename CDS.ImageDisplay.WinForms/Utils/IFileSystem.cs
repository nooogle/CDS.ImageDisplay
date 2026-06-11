namespace CDS.ImageDisplay.WinForms.Utils;

/// <summary>
/// Abstraction over file-system operations, allowing deterministic unit testing
/// without touching the real file system.
/// </summary>
public interface IFileSystem
{
    /// <summary>Returns <see langword="true"/> if the specified file exists.</summary>
    bool FileExists(string path);

    /// <summary>Opens a file, reads all text, and closes the file.</summary>
    string ReadAllText(string path);

    /// <summary>
    /// Creates or overwrites a file, writes <paramref name="contents"/>, and closes the file.
    /// </summary>
    void WriteAllText(string path, string contents);

    /// <summary>Returns <see langword="true"/> if the specified directory exists.</summary>
    bool DirectoryExists(string path);

    /// <summary>Creates the specified directory and any necessary intermediate directories.</summary>
    void CreateDirectory(string path);
}
