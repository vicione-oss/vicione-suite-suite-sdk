# Changelog

## 3.2.0 - unreleased

### Added

- `Sdk.Client`, `SettingsLayout.ContentPaddingRight` sets the free space right of settings contents, for use in narrow places such as dialogs

### Changed

- Rename the company to `ViciOne open automation gmbh` in the license and the package metadata

### Fixed

- `Sdk.Client`
  - `SettingsFieldSpinEdit`, `SettingsFieldSwitch` and `SettingsFieldTextBox` no longer swallow exceptions thrown by async `ValueChanged` handlers
  - `NotificationElementState.EndUpdate` passes `Changed` a stable copy of the property names and raises it outside its lock
  - `ControlPanelState` no longer delays a change made while the last update cycle ends
  - `ControlPanelState.BeginLoading` and `EndLoading` raise `Changed` outside the state's lock
  - `ItemValidatorBase.Validate` returns a new dictionary on every call. **Behaviour change:** a result kept from an earlier call no longer changes when validating again
  - `SettingsFieldFileUpload` matches dropped files against `Accept` like the browser does, including MIME types and wildcards
  - `SettingsFieldFileUpload` no longer cancels a running upload when a parent that does not pass `UploadTicket` renders again
  - Module stylesheet bundles no longer import the bundles the suite already loads, such as QuickGrid and TreeEditor

## 3.1.0 - 2026-09-24

### Added

- `Sdk.UserManagement`, added request `GetUserInformation` to fetch a list of users with basic information
- `Sdk.Backend.Messaging`
  - Added `MessagingHelper.FindAllMessageTypes`, returning every message type a consumer or activity handles, including the generic ones (`Fault<T>`) and the routing-slip contracts that `MessagingHelper.FindMessageTypes` filters out. `FindMessageTypes` answers "which message may name this receive endpoint" and is unchanged; `FindAllMessageTypes` is the set to use when classifying a consumer - which bus it belongs on, which retry ladder it gets. See ADR-002
- `Sdk.Client`, `SettingsFieldButton` forwards `Busy` and `BusyIndication` to the underlying `Button`, so an action can show that it is running on the button itself instead of the field replacing the button with a spinner
- `Sdk.Testing`, `TestWithDbContextSqlite` gets the standard async-dispose hook `DisposeAsyncCore`. `DisposeAsync` now calls it and then `Dispose(false)`, so a derived class can clean up on either path; before, `DisposeAsync` bypassed `Dispose(bool)` and an override never ran when the test framework disposed asynchronously. Both hooks run at most once

### Deprecated

- `Sdk`, `FormatExtensions.CalculateBytesToMb`, which is unused and whose name states the conversion backwards (it converts megabytes to bytes). It will be removed in the next major version; to display a file size, use `LongExtensions.LocalizeFileSizeHumanReadable` from `ViciOne.Ui.Localization`
- `Sdk.Testing`, `IRenderedComponentExtensions.TriggerGridFirstRowSelectionChange` and `TriggerGridRowSelectionChange`. They will be removed in the next major version. There is no replacement; implement your own helper if needed

### Fixed

- `Sdk.Backend.Messaging`
  - `MessagingHelper.ConsumesRequest` no longer reports non-request consumers as request consumers. It was an `All` over `FindMessageTypes`, which filters out generic message types, so a consumer with no remaining message types passed vacuously: every `IConsumer<Fault<T>>` and every `ConsumerDefinition<T>` answered `true`. It now requires at least one message type and evaluates all of them via `FindAllMessageTypes`. **Behaviour change:** `AddConsumers(MessagingHelper.ConsumesRequest, ...)` registers fewer consumers than before - fault consumers and consumer definitions are no longer registered by it, and any code that used the predicate to pick a bus or a retry policy will now classify those types differently. Classified as a behavioural defect fix under [ADR-002](docs/ADRs/ADR-002-contract-governance-for-behavioural-change.md), hence a minor rather than a major release
- `Sdk.Testing`, SQLite contexts from `TestDbContextFactory` and `TestModuleDbContextRegistrar` now close their connection when disposed

- `Sdk`
  - `ModuleIdResolver.ResolveId` removes only the trailing `.Backend`, `.Client`, `.Internal` or `.Public` suffix. It used `string.Replace`, so a name that contained the suffix elsewhere lost that part too: `Acme.Backend.Tools.Backend` resolved to `Acme.Tools` instead of `Acme.Backend.Tools`
- `Sdk.Backend`
  - `ConfigurationExtensions.BindSection(key, defaultValue)` binds into `defaultValue` as documented, so values the section does not set keep their defaults. For an object section it returned a freshly bound instance and dropped every value of `defaultValue`; for a scalar section it ignored the configured value and returned `defaultValue`. **Behaviour change:** a partially configured section now yields the merged object
- `Sdk.Backend.SourceGenerators`
  - `ModuleDbContextGenerator` now generates the `DbSet<T>` properties of every interface deriving from `IModuleDbContext`. It used to read only the first such interface and only the members that interface declared itself, so the `DbSet`s of a layered module interface (`IMyDbContext : IBaseDbContext : IModuleDbContext`) or of a second module interface were not generated and had to be written by hand. Hand-written properties are still skipped, so existing modules compile unchanged

- `Sdk.Testing`
  - `MetadataValidator.ValidateMetadata` accepts `*.Public` dependency assemblies again. It compared the assembly name against a `.Public.dll` suffix, which an assembly name never carries, so every dependency passed in was rejected as "no public module library". Its "invalid version" errors now name the value that failed to parse instead of the module's own version
  - `HttpMessageHandlerMock`, and so `HttpClientFactory.GetHttpClientWithResponse` and `AddHttpClient`, answer every request with the configured body. They returned the same `HttpContent` instance each time, so once the first response was disposed every later request failed with `ObjectDisposedException`

### Changed

- `Sdk.Client`, `_overflow.scss`, `.overflow-area` allows mouse events to pass through to underlying elements
- `Sdk`, `AddModuleFeature` rejects a feature name that contains `_` with an `ArgumentException`: the name-and-description overload when called, the factory overload when the feature is resolved. Policy names use `_` as separator, so `AccessLevelPolicyParser` cut such a name short and the policy never matched the feature. **Behaviour change:** a module that registered such a name now fails at startup instead of silently mis-authorizing

