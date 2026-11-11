use crate::{AtomPaint, Image, SizedContainerAtom, SizedWidgetAtom};
use alloc::sync::Arc;
use core::fmt::Debug;
use emath::Vec2;
use epaint::Galley;

/// A sized [`crate::AtomKind`].
#[derive(Clone)]
pub enum SizedAtomKind {
    Empty { size: Option<Vec2> },
    Text(Arc<Galley>),
    Image { image: Image, size: Vec2 },
    Paint { paint: AtomPaint, size: Vec2 },
    Container(Box<SizedContainerAtom>),
    Widget(Box<SizedWidgetAtom>),
}

impl Debug for SizedAtomKind {
    fn fmt(&self, f: &mut core::fmt::Formatter<'_>) -> core::fmt::Result {
        match self {
            SizedAtomKind::Empty { size } => write!(f, "SizedAtomKind::Empty({size:?})"),
            SizedAtomKind::Text(galley) => write!(f, "SizedAtomKind::Text({galley:?})"),
            SizedAtomKind::Image { image, size } => {
                write!(f, "SizedAtomKind::Image({image:?}, {size:?})")
            }
            SizedAtomKind::Paint { size, .. } => {
                write!(f, "SizedAtomKind::Paint(<closure>, {size:?})")
            }
            SizedAtomKind::Widget(layout) => write!(f, "SizedAtomKind::Widget({layout:?})"),
            SizedAtomKind::Container(container) => {
                write!(f, "SizedAtomKind::Container({container:?})")
            }
        }
    }
}

impl Default for SizedAtomKind {
    fn default() -> Self {
        Self::Empty { size: None }
    }
}

impl SizedAtomKind {
    /// Get the calculated size.
    pub fn size(&self) -> Vec2 {
        match self {
            SizedAtomKind::Text(galley) => galley.size(),
            SizedAtomKind::Image { size, .. } | SizedAtomKind::Paint { size, .. } => *size,
            SizedAtomKind::Empty { size } => size.unwrap_or_default(),
            SizedAtomKind::Widget(layout) => layout.outer_size,
            SizedAtomKind::Container(container) => container.outer_size,
        }
    }
}

// Nested layouts and paint closures cannot cross the C# FFI boundary, so they serialize as empty.
#[cfg(feature = "serde")]
impl serde::Serialize for SizedAtomKind {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: serde::Serializer,
    {
        match self {
            SizedAtomKind::Empty { size } => {
                sized_atom_kind_serde_helper::SizedAtomKind::Empty { size: *size }
                    .serialize(serializer)
            }
            SizedAtomKind::Text(galley) => {
                sized_atom_kind_serde_helper::SizedAtomKind::Text(galley.clone())
                    .serialize(serializer)
            }
            SizedAtomKind::Image { image, size } => {
                sized_atom_kind_serde_helper::SizedAtomKind::Image {
                    image: image.clone(),
                    size: *size,
                }
                .serialize(serializer)
            }
            SizedAtomKind::Paint { size, .. } => {
                log::warn!("Cannot serialize atom paint closures");
                sized_atom_kind_serde_helper::SizedAtomKind::Empty { size: Some(*size) }
                    .serialize(serializer)
            }
            SizedAtomKind::Widget(_) | SizedAtomKind::Container(_) => {
                log::warn!("Cannot serialize a nested sized atom layout across the C# FFI boundary");
                sized_atom_kind_serde_helper::SizedAtomKind::Empty { size: None }
                    .serialize(serializer)
            }
        }
    }
}

#[cfg(feature = "serde")]
impl<'de> serde::Deserialize<'de> for SizedAtomKind {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: serde::Deserializer<'de>,
    {
        Ok(
            match sized_atom_kind_serde_helper::SizedAtomKind::deserialize(deserializer)? {
                sized_atom_kind_serde_helper::SizedAtomKind::Empty { size } => {
                    SizedAtomKind::Empty { size }
                }
                sized_atom_kind_serde_helper::SizedAtomKind::Text(galley) => {
                    SizedAtomKind::Text(galley)
                }
                sized_atom_kind_serde_helper::SizedAtomKind::Image { image, size } => {
                    SizedAtomKind::Image { image, size }
                }
            },
        )
    }
}

#[cfg(feature = "serde")]
mod sized_atom_kind_serde_helper {
    use super::*;

    /// The data to serialize for a [`super::SizedAtomKind`]. Paint closures and nested layouts
    /// cannot be serialized, so they are represented as [`Self::Empty`].
    #[derive(serde::Deserialize, serde::Serialize)]
    pub enum SizedAtomKind {
        Empty { size: Option<Vec2> },
        Text(
            #[serde(
                serialize_with = "Galley::ffi_serialize_arc_handle",
                deserialize_with = "Galley::ffi_deserialize_arc_handle"
            )]
            Arc<Galley>,
        ),
        Image { image: Image, size: Vec2 },
    }
}
