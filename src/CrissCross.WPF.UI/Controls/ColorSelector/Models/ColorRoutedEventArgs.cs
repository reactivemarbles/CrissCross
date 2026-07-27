// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents ColorRoutedEventArgs.</summary>
/// <seealso cref="RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="ColorRoutedEventArgs"/> class.
/// </remarks>
/// <param name="routedEvent">The routed event.</param>
/// <param name="color">The color.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ColorRoutedEventArgs(RoutedEvent routedEvent, Color color) : RoutedEventArgs(routedEvent)
{
    /// <summary>Gets the color.</summary>
    /// <value>
    /// The color.
    /// </value>
    public Color Color { get; } = color;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
