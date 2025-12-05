// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that creates a pop-up window that displays information for an element.
/// </summary>
public class ToolTip : Control
{
    /// <summary>
    /// Property for <see cref="Text"/>.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<ToolTip, string?>(nameof(Text));

    /// <summary>
    /// Gets or sets the tooltip text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
