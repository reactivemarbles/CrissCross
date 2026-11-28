// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Locks the minimum coordinate of a vertical axis.</summary>
/// <param name="yAxis">The vertical axis to constrain.</param>
/// <param name="yMin">The fixed minimum coordinate.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class LockedBottom(IYAxis yAxis, double yMin) : IAxisRule
{
    /// <summary>Gets the vertical axis constrained by this rule.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public readonly IYAxis YAxis = yAxis;

    /// <summary>The fixed minimum coordinate.</summary>
    private readonly double _minimumCoordinate = yMin;

    /// <summary>Applies this rule by setting the vertical axis minimum.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout) => YAxis.Range.Min = _minimumCoordinate;
}
