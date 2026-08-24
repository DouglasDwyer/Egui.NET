use csbindgen::*;
use egui_net_bindgen::*;
use std::fs::*;
use std::path::*;

/// Generates C# bindings for all C API files in the crate.
fn main() {
    // Workaround for cross-rs #1441 (https://github.com/cross-rs/cross/issues/1441)
    if std::env::var("CARGO_CFG_TARGET_OS").expect("Failed to get OS") == "windows" {
        if let Ok(root) = std::env::var("CROSS_SYSROOT") {
            let arch = match std::env::var("CARGO_CFG_TARGET_ARCH")
                .expect("Failed to get architecture")
                .as_str()
            {
                "x86_64" => "x64",
                "aarch64" => "arm64",
                _ => panic!("Unsupported architecture"),
            };

            println!(r"cargo:rustc-link-search={root}/lib/{arch}");

            let mut windows_kits = std::fs::read_dir("/opt/msvc/kits/10/lib")
                .expect("Failed to get MSVC root directory")
                .collect::<Result<Vec<_>, _>>()
                .expect("Failed to get Windows kits");

            windows_kits.retain(|x| x.file_type().expect("Failed to get file type").is_dir());
            windows_kits.sort_by_key(|x| x.file_name());
            let recent_kit = windows_kits.last().expect("No Windows kit installed");

            println!(
                r"cargo:rustc-link-search={}/um/{arch}",
                recent_kit.path().display()
            );
            println!(
                r"cargo:rustc-link-search={}/ucrt/{arch}",
                recent_kit.path().display()
            );
        }
    }

    println!("cargo::rerun-if-changed=../egui_net_bindgen");

    let output_dir =
        PathBuf::from(std::env::var("CARGO_MANIFEST_DIR").expect("Failed to get target directory"))
            .join("../target")
            .join("bindings");

    BindingsGenerator::generate(&output_dir);

    let mut builder = Builder::default()
        .csharp_namespace("Egui")
        .csharp_class_name("EguiBindings")
        .csharp_class_accessibility("internal")
        .csharp_dll_name("egui_net")
        .always_included_types([
            "EguiAnimatedUi",
            "EguiSwitchedUi",
            "EguiCallback",
            "EguiFn",
            "EguiScrollAreaShowRowsParams",
            "EguiScrollAreaShowViewportParams",
            "EguiContextRunTessellateParams",
            "EguiTextEditLayouterParams",
        ])
        .csharp_generate_const_filter(|_| true);

    for file in get_all_files("src") {
        builder = builder.input_extern_file(file);
    }

    builder = builder.input_extern_file(output_dir.join("egui_fn.rs"));

    let output_file = output_dir.join("Bindings.g.cs");
    builder
        .generate_csharp_file(&output_file)
        .expect("Failed to generate C# bindings");

    let file_contents = read_to_string(&output_file).expect("Failed to read bindings file.");
    write(output_file, file_contents).expect("Failed to generate renamed C# bindings");
}

/// Returns a list containing all files in the provided directory (and subdirectories).
fn get_all_files(path: impl AsRef<Path>) -> Vec<PathBuf> {
    let mut result = Vec::new();
    for maybe_entry in read_dir(path).expect("Failed to read directory") {
        let entry = maybe_entry.expect("Failed to read directory entry");
        let file_type = entry.file_type().expect("Failed to get file type");
        if file_type.is_dir() {
            result.extend(get_all_files(&entry.path()));
        } else if file_type.is_file() {
            result.push(entry.path());
        }
    }
    result
}
