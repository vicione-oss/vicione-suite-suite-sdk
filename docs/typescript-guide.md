[[_TOC_]]

## Introduction

This document describes how a client module writes its browser code in TypeScript instead of JavaScript. The SDK
compiles every `.ts` file of the module during the build and serves the result as a static web asset of the module.

How C# code loads and calls the compiled script is covered in the [JS interop guide](./js-interop-guide.md).

The guide uses client module `FooClientModule` and component `FooChart` as examples and assumes this file structure:

```
+ Foo.Client
  + Components
    + FooChart.razor
    + FooChart.razor.cs
    + FooChart.razor.ts
  + Foo.Client.csproj
  + FooClientModule.cs
  + package.json
  + tsconfig.json
```

## Setup

### 1. Reference the TypeScript compiler

`ViciOne.Suite.Sdk.Client` brings the build targets but not the compiler, so the module references
`Microsoft.TypeScript.MSBuild` itself. Versions up to 6.x and from 7.0 on are both supported.

```xml
<!-- Foo.Client/Foo.Client.csproj -->

<ItemGroup>
  <PackageReference Include="Microsoft.TypeScript.MSBuild" Version="7.0.1">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

Without this reference the `.ts` files are not compiled, and the build does not report it.

The targets only take effect in a project that references `ViciOne.Suite.Sdk.Client` directly; a project that gets it
transitively through another project does not compile its TypeScript.

### 2. Add `tsconfig.json`

All compiler options go into `tsconfig.json`. From `Microsoft.TypeScript.MSBuild` 7 on, the SDK clears the compiler
properties of the project file (`TypeScriptTarget`, `TypeScriptSourceMap`), so they cannot override `tsconfig.json`.

```json
// Foo.Client/tsconfig.json

{
  "compilerOptions": {
    "target": "es2022",
    "module": "es2022",
    "inlineSourceMap": true,
    "inlineSources": true,
    "strict": true,
    "noImplicitOverride": true,
    "forceConsistentCasingInFileNames": true,
    "skipLibCheck": true,
    "types": [ "blazor__javascript-interop" ]
  },
  "exclude": [
    "node_modules",
    "bin",
    "obj"
  ]
}
```

- `"module"` must produce an ES module: Blazor loads the script with `import()`.
- `"inlineSourceMap"` and `"inlineSources"` let the browser debugger show the TypeScript source.
- `"types"` provides the `DotNet` namespace, for example `DotNet.DotNetObject` for a `DotNetObjectReference` passed
  from C#. It needs the npm package below.

### 3. Add the type definitions

`Foo.Client/package.json`:

```json
{
  "private": true,
  "devDependencies": {
    "@types/blazor__javascript-interop": "^3.1.7"
  }
}
```

Run `npm install` once locally and `npm ci` in CI before `dotnet build`; the build does not install npm packages.
Add `node_modules/` to `.gitignore`.

### 4. Keep the config files out of the package

`tsconfig.json` and `package.json` are `Content` items of a Razor project and would otherwise be published and packed:

```xml
<!-- Foo.Client/Foo.Client.csproj -->

<ItemGroup>
  <Content Update="tsconfig.json;package.json;package-lock.json" CopyToPublishDirectory="Never" Pack="False" />
</ItemGroup>
```

## Write a script

Place the script next to the component it belongs to and export what C# calls. A class with a constructor and a
`dispose()` method fits the component lifecycle best; see the [JS interop guide](./js-interop-guide.md#call-the-module).

```typescript
// Foo.Client/Components/FooChart.razor.ts

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

## Where the compiled script ends up

The build moves every compiled `.js` file to `wwwroot/js/` of the project and serves it at
`_content/{assembly name}/js/{file name}`. The file name is converted to kebab case, and `.razor` and `.cs` are removed:

| Source | Served at |
|---|---|
| `Components/FooChart.razor.ts` | `_content/Foo.Client/js/foo-chart.js` |
| `Scripts/EventTargetMixins.ts` | `_content/Foo.Client/js/event-target-mixins.js` |
| `Scripts/HTMLParser.ts` | `_content/Foo.Client/js/html-parser.js` |
| `Components/MyUI.razor.ts` | `_content/Foo.Client/js/myu-i.js` |

- Folders are not kept: two sources with the same converted name overwrite each other, and so does a hand-written
  file of that name in `wwwroot/js/`.
- An acronym at the end of a name converts poorly (`MyUI` → `myu-i`); prefer names like `MyUi`.
- The path uses the assembly name, as `ModuleAssetHelper.GetModuleJsUrl<T>` does. Keep `PackageId` at its default,
  otherwise the script is served under the `PackageId` instead.
- The generated files are build output. Add them to `.gitignore`, for example `Foo.Client/wwwroot/js/*` if the module
  has no hand-written scripts there.

## Import one script from another

The browser resolves an `import` by URL, so import the served path of the other script. `paths` in `tsconfig.json`
maps that URL back to the source, so the compiler can check the types:

```typescript
// Foo.Client/Components/FooChart.razor.ts

import { formatValue } from '/_content/Foo.Client/js/value-formatter.js';
```

```json
// Foo.Client/tsconfig.json

{
  "compilerOptions": {
    "paths": {
      "/_content/Foo.Client/js/value-formatter.js": [ "./Scripts/ValueFormatter.ts" ]
    }
  }
}
```
