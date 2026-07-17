use crate::{Id, Image};
use emath::Vec2;
use epaint::Galley;
use std::sync::Arc;

/// A sized [`crate::AtomKind`].
#[derive(Clone, Default, Debug)]
#[cfg_attr(feature = "serde", derive(serde::Deserialize, serde::Serialize))]
pub enum SizedAtomKind {
    #[default]
    Empty,
    Text(Arc<Galley>),
    Image(Image, Vec2),
    Custom(Id),
}

impl SizedAtomKind {
    /// Get the calculated size.
    pub fn size(&self) -> Vec2 {
        match self {
            SizedAtomKind::Text(galley) => galley.size(),
            SizedAtomKind::Image(_, size) => *size,
            SizedAtomKind::Empty | SizedAtomKind::Custom(_) => Vec2::ZERO,
        }
    }
}
