---
Order: 30
Title: Examples
Description: Examples for using the Cake.Issues.EsLint addin.
---
The following example will install and call [ESLint] and output the number of issues.

To install [ESLint] from a Cake script you can use the [Cake.Npm] addin.

```csharp
#addin "Cake.Npm"

Task("Install-EsLint").Does(() =>
{
    // Install ESLint globally.
    NpmInstall(
        settings =>
            settings
                .AddPackage("eslint")
                .InstallGlobally());
});
```

Afterwards you can run ESLint:

```csharp
var logPath = @"c:\build\eslint.log";

Task("Lint-JavaScript").Does(() =>
{
    var processArgumentBuilder =
        new ProcessArgumentBuilder()
            .Append("-o " + logPath)
            .Append("src/**");

    var processSettings = new ProcessSettings 
    {
        Arguments = processArgumentBuilder,
        Silent = true,
        RedirectStandardOutput = true
    };

    IEnumerable<string> redirectedOutput, redirectedErrors;

    var exitCode = StartProcess(Context.Tools.Resolve("eslint"), processSettings, out redirectedOutput, out redirectedErrors);
    if (exitCode != 0)
    {
        throw new CakeException($"ESLint exited with unexpected error code: {exitCode}");
    }
});
```

To read issues from [ESLint] log files you need to import the core addin and the ESLint support:

```csharp
#addin "Cake.Issues"
#addin "Cake.Issues.EsLint"
```

Finally you can define a task where you call the core addin with the desired issue provider.

```csharp
Task("Analyze-Log")
.IsDependentOn("Lint-JavaScript")
.Does(() =>
{
    var repoRootPath = @"c:\repo";

    // Read Issues.
    var issues = ReadIssues(
        EsLintIssuesFromFilePath(logPath, logPath),
        repoRootPath);

    Information("{0} issues are found.", issues.Count());
});
```

[ESLint]: https://eslint.org
[Cake.Npm]: https://www.nuget.org/packages/Cake.Npm/