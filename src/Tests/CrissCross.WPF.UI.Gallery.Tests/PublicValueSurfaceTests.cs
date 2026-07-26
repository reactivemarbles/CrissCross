// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media;
using CrissCross.WPF.UI.Appearance;
using CrissCross.WPF.UI.Configuration;
using CrissCross.WPF.UI.Hardware;
using CrissCross.WPF.UI.Interop.WinDef;
using ReactiveDisplayDpi = CrissCross.Reactive.WPF.UI.Hardware.DisplayDpi;
using ReactiveRect = CrissCross.Reactive.WPF.UI.Interop.WinDef.RECT;
using ReactiveRectLong = CrissCross.Reactive.WPF.UI.Interop.WinDef.RECTL;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises value objects shared by the standard and reactive Windows UI variants.</summary>
public sealed class PublicValueSurfaceTests
{
    /// <summary>The initial rectangle left coordinate.</summary>
    private const int InitialLeft = 1;

    /// <summary>The initial rectangle top coordinate.</summary>
    private const int InitialTop = 2;

    /// <summary>The initial rectangle right coordinate.</summary>
    private const int InitialRight = 11;

    /// <summary>The initial rectangle bottom coordinate.</summary>
    private const int InitialBottom = 22;

    /// <summary>The union rectangle left coordinate.</summary>
    private const int UnionLeft = -1;

    /// <summary>The union rectangle top coordinate.</summary>
    private const int UnionTop = -2;

    /// <summary>The union rectangle right coordinate.</summary>
    private const int UnionRight = 5;

    /// <summary>The union rectangle bottom coordinate.</summary>
    private const int UnionBottom = 6;

    /// <summary>The horizontal offset applied to rectangles.</summary>
    private const int HorizontalOffset = 3;

    /// <summary>The vertical offset applied to rectangles.</summary>
    private const int VerticalOffset = 4;

    /// <summary>The expected rectangle width.</summary>
    private const int ExpectedWidth = 10;

    /// <summary>The expected rectangle height.</summary>
    private const int ExpectedHeight = 20;

    /// <summary>The expected offset horizontal coordinate.</summary>
    private const int ExpectedOffsetX = 4;

    /// <summary>The expected offset vertical coordinate.</summary>
    private const int ExpectedOffsetY = 6;

    /// <summary>The scaled display DPI value.</summary>
    private const double ScaledDpi = 1.5D;

    /// <summary>The baseline display DPI value.</summary>
    private const double BaselineDpi = 2D;

    /// <summary>The absolute horizontal DPI value.</summary>
    private const int AbsoluteDpiX = 144;

    /// <summary>The absolute vertical DPI value.</summary>
    private const int AbsoluteDpiY = 192;

    /// <summary>The tracked property name.</summary>
    private const string PropertyName = "Name";

    /// <summary>The tracked property value.</summary>
    private const string PropertyValue = "value";

    /// <summary>The tracked property fallback value.</summary>
    private const string FallbackValue = "fallback";

    /// <summary>The rectangle comparison text.</summary>
    private const string RectangleText = "rectangle";

    /// <summary>The dialog title.</summary>
    private const string DialogTitle = "Title";

    /// <summary>The dialog body.</summary>
    private const string DialogBody = "Body";

    /// <summary>The dialog close-button text.</summary>
    private const string DialogCloseText = "Close";

    /// <summary>The dialog primary-button text.</summary>
    private const string DialogPrimaryText = "Save";

    /// <summary>The dialog secondary-button text.</summary>
    private const string DialogSecondaryText = "Cancel";

    /// <summary>Verifies rectangle projection, union, offset, equality, and hashing.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NativeRectangles_ProjectGeometryAndValueSemantics()
    {
        RECT rectangle = new() { Left = InitialLeft, Top = InitialTop, Right = InitialRight, Bottom = InitialBottom, };
        RECT union = RECT.Union(
            rectangle,
            new RECT { Left = UnionLeft, Top = UnionTop, Right = UnionRight, Bottom = UnionBottom, });
        rectangle.Offset(HorizontalOffset, VerticalOffset);

        await Assert.That(rectangle.Width).IsEqualTo(ExpectedWidth);
        await Assert.That(rectangle.Height).IsEqualTo(ExpectedHeight);
        await Assert.That(rectangle.Position.x).IsEqualTo(ExpectedOffsetX);
        await Assert.That(rectangle.Position.y).IsEqualTo(ExpectedOffsetY);
        await Assert.That(rectangle.Size.cx).IsEqualTo(ExpectedWidth);
        await Assert.That(rectangle.Size.cy).IsEqualTo(ExpectedHeight);
        await Assert.That(union.Left).IsEqualTo(UnionLeft);
        await Assert.That(union.Top).IsEqualTo(UnionTop);
        await Assert.That(union.Right).IsEqualTo(InitialRight);
        await Assert.That(union.Bottom).IsEqualTo(InitialBottom);
        await Assert.That(rectangle.Equals(rectangle)).IsTrue();
        await Assert.That(rectangle.Equals(default(RECT))).IsFalse();
        await Assert.That(rectangle.Equals(RectangleText)).IsFalse();
        await Assert.That(rectangle.GetHashCode()).IsNotEqualTo(0);
    }

