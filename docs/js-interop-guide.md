[[_TOC_]]

## Introduction

This document describes how a client module loads its own JavaScript module from C#, calls into it, receives calls
back, and releases it again. It also lists the ready-made browser helpers of `IJsInterop`.

The script can be written in TypeScript, see the [TypeScript guide](./typescript-guide.md), or as a plain ES module
in `wwwroot/js/` of the client project. Either way it is served at `_content/{assembly name}/js/{file name}`.

The guide uses client module `FooClientModule` in assembly `Foo.Client` and the script `foo-chart.js` as examples.

## Load the module

Load the script in `OnAfterRenderAsync`: before the first render there are no elements to pass, and during
prerendering there is no browser to call. Load it once and keep the reference.

```csharp
// Foo.Client/Components/FooChart.razor.cs

[Inject] private IJsInterop JsInterop { get; set; } = default!;

private IJSObjectReference? _jsModule;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    _jsModule ??= await JsInterop.IncludeModuleScript(ModuleAssetHelper.GetModuleJsUrl<FooClientModule>("foo-chart.js"));

    if (_jsModule is null)
        return;

    // ...
}
```

`ModuleAssetHelper.GetModuleJsUrl<T>` builds `./_content/{assembly name of T}/js/{file name}`, where `T` is the client
module type. A URL must start with `./` or `/`; the browser rejects a bare `_content/…` as a module specifier. `./`
resolves against the document base, `/` against the host root.

Two APIs load a module. They differ in how a missing module shows up:

| API | Result | Use when |
|---|---|---|
| [`IJsInterop.IncludeModuleScript(Uri)`](../src/Sdk.Client/Services/IJsInterop.cs) | `IJSObjectReference?` | Default for components; the substitute in `Sdk.Testing` returns `null`, so tests skip the JavaScript code path |
| [`IJSRuntime.ImportScript(Uri)`](../src/Sdk.Client/Extensions/JSRuntimeExtensions.cs) | `IJSObjectReference`, throws `JSException` if the browser cannot load the module | Code that has an `IJSRuntime` at hand and treats a missing script as an error |

## Call the module

A script that keeps state for one component instance exports a class. C# creates one instance per component with
`InvokeConstructorAsync` and passes the elements and a `DotNetObjectReference` for calls back into C#.

```typescript
// foo-chart.js, here written in TypeScript

export class FooChart {
    constructor(readonly element: HTMLElement, readonly dotNetHelper: DotNet.DotNetObject) {}

    public async draw(values: number[]) {
        // ...
        await this.dotNetHelper.invokeMethodAsync('ChartDrawn');
    }

    public dispose() {
        // Remove event listeners and observers here.
    }
}
```

```csharp
// Foo.Client/Components/FooChart.razor.cs

private ElementReference _element;
private DotNetObjectReference<FooChart>? _dotNetObjectReference;
private IJSObjectReference? _chart;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    _jsModule ??= await JsInterop.IncludeModuleScript(ModuleAssetHelper.GetModuleJsUrl<FooClientModule>("foo-chart.js"));

    if (_jsModule is null || _chart is not null)
        return;

    _dotNetObjectReference = DotNetObjectReference.Create(this);
    _chart = await _jsModule.InvokeConstructorAsync("FooChart", _element, _dotNetObjectReference);

    await _chart.InvokeVoidAsync("draw", Values);
}

[JSInvokable]
public Task ChartDrawn()
{
    // Called from JavaScript; may run outside the renderer's synchronization context.
    return InvokeAsync(StateHasChanged);
}
```

A stateless script can export plain functions instead and be called directly on the module reference with
`InvokeAsync` / `InvokeVoidAsync`.

## Release the module

Implement `IAsyncDisposable` and release in this order: the JavaScript instance, the `DotNetObjectReference`, the
module.

```csharp
// Foo.Client/Components/FooChart.razor.cs

[Inject] private ILogger<FooChart> Logger { get; set; } = default!;

public async ValueTask DisposeAsync()
{
    await _chart.TryInvokeVoidAsync("dispose", Logger);
    await _chart.TryDisposeAsync(Logger);

    _dotNetObjectReference?.Dispose();

    await _jsModule.TryDisposeAsync(Logger);
}
```

The `Try*` extensions of [`IJSObjectReferenceExtensions`](../src/Sdk.Client/Extensions/IJSObjectReferenceExtensions.cs)
accept a `null` reference, ignore a `JSDisconnectedException`, which a closed Blazor Server circuit throws while the
component is disposed, and log any other error instead of throwing it.

## Built-in browser helpers

[`IJsInterop`](../src/Sdk.Client/Services/IJsInterop.cs) covers common browser tasks without a script of the module:

| Task | Members |
|---|---|
| Cookies | `GetCookie`, `SetCookie` |
| Document head | `UpdateTitle`, `IncludeMeta`, `IncludeLink`, `IncludeLinks` |
| Classic scripts | `IncludeScript`, `IncludeScripts`, `RemoveScriptsBySource` |
| Elements and forms | `GetElementByName`, `SetElementAttribute`, `RemoveElementsById`, `FormValid`, `SubmitForm` |
| Files | `GetFiles`, `UploadFiles`, `DownloadAs` |
| Navigation | `RedirectBrowser`, `RefreshBrowser` |

To add stylesheets of a module at runtime, pass their paths to `IJSRuntime.IncludeCssPaths`;
`ModuleAssetHelper.GetModuleCssUrl<T>` builds them.

## Testing

`AddClientServices` of `Sdk.Testing` registers a substitute for `IJsInterop` whose `IncludeModuleScript(Uri)` returns
`null`, so a component that loads its module as shown above renders in bUnit without any JavaScript set up. To test
the JavaScript calls themselves, configure `ClientServiceConfigurator.JsInterop` to return a substitute
`IJSObjectReference`, or use bUnit's `JSInterop` for code that goes through `IJSRuntime`.
