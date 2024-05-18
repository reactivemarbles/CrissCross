// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Converters;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// InfoBadge, a control that displays a small amount of information, typically a number or a small piece of text, in a compact way.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
public class InfoBadge : System.Windows.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(InfoBadge),
        new PropertyMetadata(null, null, IconSourceElementConverter.ConvertToIconElement));

    /// <summary>
    /// Property for <see cref="Severity"/>.
    /// </summary>
    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity),
        typeof(InfoBadgeSeverity),
        typeof(InfoBadge),
        new PropertyMetadata(InfoBadgeSeverity.Informational));

    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(string),
        typeof(InfoBadge),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(InfoBadge),
        new FrameworkPropertyMetadata(
                new CornerRadius(8),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets or sets the title of the <see cref="Severity" />.
    /// </summary>
    public InfoBadgeSeverity Severity
    {
        get => (InfoBadgeSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets the title of the <see cref="Value" />.
    /// </summary>
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the title of the <see cref="CornerRadius" />.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
