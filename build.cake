#addin nuget:?package=Octokit&version=7.1.0
#load nuget:?package=Cake.Wyam.Recipe&version=2.0.1

#load build\build.cake

using Octokit;

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////

private const string GitReleaseManagerTool = "#tool nuget:?package=gitreleasemanager&version=0.11.0";

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
    webHost: "cakeissues.net",
    wyamRecipe: "Docs",
    wyamTheme: "Samson",
    shouldPublishDocumentation: StringComparer.OrdinalIgnoreCase.Equals("master", AppVeyor.Environment.Repository.Branch));

ToolSettings.SetToolPreprocessorDirectives(
    kuduSyncGlobalTool: "#tool dotnet:https://pkgs.dev.azure.com/cake-contrib/Home/_packaging/addins/nuget/v3/index.json?package=KuduSync.Tool&version=1.5.4-g13cb5857b6");

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

Task("GetAddinDocumentation")
    .IsDependentOn("CleanAddinPackages")
    .IsDependentOn("GetAddinSpecs")
    .Does(() =>
    {
        foreach(var addinSpec in addinSpecs.Where(x => !string.IsNullOrEmpty(x.RepositoryDocumentationPath) && !string.IsNullOrEmpty(x.DocumentationLink)))
        {
            Information("Cloning " + addinSpec.RepositoryName + "...");
            RepositoryHelper.GitCopyFromRepository(
                Context,
                new Uri("https://github.com/" + addinSpec.RepositoryOwner + "/" + addinSpec.RepositoryName),
                new List<DirectoryPath> {addinSpec.RepositoryDocumentationPath},
                "input" + addinSpec.DocumentationLink);
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
            GitReleaseManagerExport(
                BuildParameters.Wyam.AccessToken,
                addinSpec.RepositoryOwner,
                addinSpec.RepositoryName,
                addinSpec.ReleaseNotesFilePath);

            Information("Adding metadata for " + addinSpec.Name);
            string fileContent = FileReadText(addinSpec.ReleaseNotesFilePath);
            DeleteFile(addinSpec.ReleaseNotesFilePath);
            var frontMatter =
                new List<string>
                {
                    "---",
                    "Title: " + addinSpec.Name + " Release Notes",
                    "Description: Release notes for " + addinSpec.Name,
                    "---"
                };
            var releaseNotesContent = string.Join("\r\n", frontMatter) + "\r\n";
            if (addinSpec.ReleaseNotesHeader != null)
            {
                releaseNotesContent += "<p>" + string.Join("\r\n", addinSpec.ReleaseNotesHeader) + "</p>\r\n";
            }
            releaseNotesContent += fileContent;

            FileWriteText(addinSpec.ReleaseNotesFilePath, releaseNotesContent);
        }
    }));

Task("GetIssues")
    .Does(() => {
        CleanDirectory("./input/issues/");
        var appName = "cake-issues-websit";
        var organization = "cake-contrib";
        var label = "help wanted";
        var topic = "topic:cake-issues";

        var client = new GitHubClient(new Octokit.ProductHeaderValue(appName));
        if (!string.IsNullOrEmpty (BuildParameters.Wyam.AccessToken))
        {
            var tokenAuth = new Octokit.Credentials(BuildParameters.Wyam.AccessToken);
            client.Credentials = tokenAuth;
        }

        var repositoryRequest = new SearchRepositoriesRequest(topic)
        {
            User = organization
        };

        var repositoryResult = client.Search.SearchRepo(repositoryRequest).GetAwaiter().GetResult();

        var issuesRequest = new SearchIssuesRequest();
        issuesRequest.Labels = new []{ label };
        issuesRequest.State = ItemState.Open;

        foreach (var repoEntry in repositoryResult.Items)
        {
            issuesRequest.Repos.Add(repoEntry.FullName);
        }

        var issuesResult = client.Search.SearchIssues(issuesRequest).GetAwaiter().GetResult();
        foreach (var issuesEntry in issuesResult.Items)
        {
            FileWriteText($"./input/issues/{issuesEntry.Id}.yml",
$@"Number: {issuesEntry.Number}
HtmlUrl: {issuesEntry.HtmlUrl}
Title: ""{issuesEntry.Title}""
Repository: {issuesEntry.HtmlUrl.Split('/')[4]}");
        }
    });

Task("GetArtifacts")
    .IsDependentOn("GetAddinDocumentation")
    .IsDependentOn("GetAddinPackages")
    .IsDependentOn("GetReleaseNotes")
    .IsDependentOn("GetIssues");

BuildParameters.Tasks.BuildDocumentationTask
    .IsDependentOn("GetArtifacts");

BuildParameters.Tasks.PreviewDocumentationTask
    .IsDependentOn("GetAddinSpecs");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Build.Run();
