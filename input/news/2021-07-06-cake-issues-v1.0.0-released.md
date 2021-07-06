---
title: Cake Issues v1.0.0 Released
category: Release Notes
---

More than 4 years after the [first commit for Cake.Prca](https://github.com/cake-contrib/Cake.Prca/commit/438b3a1a609e5b9cc9e6f8f489a73988f9ed1f4d),
the predecessor of Cake Issues, we're happy to announce that Cake Issues version 1.0.0 has been released.
This is a major release bringing a lot of new features across all addins.

<!--excerpt-->

This post shows the highlights included in this release.
For update instructions skip to [Updating from previous versions](#updating-from-previous-versions).

❤ Huge thanks to our community! This release would not have been possible without your support and contributions! ❤

People working on this release:

* [eoehen](https://github.com/eoehen)
* [janniksam](https://github.com/janniksam)
* [pascalberger](https://github.com/pascalberger)
* [phlorian](https://github.com/phlorian)
* [jokay](https://github.com/jokay)

## Support for Cake 1.0

All addins have been updated to support Cake 1.x.

## Support for arbitrary values in an issue

Specific issue providers might have additional information for which no equivalent does exist on `IIssue`.
These kind of information can now be stored in the `IIssue.AdditionalInformation` property.

## New provider type property

While there are aliases to get provider type name (e.g. [MsBuildIssuesProviderTypeName]), this information was in previous versions
not available through the `IIssueProvider` interface.
There is a new `IIssueProvider.ProviderType` property which can be used to retrieve the provider type.

[MsBuildIssuesProviderTypeName]: /api/Cake.Issues.MsBuild/MsBuildIssuesAliases/0A221402

## PDF export

A new PDF export has been added to the `HtmlDxDataGrid` template of the `Cake.Issues.Reporting.Generic` addin.

## New recipe for Cake Frosting

TODO

## Simplified release process

Starting with Cake Issues 1.0.0 the three core addins `Cake.Issues`, `Cake.Issues.PullRequests` and
`Cake.Issues.Reporting` will be always released together.
For that source code for the addin has been merged in the [Cake.Issues repository].
Please open any issues related to any of the core addins in the [Cake.Issues issue tracker].

[Cake.Issues repository]: https://github.com/cake-contrib/Cake.Issues
[Cake.Issues issue tracker]: https://github.com/cake-contrib/Cake.Issues/issues

## Get everything together

[Cake.Issues.Recipe], the Cake Recipe script which you can integrate into your build script for easy integration of
full feature issue management, has been updated to version 1.0.0, bringing all the new features of Cake.Issues 1.0.0.

[Cake.Issues.Recipe]: ../docs/recipe/overview

## Updating from previous versions

Cake.Issues 1.0.0 is a breaking release, which means that it probably requires changes to your build script.
This section documents the most common changes which might be required:

* Cake.Issues
  * Serialization format has been updated to version 4.
    Older version are still supported for deserialization.
  * `IIssueProvider` was extended with an additional `ProviderType` property.
    For issue providers inheriting from `BaseIssueProvider` no action is required to keep the same behavior
    as with previous versions, with the type name used as provider type.

For details see release notes of the individual addins:

* [Cake.Issues 1.0.0](https://github.com/cake-contrib/Cake.Issues/releases/tag/1.0.0)
* [Cake.Issues.MsBuild 1.0.0](https://github.com/cake-contrib/Cake.Issues.MsBuild/releases/tag/1.0.0)
* [Cake.Issues.InspectCode 1.0.0](https://github.com/cake-contrib/Cake.Issues.InspectCode/releases/tag/1.0.0)
* [Cake.Issues.DupFinder 1.0.0](https://github.com/cake-contrib/Cake.Issues.DupFinder/releases/tag/1.0.0)
* [Cake.Issues.GitRepository 1.0.0](https://github.com/cake-contrib/Cake.Issues.GitRepository/releases/tag/1.0.0)
* [Cake.Issues.Markdownlint 1.0.0](https://github.com/cake-contrib/Cake.Issues.Markdownlint/releases/tag/1.0.0)
* [Cake.Issues.EsLint 1.0.0](https://github.com/cake-contrib/Cake.Issues.EsLint/releases/tag/1.0.0)
* [Cake.Issues.DocFx 1.0.0](https://github.com/cake-contrib/Cake.Issues.DocFx/releases/tag/1.0.0)
* [Cake.Issues.PullRequests 1.0.0](https://github.com/cake-contrib/Cake.Issues.PullRequests/releases/tag/1.0.0)
* [Cake.Issues.PullRequests.AzureDevOps 1.0.0](https://github.com/cake-contrib/Cake.Issues.PullRequests.AzureDevOps/releases/tag/1.0.0)
* [Cake.Issues.PullRequests.AppVeyor 1.0.0](https://github.com/cake-contrib/Cake.Issues.PullRequests.AppVeyor/releases/tag/1.0.0)
* [Cake.Issues.Reporting 1.0.0](https://github.com/cake-contrib/Cake.Issues.Reporting/releases/tag/1.0.0)
* [Cake.Issues.Reporting.Generic 1.0.0](https://github.com/cake-contrib/Cake.Issues.Reporting.Generic/releases/tag/1.0.0)
* [Cake.Issues.Reporting.Sarif 1.0.0](https://github.com/cake-contrib/Cake.Issues.Reporting.Sarif/releases/tag/1.0.0)
* [Cake.Issues.Recipe 1.0.0](https://github.com/cake-contrib/Cake.Issues.Recipe/releases/tag/1.0.0)
