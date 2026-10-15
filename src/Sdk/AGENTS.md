# Sdk

Core contracts package — the public API surface consumed by all modules.

- **Interfaces and contracts only** — no implementations, no logic
- Bottom of the dependency graph — Sdk.Backend and Sdk.Client depend on this, never the reverse
- Contains messaging interfaces (`ICommand`, `IEvent`, `IRequest`, `IInstanceDependent*`), authorization, connections, module metadata
- Minimal dependencies: MassTransit.Abstractions, AspNetCore.Authorization
- **Strict semver** — any public API change here impacts all module consumers
- **Types in a `JsonSerializerContext`** — the source generator passes `null` or `default` for an `init` member missing in the JSON (`set` members keep their initializer): give a defaulted non-nullable reference member `init => field = value ?? <default>`, and make any other member whose initializer is not its type's default `required` or `[JsonRequired]`
