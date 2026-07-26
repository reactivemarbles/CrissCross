// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extended <see cref="System.Windows.Controls.Primitives.ToggleButton"/>.</summary>
/// <seealso cref="System.Windows.Controls.Primitives.ToggleButton" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
{
    /// <summary>The TreeView item chevron size property.</summary>
    public static readonly DependencyProperty ChevronSizeProperty = DependencyProperty.Register(
        nameof(ChevronSize),
        typeof(double),
        typeof(ToggleButton),
        new(10D));

    /// <summary>Gets or sets the size of the TreeView item chevron.</summary>
    /// <value>
    /// The size of the TreeView item chevron.
    /// </value>
    public double ChevronSize
    {
        get => (double)GetValue(ChevronSizeProperty);
        set => SetValue(ChevronSizeProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
