#load create-reports-htmldxpivotgrid-default.cake

Task("Create-Reports-HtmlDxPivotGrid")
    .Description("Creates HtmlDxPivotGrid demo reports")
    .IsDependentOn("Create-Reports-HtmlDxPivotGrid-Default");