### Updated

- `AspNetCore.SassCompiler` package, update to version `1.103.1`
- `AwesomeAssertions` package, update to version `9.6.0`
- `bunit` package, update to version `2.11.3`
- `Microsoft.CodeAnalysis.CSharp` package, update to version `5.9.0`
- `Microsoft.Testing.Extensions.CodeCoverage` package, update to version `18.11.0`
- `Microsoft.Testing.Platform` package, update to version `2.4.1`
- `NSubstitute` package, update to version `6.2.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `6.1.1`
- `ViciOne.Ui.Design` package, update to version `2.5.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `4.18.0`
- `ViciOne.Ui.Localization` package, update to version `3.6.0`
- `xunit.v3` packages, update to version `4.0.1`

### Removed

- `CircularGaugeComponent`, not used anywhere

## 3.0.0 - 2026-08-17

### Updated

- `.NET` packages, updated to version `10.0.12`
- `Sdk.Client`, added `IClientModuleResourceProvider` to provide resources in client modules to be injected in Suite
- `ViciOne.Ui.Blazor.Components` package, update to version `5.20.0`
- `ViciOne.Ui.Design` package, update to version `2.4.0`

### Removed

- `Sdk.Client`, `SvgIcon` and `SvgIconExtensions` were removed as there is no use of it in SDK and in other repos
- `Sdk.Deployment`, removed `DevExpress` and `System.Drawing` library references

## 2.2.0 - 2026-08-03

### Updated

- `AspNetCore.SassCompiler` package, update to version `1.102.0`
- `AwesomeAssertions` package, update to version `9.5.0`
- `bunit` package, update to version `1.102.0`
- `Microsoft` packages, update to version `2.8.6`
- `Microsoft.Testing.Extensions.CodeCoverage` package, update to version `18.9.0`
- `Microsoft.Testing.Platform` package, update to version `2.3.3`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `10.0.3`
- `System.IO.Abstractions` packages, update to version `22.2.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `5.19.0`
- `ViciOne.Ui.Design` package, update to version `2.3.0`
- `ViciOne.Ui.Localization` package, update to version `3.5.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `4.15.0`

### Added

- `Sdk.Backend.Messaging`
  - Added `IInstanceDependentActivityArgument` marker interface (`IActivityArgument` + `IInstanceDependentMessage`) so an instance-targeted activity argument is expressed by a single, intent-revealing type, consistent with `IInstanceDependentCommand` / `IInstanceEvent` / `IInstanceDependentRequest`
- `Sdk.Backend.Diagnostics`
  - Added `MustDeclareAttribute` to declare that every concrete type implementing a decorated interface must itself carry a specified attribute, enforced at compile time by the `MustDeclareAnalyzer`
- `Sdk.Backend`
  - Added analyzer `VOSDK001` reporting a compile-time error when a concrete `IActivityArgument` or `IInstanceDependentActivityArgument` implementation is not decorated with the `MessageEndpointAttribute`, which previously only surfaced as an `InvalidOperationException` when the activity endpoint was resolved at runtime
  - Added `IAtomicFileWriter` (`Sdk.Backend.IO`) with a `WriteAsync` method to write a file atomically
- `Sdk`
  - Added `ModuleAuthorizeAttributeExtensions.GetAccessLevelAuthorizationRequirement` extension method, aligning the name with the `AccessLevelAuthorizationRequirement` type it returns
- `Sdk.Connections.Contracts`
  - Added `MqttProtocolVersion` enum to specify the MQTT protocol version
  - Added `MqttQualityOfServiceLevel` enum to specify the MQTT quality of service level
  - Added `MqttConnection` configuration properties: `ProtocolVersion`, `QualityOfService`, `SslProtocol`, `ClientCertificateKeyPassword`, `AllowUntrustedCertificates`, `KeepAliveSeconds`, and `ConnectTimeoutSeconds`

### Changed

- *Breaking* `Sdk`
  - Moved `IActivityArgument` from `ViciOne.Suite.Sdk` (`Sdk.Messaging`) to `ViciOne.Suite.Sdk.Backend` (`Sdk.Backend.Messaging`). Routing-slip activities are a backend-only concept, so the interface is no longer visible to client or SDK-core-only consumers; update `using Sdk.Messaging;` to `using Sdk.Backend.Messaging;` where activity arguments are defined
- *Breaking* `Sdk.Backend`
  - Split the `IRoutingSlipBuilder.AddActivity` extension overloads: the variant taking an instance id now requires `TArguments : IInstanceDependentActivityArgument` and a non-nullable `Guid`, while the id-less variant remains for load-balanced activities. Passing an instance id to a non-instance-dependent activity argument (which previously discarded the id and routed to a shared queue) is now a compile error
- `Sdk`
  - Deprecated `ModuleAuthorizeAttributeExtensions.GetAccessLevelRequirement`; use `GetAccessLevelAuthorizationRequirement` instead
- `Sdk.Connections.Contracts`
  - Deprecated `MqttConnectionType.TCPWithTLS`; use `MqttConnection.SslProtocol` instead. This value will be removed in a future version

### Fixed

- `Sdk.Backend`
  - `MessagingHelper.GetActivityEndpointName` now throws when an instance-dependent activity argument is resolved without an instance id, instead of silently producing a malformed endpoint name with an empty id segment
- `Sdk.Client`
  - `SettingsFieldComboBox`, adjusted alignment of `ComboBox`
- `Sdk.Testing`
  -  `MassTransitTester.TestEvent`, now forwards exceptions occuring on consumer instantiation

### Removed

- `Sdk.Client`
  - `ContentCardComponent` was removed, as there are dedicated components `Sdk.Client.Components.Cards.Components` to be used instead

## 2.1.0 - 2026-06-09

### Added

- Added `IControlPanelNetworkCategoryDescriptor` to allow module control panels to be categorized under the `Network` category

### Updated

- `AspNetCore.SassCompiler` package, update to version `1.100.0`
- `MassTransit` packages, update to version `8.5.10`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `10.0.2`
- `ViciOne.Ui.Localization` package, update to version `3.4.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `5.14.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `4.11.0`

## 2.0.1 - 2026-05-20

