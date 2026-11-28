// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides synchronization controls for rendering a plot on multiple threads.</summary>
public interface IMultiThreadedPlotControl
{
    /// <summary>Returns only when the plot is successfully locked and rendering has stopped.</summary>
    void Lock();

    /// <summary>Releases the plot lock and permits rendering again.</summary>
    void UnLock();

    /// <summary>Attempts to lock the plot and stop rendering.</summary>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/>.</returns>
    bool TryLock();

    /// <summary>Returns whether true if the plot currently locked.</summary>
    void IsLocked();

    /// <summary>Returns whether true if the plot is locked by the calling thread.</summary>
    void IsEntered();
}
