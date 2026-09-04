use rustdoc_types::*;
use std::collections::HashMap;
use std::fmt::Write;
use std::path::{Path, PathBuf};
use std::process::Command;

/// Types that should be ignored during generation.
const EXCLUDE_TYPES: &[&str] = &[
    "History",
    "OrderedFloat",
    "PointerState",
    "SyntectSettings",
    "Undoer",
];

/// A crate for which rustdoc JSON should be generated.
struct DocCrate {
    /// The crate's name, as it appears in its own `Cargo.toml`.
    pub name: &'static str,
    /// Path (relative to the workspace root) of the directory containing the crate's `Cargo.toml`.
    pub manifest_dir: &'static str,
    /// Path (relative to the Egui.NET repo root) of the vendored subtree.
    pub workspace_root: &'static str,
}

/// Crates that rustdoc JSON is generated for.
const DOC_CRATES: &[DocCrate] = &[
    DocCrate {
        name: "ecolor",
        manifest_dir: "egui-rs/crates/ecolor",
        workspace_root: "egui-rs",
    },
    DocCrate {
        name: "egui",
        manifest_dir: "egui-rs/crates/egui",
        workspace_root: "egui-rs",
    },
    DocCrate {
        name: "emath",
        manifest_dir: "egui-rs/crates/emath",
        workspace_root: "egui-rs",
    },
    DocCrate {
        name: "epaint",
        manifest_dir: "egui-rs/crates/epaint",
        workspace_root: "egui-rs",
    },
    DocCrate {
        name: "egui_extras",
        manifest_dir: "egui-rs/crates/egui_extras",
        workspace_root: "egui-rs",
    },
    DocCrate {
        name: "egui_plot",
        manifest_dir: "egui_plot-rs/egui_plot",
        workspace_root: "egui_plot-rs",
    },
];

/// Determines the `--features` argument passed to `cargo rustdoc` for a given crate.
fn doc_features(crate_name: &str) -> &'static str {
    match crate_name {
        "egui_extras" => "serde,datepicker,image,svg,svg_text,syntect",
        _ => "serde",
    }
}

/// Determines whether `x` implements the trait with `path`.
fn impls_contains(krate: &Crate, impls: &[Id], path: &str) -> bool {
    for id in impls {
        let ItemEnum::Impl(impl_block) = &krate.index[id].inner else {
            unreachable!()
        };
        if impl_block.trait_.as_ref().is_some_and(|x| x.path == path) {
            return true;
        }
    }

    false
}

/// Retrieves a list of all `egui` types that are serializable.
fn gather_serde_tys(krate: &Crate, exclude_tys: &[&str]) -> Vec<Id> {
    let mut result = Vec::new();

    for (id, item) in &krate.index {
        let impls = match &item.inner {
            ItemEnum::Enum(x) => &x.impls,
            ItemEnum::Struct(x) => &x.impls,
            _ => continue,
        };

        if item
            .name
            .as_deref()
            .is_some_and(|x| exclude_tys.contains(&x))
        {
            continue;
        }

        if krate.paths[id].crate_id == 0
            && impls_contains(&krate, impls, "Serialize")
            && impls_contains(&krate, impls, "Deserialize")
        {
            result.push(id.clone());
        }
    }

    result.sort_by_key(|x| krate.index[x].name.as_deref());

    result
}

/// Emits a function that will perform reflection on all serializable types.
fn emit_tracer(crate_name: &str, krate: &Crate, exclude_tys: &[&str]) -> String {
    let ids = gather_serde_tys(krate, exclude_tys);

    let mut result = String::new();

    result.push_str("/// Registers all serializable `egui` types with the reflection system.\n");
    result.push_str("#[allow(warnings)]\n");
    result.push_str(&format!(
        "fn trace_auto_{crate_name}_types(tracer: &mut ::serde_reflection::Tracer) {{\n"
    ));

    for id in ids {
        let ty_name = krate.index[&id].name.clone().unwrap_or_default();
        write!(
            &mut result,
            "    tracer.trace_simple_type::<{ty_name}>().expect(\"Failed to trace {ty_name}\");\n"
        )
        .expect("Failed to write to string");
    }

    result.push_str("}\n");
    result
}

