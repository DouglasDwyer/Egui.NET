use crate::SizedAtomKind;
use emath::Vec2;

/// A [`crate::Atom`] which has been sized.
#[derive(Clone, Debug)]
#[cfg_attr(feature = "serde", derive(serde::Deserialize, serde::Serialize))]
pub struct SizedAtom {
    pub id: Option<crate::Id>,

    pub(crate) grow: bool,

    /// The size of the atom.
    ///
    /// Used for placing this atom in [`crate::AtomLayout`], the cursor will advance by
    /// size.x + gap.
    pub size: Vec2,

    /// Intrinsic size of the atom. This is used to calculate `Response::intrinsic_size`.
    pub intrinsic_size: Vec2,

    /// How will the atom be aligned in its available space?
    pub align: emath::Align2,

    pub kind: SizedAtomKind,
}

impl SizedAtom {
    /// Was this [`crate::Atom`] marked as `grow`?
    pub fn is_grow(&self) -> bool {
        self.grow
    }
}
