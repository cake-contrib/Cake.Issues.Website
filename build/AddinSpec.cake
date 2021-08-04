public class AddinSpec
{
    public string Name { get; set; }
    public string NuGet { get; set; }
    public bool Prerelease { get; set; }
    public List<string> Assemblies { get; set; }
    public string RepositoryOwner { get; set; }
    public string RepositoryName { get; set; }
    public string RepositoryDocumentationPath { get; set; }
    public string DocumentationLink { get; set; }
    public string ReleaseNotesFilePath { get; set; }
    public List<string> ReleaseNotesHeader { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public List<string> Categories { get; set; }
}