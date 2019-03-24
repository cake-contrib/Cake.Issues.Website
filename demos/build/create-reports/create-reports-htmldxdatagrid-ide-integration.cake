Task("Create-Reports-HtmlDxDataGrid-IDE-Integration")
    .Description("Creates HtmlDxDataGrid demo report showing how to have IDE integration for issues")
    .IsDependentOn("Read-Issues")
    .Does<BuildData>(data =>
{
    CreateIssueReport(
        data.Issues,
        GenericIssueReportFormatFromEmbeddedTemplate(
            GenericIssueReportTemplate.HtmlDxDataGrid,
            settings => settings
                .WithOption(
                    HtmlDxDataGridOption.IdeIntegrationSettings,
                    GenericIssueReportIdeIntegrationSettingsForVisualStudioUsingTeamCityAddin())),
        data.RepoRootFolder,
        data.TemplateGalleryFolder.CombineWithFilePath("htmldxdatagrid-demo-ideintegration.html"));
});