using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace CDS.Imaging.Demo
{
    internal class JSONSettingsManager<T> where T : new()
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = CreateJsonSerializerOptions();

        private readonly string filePath;
        
        public T Settings { get; private set; }


        /// <summary>
        /// Initialises a new instance of the SettingsManager class.
        /// </summary>
        /// <param name="fileName">The name of the settings file (default is "settings.json").</param>
        /// <param name="appFolderName">The name of the folder within Application Data (default is "MyApp").</param>
        public JSONSettingsManager()
        {
            // Obtain the path for the per-user Application Data folder.
            var userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string? applicationName = Application.ProductName;
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new InvalidOperationException("Application.ProductName is null or empty.");
            }

            // Create a folder for the application within the Application Data folder.
            var appFolderPath = Path.Combine(userAppData, applicationName);
            if (!Directory.Exists(appFolderPath))
            {
                Directory.CreateDirectory(appFolderPath);
            }

            filePath = Path.Combine(appFolderPath, "AppSettings_V2.json");
            Settings = Load(filePath);
        }

        /// <summary>
        /// Saves the settings by serialising them to JSON and writing to a file.
        /// </summary>
        /// <param name="settings">The settings object to save.</param>
        public void Save()
        {
            var json = JsonSerializer.Serialize(Settings, JsonSerializerOptions);
            File.WriteAllText(filePath, json);
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

            var json = File.ReadAllText(filePath) ?? string.Empty;
            T settings = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? new T();
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

            jsonSerializerOptions.Converters.Add(new CDS.Imaging.Utils.ColorJsonConverter());
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            return jsonSerializerOptions;
        }
    }
}
