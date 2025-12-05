// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// PersonPicture.
/// </summary>
public class PersonPicture : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="DisplayName"/>.
    /// </summary>
    public static readonly StyledProperty<string> DisplayNameProperty = AvaloniaProperty.Register<PersonPicture, string>(
        nameof(DisplayName), string.Empty);

    /// <summary>
    /// Property for <see cref="Initials"/>.
    /// </summary>
    public static readonly StyledProperty<string> InitialsProperty = AvaloniaProperty.Register<PersonPicture, string>(
        nameof(Initials), string.Empty);

    /// <summary>
    /// Property for <see cref="IsGroup"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsGroupProperty = AvaloniaProperty.Register<PersonPicture, bool>(
        nameof(IsGroup), false);

    /// <summary>
    /// Property for <see cref="BadgeNumber"/>.
    /// </summary>
    public static readonly StyledProperty<int> BadgeNumberProperty = AvaloniaProperty.Register<PersonPicture, int>(
        nameof(BadgeNumber), 0);

    /// <summary>
    /// Property for <see cref="BadgeGlyph"/>.
    /// </summary>
    public static readonly StyledProperty<string> BadgeGlyphProperty = AvaloniaProperty.Register<PersonPicture, string>(
        nameof(BadgeGlyph), string.Empty);

    /// <summary>
    /// Property for <see cref="BadgeText"/>.
    /// </summary>
    public static readonly StyledProperty<string> BadgeTextProperty = AvaloniaProperty.Register<PersonPicture, string>(
        nameof(BadgeText), string.Empty);

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName
    {
        get => GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the initials.
    /// </summary>
    public string Initials
    {
        get => GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is a group.
    /// </summary>
    public bool IsGroup
    {
        get => GetValue(IsGroupProperty);
        set => SetValue(IsGroupProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge number.
    /// </summary>
    public int BadgeNumber
    {
        get => GetValue(BadgeNumberProperty);
        set => SetValue(BadgeNumberProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge glyph.
    /// </summary>
    public string BadgeGlyph
    {
        get => GetValue(BadgeGlyphProperty);
        set => SetValue(BadgeGlyphProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge text.
    /// </summary>
    public string BadgeText
    {
        get => GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }
}
