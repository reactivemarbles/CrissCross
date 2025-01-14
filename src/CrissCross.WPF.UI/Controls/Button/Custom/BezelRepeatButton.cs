// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// GenericRepeatButton.
/// </summary>
public class BezelRepeatButton : CommonRepeatButtonBase
{
    /// <summary>
    /// The glare opacity mask property.
    /// </summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register("GlareOpacityMask", typeof(Brush), typeof(BezelRepeatButton), new PropertyMetadata(null));

    /// <summary>
    /// The pressed brush property.
    /// </summary>
    public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.Register("PressedBrush", typeof(Brush), typeof(BezelRepeatButton), new PropertyMetadata(Brushes.Green));

    /// <summary>
    /// Initializes a new instance of the <see cref="BezelRepeatButton"/> class.
    /// </summary>
    public BezelRepeatButton()
        : base("CrissCross.WPF.UI.Controls.BezelRepeatButton")
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
