// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// SnackbarPresenter.
/// </summary>
/// <seealso cref="ContentPresenter" />
public class SnackbarPresenter : ContentPresenter
{
    /// <summary>
    /// Property for <see cref="SnackbarContent"/>.
    /// </summary>
    public static readonly StyledProperty<Snackbar?> SnackbarContentProperty =
        AvaloniaProperty.Register<SnackbarPresenter, Snackbar?>(nameof(SnackbarContent));

    /// <summary>
    /// Initializes a new instance of the <see cref="SnackbarPresenter"/> class.
    /// </summary>
    public SnackbarPresenter() => Unloaded += OnUnloadedHandler;

    /// <summary>
    /// Finalizes an instance of the <see cref="SnackbarPresenter"/> class.
    /// </summary>
    ~SnackbarPresenter()
    {
        if (!CancellationTokenSource.IsCancellationRequested)
        {
            CancellationTokenSource.Cancel();
        }

        CancellationTokenSource.Dispose();
    }

    /// <summary>
    /// Gets or sets the snackbar content.
    /// </summary>
    /// <value>
    /// The snackbar content.
    /// </value>
    public Snackbar? SnackbarContent
    {
        get => GetValue(SnackbarContentProperty);
        protected set => SetValue(SnackbarContentProperty, value);
    }

    /// <summary>
    /// Gets the queue.
    /// </summary>
    /// <value>
    /// The queue.
    /// </value>
    protected Queue<Snackbar> Queue { get; } = new();

    /// <summary>
    /// Gets or sets the cancellation token source.
    /// </summary>
    /// <value>
    /// The cancellation token source.
    /// </value>
    protected CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    /// Adds to queue.
    /// </summary>
    /// <param name="snackbar">The snackbar.</param>
    public virtual void AddToQueue(Snackbar snackbar)
    {
        Queue.Enqueue(snackbar);

        if (SnackbarContent is null)
        {
            _ = ShowQueuedSnackbarsAsync();
        }
    }

    /// <summary>
    /// Immediately displays the snackbar.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <param name="snackbar">The snackbar.</param>
    public virtual async Task ImmediatelyDisplayAsync(Snackbar snackbar)
    {
        if (snackbar is null)
        {
            return;
        }

        await HideCurrentAsync();
        await ShowSnackbarAsync(snackbar);

        await ShowQueuedSnackbarsAsync();
    }

    /// <summary>
    /// Hides the current snackbar.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task HideCurrentAsync()
    {
        if (SnackbarContent is null)
        {
            return;
        }

        CancellationTokenSource.Cancel();
        await HideSnackbarAsync(SnackbarContent);
        ResetCancellationTokenSource();
    }

    /// <summary>
    /// Called when [unloaded].
    /// </summary>
    protected virtual void OnUnloaded()
    {
        if (CancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        ImmediatelyHideCurrent();
        ResetCancellationTokenSource();
    }

    /// <summary>
    /// Resets the cancellation token source.
    /// </summary>
    protected void ResetCancellationTokenSource()
    {
        CancellationTokenSource.Dispose();
        CancellationTokenSource = new CancellationTokenSource();
    }

    private static void OnUnloadedHandler(object? sender, RoutedEventArgs e)
    {
        if (sender is SnackbarPresenter self)
        {
            self.OnUnloaded();
        }
    }

    private void ImmediatelyHideCurrent()
    {
        if (SnackbarContent is null)
        {
            return;
        }

        CancellationTokenSource.Cancel();
        ImmediatelyHideSnackbar(SnackbarContent);
    }

    private void ImmediatelyHideSnackbar(Snackbar snackbar)
    {
        snackbar.IsShown = false;
        SnackbarContent = null;
        Content = null;
    }

    private async Task ShowQueuedSnackbarsAsync()
    {
        while (Queue.Count > 0 && !CancellationTokenSource.IsCancellationRequested)
        {
            var snackbar = Queue.Dequeue();

            await ShowSnackbarAsync(snackbar);
        }
    }

    private async Task ShowSnackbarAsync(Snackbar snackbar)
    {
        SnackbarContent = snackbar;
        Content = snackbar;

        snackbar.IsShown = true;

        try
        {
            await Task.Delay(snackbar.Timeout, CancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        await HideSnackbarAsync(snackbar);
    }

    private async Task HideSnackbarAsync(Snackbar snackbar)
    {
        snackbar.IsShown = false;

        await Task.Delay(300);

        SnackbarContent = null;
        Content = null;
    }
}
