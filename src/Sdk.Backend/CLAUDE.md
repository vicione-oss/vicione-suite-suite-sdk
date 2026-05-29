# Sdk.Backend

Backend module abstractions — base class, persistence, MassTransit consumers, ISuiteMediator.

- `BackendModule` base class with lifecycle hooks (ConfigureServices → ConfigureMessageBus → UseServices → MapEndpoints)
- `[ModuleDbContext]` source generator produces SQLite/Postgres subclasses and design-time factories
- `ISuiteMediator` is the primary messaging abstraction for backend modules
- Consumers auto-discovered via assembly scanning — no registration needed
- Instance-dependent state changes (outside `IModuleDbContext`) MUST use `IInstanceDependentCommand` consumers
- Depends on Sdk (contracts) only — never reference Core.OS or Blazor projects
