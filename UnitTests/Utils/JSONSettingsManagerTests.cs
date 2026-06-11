using AwesomeAssertions;
using CDS.ImageDisplay.WinForms;
using CDS.ImageDisplay.WinForms.Utils;

namespace UnitTests.Utils;

/// <summary>
/// Tests for <see cref="JSONSettingsManager{T}"/> using an in-memory file system.
/// </summary>
[TestClass]
public sealed class JSONSettingsManagerTests
{
    private const string TestFilePath = @"C:\TestApp\settings.json";
    private const string TestDirectory = @"C:\TestApp";

    private sealed class FakeFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);

        public int CreateDirectoryCallCount { get; private set; }

        public bool FileExists(string path) => _files.ContainsKey(path);
        public string ReadAllText(string path) => _files[path];
        public void WriteAllText(string path, string contents) => _files[path] = contents;
        public bool DirectoryExists(string path) => _directories.Contains(path);

        public void CreateDirectory(string path)
        {
            CreateDirectoryCallCount++;
            _directories.Add(path);
        }

        public void SeedFile(string path, string contents) => _files[path] = contents;
        public string? GetFile(string path) => _files.TryGetValue(path, out string? contents) ? contents : null;
    }

    private sealed class TestSettings
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    /// <summary>
    /// Verifies that an empty file path throws <see cref="ArgumentException"/>.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenFilePathIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();

        // Act
        Action action = () => new JSONSettingsManager<TestSettings>(string.Empty, fakeFs);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Verifies that a null file system throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenFileSystemIsNull_ThrowsArgumentNullException()
    {
        // Act
        Action action = () => new JSONSettingsManager<TestSettings>(TestFilePath, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that the settings directory is created when it does not yet exist.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenDirectoryDoesNotExist_CreatesDirectory()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();

        // Act
        _ = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);

        // Assert
        fakeFs.CreateDirectoryCallCount.Should().Be(1);
    }

    /// <summary>
    /// Verifies that <see cref="IFileSystem.CreateDirectory"/> is not called when the directory already exists.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenDirectoryAlreadyExists_DoesNotCreateDirectory()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();
        fakeFs.CreateDirectory(TestDirectory);
        int callsBefore = fakeFs.CreateDirectoryCallCount;

        // Act
        _ = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);

        // Assert
        fakeFs.CreateDirectoryCallCount.Should().Be(callsBefore);
    }

    /// <summary>
    /// Verifies that <see cref="JSONSettingsManager{T}.Settings"/> is a default instance when no file exists.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenFileDoesNotExist_SettingsIsDefault()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();

        // Act
        var manager = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);

        // Assert
        manager.Settings.Should().BeEquivalentTo(new TestSettings());
    }

    /// <summary>
    /// Verifies that settings are deserialized from an existing JSON file.
    /// </summary>
    [TestMethod]
    public void Constructor_WhenFileContainsValidJson_SettingsIsDeserialized()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();
        fakeFs.SeedFile(TestFilePath, """{"Name":"Alice","Count":42}""");

        // Act
        var manager = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);

        // Assert
        manager.Settings.Should().BeEquivalentTo(new TestSettings { Name = "Alice", Count = 42 });
    }

    /// <summary>
    /// Verifies that <see cref="JSONSettingsManager{T}.Save"/> writes the settings as JSON to the file.
    /// </summary>
    [TestMethod]
    public void Save_WhenCalled_WritesSettingsAsJson()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();
        var manager = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);
        manager.Settings.Name = "Bob";
        manager.Settings.Count = 7;

        // Act
        manager.Save();

        // Assert
        string? json = fakeFs.GetFile(TestFilePath);
        json.Should().NotBeNull();
        json.Should().Contain("\"Bob\"");
        json.Should().Contain("7");
    }

    /// <summary>
    /// Verifies that a save followed by a load in a new manager instance produces identical settings.
    /// </summary>
    [TestMethod]
    public void Save_ThenLoadInNewInstance_RestoresSettings()
    {
        // Arrange
        var fakeFs = new FakeFileSystem();
        var original = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);
        original.Settings.Name = "Charlie";
        original.Settings.Count = 99;
        original.Save();

        // Act
        var restored = new JSONSettingsManager<TestSettings>(TestFilePath, fakeFs);

        // Assert
        restored.Settings.Should().BeEquivalentTo(new TestSettings { Name = "Charlie", Count = 99 });
    }
}
