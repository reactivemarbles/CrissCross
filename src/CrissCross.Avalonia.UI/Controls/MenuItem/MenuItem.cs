// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a menu item that can display an icon in addition to its content.
/// </summary>
public class MenuItem : global::Avalonia.Controls.MenuItem
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<MenuItem, object?>(
        nameof(Icon), null);

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public new object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
