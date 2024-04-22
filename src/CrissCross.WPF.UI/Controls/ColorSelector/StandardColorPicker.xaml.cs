// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// StandardColorPicker.
/// </summary>
/// <seealso cref="DualPickerControlBase" />
public partial class StandardColorPicker : DualPickerControlBase
{
    /// <summary>
    /// The small change property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(StandardColorPicker),
            new PropertyMetadata(1.0));

    /// <summary>
    /// The show alpha property.
    /// </summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(StandardColorPicker),
            new PropertyMetadata(true));

    /// <summary>
    /// The show hexadecimal property.
    /// </summary>
    public static readonly DependencyProperty ShowHexProperty =
        DependencyProperty.Register(
            nameof(ShowHex),
            typeof(Visibility),
            typeof(StandardColorPicker),
            new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The show color swap property.
    /// </summary>
    public static readonly DependencyProperty ShowColorSwapProperty =
        DependencyProperty.Register(
            nameof(ShowColorSwap),
            typeof(Visibility),
            typeof(StandardColorPicker),
            new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The show sliders property.
    /// </summary>
    public static readonly DependencyProperty ShowSlidersProperty =
        DependencyProperty.Register(
            nameof(ShowSliders),
            typeof(Visibility),
            typeof(StandardColorPicker),
            new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The show picker type property.
    /// </summary>
    public static readonly DependencyProperty ShowPickerTypeProperty =
        DependencyProperty.Register(
            nameof(ShowPickerType),
            typeof(Visibility),
            typeof(StandardColorPicker),
            new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// The picker type property.
    /// </summary>
    public static readonly DependencyProperty PickerTypeProperty
        = DependencyProperty.Register(
            nameof(PickerType),
            typeof(PickerType),
            typeof(StandardColorPicker),
            new PropertyMetadata(PickerType.HSV));

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardColorPicker"/> class.
    /// </summary>
    public StandardColorPicker() => InitializeComponent();

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
    /// Gets or sets the show hexadecimal.
    /// </summary>
    /// <value>
    /// The show hexadecimal.
    /// </value>
    public Visibility ShowHex
    {
        get => (Visibility)GetValue(ShowHexProperty);
        set => SetValue(ShowHexProperty, value);
    }

    /// <summary>
    /// Gets or sets the show sliders.
    /// </summary>
    /// <value>
    /// The show sliders.
    /// </value>
    public Visibility ShowSliders
    {
        get => (Visibility)GetValue(ShowSlidersProperty);
        set => SetValue(ShowSlidersProperty, value);
    }

    /// <summary>
    /// Gets or sets the show color swap.
    /// </summary>
    /// <value>
    /// The show color swap.
    /// </value>
    public Visibility ShowColorSwap
    {
        get => (Visibility)GetValue(ShowColorSwapProperty);
        set => SetValue(ShowColorSwapProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of the show picker.
    /// </summary>
    /// <value>
    /// The type of the show picker.
    /// </value>
    public Visibility ShowPickerType
    {
        get => (Visibility)GetValue(ShowPickerTypeProperty);
        set => SetValue(ShowPickerTypeProperty, value);
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