### Changed

- `_colors-staged.scss`, adjusted color `$color-red`

### Updated

- `Microsoft` packages, update to version `10.0.8`
- `ViciOne.Ui.Blazor.Components` package, update to version `5.12.0`
- `ViciOne.Ui.Design` package, update to version `2.1.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `4.10.0`

## 2.0.0 - 2026-04-16

### Added

- Publish samples to [GitLab pages](https://suite-sdk-0ceb85.gitlab.io)
- `Sdk.Messaging`
  - Added `ResponseEventBase` and `InstanceResponseEventBase` base class for response events
- *Breaking* Added `SystemType` property to `IInstanceInformation` interface
- *Breaking* Added `IsAvailable` property to `IControlServiceManagement` interface and renamed `ControleService` to `TryControlService` for clarity.
- `Sdk.Backend.Persistence`
  - Added `IModuleDbContextRegistrar` interface — the host application (suite) implements this to provide the actual database context resolution strategy
  - Added `ModuleDbContext` abstract base class with built-in default schema enforcement, `OnModuleModelCreating` / `OnConfigureConventions` hooks, and SQLite `DateTimeOffset` workaround
  - Added `Database` property to `IModuleDbContext` interface, exposing the `DatabaseFacade`
  - Added `AddModuleDbContext` extension method on `IServiceCollection` — modules call this to register their DbContext; it delegates to the host-provided `IModuleDbContextRegistrar`
  - Added `[ModuleDbContext]` attribute and incremental source generator (`Sdk.Backend.SourceGenerators`) that eliminates boilerplate for `ModuleDbContext` subclasses — automatically generates the `Sqlite`/`Postgres` implementation classes, their `IDesignTimeDbContextFactory` factories, a `protected` constructor, and optionally the `DefaultSchemaName` override
- `Sdk.Backend.Artifacts`
  - Added `ModifiedAfter` method to `IArtifactQueryBuilder` to limit the query results to artifacts modified after a certain date
- `Sdk.Client`
  - Added `ControlPanelSaveHandlerBase` and `ControlPanelResetHandlerBase` services with built-in support for async command, event operations and cancellation
  - Added `ILocalHttpClient` to support DI for typed `HttpClient` instances that are pre-configured for the local suite environment
  - Added `SettingsFieldSwitch` component for boolean settings
  - Added `SettingsFieldSpinEdit` component for numeric settings

### Fixed

- `NavTileStandardContent`, missing render cycle, which may cause nothing to be displayed on the navigation tile

### Changed

- *Breaking* Redesigned `Sdk.SystemConfiguration.Contracts` for a read-only, module-friendly API
  - Flattened namespace: all contract types are now in `Sdk.SystemConfiguration.Contracts` (removed `.Network`, `.Service`, `.Extensions` sub-namespaces)
  - All properties are now `init`-only; all collections are `IReadOnlyList<T>`
  - `SystemConfiguration`: replaced `NetworkInterfacesSettings`, `NetworkDNSSettings`, `NetworkProxySettings`, `NetworkNTPSettings` with `NetworkInterfaces`, `Dns`, `Proxy`, `Ntp`; replaced `List<ServiceDetail> Services` with `IReadOnlyList<ServiceInfo> Services`
  - `NetworkInterface` (new): flattened from `NetworkInterfaceDetail`, `NetworkInterfaceCommonInformation`, and `IPv4Settings`. `IpAddress`, `Netmask`, and `Gateway` always reflect the currently effective values (static or DHCP-assigned). `DhcpLease` (`DhcpLeaseInfo?`) is non-null when DHCP is active. `Vlan` (`VlanInfo?`) is non-null when VLAN is active. Additional IPs are in `AdditionalAddresses`.
  - `DnsSettings` (new, replaces `NetworkDNSSettings`): removed `StaticHostsEnabled`, `NameServersEnabled`, `DNSSuffixEnabled`, `SearchDomainsEnabled` flags — collections/values are only populated when the feature is enabled
  - `ProxySettings` (new, replaces `NetworkProxySettings`): each proxy is `ProxyInfo?` — `null` means disabled; removed `DoNotProxyListEnabled` flag
  - `ProxyInfo` (new, replaces `NetworkProxyDetail`): `Server` and `Port` are required; no `Enabled` flag — presence indicates enabled
  - `NtpSettings` (new, replaces `NetworkNTPSettings`): renamed `NTPServers` to `Servers`; removed `NTPServersEnabled` flag; added `FallbackServers`
  - `ServiceInfo` (new, replaces `ServiceDetail`)
  - `StaticHost` (new, replaces `StaticHostDetail`)
  - `IpAddressInfo` (new, replaces `IPv4Detail`)
  - `DhcpLeaseInfo` (new, replaces `DHCPLease`): simplified to only contain lease timestamps; IP/Gateway moved to `NetworkInterface`
  - `VlanInfo` (new): contains only `Id`
- Improve async disposal and error handling in `NavTileStandardContent`
- *Breaking* `IJsInterop.IncludeModuleScript` now requires a `Uri` parameter instead of string path for better type safety
- *Breaking* Overhauled `ModuleAssetHelper` which now provides `Uri` for module assets instead of string paths
- *Breaking* `IControlPanelDescriptor` also requires `Uri` for its icon source instead of string paths
- *Breaking* Unified naming of `Uri` properties (former strings) and methods to `...Url`
- *Breaking* `ICommand` and `IInstanceDependentCommand` now enforce providing an `init` for `CorrelationId` to ensure deserialization
- *Breaking* `RequestConsumer<TRequest, TResponse>` and `InstanceDependentRequestConsumer<TRequest, TResponse>` no longer use `IConsumeContext<TRequest>` in its methods
- `INavTileRegistry`, add `BeginUpdate` and `EndUpdate` methods by inheriting `IHasUpdateLock`

### Removed

- *Breaking* Removed `DbContextResolver` and `DbContextResolverOptions` from `Sdk.Backend.Persistence` — resolution logic is now provided by the host application via `IModuleDbContextRegistrar`
- *Breaking* Removed `AddDynamicDbContext` extension method — replaced by `AddModuleDbContext`
- *Breaking* Removed extension method `AppendFormattedLine` for `StringBuilder` class, as it can be easily replaced
- *Breaking* Removed `Sdk.SystemConfiguration.Contracts.Network` namespace and all types within (`NetworkInterfacesSettings`, `NetworkInterfaceDetail`, `NetworkInterfaceCommonInformation`, `IPv4Settings`, `IPv4Detail`, `DHCPLease`, `NetworkDNSSettings`, `NetworkNTPSettings`, `NetworkProxySettings`, `NetworkProxyDetail`, `StaticHostDetail`) — replaced by flattened types in `Sdk.SystemConfiguration.Contracts`
- *Breaking* Removed `Sdk.SystemConfiguration.Contracts.Service` namespace (`ServiceDetail` replaced by `ServiceInfo`)
- *Breaking* Removed `Sdk.SystemConfiguration.Contracts.Extensions` namespace (`IEnumerableExtensions`)
- *Breaking* Removed `MaterialDesignIconComponent`
- *Breaking* Removed `ControlServiceError`, `ConnectionErrorOccured` and `RoleErrorEvent`

### Updated

- `AspNetCore.SassCompiler` package, update to version `1.99.0`
- `bunit` package, update to version `2.7.2`
- `MassTransit` packages, update to version `8.5.9`
- `Microsoft` packages, update to version `10.0.6`
- `Microsoft.Testing.Platform` packages, update to version `2.2.1`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `10.0.1`
- `ViciOne.Ui.Blazor.Components` package, update to version `5.8.0`
- `ViciOne.Ui.Design` package, update to version `2.0.3`
- `ViciOne.Ui.Localization` package, update to version `3.3.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `4.7.0`
- `xunit` packages, update to version `xunit.v3 - 3.2.2`
- `xunit.extensibility.core` packages, update to version `xunit.v3.extensibility.core - 3.2.2`

