# Sdk

Core contracts package — the public API surface consumed by all modules.

- **Interfaces and contracts only** — no implementations, no logic
- Bottom of the dependency graph — Sdk.Backend and Sdk.Client depend on this, never the reverse
- Contains messaging interfaces (`ICommand`, `IEvent`, `IRequest`, `IInstanceDependent*`), authorization, connections, module metadata
- Minimal dependencies: MassTransit.Abstractions, AspNetCore.Authorization
- **Strict semver** — any public API change here impacts all module consumers
