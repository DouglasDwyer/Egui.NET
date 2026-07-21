use crate::{Image, SizedAtomLayout};
use emath::Vec2;
use epaint::Galley;
use std::sync::Arc;

/// A sized [`crate::AtomKind`].
#[derive(Clone, Debug)]
pub enum SizedAtomKind {
    Empty { size: Option<Vec2> },
    Text(Arc<Galley>),
    Image { image: Image, size: Vec2 },
    Layout(Box<SizedAtomLayout>),
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
            SizedAtomKind::Image { image: _, size } => *size,
            SizedAtomKind::Empty { size } => size.unwrap_or_default(),
            SizedAtomKind::Layout(layout) => layout.outer_size,
        }
    }
}

// `SizedAtomLayout` embeds `SizedAtom`, which embeds `SizedAtomKind` again. Serde's reflection
// (used to generate the C# bindings) cannot see through `Box`, so tracing `Layout` directly
// produces a self-referential value type on the C# side that the CLR cannot load. Until the C#
// code generator can detect such cycles and fall back to a reference type, nested layouts cannot
// cross the C# FFI boundary, so (mirroring `AtomKind::Layout`) they are serialized as
// `SizedAtomKind::Empty` instead.
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
            SizedAtomKind::Layout(_) => {
                log::warn!(
                    "Cannot serialize a nested SizedAtomKind::Layout across the C# FFI boundary"
                );
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

    /// The data to serialize for a [`super::SizedAtomKind`]. Nested layouts cannot be serialized
    /// (see [`super::SizedAtomKind::Layout`]), so they are represented as [`Self::Empty`].
    #[derive(serde::Deserialize, serde::Serialize)]
    pub enum SizedAtomKind {
        Empty { size: Option<Vec2> },
        Text(Arc<Galley>),
        Image { image: Image, size: Vec2 },
    }
}
