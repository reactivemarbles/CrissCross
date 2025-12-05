// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an info bar control for displaying messages.
/// </summary>
public class InfoBar : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<InfoBar, string?>(nameof(Title));

    /// <summary>
    /// Property for <see cref="Message"/>.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<InfoBar, string?>(nameof(Message));

    /// <summary>
    /// Property for <see cref="IsOpen"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsOpen), true);

    /// <summary>
    /// Property for <see cref="IsClosable"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsClosable), true);

    /// <summary>
    /// Property for <see cref="Severity"/>.
    /// </summary>
    public static readonly StyledProperty<InfoBarSeverity> SeverityProperty =
        AvaloniaProperty.Register<InfoBar, InfoBarSeverity>(nameof(Severity), InfoBarSeverity.Informational);

    /// <summary>
    /// Property for <see cref="IsIconVisible"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsIconVisible), true);

    /// <summary>
    /// Gets or sets the title of the info bar.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the message of the info bar.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the info bar is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the info bar is closable.
    /// </summary>
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    /// <summary>
    /// Gets or sets the severity of the info bar.
    /// </summary>
    public InfoBarSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the icon is visible.
    /// </summary>
    public bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    /// <summary>
    /// Closes the info bar.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }
}
