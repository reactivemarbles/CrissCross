// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Represents HexColorTextBox.</summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class HexColorTextBox : PickerControlBase
{
    /// <summary>The show alpha property.</summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(HexColorTextBox),
            new PropertyMetadata(true));

    /// <summary>Initializes a new instance of the <see cref="HexColorTextBox"/> class.</summary>
    public HexColorTextBox() => InitializeComponent();

    /// <summary>Gets or sets a value indicating whether [show alpha].</summary>
    /// <value>
    ///   <c>true</c> if [show alpha]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>Provides the ColorToHexConverter_OnShowAlphaChange member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void ColorToHexConverter_OnShowAlphaChange(object sender, EventArgs e) =>
        textbox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateTarget();
}
