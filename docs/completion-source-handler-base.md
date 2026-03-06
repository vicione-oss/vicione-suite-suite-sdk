# CompletionSourceHandlerBase

## Overview

`CompletionSourceHandlerBase<TServiceResult>` is an abstract base class that implements the **fire-and-correlate** messaging pattern: a command is sent to the backend and the caller asynchronously awaits a correlated backend event that resolves the operation — or a timeout occurs after **x seconds** (`IUiMediator.CommandTimeoutMs`).

## Implementing a Subclass

There are **three mandatory steps**.

### 1 — Inherit and define the result type

Pick a result interface/type as the generic parameter. Implement the two abstract factories.

```csharp
protected override IYourHandlerResult CreateSuccessResult()
    => new YourHandlerSuccessResult();

protected override IYourHandlerResult CreateErrorResult(string errorMessage, int? errorCode = null)
    => new YourHandlerErrorResult(errorMessage, errorCode);
```

### 2 — Register event consumers in the constructor

Call `Register<TEvent>()` for every backend event the service must react to.
Subscriptions are tracked and disposed automatically when the service is disposed.

### 3 — Close the loop from `Consume`

In each `Consume` implementation, call `CompleteWithSuccess` or `CompleteWithError`
using the **event's correlation ID**. This resolves the `Task` that the caller is awaiting.

```csharp
// Success path 
public Task Consume(ClientContext<SomeCreatedEvent> context, CancellationToken cancellationToken) 
{ 
    CompleteWithSuccess(context.Message.CorrelationId); return Task.CompletedTask; 
}

// Error path 
public Task Consume(ClientContext<SomeErrorEvent> context, CancellationToken cancellationToken) 
{ 
    CompleteWithError(context.Message.CorrelationId, context.Message.ErrorInfo); return Task.CompletedTask; 
}
```

## Sending Commands

Use the protected `SendAndWaitForCompletion` overloads inside your public service methods.
The command's own `CorrelationId` is used automatically to match the incoming event.

### Standard — uses `CreateErrorResult` on failure

```csharp
public Task<IYourHandlerResult> CreateSomething(Some thing, CancellationToken cancellationToken = default) 
    => SendAndWaitForCompletion(new CreateSomethingCommand(thing), cancellationToken);
```

### Custom error factory — override error handling per call

```csharp
return await SendAndWaitForCompletion(command, errorInfo => new YourHandlerErrorResult($"The operation failed: {errorInfo.Message}"), cancellationToken);
```

### With post-send continuation — run extra logic after dispatch, before waiting

```csharp
return await SendAndWaitForCompletionAfterwards(command, afterSend: async ct => await NotifySomethingElse(ct), cancellationToken);
```

### Instance-scoped commands — routes the command to a specific backend instance

```csharp
return await SendAndWaitForCompletion(command, instanceId, cancellationToken);
```

## Disposal and Lifecycle

The class implements the full dispose pattern. On disposal:

- All event subscriptions are unregistered.
- All in-flight operations are **cancelled** immediately.
- Any `SendAndWaitForCompletion` call racing against disposal returns gracefully via `OperationCanceledException` (mapped to `CreateSuccessResult` internally — no exception escapes to the caller).

Concrete classes that hold their own disposable resources must override `Dispose(bool)`:

```csharp
protected override void Dispose(bool disposing) 
{ 
    if (disposing) _myResource?.Dispose();
    base.Dispose(disposing); // always last
}
```

## Behaviour Summary

| Scenario | Result returned |
|---|---|
| Backend fires success event within timeout | `CreateSuccessResult()` |
| Backend fires error event within timeout | `CreateErrorResult(message, code)` or custom factory |
| Timeout expires (10 s) | `CreateErrorResult(TheOperationHasTimedOut)` |
| `CancellationToken` cancelled | `CreateSuccessResult()` (silent cancel) |
| Command dispatch or `afterSend` throws | `CreateErrorResult(exception.Message)` |
| Disposed before or during the call | Operation cancelled, no exception to caller |
