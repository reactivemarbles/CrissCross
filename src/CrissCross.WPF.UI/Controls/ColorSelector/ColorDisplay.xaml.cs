// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents ColorDisplay.</summary>
/// <seealso cref="DualPickerControlBase" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class ColorDisplay : DualPickerControlBase
{
    /// <summary>The corner radius property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(double),
        typeof(ColorDisplay),
        new(0D));

    /// <summary>Initializes a new instance of the <see cref="ColorDisplay"/> class.</summary>
    public ColorDisplay() => InitializeComponent();

    /// <summary>Gets or sets the corner radius.</summary>
    /// <value>
    /// The corner radius.
    /// </value>
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Provides the SwapButton_Click member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    protected void SwapButton_Click(object sender, RoutedEventArgs e) => SwapColors();

    /// <summary>Provides the HintColor_OnMouseLeftButtonDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void HintColor_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => SetMainColorFromHintColor();
}
