// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended <see cref="System.Windows.Controls.Primitives.ToggleButton"/>.
/// </summary>
/// <seealso cref="System.Windows.Controls.Primitives.ToggleButton" />
public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
{
    /// <summary>
    /// The TreeView item chevron size property.
    /// </summary>
    public static readonly DependencyProperty ChevronSizeProperty =
        DependencyProperty.Register(
            nameof(ChevronSize),
            typeof(double),
            typeof(ToggleButton),
            new PropertyMetadata(10d));

    /// <summary>
    /// Gets or sets the size of the TreeView item chevron.
    /// </summary>
    /// <value>
    /// The size of the TreeView item chevron.
    /// </value>
    public double ChevronSize
    {
        get => (double)GetValue(ChevronSizeProperty);
        set => SetValue(ChevronSizeProperty, value);
    }
}
