[[_TOC_]]

## Introduction

This document describes how backend modules can ship resource files (e.g. default configurations, templates, certificates) and control how those files are deployed to the module's workspace directory at startup.

It uses the term `FooBackendModule` for an exemplary backend module.

## Implementation

### Basic setup

To ship resource files with a module:

1. Place files in a directory relative to the module assembly (typically `AppData/`)
2. Ensure the files are copied to the build output (set `CopyToOutputDirectory` in the `.csproj`)
3. Override `GetResourceOptions` in your module class

```csharp
public sealed class FooBackendModule : BackendModule
{
    public override ModuleResourceOptions GetResourceOptions(IServiceProvider services) =>
        new() { Directory = "AppData" };
}
```

With this setup, all files under `AppData/` are deployed using the default behavior `CopyIfNotExists` — they are copied to the module's workspace the first time and never overwritten afterwards.

### Project file configuration

Ensure the resource files are included in the build output:

```xml
<ItemGroup>
    <None Update="AppData\**\*">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
</ItemGroup>
```

### Directory structure

Resource files are deployed to the module's workspace directory, maintaining the relative directory structure from the source. For example:

```
Assembly output directory         Module workspace directory
└── AppData/                      └── {ModuleId}/
    ├── Dashboard/                    ├── Dashboard/
    │   ├── overview.xml              │   ├── overview.xml
    │   └── details.xml               │   └── details.xml
    └── config.json                   └── config.json
```

## Copy behaviors

| Behavior | Description |
|---|---|
| `CopyIfNotExists` | Copy only if the target file does not exist. This is the default. Use this for files that users may customize after initial deployment. |
| `CopyAlways` | Always copy, overwriting any existing file. Use this for files that must always match the version shipped with the module. |
| `CopyIfNewer` | Copy only if the source file's last write time is newer than the target. Use this for files that should be updated with new module versions but not overwritten if unchanged. |
| `NeverCopy` | Never copy this file. Use this to exclude files from deployment that exist in the resource directory for development purposes only. |

## Rules

Rules allow overriding the `DefaultBehavior` for files matching a glob pattern. Rules are evaluated in order and the first matching rule wins. Files that don't match any rule use `DefaultBehavior`.

### Glob pattern syntax

Patterns are matched against the relative path of each resource file within the resource directory using [`Microsoft.Extensions.FileSystemGlobbing`](https://learn.microsoft.com/en-us/dotnet/core/extensions/file-globbing).

| Pattern | Matches |
|---|---|
| `*.xml` | All `.xml` files in the root of the resource directory |
| `**/*.xml` | All `.xml` files in any subdirectory |
| `Dashboard/**` | All files under `Dashboard/` |
| `config.json` | A specific file |
| `certs/*.pem` | All `.pem` files directly inside `certs/` |

### Example: mixed behaviors

```csharp
public override ModuleResourceOptions GetResourceOptions(IServiceProvider services) =>
    new()
    {
        Directory = "AppData",
        DefaultBehavior = ResourceCopyBehavior.CopyIfNotExists,
        Rules =
        [
            new ResourceRule("**/*.xml", ResourceCopyBehavior.CopyAlways),
            new ResourceRule("dev/**", ResourceCopyBehavior.NeverCopy),
        ],
    };
```

In this configuration:
- XML files are always overwritten with the latest version
- Files under `dev/` are never deployed
- All other files are copied only if they don't already exist

### Example: conditional resources

Resource options can depend on runtime configuration:

```csharp
public override ModuleResourceOptions? GetResourceOptions(IServiceProvider services)
{
    var options = services.GetRequiredService<IOptions<FooOptions>>().Value;
    return options.EnableDemoResources ? new ModuleResourceOptions { Directory = "AppData" } : null;
}
```

## Accessing deployed resources

Use `IWorkspaceProvider<T>` to access the module's workspace directory where resources have been deployed:

```csharp
public class FooService(IWorkspaceProvider<FooBackendModule> workspace)
{
    public string GetConfigPath() => Path.Combine(workspace.Home, "config.json");
}
```
