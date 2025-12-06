// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Dialogue displayed inside the application covering its internals, displaying some content.
/// </summary>
public class ContentDialog : ContentControl
{
    /// <summary>
    /// Property for <see cref="Title"/>.
    /// </summary>
    public static readonly StyledProperty<object?> TitleProperty = AvaloniaProperty.Register<ContentDialog, object?>(
        nameof(Title), null);

    /// <summary>
    /// Property for <see cref="TitleTemplate"/>.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> TitleTemplateProperty = AvaloniaProperty.Register<ContentDialog, IDataTemplate?>(
        nameof(TitleTemplate), null);

    /// <summary>
    /// Property for <see cref="PrimaryButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> PrimaryButtonTextProperty = AvaloniaProperty.Register<ContentDialog, string>(
        nameof(PrimaryButtonText), string.Empty);

    /// <summary>
    /// Property for <see cref="SecondaryButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> SecondaryButtonTextProperty = AvaloniaProperty.Register<ContentDialog, string>(
        nameof(SecondaryButtonText), string.Empty);

    /// <summary>
    /// Property for <see cref="CloseButtonText"/>.
    /// </summary>
    public static readonly StyledProperty<string> CloseButtonTextProperty = AvaloniaProperty.Register<ContentDialog, string>(
        nameof(CloseButtonText), "Close");

    /// <summary>
    /// Property for <see cref="PrimaryButtonCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> PrimaryButtonCommandProperty = AvaloniaProperty.Register<ContentDialog, ICommand?>(
        nameof(PrimaryButtonCommand), null);

    /// <summary>
    /// Property for <see cref="SecondaryButtonCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> SecondaryButtonCommandProperty = AvaloniaProperty.Register<ContentDialog, ICommand?>(
        nameof(SecondaryButtonCommand), null);

    /// <summary>
    /// Property for <see cref="CloseButtonCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> CloseButtonCommandProperty = AvaloniaProperty.Register<ContentDialog, ICommand?>(
        nameof(CloseButtonCommand), null);

    /// <summary>
    /// Property for <see cref="IsPrimaryButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsPrimaryButtonEnabledProperty = AvaloniaProperty.Register<ContentDialog, bool>(
        nameof(IsPrimaryButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="IsSecondaryButtonEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsSecondaryButtonEnabledProperty = AvaloniaProperty.Register<ContentDialog, bool>(
        nameof(IsSecondaryButtonEnabled), true);

    /// <summary>
    /// Property for <see cref="DefaultButton"/>.
    /// </summary>
    public static readonly StyledProperty<ContentDialogButton> DefaultButtonProperty = AvaloniaProperty.Register<ContentDialog, ContentDialogButton>(
        nameof(DefaultButton), ContentDialogButton.Primary);

    /// <summary>
    /// Property for <see cref="PrimaryButtonAppearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> PrimaryButtonAppearanceProperty = AvaloniaProperty.Register<ContentDialog, ControlAppearance>(
        nameof(PrimaryButtonAppearance), ControlAppearance.Primary);

    /// <summary>
    /// Property for <see cref="SecondaryButtonAppearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> SecondaryButtonAppearanceProperty = AvaloniaProperty.Register<ContentDialog, ControlAppearance>(
        nameof(SecondaryButtonAppearance), ControlAppearance.Secondary);

    /// <summary>
    /// Property for <see cref="CloseButtonAppearance"/>.
    /// </summary>
    public static readonly StyledProperty<ControlAppearance> CloseButtonAppearanceProperty = AvaloniaProperty.Register<ContentDialog, ControlAppearance>(
        nameof(CloseButtonAppearance), ControlAppearance.Secondary);

    /// <summary>
    /// Property for <see cref="DialogWidth"/>.
    /// </summary>
    public static readonly StyledProperty<double> DialogWidthProperty = AvaloniaProperty.Register<ContentDialog, double>(
        nameof(DialogWidth), double.PositiveInfinity);

    /// <summary>
    /// Property for <see cref="DialogHeight"/>.
    /// </summary>
    public static readonly StyledProperty<double> DialogHeightProperty = AvaloniaProperty.Register<ContentDialog, double>(
        nameof(DialogHeight), double.PositiveInfinity);

    /// <summary>
    /// Property for <see cref="DialogMaxWidth"/>.
    /// </summary>
    public static readonly StyledProperty<double> DialogMaxWidthProperty = AvaloniaProperty.Register<ContentDialog, double>(
        nameof(DialogMaxWidth), double.PositiveInfinity);

    /// <summary>
    /// Property for <see cref="DialogMaxHeight"/>.
    /// </summary>
    public static readonly StyledProperty<double> DialogMaxHeightProperty = AvaloniaProperty.Register<ContentDialog, double>(
        nameof(DialogMaxHeight), double.PositiveInfinity);

    /// <summary>
    /// Property for <see cref="DialogMargin"/>.
    /// </summary>
    public static readonly StyledProperty<Thickness> DialogMarginProperty = AvaloniaProperty.Register<ContentDialog, Thickness>(
        nameof(DialogMargin), default);

    /// <summary>
    /// Property for <see cref="IsFooterVisible"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsFooterVisibleProperty = AvaloniaProperty.Register<ContentDialog, bool>(
        nameof(IsFooterVisible), true);

    /// <summary>
    /// Routed event for <see cref="Opened"/>.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<ContentDialog, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

    /// <summary>
    /// Routed event for <see cref="Closing"/>.
    /// </summary>
    public static readonly RoutedEvent<ContentDialogClosingEventArgs> ClosingEvent =
        RoutedEvent.Register<ContentDialog, ContentDialogClosingEventArgs>(nameof(Closing), RoutingStrategies.Bubble);

    /// <summary>
    /// Routed event for <see cref="Closed"/>.
    /// </summary>
    public static readonly RoutedEvent<ContentDialogClosedEventArgs> ClosedEvent =
        RoutedEvent.Register<ContentDialog, ContentDialogClosedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    /// <summary>
    /// Routed event for <see cref="ButtonClicked"/>.
    /// </summary>
    public static readonly RoutedEvent<ContentDialogButtonClickEventArgs> ButtonClickedEvent =
        RoutedEvent.Register<ContentDialog, ContentDialogButtonClickEventArgs>(nameof(ButtonClicked), RoutingStrategies.Bubble);

    private TaskCompletionSource<ContentDialogResult>? _tcs;

    /// <summary>
    /// Occurs after the dialog is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    /// <summary>
    /// Occurs after the dialog starts to close, but before it is closed.
    /// </summary>
    public event EventHandler<ContentDialogClosingEventArgs>? Closing
    {
        add => AddHandler(ClosingEvent, value);
        remove => RemoveHandler(ClosingEvent, value);
    }

    /// <summary>
    /// Occurs after the dialog is closed.
    /// </summary>
    public event EventHandler<ContentDialogClosedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    /// <summary>
    /// Occurs after a button is clicked.
    /// </summary>
    public event EventHandler<ContentDialogButtonClickEventArgs>? ButtonClicked
    {
        add => AddHandler(ButtonClickedEvent, value);
        remove => RemoveHandler(ButtonClickedEvent, value);
    }

    /// <summary>
    /// Gets or sets the title of the <see cref="ContentDialog"/>.
    /// </summary>
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the title template.
    /// </summary>
    public IDataTemplate? TitleTemplate
    {
        get => GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the primary button.
    /// </summary>
    public string PrimaryButtonText
    {
        get => GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to be displayed on the secondary button.
    /// </summary>
    public string SecondaryButtonText
    {
        get => GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text to display on the close button.
    /// </summary>
    public string CloseButtonText
    {
        get => GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the command for the primary button.
    /// </summary>
    public ICommand? PrimaryButtonCommand
    {
        get => GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command for the secondary button.
    /// </summary>
    public ICommand? SecondaryButtonCommand
    {
        get => GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command for the close button.
    /// </summary>
    public ICommand? CloseButtonCommand
    {
        get => GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the primary button is enabled.
    /// </summary>
    public bool IsPrimaryButtonEnabled
    {
        get => GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the secondary button is enabled.
    /// </summary>
    public bool IsSecondaryButtonEnabled
    {
        get => GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates which button on the dialog is the default action.
    /// </summary>
    public ContentDialogButton DefaultButton
    {
        get => GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance of the primary button.
    /// </summary>
    public ControlAppearance PrimaryButtonAppearance
    {
        get => GetValue(PrimaryButtonAppearanceProperty);
        set => SetValue(PrimaryButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance of the secondary button.
    /// </summary>
    public ControlAppearance SecondaryButtonAppearance
    {
        get => GetValue(SecondaryButtonAppearanceProperty);
        set => SetValue(SecondaryButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the appearance of the close button.
    /// </summary>
    public ControlAppearance CloseButtonAppearance
    {
        get => GetValue(CloseButtonAppearanceProperty);
        set => SetValue(CloseButtonAppearanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the dialog.
    /// </summary>
    public double DialogWidth
    {
        get => GetValue(DialogWidthProperty);
        set => SetValue(DialogWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the dialog.
    /// </summary>
    public double DialogHeight
    {
        get => GetValue(DialogHeightProperty);
        set => SetValue(DialogHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the max width of the dialog.
    /// </summary>
    public double DialogMaxWidth
    {
        get => GetValue(DialogMaxWidthProperty);
        set => SetValue(DialogMaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the max height of the dialog.
    /// </summary>
    public double DialogMaxHeight
    {
        get => GetValue(DialogMaxHeightProperty);
        set => SetValue(DialogMaxHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin of the dialog.
    /// </summary>
    public Thickness DialogMargin
    {
        get => GetValue(DialogMarginProperty);
        set => SetValue(DialogMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the footer is visible.
    /// </summary>
    public bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the content presenter for displaying the dialog.
    /// </summary>
    public ContentPresenter? ContentPresenter { get; set; }

    /// <summary>
    /// Gets or sets the content presenter for displaying the dialog.
    /// </summary>
    [Obsolete("Use ContentPresenter instead.")]
    public ContentPresenter? DialogPresenter
    {
        get => ContentPresenter;
        set => ContentPresenter = value;
    }

    /// <summary>
    /// Shows the dialog asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ContentDialogResult.</returns>
    public async Task<ContentDialogResult> ShowAsync(CancellationToken cancellationToken = default)
    {
        if (ContentPresenter == null)
        {
            throw new InvalidOperationException("ContentPresenter is not set");
        }

        _tcs = new TaskCompletionSource<ContentDialogResult>();
        var tokenRegistration = cancellationToken.Register(
            o => _tcs.TrySetCanceled((CancellationToken)o!),
            cancellationToken);

        var result = ContentDialogResult.None;

        try
        {
            ContentPresenter.Content = this;
            RaiseEvent(new RoutedEventArgs(OpenedEvent));
            result = await _tcs.Task;
            return result;
        }
        finally
        {
            await tokenRegistration.DisposeAsync();
            ContentPresenter.Content = null;
            OnClosed(result);
        }
    }

    /// <summary>
    /// Hides the dialog with result.
    /// </summary>
    /// <param name="result">The result.</param>
    public virtual void Hide(ContentDialogResult result = ContentDialogResult.None)
    {
        var closingEventArgs = new ContentDialogClosingEventArgs(result);
        RaiseEvent(closingEventArgs);

        if (!closingEventArgs.Cancel)
        {
            _tcs?.TrySetResult(result);
        }
    }

    /// <summary>
    /// Called when a button is clicked.
    /// </summary>
    /// <param name="button">The button that was clicked.</param>
    protected virtual void OnButtonClick(ContentDialogButton button)
    {
        var buttonClickEventArgs = new ContentDialogButtonClickEventArgs(button);
        RaiseEvent(buttonClickEventArgs);

        var result = button switch
        {
            ContentDialogButton.Primary => ContentDialogResult.Primary,
            ContentDialogButton.Secondary => ContentDialogResult.Secondary,
            _ => ContentDialogResult.None
        };

        Hide(result);
    }

    /// <summary>
    /// Called after the dialog is closed.
    /// </summary>
    /// <param name="result">The result.</param>
    protected virtual void OnClosed(ContentDialogResult result)
    {
        var closedEventArgs = new ContentDialogClosedEventArgs(result);
        RaiseEvent(closedEventArgs);
    }
}
