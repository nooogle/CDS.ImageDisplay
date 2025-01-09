# CDS.Imaging
Image processing and related utilities


## Building and deploying

### Making changes

1. Make code changes.
1. Add unit tests and/or demo code as appropriate.
1. Ensure that all unit tests pass.
1. Edit the AmberOCR project file and change the version, using the Semantic Versioning format.
1. Test the solution and tests formally by running the Cake script (see below).
1. Deploy the new NuGet package to the local NuGet folder (see below).


### Install Cake (one time only):
1. Open the solution and start the Developer Command Prompt for Visual Studio.
2. Run the following commands:
```shell
    dotnet new tool-manifest
    dotnet tool install Cake.Tool
    dotnet cake --version
```
3. Create a build script called build.cake in the root of the solution.


### Build using Cake

Start a Developer Command Prompt for Visual Studio and navigate to the root of the solution.
Run the following command:
```shell
dotnet cake
```

This will build the solution and run the tests. 
This does not deploy the library (see below).
