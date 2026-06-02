---
name: check-api-compat
description: Analyze proposed changes for semantic versioning compliance. Detect breaking, additive, and patch-level changes. Use when checking API compatibility, version bumps, or reviewing public API changes.
---

# Check API compatibility

Analyze the proposed changes for semantic versioning compliance:

1. **Breaking changes detection:**
   - Removed or renamed public types, methods, properties
   - Changed method signatures (parameters, return types)
   - Changed interface contracts (added members to interfaces implemented by consumers)
   - Removed or changed enum values
   - Changed base classes

2. **Deprecations (use `[Obsolete]` before removal):**
   - Mark deprecated APIs with `[Obsolete("Use XYZ instead.")]` in a minor release
   - Only remove `[Obsolete]`-marked APIs in the next major version
   - Ensure the obsolete message points to the replacement API

3. **Additive changes (minor version):**
   - New public types or members
   - New optional parameters with defaults
   - New interfaces (not breaking existing implementations)

4. **Patch-level changes:**
   - Bug fixes that don't change the public API surface
   - Documentation updates
   - Internal implementation changes

Compare against the current public API surface in the `src/` projects. Flag any change that would require a major or minor version bump.

Output format:
- 🔴 BREAKING (requires major bump): [description]
- 🟡 ADDITIVE (requires minor bump): [description]
- 🟢 PATCH (no version change needed): [description]