    /// <summary>Verifies long-coordinate rectangle behavior in both namespace variants.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task LongAndReactiveRectangles_ProjectEquivalentGeometry()
    {
        RECTL rectangle = new() { Left = InitialLeft, Top = InitialTop, Right = InitialRight, Bottom = InitialBottom, };
        RECTL union = RECTL.Union(
            rectangle,
            new RECTL { Left = UnionLeft, Top = UnionTop, Right = UnionRight, Bottom = UnionBottom, });
        rectangle.Offset(HorizontalOffset, VerticalOffset);

        ReactiveRect reactive = new() { Left = InitialLeft, Top = InitialTop, Right = InitialRight, Bottom = InitialBottom, };
        ReactiveRect reactiveUnion = ReactiveRect.Union(
            reactive,
            new ReactiveRect { Left = UnionLeft, Top = UnionTop, Right = UnionRight, Bottom = UnionBottom, });
        reactive.Offset(HorizontalOffset, VerticalOffset);

        ReactiveRectLong reactiveLong = new() { Left = InitialLeft, Top = InitialTop, Right = InitialRight, Bottom = InitialBottom, };
        ReactiveRectLong reactiveLongUnion = ReactiveRectLong.Union(
            reactiveLong,
            new ReactiveRectLong { Left = UnionLeft, Top = UnionTop, Right = UnionRight, Bottom = UnionBottom, });
        reactiveLong.Offset(HorizontalOffset, VerticalOffset);

        await Assert.That(rectangle.Width).IsEqualTo(ExpectedWidth);
        await Assert.That(rectangle.Height).IsEqualTo(ExpectedHeight);
        await Assert.That(rectangle.Position.x).IsEqualTo(ExpectedOffsetX);
        await Assert.That(rectangle.Size.cy).IsEqualTo(ExpectedHeight);
        await Assert.That(union.Left).IsEqualTo(UnionLeft);
        await Assert.That(rectangle.Equals(rectangle)).IsTrue();
        object rectangleComparison = new();
        await Assert.That(rectangle.Equals(rectangleComparison)).IsFalse();
        await Assert.That(rectangle.GetHashCode()).IsNotEqualTo(0);

        await Assert.That(reactive.Width).IsEqualTo(ExpectedWidth);
        await Assert.That(reactive.Height).IsEqualTo(ExpectedHeight);
        await Assert.That(reactive.Position.x).IsEqualTo(ExpectedOffsetX);
        await Assert.That(reactive.Size.cy).IsEqualTo(ExpectedHeight);
        await Assert.That(reactiveUnion.Left).IsEqualTo(UnionLeft);
        await Assert.That(reactive.Equals(reactive)).IsTrue();
        object reactiveComparison = new();
        await Assert.That(reactive.Equals(reactiveComparison)).IsFalse();
        await Assert.That(reactive.GetHashCode()).IsNotEqualTo(0);

        await Assert.That(reactiveLong.Width).IsEqualTo(ExpectedWidth);
        await Assert.That(reactiveLong.Height).IsEqualTo(ExpectedHeight);
        await Assert.That(reactiveLong.Position.x).IsEqualTo(ExpectedOffsetX);
        await Assert.That(reactiveLong.Size.cy).IsEqualTo(ExpectedHeight);
        await Assert.That(reactiveLongUnion.Left).IsEqualTo(UnionLeft);
        await Assert.That(reactiveLong.Equals(reactiveLong)).IsTrue();
        object reactiveLongComparison = new();
        await Assert.That(reactiveLong.Equals(reactiveLongComparison)).IsFalse();
        await Assert.That(reactiveLong.GetHashCode()).IsNotEqualTo(0);
    }

    /// <summary>Verifies DPI constructors and immutable configuration records preserve their values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DpiAndConfigurationValues_PreserveProjectedState()
    {
        DisplayDpi scaled = new(ScaledDpi, BaselineDpi);
        DisplayDpi absolute = new(AbsoluteDpiX, AbsoluteDpiY);
        ReactiveDisplayDpi reactiveScaled = new(ScaledDpi, BaselineDpi);
        ReactiveDisplayDpi reactiveAbsolute = new(AbsoluteDpiX, AbsoluteDpiY);
        PropertyOperationData operation = new(PropertyName, PropertyValue) { Cancel = true, };
        TrackedPropertyInfo tracked = new(
            static target => target,
            static (_, _) => { },
            true,
            FallbackValue);
        SimpleContentDialogCreateOptions dialog = new(DialogTitle, DialogBody, DialogCloseText) { PrimaryButtonText = DialogPrimaryText, SecondaryButtonText = DialogSecondaryText, };
        ThemeChangedEventArgs theme = new(ApplicationTheme.Dark, Colors.CornflowerBlue);

        await Assert.That(scaled.DpiX).IsEqualTo(AbsoluteDpiX);
        await Assert.That(scaled.DpiY).IsEqualTo(AbsoluteDpiY);
        await Assert.That(absolute.DpiScaleX).IsEqualTo(ScaledDpi);
        await Assert.That(absolute.DpiScaleY).IsEqualTo(BaselineDpi);
        await Assert.That(reactiveScaled.DpiX).IsEqualTo(AbsoluteDpiX);
        await Assert.That(reactiveScaled.DpiY).IsEqualTo(AbsoluteDpiY);
        await Assert.That(reactiveAbsolute.DpiScaleX).IsEqualTo(ScaledDpi);
        await Assert.That(reactiveAbsolute.DpiScaleY).IsEqualTo(BaselineDpi);
        await Assert.That(operation.Cancel).IsTrue();
        await Assert.That(operation.Property).IsEqualTo(PropertyName);
        await Assert.That(tracked.IsDefaultSpecified).IsTrue();
        await Assert.That(tracked.DefaultValue).IsEqualTo(FallbackValue);
        await Assert.That(dialog.PrimaryButtonText).IsEqualTo(DialogPrimaryText);
        await Assert.That(dialog.SecondaryButtonText).IsEqualTo(DialogSecondaryText);
        await Assert.That(theme.CurrentApplicationTheme).IsEqualTo(ApplicationTheme.Dark);
        await Assert.That(theme.SystemAccent).IsEqualTo(Colors.CornflowerBlue);
    }
}
