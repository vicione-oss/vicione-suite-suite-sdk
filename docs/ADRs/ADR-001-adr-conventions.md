# ADR-001: ADR Conventions

## Status

Accepted

## Date

2026-05-27

## Context

ViciOne Suite is a complex distributed platform with multiple repositories (`suite`, `suite-sdk`, module repos) and architectural constraints (Edge-S resource limits, master-slave topology, offline-first operation). Architectural decisions are currently embedded in `CLAUDE.md` files, scattered across documentation, or exist only as tribal knowledge.

As we undertake a systematic architecture review focused on reliability and fault tolerance, we need a lightweight, structured way to:

1. Capture decisions with their rationale and trade-offs
2. Make decisions discoverable across context switches (human or agent)
3. Track decision lifecycle (proposed → accepted → deprecated/superseded)
4. Provide a shared convention that module developers can follow

This ADR lives in `suite-sdk` because the SDK is the primary public interface for the platform — module developers will reference it most frequently. It documents itself — it defines the format while being the first instance of that format.

## Options Considered

### Option A: Informal documentation in CLAUDE.md

- Embed decisions inline in existing architecture docs
- **Pros:** No new files, low ceremony
- **Cons:** Decisions become buried, no lifecycle tracking, hard to reference individually, difficult to supersede

### Option B: Structured ADRs in a dedicated directory

- One file per decision, numbered sequentially, following a consistent template
- **Pros:** Discoverable, individually referenceable, lifecycle is explicit, supports tooling
- **Cons:** Slightly more ceremony per decision, additional directory to maintain

### Option C: ADRs in a shared mono-repo location

- Single ADR directory covering all repositories
- **Pros:** One place to look
- **Cons:** Tight coupling between repos, numbering conflicts during parallel work, SDK decisions mixed with platform decisions

## Decision

Adopt **Option B** with independent numbering per repository:

### Location

| Repository | ADR Path | Numbering |
|------------|----------|-----------|
| `suite-sdk` | `docs/ADRs/ADR-NNN-short-title.md` | ADR-001, ADR-002, ... (this repo — canonical conventions) |
| `suite` | `docs/ADRs/ADR-NNN-short-title.md` | ADR-001, ADR-002, ... (independent sequence) |
| Module repos | `docs/ADRs/ADR-NNN-short-title.md` | ADR-001, ADR-002, ... (independent per module) |

Each repository maintains its own independent ADR sequence. Module repos are encouraged (not required) to adopt this format for their own architectural decisions.

### File Naming

- Format: `ADR-NNN-kebab-case-short-title.md`
- Numbers are zero-padded to three digits
- Numbers are never reused, even if an ADR is deprecated

### Template

Every ADR must include these sections:

```markdown
# ADR-NNN: [Decision Title]

## Status
Proposed | Accepted | Deprecated | Superseded by ADR-XXX

## Date
YYYY-MM-DD

## Context
What is the issue motivating this decision?
Include constraints (Edge-S, cluster topology, offline-first, semver) where relevant.

## Options Considered
### Option A: [Name]
- Description, pros, cons

### Option B: [Name]
- Description, pros, cons

## Decision
What are we doing? Reference files, patterns, or interfaces by name.

## Consequences
### Positive
### Negative
### Risks

## Compliance
How do we verify this decision is being followed?
```

### Status Lifecycle

```
Proposed → Accepted → Deprecated
                    → Superseded by ADR-XXX
```

- **Proposed:** Decision is drafted but not yet agreed upon
- **Accepted:** Decision is in effect
- **Deprecated:** Decision is no longer relevant (context changed)
- **Superseded:** A newer ADR replaces this one (link to successor)

When superseding an ADR, update the old ADR's status line to `Superseded by ADR-XXX`.

### Cross-Repository References

- Platform-wide conventions (like this one) live in `suite-sdk` since it is the shared public surface
- Decisions specific to the Suite runtime live in `suite`
- If a decision affects both, create it where it has the most impact and reference it from the other
- Module repositories may maintain their own ADR sequences for module-specific decisions; platform-level decisions should not be duplicated into module repos

### Authoring Guidelines

1. Keep ADRs concise — 1-2 pages maximum
2. Always present at least two options (even if one is "do nothing")
3. Name trade-offs explicitly — what are we giving up?
4. Reference ViciOne-specific constraints where they influenced the decision
5. Include a Compliance section defining how adherence is verified
6. Write in English for a technical audience

## Consequences

### Positive

- Decisions are discoverable and individually referenceable
- Rationale survives context switches between humans and agents
- Lifecycle tracking makes it clear which decisions are current
- Independent numbering avoids coordination overhead between repos

### Negative

- Slightly more ceremony than informal documentation
- Need to remember to check existing ADRs before writing new ones
- Directory must be created in each repository

### Risks

- ADRs could become stale if not referenced during code review
- Over-documentation: trivial decisions captured as ADRs add noise

## Compliance

- PR reviews for `suite`, `suite-sdk`, and module repos should check: does this change warrant an ADR?
- Existing ADRs should be referenced in code review when a change touches a governed area
- The `suite-adr-writer` skill (`.claude/skills/suite-adr-writer.md`) enforces template and quality standards during agent-driven authoring
- Module developers can reference [this ADR](https://gitlab.com/vicione-oss/vicione/suite/suite-sdk/-/blob/master/docs/ADRs/ADR-001-adr-conventions.md) as the canonical format definition
