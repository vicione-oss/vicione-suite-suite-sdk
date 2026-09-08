# ADR-002: Contract Governance for Behavioural Change

## Status

Accepted

## Date

2026-09-07

## Context

The SDK ships as NuGet packages that `suite` and every module repository pin to an exact version. `AGENTS.md` states that the SDK follows strict semantic versioning and that "any breaking change to a public API requires a major version bump", and the `check-api-compat` skill turns that into a checklist. That checklist knows three shapes of change: signature-level breaks (major), new surface (minor), and "bug fixes that don't change the public API surface" (patch).

There is a fourth shape it has no name for, and it is the one that actually reaches module authors: **the signature is unchanged and the answer changes**. A predicate keeps its name, parameters and return type, and starts returning `false` where it returned `true`.

The concrete instance that forced this decision is `MessagingHelper.ConsumesRequest`. It was documented as "checks if a consumer type handles request messages" and implemented as

```csharp
consumerType.FindMessageTypes().All(a => a.HasInterface(typeof(IRequest<>)));
```

`FindMessageTypes` skips generic message types and the routing-slip contracts, so a consumer whose message types are all generic reports an empty sequence — and `All` over an empty sequence is `true`. The predicate therefore answered "yes, this is a request consumer" for every `IConsumer<Fault<T>>` (because `Fault<T>` is generic) and for every `ConsumerDefinition<T>` (which implements no `IConsumer<>` at all, and reaches the predicate wherever a scan is seeded with `RegistrationMetadata.IsConsumerOrDefinition`).

This was found during the `suite` architecture review (ADR-004, Phase 3a), where it had two live effects: fault consumers were handed the shortest retry ladder, and `AddLocalBus` registered fault consumers on every slave's local bus, contradicting `suite` ADR-004 (D6). Both were worked around inside `suite` (`ConsumerTypeExtensions.ConsumesOnlyRequests`) because the SDK is a pinned package and could not be changed from that branch. The defect stays in the SDK until it is fixed here, and every module author writing a fault consumer hits it.

The question this ADR answers is not "should we fix it" — it is "what does fixing it cost the version number", because that answer decides whether a defect of this shape can be fixed at all within a major line.

## Options Considered

### Option A: Treat every observable behaviour change as breaking

- The fix ships in 4.0.0
- **Pros:** Literal reading of `AGENTS.md`; no consumer can be surprised inside a major line
- **Cons:** A one-line defect fix costs every module repository a major upgrade, so in practice the fix is deferred indefinitely and the workaround in `suite` becomes permanent. It also prices honesty badly — the cheaper move becomes leaving the defect in place

### Option B: Additive only — new correct API, defective one left alone

- Add a correctly-behaving helper, mark `ConsumesRequest` `[Obsolete]`, remove it in the next major
- **Pros:** Nothing observable changes for existing callers; fits the deprecation policy exactly
- **Cons:** The defect has no correct dependents to protect. Every module author who reads the XML documentation and calls the documented API keeps getting the wrong bus registration for a whole major line, and the SDK carries two predicates that differ only in that one of them is wrong

### Option C: Classify by direction — a behavioural change toward the documented contract is a defect fix

- Introduce a fourth category and a rule for which side of the line a behaviour change falls on
- **Pros:** Distinguishes "the implementation was wrong" from "we changed our minds", which is the distinction that actually matters to a consumer
- **Cons:** Requires judgement about what the documented contract *was*, and that judgement is only as good as the XML documentation. Needs an explicit changelog obligation to stay safe

## Decision

Adopt **Option C**. SDK changes are classified in four categories, not three:

| Category    | Meaning                                                                                     | Version impact |
|-------------|---------------------------------------------------------------------------------------------|----------------|
| BREAKING    | Public surface removed, renamed or re-shaped; or a documented contract deliberately changed | Major          |
| BEHAVIOURAL | Signature unchanged, answers change for existing inputs                                     | See rule below |
| ADDITIVE    | New public types or members                                                                 | Minor          |
| PATCH       | No observable difference: internals, documentation                                          | Patch          |

**The rule for BEHAVIOURAL.** Determine which way the change moves the implementation relative to its documented contract:

