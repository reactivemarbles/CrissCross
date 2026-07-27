// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents PortableColorPicker.</summary>
/// <seealso cref="DualPickerControlBase" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class PortableColorPicker : DualPickerControlBase
{
    /// <summary>The small change property.</summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
        nameof(SmallChange),
        typeof(double),
        typeof(PortableColorPicker),
        new(1.0));

    /// <summary>The show alpha property.</summary>
    public static readonly DependencyProperty ShowAlphaProperty = DependencyProperty.Register(
        nameof(ShowAlpha),
        typeof(bool),
        typeof(PortableColorPicker),
        new(true));

    /// <summary>The picker type property.</summary>
    public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register(
        nameof(PickerType),
        typeof(PickerType),
        typeof(PortableColorPicker),
        new(PickerType.HSV));

    /// <summary>Initializes a new instance of the <see cref="PortableColorPicker"/> class.</summary>
    public PortableColorPicker() => InitializeComponent();

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
