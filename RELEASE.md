# Releasing ZeepSDK

This guide describes how to publish stable and beta releases of ZeepSDK.

## Overview

| Release type | GitHub pre-release | NuGet | mod.io | GitHub release asset | Docs |
|--------------|-------------------|-------|--------|---------------------|------|
| **Stable** | unchecked | stable version | uploaded | — | site root |
| **Beta** | checked | prerelease version | skipped | zip attached | `/beta/` subpath |

Beta releases are published to NuGet with a semver prerelease label (for example `2.4.0-beta.1`). They do **not** become the default package version when someone runs `dotnet add package ZeepSDK` without specifying a version.

## Beta release

1. Merge the changes you want to test into `main`.
2. Create a GitHub release:
   - Tag: `v2.4.0-beta.1` (or the next beta number)
   - Check **Set as a pre-release**
   - Write a changelog describing what testers should verify
3. Publishing happens automatically when the release is published:
   - **NuGet**: `ZeepSDK` version `2.4.0-beta.1` is pushed to [nuget.org](https://www.nuget.org/packages/ZeepSDK)
   - **Docs**: deployed to [beta documentation](https://donderjoekel.github.io/ZeepSDK/beta/)
   - **GitHub release asset**: the build zip is attached to the release for manual testing
   - **mod.io**: not updated

### Installing a beta NuGet package

```bash
dotnet add package ZeepSDK --version 2.4.0-beta.1
```

### Manual beta workflows

You can also trigger workflows manually from the Actions tab:

- **Publish NuGet Package**: provide a version such as `2.4.0-beta.1`
- **Publish DocFX**: enable **prerelease** and optionally provide the version for the beta banner
- **Release**: use **dry run** to build a zip without uploading to mod.io

## Stable release

1. Create a GitHub release:
   - Tag: `v2.4.0`
   - Leave **Set as a pre-release** unchecked
   - Write release notes
2. Publishing happens automatically:
   - **NuGet**: stable package pushed to nuget.org
   - **Docs**: deployed to the [stable documentation site](https://donderjoekel.github.io/ZeepSDK/)
   - **mod.io**: build uploaded and set active for automatic updates

## Version tags

Use semantic versioning:

- Stable: `v2.4.0`
- Beta: `v2.4.0-beta.1`, `v2.4.0-beta.2`, and so on

The leading `v` in the GitHub tag is optional in workflow inputs but recommended for release tags. Workflows strip it before setting the project version.

## Workflows

| Workflow | File | Trigger |
|----------|------|---------|
| Release | `.github/workflows/publish-release.yml` | GitHub release, manual |
| Publish NuGet Package | `.github/workflows/publish-nuget.yml` | GitHub release, manual |
| Publish DocFX | `.github/workflows/publish-docs.yml` | GitHub release, manual |

All three workflows read the version from the release tag or from the manual `version` input.

## Promoting beta to stable

There is no automatic promotion. When the beta is ready:

1. Create a new **stable** GitHub release with the final version tag (for example `v2.4.0`).
2. Leave the old pre-release on GitHub for history, or delete it if you no longer want it visible.

## Verification checklist

After a beta release:

- [ ] NuGet lists `2.4.0-beta.1` as a prerelease
- [ ] `dotnet add package ZeepSDK` still resolves the latest stable version
- [ ] mod.io was not updated
- [ ] Beta docs are live under `/beta/`
- [ ] Stable docs at the site root are unchanged
- [ ] The zip is attached to the GitHub pre-release

After a stable release:

- [ ] NuGet lists the new stable version
- [ ] mod.io received the new build
- [ ] Stable docs were updated at the site root
