Task("Create-Reports-HtmlDxPivotGrid-Default")
    .Description("Creates HtmlDxPivotGrid default demo report")
    .IsDependentOn("Read-Issues")
    .Does<BuildData>(data =>
{
    CreateIssueReport(
        data.Issues,
        GenericIssueReportFormatFromEmbeddedTemplate(GenericIssueReportTemplate.HtmlDxPivotGrid),
        data.RepoRootFolder,
        data.TemplateGalleryFolder.CombineWithFilePath("htmldxpivotgrid-demo-default.html"));
});