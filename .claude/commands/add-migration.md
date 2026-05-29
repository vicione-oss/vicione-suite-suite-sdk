# Scaffold EF migration

Create Entity Framework migrations for both SQLite and PostgreSQL providers.

## Prerequisites

- The `ModuleDbContext` subclass must be annotated with `[ModuleDbContext]`
- Source-generated `{ClassName}Sqlite` and `{ClassName}Postgres` classes must exist (build first)

## Steps

1. Ask for:
   - Migration name (e.g., `Add_FooBar`)
   - DbContext class name (e.g., `MyModuleDbContext`)
   - Startup project path (e.g., `./src/Module.Backend`)

2. Run both commands (always scaffold for **both** providers):

```shell
dotnet ef migrations add <MigrationName> --output-dir Migrations/<ContextName>/Sqlite --context <ContextName>Sqlite --startup-project <StartupProject>

dotnet ef migrations add <MigrationName> --output-dir Migrations/<ContextName>/Postgres --context <ContextName>Postgres --startup-project <StartupProject>
```

3. Verify the generated migration files look correct and contain the expected schema changes.

## Important

- Both providers MUST always be migrated together
- Migration names should describe the change (e.g., `Add_UserPreferences`, `Remove_LegacyColumn`)
- Never modify previously applied migrations — always create new ones
