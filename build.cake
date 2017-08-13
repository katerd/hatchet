#tool nuget:?package=NUnit.ConsoleRunner&version=3.6.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageVersion = Argument<string>("packageVersion");

Task("Restore-NuGet-Packages")
    .Does(() =>
    {
        NuGetRestore("./src/Hatchet/Hatchet.sln");

    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        MSBuild("./src/Hatchet/Hatchet.sln", settings =>
            settings.SetConfiguration(configuration));
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        NUnit3("./src/Hatchet.Tests/bin/" 
            + configuration + "/Hatchet.Tests.dll");

    });

Task("Create-NuGet-Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
    {
        CreateDirectory("_package");
        NuGetPack("./src/Hatchet/Hatchet.csproj",
            new NuGetPackSettings {
                Version = packageVersion,
                ArgumentCustomization = args => args.Append("-Prop Configuration=" + configuration),
                OutputDirectory = "./_package"
            });
    });

Task("Default")
    .IsDependentOn("Create-NuGet-Package");

RunTarget(target);