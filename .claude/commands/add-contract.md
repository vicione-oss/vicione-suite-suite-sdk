# Add a new SDK contract

When adding a new public contract (interface, class, enum, attribute) to the SDK:

1. **Determine the correct package:**
   - `Sdk` — Shared contracts used by both Backend and Client (messaging interfaces, authorization, connections, module metadata)
   - `Sdk.Backend` — Backend-only abstractions (persistence, MassTransit consumers, ISuiteMediator)
   - `Sdk.Client` — Client-only abstractions (Blazor components, UI extension points)

2. **Requirements:**
   - Add comprehensive XML documentation (required by build — `GenerateDocumentationFile=true`)
   - Follow the code style guide (member ordering: constants → static fields → fields → properties → events → constructors → methods)
   - Consider backward compatibility — new interface members on interfaces implemented by module authors are BREAKING unless a default implementation is provided
   - C# default interface members (DIM) can be used to extend existing interfaces without breaking module authors — provide a sensible default
   - When DIM is not appropriate (e.g., no reasonable default exists), prefer adding new interfaces over extending existing ones

3. **Testing:**
   - Add unit tests in the corresponding test project
   - If the contract is testable by module authors, add helpers in `Sdk.Testing`

4. **Constraints to verify:**
   - Does this work on Edge-S (1 GB RAM, flash storage)?
   - Does this work in offline/disconnected scenarios?
   - Is this cluster-aware (master/slave)?
   - Is this idempotent if it involves messaging?

Provide the namespace, full implementation, XML docs, and tests.
