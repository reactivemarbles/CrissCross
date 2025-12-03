// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a text control that makes suggestions to users as they enter text.
/// </summary>
public class AutoSuggestBox : global::Avalonia.Controls.AutoCompleteBox
{
    /// <summary>
    /// Property for <see cref="PlaceholderText"/>.
    /// </summary>
    public static readonly StyledProperty<string> PlaceholderTextProperty = AvaloniaProperty.Register<AutoSuggestBox, string>(
        nameof(PlaceholderText), string.Empty);

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }
}
