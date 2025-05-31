// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Interface IAppearence.
/// </summary>
public interface IPlottableUI : IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether [automatic scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic scale]; otherwise, <c>false</c>.
    /// </value>
    bool AutoScale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [manual scale].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [manual scale]; otherwise, <c>false</c>.
    /// </value>
    bool ManualScale { get; set; }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    int Mode { get; set; }

    /// <summary>
    /// Gets or sets the number points plotted.
    /// </summary>
    /// <value>
    /// The number points plotted.
    /// </value>
    int NumberPointsPlotted { get; set; }

    /// <summary>
    /// Gets or sets the chart settings.
    /// </summary>
    /// <value>
    /// The chart settings.
    /// </value>
    ChartObjects ChartSettings { get; set; }
}
