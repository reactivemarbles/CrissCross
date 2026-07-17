// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a menu item that can display an icon in addition to its content.</summary>
public class MenuItem : global::Avalonia.Controls.MenuItem
{
    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static new readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<MenuItem, object?>(
        nameof(Icon));

    /// <summary>Gets or sets the icon.</summary>
    public new object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
