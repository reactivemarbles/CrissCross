// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Presents an inline alarm with reactive acknowledge and close actions.</summary>
public class AlarmBanner : ContentView
{
    /// <summary>Bindable property for <see cref="IsActive"/>.</summary>
    public static readonly BindableProperty IsActiveProperty = CreateProperty(nameof(IsActive), true);

    /// <summary>Bindable property for <see cref="IsClosable"/>.</summary>
    public static readonly BindableProperty IsClosableProperty = CreateProperty(nameof(IsClosable), true);

    /// <summary>Bindable property for <see cref="Message"/>.</summary>
    public static readonly BindableProperty MessageProperty = CreateProperty(nameof(Message), string.Empty);

    /// <summary>Bindable property for <see cref="Severity"/>.</summary>
    public static readonly BindableProperty SeverityProperty = CreateProperty(nameof(Severity), InfoBarSeverity.Error);

    /// <summary>Bindable property for <see cref="AcknowledgeText"/>.</summary>
    public static readonly BindableProperty AcknowledgeTextProperty = CreateProperty(nameof(AcknowledgeText), "Acknowledge");

    /// <summary>Bindable property for <see cref="IsAcknowledgeVisible"/>.</summary>
    public static readonly BindableProperty IsAcknowledgeVisibleProperty = CreateProperty(nameof(IsAcknowledgeVisible), true);

    /// <summary>Bindable property for <see cref="AcknowledgeCommand"/>.</summary>
    public static readonly BindableProperty AcknowledgeCommandProperty = CreateProperty<ICommand?>(nameof(AcknowledgeCommand), null);

    /// <summary>Bindable property for <see cref="CloseCommand"/>.</summary>
    public static readonly BindableProperty CloseCommandProperty = CreateProperty<ICommand?>(nameof(CloseCommand), null);

    /// <summary>Spacing between inline actions.</summary>
    private const double ActionSpacing = 8;

    /// <summary>Displays the alarm message.</summary>
    private readonly Label _message = new();

    /// <summary>Hosts the acknowledgement action.</summary>
    private readonly Button _acknowledge = new();

    /// <summary>Hosts the close action.</summary>
    private readonly Button _close = new() { Text = "Dismiss" };

    /// <summary>Initializes a new instance of the <see cref="AlarmBanner"/> class.</summary>
    public AlarmBanner()
    {
        var actions = new HorizontalStackLayout { Spacing = ActionSpacing, Children = { _acknowledge, _close } };
        Content = new Grid { ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)], Children = { _message, actions } };
        Grid.SetColumn(actions, 1);
        AcknowledgeCommand = ReactiveCommand.Create(Dismiss);
        CloseCommand = ReactiveCommand.Create(Dismiss);
        Refresh();
    }

    /// <summary>Gets or sets a value indicating whether the banner is visible.</summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the close action is visible.</summary>
    public bool IsClosable
    {
        get => (bool)GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>Gets or sets the alarm message.</summary>
    public string? Message
    {
        get => (string?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Gets or sets the semantic severity.</summary>
    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>Gets or sets the acknowledgement label.</summary>
    public string? AcknowledgeText
    {
        get => (string?)GetValue(AcknowledgeTextProperty);
        set => SetValue(AcknowledgeTextProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether acknowledgement is available.</summary>
    public bool IsAcknowledgeVisible
    {
        get => (bool)GetValue(IsAcknowledgeVisibleProperty);
        set => SetValue(IsAcknowledgeVisibleProperty, value);
    }

    /// <summary>Gets or sets the command that acknowledges the alarm.</summary>
    public ICommand? AcknowledgeCommand
    {
        get => (ICommand?)GetValue(AcknowledgeCommandProperty);
        set => SetValue(AcknowledgeCommandProperty, value);
    }

    /// <summary>Gets or sets the command that dismisses the alarm.</summary>
    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    /// <summary>Creates a bindable property that refreshes the composed native view.</summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="name">The public property name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The bindable property definition.</returns>
    private static BindableProperty CreateProperty<T>(string name, T defaultValue) => BindableProperty.Create(
        name,
        typeof(T),
        typeof(AlarmBanner),
        defaultValue,
        propertyChanged: static (view, _, _) => ((AlarmBanner)view).Refresh());

    /// <summary>Updates the visual and accessibility state from the bindable snapshot.</summary>
    private void Refresh()
    {
        IsVisible = IsActive;
        _message.Text = Message;
        _acknowledge.Text = AcknowledgeText;
        _acknowledge.Command = AcknowledgeCommand;
        _acknowledge.IsVisible = IsAcknowledgeVisible && AcknowledgeCommand is not null;
        _close.Command = CloseCommand;
        _close.IsVisible = IsClosable && CloseCommand is not null;
        SemanticProperties.SetDescription(this, $"{Severity}: {Message}");
    }

    /// <summary>Completes the default reactive banner action.</summary>
    private void Dismiss() => IsActive = false;
}
