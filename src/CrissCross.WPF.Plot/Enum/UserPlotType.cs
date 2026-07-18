// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides the UserPlotType member.</summary>
/// <remarks>Use this enumeration to indicate the format or style of data visualization required, such as
/// signal-based plots, scatter plots, or data logger outputs. The specific plot type determines how data points are
/// interpreted and displayed. This enumeration is typically used when configuring or requesting plots in user-facing
/// APIs.</remarks>
public enum UserPlotType
{
    /// <summary>The signal enum obs ticks.</summary>
    SignalEnumObsTicks,

    /// <summary>The data logger enum obs points.</summary>
    DataLoggerEnumObsPoints,

    /// <summary>The signal xy timestamp.</summary>
    SignalXYTimestamp,

    /// <summary>The signal xy points.</summary>
    SignalXYPoints,

    /// <summary>The signal xy enum points.</summary>
    SignalXYEnumPoints,

    /// <summary>The streamer enum obs points.</summary>
    StreamerEnumObsPoints,

    /// <summary>The scatter enum obs points.</summary>
    ScatterEnumObsPoints,

    /// <summary>The scatter points.</summary>
    ScatterPoints,
}
