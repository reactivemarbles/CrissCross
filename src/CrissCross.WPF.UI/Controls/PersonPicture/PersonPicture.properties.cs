// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// PersonPicture.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
public partial class PersonPicture
{
    /// <summary>
    /// The badge glyph property.
    /// </summary>
    public static readonly DependencyProperty BadgeGlyphProperty =
        DependencyProperty.Register(
            nameof(BadgeGlyph),
            typeof(string),
            typeof(PersonPicture),
            new PropertyMetadata(string.Empty, OnBadgeGlyphPropertyChanged, CoerceStringProperty));

    /// <summary>
    /// The badge image source property.
    /// </summary>
    public static readonly DependencyProperty BadgeImageSourceProperty =
        DependencyProperty.Register(
            nameof(BadgeImageSource),
            typeof(ImageSource),
            typeof(PersonPicture),
            new PropertyMetadata(null, OnBadgeImageSourcePropertyChanged));

    /// <summary>
    /// The badge number property.
    /// </summary>
    public static readonly DependencyProperty BadgeNumberProperty =
        DependencyProperty.Register(
            nameof(BadgeNumber),
            typeof(int),
            typeof(PersonPicture),
            new PropertyMetadata(0, OnBadgeNumberPropertyChanged));

    /// <summary>
    /// The badge text property.
    /// </summary>
    public static readonly DependencyProperty BadgeTextProperty =
        DependencyProperty.Register(
            nameof(BadgeText),
            typeof(string),
            typeof(PersonPicture),
            new PropertyMetadata(string.Empty, OnBadgeTextPropertyChanged, CoerceStringProperty));

    /// <summary>
    /// The display name property.
    /// </summary>
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(
            nameof(DisplayName),
            typeof(string),
            typeof(PersonPicture),
            new PropertyMetadata(string.Empty, OnDisplayNamePropertyChanged, CoerceStringProperty));

    /// <summary>
    /// The initials property.
    /// </summary>
    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(
            nameof(Initials),
            typeof(string),
            typeof(PersonPicture),
            new PropertyMetadata(string.Empty, OnInitialsPropertyChanged, CoerceStringProperty));

    /// <summary>
    /// The is group property.
    /// </summary>
    public static readonly DependencyProperty IsGroupProperty =
        DependencyProperty.Register(
            nameof(IsGroup),
            typeof(bool),
            typeof(PersonPicture),
            new PropertyMetadata(false, OnIsGroupPropertyChanged));

    /// <summary>
    /// The profile picture property.
    /// </summary>
    public static readonly DependencyProperty ProfilePictureProperty =
        DependencyProperty.Register(
            nameof(ProfilePicture),
            typeof(ImageSource),
            typeof(PersonPicture),
            new PropertyMetadata(null, OnProfilePicturePropertyChanged));

    private static readonly DependencyPropertyKey TemplateSettingsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(TemplateSettings),
            typeof(PersonPictureTemplateSettings),
            typeof(PersonPicture),
            new PropertyMetadata(null, OnTemplateSettingsPropertyChanged));

    /// <summary>
    /// The template settings property.
    /// </summary>
#pragma warning disable SA1202 // Elements should be ordered by access
    public static readonly DependencyProperty TemplateSettingsProperty =
#pragma warning restore SA1202 // Elements should be ordered by access
        TemplateSettingsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets the badge glyph.
    /// </summary>
    /// <value>
    /// The badge glyph.
    /// </value>
    public string BadgeGlyph
    {
        get => (string)GetValue(BadgeGlyphProperty);
        set => SetValue(BadgeGlyphProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge image source.
    /// </summary>
    /// <value>
    /// The badge image source.
    /// </value>
    public ImageSource BadgeImageSource
    {
        get => (ImageSource)GetValue(BadgeImageSourceProperty);
        set => SetValue(BadgeImageSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge number.
    /// </summary>
    /// <value>
    /// The badge number.
    /// </value>
    public int BadgeNumber
    {
        get => (int)GetValue(BadgeNumberProperty);
        set => SetValue(BadgeNumberProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge text.
    /// </summary>
    /// <value>
    /// The badge text.
    /// </value>
    public string BadgeText
    {
        get => (string)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the initials.
    /// </summary>
    /// <value>
    /// The initials.
    /// </value>
    public string Initials
    {
        get => (string)GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is group.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is group; otherwise, <c>false</c>.
    /// </value>
    public bool IsGroup
    {
        get => (bool)GetValue(IsGroupProperty);
        set => SetValue(IsGroupProperty, value);
    }

    /// <summary>
    /// Gets or sets the profile picture.
    /// </summary>
    /// <value>
    /// The profile picture.
    /// </value>
    public ImageSource ProfilePicture
    {
        get => (ImageSource)GetValue(ProfilePictureProperty);
        set => SetValue(ProfilePictureProperty, value);
    }

    /// <summary>
    /// Gets the template settings.
    /// </summary>
    /// <value>
    /// The template settings.
    /// </value>
    public PersonPictureTemplateSettings TemplateSettings
    {
        get => (PersonPictureTemplateSettings)GetValue(TemplateSettingsProperty);
        private set => SetValue(TemplateSettingsPropertyKey, value);
    }

    private static void OnBadgeImageSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnBadgeGlyphPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnBadgeNumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnBadgeTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnDisplayNamePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnInitialsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnIsGroupPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnProfilePicturePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static void OnTemplateSettingsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var owner = (PersonPicture)sender;
        owner.PrivateOnPropertyChanged(args);
    }

    private static object CoerceStringProperty(DependencyObject d, object baseValue) => baseValue ?? string.Empty;
}
