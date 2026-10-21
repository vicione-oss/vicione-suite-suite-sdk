# Sdk.Client

Client module abstractions — Blazor components, UI extension points, IUiMediator.

- `ClientModule` base class with `Configure`, `InitializeServices`, `OnUserAuthenticated` hooks
- `IUiMediator` for sending commands/requests and subscribing to events from UI code
- UI extension points: navigation tiles, control panels, wizards, notification elements (see docs/)
- State classes implement change-notifying properties with `[NotifyChanged]` from the `ViciOne.SourceGenerators.NotifyChanged` package, which is not passed on to consumers; a property that depends on another one is updated in the generated partial methods `On<PropertyName>Changing` or `On<PropertyName>Changed`, never in `OnPropertyChanged` (see `NotificationElementState`)
- Auto-discovery via attributes and builder pattern (`.WithAutoDiscovery()`, `.WithSaveHandler()`)
- Depends on `ViciOne.Ui.Blazor.Components`
- TypeScript/SCSS compiled inline; coordinate CSS bundle changes with Sdk.Deployment
- `Targets/TypeScript.targets` ships to modules; keep `docs/typescript-guide.md` in sync with it, and `docs/js-interop-guide.md` with `IJsInterop`
