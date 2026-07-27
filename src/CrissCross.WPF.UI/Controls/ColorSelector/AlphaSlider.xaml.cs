// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents AlphaSlider.</summary>
/// <seealso cref="PickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class AlphaSlider : PickerControlBase
{
    /// <summary>The small change property.</summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
        nameof(SmallChange),
        typeof(double),
        typeof(AlphaSlider),
        new(1.0));

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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
