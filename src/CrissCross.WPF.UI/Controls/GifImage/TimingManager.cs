// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the TimingManager member.</summary>
/// <param name="repeatBehavior">The repeatBehavior value.</param>
internal sealed class TimingManager(RepeatBehavior repeatBehavior)
{
    /// <summary>Stores the _timeSpans value.</summary>
    private readonly List<TimeSpan> _timeSpans = [];

    /// <summary>Stores the _completedTask value.</summary>
    private readonly Task _completedTask = Task.CompletedTask;

    /// <summary>Stores the _current value.</summary>
    private int _current;

    /// <summary>Stores the _count value.</summary>
    private int _count;

    /// <summary>Stores the _elapsed value.</summary>
    private TimeSpan _elapsed;

    /// <summary>Stores the _pauseCompletionSource value.</summary>
    private TaskCompletionSource<int>? _pauseCompletionSource;

    /// <summary>Provides the Completed member.</summary>
    internal event EventHandler? Completed;

    /// <summary>Gets or sets RepeatBehavior.</summary>
    internal RepeatBehavior RepeatBehavior { get; set; } = repeatBehavior;

    /// <summary>Gets a value indicating whether playback is complete.</summary>
    internal bool IsComplete
    {
        get => field;
        private set
        {
            field = value;
            if (!value)
            {
                return;
            }

            OnCompleted();
        }
    }

    /// <summary>Gets the IsPaused value.</summary>
    internal bool IsPaused { get; private set; }

    /// <summary>Provides the Add member.</summary>
    /// <param name="timeSpan">The timeSpan value.</param>
    internal void Add(TimeSpan timeSpan) => _timeSpans.Add(timeSpan);

    /// <summary>Provides the NextAsync member.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    internal async Task<bool> NextAsync(CancellationToken cancellationToken)
    {
        if (IsComplete)
        {
            return false;
        }

        await IsPausedAsync(cancellationToken);

        var repeatBehavior = RepeatBehavior;

        var ts = _timeSpans[_current];
        await Task.Delay(ts, cancellationToken);
        _current++;
        _elapsed += ts;

        if (repeatBehavior.HasDuration && _elapsed >= repeatBehavior.Duration)
        {
            IsComplete = true;
            return false;
        }

        if (_current < _timeSpans.Count)
        {
            return true;
        }

        _count++;
        if (repeatBehavior.HasCount)
        {
            if (_count < repeatBehavior.Count)
            {
                _current = 0;
                return true;
            }

            IsComplete = true;
            return false;
        }

        _current = 0;

        return true;
    }

    /// <summary>Provides the Reset member.</summary>
    internal void Reset()
    {
        _current = 0;
        _count = 0;
        _elapsed = TimeSpan.Zero;
        IsComplete = false;
    }

    /// <summary>Provides the Pause member.</summary>
    internal void Pause()
    {
        if (IsPaused)
        {
            return; // Make this a no-op.
        }

        IsPaused = true;
        _pauseCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Provides the Resume member.</summary>
    internal void Resume()
    {
        if (!IsPaused)
        {
            return; // Make this a no-op.
        }

        var tcs = _pauseCompletionSource;
        tcs?.TrySetResult(0);
        _pauseCompletionSource = null;
        IsPaused = false;
    }

    /// <summary>Provides the OnCompleted member.</summary>
    private void OnCompleted() => Completed?.Invoke(this, EventArgs.Empty);

    /// <summary>Provides the IsPausedAsync member.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    private Task IsPausedAsync(CancellationToken cancellationToken)
    {
        var tcs = _pauseCompletionSource;
        return tcs is not null ? tcs.Task.WithCancellationToken(cancellationToken) : _completedTask;
    }
}
