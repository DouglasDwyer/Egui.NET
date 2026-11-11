use crate::{
    ContainerAtom, FontSelection, Image, ImageSource, SizedAtomKind, Ui, WidgetAtom, WidgetText,
};
use alloc::sync::Arc;
use core::fmt::Debug;
use emath::{Rect, Vec2};
use epaint::Color32;
use epaint::text::TextWrapMode;

/// Args passed when sizing an [`super::Atom`]
#[cfg_attr(feature = "serde", derive(serde::Deserialize, serde::Serialize))]
pub struct IntoSizedArgs {
    pub available_size: Vec2,
    pub wrap_mode: TextWrapMode,
    pub fallback_font: FontSelection,
}

/// Result returned when sizing an [`super::Atom`]
#[cfg_attr(feature = "serde", derive(serde::Deserialize, serde::Serialize))]
pub struct IntoSizedResult {
    pub intrinsic_size: Vec2,
    pub sized: SizedAtomKind,
}

/// See [`AtomKind::Closure`]
pub type AtomClosure = Box<dyn FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'static>;

/// Args passed when painting an [`AtomKind::Paint`] atom.
#[derive(Clone, Copy, Debug)]
pub struct AtomPaintArgs {
    /// The rect the layout gave this atom.
    pub rect: Rect,

    /// The text color of the containing widget, e.g. the color the [`AtomKind::Text`] atoms
    /// next to this one are painted in.
    pub fallback_text_color: Color32,
}

/// See [`AtomKind::Paint`]
///
/// It is an [`Arc`] so the atom stays cloneable.
pub type AtomPaint = Arc<dyn Fn(&Ui, AtomPaintArgs) + 'static>;

/// The different kinds of [`crate::Atom`]s.
#[derive(Default)]
pub enum AtomKind {
    /// Empty, that can be used with [`crate::AtomExt::atom_grow`] to reserve space.
    #[default]
    Empty,

    /// Text atom.
    ///
    /// Truncation within [`crate::WidgetAtom`] works like this:
    /// - if `wrap_mode` is not Extend
    ///   - if no atom is `shrink`
    ///     - the first text atom is selected and will be marked as `shrink`
    ///   - the atom marked as `shrink` will shrink / wrap based on the selected wrap mode
    ///   - any other text atoms will have `wrap_mode` extend
    /// - if `wrap_mode` is extend, Text will extend as expected.
    ///
    /// Unless [`crate::AtomExt::atom_max_width`] is set, `wrap_mode` should only be set via [`crate::Style`] or
    /// [`crate::ContainerAtom::wrap_mode`], as setting a wrap mode on a [`WidgetText`] atom
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
    Closure(AtomClosure),

    /// A closure that paints the atom at the [`Rect`] the layout gives it.
    ///
    /// It has no size of its own, so set one with [`crate::AtomExt::atom_size`], or use
    /// [`crate::Atom::paint`], which does that for you. If the size depends on the [`Ui`],
    /// return a [`SizedAtomKind::Paint`] from an [`AtomKind::Closure`] instead.
    ///
    /// Use this for widgets that draw their own shapes, like the check mark of
    /// [`crate::Checkbox`].
    Paint(AtomPaint),

    /// A nested [`WidgetAtom`], letting you embed an atom-based widget as a single atom
    /// inside another [`WidgetAtom`].
    ///
    /// The nested widget is measured (sized) when the parent is sized, and painted (and
    /// interacted with) at the cell rect the parent computes for it.
    ///
    /// Use [`Self::Container`] instead if you don't need the nested layout to interact.
    Widget(Box<WidgetAtom>),

    /// A nested [`ContainerAtom`], letting you embed a non-interactive atom-based layout as a
    /// single atom inside another [`WidgetAtom`].
    ///
    /// Like [`Self::Widget`], the nested layout is measured when the parent is sized and painted
    /// at the cell rect the parent computes for it. Unlike [`Self::Widget`], a [`ContainerAtom`]
    /// has no [`Id`](crate::Id) or [`Sense`](crate::Sense), so it is never interacted with.
    Container(Box<ContainerAtom>),
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
            AtomKind::Paint(paint) => AtomKind::Paint(Arc::clone(paint)),
            AtomKind::Widget(layout) => AtomKind::Widget(layout.clone()),
            AtomKind::Container(container) => AtomKind::Container(container.clone()),
        }
    }
}

