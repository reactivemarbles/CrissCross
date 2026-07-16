// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using CrissCross.WPF.UI.Controls.BBCode;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Displays lightweight, theme-aware rich content expressed as BBCode.</summary>
[ContentProperty(nameof(BBCode))]
public class BBCodeBlock : TextBlock, ICommandSource
{
    /// <summary>Identifies the <see cref="BBCode"/> dependency property.</summary>
    public static readonly DependencyProperty BBCodeProperty = DependencyProperty.Register(
        nameof(BBCode),
        typeof(string),
        typeof(BBCodeBlock),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, OnBBCodeChanged));

    /// <summary>Identifies the <see cref="Command"/> dependency property.</summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(BBCodeBlock));

    /// <summary>Identifies the <see cref="CommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter),
        typeof(object),
        typeof(BBCodeBlock));

    /// <summary>Identifies the <see cref="CommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
        nameof(CommandTarget),
        typeof(IInputElement),
        typeof(BBCodeBlock));

    /// <summary>Identifies the <see cref="OpenExternalLinks"/> dependency property.</summary>
    public static readonly DependencyProperty OpenExternalLinksProperty = DependencyProperty.Register(
        nameof(OpenExternalLinks),
        typeof(bool),
        typeof(BBCodeBlock),
        new PropertyMetadata(true));

    /// <summary>The length of the command URI scheme and separator.</summary>
    private const int CommandPrefixLength = 4;

    /// <summary>Initializes static members of the <see cref="BBCodeBlock"/> class.</summary>
    static BBCodeBlock() => DefaultStyleKeyProperty.OverrideMetadata(
        typeof(BBCodeBlock),
        new FrameworkPropertyMetadata(typeof(BBCodeBlock)));

    /// <summary>Initializes a new instance of the <see cref="BBCodeBlock"/> class.</summary>
    public BBCodeBlock() => AddHandler(
        Hyperlink.RequestNavigateEvent,
        new RequestNavigateEventHandler(OnRequestNavigate));

    /// <summary>Occurs when an allowed external link cannot be opened.</summary>
    public event EventHandler<BBCodeNavigationFailedEventArgs>? NavigationFailed;

    /// <summary>Gets or sets the BBCode source.</summary>
    /// <value>The BBCode source to render.</value>
    public string BBCode
    {
        get => (string)GetValue(BBCodeProperty);
        set => SetValue(BBCodeProperty, value);
    }

    /// <summary>Gets or sets the command invoked by a <c>cmd:</c> hyperlink.</summary>
    /// <value>The hyperlink command.</value>
    [Bindable(true)]
    [Category("Action")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Gets or sets the parameter passed to <see cref="Command"/>.</summary>
    /// <value>The most recent <c>cmd:</c> link payload.</value>
    [Bindable(true)]
    [Category("Action")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Gets or sets the target for a routed <see cref="Command"/>.</summary>
    /// <value>The routed command target.</value>
    [Bindable(true)]
    [Category("Action")]
    public IInputElement? CommandTarget
    {
        get => (IInputElement?)GetValue(CommandTargetProperty);
        set => SetValue(CommandTargetProperty, value);
    }

    /// <summary>Gets or sets whether HTTP, HTTPS, and mail links may open through the shell.</summary>
    /// <value><see langword="true"/> to open allowed external links; otherwise, <see langword="false"/>.</value>
    public bool OpenExternalLinks
    {
        get => (bool)GetValue(OpenExternalLinksProperty);
        set => SetValue(OpenExternalLinksProperty, value);
    }

    /// <summary>Updates rendered content when the source property changes.</summary>
    /// <param name="dependencyObject">The changed control.</param>
    /// <param name="eventArgs">The property change.</param>
    private static void OnBBCodeChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs eventArgs) =>
        ((BBCodeBlock)dependencyObject).UpdateContent((string?)eventArgs.NewValue);

    /// <summary>Handles navigation from rendered hyperlinks.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="eventArgs">The navigation request.</param>
    private void OnRequestNavigate(object sender, RequestNavigateEventArgs eventArgs)
    {
        eventArgs.Handled = true;
        var address = eventArgs.Uri.OriginalString;
        if (eventArgs.Uri.Scheme.Equals("cmd", StringComparison.OrdinalIgnoreCase))
        {
            ExecuteCommand(address.Length > CommandPrefixLength ? address[CommandPrefixLength..] : string.Empty);
            return;
        }

        if (!OpenExternalLinks || eventArgs.Uri.Scheme is not ("http" or "https" or "mailto"))
        {
            return;
        }

        try
        {
            _ = Process.Start(new ProcessStartInfo(address) { UseShellExecute = true });
        }
        catch (Win32Exception exception)
        {
            NavigationFailed?.Invoke(this, new BBCodeNavigationFailedEventArgs(eventArgs.Uri, exception));
        }
        catch (InvalidOperationException exception)
        {
            NavigationFailed?.Invoke(this, new BBCodeNavigationFailedEventArgs(eventArgs.Uri, exception));
        }
    }

    /// <summary>Executes the command attached to the control.</summary>
    /// <param name="parameter">The command-link payload.</param>
    private void ExecuteCommand(string parameter)
    {
        CommandParameter = parameter;
        if (Command is RoutedCommand routedCommand)
        {
            var target = CommandTarget ?? this;
            if (routedCommand.CanExecute(parameter, target))
            {
                routedCommand.Execute(parameter, target);
            }

            return;
        }

        if (Command?.CanExecute(parameter) != true)
        {
            return;
        }

        Command.Execute(parameter);
    }

    /// <summary>Parses and renders new BBCode content.</summary>
    /// <param name="source">The new BBCode source.</param>
    private void UpdateContent(string? source)
    {
        Inlines.Clear();
        if (string.IsNullOrEmpty(source))
        {
            return;
        }

        var document = new BbCodeParser(source).Parse();
        Inlines.Add(new BbCodeRenderer(this).Render(document));
    }
}
