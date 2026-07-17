// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents an image with additional properties for Borders and Rounded corners.</summary>
public class Image : Control
{
    /// <summary>Gets/Sets the Source on this Image.</summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(ImageSource),
        typeof(Image),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            null),
        null);

    /// <summary>DependencyProperty for CornerRadius property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(Image),
        new PropertyMetadata(new CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged)));

    /// <summary>DependencyProperty for StretchDirection property.</summary>
    /// <seealso cref="Viewbox.Stretch" />
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        nameof(Stretch),
        typeof(Stretch),
        typeof(Image),
        new FrameworkPropertyMetadata(
            Stretch.Uniform,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        null);

    /// <summary>DependencyProperty for Stretch property.</summary>
    public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(
        nameof(StretchDirection),
        typeof(StretchDirection),
        typeof(Image),
        new FrameworkPropertyMetadata(
            StretchDirection.Both,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        null);

    /// <summary>DependencyPropertyKey for InnerCornerRadius property.</summary>
    public static readonly DependencyPropertyKey InnerCornerRadiusPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(InnerCornerRadius),
        typeof(CornerRadius),
        typeof(Image),
        new PropertyMetadata(new CornerRadius(0)));

    /// <summary>DependencyProperty for InnerCornerRadius property.</summary>
    public static readonly DependencyProperty InnerCornerRadiusProperty =
        InnerCornerRadiusPropertyKey.DependencyProperty;

    /// <summary>Divisor used to offset corner radius by half of the border thickness.</summary>
    private const double CornerRadiusThicknessDivisor = 2.0;

    /// <summary>Gets or sets the Source on this Image.</summary>
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>Gets or sets the Stretch on this Image.</summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Gets or sets the stretch direction of the Viewbox, which determines the restrictions on
    /// scaling that are applied to the content inside the Viewbox.  For instance, this property
    /// can be used to prevent the content from being smaller than its native size or larger than
    /// its native size.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => (StretchDirection)GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the CornerRadius property allows users to control the roundness of the corners independently by
    /// setting a radius value for each corner.  Radius values that are too large are scaled so that they
    /// smoothly blend from corner to corner.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Gets the CornerRadius for the inner image's Mask.</summary>
    internal CornerRadius InnerCornerRadius => (CornerRadius)GetValue(InnerCornerRadiusProperty);

    /// <summary>Provides the OnCornerRadiusChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var thickness = (Thickness)d.GetValue(BorderThicknessProperty);
        var outerRarius = (CornerRadius)e.NewValue;

        d.SetValue(
            InnerCornerRadiusPropertyKey,
            new CornerRadius(
                topLeft: Math.Max(
                    0,
                    (int)Math.Round(outerRarius.TopLeft - (thickness.Left / CornerRadiusThicknessDivisor), 0)),
                topRight: Math.Max(
                    0,
                    (int)Math.Round(outerRarius.TopRight - (thickness.Top / CornerRadiusThicknessDivisor), 0)),
                bottomRight: Math.Max(
                    0,
                    (int)Math.Round(outerRarius.BottomRight - (thickness.Right / CornerRadiusThicknessDivisor), 0)),
                bottomLeft: Math.Max(
                    0,
                    (int)Math.Round(outerRarius.BottomLeft - (thickness.Bottom / CornerRadiusThicknessDivisor), 0))));
    }
}