## 1.1.0 - 2025-12-03

### Added

- `Sdk.Client.Wizards` namespace, for implementing wizards in client modules, see [guide](docs/wizard-guide.md) to learn more
- `Sdk.Client.Modules.ClientModule`, added method `Configure` to replace `ConfigureServices`
- `Sdk.Client.NavTiles`, added `INavTileRegistryFactory`, reserved for internal use
- `Sdk.Client.NotificationArea`, added `INotificationElementRegistryFactory`, reserved for internal use
- `Sdk.Client.ControlPanels.Services`
    - `ControlPanelState`, implemented `IHasUpdateLock`
    - Added `IControlPanelRegistryFactory`, reserved for internal use
- Added flag `Managed` to `Sdk.Connections.Contracts.Connection`
- `Sdk.Backend.Artifacts`
    - `IArtifactRepository`, added method `GetSourceKeys` and marked `CreateArtifact` as obsolete
    - `IArtifactQueryBuilder`, added methods `FilterBy` and `Limit`
    - `IArtifactQueryResult`, added optional property `Errors` and changed single `Range` to list of `Ranges`
    - `IArtifactQueryRange`, added property `Source`
    - `IArtifact`, added property `Kind` and `SourceKey`
- Sample application to showcase aspects of `Sdk.Client`
- New property `DisableDefaultFeature` added to `BackendModule`
- `Sdk.Connections.Contracts`
    - Added `IDatabaseConnection` interface for database connections
    - Added `PostgresConnection` for PostgreSQL database connections
    - Added `SQLiteConnection` for SQLite database connections
- `Sdk.Client.Connections`
    - Added `DatabaseSettings` component for database connection configuration
    - Added `DatabaseConnectionValidator` for validating database connections

### Changed

- `Sdk.Instance`
  - `IClusterInformationProvider`, changed `DateTime` to `DateTimeOffset`
  - `IInstanceInformation`, changed `DateTime` to `DateTimeOffset`
- `Sdk.Backend.ArtifactApi`, renamed to `Sdk.Backend.Artifacts`
  - `IArtifactQueryApi`, renamed to `IArtifactRepository`
  - `IArtifactItem`, renamed to `IArtifact`
  - `IArtifactRepository`, changed `DateTime` to `DateTimeOffset`
- `Sdk.Client`
  - `ControlPanels`
    - `ControlPanelBase`, marked `IsDirty` and `OnEdit` as obsolete
    - `IControlPanelService`, marked as obsolete
  - `colors.scss`, moved variables to `_colors-deprecated.scss` and `_colors-staged.scss` to produce a warning for deprecated variables
  - `Services`, marked interface `IClientModuleService` as obsolete
- `Sdk.Client.Components.Settings`
  - `SettingsField` and `SwitchExpander`, custom loading spinner replaced with `ViciOne.Blazor.Components.ContentLoadingIndication`
- `Sdk.Client.Modules`
    - `HostingModel` marked as obsolete
    - `ClientModule` marked `ConfigureServices` as obsolete
- `Sdk.Testing.Backend`
  - `TestInstanceInformation`, changed `DateTime` to `DateTimeOffset`
- `Sdk.Connections`
    - `ConnectionExtensions`, added type-specific methods for database connections
    - `ConnectionType`, updated to support new database connection types

### Updated

- `AspNetCore.SassCompiler` package, update to version `1.94.2`
- `MassTransit` packages, update to version `8.5.7`
- `Microsoft` packages, update to version `9.0.11`
- `Microsoft.TypeScript.MSBuild` package, update to version `5.9.3`
- `System.IO.Abstractions` package, update to version `22.1.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `4.2.0`
- `ViciOne.Ui.Design` package, update to version `1.1.1`
- `ViciOne.Ui.Localization` package, update to version `2.35.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.10.0`

### Removed

- Removed unused ConnectionType `Azure IoT Hub` from `ConnectionManagement`
- `Sdk.Connections.Contracts`
    - Removed `DatabaseConnection` class
    - Removed `DatabaseConnectionType`

### Fixed

- `NavTileStandardContent`, missing reinitialization when `Subline` has changed
- `npm`, vulnerabilities fixed

## 1.0.0 - 2025-07-24

### Added

- Support registry to register connection types

### Changed

- Remove `required` keyword from Blazor component parameters annotated with `EditorRequired`
- Replaced `FluentAssertions` with `AwesomeAssertions` version `9.1.0`
- Updated system configuration to match HostManagement 1.0.0
- `Sdk.Client.Components.Wallpaper`, vector-based wallpaper images to support high resolution displays

### Updated

- `MassTransit` packages, update to version `8.5.1`
- `Microsoft` packages, update to version `9.0.7`
- `System.IO.Abstractions` packages, update to version `22.0.15`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.8.7`
- `ViciOne.Ui.Localization` package, update to version `2.31.0`

