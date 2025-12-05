// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an icon that uses an IconSource as its content.
/// </summary>
public class IconSourceElement : IconElement
{
    /// <summary>
    /// Property for <see cref="IconSource"/>.
    /// </summary>
    public static readonly StyledProperty<IconSource?> IconSourceProperty = AvaloniaProperty.Register<IconSourceElement, IconSource?>(
        nameof(IconSource), null);

    /// <summary>
    /// Gets or sets the IconSource.
    /// </summary>
    public IconSource? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Creates the icon element.
    /// </summary>
    /// <returns>An IconElement.</returns>
    public IconElement? CreateIconElement() => IconSource?.CreateIconElement();
}
