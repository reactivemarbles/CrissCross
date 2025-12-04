// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Inherited from the <see cref="Avalonia.Controls.Button"/> interactive card styled according to Fluent Design.
/// </summary>
public class CardAction : global::Avalonia.Controls.Button
{
    /// <summary>
    /// Property for <see cref="IsChevronVisible"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsChevronVisibleProperty = AvaloniaProperty.Register<CardAction, bool>(
        nameof(IsChevronVisible), true);

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<CardAction, object>(
        nameof(Icon), null);

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether to display the chevron icon on the right side of the card.
    /// </summary>
    public bool IsChevronVisible
    {
        get => GetValue(IsChevronVisibleProperty);
        set => SetValue(IsChevronVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
