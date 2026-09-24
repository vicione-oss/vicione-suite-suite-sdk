# ViciOne Suite SDK

Welcome to the readme of ViciOne Suite. Questions and suggestions for improvement are warmly appreciated!

## Contents
* [Configure local development experience](#configure-local-development-experience)
* [Database migration](#database-migration)
    * [`Package Manager Console` of Visual Studio](#package-manager-console-of-visual-studio)
    * [Entity Framework CLI](#entity-framework-cli)
* [Code comments](./docs/code-style-guide.md)

## Configure local development experience

Some aspects of the build process differs when running a build in a local developer environment compared to when it would run in CI pipeline.

We provide support for environment variables to customize the local development experience.

Environment Variable | Description | Default Value | Value in CI | Example usage in PowerShell
-|-|-|-|-
`TREAT_WARNINGS_AS_ERRORS` | When set to `true`, all compiler warnings are treated as errors. | `false` | `true` | `[Environment]::SetEnvironmentVariable("TREAT_WARNINGS_AS_ERRORS", "true", "User")`

## Database migration

The following examples creates migrations for a newly introduced entity `FooBar` in `ModuleDbContext`:

### `Package Manager Console` of Visual Studio.

```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools

Add-Migration Add_FooBar -OutputDir Migrations\ModuleDbContext\Sqlite -Context ModuleDbContextSqlite -StartupProject Module.Backend

Add-Migration Add_FooBar -OutputDir Migrations\ModuleDbContext\Postgres -Context ModuleDbContextPostgres -StartupProject Module.Backend
```

### Entity Framework CLI

```bash
dotnet ef install -g dotnet-ef

dotnet ef migrations add Add_FooBar --output-dir Migrations\ModuleDbContext\Sqlite --context ModuleDbContextSqlite --startup-project .\src\Module.Backend\Module.Backend.csproj

dotnet ef migrations add Add_FooBar --output-dir Migrations\ModuleDbContext\Postgres --context ModuleDbContextPostgres --startup-project .\src\Module.Backend\Module.Backend.csproj
```

## Code comments

Comment rules for hand-written `*.cs` and `*.razor` files live in [Code Style Guide → Code comments](./docs/code-style-guide.md#code-comments).