impl Debug for AtomKind {
    fn fmt(&self, f: &mut core::fmt::Formatter<'_>) -> core::fmt::Result {
        match self {
            AtomKind::Empty => write!(f, "AtomKind::Empty"),
            AtomKind::Text(text) => write!(f, "AtomKind::Text({text:?})"),
            AtomKind::Image(image) => write!(f, "AtomKind::Image({image:?})"),
            AtomKind::Closure(_) => write!(f, "AtomKind::Closure(<closure>)"),
            AtomKind::Paint(_) => write!(f, "AtomKind::Paint(<closure>)"),
            AtomKind::Widget(_) => write!(f, "AtomKind::Widget(<widget>)"),
            AtomKind::Container(_) => write!(f, "AtomKind::Container(<container>)"),
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
            AtomKind::Closure(_) | AtomKind::Paint(_) => {
                log::warn!("Cannot serialize atom closures");
                atom_kind_serde_helper::AtomKind::Empty.serialize(serializer)
            }
            AtomKind::Widget(_) | AtomKind::Container(_) => {
                // Nested layouts are self-referential, which the C# generator cannot represent.
                log::warn!("Cannot serialize a nested atom layout across the C# FFI boundary");
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
    /// serialized (see the `serde` impls above), so they are represented as [`Self::Empty`].
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

    /// See [`Self::Paint`]
    ///
    /// `func`'s lifetime is erased so that `AtomKind` stays lifetime-free for the C# bindings.
    pub fn paint<'c>(func: impl Fn(&Ui, AtomPaintArgs) + 'c) -> Self {
        let paint: Arc<dyn Fn(&Ui, AtomPaintArgs) + 'c> = Arc::new(func);
        // SAFETY: the erased lifetime does not outlive the widget call that paints this atom.
        let paint: AtomPaint = unsafe { core::mem::transmute(paint) };
        AtomKind::Paint(paint)
    }

    /// See [`Self::Closure`]
    ///
    /// `func`'s lifetime is erased so that `AtomKind` stays lifetime-free for the C# bindings.
    /// This is sound because a closure atom is created and resolved within one widget call.
    pub fn closure<'c>(func: impl FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'c) -> Self {
        let boxed: Box<dyn FnOnce(&Ui, IntoSizedArgs) -> IntoSizedResult + 'c> = Box::new(func);
        // SAFETY: the erased lifetime does not outlive the call that resolves this atom.
        let boxed: AtomClosure = unsafe { core::mem::transmute(boxed) };
        AtomKind::Closure(boxed)
    }

    /// Turn this [`AtomKind`] into a [`SizedAtomKind`].
    ///
    /// This converts [`WidgetText`] into [`crate::Galley`] and tries to load and size [`Image`].
    /// The first returned argument is the preferred size.
    pub fn into_sized(self, ui: &Ui, args: IntoSizedArgs) -> IntoSizedResult {
        // Destructured here so the parameter keeps a name that the C# bindgen can see.
        let IntoSizedArgs {
            available_size,
            wrap_mode,
            fallback_font,
        } = args;

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
            AtomKind::Paint(paint) => IntoSizedResult {
                intrinsic_size: Vec2::ZERO,
                sized: SizedAtomKind::Paint {
                    paint,
                    size: Vec2::ZERO,
                },
            },
            AtomKind::Widget(layout) => {
                let sized = layout.measure(ui, available_size);
                IntoSizedResult {
                    intrinsic_size: sized.intrinsic_size,
                    sized: SizedAtomKind::Widget(Box::new(sized)),
                }
            }
            AtomKind::Container(layout) => {
                let sized = layout.measure(ui, available_size);
                IntoSizedResult {
                    intrinsic_size: sized.intrinsic_size,
                    sized: SizedAtomKind::Container(Box::new(sized)),
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

impl From<WidgetAtom> for AtomKind {
    fn from(layout: WidgetAtom) -> Self {
        AtomKind::Widget(Box::new(layout))
    }
}

impl From<ContainerAtom> for AtomKind {
    fn from(container: ContainerAtom) -> Self {
        AtomKind::Container(Box::new(container))
    }
}
