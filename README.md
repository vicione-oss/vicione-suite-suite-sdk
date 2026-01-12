# ViciOne Suite SDK

Welcome to the readme of ViciOne Suite. Questions and suggestions for improvement are warmly appreciated!

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