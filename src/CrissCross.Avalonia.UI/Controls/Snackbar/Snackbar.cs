// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Snackbar inform user of a process that an app has performed or will perform.
/// </summary>
public class Snackbar : global::Avalonia.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="IsCloseButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsCloseButtonEnabledProperty = AvaloniaProperty.Register<Snackbar, bool>(
        nameof(IsCloseButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="IsShown"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsShownProperty = AvaloniaProperty.Register<Snackbar, bool>(
        nameof(IsShown), false);

    /// <summary>
    /// Property for <see cref="Timeout"/>.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TimeoutProperty = AvaloniaProperty.Register<Snackbar, TimeSpan>(
        nameof(Timeout), TimeSpan.FromSeconds(2));

    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly StyledProperty<object?> TitleProperty = AvaloniaProperty.Register<Snackbar, object?>(
        nameof(Title), null);

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly StyledProperty<IconElement?> IconProperty = AvaloniaProperty.Register<Snackbar, IconElement?>(
        nameof(Icon), null);

    /// <summary>
    /// Property for <see cref="Appearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> AppearanceProperty = AvaloniaProperty.Register<Snackbar, ControlAppearance>(
        nameof(Appearance), ControlAppearance.Secondary);

    /// <summary>
    /// Property for <see cref="ContentForeground"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ContentForegroundProperty = AvaloniaProperty.Register<Snackbar, IBrush?>(
        nameof(ContentForeground), Brushes.Black);

    private SnackbarPresenter? _presenter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Snackbar"/> class.
    /// </summary>
    public Snackbar()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Snackbar"/> class.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    public Snackbar(SnackbarPresenter presenter) => _presenter = presenter;

    /// <summary>
    /// Gets or sets a value indicating whether the close button is enabled.
    /// </summary>
    public bool IsCloseButtonEnabled
    {
        get => GetValue(IsCloseButtonEnabledProperty);
        set => SetValue(IsCloseButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the snackbar is shown.
    /// </summary>
    public bool IsShown
    {
        get => GetValue(IsShownProperty);
        set => SetValue(IsShownProperty, value);
    }

    /// <summary>
    /// Gets or sets the timeout.
    /// </summary>
    public TimeSpan Timeout
    {
        get => GetValue(TimeoutProperty);
        set => SetValue(TimeoutProperty, value);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public IconElement? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance.
    /// </summary>
    public ControlAppearance Appearance
    {
        get => GetValue(AppearanceProperty);
        set => SetValue(AppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the content foreground.
    /// </summary>
    public IBrush? ContentForeground
    {
        get => GetValue(ContentForegroundProperty);
        set => SetValue(ContentForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the snackbar presenter.
    /// </summary>
    protected SnackbarPresenter? Presenter { get; set; }

    /// <summary>
    /// Shows the snackbar.
    /// </summary>
    /// <param name="immediately">if set to <c>true</c> shows immediately.</param>
    public virtual void Show(bool immediately = false)
    {
        if (_presenter is null)
        {
            throw new InvalidOperationException("SnackbarPresenter is not set.");
        }

        if (immediately)
        {
            _ = _presenter.ImmediatelyDisplayAsync(this);
        }
        else
        {
            _presenter.AddToQueue(this);
        }
    }

    /// <summary>
    /// Hides the snackbar asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task HideAsync()
    {
        if (_presenter is null)
        {
            return;
        }

        await _presenter.HideCurrentAsync();
    }
}
