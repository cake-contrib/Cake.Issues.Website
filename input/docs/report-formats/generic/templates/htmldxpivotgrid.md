---
Order: 20
Title: HTML DevExtreme Pivot Grid
Description: Template for a HTML report containing a rich pivot grid with sorting, filtering, grouping and search capabilities.
---
Template for a HTML report containing a rich data pivot with sorting, filtering, grouping and search capabilities powered by [DevExtreme].

![HTML DevExtreme Pivot Grid](htmldxpivotgrid01.png "HTML DevExtreme Pivot Grid")

# Features

* Table with `Provider`, `Severity`, `Project`, `Path`, `File`, `Line`, `Rule`, `Message` by default.
* Support for grouping by multiple columns by user.
* Total number of issues by each group level.
* Each column sortable by user.
* Data can be filtered by any column by user.
* Paged view.
* Client-side full text search.
* Fully customizable through [options](#options).

# Requirements

* Cake.Issues.Reporting.Generic 0.3.1 or higher
* Internet access

# Usage

To create a report using the HTML DevExtreme Pivot Grid template you can use the [GenericIssueReportTemplate.HtmlDxPivotGrid] enum value:

```csharp
CreateIssueReport(
    issues,
    GenericIssueReportFormatFromEmbeddedTemplate(GenericIssueReportTemplate.HtmlDxPivotGrid),
    @"c:\repo",
    @"c:\report.html");
```

# Options

See [HtmlDxPivotGridOption] for a list of possible options.

# Demos

The following demo shows the template with its default options:

* <a href="htmldxpivotgrid-demo-default.html" target="_blank">Default</a>
  (<a href="https://github.com/cake-contrib/Cake.Issues.Website/blob/develop/demos/build/create-reports/create-reports-htmldxpivotgrid-default.cake" target="_blank">Source Code</a>)

# Source Code

:::{.alert .alert-info}
You can use the source code as a template for your [custom template].
:::

Source code is available on [GitHub].

[DevExtreme]: https://js.devexpress.com
[GenericIssueReportTemplate.HtmlDxPivotGrid]: ../../../../../Cake.Issues.Website/api/Cake.Issues.Reporting.Generic/GenericIssueReportTemplate/0E9E9D94
[HtmlDxPivotGridOption]: ../../../../../Cake.Issues.Website/api/Cake.Issues.Reporting.Generic/HtmlDxPivotGridOption/
[custom template]: ../examples/custom-template
[GitHub]: https://github.com/cake-contrib/Cake.Issues.Reporting.Generic/blob/develop/src/Cake.Issues.Reporting.Generic/Templates/DxPivotGrid.cshtml
