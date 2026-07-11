// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Provides a manually pumped scheduler for deterministic binder tests.</summary>
internal sealed class ManualPumpScheduler : IScheduler
{
    /// <summary>Stores queued work items.</summary>
    private readonly Queue<IWorkItem> _actions = new();

    /// <inheritdoc />
    public DateTimeOffset Now { get; } = DateTimeOffset.UtcNow;

    /// <summary>Gets the last scheduled timestamp.</summary>
    public long Timestamp { get; private set; }

    /// <inheritdoc />
    public void Schedule(IWorkItem item) => _actions.Enqueue(item);

    /// <inheritdoc />
    public void Schedule(IWorkItem item, long dueTimestamp)
    {
        Timestamp = dueTimestamp;
        _actions.Enqueue(item);
    }

    /// <summary>Runs all queued work items.</summary>
    public void RunAll()
    {
        while (_actions.Count > 0)
        {
            _actions.Dequeue().Execute();
        }
    }
}
