# Sdk.Client

Client module abstractions — Blazor components, UI extension points, IUiMediator.

- `ClientModule` base class with `Configure`, `InitializeServices`, `OnUserAuthenticated` hooks
- `IUiMediator` for sending commands/requests and subscribing to events from UI code
- UI extension points: navigation tiles, control panels, wizards, notification elements (see docs/)
- Auto-discovery via attributes and builder pattern (`.WithAutoDiscovery()`, `.WithSaveHandler()`)
- Depends on `ViciOne.Ui.Blazor.Components`
- TypeScript/SCSS compiled inline; coordinate CSS bundle changes with Sdk.Deployment
