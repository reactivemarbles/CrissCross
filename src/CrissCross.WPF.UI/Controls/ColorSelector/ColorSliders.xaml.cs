// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// ColorSliders.
/// </summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class ColorSliders : PickerControlBase
{
    /// <summary>
    /// The small change property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(ColorSliders),
            new PropertyMetadata(1.0));

    /// <summary>
    /// The show alpha property.
    /// </summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(ColorSliders),
            new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorSliders"/> class.
    /// </summary>
    public ColorSliders() => InitializeComponent();

    /// <summary>
    /// Gets or sets the small change.
    /// </summary>
    /// <value>
    /// The small change.
    /// </value>
    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show alpha].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show alpha]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }
}
