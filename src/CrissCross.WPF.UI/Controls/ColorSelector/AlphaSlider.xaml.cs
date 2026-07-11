// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Represents AlphaSlider.</summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class AlphaSlider : PickerControlBase
{
    /// <summary>The small change property.</summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(AlphaSlider),
            new PropertyMetadata(1.0));

    /// <summary>Initializes a new instance of the <see cref="AlphaSlider"/> class.</summary>
    public AlphaSlider() => InitializeComponent();

    /// <summary>Gets or sets the small change.</summary>
    /// <value>
    /// The small change.
    /// </value>
    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }
}
