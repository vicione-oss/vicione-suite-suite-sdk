# Review SDK changes

Review the current changes for SDK quality standards:

1. **Public API quality:**
   - All public members have XML documentation
   - Naming follows .NET conventions
   - No unnecessary exposure of internal types

2. **Semantic versioning:**
   - Are there breaking changes? (removed/changed public APIs)
   - Are there additions? (new public APIs)
   - Is the version bump appropriate?

3. **Module author impact:**
   - Will existing modules need changes?
   - Are new features discoverable?
   - Is the API intuitive for module developers?

4. **Cross-cutting concerns:**
   - Works on constrained hardware (Edge-S: 1GB RAM, flash)
   - Cluster-aware (master/slave topology)
   - Offline-capable
   - Consumers are idempotent

5. **Changelog:**
   - `CHANGELOG.md` is updated following [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) format
   - Changes are categorized as Added, Changed, Deprecated, Removed, Fixed, Security
   - Entry describes the change from a module author's perspective

6. **Test coverage:**
   - New public APIs have corresponding tests
   - Sdk.Testing helpers updated if needed
   - Test naming follows snake_case convention
   - Tests have explicit `// Arrange`, `// Act`, `// Assert` comments

Report **only actual issues found.** For each issue state: file path, line number (if applicable), severity, and a one-line description of the violation. Do not report positive findings or praise. If nothing is found, say so.
