// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// AlarmBanner is an inline notification optimized for industrial/commercial alarm messages.
/// It reuses the InfoBar visual language and supports acknowledgement and close actions.
/// </summary>
public class AlarmBanner : global::Avalonia.Controls.ContentControl
{
    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<AlarmBanner, bool>(
        nameof(IsActive));

    /// <summary>
    /// Identifies the <see cref="IsClosable"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty = AvaloniaProperty.Register<AlarmBanner, bool>(
        nameof(IsClosable), true);

    /// <summary>
    /// Identifies the <see cref="Message"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<string> MessageProperty = AvaloniaProperty.Register<AlarmBanner, string>(
        nameof(Message), string.Empty);

    /// <summary>
    /// Identifies the <see cref="Severity"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<InfoBarSeverity> SeverityProperty = AvaloniaProperty.Register<AlarmBanner, InfoBarSeverity>(
        nameof(Severity), InfoBarSeverity.Error);

    /// <summary>
    /// Identifies the <see cref="AcknowledgeText"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<string> AcknowledgeTextProperty = AvaloniaProperty.Register<AlarmBanner, string>(
        nameof(AcknowledgeText), "Acknowledge");

    /// <summary>
    /// Identifies the <see cref="IsAcknowledgeVisible"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAcknowledgeVisibleProperty = AvaloniaProperty.Register<AlarmBanner, bool>(
        nameof(IsAcknowledgeVisible), true);

    /// <summary>
    /// Identifies the <see cref="AcknowledgeCommand"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<IReactiveCommand> AcknowledgeCommandProperty = AvaloniaProperty.Register<AlarmBanner, IReactiveCommand>(
        nameof(AcknowledgeCommand));

    /// <summary>
    /// Identifies the <see cref="CloseCommand"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<IReactiveCommand> CloseCommandProperty = AvaloniaProperty.Register<AlarmBanner, IReactiveCommand>(
        nameof(CloseCommand));

    /// <summary>
    /// Initializes a new instance of the <see cref="AlarmBanner"/> class.
    /// </summary>
    public AlarmBanner()
    {
        SetValue(
            AcknowledgeCommandProperty,
            ReactiveCommand.Create<object>(_ => SetCurrentValue(IsActiveProperty, false)));

        SetValue(
            CloseCommandProperty,
            ReactiveCommand.Create<object>(_ => SetCurrentValue(IsActiveProperty, false)));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the alarm banner is active and visible.
    /// </summary>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user can close the banner.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets the text message displayed by the banner.
    /// </summary>
    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the severity for the banner.
    /// </summary>
    public InfoBarSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets the label for the acknowledge button.
    /// </summary>
    public string AcknowledgeText
    {
        get => GetValue(AcknowledgeTextProperty);
        set => SetValue(AcknowledgeTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the acknowledge button is visible.
    /// </summary>
    public bool IsAcknowledgeVisible
    {
        get => GetValue(IsAcknowledgeVisibleProperty);
        set => SetValue(IsAcknowledgeVisibleProperty, value);
    }

    /// <summary>
    /// Gets the acknowledge command.
    /// </summary>
    public IReactiveCommand AcknowledgeCommand => GetValue(AcknowledgeCommandProperty);

    /// <summary>
    /// Gets the close command.
    /// </summary>
    public IReactiveCommand CloseCommand => GetValue(CloseCommandProperty);
}
