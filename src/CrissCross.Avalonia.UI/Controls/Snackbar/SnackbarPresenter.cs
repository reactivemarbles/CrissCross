// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>SnackbarPresenter member.</summary>
/// <seealso cref="ContentPresenter" />
public class SnackbarPresenter : ContentPresenter, IDisposable
{
    /// <summary>Property for <see cref="SnackbarContent"/>.</summary>
    public static readonly StyledProperty<Snackbar?> SnackbarContentProperty =
        AvaloniaProperty.Register<SnackbarPresenter, Snackbar?>(nameof(SnackbarContent));

    /// <summary>Delay that allows the hide animation to complete.</summary>
    private const int HideCompletionDelayMilliseconds = 300;

    /// <summary>Tracks whether owned resources have been released.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="SnackbarPresenter"/> class.</summary>
    public SnackbarPresenter() => Unloaded += OnUnloadedHandler;

    /// <summary>Gets the SnackbarContent value.</summary>
    /// <value>
    /// The snackbar content.
    /// </value>
    public Snackbar? SnackbarContent
    {
        get => GetValue(SnackbarContentProperty);
        protected set => SetValue(SnackbarContentProperty, value);
    }

    /// <summary>Gets the queue.</summary>
    /// <value>
    /// The queue.
    /// </value>
    protected Queue<Snackbar> Queue { get; } = new();

    /// <summary>Gets or sets the cancellation token source.</summary>
    /// <value>
    /// The cancellation token source.
    /// </value>
    protected CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>Adds to queue.</summary>
    /// <param name="snackbar">The snackbar.</param>
    public virtual void AddToQueue(Snackbar snackbar)
    {
        Queue.Enqueue(snackbar);

        if (SnackbarContent is not null)
        {
            return;
        }

        _ = ShowQueuedSnackbarsAsync();
    }

    /// <summary>Immediately displays the snackbar.</summary>
    /// <param name="snackbar">The snackbar.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

    /// <summary>Hides the current snackbar.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task HideCurrentAsync()
    {
        if (SnackbarContent is null)
        {
            return;
        }

        await CancellationTokenSource.CancelAsync();
        await HideSnackbarAsync(SnackbarContent);
        ResetCancellationTokenSource();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Called when [unloaded].</summary>
    protected virtual void OnUnloaded()
    {
        if (CancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        ImmediatelyHideCurrent();
        ResetCancellationTokenSource();
    }

    /// <summary>Releases resources owned by the presenter.</summary>
    /// <param name="disposing">Whether the call originates from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }

        if (!CancellationTokenSource.IsCancellationRequested)
        {
            CancellationTokenSource.Cancel();
        }

        CancellationTokenSource.Dispose();
        _disposed = true;
    }

    /// <summary>Resets the cancellation token source.</summary>
    protected void ResetCancellationTokenSource()
    {
        CancellationTokenSource.Dispose();
        CancellationTokenSource = new();
    }

    /// <summary>Provides the OnUnloadedHandler member.</summary>
    /// <param name="sender">The sender value.</param>
    /// <param name="e">The e value.</param>
    private static void OnUnloadedHandler(object? sender, RoutedEventArgs e)
    {
        if (sender is not SnackbarPresenter self)
        {
            return;
        }

        self.OnUnloaded();
    }

    /// <summary>Provides the ImmediatelyHideCurrent member.</summary>
    private void ImmediatelyHideCurrent()
    {
        if (SnackbarContent is null)
        {
            return;
        }

        CancellationTokenSource.Cancel();
        ImmediatelyHideSnackbar(SnackbarContent);
    }

    /// <summary>Provides the ImmediatelyHideSnackbar member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    private void ImmediatelyHideSnackbar(Snackbar snackbar)
    {
        snackbar.IsShown = false;
        SnackbarContent = null;
        Content = null;
    }

    /// <summary>Provides the ShowQueuedSnackbarsAsync member.</summary>
    /// <returns>The result.</returns>
    private async Task ShowQueuedSnackbarsAsync()
    {
        while (Queue.Count > 0 && !CancellationTokenSource.IsCancellationRequested)
        {
            var snackbar = Queue.Dequeue();

            await ShowSnackbarAsync(snackbar);
        }
    }

    /// <summary>Provides the ShowSnackbarAsync member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the HideSnackbarAsync member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    /// <returns>The result.</returns>
    private async Task HideSnackbarAsync(Snackbar snackbar)
    {
        snackbar.IsShown = false;

        await Task.Delay(HideCompletionDelayMilliseconds);

        SnackbarContent = null;
        Content = null;
    }
}
