// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a rich editing control.
/// </summary>
public class RichTextBox : global::Avalonia.Controls.TextBox
{
    /// <summary>
    /// Property for <see cref="IsTextSelectionEnabledProperty"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty = AvaloniaProperty.Register<RichTextBox, bool>(
        nameof(IsTextSelectionEnabled), false);

    /// <summary>
    /// Gets or sets a value indicating whether this instance is text selection enabled.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }
}
