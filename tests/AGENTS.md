# Tests

Test projects for the ViciOne Suite SDK packages.

## Conventions
- Test method names: `snake_case` (displayed as sentences in Test Explorer)
- All tests structured with `// Arrange`, `// Act`, `// Assert` comments
- Frameworks: xUnit, NSubstitute, AwesomeAssertions, bUnit, MassTransit.TestFramework

## Projects
- `Sdk.Tests` — tests for core contracts
- `Sdk.Backend.Tests` — tests for backend abstractions and persistence
- `Sdk.Backend.SourceGenerators.Tests` — verifies source generator output
- `Sdk.Client.Tests` — tests for client components and UI extensions
- `Sdk.Testing.Tests` — tests for the test utilities themselves
- `TestModule.Backend` / `TestModule.Client` — integration scaffolds validating real module patterns

## Rules
- XML documentation is enforced — public API tests should verify doc presence where relevant
- TestModule projects are reference implementations — keep them representative of real module usage
- Prefer MassTransit in-memory test harness over mocking consumers directly
