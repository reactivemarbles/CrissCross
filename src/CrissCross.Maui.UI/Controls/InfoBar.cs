// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Presents a concise themed status message with an optional reactive action.</summary>
public class InfoBar : ContentView
{
    /// <summary>Bindable property for <see cref="Title"/>.</summary>
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(InfoBar),
        propertyChanged: static (view, _, _) => ((InfoBar)view).Refresh());

    /// <summary>Bindable property for <see cref="Message"/>.</summary>
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message),
        typeof(string),
        typeof(InfoBar),
        propertyChanged: static (view, _, _) => ((InfoBar)view).Refresh());

    /// <summary>Bindable property for <see cref="Severity"/>.</summary>
    public static readonly BindableProperty SeverityProperty = BindableProperty.Create(
        nameof(Severity),
        typeof(InfoBarSeverity),
        typeof(InfoBar),
        InfoBarSeverity.Informational,
        propertyChanged: static (view, _, _) => ((InfoBar)view).Refresh());

    /// <summary>Bindable property for <see cref="ActionText"/>.</summary>
    public static readonly BindableProperty ActionTextProperty = BindableProperty.Create(
        nameof(ActionText),
        typeof(string),
        typeof(InfoBar),
        propertyChanged: static (view, _, _) => ((InfoBar)view).Refresh());

    /// <summary>Bindable property for <see cref="ActionCommand"/>.</summary>
    public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(
        nameof(ActionCommand),
        typeof(ICommand),
        typeof(InfoBar),
        propertyChanged: static (view, _, _) => ((InfoBar)view).Refresh());

    /// <summary>Spacing between the title and message.</summary>
    private const double TextSpacing = 2;

    /// <summary>Hosts the optional action.</summary>
    private readonly Button _action = new();

    /// <summary>Displays the status detail.</summary>
    private readonly Label _message = new();

    /// <summary>Displays the optional status heading.</summary>
    private readonly Label _title = new() { FontAttributes = FontAttributes.Bold };

    /// <summary>Initializes a new instance of the <see cref="InfoBar"/> class.</summary>
    public InfoBar()
    {
        var text = new VerticalStackLayout { Spacing = TextSpacing, Children = { _title, _message } };
        Content = new Grid { ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)], Children = { text, _action }, };
        Grid.SetColumn(_action, 1);
        Refresh();
    }

    /// <summary>Gets or sets the optional status heading.</summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the status detail.</summary>
    public string? Message
    {
        get => (string?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Gets or sets the semantic severity used by the current theme.</summary>
    public InfoBarSeverity Severity
    {
        get => (InfoBarSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>Gets or sets optional action text.</summary>
    public string? ActionText
    {
        get => (string?)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    /// <summary>Gets or sets the optional action command.</summary>
    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    /// <summary>Updates the visual children from the current bindable-property snapshot.</summary>
    private void Refresh()
    {
        _title.Text = Title;
        _title.IsVisible = !string.IsNullOrWhiteSpace(Title);
        _message.Text = Message;
        _action.Text = ActionText;
        _action.Command = ActionCommand;
        _action.IsVisible = !string.IsNullOrWhiteSpace(ActionText) && ActionCommand is not null;
        SemanticProperties.SetDescription(this, $"{Severity}: {Title} {Message}".Trim());
    }
}
