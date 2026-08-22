//! Allows for efficiently communicating with a C#-side `egui` Context from [Egui.NET](https://github.com/DouglasDwyer/Egui.NET).

use std::ops::Deref;

use egui::*;
use egui::epaint::*;
use egui::output::{IMEOutput, OutputEvent};
use serde::*;

/// Allows for passing `egui` data back and forth with C#.
#[repr(C)]
pub struct EguiFfi {
    /// The serialized bytes of the most recent [`FullOutput::platform_output`].
    platform_output: FfiVec<u8>,
    /// The serialized bytes of the most recent [`FullOutput::textures_delta`].
    textures_delta: FfiVec<u8>,
    /// The most recent [`FullOutput::pixels_per_point`].
    pixels_per_point: f32,
    /// The meshes from the most recent call to [`Context::tessellate`].
    meshes: FfiVec<(Rect, FfiTextureId, FfiVec<u32>, FfiVec<Vertex>)>,
    /// The most recent raw input.
    raw_input: FfiVec<u8>,
}

impl EguiFfi {
    /// Gets the most recent [`FullOutput::platform_output`].
    ///
    /// This is a separate call from [`Self::textures_delta`] so that a caller
    /// that only needs one of the two does not have to pay for decoding
    /// (and copying) the other.
    pub fn platform_output(&self) -> PlatformOutput {
        bincode::serde::decode_from_slice::<PlatformOutput2, _>(&self.platform_output, bincode::config::legacy())
            .expect("Failed to deserialize PlatformOutput").0.into()
    }

    /// Sets the most recent [`FullOutput::platform_output`].
    pub fn set_platform_output(&mut self, platform_output: PlatformOutput) {
        self.platform_output = bincode::serde::encode_to_vec(&PlatformOutput2::from(platform_output), bincode::config::legacy())
            .expect("Failed to serialize PlatformOutput").into();
    }

    /// Gets the most recent [`FullOutput::textures_delta`].
    ///
    /// This is a separate call from [`Self::platform_output`] so that a caller
    /// that only needs one of the two does not have to pay for decoding
    /// (and copying) the other.
    pub fn textures_delta(&self) -> TexturesDelta {
        bincode::serde::decode_from_slice(&self.textures_delta, bincode::config::legacy())
            .expect("Failed to deserialize TexturesDelta").0
    }

    /// Sets the most recent [`FullOutput::textures_delta`].
    pub fn set_textures_delta(&mut self, textures_delta: &TexturesDelta) {
        self.textures_delta = bincode::serde::encode_to_vec(textures_delta, bincode::config::legacy())
            .expect("Failed to serialize TexturesDelta").into();
    }

    /// Gets the most recent [`FullOutput::pixels_per_point`].
    pub fn pixels_per_point(&self) -> f32 {
        self.pixels_per_point
    }

    /// Sets the most recent [`FullOutput::pixels_per_point`].
    pub fn set_pixels_per_point(&mut self, pixels_per_point: f32) {
        self.pixels_per_point = pixels_per_point;
    }

    /// Sets the most recent [`FullOutput`], by dispatching to
    /// [`Self::set_platform_output`], [`Self::set_textures_delta`], and
    /// [`Self::set_pixels_per_point`]. [`FullOutput::shapes`] and
    /// [`FullOutput::viewport_output`] are not stored.
    pub fn set_full_output(&mut self, full_output: FullOutput) {
        self.set_platform_output(full_output.platform_output);
        self.set_textures_delta(&full_output.textures_delta);
        self.set_pixels_per_point(full_output.pixels_per_point);
    }

    /// Gets the most recent [`RawInput`].
    pub fn raw_input(&self) -> RawInput {
        bincode::serde::decode_from_slice(&self.raw_input, bincode::config::legacy())
            .expect("Failed to deserialize RawInput").0
    }

    /// Sets the most recent [`RawInput`].
    pub fn set_raw_input(&mut self, raw_input: RawInput) {
        self.raw_input = bincode::serde::encode_to_vec(&raw_input, bincode::config::legacy())
            .expect("Failed to serialize RawInput").into();
    }

    /// Gets the most recent output of [`Context::tessellate`].
    pub fn meshes(&self) -> Vec<(Rect, Mesh)> {
        self.meshes.iter().map(|(clip_rect, texture_id, indices, vertices)|
            (*clip_rect, Mesh {
                indices: indices.to_vec(),
                texture_id: (*texture_id).into(),
                vertices: vertices.to_vec()
            })).collect()
    }

    /// Sets the most recent output of [`Context::tessellate`].
    pub fn set_meshes(&mut self, meshes: impl IntoIterator<Item = (Rect, Mesh)>) {
        self.meshes = meshes.into_iter()
            .map(|(clip_rect, mesh)| (clip_rect, mesh.texture_id.into(), mesh.indices.into(), mesh.vertices.into()))
            .collect::<Vec<_>>().into()
    }
}

