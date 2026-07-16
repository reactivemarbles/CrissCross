// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>Represents one normalized numeric plot coordinate.</summary>
/// <param name="X">The numeric or OLE Automation date X value.</param>
/// <param name="Y">The Y value.</param>
public readonly record struct PlotDataPoint(double X, double Y);