### Removed

- Removed unused `IBrowserLocalStorageService`
- `Sdk.Client`, removed method `AddDevExtremeResources` from `ResourceExtensions`

## 0.31.0 - 2025-07-08

### Changed

- [**Breaking**] Replaced former AccessLevels with `Partial` and `Full`
- `Sdk.Client.Components.LinkButton`
  -  `LinkButton` stylesheets revised
- `Sdk.Client.Components.Card`, use of `LinkButton`

## 0.30.5 - 2025-07-03

### Added

- `Sdk`, added dependency to `ViciOne.CodeAnalysis.MustCallBase` to have an analyzer for missing base calls in consumer projects
- `Sdk.Client.ControlPanels`
  - `IControlPanelCancelHandler` reintroduced
  - `ControlPanelBase`, replaced `IDisposable` with `IAsyncDisposable`
- `Sdk.Client.Components`, added `Wallpaper`
- `Journal`, added `IJournalMonitoring`
- `Sdk.Backend.ArtifactApi`
    - `IArtifactQueryApi`, to query artifacts available via infrastructure
    - `IArtifactQueryBuilder`, to create queries to be used with `IArtifactQueryApi`

### Updated

- `ViciOne.CodeAnalysis` package, update to version `1.2.1`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.8.6`
- `MassTransit` packages, update to version `8.5.0`

## 0.30.4 - 2025-06-18

### Added

- `Sdk.Client.Modules.ModuleAssetHelper`, added `GetModuleImagePath<T>()`
- `Sdk.Client.Components.Settings.Expanders.SwitchExpander`, added `IsLoading`
- Added branch information to IInstanceInformation
- Added `SatelliteResourceLanguages` parameter on publish module script

### Removed

- Remove `MQTTnet.AspNetCore.dll` and `MQTTnet.Server.dll` from suite library set

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.89.2`
- `Microsoft` packages, update to version `9.0.6`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.8.5`
- `ViciOne.Ui.Localization` package, update to version `2.29.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.6.0`

## 0.30.3 - 2025-06-04

### Added

- `Sdk.Client.Components.StripComponent`, added `CssClass` and `OnClick`

### Changed

- `Sdk.Modules.ModuleMetadata`, add `AutonomousMigration` flag
- `Sdk.Client.NavTiles`
  - Adjust `letter-spacing` for subline

### Fixed

- Remove of `Microsoft.AspNetCore.Components.QuickGrid` module publish process

## 0.30.2 - 2025-05-30

### Updated

- `ViciOne.Ui.Blazor.Components` package, update to version `3.8.0`
- `ViciOne.Ui.Localization` package, update to version `2.27.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.5.0`

## 0.30.1 - 2025-05-28

### Added

- `Sdk.Backend.Modules`, added `IModuleHostRequestHandler`
- `Sdk.Client.NotificationArea`, added `NotificationElementFlyoutContentLayout`

## 0.30.0 - 2025-05-21

### Added

- Support for feature based authorization
- Add commands `Enable` and `Disable` to control services

### Changed

- Updated `Sdk.SystemConfiguration.Contracts` to match HM contracts version `0.10.0`

### Updated

- `ViciOne.Ui.Blazor.Components` package, update to version `3.7.1`
- `Microsoft.NET.Test.Sdk` package, update to version `17.14.0`

### Removed

- `Sdk`, removed classification suffix from request / event class names
- `Sdk.Client.ControlPanels`, removed obsolete code

## 0.29.1 - 2025-05-16

### Added

- `Sdk.Client.Components.FileDropComponent`, added localization
- `Sdk.Backend.SystemConfiguration`, added `IControlServiceManagement` to start/stop services on Linux OS

### Changed

- Updated `ViciOne.Suite.Sdk.Client.targets` to .NET SDK 9.0.300

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.89.0`
- `MassTransit` packages, update to version `8.4.1`
- `Microsoft` packages, update to version `9.0.5`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.7.0`

## 0.29.0 - 2025-04-29

### Added

- `Sdk.Client.ControlPanels`
  - Added `IControlPanelResetHandler`
  - Added additional parameter to methods of `IControlPanelSaveHandler` and `IControlPanelResetHandler` to allow passing a `CancellationToken`
- `Sdk.Connections.Events`, added `CorrelationId`
- `Sdk.SystemConfiguration.Events`, added `CorrelationId`

### Changed

- `Sdk.Connections.Events`
  - Renamed `TagChangedEvent` to `TagsChangedEvent`
  - Replaced parameter `Tag` with `Tags` in `TagsChangedEvent`
- `HostManagement` contracts updated to version `0.9.0`

### Removed

- `Sdk.Client.ControlPanels`, removed `IControlPanelCancelHandler`, cancel is signaled via `CancellationToken` to `IControlPanelSaveHandler` and `IControlPanelResetHandler`

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.87.0`
- `Microsoft.TypeScript.MSBuild` package, update to version `5.8.3`
- `System.IO.Abstractions` package, update to version `22.0.14`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.6.0`
- `ViciOne.Ui.Localization` package, update to version `2.25.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.4.0`

## 0.28.1 - 2025-04-15

### Added

- `Sdk.Client`, added `IJSObjectReferenceExtensions` class with method `TryInvokeAsync`, `TryInvokeVoidAsync` and `TryDisposeAsync` to safely cleanup JS references
- `ModuleDbContext`, added support for `DateTimeOffset` when context targets SQLite
- `Sdk.Testing`, added method `TriggerGridRowSelectionChange` to class `IRenderedComponentExtensions`

### Changed

-  `Sdk.Client.ControlPanels`, marked `InitialControlPanel` attributes as obsolete, warnings produced by this change inform on how to update existing code

### Fixed

- `ILayoutService`, `IsSettingsLoadingOverlayVisible` removed
- `Sdk.Client.ControlPanels`, resolve of `IControlPanelDescriptor` implementations is not throwing exceptions anymore when type is not public

### Removed

- `Sdk`, removed method `GetHumanReadableValue` from class `FormatExtensions`
- `Sdk.Backend`, removed property `ResourceDirectory` from class `BackendModule`
- `Sdk.Client.Components` package was removed, containing components were moved to `Sdk.Client` under the same namespace

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.86.2`
- `Microsoft` packages, update to version `9.0.4`
- `System.IO.Abstractions` package, update to version `22.0.13`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.5.1`

## 0.28.0 - 2025-03-19

### Added

- `Sdk.Authorization`, pre-defined type and structured value for module authorization claims
- `Sdk.Client.ControlPanels`, added support for `ModuleAuthorizeAttribute`
- `Sdk.Client.NotificationArea`, added support for `ModuleAuthorizeAttribute`

### Changed

-  Added the ServiceProvider to getting the ResourceDirectory from the BackendModule for more flexibility

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.86.0`
- `MassTransit` packages, update to version `8.4.0`
- `Microsoft` packages, update to version `9.0.3`
- `Microsoft.TypeScript.MSBuild` package, update to version `5.8.1`
- `System.IO.Abstractions` package, update to version `22.0.12`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `9.0.4`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.4.0`
- `ViciOne.Ui.Localization` package, update to version `2.22.1`

### Removed

- Removed obsolete `IWorkspaceService` class

## 0.27.4 - 2025-03-10

### Added

- `Sdk.Client.NavTiles`, added support for `ModuleAuthorizeAttribute`

### Removed

- Removed `EfMigration` tool

## 0.27.3 - 2025-03-05

### Changed

- `DescriptionBanner`, description text is now justified

## 0.27.2 - 2025-03-03

### Added

- `Sdk.Client.ControlPanels`, `IControlPanelSaveHandler` and `IControlPanelCancelHandler`

### Fixed

- `ComboBoxExpander`, `ReadOnly` is now `false` by default

## 0.27.1 - 2025-02-27

### Added

- `SettingsFieldCultureComboBox` / `SettingsFieldComboBox` / `ComboBoxExpander`, added `Enabled` and `ReadOnly`

### Changed

- `AspNetCore.SassCompiler` packages, update to version `1.85.1`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.3.1`
- `ViciOne.Ui.Localization` package, update to version `2.21.0`

