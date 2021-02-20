//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var artifactsDir = "./artifacts";
var projectToPackage = "./src/SqlAlias";

var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

GitVersion gitVersionInfo;
string nugetVersion;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    gitVersionInfo = GitVersion(new GitVersionSettings {
        OutputType = GitVersionOutput.Json
    });
    nugetVersion = gitVersionInfo.NuGetVersion;

    Information("Building SqlAlias v{0}", nugetVersion);
    if(BuildSystem.IsRunningOnAppVeyor)
        AppVeyor.UpdateBuildVersion(gitVersionInfo.NuGetVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("Restore")
    .Does(() => DotNetCoreRestore("./src"));


Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild("./src", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
        DotNetCoreTest("./src/Tests/Tests.csproj", new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true
        });
});


Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePack(projectToPackage, new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = artifactsDir,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

Task("PushPackages")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var package = $"{artifactsDir}/SqlAlias.{nugetVersion}.nupkg";
    var localPackagesDir = "../LocalPackages";

    if(DirectoryExists(localPackagesDir))
        CopyFileToDirectory(package, localPackagesDir);

    if(isContinuousIntegrationBuild && gitVersionInfo.PreReleaseTag == "")
    {
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NuGetApiKey"),
            Timeout = TimeSpan.FromMinutes(20)
        });
    }

});


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("PushPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);