// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// AlarmBanner is an inline notification optimized for industrial/commercial alarm messages.
/// It reuses the InfoBar visual language and supports acknowledgement and close actions.
/// </summary>
public class AlarmBanner : System.Windows.Controls.ContentControl
{
    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(AlarmBanner),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsClosable"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register(
        nameof(IsClosable),
        typeof(bool),
        typeof(AlarmBanner),
        new PropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="Message"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(AlarmBanner),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="Severity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register(
        nameof(Severity),
        typeof(InfoBarSeverity),
        typeof(AlarmBanner),
        new PropertyMetadata(InfoBarSeverity.Error));

    /// <summary>
    /// Identifies the <see cref="AcknowledgeText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AcknowledgeTextProperty = DependencyProperty.Register(
        nameof(AcknowledgeText),
        typeof(string),
        typeof(AlarmBanner),
        new PropertyMetadata("Acknowledge"));

    /// <summary>
    /// Identifies the <see cref="IsAcknowledgeVisible"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAcknowledgeVisibleProperty = DependencyProperty.Register(
        nameof(IsAcknowledgeVisible),
        typeof(bool),
        typeof(AlarmBanner),
        new PropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="AcknowledgeCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AcknowledgeCommandProperty = DependencyProperty.Register(
        nameof(AcknowledgeCommand),
        typeof(IReactiveCommand),
        typeof(AlarmBanner),
        new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="CloseCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        nameof(CloseCommand),
        typeof(IReactiveCommand),
        typeof(AlarmBanner),
        new PropertyMetadata(null));

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
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user can close the banner.
    /// </summary>
    public bool IsClosable
    {
        get => (bool)GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets the text message displayed by the banner.
    /// </summary>
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the severity for the banner.
    /// </summary>
    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets the label for the acknowledge button.
    /// </summary>
    public string AcknowledgeText
    {
        get => (string)GetValue(AcknowledgeTextProperty);
        set => SetValue(AcknowledgeTextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the acknowledge button is visible.
    /// </summary>
    public bool IsAcknowledgeVisible
    {
        get => (bool)GetValue(IsAcknowledgeVisibleProperty);
        set => SetValue(IsAcknowledgeVisibleProperty, value);
    }

    /// <summary>
    /// Gets the acknowledge command.
    /// </summary>
    public IReactiveCommand AcknowledgeCommand => (IReactiveCommand)GetValue(AcknowledgeCommandProperty);

    /// <summary>
    /// Gets the close command.
    /// </summary>
    public IReactiveCommand CloseCommand => (IReactiveCommand)GetValue(CloseCommandProperty);
}
