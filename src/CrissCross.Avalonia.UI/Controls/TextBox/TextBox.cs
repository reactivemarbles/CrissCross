// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a control that can be used to display and edit plain text.
/// </summary>
public class TextBox : global::Avalonia.Controls.TextBox
{
    /// <summary>
    /// Property for <see cref="PlaceholderText"/>.
    /// </summary>
    public static readonly StyledProperty<string> PlaceholderTextProperty = AvaloniaProperty.Register<TextBox, string>(
        nameof(PlaceholderText), string.Empty);

    /// <summary>
    /// Property for <see cref="CurrentPlaceholderEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CurrentPlaceholderEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(CurrentPlaceholderEnabled), true);

    /// <summary>
    /// Property for <see cref="ClearButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ClearButtonEnabledProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ClearButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="ShowClearButton"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowClearButtonProperty = AvaloniaProperty.Register<TextBox, bool>(
        nameof(ShowClearButton), true);

    /// <summary>
    /// Property for <see cref="IconPlacement"/>.
    /// </summary>
    public static readonly StyledProperty<string> IconPlacementProperty = AvaloniaProperty.Register<TextBox, string>(
        nameof(IconPlacement), "Left");

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<TextBox, object>(
        nameof(Icon), null);

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the placeholder is enabled.
    /// </summary>
    public bool CurrentPlaceholderEnabled
    {
        get => GetValue(CurrentPlaceholderEnabledProperty);
        set => SetValue(CurrentPlaceholderEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the clear button is enabled.
    /// </summary>
    public bool ClearButtonEnabled
    {
        get => GetValue(ClearButtonEnabledProperty);
        set => SetValue(ClearButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the clear button.
    /// </summary>
    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon placement.
    /// </summary>
    public string IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
