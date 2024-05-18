// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// ColorRoutedEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="ColorRoutedEventArgs"/> class.
/// </remarks>
/// <param name="routedEvent">The routed event.</param>
/// <param name="color">The color.</param>
public class ColorRoutedEventArgs(RoutedEvent routedEvent, Color color) : RoutedEventArgs(routedEvent)
{
    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Color Color { get; private set; } = color;
}
