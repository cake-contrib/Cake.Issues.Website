using LibGit2Sharp;
using System.Globalization;

/// <summary>
/// Class for determining information about a repository.
/// </summary>
public static class RepositoryHelper
{
    /// <summary>
    /// Copies part of a Git repository to a specific directory.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="repositoryRemoteUrl">The remote URL to the repository.</param>
    /// <param name="branchName">The branch which should be checked out.</param>
    /// <param name="sourcePathRelative">Path which should be copied relative to the repository root.</param>
    /// <param name="targetPath">Directory to which the files should be copied to.</param>
    public static void GitCopyFromRepository(
        ICakeContext context,
        Uri repositoryRemoteUrl,
        string branchName,
        DirectoryPath sourcePathRelative,
        DirectoryPath targetPath)
    {
        GitCopyFromRepository(
            context,
            repositoryRemoteUrl,
            branchName,
            new DirectoryPathCollection(new List<DirectoryPath> {sourcePathRelative}, PathComparer.Default),
            targetPath);
    }

    /// <summary>
    /// Copies part of a Git repository to a specific directory.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="repositoryRemoteUrl">The remote URL to the repository.</param>
    /// <param name="branchName">The branch which should be checked out.</param>
    /// <param name="sourcePathsRelative">Paths which should be copied relative to the repository root.</param>
    /// <param name="targetPath">Directory to which the files should be copied to.</param>
    public static void GitCopyFromRepository(
        ICakeContext context,
        Uri repositoryRemoteUrl,
        string branchName,
        IEnumerable<DirectoryPath> sourcePathsRelative,
        DirectoryPath targetPath)
    {
        GitCopyFromRepository(
            context,
            repositoryRemoteUrl,
            branchName,
            new DirectoryPathCollection(sourcePathsRelative, PathComparer.Default),
            targetPath);
    }

    /// <summary>
    /// Copies part of a Git repository to a specific directory.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="repositoryRemoteUrl">The remote URL to the repository.</param>
    /// <param name="branchName">The branch which should be checked out.</param>
    /// <param name="sourcePathsRelative">Paths which should be copied relative to the repository root.</param>
    /// <param name="targetPath">Directory to which the files should be copied to.</param>
    public static void GitCopyFromRepository(
        ICakeContext context,
        Uri repositoryRemoteUrl,
        string branchName,
        DirectoryPathCollection sourcePathsRelative,
        DirectoryPath targetPath)
    {
        context.ThrowIfNull(nameof(context));
        repositoryRemoteUrl.ThrowIfNull(nameof(repositoryRemoteUrl));
        sourcePathsRelative.ThrowIfNull(nameof(sourcePathsRelative));
        targetPath.ThrowIfNull(nameof(targetPath));

        var localPath = new DirectoryPath(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString()));
        try
        {
            // Clone repository into temporary directory.
            GitClone(context, repositoryRemoteUrl, localPath);

            // Checkout the requested branch.
            if (!string.IsNullOrEmpty(branchName))
            {
                context.GitCheckout(localPath, branchName);
            }

            // Clean target directory.
            if (context.DirectoryExists(targetPath))
            {
                context.DeleteDirectory(
                    targetPath,
                    new DeleteDirectorySettings {
                        Recursive = true,
                        Force = true
                    });
            }

            // Copy relevant part of the repository to the target directory.
            foreach (var sourcePathRelative in sourcePathsRelative)
            {
                var currentSourcePath = sourcePathRelative.MakeAbsolute(localPath);
                context.Verbose("Copying {0} to {1}", currentSourcePath, targetPath);
                context.CopyDirectory(currentSourcePath, targetPath);
            }
        }
        finally 
        {
            // Delete local repository.
            context.DeleteDirectory(
                localPath, 
                new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                });
        }
    }

    /// <summary>
    /// Clones a repository.
    /// </summary>
    /// <param name="context">Current Cake context.</param>
    /// <param name="repositoryRemoteUrl">The remote URL to the repository.</param>
    /// <param name="localPath">The local path to the repository root.</param>
    public static void GitClone(ICakeContext context, Uri repositoryRemoteUrl, DirectoryPath localPath)
    {
        context.ThrowIfNull(nameof(context));
        repositoryRemoteUrl.ThrowIfNull(nameof(repositoryRemoteUrl));
        localPath.ThrowIfNull(nameof(localPath));

        context.Verbose(
            string.Format(
                CultureInfo.InvariantCulture,
                "Cloning repository from {0} to {1}...", 
                repositoryRemoteUrl,
                localPath));

        // Use command line git because LFS might be active in the repository.
        GitCommand(
            context, 
            System.Environment.CurrentDirectory,
            "clone",
            repositoryRemoteUrl.OriginalString,
            IOHelper.QuotePath(localPath));
    }

    /// <summary>
    /// Executes a git command with arguments.
    /// Returns the standard output of the git command.
    /// </summary>
    private static IEnumerable<string> GitCommand(ICakeContext context, DirectoryPath repositoryRootFolder, params string[] arguments)
    {
        context.ThrowIfNull(nameof(context));
        repositoryRootFolder.ThrowIfNull(nameof(repositoryRootFolder));
        arguments.ThrowIfNull(nameof(arguments));

        if (!arguments.Any())
        {
            throw new ArgumentOutOfRangeException(nameof(arguments));
        }

        string gitArguments = string.Join(" ", arguments);

        IEnumerable<string> redirectedStandardOutput;
        IEnumerable<string> redirectedErrorOutput;
        var exitCode = context.StartProcess(
            "git",
            new ProcessSettings 
            {
                Arguments = gitArguments,
                WorkingDirectory = repositoryRootFolder.FullPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            out redirectedStandardOutput,
            out redirectedErrorOutput);

        if (exitCode != 0)
        {
            throw new Exception(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Git command failed with arguments {0}. Exit code: {1}, Error Output: {2}",
                    gitArguments,
                    exitCode,
                    string.Join(System.Environment.NewLine, redirectedErrorOutput))
                );
        }

        return redirectedStandardOutput;
    }
}
