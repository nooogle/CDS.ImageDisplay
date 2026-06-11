using System.Text.Json;
using System.Text.Json.Serialization;
using CDS.ImageDisplay.WinForms.Utils;

namespace CDS.ImageDisplay.WinForms;

/// <summary>
/// Manages loading and saving strongly-typed settings serialised as JSON.
/// </summary>
/// <remarks>
/// The default constructor resolves the settings file to
/// <c>%AppData%\{Application.ProductName}\AppSettings_V2.json</c>.
/// Inject an explicit path and an <see cref="IFileSystem"/> via the secondary constructor
/// for deterministic unit testing.
/// </remarks>
public sealed class JSONSettingsManager<T> where T : new()
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = CreateJsonSerializerOptions();

    private readonly string _filePath;
    private readonly IFileSystem _fileSystem;

    /// <summary>Gets the current settings.</summary>
    public T Settings { get; private set; }

    /// <summary>
    /// Initialises the manager, resolving the settings file from
    /// <see cref="Environment.SpecialFolder.ApplicationData"/> / <see cref="Application.ProductName"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <see cref="Application.ProductName"/> is null or empty.
    /// </exception>
    public JSONSettingsManager()
        : this(ResolvePath(), new RealFileSystem())
    {
    }

    /// <summary>
    /// Initialises the manager with an explicit file path and file-system abstraction.
    /// The directory portion of <paramref name="filePath"/> is created if it does not exist.
    /// </summary>
    public JSONSettingsManager(string filePath, IFileSystem fileSystem)
    {
        if (string.IsNullOrEmpty(filePath)) { throw new ArgumentException("Value cannot be null or empty.", nameof(filePath)); }
        if (fileSystem is null) { throw new ArgumentNullException(nameof(fileSystem)); }

        _filePath = filePath;
        _fileSystem = fileSystem;

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !_fileSystem.DirectoryExists(directory))
        {
            _fileSystem.CreateDirectory(directory);
        }

        Settings = Load();
    }

    /// <summary>Serialises <see cref="Settings"/> to the JSON file.</summary>
    public void Save()
    {
        string json = JsonSerializer.Serialize(Settings, s_jsonSerializerOptions);
        _fileSystem.WriteAllText(_filePath, json);
    }

    private T Load()
    {
        if (!_fileSystem.FileExists(_filePath))
        {
            return new T();
        }

        string json = _fileSystem.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<T>(json, s_jsonSerializerOptions) ?? new T();
    }

    private static string ResolvePath()
    {
        string? appName = Application.ProductName;
        if (string.IsNullOrEmpty(appName))
        {
            throw new InvalidOperationException("Application.ProductName is null or empty.");
        }

        string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(userAppData, appName, "AppSettings_V2.json");
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        options.Converters.Add(new ColorJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}
