// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Specifies the available plot types for visualizing data in the plotting library.
/// </summary>
/// <remarks>Use this enumeration to select the style of plot to render, such as signal, scatter, data logger,
/// streamer, or signal XY. The choice of plot type determines how data points are displayed and which features are
/// available for interaction and analysis.</remarks>
public enum PlotType
{
    /// <summary>
    /// The signal.
    /// </summary>
    Signal,
    /// <summary>
    /// The scatter.
    /// </summary>
    Scatter,
    /// <summary>
    /// The data logger.
    /// </summary>
    DataLogger,
    /// <summary>
    /// The streamer.
    /// </summary>
    Streamer,
    /// <summary>
    /// The signal xy.
    /// </summary>
    SignalXY
}