## 0.27.0 - 2025-02-20

### Changed

- `SettingsFieldTextBox`, added `Subline`
- `Sdk.Testing` package
  - Removed classes `TestLoadContext`, `TestPersistenceConfiguration` and `TestApplicationFactory`
  - Removed extensions `AddClientLocalizer`, `CreateNavigationManager` `AddTestSetupMvc`, `ConfigureTestSetup`,
  - `IRenderedComponentExtensions`, added `GetSettingsField` and `AssertSettingsField` variants
- `IWorkspaceService`, obsoleted and replaced by `IWorkspaceProvider`
- `Sdk.Client.Components` package
  - Removed `CultureSelectorComponent`, `BreadcrumbComponent` and `LoadingSpinnerComponent`
  - Moved `IconAndValueComponent` to `Sdk.Client`

### Fixed

- `AddDynamicDbContext` extension, fixed generation of Sqlite dbname from ModuleId

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.85.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.2.0`
- `ViciOne.Ui.Localization` package, update to version `2.20.0`

## 0.26.4 - 2025-02-13

- `ControlPanelBase`, added `OnEdit()`

## 0.26.3 - 2025-02-13

- Events form controlling services will now be forwarded to the UI

## 0.26.2 - 2025-02-12

- `Sdk.Client.Components.Settings`, added `SettingsFieldComboBox` and `SettingsFieldCultureComboBox`

## 0.26.1 - 2025-02-12

### Changed

- `IInstanceInformation` class, added property `InRecoveryMode`
- `ControlService` command, added to control services using HostManagement

### Updated

- `MassTransit` packages, update to version `8.3.6`
- `Microsoft` packages, update to version `9.0.2`
- `Microsoft.TypeScript.MSBuild` package, update to version `5.7.3`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.1.0`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.3.0`
- `ViciOne.Ui.Localization` package, update to version `2.16.0`

## 0.26.0 - 2025-01-31

### Added

- Added a method for testing instance dependent requests to `MassTransitTester`
- Added `ModuleIdResolver` to resolve the `ModuleId` by reflection
- Added target `GetAbsolutePathsOfGeneratedJavaScriptFiles` to improve `TypeScript` file handling
- Added `ViciOne.Ui.Localization` package, version `2.13.0`
- Added `Sdk.NetworkStatus` to adopt network check functionality from Host Management
- `Sdk.Client.Components.Settings.SettingsField`, added `IsLoading` and `LoadingIndication`
- `Sdk.SystemConfiguration.Contracts`
  - `Network`
    - `NetworkDNSSettings`, adopted `MulticastDNSEnabled`, `NameServersEnabled`, `StaticHostsEnabled` and `StaticHosts` from Host Management
    - `IPv4Settings`, adopted `VLANEnabled` and `VLANID` from Host Management
  - `Service`, adopted code from Host Management

### Changed

- `BackendModule` class, removed abstract keyword `ModuleId`, gets initialized by reflection
- `ClientModule` class,  removed abstract keyword `ModuleId`, gets initialized by reflection
- `Sdk.Client.Components.Settings.SettingsGroup`, removed required constraint from `ChildContent`
- `Sdk.SystemConfiguration.Contracts.Network`, adopted change from `DateTime` to `DateTimeOffset` from Host Management
- Replaced unused `FeatureAuthorizeAttribute` with new `ModuleAuthorizeAttribute` and removed handling code from Sdk

### Updated

- `AspNetCore.SassCompiler` packages, update to version `1.83.4`
- `bunit` package, update to version `1.38.5`
- `FluentAssertions` packages, update to version `7.1.0`
- `Masstransit` packages, update to version `8.3.5`
- `Microsoft` packages, update to version `9.0.1`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `9.0.3`
- `System.IO.Abstractions` package, update to version `21.3.1`
- `ViciOne.Ui.MonochromeIcons` packages, update to version `3.2.0`
- `xunit` packages, update to version `2.9.3`

### Removed

- `ViciOne.Sdk.Localization` package, removed and replaced by [ViciOne.Ui.Localization](https://gitlab.com/vicione-oss/vicione/ui-libs/localization)

### Fixed

- Dispose `ITestHarness` in `MassTransitTester` to ensure correct stopping of `IHostedService`s

## 0.25.0 - 2024-12-18

### Added

- Added `IInstanceDependentRequest` for a more consistent messaging usage

### Removed

- Removed `SagaAttribute` as it wasn't really doing anything

## 0.24.0 - 2024-12-17

### Changed

- `AspNetCore.SassCompiler` package, update to version `1.83.0`
- `bunit` package, update to version `1.37.7`
- `MassTransit` packages, update to version `8.3.4`
- `xunit.runner.visualstudio` package, update to version `3.0.0`

## 0.23.0 - 2024-12-16

### Added

- Moved some existing unit tests from Suite to SDK

### Changed

- `MassTransit` packages, update to version `8.3.3`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `9.0.2`

### Messaging Overhaul
- Changed Messaging to use interfaces instead of attributes for most features in order to achieve better compiler support
- Unified Naming for methods of Backend- and UI-Mediator

## 0.22.0 - 2024-12-06

### Changed

- `AspNetCore.SassCompiler` package, update to version `1.82.0`
- `Sdk.Localization`, `CommonVocabulary`
  - Removed `Edit`, use `ViciOne.Ui.Localization.Resources.CommonVocabulary.EditVerb` instead
  - Removed `Username`, use `ViciOne.Ui.Localization.Resources.TechnicalTerms.Username` instead
- `Sdk`, removed obsolete classes `SystemUiEventAttribute`, `AssemblyInfoDto`, `PathUtils`
- `Sdk.Testing`, removed obsolete class `PathUtils`
- `System.IO.Abstractions` package, update to version `21.1.7`

## 0.21.0 - 2024-12-03

### Added

- `ViciOne.Suite.Sdk.Client.targets`, added target `CleanGeneratedJavaScriptFiles` to wipe generated assets on `Clean`

### Changed

- `AspNetCore.SassCompiler` package, update to version `1.81.1`
- `MassTransit.Abstractions` package, update to version `8.3.2`
- `Microsoft.AspNetCore.Authorization` package, update to version `9.0.0`
- `Microsoft.AspNetCore.Components.Web` package, update to version `9.0.0`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` package, update to version `9.0.0`
- `Microsoft.AspNetCore.Identity.UI` package, update to version `9.0.0`
- `Microsoft.EntityFrameworkCore.Design` package, update to version `9.0.0`
- `Microsoft.EntityFrameworkCore.Sqlite` package, update to version `9.0.0`
- `Microsoft.EntityFrameworkCore.Tools` package, update to version `9.0.0`
- `Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions` package, update to version `9.0.0`
- `Microsoft.Extensions.Localization` package, update to version `9.0.0`
- `Microsoft.TypeScript.MSBuild` package, update to version `5.7.1`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `9.0.1`
- `System.IdentityModel.Tokens.Jwt` package, update to version `8.2.1`
- `ViciOne.Ui.Blazor.Components` package, update to version `3.0.0`
- `ViciOne.Ui.MonochromeIcons.Assets` package, update to version `3.0.0`
- `ViciOne.Ui.MonochromeIcons.Components` package, update to version `3.0.0`
- `bunit` package, update to version `1.36.0`
- `FluentAssertions` package, update to version `7.0.0`
- `Microsoft.AspNetCore.Mvc.Testing` package, update to version `9.0.0`
- `Microsoft.EntityFrameworkCore.InMemory` package, update to version `9.0.0`
- `Microsoft.NET.Test.Sdk` package, update to version `17.12.0`
- `Microsoft.Build` package, update to version `17.12.6`
- `Microsoft.Extensions.Logging.Console` package, update to version `9.0.0`
- Classes implementing `IControlPanelDescriptor`, `IControlPanelCategoryDescriptor` or `IControlPanelGroupDescriptor` need to be public

