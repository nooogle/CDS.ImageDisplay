using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace CDS.ImageDisplay.Demo;

internal sealed class JSONSettingsManager<T> where T : new()
{
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = CreateJsonSerializerOptions();

    private readonly string _filePath;

    public T Settings { get; private set; }


    /// <summary>
    /// Initialises a new instance of the <see cref="JSONSettingsManager{T}"/> class.
    /// </summary>
    public JSONSettingsManager()
    {
        // Obtain the path for the per-user Application Data folder.
        string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string? applicationName = Application.ProductName;
        if (string.IsNullOrEmpty(applicationName))
        {
            throw new InvalidOperationException("Application.ProductName is null or empty.");
        }

        // Create a folder for the application within the Application Data folder.
        string appFolderPath = Path.Combine(userAppData, applicationName);
        if (!Directory.Exists(appFolderPath))
        {
            Directory.CreateDirectory(appFolderPath);
        }

        _filePath = Path.Combine(appFolderPath, "AppSettings_V2.json");
        Settings = Load(_filePath);
    }

    /// <summary>
    /// Saves the settings by serialising them to JSON and writing to a file.
    /// </summary>
    public void Save()
    {
        string json = JsonSerializer.Serialize(Settings, s_jsonSerializerOptions);
        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Loads the settings by reading from the JSON file and deserialising them.
    /// If the file does not exist, a new instance of T is returned.
    /// </summary>
    /// <returns>The deserialised settings object.</returns>
    private static T Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new T();
        }

        string json = File.ReadAllText(filePath) ?? string.Empty;
        T settings = JsonSerializer.Deserialize<T>(json, s_jsonSerializerOptions) ?? new T();
        return settings;
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        jsonSerializerOptions.Converters.Add(new CDS.ImageDisplay.Utils.ColorJsonConverter());
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        return jsonSerializerOptions;
    }
}
