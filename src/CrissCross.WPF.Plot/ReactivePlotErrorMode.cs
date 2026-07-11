// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Describes how the binder surfaces invalid updates and source failures.</summary>
public enum ReactivePlotErrorMode
{
    /// <summary>Surface the error and stop the affected series.</summary>
    SurfaceAndStopSeries,

    /// <summary>Surface the error and allow retry-capable sources to continue.</summary>
    SurfaceAndContinueWithRetry,

    /// <summary>Ignore invalid update envelopes without mutating the chart.</summary>
    IgnoreInvalidUpdates,
}