### Removed

- Remove localization resources that have been added in `ViciOne.Ui.Localization`

## 0.20.0 - 2024-11-28

### Added

- Added `FileSystemExtensions` to `Sdk.Testing` to provide useful utility methods for the filesystem
- Added new properties to `IInstanceInformation` and `ILayoutService` to allow custom Instance titles

### Changed

- Changed `ModuleDependencyPackage.Version` property type to string
- Changed `MetadataValidator.ValidateMetadata` to support validation of module dependencies
- `ViciOne.Ui.Blazor.Components` package, update to version `2.0.0`

## 0.19.0 - 2024-11-18

### Added

- Added `AddModuleSection` to `IServiceCollectionExtensions` as helper to bind module configuration
- Added `MetadataValidator` to `Sdk.Testing` to provide metadata serialization test support
- Added `HostManagement` contract `SystemConfiguration`

### Changed

- `AspNetCore.SassCompiler` package, update to version `1.80.6`
- `MassTransit` package, update to version `8.3.1`
- `System.IO.Abstractions` package, update to version `21.1.3`
- `ViciOne.Ui.Blazor.Components` package, update to version `1.10.1`
- `ViciOne.Ui.MonochromeIcons` package, update to version `2.2.0`

### Removed

- `Sdk.Client.ControlPanels`
  - `IControlPanelRegistry<>`, removed obsolete `Add(descriptor, state)`
  - `IControlPanelDescriptor`, removed obsolete `Category`
  - `IActiveControlPanelPageProvider`, removed obsolete `GetActiveControlPanelPage()`

- `HostManagement.Shared` reference from `ViciOne.Suite.Sdk`

## 0.18.0 - 2024-10-28

### Added

- `Sdk.Client`
  - `ControlPanels`
    - `IControlPanelDescriptor`, property `ShowInNavigation` to specify whether control panel should be displayed in navigation or not
    - `IControlPanelRequest` to request a control panel from code

### Changed

- `AspNetCore.SassCompiler` package, update to version `1.80.4`
- `MassTransit` package, update to version `8.3.0`
- `Npgsql.EntityFrameworkCore.PostgreSQL` package, update to version `8.0.10`
- `ViciOne.Ui.Blazor.Components` package, update to version `1.7.5`
- `ViciOne.HostManagement` package, update to version `0.4.0`

