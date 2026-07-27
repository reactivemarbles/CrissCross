// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Represents one normalized numeric plot coordinate.</summary>
/// <param name="X">The numeric or OLE Automation date X value.</param>
/// <param name="Y">The Y value.</param>
public readonly record struct PlotDataPoint(double X, double Y);
