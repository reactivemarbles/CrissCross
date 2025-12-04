// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Inherited from the <see cref="Avalonia.Controls.Expander"/> control which can hide the collapsible content.
/// </summary>
public class CardExpander : global::Avalonia.Controls.Expander
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<CardExpander, object>(
        nameof(Icon), null);

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<CardExpander, CornerRadius>(
        nameof(CornerRadius), new CornerRadius(4));

    /// <summary>
    /// Property for <see cref="ContentPadding"/>.
    /// </summary>
    public static readonly StyledProperty<Thickness> ContentPaddingProperty = AvaloniaProperty.Register<CardExpander, Thickness>(
        nameof(ContentPadding), default);

    /// <summary>
    /// Gets or sets displayed icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets content padding.
    /// </summary>
    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
}
