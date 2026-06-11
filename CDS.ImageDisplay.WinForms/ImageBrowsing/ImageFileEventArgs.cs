namespace CDS.ImageDisplay.WinForms.ImageBrowsing;

/// <summary>
/// Event arguments carrying the full path of an image file.
/// </summary>
public sealed class ImageFileEventArgs : EventArgs
{
    /// <summary>Gets the full path of the image file.</summary>
    public string FilePath { get; }

    /// <inheritdoc cref="ImageFileEventArgs"/>
    public ImageFileEventArgs(string filePath)
    {
        FilePath = filePath;
    }
}
