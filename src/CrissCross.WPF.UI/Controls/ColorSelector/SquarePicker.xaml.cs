// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// SquarePicker.
/// </summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class SquarePicker : PickerControlBase
{
    /// <summary>
    /// The small change property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(SquarePicker),
            new PropertyMetadata(1.0));

    /// <summary>
    /// The picker type property.
    /// </summary>
    public static readonly DependencyProperty PickerTypeProperty =
        DependencyProperty.Register(
            nameof(PickerType),
            typeof(PickerType),
            typeof(SquarePicker),
            new PropertyMetadata(PickerType.HSV));

    /// <summary>
    /// Initializes a new instance of the <see cref="SquarePicker"/> class.
    /// </summary>
    public SquarePicker() => InitializeComponent();

    /// <summary>
    /// Gets or sets the type of the picker.
    /// </summary>
    /// <value>
    /// The type of the picker.
    /// </value>
    public PickerType PickerType
    {
        get => (PickerType)GetValue(PickerTypeProperty);
        set => SetValue(PickerTypeProperty, value);
    }

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
}
