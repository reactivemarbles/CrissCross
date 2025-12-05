// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents the base class for an icon source.
/// </summary>
public abstract class IconSource : AvaloniaObject
{
    /// <summary>
    /// Property for <see cref="Foreground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty = AvaloniaProperty.Register<IconSource, IBrush?>(
        nameof(Foreground), defaultValue: Brushes.Black);

    /// <summary>
    /// Gets or sets the foreground.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Creates the icon element.
    /// </summary>
    /// <returns>An IconElement.</returns>
    public abstract IconElement CreateIconElement();
}
