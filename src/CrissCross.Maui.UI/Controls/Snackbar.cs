// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Presents a transient, closable feedback message through native MAUI composition.</summary>
public class Snackbar : ContentView
{
    /// <summary>Bindable property for <see cref="IsShown"/>.</summary>
    public static readonly BindableProperty IsShownProperty = CreateProperty(nameof(IsShown), false);

    /// <summary>Bindable property for <see cref="IsCloseButtonEnabled"/>.</summary>
    public static readonly BindableProperty IsCloseButtonEnabledProperty = CreateProperty(nameof(IsCloseButtonEnabled), true);

    /// <summary>Bindable property for <see cref="Title"/>.</summary>
    public static readonly BindableProperty TitleProperty = CreateProperty(nameof(Title), string.Empty);

    /// <summary>Bindable property for <see cref="Message"/>.</summary>
    public static readonly BindableProperty MessageProperty = CreateProperty(nameof(Message), string.Empty);

    /// <summary>Bindable property for <see cref="CloseCommand"/>.</summary>
    public static readonly BindableProperty CloseCommandProperty = CreateProperty<ICommand?>(nameof(CloseCommand), null);

    /// <summary>Spacing between title and message.</summary>
    private const double TextSpacing = 2;

    /// <summary>Displays the optional title.</summary>
    private readonly Label _title = new() { FontAttributes = FontAttributes.Bold };

    /// <summary>Displays the notification message.</summary>
    private readonly Label _message = new();

    /// <summary>Hosts the close action.</summary>
    private readonly Button _close = new() { Text = "Dismiss" };

    /// <summary>Initializes a new instance of the <see cref="Snackbar"/> class.</summary>
    public Snackbar()
    {
        var text = new VerticalStackLayout { Spacing = TextSpacing, Children = { _title, _message } };
        Content = new Grid { ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)], Children = { text, _close } };
        Grid.SetColumn(_close, 1);
        CloseCommand = ReactiveCommand.Create(Hide);
        Refresh();
    }

    /// <summary>Gets or sets a value indicating whether the notification is visible.</summary>
    public bool IsShown
    {
        get => (bool)GetValue(IsShownProperty);
        set => SetValue(IsShownProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the close button is visible.</summary>
    public bool IsCloseButtonEnabled
    {
        get => (bool)GetValue(IsCloseButtonEnabledProperty);
        set => SetValue(IsCloseButtonEnabledProperty, value);
    }

    /// <summary>Gets or sets the optional notification title.</summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the notification message.</summary>
    public string? Message
    {
        get => (string?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>Gets or sets the reactive command that dismisses the notification.</summary>
    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    /// <summary>Makes the snackbar visible.</summary>
    public void Show() => IsShown = true;

    /// <summary>Creates a bindable property that refreshes the native composition.</summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="name">The public property name.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The bindable property definition.</returns>
    private static BindableProperty CreateProperty<T>(string name, T defaultValue) => BindableProperty.Create(
        name,
        typeof(T),
        typeof(Snackbar),
        defaultValue,
        propertyChanged: static (view, _, _) => ((Snackbar)view).Refresh());

    /// <summary>Updates the visible and accessible snapshot.</summary>
    private void Refresh()
    {
        IsVisible = IsShown;
        _title.Text = Title;
        _title.IsVisible = !string.IsNullOrWhiteSpace(Title);
        _message.Text = Message;
        _close.Command = CloseCommand;
        _close.IsVisible = IsCloseButtonEnabled && CloseCommand is not null;
        SemanticProperties.SetDescription(this, $"{Title}: {Message}");
    }

    /// <summary>Completes the default reactive close action.</summary>
    private void Hide() => IsShown = false;
}
