use crate::{AtomLayout, FontSelection, Image, ImageSource, SizedAtomKind, Ui, WidgetText};
use emath::Vec2;
use epaint::text::TextWrapMode;
use std::fmt::Debug;

/// Args passed when sizing an [`super::Atom`]
pub struct IntoSizedArgs {
    pub available_size: Vec2,
    pub wrap_mode: TextWrapMode,
    pub fallback_font: FontSelection,
}

/// Result returned when sizing an [`super::Atom`]
pub struct IntoSizedResult {
    pub intrinsic_size: Vec2,
    pub sized: SizedAtomKind,
}

/// See [`AtomKind::Closure`]
pub type AtomClosure = Box<dyn FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'static>;

/// The different kinds of [`crate::Atom`]s.
#[derive(Default)]
pub enum AtomKind {
    /// Empty, that can be used with [`crate::AtomExt::atom_grow`] to reserve space.
    #[default]
    Empty,

    /// Text atom.
    ///
    /// Truncation within [`crate::AtomLayout`] works like this:
    /// -
    /// - if `wrap_mode` is not Extend
    ///   - if no atom is `shrink`
    ///     - the first text atom is selected and will be marked as `shrink`
    ///   - the atom marked as `shrink` will shrink / wrap based on the selected wrap mode
    ///   - any other text atoms will have `wrap_mode` extend
    /// - if `wrap_mode` is extend, Text will extend as expected.
    ///
    /// Unless [`crate::AtomExt::atom_max_width`] is set, `wrap_mode` should only be set via [`crate::Style`] or
    /// [`crate::AtomLayout::wrap_mode`], as setting a wrap mode on a [`WidgetText`] atom
    /// that is not `shrink` will have unexpected results.
    ///
    /// The size is determined by converting the [`WidgetText`] into a galley and using the galleys
    /// size. You can use [`crate::AtomExt::atom_size`] to override this, and [`crate::AtomExt::atom_max_width`]
    /// to limit the width (Causing the text to wrap or truncate, depending on the `wrap_mode`.
    /// [`crate::AtomExt::atom_max_height`] has no effect on text.
    Text(WidgetText),

    /// Image atom.
    ///
    /// By default the size is determined via [`Image::calc_size`].
    /// You can use [`crate::AtomExt::atom_max_size`] or [`crate::AtomExt::atom_size`] to customize the size.
    /// There is also a helper [`crate::AtomExt::atom_max_height_font_size`] to set the max height to the
    /// default font height, which is convenient for icons.
    Image(Image),

    /// A custom closure that produces a sized atom.
    ///
    /// The vec2 passed in is the available size to this atom. The returned vec2 should be the
    /// preferred / intrinsic size.
    ///
    /// Note: This api is experimental, expect breaking changes here.
    /// When cloning, this will be cloned as [`AtomKind::Empty`].
    /// Closures cannot cross the C# FFI boundary, so they are serialized as [`AtomKind::Empty`] too.
    Closure(AtomClosure),

    /// A nested [`AtomLayout`], letting you embed an atom-based widget as a single atom
    /// inside another [`AtomLayout`].
    ///
    /// The nested layout is measured (sized) when the parent is sized, and painted (and
    /// interacted with) at the cell rect the parent computes for it.
    Layout(Box<AtomLayout>),
}

impl Clone for AtomKind {
    fn clone(&self) -> Self {
        match self {
            AtomKind::Empty => AtomKind::Empty,
            AtomKind::Text(text) => AtomKind::Text(text.clone()),
            AtomKind::Image(image) => AtomKind::Image(image.clone()),
            AtomKind::Closure(_) => {
                log::warn!("Cannot clone atom closures");
                AtomKind::Empty
            }
            AtomKind::Layout(layout) => AtomKind::Layout(layout.clone()),
        }
    }
}

impl Debug for AtomKind {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            AtomKind::Empty => write!(f, "AtomKind::Empty"),
            AtomKind::Text(text) => write!(f, "AtomKind::Text({text:?})"),
            AtomKind::Image(image) => write!(f, "AtomKind::Image({image:?})"),
            AtomKind::Closure(_) => write!(f, "AtomKind::Closure(<closure>)"),
            AtomKind::Layout(_) => write!(f, "AtomKind::Layout(<layout>)"),
        }
    }
}

#[cfg(feature = "serde")]
impl serde::Serialize for AtomKind {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: serde::Serializer,
    {
        match self {
            AtomKind::Empty => atom_kind_serde_helper::AtomKind::Empty.serialize(serializer),
            AtomKind::Text(text) => {
                atom_kind_serde_helper::AtomKind::Text(text.clone()).serialize(serializer)
            }
            AtomKind::Image(image) => {
                atom_kind_serde_helper::AtomKind::Image(image.clone()).serialize(serializer)
            }
            AtomKind::Closure(_) => {
                log::warn!("Cannot serialize atom closures");
                atom_kind_serde_helper::AtomKind::Empty.serialize(serializer)
            }
            AtomKind::Layout(_) => {
                // `AtomLayout` embeds `Atoms`, which embeds `AtomKind` again. Serde's reflection
                // (used to generate the C# bindings) cannot see through `Box`, so tracing this
                // variant produces a self-referential value type on the C# side that the CLR
                // cannot load (it would need infinite size). Until the C# code generator can
                // detect such cycles and fall back to a reference type, nested layouts cannot
                // cross the C# FFI boundary, so they are serialized as `AtomKind::Empty` too.
                log::warn!("Cannot serialize a nested AtomKind::Layout across the C# FFI boundary");
                atom_kind_serde_helper::AtomKind::Empty.serialize(serializer)
            }
        }
    }
}

