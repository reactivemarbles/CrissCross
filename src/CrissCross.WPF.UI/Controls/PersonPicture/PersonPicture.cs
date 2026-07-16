// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents PersonPicture.</summary>
/// <seealso cref="Control" />
public partial class PersonPicture : Control, IDisposable
{
    /// <summary>Divisor used to evaluate plural resource suffixes.</summary>
    private const int PluralFormDivisor = 10;

    /// <summary>Start of the second plural resource range.</summary>
    private const int BadgeItemPluralTwoStart = 3;

    /// <summary>Start of the third plural resource range.</summary>
    private const int BadgeItemPluralThreeStart = 2;

    /// <summary>End of the second plural resource range.</summary>
    private const int BadgeItemPluralTwoEnd = 4;

    /// <summary>Start of the fifth plural resource range.</summary>
    private const int BadgeItemPluralFiveStart = 5;

    /// <summary>End of the fifth plural resource range.</summary>
    private const int BadgeItemPluralFiveEnd = 10;

    /// <summary>Start of the sixth plural resource range.</summary>
    private const int BadgeItemPluralSixStart = 11;

    /// <summary>End of the sixth plural resource range.</summary>
    private const int BadgeItemPluralSixEnd = 19;

    /// <summary>Largest badge number displayed directly before using a capped label.</summary>
    private const int BadgeMaximumDisplayNumber = 99;

    /// <summary>Initials font size ratio relative to the picture width.</summary>
    private const double InitialsFontSizeRatio = 0.42;

    /// <summary>Badge ellipse size ratio relative to the picture size.</summary>
    private const double BadgeSizeRatio = 0.5;

    /// <summary>Badge text font size ratio relative to the badge ellipse height.</summary>
    private const double BadgeFontSizeRatio = 0.6;

    /// <summary>The visual state used when no badge is displayed.</summary>
    private const string NoBadgeVisualState = "NoBadge";

    /// <summary>Provides the ResourceAccessor member.</summary>
    private static readonly ResourceAccessor ResourceAccessor = new(typeof(PersonPicture));

    /// <summary>Stores the _m_initialsTextBlock value.</summary>
    private TextBlock? _m_initialsTextBlock;

    /// <summary>Stores the _m_badgeNumberTextBlock value.</summary>
    private TextBlock? _m_badgeNumberTextBlock;

    /// <summary>Stores the _m_badgeGlyphIcon value.</summary>
    private FontIcon? _m_badgeGlyphIcon;

    /// <summary>Stores the _m_badgeImageBrush value.</summary>
    private ImageBrush? _m_badgeImageBrush;

    /// <summary>Stores the _m_badgingEllipse value.</summary>
    private Ellipse? _m_badgingEllipse;

    /// <summary>Stores the _m_badgingBackgroundEllipse value.</summary>
    private Ellipse? _m_badgingBackgroundEllipse;

    /// <summary>Stores the _m_displayNameInitials value.</summary>
    private string? _m_displayNameInitials;

    /// <summary>Stores the _disposed value.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="PersonPicture"/> class.</summary>
    public PersonPicture()
    {
        TemplateSettings = new();

        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    /// <summary>Disposes resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>When template applied.</summary>
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

    /// <summary>Returns class-specific automation peer.</summary>
    /// <returns>The automation peer.</returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new PersonPictureAutomationPeer(this);

    /// <summary>Core dispose logic.</summary>
    /// <param name="disposing">True when called from Dispose.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Unloaded -= OnUnloaded;
            SizeChanged -= OnSizeChanged;
        }

