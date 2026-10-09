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

5. **Behavioural changes (signature unchanged, answers change) — see ADR-002:**
   - A public method body that returns a different answer for an input that reaches it today
   - Apply the direction test from `docs/ADRs/ADR-002-contract-governance-for-behavioural-change.md`:
     - **Toward the documented contract** (the implementation was wrong, the XML documentation was right) → defect fix, minor or patch
     - **Away from it, or the documented contract itself changes** → breaking, major
   - Cite the XML documentation *as it stood before the change*. Editing documentation that states the old behaviour makes the change BREAKING, unless one of the two exceptions in the ADR's *Risks* › *The category is abusable* applies and the MR description names it
   - Requires a `### Fixed` changelog entry naming the API, the observable difference, and the call pattern most likely to notice - one line plus a `**Behaviour change:**` sentence, per the *Changelog* rules in `AGENTS.md`

Compare against the current public API surface in the `src/` projects, and read the body of every changed public method - a behavioural change is invisible to a signature-level diff. Flag any change that would require a major or minor version bump.

Output format:
- 🔴 BREAKING (requires major bump): [description]
- 🟠 BEHAVIOURAL (direction test, then minor/patch or major): [description]
- 🟡 ADDITIVE (requires minor bump): [description]
- 🟢 PATCH (no version change needed): [description]
