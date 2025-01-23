// Define build parameters
var target = Argument("target", "Default");
var buildConfiguration = Argument("configuration", "Release");
var testConfiguration = "Debug"; // Run unit tests in Debug mode


// Paths
var solution = "./CDS.Imaging.sln";
var testProject = "./UnitTests/UnitTests.csproj";
var testResultsDirectory = "./TestResults";


// Clean directories
Task("Clean")
    .Does(() =>
{
    Information("🧹 Cleaning output directories...");
    CleanDirectory(testResultsDirectory);
    Information("✅ Clean complete.");
});

// Restore NuGet packages
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("📦 Restoring NuGet packages...");
    DotNetRestore(solution);
    Information("✅ NuGet restore complete.");
});

// Build the solution
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    try
    {
        Information($"🛠️ Building solution '{solution}' in '{buildConfiguration}' configuration...");

        DotNetClean(solution, new DotNetCleanSettings
        {
            Configuration = buildConfiguration
        });
        
        Information("✅ Solution cleaned.");

        //DotNetBuild(solution, new DotNetBuildSettings
       // {
         //   Configuration = buildConfiguration,
        //});

        MSBuild(solution, settings =>
        {
            settings
            .SetConfiguration(buildConfiguration)
            .SetVerbosity(Verbosity.Minimal);
        });

        Information($"✅ Build succeeded for configuration '{buildConfiguration}'.");
    }
    catch (Exception ex)
    {
        Error($"❌ Build failed: {ex.Message}");
        throw;
    }
});

// Run unit tests
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    try
    {
        Information($"🧪 Running unit tests in '{testConfiguration}' configuration...");

        DotNetTest(testProject, new DotNetTestSettings
        {
            Configuration = testConfiguration,
            NoBuild = false,
            ResultsDirectory = testResultsDirectory,
            ArgumentCustomization = args => args.Append("--arch x64 --logger trx")
        });

        Information("✅ Unit tests completed successfully.");
    }
    catch (Exception ex)
    {
        Error($"❌ Unit tests failed: {ex.Message}");
        throw;
    }
});

// Default target
Task("Default")
    .IsDependentOn("Test")
    .Does(() =>
{
    Information("🎉 Build and test process completed successfully!");
});


// Execute the specified task
RunTarget(target);