        _disposed = true;
    }

    /// <summary>Static helper after fields / ctor per SA12 series preference for fields, ctors, methods.</summary>
    /// <param name="numericValue">The numericvalue.</param>
    /// <returns>The localized badge text for the numeric value.</returns>
    private static string? GetLocalizedPluralBadgeItemStringResource(int numericValue)
    {
        var valueMod10 = numericValue % PluralFormDivisor;
        var resourceName = numericValue switch
        {
            1 => ResourceAccessor.BadgeItemSingular,
            BadgeItemPluralThreeStart => ResourceAccessor.BadgeItemPlural7,
            >= BadgeItemPluralTwoStart and <= BadgeItemPluralTwoEnd => ResourceAccessor.BadgeItemPlural2,
            >= BadgeItemPluralFiveStart and <= BadgeItemPluralFiveEnd => ResourceAccessor.BadgeItemPlural5,
            >= BadgeItemPluralSixStart and <= BadgeItemPluralSixEnd => ResourceAccessor.BadgeItemPlural6,
            _ when valueMod10 == 1 => ResourceAccessor.BadgeItemPlural1,
            _ when valueMod10 is >= BadgeItemPluralThreeStart and <= BadgeItemPluralTwoEnd =>
                ResourceAccessor.BadgeItemPlural3,
            _ => ResourceAccessor.BadgeItemPlural4,
        };

        return ResourceAccessor.GetLocalizedStringResource(resourceName);
    }

    /// <summary>Gets the square size required after a size change.</summary>
    /// <param name="args">The size changed event arguments.</param>
    /// <returns>The square size, or <see langword="null"/> when neither dimension changed.</returns>
    private static double? GetSquareSize(SizeChangedEventArgs args)
    {
        var widthChanged = !DoubleComparison.AreClose(args.NewSize.Width, args.PreviousSize.Width);
        var heightChanged = !DoubleComparison.AreClose(args.NewSize.Height, args.PreviousSize.Height);

        return (widthChanged, heightChanged) switch
        {
            (true, true) => Math.Min(args.NewSize.Width, args.NewSize.Height),
            (true, false) => args.NewSize.Width,
            (false, true) => args.NewSize.Height,
            _ => null,
        };
    }

    /// <summary>Helper to determine the initials that should be shown.</summary>
    /// <returns>The result.</returns>
    private string? GetInitials() =>
        (Initials, _m_displayNameInitials) switch
        {
            ({ Length: > 0 } initials, _) => initials,
            (_, { Length: > 0 } displayNameInitials) => displayNameInitials,
            _ => null,
        };

    /// <summary>Helper to determine the image source that should be shown.</summary>
    /// <returns>The result.</returns>
    private ImageSource? GetImageSource() => ProfilePicture;

    /// <summary>Updates Control elements, if available, with the latest values.</summary>
    private void UpdateIfReady()
    {
        var initials = GetInitials();
        var imageSrc = GetImageSource();

        var templateSettings = TemplateSettings;
        templateSettings.ActualInitials = initials;
        if (imageSrc is not null)
        {
            var imageBrush = templateSettings.ActualImageBrush;
            if (imageBrush is null)
            {
                imageBrush = new ImageBrush { Stretch = Stretch.UniformToFill };
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
            _ = VisualStateManager.GoToState(this, "Group", false);
        }
        else if (imageSrc is not null)
        {
            _ = VisualStateManager.GoToState(this, "Photo", false);
        }
        else if (!string.IsNullOrEmpty(initials))
        {
            _ = VisualStateManager.GoToState(this, nameof(Initials), false);
        }
        else
        {
            _ = VisualStateManager.GoToState(this, "NoPhotoOrInitials", false);
        }

        UpdateAutomationName();
    }

    /// <summary>Updates the state of the Badging element.</summary>
    private void UpdateBadge()
    {
        if (BadgeImageSource is not null)
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
        { // No badge properties set, so clear the badge XAML
            _ = VisualStateManager.GoToState(this, NoBadgeVisualState, false);

            var badgeNumberTextBlock = _m_badgeNumberTextBlock;
            if (badgeNumberTextBlock is not null)
            {
                badgeNumberTextBlock.Text = string.Empty;
            }

            var badgeGlyphIcon = _m_badgeGlyphIcon;
            if (badgeGlyphIcon is not null)
            {
                badgeGlyphIcon.Glyph = string.Empty;
            }
        }

        UpdateAutomationName();
    }

    /// <summary>Updates Badging Number text element.</summary>
    private void UpdateBadgeNumber()
    {
        if (_m_badgingEllipse is null || _m_badgeNumberTextBlock is null)
        {
            return;
        }

        var badgeNumber = BadgeNumber;

        if (badgeNumber <= 0)
        {
            _ = VisualStateManager.GoToState(this, NoBadgeVisualState, false);
            _m_badgeNumberTextBlock.Text = string.Empty;
            return;
        }

        // should have badging number to show if we are here
        _ = VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        if (badgeNumber <= BadgeMaximumDisplayNumber)
        {
            _m_badgeNumberTextBlock.Text = badgeNumber.ToString();
        }
        else
        {
            _m_badgeNumberTextBlock.Text = "99+";
        }
    }

    /// <summary>Updates Badging Glyph element.</summary>
    private void UpdateBadgeGlyph()
    {
        if (_m_badgingEllipse is null || _m_badgeGlyphIcon is null)
        {
            return;
        }

        var badgeGlyph = BadgeGlyph;

        if (string.IsNullOrEmpty(badgeGlyph))
        {
            _ = VisualStateManager.GoToState(this, NoBadgeVisualState, false);
            _m_badgeGlyphIcon.Glyph = string.Empty;
            return;
        }

        // should have badging Glyph to show if we are here
        _ = VisualStateManager.GoToState(this, "BadgeWithoutImageSource", false);

        _m_badgeGlyphIcon.Glyph = badgeGlyph;
    }

    /// <summary>Updates Badging Image element.</summary>
    private void UpdateBadgeImageSource()
    {
        _m_badgeImageBrush ??= GetTemplateChild("BadgeImageBrush") as ImageBrush;

        if (_m_badgingEllipse is null || _m_badgeImageBrush is null)
        {
            return;
        }

        _m_badgeImageBrush.ImageSource = BadgeImageSource;

        if (BadgeImageSource is not null)
        {
            _ = VisualStateManager.GoToState(this, "BadgeWithImageSource", false);
        }
        else
        {
            _ = VisualStateManager.GoToState(this, NoBadgeVisualState, false);
        }
    }

    /// <summary>Sets the UI Automation name for the control based on contact name and badge state.</summary>
    private void UpdateAutomationName()
    {
        // The AutomationName for the control is in the format: PersonName, BadgeInformation.
        // PersonName is set based on the name / initial properties in the order below.
        // if none exist, it defaults to "Person"
        var contactName = IsGroup switch
        {
            true => ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.GroupName),
            _ when !string.IsNullOrEmpty(DisplayName) => DisplayName,
            _ when !string.IsNullOrEmpty(Initials) => Initials,
            _ => ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.PersonName),
        };

        // BadgeInformation portion of the AutomationName is set to 'n items' if there is a BadgeNumber,
        // or 'icon' for BadgeGlyph or BadgeImageSource. If BadgeText is specified, it will override
        // the string 'items' or 'icon'
        var automationName = BadgeNumber switch
        {
            > 0 when !string.IsNullOrEmpty(BadgeText) => string.Format(
                ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.BadgeItemTextOverride)!,
                contactName,
                BadgeNumber,
                BadgeText),
            > 0 => string.Format(GetLocalizedPluralBadgeItemStringResource(BadgeNumber)!, contactName, BadgeNumber),
            _ when (!string.IsNullOrEmpty(BadgeGlyph) || BadgeImageSource is not null)
                    && !string.IsNullOrEmpty(BadgeText) => string.Format(
                ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.BadgeIconTextOverride)!,
                contactName,
                BadgeText),
            _ when !string.IsNullOrEmpty(BadgeGlyph) || BadgeImageSource is not null => string.Format(
                ResourceAccessor.GetLocalizedStringResource(ResourceAccessor.BadgeIcon)!,
                contactName),
            _ => contactName,
        };

        AutomationProperties.SetName(this, automationName);
    }

    /// <summary>Provides the PrivateOnPropertyChanged member.</summary>
    /// <param name="args">The event arguments.</param>
    private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        var property = args.Property;

        if (property == BadgeNumberProperty || property == BadgeGlyphProperty || property == BadgeImageSourceProperty)
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
        else if (property == ProfilePictureProperty || property == InitialsProperty || property == IsGroupProperty)
        {
            UpdateIfReady();
        }
    }

    /// <summary>DependencyProperty changed event handlers.</summary>
    private void OnDisplayNameChanged()
    {
        _m_displayNameInitials = InitialsGeneratorExtensions.InitialsFromDisplayName(DisplayName);

        UpdateIfReady();
    }

    /// <summary>Event handlers.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        var squareSize = GetSquareSize(args);
        if (squareSize is null)
        {
            return;
        }

        Height = squareSize.Value;
        Width = squareSize.Value;

        // Calculate the FontSize of the control's text. Design guidelines have specified the
        // font size to be 42% of the container. Since it's circular, 42% of either Width or Height.
        // Note that we cap it to a minimum of 1, since a font size of less than 1 is an invalid value
        // that will result in a failure.
        var fontSize = Math.Max(1.0, Width * InitialsFontSizeRatio);

        var initialsTextBlock = _m_initialsTextBlock;
        if (initialsTextBlock is not null)
        {
            initialsTextBlock.FontSize = fontSize;
        }

        UpdateBadgeSize(args.NewSize);
    }

    /// <summary>Updates the badge visual size for the control size.</summary>
    /// <param name="newControlSize">The new control size.</param>
    private void UpdateBadgeSize(Size newControlSize)
    {
        if (
            _m_badgingEllipse is null
            || _m_badgingBackgroundEllipse is null
            || _m_badgeNumberTextBlock is null
            || _m_badgeGlyphIcon is null)
        {
            return;
        }

        // Maintain badging circle and font size by enforcing the new size on both Width and Height.
        // Design guidelines have specified the font size to be 60% of the badging plate, and we want to keep
        // badging plate to be about 50% of the control so that don't block the initial/profile picture.
        var newSize = Math.Min(newControlSize.Width, newControlSize.Height);
        _m_badgingEllipse.Height = newSize * BadgeSizeRatio;
        _m_badgingEllipse.Width = newSize * BadgeSizeRatio;
        _m_badgingBackgroundEllipse.Height = newSize * BadgeSizeRatio;
        _m_badgingBackgroundEllipse.Width = newSize * BadgeSizeRatio;
        _m_badgeNumberTextBlock.FontSize = Math.Max(1.0, _m_badgingEllipse.Height * BadgeFontSizeRatio);
        _m_badgeGlyphIcon.FontSize = Math.Max(1.0, _m_badgingEllipse.Height * BadgeFontSizeRatio);
    }

    /// <summary>Provides the OnUnloaded member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Dispose();
    }
}