- **Toward the contract** — the implementation was wrong and the documentation was right. This is a **defect fix** and ships in a minor or patch release. The documented contract, which is what a caller was entitled to rely on, does not change; only an undocumented accident of the implementation does.
- **Away from the contract, or the contract itself changes** — this is **breaking** and requires a major bump, however small the code change is.

A BEHAVIOURAL defect fix carries two obligations:

1. A `### Fixed` changelog entry that names the API, states the observable difference in terms of inputs and answers, and names the call pattern most likely to notice.
2. If the previous behaviour was plausibly *useful* to someone — as opposed to merely wrong — a replacement must be offered in the same release.

### Application to `MessagingHelper.ConsumesRequest`

Classified **BEHAVIOURAL, toward the contract → minor**, shipping in the unreleased **3.1.0**:

- `ConsumesRequest` now requires at least one message type and evaluates every message type the consumer handles. An `IConsumer<Fault<T>>`, a `ConsumerDefinition<T>` and a message-less type return `false`; a request consumer, including one deriving from `RequestConsumer<,>`, still returns `true`.
- The vacuous `true` was never the documented contract. No caller could correctly depend on "handles request messages" being true for a type that handles no request.
- No replacement is offered for the old answer, because obligation 2 does not apply: there is no legitimate use for classifying a fault consumer as a request consumer.

The deeper cause is addressed rather than papered over. `FindMessageTypes` filtering generic message types is **correct for what it is for** — resolving the queue a consumer binds to, where `Fault<T>` and the routing-slip contracts must never decide the name — but it is the wrong set for *classifying* a consumer, because a consumer of only such messages appears to consume nothing. The two uses are now separate APIs:

| API | Set | Use |
|---|---|---|
| `FindMessageTypes` | Behaviour unchanged; generic message types and routing-slip contracts filtered out, with the filtering and its rationale now documented instead of implicit | Naming a receive endpoint |
| `FindAllMessageTypes` | ADDITIVE; every consumed message type, `Fault<T>` and the routing-slip contracts included | Classifying a consumer — which bus, which retry ladder |

`ConsumesRequest` is built on `FindAllMessageTypes`, and so must every other classification be.

## Consequences

### Positive

- Defects in behaviour can be fixed inside a major line, so the correct action is also the cheap one
- The distinction a consumer cares about — "was I relying on documented behaviour, or on an accident?" — becomes the one the version number encodes
- The `Fault<T>` blind spot is visible in the API rather than hidden in a `continue`, so the next caller that needs to classify a consumer has the right tool
- `suite` can delete `ConsumerTypeExtensions.ConsumesOnlyRequests` once it pins a 3.1.0 package containing this fix

### Negative

- Classification needs judgement, and that judgement rests on the XML documentation being accurate. A vague doc comment makes a change unclassifiable
- A consumer who depended on the defect gets no compile error, only a behaviour change discovered at runtime
- One more category for reviewers to carry

### Risks

- **The category is abusable.** "The documentation was right, the code was wrong" can be told about almost any behaviour change after the fact. Mitigation: the direction test must cite the documentation *as it stood before the change*; if the documentation has to be edited to make the new behaviour correct, the change is BREAKING, not BEHAVIOURAL
- **Silent adoption.** A module pinning 3.1.0 for an unrelated feature also gets this fix. That is intended, and is why the changelog obligation is part of the rule rather than a courtesy
- **Under-detection.** `check-api-compat` compares public surface, which by definition cannot see a BEHAVIOURAL change. It has to be caught by a human reading the diff

## Compliance

- The `check-api-compat` skill carries the BEHAVIOURAL category and its direction rule; its output classifies every changed method body in a public type, not only changed signatures
- PR review for `suite-sdk` asks of any edited public method body: *does this return a different answer for an input that reaches it today?* If yes, the direction test above is applied in the MR description
- A BEHAVIOURAL defect fix without a `### Fixed` changelog entry naming the observable difference is not mergeable
- Behavioural fixes are cross-referenced from the consuming repository's ADR wherever a local workaround exists, so the workaround can be removed when the package is bumped — here `suite` ADR-004 (D2) and (D6)
