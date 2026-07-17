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
    "Undoer",
];

/// Crates in the egui-rs subtree that rustdoc JSON is generated for.
const DOC_CRATES: &[&str] = &["ecolor", "egui", "emath", "epaint"];

/// Determines whether `x` implements the trait with `path`.
fn impls_contains(krate: &Crate, impls: &[Id], path: &str) -> bool {
    for id in impls {
        let ItemEnum::Impl(impl_block) = &krate.index[id].inner else { unreachable!() };
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
            _ => continue
        };

        if item.name.as_deref().is_some_and(|x| exclude_tys.contains(&x)) {
            continue;
        }

        if krate.paths[id].crate_id == 0
            && impls_contains(&krate, impls, "Serialize")
            && impls_contains(&krate, impls, "Deserialize") {
            result.push(id.clone());
        }
    }

    result.sort_by_key(|x| krate.index[x].name.as_deref());

    result
}

/// Emits a function that will perform reflection on all serializable types.
fn emit_tracer(name: &str, krate: &Crate, exclude_tys: &[&str]) -> String {
    let ids = gather_serde_tys(krate, exclude_tys);

    let mut result = String::new();

    result.push_str("/// Registers all serializable `egui` types with the reflection system.\n");
    result.push_str("#[allow(warnings)]\n");
    result.push_str(&format!("fn trace_auto_{name}_types(tracer: &mut ::serde_reflection::Tracer) {{\n"));

    for id in ids {
        let name = krate.index[&id].name.clone().unwrap_or_default();
        write!(&mut result, "    tracer.trace_simple_type::<{name}>().expect(\"Failed to trace {name}\");\n").expect("Failed to write to string");
    }

    result.push_str("}\n");
    result
}

/// Runs `cargo rustdoc --output-format=json` against the egui-rs subtree for
/// each of [`DOC_CRATES`], parses the results, and writes the raw JSON into
/// `out_dir` (as `<crate>.json`) for `src/lib.rs` to embed via
/// `include_str!(concat!(env!("OUT_DIR"), ...))`.
fn regenerate_json_docs(manifest_dir: &Path, egui_rs: &Path, out_dir: &Path) -> HashMap<&'static str, Crate> {
    let cargo = std::env::var("CARGO").unwrap_or_else(|_| "cargo".to_string());
    let mut docs = HashMap::new();

    for &crate_name in DOC_CRATES {
        let crate_manifest = egui_rs.join("crates").join(crate_name).join("Cargo.toml");

        // cwd = manifest_dir so rustup resolves the toolchain from Egui.NET/rust-toolchain.toml.
        let status = Command::new(&cargo)
            .current_dir(manifest_dir)
            .args([
                "rustdoc",
                "--manifest-path", crate_manifest.to_str().expect("non-UTF8 path"),
                "--lib",
                "--features", "serde",
                "--",
                "-Z", "unstable-options",
                "--output-format=json",
            ])
            .status()
            .unwrap_or_else(|e| panic!("Failed to run `{cargo} rustdoc` for `{crate_name}`: {e}"));

        if !status.success() {
            panic!("`cargo rustdoc` failed for crate `{crate_name}`");
        }

        let doc_path = egui_rs.join("target/doc").join(format!("{crate_name}.json"));
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
    let manifest_dir = PathBuf::from(env!("CARGO_MANIFEST_DIR"));
    let egui_rs = manifest_dir.join("../egui-rs");

    // Deliberately not the whole egui-rs/ dir: `cargo rustdoc` below writes
    // into egui-rs/target/, which is nested inside it.
    println!("cargo::rerun-if-changed=build/main.rs");
    println!("cargo::rerun-if-changed={}", egui_rs.join("crates").display());
    println!("cargo::rerun-if-changed={}", egui_rs.join("Cargo.toml").display());
    println!("cargo::rerun-if-changed={}", egui_rs.join("Cargo.lock").display());

    let out_dir = PathBuf::from(std::env::var("OUT_DIR").expect("Failed to get output directory"));
    let docs = regenerate_json_docs(&manifest_dir, &egui_rs, &out_dir);

    let out_file = out_dir.join("tracer.rs");

    let egui_tracer = emit_tracer("egui", &docs["egui"], EXCLUDE_TYPES);
    let emath_tracer = emit_tracer("emath", &docs["emath"], EXCLUDE_TYPES);
    let epaint_tracer = emit_tracer("epaint", &docs["epaint"], EXCLUDE_TYPES);
    let ecolor_tracer = emit_tracer("ecolor", &docs["ecolor"], EXCLUDE_TYPES);

    std::fs::write(out_file, format!("{egui_tracer}\n{emath_tracer}\n{epaint_tracer}\n{ecolor_tracer}")).expect("Failed to write tracer bindings");
}
