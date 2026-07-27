// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.UI.Controls.BBCode;
#else
using CrissCross.Avalonia.UI.Controls.BBCode;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Displays lightweight, theme-aware rich content expressed as BBCode.</summary>
public class BBCodeBlock : TextBlock
{
    /// <summary>Identifies the <see cref="BBCode"/> styled property.</summary>
    public static readonly StyledProperty<string> BBCodeProperty = AvaloniaProperty.Register<BBCodeBlock, string>(nameof(BBCode), string.Empty);

    /// <summary>Identifies the <see cref="Command"/> styled property.</summary>
    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<BBCodeBlock, ICommand?>(nameof(Command));

    /// <summary>Identifies the <see cref="CommandParameter"/> styled property.</summary>
    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<BBCodeBlock, object?>(nameof(CommandParameter));

    /// <summary>Identifies the <see cref="OpenExternalLinks"/> styled property.</summary>
    public static readonly StyledProperty<bool> OpenExternalLinksProperty = AvaloniaProperty.Register<BBCodeBlock, bool>(nameof(OpenExternalLinks), true);

    /// <summary>Occurs when an allowed external BBCode link is selected.</summary>
    public event EventHandler<BBCodeLinkRequestedEventArgs>? ExternalLinkRequested;

    /// <summary>Gets or sets the BBCode source.</summary>
    public string BBCode
    {
        get => GetValue(BBCodeProperty);
        set => SetValue(BBCodeProperty, value);
    }

    /// <summary>Gets or sets the command invoked by a <c>cmd:</c> hyperlink.</summary>
    [Bindable(true)]
    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Gets or sets the parameter passed to <see cref="Command"/>.</summary>
    [Bindable(true)]
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Gets or sets whether HTTP, HTTPS, and mail links may open through the shell.</summary>
    public bool OpenExternalLinks
    {
        get => GetValue(OpenExternalLinksProperty);
        set => SetValue(OpenExternalLinksProperty, value);
    }

    /// <summary>Handles a parsed BBCode URI.</summary>
    /// <param name="uri">The selected URI.</param>
    public void Navigate(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (uri.Scheme.Equals("cmd", StringComparison.OrdinalIgnoreCase))
        {
            ExecuteCommand(uri.OriginalString.Length > "cmd:".Length ? uri.OriginalString["cmd:".Length..] : string.Empty);
            return;
        }

        if (!OpenExternalLinks || uri.Scheme is not ("http" or "https" or "mailto"))
        {
            return;
        }

        ExternalLinkRequested?.Invoke(this, new BBCodeLinkRequestedEventArgs(uri));
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != BBCodeProperty)
        {
            return;
        }

        UpdateContent(change.GetNewValue<string>() ?? string.Empty);
    }

    /// <summary>Executes the configured command.</summary>
    /// <param name="parameter">The command-link payload.</param>
    private void ExecuteCommand(string parameter)
    {
        var command = Command;
        CommandParameter = parameter;
        if (command?.CanExecute(parameter) != true)
        {
            return;
        }

        command.Execute(parameter);
    }

    /// <summary>Parses and renders BBCode content.</summary>
    /// <param name="source">The source to render.</param>
    private void UpdateContent(string? source)
    {
        Inlines?.Clear();
        if (string.IsNullOrEmpty(source))
        {
            return;
        }

        Inlines?.Add(new BbCodeRenderer(this).Render(new BbCodeParser(source).Parse()));
    }
}
