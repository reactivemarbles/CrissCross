// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>
/// Class used to create identifiers of threads or tasks that can be performed multiple times within one instance.
/// <see cref="Current"/> represents roughly the time in microseconds at which it was taken.
/// </summary>
internal sealed class EventIdentifier
{
    /// <summary>Stores the source of current time used to create identifiers.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Initializes a new instance of the <see cref="EventIdentifier"/> class.</summary>
    /// <param name="timeProvider">The source of current time. When omitted, the system time provider is used.</param>
    internal EventIdentifier(TimeProvider? timeProvider = null) => _timeProvider = timeProvider ?? TimeProvider.System;

    /// <summary>Gets or sets the current identifier.</summary>
    internal long Current { get; set; }

    /// <summary>Creates and gets the next identifier.</summary>
    /// <returns>The result.</returns>
    internal long GetNext()
    {
        UpdateIdentifier();

        return Current;
    }

    /// <summary>Checks if the identifiers are the same.</summary>
    /// <param name="storedId">The storedId value.</param>
    /// <returns>The result.</returns>
    internal bool IsEqual(long storedId) => Current == storedId;

    /// <summary>Creates and assigns a random value with an extra time code if possible.</summary>
    private void UpdateIdentifier() => Current = _timeProvider.GetLocalNow().ToUnixTimeMilliseconds();
}