- `Sdk.Client`
  - `ControlPanels`
    - `IActiveControlPanelPageProvider`, marked `GetActiveControlPanelPage()` as obsolete

      > This change affects all modules not updated to this SDK release as control panels using pages will display an initial empty page until update is done.

- `Sdk.Client.Components`
  - `Settings`
    - `DescriptionBanner`, some internals changed

      > This affects all modules not updated to this SDK release as description banner content will not be displayed until update is done.

## 0.17.0 - 2024-10-14

### Changed

- Target `PostProcessCssBundle` gets executed automatically on build and can be removed from Client projects
- Import `MoveGeneratedJavaScriptFiles.targets` gets executed automatically on build and can be removed from Client projects
- `ViciOne.Ui.MonochromeIcons` package, update to version `2.1.0`
- `AspNetCore.SassCompiler` package, update to version `1.79.5`
- `bunit` package, update to version `1.33.3`

### Added

- Added shared SCSS styles 'animations'
- Added shared SCSS styles 'badges'
- Added shared SCSS styles 'loading-spinner'
- Added shared SCSS styles 'nav-tiles'
- Added ability to send instance dependent requests using the UIMediator
- Added `IModuleMetadata` to provide module description for appstore

## 0.16.0 - 2024-09-30

### Changed

- `ViciOne.Ui.MonochromeIcons` package, update to version `1.15.0`
- `ViciOne.Ui.Blazor.Components` package, update to version `1.5.0`
- `IUiModuleBundle` support one module per assembly

## 0.15.0 - 2024-09-03

### Added

- Added SerialNumber to Instance-Information and Instance-Options

### Changed

- `HostManagement` packages, update to version `0.3.0`
- `Sdk.Deployment`, extend module CSS import optimization for `ViciOne.Ui.Blazor.Components`
- `ViciOne.Ui.Shared.Dx` packages, update to version `0.2.0`
- `ViciOne.Ui.MonochromeIcons.Assets` packages, update to version `1.14.0`

## 0.14.0 - 2024-08-21

### Added

- `Sdk.Client.Components.Settings`, `SettingsFieldSeparator` for visually separating a set of fields

### Changed

- Updated `DevExpress.Blazor` to `24.1.5`

## 0.13.0 - 2024-07-24

### Added

- `Sdk.Client`
  - `ControlPanels`
    - `ControlPanelPage` for wrapping content in control panels
    - `IControlPanelCategoryDescriptor` and `InitialControlPanelCategoryAttribute` for control panel categorization
    - `IControlPanelGroupDescriptor` and `InitialControlPanelGroupAttribute` for control panel grouping
    - `IControlPanelDescriptor`, property `Position` for controlling control panel order
- `Sdk.Client.Components`
  - `Settings`
    - `DescriptionBanner` for brief description of content rendered in control panel / control panel page
    - `SettingsLayout`, `SettingsGroup`, `SettingsField`, `SettingsFieldTextBox`, `SettingsFieldButton`, `SettingsStepper` and `SettingsInformation` for use in control panels / control panel pages
    - `ComboBoxExpander` and `SwitchExpander` for use in `SettingsGroup` for expander customization
  - `Switch` for toggling a boolean value
  - `TextBox` for string input

### Changed

- `Sdk.Client`
  - `ControlPanels`
    - `ControlPanelServiceKey`, removed generic parameter `TClientModule`

## 0.12.0 - 2024-06-12

### Added

- `Sdk.Client`
  - Support configuration of notification element order
  - Support TypeScript

### Changed

- `Sdk.Client`
  - Removed obsolete `NotificationBarElement` and dependencies
  - Removed obsolete `AddDxAllResources`
  - Removed obsolete code from `IBrowserLocalStorageService`

## 0.11.0 - 2024-04-10

### Changed

- `Sdk.Deployment` mandatory platform parameter was added to publish script

## 0.10.0 - 2024-02-27

### Added

- `Sdk.Localization` is published as NuGet package `ViciOne.Suite.Sdk.Localization` from now on

## 0.9.0 - 2024-02-19

### Added

- `Sdk`, `MasterHealthInfoChanged` sealed and parameter `type` removed
- `Sdk.Client`
  - `_typography.scss` added
  - `NotificationArea` feature (`NotificationBarElement` is obsolete)
- `Sdk.Client.Components`, `MaterialDesignIconComponent` added
- `Sdk.Localization`
  - `CommonVocabulary`, `Copy` added
  - `UserActions` added
- `Sdk.Testing`, `RandomExtensions` added

### Changed

- `Sdk.Client`
  - `IControlPanelRegistry<>` inherits from `IRegistry<>`
  - more robust service registration using keyed services

## 0.8.0 - 2024-02-06

### Changed

- Removed `Async` suffix from SDK api
- Updated engine host to `0.13.0`

### Removed

- `ControlPanelElement` support

## 0.7.0 - 2024-01-19

### Added

- `ControlPanel` feature (`ControlPanelElement` is obsolete)
- CSS post-processing during client module deployment (removal of `@import` statements targeting SDK CSS bundles)

### Removed

- `IBackgroundTaskQueue` support, replaced by HostedService

## 0.6.0 - 2023-12-14

### Changed

- Removed obsolete methods from SDK
- Updated target framework to .NET8

## 0.5.0 - 2023-12-07

### Changed

- Keep libraries for runtime `win` on module deployment

### Added

- `NavTile` feature (`NavItem` is obsolete)

## 0.4.0 - 2023-11-21

### Added

- Support Http connection
- Support Azure.Iot.Hub connection

### Changed

- Remove unsupported languages on module deployment

## 0.3.0 - 2023-11-06

### Added

- Property `Localizer` in `ControlPanelElement`
- Property `Localizer` in `NotificationBarElement`

### Changed

- `ClientModule`, property `LocalizationProvider` renamed to `Localizer`
- `NavItem`, property `LocalizationProvider` renamed to `Localizer`
- `BackendModule`, property `Version` removed

## 0.2.0 - 2023-10-27

### Added

- Provide shared `scss` files via `Sdk.Client` package
- `Sdk.Deployment` package with support scripts
- Property `LocalizationProvider` in `ClientModule` and `NavItem`

## 0.1.0 - 2023-10-16

- Initial Release
