# CLAUDE.md — ViciOne Suite SDK

## Project Overview

The ViciOne Suite SDK provides the public API surface and extension points for building modules that run inside ViciOne Suite. Modules consume these packages to integrate backend services, UI components, messaging, and persistence into the platform.

- **Version:** See `VERSION` file
- **Solution:** `vicione-suite-sdk.slnx`
- **Company:** ifm software
- **License:** See LICENSE.txt
- **Published as:** NuGet packages consumed by module repositories

## SDK Packages

| Package              | Purpose                                                     |
|----------------------|-------------------------------------------------------------|
| `Sdk`               | Core contracts: messaging interfaces, authorization, connections, modules, instance info |
| `Sdk.Backend`       | Backend module base class, persistence (EF Core), MassTransit consumers, ISuiteMediator |
| `Sdk.Client`        | Client module base class, Blazor components, navigation tiles, control panels, wizards |
| `Sdk.Testing`       | Test helpers, bUnit utilities, MassTransit test framework integration |
| `Sdk.Deployment`    | MSBuild targets and scripts for publishing/cleaning modules |
| `Sdk.Backend.SourceGenerators` | Roslyn source generators for backend modules     |

## Build & Run

```shell
# Build
dotnet build vicione-suite-sdk.slnx

# Tests
dotnet test vicione-suite-sdk.slnx

# Lint TypeScript (Sdk.Client interop)
npm run lint
```

**Target framework:** net10.0
**Test runner:** Microsoft.Testing.Platform (see global.json)
**Test frameworks:** xUnit, NSubstitute, AwesomeAssertions, bUnit, MassTransit.TestFramework

## Code Style & Conventions

- See `docs/code-style-guide.md` for member ordering rules
- Test method names: `snake_case` (displayed as sentences in Test Explorer)
- All tests are structured by arrange/act/assert with explicit `// Arrange`, `// Act`, `// Assert` comments
- XML documentation is **required** for all public APIs (`GenerateDocumentationFile=true`, `EnforceCodeStyleInBuild=true`)
- Warnings are errors in CI (`TREAT_WARNINGS_AS_ERRORS=true`)
- Languages: en-US primary, de secondary

## Messaging Architecture (Sdk.Messaging)

The messaging system is the backbone of inter-module and inter-instance communication. All message interfaces live in `Sdk.Messaging`.

### Message Interfaces

| Interface                      | Scope                | Consumed by       | Description                                    |
|--------------------------------|----------------------|-------------------|------------------------------------------------|
| `ICommand`                     | Global → Master only | Master            | State change sent to the central broker        |
| `IInstanceDependentCommand`    | Targeted → one node  | Named instance    | State change on a specific instance            |
| `IEvent`                       | Broadcast → all      | All subscribers   | Something happened (fan-out)                   |
| `IInstanceEvent`               | Local → current node | Current instance  | Something happened on this instance only       |
| `IInstanceDependentRequest<T>` | Targeted → one node  | Named instance    | Request/response from a specific instance      |
| `IRequest<T>`                  | Local                | Local instance    | Request/response that never leaves the process |

### Key Contracts

- `IRoutableMessage` — Base marker for all routable messages
- `IInstanceDependentMessage` — Carries a target instance ID
- `IResponse` — Base for all response types
- `CorrelatedBy<Guid>` — All commands carry a correlation ID for tracing

### ISuiteMediator (Backend)

The primary abstraction for sending/publishing messages from backend module code:

```csharp
// Global command (processed by master)
await mediator.Send(new CreateUser { ... });

// Instance-targeted command
await mediator.Send(new RestartService { ... }, targetInstanceId);

// Broadcast event
await mediator.Publish(new UserCreatedEvent { ... });

// Local request/response
var result = await mediator.Request<GetUsers, GetUsersResponse>(new GetUsers());

// Instance-targeted request
var result = await mediator.Request<GetStatus, StatusResponse>(request, targetInstanceId);
```

### IUiMediator (Client)

`Sdk.Client.Infrastructure.IUiMediator` provides similar messaging capabilities for client/UI modules. Use this in Blazor components instead of `ISuiteMediator`:

```csharp
// Send command from UI
await uiMediator.Send(new CreateUser { ... });

// Request/response from UI
var result = await uiMediator.Request<GetUsers, GetUsersResponse>(new GetUsers());

// Register for events in UI components
using var subscription = uiMediator.Register<UserCreatedEvent>(handler);
```

`IUiMediator` also exposes `CommandTimeoutMs` for UI-specific timeout configuration and supports event subscriptions via `Register<TEvent>()`.

### Consumer Discovery

Consumers are auto-discovered via **assembly scanning** (configured in `MassTransitConfiguration.cs` in Core.OS). No manual registration is needed — just implement the consumer class.

### Consumer Rules

- **Consumers MUST be idempotent.** Messages may be redelivered after connectivity loss.
- **Instance-dependent state changes** (anything not in `IModuleDbContext`, e.g. local file operations, hostmanagement calls) **MUST** use `IInstanceDependentCommand` consumers.
- Commands are fire-and-forget; results are communicated via corresponding `IEvent`.

## Module Development

### Backend Module

Extend `BackendModule` to create a backend module:

```csharp
public class MyModule : BackendModule
{
    public override IModuleInitializer? ModuleInitializer => new MyModuleInitializer();

    public override void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder) { }
    public override void ConfigureMessageBus(IServiceCollection busConfig, InstanceType instanceType) { }
    public override void UseServices(IApplicationBuilder app) { }
    public override void MapEndpoints(IEndpointRouteBuilder endpoints) { }
}
```

### Client Module

