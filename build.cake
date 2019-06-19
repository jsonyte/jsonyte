#tool "GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var nugetApiKey = Argument("nugetapikey", EnvironmentVariable("NUGET_API_KEY"));

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////
var version = "0.1.0";
var versionNumber = "0.1.0";

var artifacts = Directory("artifacts");
var solution = File("src/Jsonapi.sln");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() => 
{
    CleanDirectories("src/**/bin");
    CleanDirectories("src/**/obj");

    if (DirectoryExists(artifacts))
    {
        DeleteDirectory(artifacts, new DeleteDirectorySettings 
        {
            Recursive = true,
            Force = true
        });
    }
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => 
{
    DotNetCoreRestore(solution);
});

Task("Versioning")
    .IsDependentOn("Clean")
    .Does(() => 
{
    if (!BuildSystem.IsLocalBuild)
    {
        GitVersion(new GitVersionSettings
        {
            OutputType = GitVersionOutput.BuildServer
        });
    }

    var result = GitVersion();

    version = result.NuGetVersion;
    versionNumber = result.MajorMinorPatch;
});

Task("Build")
    .IsDependentOn("Versioning")
    .IsDependentOn("Restore")
    .Does(() => 
{
    CreateDirectory(artifacts);

    DotNetCoreBuild(solution, new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        ArgumentCustomization = x => x
            .Append("/p:Version={0}", version)
            .Append("/p:AssemblyVersion={0}", versionNumber)
            .Append("/p:FileVersion={0}", versionNumber)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    var projects = GetFiles("src/**/*.Tests.csproj");

    foreach (var project in projects)
    {
        DotNetCoreTest(project.FullPath, new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true
        });
    }
});

Task("Package")
    .IsDependentOn("Test")
    .Does(() => 
{
    DotNetCorePack(solution, new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = artifacts,
        NoBuild = true,
        NoRestore = true,
        ArgumentCustomization = x => x
            .Append("/p:Version={0}", version)
    });
});

Task("Publish")
    .IsDependentOn("Package")
    .WithCriteria(() => BuildSystem.IsRunningOnAppVeyor)
    .WithCriteria(() => AppVeyor.Environment.Repository.Tag.IsTag)
    .Does(() =>
{
    var packages = GetFiles("artifacts/**/*.nupkg");

    foreach (var package in packages)
    {
        DotNetCoreNuGetPush(package.FullPath, new DotNetCoreNuGetPushSettings
        {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = nugetApiKey
        });
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
