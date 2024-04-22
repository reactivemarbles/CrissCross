// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// PersonPicture.
/// </summary>
/// <seealso cref="Control" />
public partial class PersonPicture : Control
{
    private static readonly ResourceAccessor ResourceAccessor = new(typeof(PersonPicture));

    private TextBlock? _m_initialsTextBlock;
    private TextBlock? _m_badgeNumberTextBlock;
    private FontIcon? _m_badgeGlyphIcon;
    private ImageBrush? _m_badgeImageBrush;
    private Ellipse? _m_badgingEllipse;
    private Ellipse? _m_badgingBackgroundEllipse;
    private string? _m_displayNameInitials;

    static PersonPicture() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PersonPicture), new FrameworkPropertyMetadata(typeof(PersonPicture)));

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonPicture"/> class.
    /// </summary>
    public PersonPicture()
    {
        TemplateSettings = new PersonPictureTemplateSettings();

        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _m_initialsTextBlock = GetTemplateChild("InitialsTextBlock") as TextBlock;

        _m_badgeNumberTextBlock = GetTemplateChild("BadgeNumberTextBlock") as TextBlock;
        _m_badgeGlyphIcon = GetTemplateChild("BadgeGlyphIcon") as FontIcon;
        _m_badgingEllipse = GetTemplateChild("BadgingEllipse") as Ellipse;
        _m_badgingBackgroundEllipse = GetTemplateChild("BadgingBackgroundEllipse") as Ellipse;

        UpdateBadge();
        UpdateIfReady();
    }

    /// <summary>
    /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations for the Windows Presentation Foundation (WPF) infrastructure.
    /// </summary>
    /// <returns>
    /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new PersonPictureAutomationPeer(this);

    private static string? GetLocalizedPluralBadgeItemStringResource(int numericValue)
    {
        var valueMod10 = numericValue % 10;

        if (numericValue == 1)
        {
            // Singular
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemSingular);
        }
        else if (numericValue == 2)
        {
            // 2
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural7);
        }
        else if (numericValue == 3 || numericValue == 4)
        {
            // 3,4
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural2);
        }
        else if (numericValue >= 5 && numericValue <= 10)
        {
            // 5-10
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural5);
        }
        else if (numericValue >= 11 && numericValue <= 19)
        {
            // 11-19
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural6);
        }
        else if (valueMod10 == 1)
        {
            // 21, 31, 41, etc.
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural1);
        }
        else if (valueMod10 >= 2 && valueMod10 <= 4)
        {
            // 22-24, 32-34, 42-44, etc.
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural3);
        }
        else
        {
            // Everything else... 0, 20, 25-30, 35-40, etc.
            return ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemPlural4);
        }
    }

    /// <summary>
    /// Helper to determine the initials that should be shown.
    /// </summary>
    private string? GetInitials()
    {
        if (!string.IsNullOrEmpty(Initials))
        {
            return Initials;
        }
        else if (!string.IsNullOrEmpty(_m_displayNameInitials))
        {
            return _m_displayNameInitials;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Helper to determine the image source that should be shown.
    /// </summary>
    private ImageSource? GetImageSource() => ProfilePicture;

    /// <summary>
    /// Updates Control elements, if available, with the latest values.
    /// </summary>
    private void UpdateIfReady()
    {
        var initials = GetInitials();
        var imageSrc = GetImageSource();

        var templateSettings = TemplateSettings;
        templateSettings.ActualInitials = initials;
        if (imageSrc != null)
        {
            var imageBrush = templateSettings.ActualImageBrush;
            if (imageBrush == null)
            {
                imageBrush = new ImageBrush
                {
                    Stretch = Stretch.UniformToFill
                };
                templateSettings.ActualImageBrush = imageBrush;
            }

            imageBrush.ImageSource = imageSrc;
        }
        else
        {
            templateSettings.ActualImageBrush = null!;
        }

        // If the control is converted to 'Group-mode', we'll clear individual-specific information.
        // When IsGroup evaluates to false, we will restore state.
        if (IsGroup)
        {
            VisualStateManager.GoToState(this, "Group", false);
        }
        else if (imageSrc != null)
        {
            VisualStateManager.GoToState(this, "Photo", false);
        }
        else if (!string.IsNullOrEmpty(initials))
        {
            VisualStateManager.GoToState(this, "Initials", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "NoPhotoOrInitials", false);
        }

        UpdateAutomationName();
    }

    /// <summary>
    /// Updates the state of the Badging element.
    /// </summary>
    private void UpdateBadge()
    {
        if (BadgeImageSource != null)
        {
            UpdateBadgeImageSource();
        }
        else if (BadgeNumber != 0)
        {
            UpdateBadgeNumber();
        }
        else if (!string.IsNullOrEmpty(BadgeGlyph))
        {
            UpdateBadgeGlyph();
        }
        else
        {// No badge properties set, so clear the badge XAML
            VisualStateManager.GoToState(this, "NoBadge", false);

            var badgeNumberTextBlock = _m_badgeNumberTextBlock;
            if (badgeNumberTextBlock != null)
            {
                badgeNumberTextBlock.Text = string.Empty;
            }

            var badgeGlyphIcon = _m_badgeGlyphIcon;
            if (badgeGlyphIcon != null)
            {
                badgeGlyphIcon.Glyph = string.Empty;
            }
        }

        UpdateAutomationName();
    }

    /// <summary>
    /// Updates Badging Number text element.
    /// </summary>
    private void UpdateBadgeNumber()
    {
        if (_m_badgingEllipse == null || _m_badgeNumberTextBlock == null)
        {
            return;
        }

        var badgeNumber = BadgeNumber;

        if (badgeNumber <= 0)
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
            _m_badgeNumberTextBlock.Text = string.Empty;
            return;
        }

        // should have badging number to show if we are here
        VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        if (badgeNumber <= 99)
        {
            _m_badgeNumberTextBlock.Text = badgeNumber.ToString();
        }
        else
        {
            _m_badgeNumberTextBlock.Text = "99+";
        }
    }

    /// <summary>
    /// Updates Badging Glyph element.
    /// </summary>
    private void UpdateBadgeGlyph()
    {
        if (_m_badgingEllipse == null || _m_badgeGlyphIcon == null)
        {
            return;
        }

        var badgeGlyph = BadgeGlyph;

        if (string.IsNullOrEmpty(badgeGlyph))
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
            _m_badgeGlyphIcon.Glyph = string.Empty;
            return;
        }

        // should have badging Glyph to show if we are here
        VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        _m_badgeGlyphIcon.Glyph = badgeGlyph;
    }

    /// <summary>
    /// Updates Badging Image element.
    /// </summary>
    private void UpdateBadgeImageSource()
    {
        _m_badgeImageBrush ??= GetTemplateChild("BadgeImageBrush") as ImageBrush;

        if (_m_badgingEllipse == null || _m_badgeImageBrush == null)
        {
            return;
        }

        _m_badgeImageBrush.ImageSource = BadgeImageSource;

        if (BadgeImageSource != null)
        {
            VisualStateManager.GoToState(this, "BadgeWithImageSource", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "NoBadge", false);
        }
    }

    /// <summary>
    /// Sets the UI Automation name for the control based on contact name and badge state.
    /// </summary>
    private void UpdateAutomationName()
    {
        string? automationName;
        string? contactName;

        // The AutomationName for the control is in the format: PersonName, BadgeInformation.
        // PersonName is set based on the name / initial properties in the order below.
        // if none exist, it defaults to "Person"
        if (IsGroup)
        {
            contactName = ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_GroupName);
        }
        else if (!string.IsNullOrEmpty(DisplayName))
        {
            contactName = DisplayName;
        }
        else if (!string.IsNullOrEmpty(Initials))
        {
            contactName = Initials;
        }
        else
        {
            contactName = ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_PersonName);
        }

        // BadgeInformation portion of the AutomationName is set to 'n items' if there is a BadgeNumber,
        // or 'icon' for BadgeGlyph or BadgeImageSource. If BadgeText is specified, it will override
        // the string 'items' or 'icon'
        if (BadgeNumber > 0)
        {
            if (!string.IsNullOrEmpty(BadgeText))
            {
                automationName = string.Format(
                    ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeItemTextOverride)!,
                    contactName,
                    BadgeNumber,
                    BadgeText);
            }
            else
            {
                automationName = string.Format(
                    GetLocalizedPluralBadgeItemStringResource(BadgeNumber)!,
                    contactName,
                    BadgeNumber);
            }
        }
        else if (!string.IsNullOrEmpty(BadgeGlyph) || BadgeImageSource != null)
        {
            if (!string.IsNullOrEmpty(BadgeText))
            {
                automationName = string.Format(
                    ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeIconTextOverride)!,
                    contactName,
                    BadgeText);
            }
            else
            {
                automationName = string.Format(
                    ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.SR_BadgeIcon)!,
                    contactName);
            }
        }
        else
        {
            automationName = contactName;
        }

        AutomationProperties.SetName(this, automationName);
    }

    private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        var property = args.Property;

        if (property == BadgeNumberProperty ||
            property == BadgeGlyphProperty ||
            property == BadgeImageSourceProperty)
        {
            UpdateBadge();
        }
        else if (property == BadgeTextProperty)
        {
            UpdateAutomationName();
        }
        else if (property == DisplayNameProperty)
        {
            OnDisplayNameChanged();
        }
        else if (property == ProfilePictureProperty ||
            property == InitialsProperty ||
            property == IsGroupProperty)
        {
            UpdateIfReady();
        }
    }

    // DependencyProperty changed event handlers
    private void OnDisplayNameChanged()
    {
        _m_displayNameInitials = InitialsGenerator.InitialsFromDisplayName(DisplayName);

        UpdateIfReady();
    }

    // Event handlers
    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        {
            var widthChanged = args.NewSize.Width != args.PreviousSize.Width;
            var heightChanged = args.NewSize.Height != args.PreviousSize.Height;
            double newSize;

            if (widthChanged && heightChanged)
            {
                // Maintain circle by enforcing the new size on both Width and Height.
                // To do so, we will use the minimum value.
                newSize = (args.NewSize.Width < args.NewSize.Height) ? args.NewSize.Width : args.NewSize.Height;
            }
            else if (widthChanged)
            {
                newSize = args.NewSize.Width;
            }
            else if (heightChanged)
            {
                newSize = args.NewSize.Height;
            }
            else
            {
                return;
            }

            Height = newSize;
            Width = newSize;
        }

        // Calculate the FontSize of the control's text. Design guidelines have specified the
        // font size to be 42% of the container. Since it's circular, 42% of either Width or Height.
        // Note that we cap it to a minimum of 1, since a font size of less than 1 is an invalid value
        // that will result in a failure.
        var fontSize = Math.Max(1.0, Width * .42);

        var initialsTextBlock = _m_initialsTextBlock;
        if (initialsTextBlock != null)
        {
            initialsTextBlock.FontSize = fontSize;
        }

        if (_m_badgingEllipse != null && _m_badgingBackgroundEllipse != null && _m_badgeNumberTextBlock != null && _m_badgeGlyphIcon != null)
        {
            // Maintain badging circle and font size by enforcing the new size on both Width and Height.
            // Design guidelines have specified the font size to be 60% of the badging plate, and we want to keep
            // badging plate to be about 50% of the control so that don't block the initial/profile picture.
            var newSize = (args.NewSize.Width < args.NewSize.Height) ? args.NewSize.Width : args.NewSize.Height;
            _m_badgingEllipse.Height = newSize * 0.5;
            _m_badgingEllipse.Width = newSize * 0.5;
            _m_badgingBackgroundEllipse.Height = newSize * 0.5;
            _m_badgingBackgroundEllipse.Width = newSize * 0.5;
            _m_badgeNumberTextBlock.FontSize = Math.Max(1.0, _m_badgingEllipse.Height * 0.6);
            _m_badgeGlyphIcon.FontSize = Math.Max(1.0, _m_badgingEllipse.Height * 0.6);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
    }
}
