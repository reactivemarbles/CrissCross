// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
public class ColorRoutedEventArgs(RoutedEvent routedEvent, Color color) : RoutedEventArgs(routedEvent)
{
    /// <summary>Gets the color.</summary>
    /// <value>
    /// The color.
    /// </value>
    public Color Color { get; private set; } = color;
}
