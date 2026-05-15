// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Immutable identity for a chart series.
/// </summary>
/// <param name="Name">The stable logical series name.</param>
/// <param name="Axis">The target Y-axis index.</param>
public readonly record struct PlotSeriesKey(string Name, int Axis);
