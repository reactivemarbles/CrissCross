// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Interactivity;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the ColorChanged routed event.
/// </summary>
public class ColorChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event.</param>
    /// <param name="source">The source.</param>
    /// <param name="oldColor">The old color.</param>
    /// <param name="newColor">The new color.</param>
    public ColorChangedEventArgs(RoutedEvent routedEvent, object? source, Color oldColor, Color newColor)
        : base(routedEvent, source)
    {
        OldColor = oldColor;
        NewColor = newColor;
    }

    /// <summary>
    /// Gets the old color.
    /// </summary>
    public Color OldColor { get; }

    /// <summary>
    /// Gets the new color.
    /// </summary>
    public Color NewColor { get; }
}
