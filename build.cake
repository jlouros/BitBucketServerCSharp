#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var versionSuffix = Argument<string>("versionSuffix", "");
var nugetApiKey = Argument<string>("nugetApiKey", "");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var mainProjDir = Directory("./src/BitBucketServerCSharp");
var unitTestsDir = Directory("./test/BitBucketServerCSharp.UnitTests");
var outputDir = Directory("./nupkgs");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(outputDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    var buildSettings = new DotNetCoreBuildSettings
    {
        Configuration = configuration
    };

    DotNetCoreBuild(mainProjDir, buildSettings);
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest(unitTestsDir);
});

Task("Create-NuGet-Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var packSettings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = outputDir,
        VersionSuffix = versionSuffix
    };

    DotNetCorePack(mainProjDir, packSettings);    
});

Task("Publish-NuGet-Package")
    .IsDependentOn("Create-NuGet-Package")
    .Does(() =>
{
    // Get the path to the package.
    var pkgToPublish = GetFiles(outputDir.Path.FullPath + "/BitBucketServerCSharp*.nupkg")
                        .Single(x => !x.FullPath.EndsWith("symbols.nupkg"));
    
    // Push the package.
    NuGetPush(pkgToPublish, new NuGetPushSettings 
    {
        Source = "https://www.nuget.org/",
        ApiKey = nugetApiKey
    });
    
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);