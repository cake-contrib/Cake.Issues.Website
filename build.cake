#load nuget:?package=Cake.Wyam.Recipe&version=0.6.0

#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Yaml&version=3.0.0"
#addin "nuget:https://api.nuget.org/v3/index.json?package=YamlDotNet&version=5.2.1"
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.FileHelpers&version=3.1.0"

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////

private const string GitReleaseManagerTool = "#tool nuget:?package=gitreleasemanager&version=0.7.1";

// Definitions
class AddinSpec
{
    public string Name { get; set; }
    public string NuGet { get; set; }
    public bool Prerelease { get; set; }
    public List<string> Assemblies { get; set; }
    public string RepositoryOwner { get; set; }
    public string RepositoryName { get; set; }
    public string Documentation { get; set; }
    public string ReleaseNotesFilePath { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public List<string> Categories { get; set; }
}

// Variables
var addinDir = Directory("./release/addins");
var addinSpecs = new List<AddinSpec>();

// Cake.Wyam.Recipe parameters
Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    title: "Cake.Issues.Website",
    repositoryOwner: "cake-contrib",
    repositoryName: "Cake.Issues.Website",
    webHost: "cake-contrib.github.io",
    webLinkRoot: "Cake.Issues.Website",
    wyamRecipe: "Docs",
    wyamTheme: "Samson",
    shouldPublishDocumentation: StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch));

BuildParameters.PrintParameters(Context);

//////////////////////////////////////////////////////////////////////
// CUSTOM TASKS
//////////////////////////////////////////////////////////////////////

Task("CleanAddinPackages")
    .Does(() =>
{
    CleanDirectory(addinDir);
});

Task("GetAddinSpecs")
    .Does(() =>
{
    var addinSpecFiles = GetFiles("./addins/*.yml");
    addinSpecs
        .AddRange(addinSpecFiles
            .Select(x =>
            {
                Verbose("Deserializing addin YAML from " + x);
                return DeserializeYamlFromFile<AddinSpec>(x);
            })
        );

    foreach (var addinSpec in addinSpecs.Where(x => x.Assemblies != null).SelectMany(x => x.Assemblies).Select(x => "../release/addins" + x))
    {
        Verbose("Add '{0}' to Wyam", addinSpec);
        BuildParameters.WyamAssemblyFiles.Add(addinSpec);
    }
});

Task("GetAddinPackages")
    .IsDependentOn("CleanAddinPackages")
    .IsDependentOn("GetAddinSpecs")
    .Does(() =>
    {
        var packagesPath = MakeAbsolute(Directory("./output")).Combine("packages");
        foreach(var addinSpec in addinSpecs.Where(x => !string.IsNullOrEmpty(x.NuGet)))
        {
            Information("Installing addin package " + addinSpec.NuGet);
            NuGetInstall(addinSpec.NuGet,
                new NuGetInstallSettings
                {
                    OutputDirectory = addinDir,
                    Prerelease = addinSpec.Prerelease,
                    Verbosity = NuGetVerbosity.Quiet,
                    Source = new [] { "https://api.nuget.org/v3/index.json" },
                    NoCache = true,
                    EnvironmentVariables = 
                        new Dictionary<string, string>
                        {
                            {"EnableNuGetPackageRestore", "true"},
                            {"NUGET_XMLDOC_MODE", "None"},
                            {"NUGET_PACKAGES", packagesPath.FullPath},
                            {"NUGET_EXE",  Context.Tools.Resolve("nuget.exe").FullPath }
                        }
                });
        }
    });

Task("GetReleaseNotes")
    .IsDependentOn("GetAddinSpecs")
    .WithCriteria(!string.IsNullOrEmpty(BuildParameters.Wyam.AccessToken))
    .Does(() => RequireTool(GitReleaseManagerTool, () =>
    {
        var packagesPath = MakeAbsolute(Directory("./output")).Combine("packages");
        foreach(var addinSpec in addinSpecs.Where(x => !string.IsNullOrEmpty(x.RepositoryOwner) && !string.IsNullOrEmpty(x.RepositoryName) && !string.IsNullOrEmpty(x.ReleaseNotesFilePath)))
        {
            Information("Retrieving release notes for " + addinSpec.Name);
            GitReleaseManagerExport("pat", BuildParameters.Wyam.AccessToken, addinSpec.RepositoryOwner, addinSpec.RepositoryName, addinSpec.ReleaseNotesFilePath);

            Information("Adding metadata for " + addinSpec.Name);
            string fileContent = FileReadText(addinSpec.ReleaseNotesFilePath);
            DeleteFile(addinSpec.ReleaseNotesFilePath);
            var title = addinSpec.Categories.Contains("Core") ? addinSpec.Name : "Release Notes";
            var frontMatter =
                new List<string>
                {
                    "---",
                    "Title: " + title,
                    "Description: Release notes for " + addinSpec.Name,
                    "---"
                };
            fileContent = string.Join("\r\n", frontMatter) + "\r\n" + fileContent;
            FileWriteText(addinSpec.ReleaseNotesFilePath, fileContent);
        }
    }));

Task("GetArtifacts")
    .IsDependentOn("GetAddinPackages")
    .IsDependentOn("GetReleaseNotes");

BuildParameters.Tasks.BuildDocumentationTask
    .IsDependentOn("GetArtifacts");

BuildParameters.Tasks.PreviewDocumentationTask
    .IsDependentOn("GetAddinSpecs");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();
