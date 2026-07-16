// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Specifies the available plot types for visualizing data in the plotting library.</summary>
/// <remarks>Use this enumeration to select the style of plot to render, such as signal, scatter, data logger,
/// streamer, or signal XY. The choice of plot type determines how data points are displayed and which features are
/// available for interaction and analysis.</remarks>
public enum PlotType
{
    /// <summary>The signal.</summary>
    Signal,

    /// <summary>The scatter.</summary>
    Scatter,

    /// <summary>The data logger.</summary>
    DataLogger,

    /// <summary>The streamer.</summary>
    Streamer,

    /// <summary>The signal xy.</summary>
    SignalXY,

    /// <summary>A conventional connected XY line.</summary>
    Line,

    /// <summary>A horizontal-step XY line.</summary>
    StepLine,

    /// <summary>An XY line filled to a configured baseline.</summary>
    Area,

    /// <summary>A vertical bar chart.</summary>
    Bar,

    /// <summary>A lollipop or stem chart.</summary>
    Stem,

    /// <summary>An unconnected point chart.</summary>
    Points,
}
