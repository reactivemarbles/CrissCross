// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross.WPF.UI;

/// <summary>
/// ColorDisplay.
/// </summary>
/// <seealso cref="DualPickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class ColorDisplay : DualPickerControlBase
{
    /// <summary>
    /// The corner radius property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(double),
            typeof(ColorDisplay),
            new PropertyMetadata(0d));

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorDisplay"/> class.
    /// </summary>
    public ColorDisplay() => InitializeComponent();

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>
    /// The corner radius.
    /// </value>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private void SwapButton_Click(object sender, RoutedEventArgs e) => SwapColors();

    private void HintColor_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => SetMainColorFromHintColor();
}
