// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// PortableColorPicker.
/// </summary>
/// <seealso cref="DualPickerControlBase" />
public partial class PortableColorPicker : DualPickerControlBase
{
    /// <summary>
    /// The small change property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(PortableColorPicker),
            new PropertyMetadata(1.0));

    /// <summary>
    /// The show alpha property.
    /// </summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(PortableColorPicker),
            new PropertyMetadata(true));

    /// <summary>
    /// The picker type property.
    /// </summary>
    public static readonly DependencyProperty PickerTypeProperty
        = DependencyProperty.Register(
            nameof(PickerType),
            typeof(PickerType),
            typeof(PortableColorPicker),
            new PropertyMetadata(PickerType.HSV));

    /// <summary>
    /// Initializes a new instance of the <see cref="PortableColorPicker"/> class.
    /// </summary>
    public PortableColorPicker() => InitializeComponent();

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
}
