// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// InfoBadge, a control that displays a small amount of information, typically a number or a small piece of text, in a compact way.
/// </summary>
public class InfoBadge : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Severity"/>.
    /// </summary>
    public static readonly StyledProperty<InfoBadgeSeverity> SeverityProperty = AvaloniaProperty.Register<InfoBadge, InfoBadgeSeverity>(
        nameof(Severity), InfoBadgeSeverity.Informational);

    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<InfoBadge, string>(
        nameof(Value), string.Empty);

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<InfoBadge, CornerRadius>(
        nameof(CornerRadius), new CornerRadius(8));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<InfoBadge, object>(
        nameof(Icon), null);

    /// <summary>
    /// Gets or sets the severity.
    /// </summary>
    public InfoBadgeSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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
    /// Gets or sets the icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
