// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents GenericToggleButton.</summary>
/// <seealso cref="CommonToggleButtonBase" />
public class BezelToggleButton : CommonToggleButtonBase
{
    /// <summary>The glare opacity mask property.</summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register(
        nameof(GlareOpacityMask),
        typeof(Brush),
        typeof(BezelToggleButton),
        new PropertyMetadata(null));

    /// <summary>The minor background1 property.</summary>
    public static readonly DependencyProperty MinorBackground1Property = DependencyProperty.Register(
        nameof(MinorBackground1),
        typeof(Brush),
        typeof(BezelToggleButton),
        new PropertyMetadata(null));

    /// <summary>The pressed brush property.</summary>
    public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register(
        nameof(PressedBrush),
        typeof(Brush),
        typeof(BezelToggleButton),
        new PropertyMetadata(Brushes.Green));

    /// <summary>Initializes a new instance of the <see cref="BezelToggleButton"/> class.</summary>
    public BezelToggleButton()
        : base(typeof(BezelToggleButton).FullName!) { }

    /// <summary>Gets or sets the glare opacity mask.</summary>
    /// <value>
    /// The glare opacity mask.
    /// </value>
    public Brush GlareOpacityMask
    {
        get => (Brush)GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>Gets or sets the minor background1.</summary>
    /// <value>
    /// The minor background1.
    /// </value>
    public Brush MinorBackground1
    {
        get => (Brush)GetValue(MinorBackground1Property);
        set => SetValue(MinorBackground1Property, value);
    }

    /// <summary>Gets or sets the pressed brush.</summary>
    /// <value>
    /// The pressed brush.
    /// </value>
    public Brush PressedBrush
    {
        get => (Brush)GetValue(PressedBrushProperty);
        set => SetValue(PressedBrushProperty, value);
    }
}
