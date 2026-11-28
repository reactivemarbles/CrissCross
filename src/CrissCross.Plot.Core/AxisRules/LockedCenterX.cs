// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Locks the center coordinate of a horizontal axis.</summary>
/// <param name="xAxis">The horizontal axis to constrain.</param>
/// <param name="xCenter">The fixed center coordinate.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class LockedCenterX(IXAxis xAxis, double xCenter) : IAxisRule
{
    /// <summary>Gets the horizontal axis constrained by this rule.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public readonly IXAxis XAxis = xAxis;

    /// <summary>The fixed center coordinate.</summary>
    private readonly double _centerCoordinate = xCenter;

    /// <summary>Applies this rule by panning the horizontal axis to the configured center.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout) => XAxis.Range.Pan(_centerCoordinate - XAxis.Range.Center);
}