Extend `ClientModule` for UI modules:

```csharp
public class MyClientModule : ClientModule
{
    public override Action<IServiceCollection>? Configure => services => { ... };
    public override Func<IServiceProvider, Task>? InitializeServices => async sp => { ... };
    public override Func<IServiceProvider, ClaimsPrincipal, Task>? OnUserAuthenticated => async (sp, user) => { ... };
}
```

### Module ID Convention

The module ID is derived from the assembly name by stripping the suffix:

| Assembly Name                          | Module ID                |
|----------------------------------------|--------------------------|
| `ViciOne.Suite.MyModule.Backend`       | `ViciOne.Suite.MyModule` |
| `ViciOne.Suite.MyModule.Client`        | `ViciOne.Suite.MyModule` |

### Module Lifecycle (Backend)

1. `ConfigureServices` — Register DI services
2. `ConfigureMessageBus` — Configure bus (consumers are auto-discovered)
3. `IModuleInitializer.OnPreMigrate` — Pre-migration validation
4. `IModuleInitializer.Migrate` — Apply EF migrations
5. `IModuleInitializer.OnPostMigrate` — Seed data, build caches
6. `IModuleInitializer.OnInitialized` — Final startup logic
7. `UseServices` — Configure middleware
8. `MapEndpoints` — Register HTTP endpoints

## Persistence

### Dual-Provider Requirement

Modules **MUST** implement both database providers:
- `ISqliteDbContext` — Used on slaves and standalone instances
- `IPostgresDbContext` — Used on master instances

### Source Generation with `[ModuleDbContext]`

Annotate a `partial` class deriving from `ModuleDbContext` with `[ModuleDbContext]` to auto-generate:
- `{ClassName}Sqlite` — sealed subclass implementing `ISqliteDbContext`
- `{ClassName}Postgres` — sealed subclass implementing `IPostgresDbContext`
- `{ClassName}SqliteFactory` — `IDesignTimeDbContextFactory<T>` for SQLite
- `{ClassName}PostgresFactory` — `IDesignTimeDbContextFactory<T>` for PostgreSQL
- A `DefaultSchemaName` override (if `DefaultSchemaName` property is set on the attribute)

```csharp
[ModuleDbContext(DefaultSchemaName = "mymodule")]
public partial class MyModuleDbContext : ModuleDbContext
{
    protected override void OnModuleModelCreating(ModelBuilder modelBuilder) { ... }
}
```

### EF Migrations

Migrations are scaffolded per provider. Both SQLite and PostgreSQL migrations must be maintained:

```shell
# Via Entity Framework CLI
dotnet ef migrations add Add_FooBar --output-dir Migrations/ModuleDbContext/Sqlite --context MyModuleDbContextSqlite --startup-project ./src/Module.Backend
dotnet ef migrations add Add_FooBar --output-dir Migrations/ModuleDbContext/Postgres --context MyModuleDbContextPostgres --startup-project ./src/Module.Backend
```

### ModuleDbContext Behavior

Both inherit from `ModuleDbContext` which enforces:
- A default schema name (`DefaultSchemaName`)
- Sealed `OnModelCreating` → override `OnModuleModelCreating` instead
- `NotSynchronizedEntityTypes` for entities excluded from cluster replication

### Replication Constraints

- Bulk EF operations (`.ExecuteUpdate()`, `.ExecuteDelete()`) are **NOT** captured by the `ChangeTrackingInterceptor`
- Shadow properties are not replicated
- Non-JSON-serializable types are not replicated

## Authorization

- `ModuleAuthorizeAttribute` — Attribute-based access control on components
- `AccessLevel` enum: `Partial`, `Full`
- `IModuleFeature` — Define named features with access levels
- Modules can disable the default feature via `BackendModule.DisableDefaultFeature`

## UI Extension Points (Sdk.Client)

| Concept          | Purpose                              | Guide                              |
|------------------|--------------------------------------|------------------------------------|
| Navigation Tiles | Main navigation entries              | `docs/navigation-tile-guide.md`    |
| Control Panels   | Settings panels in system config     | `docs/control-panel-guide.md`      |
| Wizards          | Multi-step workflows                 | `docs/wizard-guide.md`             |
| Notifications    | Notification area elements           | `docs/notification-element-guide.md`|
| Module Resources | Static files deployed with module    | `docs/module-resources-guide.md`   |
| Localization     | i18n via .resx and Sdk extensions    | `docs/localization-guide.md`       |

## Semantic Versioning

**This SDK follows strict semantic versioning.** Any breaking change to a public API requires a major version bump. Module authors depend on SDK stability — changes to contracts, interfaces, or behavior must be backward-compatible within a major version.

**Deprecation policy:** Use `[Obsolete("Use XYZ instead.")]` to mark APIs for removal. Deprecated APIs stay for at least one minor release before being removed in the next major version. The obsolete message must always point to the replacement.

## Design Priorities

1. **Edge-S first** — 1 GB RAM, flash storage, limited write cycles
2. **Always cluster-aware** — Features must work across master/slave topology
3. **Offline-first** — Nodes must operate during connectivity loss
4. **Module isolation** — Clean boundaries; modules interact via messaging and public contracts

## Current Focus

- Stabilizing master-slave architecture capabilities
- DevExpress UI components being replaced by custom `blazor-components`
- Defining integration and UI testing strategy

## Project Structure

Each key directory has its own `CLAUDE.md` with project-specific rules and constraints.

## Related Repositories

`suite` · `blazor-components` · `ping` · `addon-project-templates` · `cluster-mgmt` · `data-collection-wizard` · `hostmgmt` · `dx`