/// Runs `cargo rustdoc --output-format=json` against the vendored subtree for
/// each of [`DOC_CRATES`], parses the results, and writes the raw JSON into
/// `out_dir` (as `<crate>.json`) for `src/lib.rs` to embed via
/// `include_str!(concat!(env!("OUT_DIR"), ...))`.
fn regenerate_json_docs(root_dir: &Path, out_dir: &Path) -> HashMap<&'static str, Crate> {
    let cargo = std::env::var("CARGO").unwrap_or_else(|_| "cargo".to_string());
    let mut docs = HashMap::new();

    for doc_crate in DOC_CRATES {
        let crate_name = doc_crate.name;
        let workspace_root = root_dir.join(doc_crate.workspace_root);
        let crate_manifest = root_dir.join(doc_crate.manifest_dir).join("Cargo.toml");
        let target_dir = workspace_root.join("target");

        let status = Command::new(&cargo)
            // cwd = root_dir so rustup resolves the toolchain from Egui.NET/rust-toolchain.toml.
            .current_dir(root_dir)
            .args([
                "rustdoc",
                "--manifest-path",
                crate_manifest.to_str().expect("non-UTF8 path"),
                // Overrides any inherited CARGO_TARGET_DIR (e.g. set by `cross`).
                "--target-dir",
                target_dir.to_str().expect("non-UTF8 path"),
                "--lib",
                "--features",
                doc_features(crate_name),
                "--",
                "-Z",
                "unstable-options",
                "--output-format=json",
            ])
            .status()
            .unwrap_or_else(|e| panic!("Failed to run `{cargo} rustdoc` for `{crate_name}`: {e}"));

        if !status.success() {
            panic!("`cargo rustdoc` failed for crate `{crate_name}`");
        }

        let doc_path = target_dir.join("doc").join(format!("{crate_name}.json"));
        let contents = std::fs::read_to_string(&doc_path)
            .unwrap_or_else(|e| panic!("Failed to read {}: {e}", doc_path.display()));

        let embed_path = out_dir.join(format!("{crate_name}.json"));
        std::fs::write(&embed_path, &contents)
            .unwrap_or_else(|e| panic!("Failed to write {}: {e}", embed_path.display()));

        let krate = serde_json::from_str(&contents)
            .unwrap_or_else(|e| panic!("Failed to parse {}: {e}", doc_path.display()));

        docs.insert(crate_name, krate);
    }

    docs
}

/// Autogenerates a function for performing reflection on `egui` types.
fn main() {
    let root_dir = PathBuf::from(env!("CARGO_MANIFEST_DIR"))
        .join("..")
        .canonicalize()
        .expect("Failed to canonicalize repo root");

    println!("cargo::rerun-if-changed=build/main.rs");
    for doc_crate in DOC_CRATES {
        let root = root_dir.join(doc_crate.workspace_root);
        println!(
            "cargo::rerun-if-changed={}",
            root_dir.join(doc_crate.manifest_dir).display()
        );
        println!(
            "cargo::rerun-if-changed={}",
            root.join("Cargo.toml").display()
        );
        println!(
            "cargo::rerun-if-changed={}",
            root.join("Cargo.lock").display()
        );
    }

    let out_dir = PathBuf::from(std::env::var("OUT_DIR").expect("Failed to get output directory"));
    let docs = regenerate_json_docs(&root_dir, &out_dir);

    let out_file = out_dir.join("tracer.rs");

    let tracers: String = DOC_CRATES
        .iter()
        .map(|doc_crate| emit_tracer(doc_crate.name, &docs[doc_crate.name], EXCLUDE_TYPES))
        .collect::<Vec<_>>()
        .join("\n");

    std::fs::write(out_file, tracers).expect("Failed to write tracer bindings");
}
