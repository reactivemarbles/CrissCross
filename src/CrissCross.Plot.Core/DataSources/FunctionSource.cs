// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides function-backed data for plotting.</summary>
/// <param name="func">The function that produces Y values from X values.</param>
public class FunctionSource(Func<double, double> func) : IFunctionSource
{
    /// <summary>Gets or sets the horizontal range supported by the function.</summary>
    public CoordinateRange RangeX { get; set; } = new(double.NegativeInfinity, double.PositiveInfinity);

    /// <summary>Gets or sets the function that produces Y values.</summary>
    public Func<double, double> Function { get; set; } = func;

    /// <summary>Gets or sets the optional function that estimates Y bounds for an X range.</summary>
    public Func<CoordinateRange, CoordinateRange>? GetRangeYFunc { get; set; }

    /// <summary>Evaluates the function at the specified horizontal coordinate.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <returns>The function value.</returns>
    public double Get(double x) => Function(x);

    /// <summary>Gets the vertical range for the specified horizontal range.</summary>
    /// <param name="rangeX">The horizontal range.</param>
    /// <returns>The estimated vertical range, or <see cref="CoordinateRange.NotSet"/> when no estimator is configured.</returns>
    public CoordinateRange GetRangeY(CoordinateRange rangeX) => GetRangeYFunc is null ? CoordinateRange.NotSet : GetRangeYFunc(rangeX);
}
