// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents SnackbarPresenter.</summary>
/// <seealso cref="System.Windows.Controls.ContentPresenter" />
public class SnackbarPresenter : System.Windows.Controls.ContentPresenter
{
    /// <summary>Provides the duration of the snackbar hide transition.</summary>
    private const int HideTransitionDurationMilliseconds = 300;

    /// <summary>Initializes a new instance of the <see cref="SnackbarPresenter"/> class.</summary>
    public SnackbarPresenter() =>
        Unloaded += static (sender, _) =>
        {
            var self = (SnackbarPresenter)sender;
            self.OnUnloaded();
        };

    /// <summary>Finalizes an instance of the <see cref="SnackbarPresenter"/> class.</summary>
    ~SnackbarPresenter()
    {
        if (!CancellationTokenSource.IsCancellationRequested)
        {
            CancellationTokenSource.Cancel();
        }

        CancellationTokenSource.Dispose();
    }

    /// <summary>Gets the Gets or sets the content. value.</summary>
    /// <value>
    /// The content.
    /// </value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "WpfAnalyzers.DependencyProperty",
        "WPF0012:CLR property type should match registered type",
        Justification = "seems harmless")]
    public new Snackbar? Content
    {
        get => (Snackbar?)GetValue(ContentProperty);
        protected set => SetValue(ContentProperty, value);
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

    /// <summary>Adds to que.</summary>
    /// <param name="snackbar">The snackbar.</param>
    public virtual void AddToQue(Snackbar snackbar)
    {
        Queue.Enqueue(snackbar);

        if (Content is not null)
        {
            return;
        }

        _ = ShowQueuedSnackbars(); // TODO: Fix detached process
    }

    /// <summary>Immediatelies the display.</summary>
    /// <param name="snackbar">The snackbar.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task ImmediatelyDisplay(Snackbar snackbar)
    {
        if (snackbar is null)
        {
            return;
        }

        await HideCurrent();
        await ShowSnackbar(snackbar);

        await ShowQueuedSnackbars();
    }

    /// <summary>Hides the current.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual async Task HideCurrent()
    {
        if (Content is null)
        {
            return;
        }

        await CancelCurrentAsync();
        await HidSnackbar(Content);
        ResetCancellationTokenSource();
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

    /// <summary>Resets the cancellation token source.</summary>
    protected void ResetCancellationTokenSource()
    {
        CancellationTokenSource.Dispose();
        CancellationTokenSource = new();
    }

    /// <summary>Cancels the current token source.</summary>
    /// <returns>A task representing the cancellation operation.</returns>
    private Task CancelCurrentAsync()
    {
#if NET8_0_OR_GREATER
        return CancellationTokenSource.CancelAsync();
#else
        CancelCurrent();
        return Task.CompletedTask;
#endif
    }

#if !NET8_0_OR_GREATER

    /// <summary>Cancels the current token source synchronously.</summary>
    private void CancelCurrent() => CancellationTokenSource.Cancel();
#endif

    /// <summary>Provides the ImmediatelyHideCurrent member.</summary>
    private void ImmediatelyHideCurrent()
    {
        if (Content is null)
        {
            return;
        }

        CancellationTokenSource.Cancel();
        ImmediatelyHidSnackbar(Content);
    }

    /// <summary>Provides the ImmediatelyHidSnackbar member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    private void ImmediatelyHidSnackbar(Snackbar snackbar)
    {
        snackbar.SetCurrentValue(Snackbar.IsShownProperty, false);
        Content = null;
    }

    /// <summary>Provides the ShowQueuedSnackbars member.</summary>
    /// <returns>The result.</returns>
    private async Task ShowQueuedSnackbars()
    {
        while (Queue.Count > 0 && !CancellationTokenSource.IsCancellationRequested)
        {
            var snackbar = Queue.Dequeue();

            await ShowSnackbar(snackbar);
        }
    }

    /// <summary>Provides the ShowSnackbar member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    /// <returns>The result.</returns>
    private async Task ShowSnackbar(Snackbar snackbar)
    {
        Content = snackbar;

        snackbar.SetCurrentValue(Snackbar.IsShownProperty, true);

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

    /// <summary>Provides the HidSnackbar member.</summary>
    /// <param name="snackbar">The snackbar value.</param>
    /// <returns>The result.</returns>
    private async Task HidSnackbar(Snackbar snackbar)
    {
        snackbar.SetCurrentValue(Snackbar.IsShownProperty, false);

        await Task.Delay(HideTransitionDurationMilliseconds);

        Content = null;
    }
}
