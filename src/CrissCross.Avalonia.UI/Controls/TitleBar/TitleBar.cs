// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Custom navigation buttons for the window.
/// </summary>
public class TitleBar : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<TitleBar, string>(
        nameof(Title), string.Empty);

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<TitleBar, object>(
        nameof(Header), null);

    /// <summary>
    /// Property for <see cref="ButtonsForeground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> ButtonsForegroundProperty = AvaloniaProperty.Register<TitleBar, IBrush>(
        nameof(ButtonsForeground), Brushes.Black);

    /// <summary>
    /// Property for <see cref="ButtonsBackground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> ButtonsBackgroundProperty = AvaloniaProperty.Register<TitleBar, IBrush>(
        nameof(ButtonsBackground), Brushes.White);

    /// <summary>
    /// Property for <see cref="IsMaximized"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsMaximizedProperty = AvaloniaProperty.Register<TitleBar, bool>(
        nameof(IsMaximized), false);

    /// <summary>
    /// Property for <see cref="ShowMaximize"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMaximizeProperty = AvaloniaProperty.Register<TitleBar, bool>(
        nameof(ShowMaximize), true);

    /// <summary>
    /// Property for <see cref="ShowMinimize"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMinimizeProperty = AvaloniaProperty.Register<TitleBar, bool>(
        nameof(ShowMinimize), true);

    /// <summary>
    /// Property for <see cref="ShowClose"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseProperty = AvaloniaProperty.Register<TitleBar, bool>(
        nameof(ShowClose), true);

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<object> IconProperty = AvaloniaProperty.Register<TitleBar, object>(
        nameof(Icon), null);

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the header.
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the buttons foreground.
    /// </summary>
    public IBrush ButtonsForeground
    {
        get => GetValue(ButtonsForegroundProperty);
        set => SetValue(ButtonsForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the buttons background.
    /// </summary>
    public IBrush ButtonsBackground
    {
        get => GetValue(ButtonsBackgroundProperty);
        set => SetValue(ButtonsBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the window is maximized.
    /// </summary>
    public bool IsMaximized
    {
        get => GetValue(IsMaximizedProperty);
        set => SetValue(IsMaximizedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show maximize button.
    /// </summary>
    public bool ShowMaximize
    {
        get => GetValue(ShowMaximizeProperty);
        set => SetValue(ShowMaximizeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show minimize button.
    /// </summary>
    public bool ShowMinimize
    {
        get => GetValue(ShowMinimizeProperty);
        set => SetValue(ShowMinimizeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show close button.
    /// </summary>
    public bool ShowClose
    {
        get => GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
