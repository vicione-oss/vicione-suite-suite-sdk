# AGENTS.md — ViciOne Suite SDK

This repository uses [CLAUDE.md](CLAUDE.md) as the primary source of agent instructions, along with the [.claude/](.claude/) directory for commands and settings.

## For Copilot users

The instructions in `CLAUDE.md` apply regardless of which agent you use. Key points:

- **Purpose:** Public SDK packages consumed by all ViciOne Suite modules
- **Build:** `dotnet build vicione-suite-sdk.slnx`
- **Tests:** `dotnet test vicione-suite-sdk.slnx` — xUnit, NSubstitute, AwesomeAssertions, bUnit, MassTransit.TestFramework
- **Test style:** `snake_case` method names, explicit `// Arrange`, `// Act`, `// Assert` comments
- **Semver:** Strict semantic versioning — breaking changes require major bump; deprecate via `[Obsolete]` first
- **XML docs:** Required on all public APIs (`GenerateDocumentationFile=true`)
- **Messaging:** `ICommand`, `IInstanceDependentCommand`, `IEvent`, `IRequest` in `Sdk.Messaging`; `ISuiteMediator` (backend), `IUiMediator` (client)
- **Persistence:** `[ModuleDbContext]` attribute for source generation; dual provider (SQLite + PostgreSQL) required
- **Design defaults:** Edge-S first, always cluster-aware, offline-first, module isolation

Per-directory `CLAUDE.md` files exist in key projects with project-specific constraints:
- `src/Sdk/CLAUDE.md` — contracts only, bottom of dependency graph
- `src/Sdk.Backend/CLAUDE.md` — BackendModule, persistence, consumers
- `src/Sdk.Client/CLAUDE.md` — ClientModule, IUiMediator, UI extensions
- `src/Sdk.Testing/CLAUDE.md` — helper library, not a test project
- `tests/CLAUDE.md` — test conventions, TestModule as reference impl

For full details, read [CLAUDE.md](CLAUDE.md).
