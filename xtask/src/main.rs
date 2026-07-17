//! Vendoring helper for the `egui-rs` subtree.
//!
//! Run with `cargo run --manifest-path xtask/Cargo.toml` (or the `cargo vendor-egui`
//! alias defined in `.cargo/config.toml`) after cloning, and again whenever
//! `egui-rs/` or `patches/egui_net.patch` change. It must be run before the main
//! workspace can build, since the main `Cargo.toml` patches `egui`/`egui_extras`
//! to a path this script produces.
//!
//! Steps:
//! 1. Copy `egui-rs/` to `egui-rs-patched/` (gitignored).
//! 2. Apply `patches/egui_net.patch` to that copy.
//! 3. Regenerate the rustdoc JSON files consumed by `egui_net_bindgen`.

use std::fs;
use std::path::{Path, PathBuf};
use std::process::{Command, ExitStatus};

/// Crates that `egui_net_bindgen` needs rustdoc JSON for. Must stay in sync
/// with the `include_str!` calls in `egui_net_bindgen/build/main.rs`.
const DOC_CRATES: &[&str] = &["ecolor", "emath", "epaint", "egui"];

fn main() {
    let skip_json = std::env::args().any(|arg| arg == "--skip-json");

    let root = repo_root();
    let egui_rs = root.join("egui-rs");
    let staged = root.join("egui-rs-patched");
    let patch_file = root.join("patches/egui_net.patch");

    println!("== Staging egui-rs -> {} ==", staged.display());
    stage_egui(&egui_rs, &staged);

    println!("== Applying {} ==", patch_file.display());
    apply_patch(&root, &staged);

    if skip_json {
        println!("== Skipping rustdoc JSON generation (--skip-json) ==");
    } else {
        println!("== Regenerating rustdoc JSON for {DOC_CRATES:?} ==");
        generate_json_docs(&root, &staged);
    }

    println!("== Done. egui-rs-patched/ is ready for `cargo build`. ==");
}

/// Returns the Egui.NET repo root (the parent of this `xtask` crate).
fn repo_root() -> PathBuf {
    let manifest_dir = PathBuf::from(env!("CARGO_MANIFEST_DIR"));
    manifest_dir
        .parent()
        .expect("xtask should be a subdirectory of the repo root")
        .to_path_buf()
}

/// Replaces `staged` with a fresh recursive copy of `egui_rs`.
fn stage_egui(egui_rs: &Path, staged: &Path) {
    if !egui_rs.is_dir() {
        panic!(
            "{} does not exist; is the egui-rs subtree checked out?",
            egui_rs.display()
        );
    }

    if staged.exists() {
        fs::remove_dir_all(staged)
            .unwrap_or_else(|e| panic!("Failed to remove old {}: {e}", staged.display()));
    }

    copy_dir_recursive(egui_rs, staged);
}

/// Recursively copies every file and subdirectory from `src` into `dst`.
fn copy_dir_recursive(src: &Path, dst: &Path) {
    fs::create_dir_all(dst).unwrap_or_else(|e| panic!("Failed to create {}: {e}", dst.display()));

    for entry in fs::read_dir(src).unwrap_or_else(|e| panic!("Failed to read {}: {e}", src.display())) {
        let entry = entry.expect("Failed to read directory entry");
        let file_type = entry.file_type().expect("Failed to get file type");
        let src_path = entry.path();
        let dst_path = dst.join(entry.file_name());

        if file_type.is_dir() {
            copy_dir_recursive(&src_path, &dst_path);
        } else if file_type.is_file() {
            copy_file(&src_path, &dst_path);
        }
        // Symlinks are not expected in the vendored subtree; skip anything else.
    }
}

/// Applies `patches/egui_net.patch` to the staged copy via `git apply`.
///
/// The patch is rooted at `a/egui-rs/...` / `b/egui-rs/...`, so `-p2` strips
/// the `a/egui-rs`/`b/egui-rs` prefix and `--directory` re-roots the result
/// under `egui-rs-patched/`.
fn apply_patch(root: &Path, staged: &Path) {
    let staged_rel = staged
        .strip_prefix(root)
        .expect("staged path should be under repo root");

    let status = run(
        "git",
        &[
            "apply",
            "-p2",
            &format!("--directory={}", staged_rel.display()),
            "patches/egui_net.patch",
        ],
        root,
    );

    if !status.success() {
        panic!(
            "Failed to apply patches/egui_net.patch to {}. \n\
             This usually means egui-rs/ has drifted from the commit the patch was \n\
             generated against. Regenerate the patch (diff the egui_net_patches fork \n\
             branch against the egui-rs subtree's current commit) and try again.",
            staged.display()
        );
    }
}

/// Runs `cargo rustdoc --output-format=json` for each crate in [`DOC_CRATES`]
/// and copies the results into `egui_net_bindgen/src/`.
fn generate_json_docs(root: &Path, staged: &Path) {
    let bindgen_src = root.join("egui_net_bindgen/src");

    for crate_name in DOC_CRATES {
        let manifest_path = staged.join("crates").join(crate_name).join("Cargo.toml");

        println!("-- cargo rustdoc -p {crate_name} --");
        // Run with cwd = repo root so rustup resolves the toolchain from
        // Egui.NET/rust-toolchain.toml (egui-rs-patched has no toolchain file
        // of its own; the patch removes it for exactly this reason).
        let status = run(
            "cargo",
            &[
                "rustdoc",
                "--manifest-path",
                manifest_path.to_str().expect("non-UTF8 path"),
                "--lib",
                "--features",
                "serde",
                "--",
                "-Z",
                "unstable-options",
                "--output-format=json",
            ],
            root,
        );

        if !status.success() {
            panic!("`cargo rustdoc` failed for crate `{crate_name}`");
        }

        let generated = staged
            .join("target/doc")
            .join(format!("{crate_name}.json"));
        let dest = bindgen_src.join(format!("{crate_name}.json"));

        copy_file(&generated, &dest);
    }
}

/// Copies a file via an explicit read+write rather than `fs::copy`.
///
/// `fs::copy` uses `copy_file_range` on Linux when overwriting an existing
/// destination, which fails with EPERM on some network/virtio-9p mounts
/// (e.g. a Windows drive mounted into WSL2) even though a plain read+write
/// works fine there.
fn copy_file(src: &Path, dst: &Path) {
    let contents =
        fs::read(src).unwrap_or_else(|e| panic!("Failed to read {}: {e}", src.display()));
    fs::write(dst, contents)
        .unwrap_or_else(|e| panic!("Failed to write {}: {e}", dst.display()));
}

/// Runs a command with the given args and working directory, streaming
/// stdout/stderr straight through, and returns its exit status.
fn run(program: &str, args: &[&str], cwd: &Path) -> ExitStatus {
    Command::new(program)
        .args(args)
        .current_dir(cwd)
        .status()
        .unwrap_or_else(|e| panic!("Failed to run `{program}`: {e}"))
}
