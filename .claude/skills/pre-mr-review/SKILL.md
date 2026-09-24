---
name: pre-mr-review
description: Run pre-MR review checks including translations, code style, XML docs, API compatibility, and changelog validation. Use before raising a merge request or when reviewing branch changes.
---

# Pre-MR Review

Run this after implementing changes on a branch to catch common issues before raising an MR.

---

## 1. Missing translations

For every `.resx` file changed on the branch:

1. Find all changed resx files: `git diff main...HEAD --name-only | grep '\.resx$'`
2. For each changed file, identify its sibling language files (e.g. `Foo.resx` → `Foo.de.resx`)
3. Extract all `name=` keys from the default (English) file and each language file
4. Report any key present in the default file but missing from a language file, or vice versa

---

## 2. Naming and code style consistency

For every `.cs` and `.razor` file changed on the branch, check:

**Namespace placement**
- Types implementing `IEvent` must live in an `Events` namespace and under an `Events/` folder
- Types implementing `ICommand` or `IInstanceDependentCommand` must live in a `Commands` namespace and under a `Commands/` folder
- Types implementing `IRequest` or `IInstanceDependentRequest` must live in a `Requests` namespace and under a `Requests/` folder
- Multiple unrelated types must not be bundled in the same file

**Member ordering** (within each type, groups separated by a blank line)
- Constants → Static fields → Fields → Properties → Events → Constructors → Methods

**Naming conventions**
- Public members: `PascalCase`
- Private fields: `_camelCase`
- Test methods: `snake_case`

**Attribute formatting**
- Each attribute on its own line — never combined as `[A, B]` or stacked as `[A][B]` on a single line

**Test structure**
- All tests must have `// Arrange`, `// Act`, `// Assert` comments

---

## 3. XML documentation

For every public API added or modified on the branch:
- All public types, methods, properties, and parameters must have XML doc comments
- `<summary>` must be present and meaningful (not just restating the name)

---

## 4. API compatibility

For any changed public API surface:
- Removals or signature changes require a major version bump
- New members on interfaces implemented by module authors are breaking unless they have a default implementation
- Deprecated APIs must use `[Obsolete("Use XYZ instead.")]` with a pointer to the replacement

---

## 5. Changelog

Check that every change a module author can notice has an entry in `CHANGELOG.md`, and that each entry follows the
*Changelog* rules in `AGENTS.md`: under the unreleased header, one line from the module author's view, no implementation
details. Report entries that explain a cause or a mechanism.

---

## Output

Report **only actual issues found.** For each issue state: file path, line number, and a one-line description of the violation. If nothing is found, say so.
