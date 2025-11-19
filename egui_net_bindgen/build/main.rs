use rustdoc_types::*;
use std::fmt::Write;
use std::path::*;

/// Types that should be ignored during generation.
const EXCLUDE_TYPES: &[&str] = &[
    "History",
    "OrderedFloat",
    "PointerState",
    "Undoer",
];

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

/// Autogenerates a function for performing reflection on `egui` types.
fn main() {
    let out_dir = std::env::var("OUT_DIR").expect("Failed to get output directory");
    let out_file = PathBuf::from(out_dir).join("tracer.rs");
    
    let egui_tracer = emit_tracer("egui", &serde_json::from_str::<Crate>(include_str!("../src/egui.json")).expect("Failed to parse egui"), EXCLUDE_TYPES);
    let emath_tracer = emit_tracer("epaint", &serde_json::from_str::<Crate>(include_str!("../src/emath.json")).expect("Failed to parse emath"), EXCLUDE_TYPES);
    let epaint_tracer = emit_tracer("emath", &serde_json::from_str::<Crate>(include_str!("../src/epaint.json")).expect("Failed to parse epaint"), EXCLUDE_TYPES);
    let ecolor_tracer = emit_tracer("ecolor", &serde_json::from_str::<Crate>(include_str!("../src/ecolor.json")).expect("Failed to parse ecolor"), EXCLUDE_TYPES);
    
    std::fs::write(out_file, format!("{egui_tracer}\n{emath_tracer}\n{epaint_tracer}\n{ecolor_tracer}")).expect("Failed to write tracer bindings");
}