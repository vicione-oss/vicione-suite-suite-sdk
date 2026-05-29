# Sdk.Testing

Reusable test infrastructure package — provides helpers, NOT test code.

- **Not a test project** (`IsTestProject=false`) — it exports test utilities as a library
- Includes: bUnit context extensions, MassTransit.TestFramework, in-memory EF Core, NSubstitute helpers
- References both Sdk.Backend and Sdk.Client so module test projects get everything in one dependency
- `ClientServiceConfigurator` provides pre-configured bUnit service containers with mocked `IUiMediator`
- Module authors should use these helpers rather than building ad-hoc test infrastructure
