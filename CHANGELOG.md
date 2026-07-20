# Changelog

All notable changes to this project are documented in this file.

## [1.3.0] - 2026-07-20

### Added

- Multi-organization Azure DevOps configuration with organization-specific PATs, default projects, and an optional `organization` argument across MCP tools. The existing single-organization configuration remains supported.
- Wiki tools for listing wikis and reading wiki pages.
- Work item attachment upload, download, and metadata tools.
- Work item discussion retrieval and pull request thread comments.
- Pull request thread creation, status updates, and pull request updates.
- Work item relation linking.
- `ActivatedDate` and `ClosedDate` fields in work item responses.

### Fixed

- Work item parent IDs are now derived from the Azure DevOps `Hierarchy-Reverse` relation.

### Removed

- Flow analytics, WIP analysis, bottleneck, workload, and aging report tools introduced in v1.2.0.

### Upgrade notes

- Existing single-organization installations do not require configuration changes.
- To use more than one organization, configure `AzureDevOps:DefaultOrganization` and `AzureDevOps:Organizations`, or the equivalent environment variables documented in `.env.example`.
- Clients that used the removed analytics tools must migrate those workflows before upgrading.

[1.3.0]: https://github.com/viamus/mcp-azure-devops/compare/v1.2.0...v1.3.0
