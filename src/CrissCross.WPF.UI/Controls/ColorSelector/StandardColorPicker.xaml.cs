// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents StandardColorPicker.</summary>
/// <seealso cref="DualPickerControlBase" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class StandardColorPicker : DualPickerControlBase
{
    /// <summary>The small change property.</summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
        nameof(SmallChange),
        typeof(double),
        typeof(StandardColorPicker),
        new(1.0));

    /// <summary>The show alpha property.</summary>
    public static readonly DependencyProperty ShowAlphaProperty = DependencyProperty.Register(
        nameof(ShowAlpha),
        typeof(bool),
        typeof(StandardColorPicker),
        new(true));

    /// <summary>The show hexadecimal property.</summary>
    public static readonly DependencyProperty ShowHexProperty = DependencyProperty.Register(
        nameof(ShowHex),
        typeof(Visibility),
        typeof(StandardColorPicker),
        new(Visibility.Visible));

    /// <summary>The show color swap property.</summary>
    public static readonly DependencyProperty ShowColorSwapProperty = DependencyProperty.Register(
        nameof(ShowColorSwap),
        typeof(Visibility),
        typeof(StandardColorPicker),
        new(Visibility.Visible));

    /// <summary>The show sliders property.</summary>
    public static readonly DependencyProperty ShowSlidersProperty = DependencyProperty.Register(
        nameof(ShowSliders),
        typeof(Visibility),
        typeof(StandardColorPicker),
        new(Visibility.Visible));

    /// <summary>The show picker type property.</summary>
    public static readonly DependencyProperty ShowPickerTypeProperty = DependencyProperty.Register(
        nameof(ShowPickerType),
        typeof(Visibility),
        typeof(StandardColorPicker),
        new(Visibility.Visible));

    /// <summary>The picker type property.</summary>
    public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register(
        nameof(PickerType),
        typeof(PickerType),
        typeof(StandardColorPicker),
        new(PickerType.HSV));

    /// <summary>Initializes a new instance of the <see cref="StandardColorPicker"/> class.</summary>
    public StandardColorPicker() => InitializeComponent();

    /// <summary>Gets or sets the small change.</summary>
    /// <value>
    /// The small change.
    /// </value>
    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [show alpha].</summary>
    /// <value>
    ///   <c>true</c> if [show alpha]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>Gets or sets the show hexadecimal.</summary>
    /// <value>
    /// The show hexadecimal.
    /// </value>
    public Visibility ShowHex
    {
        get => (Visibility)GetValue(ShowHexProperty);
        set => SetValue(ShowHexProperty, value);
    }

    /// <summary>Gets or sets the show sliders.</summary>
    /// <value>
    /// The show sliders.
    /// </value>
    public Visibility ShowSliders
    {
        get => (Visibility)GetValue(ShowSlidersProperty);
        set => SetValue(ShowSlidersProperty, value);
    }

    /// <summary>Gets or sets the show color swap.</summary>
    /// <value>
    /// The show color swap.
    /// </value>
    public Visibility ShowColorSwap
    {
        get => (Visibility)GetValue(ShowColorSwapProperty);
        set => SetValue(ShowColorSwapProperty, value);
    }

    /// <summary>Gets or sets the type of the show picker.</summary>
    /// <value>
    /// The type of the show picker.
    /// </value>
    public Visibility ShowPickerType
    {
        get => (Visibility)GetValue(ShowPickerTypeProperty);
        set => SetValue(ShowPickerTypeProperty, value);
    }

    /// <summary>Gets or sets the type of the picker.</summary>
    /// <value>
    /// The type of the picker.
    /// </value>
    public PickerType PickerType
    {
        get => (PickerType)GetValue(PickerTypeProperty);
        set => SetValue(PickerTypeProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
