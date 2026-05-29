# Architecture Decision Records — ViciOne Suite SDK

This directory contains Architecture Decision Records (ADRs) for the ViciOne Suite SDK. It is also the **canonical home** for platform-wide ADR conventions, since the SDK is the primary public interface for module developers.

## Format & Conventions

See [ADR-001-adr-conventions.md](ADR-001-adr-conventions.md) for the canonical ADR template, lifecycle rules, numbering scheme, and authoring guidelines. This ADR defines the format used across `suite-sdk`, `suite`, and module repositories.

## Numbering

Each repository maintains its own independent ADR sequence:

| Repository | Sequence |
|------------|----------|
| `suite-sdk` | ADR-001, ADR-002, ... (this directory) |
| `suite` | ADR-001, ADR-002, ... (independent) |
| Module repos | ADR-001, ADR-002, ... (independent, optional) |

## Cross-Repository Decisions

- Platform-wide conventions live here in `suite-sdk`
- Decisions specific to the Suite runtime live in `suite/docs/ADRs/`
- Module repos may adopt this format for their own decisions

## Index

| ADR | Title | Status |
|-----|-------|--------|
| [ADR-001](ADR-001-adr-conventions.md) | ADR Conventions | Accepted |