#[cfg(feature = "serde")]
impl<'de> serde::Deserialize<'de> for AtomKind {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: serde::Deserializer<'de>,
    {
        Ok(
            match atom_kind_serde_helper::AtomKind::deserialize(deserializer)? {
                atom_kind_serde_helper::AtomKind::Empty => AtomKind::Empty,
                atom_kind_serde_helper::AtomKind::Text(text) => AtomKind::Text(text),
                atom_kind_serde_helper::AtomKind::Image(image) => AtomKind::Image(image),
            },
        )
    }
}

#[cfg(feature = "serde")]
mod atom_kind_serde_helper {
    use super::*;

    /// The data to serialize for an [`super::AtomKind`]. Closures and nested layouts cannot be
    /// serialized (see [`super::AtomKind::Layout`]), so they are represented as [`Self::Empty`].
    #[derive(serde::Deserialize, serde::Serialize)]
    pub enum AtomKind {
        Empty,
        Text(WidgetText),
        Image(Image),
    }
}

impl AtomKind {
    /// See [`Self::Text`]
    pub fn text(text: impl Into<WidgetText>) -> Self {
        AtomKind::Text(text.into())
    }

    /// See [`Self::Image`]
    pub fn image(image: impl Into<Image>) -> Self {
        AtomKind::Image(image.into())
    }

    /// See [`Self::Closure`]
    ///
    /// `func` need not be `'static`: `AtomKind` has to be lifetime-free so it can cross the
    /// C# FFI boundary, so the closure's lifetime is erased here. This is sound because a
    /// `Closure` atom is only ever created and resolved (via [`Self::into_sized`]) synchronously
    /// within a single widget call, and is never itself serialized (see the `serde` impls above) or
    /// stored past that call.
    pub fn closure<'c>(func: impl FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'c) -> Self {
        let boxed: Box<dyn FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'c> = Box::new(func);
        // SAFETY: see the doc comment above; the erased lifetime never outlives the call that
        // resolves this `Closure` atom back into a `SizedAtomKind`.
        let boxed: AtomClosure = unsafe { std::mem::transmute(boxed) };
        AtomKind::Closure(boxed)
    }

    /// Turn this [`AtomKind`] into a [`SizedAtomKind`].
    ///
    /// This converts [`WidgetText`] into [`crate::Galley`] and tries to load and size [`Image`].
    /// The first returned argument is the preferred size.
    pub fn into_sized(
        self,
        ui: &Ui,
        IntoSizedArgs {
            available_size,
            wrap_mode,
            fallback_font,
        }: IntoSizedArgs,
    ) -> IntoSizedResult {
        match self {
            AtomKind::Text(text) => {
                let galley = text.into_galley(ui, Some(wrap_mode), available_size.x, fallback_font);
                IntoSizedResult {
                    intrinsic_size: galley.intrinsic_size(),
                    sized: SizedAtomKind::Text(galley),
                }
            }
            AtomKind::Image(image) => {
                let size = image.load_and_calc_size(ui, available_size);
                let size = size.unwrap_or(Vec2::ZERO);
                IntoSizedResult {
                    intrinsic_size: size,
                    sized: SizedAtomKind::Image { image, size },
                }
            }
            AtomKind::Empty => IntoSizedResult {
                intrinsic_size: Vec2::ZERO,
                sized: SizedAtomKind::Empty { size: None },
            },
            AtomKind::Closure(func) => func(
                ui,
                IntoSizedArgs {
                    available_size,
                    wrap_mode,
                    fallback_font,
                },
            ),
            AtomKind::Layout(layout) => {
                let sized = layout.measure(ui, available_size);
                IntoSizedResult {
                    intrinsic_size: sized.intrinsic_size,
                    sized: SizedAtomKind::Layout(Box::new(sized)),
                }
            }
        }
    }
}

impl From<ImageSource> for AtomKind {
    fn from(value: ImageSource) -> Self {
        AtomKind::Image(value.into())
    }
}

impl From<Image> for AtomKind {
    fn from(value: Image) -> Self {
        AtomKind::Image(value)
    }
}

impl<T> From<T> for AtomKind
where
    T: Into<WidgetText>,
{
    fn from(value: T) -> Self {
        AtomKind::Text(value.into())
    }
}

impl From<AtomLayout> for AtomKind {
    fn from(layout: AtomLayout) -> Self {
        AtomKind::Layout(Box::new(layout))
    }
}
