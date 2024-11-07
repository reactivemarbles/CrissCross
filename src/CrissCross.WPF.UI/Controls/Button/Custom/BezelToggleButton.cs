// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// GenericToggleButton.
/// </summary>
/// <seealso cref="CommonToggleButtonBase" />
public class BezelToggleButton : CommonToggleButtonBase
{
    /// <summary>
    /// The glare opacity mask property.
    /// </summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register("GlareOpacityMask", typeof(Brush), typeof(BezelToggleButton), new PropertyMetadata(null));

    /// <summary>
    /// The minor background1 property.
    /// </summary>
    public static readonly DependencyProperty MinorBackground1Property = DependencyProperty.Register("MinorBackground1", typeof(Brush), typeof(BezelToggleButton), new PropertyMetadata(null));

    /// <summary>
    /// The pressed brush property.
    /// </summary>
    public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register("PressedBrush", typeof(Brush), typeof(BezelToggleButton), new PropertyMetadata(Brushes.Green));

    /// <summary>
    /// Initializes a new instance of the <see cref="BezelToggleButton"/> class.
    /// </summary>
    public BezelToggleButton()
        : base("CrissCross.WPF.UI.Controls.BezelToggleButton")
    {
    }

    /// <summary>
    /// Gets or sets the glare opacity mask.
    /// </summary>
    /// <value>
    /// The glare opacity mask.
    /// </value>
    public Brush GlareOpacityMask
    {
        get => (Brush)GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>
    /// Gets or sets the minor background1.
    /// </summary>
    /// <value>
    /// The minor background1.
    /// </value>
    public Brush MinorBackground1
    {
        get => (Brush)GetValue(MinorBackground1Property);
        set => SetValue(MinorBackground1Property, value);
    }

    /// <summary>
    /// Gets or sets the pressed brush.
    /// </summary>
    /// <value>
    /// The pressed brush.
    /// </value>
    public Brush PressedBrush
    {
        get => (Brush)GetValue(PressedBrushProperty);
        set => SetValue(PressedBrushProperty, value);
    }
}
