# 🖌 Egui.NET: unofficial C# bindings for the easy-to-use egui library

[![Github](https://img.shields.io/badge/github-DouglasDwyer/Egui.NET-8da0cb?logo=github)](https://github.com/DouglasDwyer/Egui.NET)
[![NuGet version (Egui.NET)](https://img.shields.io/nuget/v/Egui.svg)](https://www.nuget.org/packages/Egui/)
[![view - documentation](https://img.shields.io/badge/view-documentation-4cbf1e)](https://douglasdwyer.github.io/Egui.NET)
[![MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/emilk/egui/blob/main/LICENSE-MIT)
[![Apache](https://img.shields.io/badge/license-Apache-blue.svg)](https://github.com/emilk/egui/blob/main/LICENSE-APACHE)

This project contains C# bindings for [egui](https://github.com/emilk/egui). egui (pronounced "e-gooey") is a simple, fast, and highly portable immediate mode GUI library written in Rust.

Egui.NET aims to provide safe, idiomatic C# bindings that mirror the Rust API as closely as possible. 

Egui.NET can be used anywhere you can draw textured triangles, which means you can easily integrate it into your game engine of choice.

For further information, please see [the primary `egui` README](https://github.com/emilk/egui/blob/main/README.md).

## Example

```csharp
ui.Heading("My egui Application");
ui.Horizontal(ui =>
{
    ui.Label("Your name:");
    ui.TextEditSingleline(ref name);
});
ui.Add(new Slider<int>(ref age, 0, 120).Text("age"));
if (ui.Button("Increment").Clicked)
{
    age += 1;
}
ui.Label($"Hello '{name}', age {age}");
ui.Image(EguiHelpers.IncludeImageResource("csharp.png"));
```

<img alt="Dark mode" src="media/demo.gif" height="278"> &nbsp; &nbsp; <img alt="Light mode" src="media/light_theme.png" height="278">

## Getting started

Egui.NET is available as a Nuget package. Use the following command to add it to your project:

```bash
dotnet add package Egui 
```

Then, create a `Context` and begin drawing UI as described in [the `egui` README](https://github.com/emilk/egui?tab=readme-ov-file#quick-start).

## Integrations

To use Egui.NET, the consumer is responsible for handling user input and rendering. To make this easy, Egui.NET comes with several _integration_ libraries:

- `egui_net_ffi`: support for passing Egui.NET data to Rust, for use with any Rust `egui` integration (like [`egui-winit`](https://crates.io/crates/egui-winit))
- `Egui.Silk.NET.OpenGL`: support for rendering to [`Silk.NET`](https://github.com/dotnet/Silk.NET) windows via OpenGL (thanks to [`@sjoerdev`](https://github.com/sjoerdev))
  - The [`SilkOpenGl`](/examples/SilkOpenGl/) example demonstrates how to incorporate Egui.NET using this backend
  
Need to add Egui.NET somewhere else? [Adding a new egui integration is easy](https://docs.rs/egui/latest/egui/#integrating-with-egui).

## WebAssembly

In addition to dynamic libraries for desktop platforms, the NuGet package ships a `wasm32-unknown-unknown` static library at `runtimes/browser-wasm/native/egui_net.a`, along with a `buildTransitive` targets file that adds it as a `NativeFileReference` whenever a consuming project builds with `RuntimeIdentifier=browser-wasm`. When building such a project with the `wasm-tools` workload installed, the .NET SDK links this archive directly into `dotnet.native.wasm`, allowing the Rust side of Egui.NET to run in the browser. To build the archive from source, run:

```bash
cargo rustc --profile release --target wasm32-unknown-unknown --package egui_net --crate-type staticlib
```

Note that `dotnet publish` for a browser-wasm app trims unused code by default; as of this writing, that trimming removes some of the reflection-based type serializers `EguiMarshal` relies on, which throws at startup (a `TypeInitializationException` for `SerializerCache<T>`). Until Egui.NET ships trim-safety annotations for that marshaling code, disable trimming for consuming projects that need it, e.g. `<PublishTrimmed>false</PublishTrimmed>` — though note that alone was not sufficient in initial testing here (it surfaced an unrelated Mono corelib/runtime version-mismatch failure); a working combination of properties has not yet been established. `dotnet build` (which never trims) is unaffected and was used to verify this target end-to-end.

## Incomplete features

Egui.NET aims to expose the entirety of `egui`'s functionality. Although 97% of bindings are complete, there are currently a few things missing:

- Functions requiring `'static` callbacks such as [`StyleModifier`](https://docs.rs/egui/latest/egui/style/struct.StyleModifier.html) or [`Grid::with_row_color`](https://docs.rs/egui/latest/egui/struct.Grid.html#method.with_row_color). These functions are difficult to bind because Rust-side code must own a C# closure
- Custom [`BytesLoader`](https://docs.rs/egui/latest/egui/load/trait.BytesLoader.html)s, [`ImageLoader`](https://docs.rs/egui/latest/egui/load/trait.ImageLoader.html)s, and [`TextureLoader`](https://docs.rs/egui/latest/egui/load/trait.TextureLoader.html)s
- Persistence (the ability to save a `Context` to disk and load it later)
- Accessibility and screen reader support with [`accesskit`](https://github.com/AccessKit/accesskit)

## Project structure

Egui.NET is designed so that:

- The bindings are completely safe; they cannot cause undefined behavior
- The bindings closely mirror the Rust API, while taking advantage of C# features like properties
- The bindings are easy to update for new `egui` releases

To this end, Egui.NET uses an *autobinder* to generate approximately 75% of the packaged code. The remaining bindings are written by hand, to deal with corner cases and make the library cleaner. Using a `json` file produced by `rustdoc`, the autobinder generates a list of **all** functions exposed by `ecolor`/`egui`/`emath`/`epaint`. The autobinder generates Rust-side and C#-side code for as many of these functions as possible. In addition, the autobinder uses [`serde-generate`](https://github.com/novifinancial/serde-reflection/tree/main/serde-generate) to emit C# type definitions for serializable `egui` types. Because the `egui` API is largely functional (with many plain-old-data types) this covers a large swath of the library.

To ensure safe communication between C# and Rust, FFI is performed using [bincode](https://github.com/bincode-org/bincode) serialization. When C# makes an `egui` function call at runtime, the arguments and return value are serialized across the language boundary.

The project is organized in the following way:

- docs -  contains files for customizing the DocFX output
- Egui - contains the C# project definition and manually-written C# bindings
- egui-rs - a vendored [subtree](https://git-scm.com/book/en/v2/Git-Tools-Advanced-Merging#_subtree_merge) of `egui`, with the C# interop changes described below committed directly on top
- egui_net - contains the runtime `egui` crate that is compiled to a `dylib` and loaded by C#.
  - progress_report.txt - describes what percentage of the API is complete, and identifies functions that have yet to be bound. Generated every time `cargo test` is run
- egui_net_bindgen - helper crate that contains the autobinder. Leverages `rustdoc` and `serde-generate` to bind most of egui automatically. Its build script regenerates rustdoc JSON straight from egui-rs/ on every build where egui-rs/ has changed, and the resulting bindings are written to `target/bindings` whenever `cargo build` is run
- examples - example programs demonstrating how to use the library in C#
- media - images and other media used in documentation
- serde-generate - a fork of the `serde-generate` crate, with some customizations for the Egui.NET API

The project needs a small amount of `egui` code to be marked as serializable so that it can be shared with C#. Those changes are committed directly onto the `egui-rs` subtree rather than maintained as a separate patch file, so `cargo build` works right after cloning with no extra setup step.

To pull in a newer `egui` release: `git subtree pull --prefix=egui-rs <egui remote> <tag> --squash`, then resolve any merge conflicts between the new upstream release and the existing C# interop changes.

### Supported platforms

- `aarch64-pc-windows-msvc`
- `x86_64-pc-windows-msvc`
- `aarch64-apple-darwin`
- `x86_64-apple-darwin`
- `aarch64-unknown-linux-gnu`
- `x86_64-unknown-linux-gnu`

### Building from source

To build from source, the following dependencies are required:

- [`.NET 9.0 SDK`](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [`Rust`](https://www.rust-lang.org/tools/install)
- [`cross-rs`](https://github.com/cross-rs/cross) (for cross-compiling the Nuget package)
- [`Docker`](https://www.docker.com/products/docker-desktop/) (for cross-compiling the Nuget package)

Once installed, the following commands should be available:

- `cargo build` - generate the Rust library for the current platform
- `cargo test` - build and update the `progress_report.txt`
- `dotnet build Egui` - build the Egui.NET library for the current platform
- `dotnet build Egui -c Release` - build the Egui.NET library in release mode for all platforms
- `dotnet pack` - generate a Nuget package for all platforms