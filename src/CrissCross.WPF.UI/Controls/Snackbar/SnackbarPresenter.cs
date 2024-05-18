// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveMarbles.ObservableEvents;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// SnackbarPresenter.
/// </summary>
/// <seealso cref="System.Windows.Controls.ContentPresenter" />
public class SnackbarPresenter : System.Windows.Controls.ContentPresenter, IDisposable
{
    /// <summary>
    /// The queue.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected readonly Queue<Snackbar> Queue = new();

    /// <summary>
    /// The cancellation token source.
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource = new();
#pragma warning restore SA1401 // Fields should be private

    private readonly IDisposable? _unloadedSubscription;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnackbarPresenter"/> class.
    /// </summary>
    public SnackbarPresenter() =>
        _unloadedSubscription = this.Events().Unloaded.Subscribe(sender =>
        {
            var self = (SnackbarPresenter)sender.Source;
            self.OnUnloaded();
        });

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    public new Snackbar? Content
    {
        get => (Snackbar)GetValue(ContentProperty);
        protected set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Adds to que.
    /// </summary>
    /// <param name="snackbar">The snackbar.</param>
    public virtual void AddToQue(Snackbar snackbar)
    {
        Queue.Enqueue(snackbar);

        if (Content is null)
        {
            _ = ShowQueuedSnackbars(); // TODO: Fix detached process
        }
    }

    /// <summary>
    /// Immediatelies the display.
    /// </summary>
    /// <param name="snackbar">The snackbar.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task ImmediatelyDisplay(Snackbar snackbar)
    {
        if (snackbar is null)
        {
            throw new ArgumentNullException(nameof(snackbar));
        }

        await HideCurrent();
        await ShowSnackbar(snackbar);

        await ShowQueuedSnackbars();
    }

    /// <summary>
    /// Hides the current.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task HideCurrent()
    {
        if (Content is null)
        {
            return;
        }

        CancellationTokenSource.Cancel();
        await HidSnackbar(Content);
        ResetCancellationTokenSource();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called when [unloaded].
    /// </summary>
    protected virtual void OnUnloaded()
    {
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();
    }

    /// <summary>
    /// Resets the cancellation token source.
    /// </summary>
    protected void ResetCancellationTokenSource()
    {
        CancellationTokenSource.Dispose();
        CancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CancellationTokenSource.Dispose();
                _unloadedSubscription?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    private async Task ShowQueuedSnackbars()
    {
        while (Queue.Count > 0 && !CancellationTokenSource.IsCancellationRequested)
        {
            var snackbar = Queue.Dequeue();

            await ShowSnackbar(snackbar);
        }
    }

    private async Task ShowSnackbar(Snackbar snackbar)
    {
        Content = snackbar;

        snackbar.IsShown = true;

        try
        {
            await Task.Delay(snackbar.Timeout, CancellationTokenSource.Token);
        }
        catch
        {
            return;
        }

        await HidSnackbar(snackbar);
    }

    private async Task HidSnackbar(Snackbar snackbar)
    {
        snackbar.IsShown = false;

        await Task.Delay(300);

        Content = null;
    }
}