impl Default for EguiFfi {
    fn default() -> Self {
        let mut result = Self {
            platform_output: Vec::new().into(),
            textures_delta: Vec::new().into(),
            pixels_per_point: 1.0,
            meshes: Vec::new().into(),
            raw_input: Vec::new().into()
        };

        result.set_full_output(FullOutput::default());
        result.set_raw_input(RawInput::default());

        result
    }
}

/// Holds the serializable members of [`PlatformOutput`], including `accesskit_update`
/// (which is `#[serde(skip)]` on [`PlatformOutput`] itself; see its doc comment).
#[derive(Clone, Serialize, Deserialize)]
struct PlatformOutput2 {
    /// The [`PlatformOutput::commands`] field.
    pub commands: Vec<OutputCommand>,
    /// The [`PlatformOutput::cursor_icon`] field.
    pub cursor_icon: CursorIcon,
    /// The [`PlatformOutput::events`] field.
    pub events: Vec<OutputEvent>,
    /// The [`PlatformOutput::mutable_text_under_cursor`] field.
    pub mutable_text_under_cursor: bool,
    /// The [`PlatformOutput::ime`] field.
    pub ime: Option<IMEOutput>,
    /// The [`PlatformOutput::accesskit_update`] field.
    pub accesskit_update: Option<accesskit::TreeUpdate>,
    /// The [`PlatformOutput::num_completed_passes`] field.
    pub num_completed_passes: usize
}

impl From<PlatformOutput2> for PlatformOutput {
    fn from(value: PlatformOutput2) -> Self {
        Self {
            commands: value.commands,
            cursor_icon: value.cursor_icon,
            events: value.events,
            mutable_text_under_cursor: value.mutable_text_under_cursor,
            ime: value.ime,
            accesskit_update: value.accesskit_update,
            num_completed_passes: value.num_completed_passes,
            ..Default::default()
        }
    }
}

impl From<PlatformOutput> for PlatformOutput2 {
    fn from(value: PlatformOutput) -> Self {
        Self {
            commands: value.commands,
            cursor_icon: value.cursor_icon,
            events: value.events,
            mutable_text_under_cursor: value.mutable_text_under_cursor,
            ime: value.ime,
            accesskit_update: value.accesskit_update,
            num_completed_passes: value.num_completed_passes
        }
    }
}

/// An FFI-compatible version of [`TextureId`].
#[derive(Copy, Clone, Debug)]
#[repr(C)]
struct FfiTextureId {
    /// The type of ID.
    pub kind: TextureIdKind,
    /// The integer identifier.
    pub id: u64
}

impl From<TextureId> for FfiTextureId {
    fn from(value: TextureId) -> Self {
        match value {
            TextureId::Managed(id) => Self {
                kind: TextureIdKind::Managed,
                id
            },
            TextureId::User(id) => Self {
                kind: TextureIdKind::User,
                id
            }
        }
    }
}

impl From<FfiTextureId> for TextureId {
    fn from(value: FfiTextureId) -> Self {
        match value.kind {
            TextureIdKind::Managed => TextureId::Managed(value.id),
            TextureIdKind::User => TextureId::User(value.id),
        }
    }
}

/// Identifies a variant of [`TextureId`].
#[derive(Copy, Clone, Debug)]
#[repr(C)]
enum TextureIdKind {
    /// [`TextureId::Managed`]
    Managed,
    /// [`TextureId::User`]
    User
}

/// A [`Vec`] that can be passed across FFI boundaries.
#[derive(Debug)]
#[repr(C)]
struct FfiVec<T> {
    /// Pointer to the allocation.
    ptr: *mut T,
    /// The length of the vector.
    len: usize,
    /// The capacity of the vector.
    capacity: usize,
    /// The function to call in order to free the vector.
    on_free: unsafe extern "C" fn(&mut Self)
}

impl<T> FfiVec<T> {
    /// Frees the vector using its original allocator.
    /// 
    /// # Safety
    /// 
    /// This function should be called a single time on each valid [`FfiVec`].
    unsafe extern "C" fn on_free(&mut self) {
        // Safety: this s
        unsafe { Vec::from_raw_parts(self.ptr, self.len, self.capacity); }
    }
}

impl<T> From<Vec<T>> for FfiVec<T> {
    fn from(mut value: Vec<T>) -> Self {
        let ptr = value.as_mut_ptr();
        let len = value.len();
        let capacity = value.capacity();
        std::mem::forget(value);
        Self {
            ptr,
            len,
            capacity,
            on_free: Self::on_free
        }
    }
}

impl<T> Deref for FfiVec<T> {
    type Target = [T];

    fn deref(&self) -> &Self::Target {
        unsafe {
            std::slice::from_raw_parts(self.ptr, self.len)
        }
    }
}

impl<T> Drop for FfiVec<T> {
    fn drop(&mut self) {
        unsafe {
            (self.on_free)(self)
        }
    }
}

unsafe impl<T: Send> Send for FfiVec<T> {}
unsafe impl<T: Send> Sync for FfiVec<T> {}