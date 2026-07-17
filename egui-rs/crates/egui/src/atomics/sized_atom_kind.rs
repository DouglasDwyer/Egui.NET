use crate::{Image, SizedAtomLayout};
use emath::Vec2;
use epaint::Galley;
use std::sync::Arc;

/// A sized [`crate::AtomKind`].
#[derive(Clone, Debug)]
#[cfg_attr(feature = "serde", derive(serde::Deserialize, serde::Serialize))]
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
