#load create-reports-htmldiagnostic.cake
#load create-reports-htmldatatable.cake
#load create-reports-htmldxdatagrid.cake
#load create-reports-htmldxpivotgrid.cake

Task("Create-Reports")
    .Description("Creates all demo reports")
    .IsDependentOn("Create-Reports-HtmlDiagnostic")
    .IsDependentOn("Create-Reports-HtmlDataTable")
    .IsDependentOn("Create-Reports-HtmlDxDataGrid")
    .IsDependentOn("Create-Reports-HtmlDxPivotGrid");