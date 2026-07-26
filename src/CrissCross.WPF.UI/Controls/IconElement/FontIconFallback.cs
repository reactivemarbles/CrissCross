// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents FontIconFallback.</summary>
/// <seealso cref="Control" />
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class FontIconFallback : Control
{
    /// <summary>The data property.</summary>
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(Geometry),
        typeof(FontIconFallback),
        null);

    /// <summary>Provides the FontIconFallback member.</summary>
    static FontIconFallback()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FontIconFallback),
            new FrameworkPropertyMetadata(typeof(FontIconFallback)));
        FocusableProperty.OverrideMetadata(typeof(FontIconFallback), new FrameworkPropertyMetadata(false));
    }

    /// <summary>Gets or sets the data.</summary>
    /// <value>
    /// The data.
    /// </value>
    public Geometry Data
    {
        get => (Geometry)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
