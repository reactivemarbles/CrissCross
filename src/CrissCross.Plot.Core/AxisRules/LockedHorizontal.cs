// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Locks the minimum and maximum coordinates of a horizontal axis.</summary>
/// <param name="xAxis">The horizontal axis to constrain.</param>
/// <param name="xMin">The fixed minimum coordinate.</param>
/// <param name="xMax">The fixed maximum coordinate.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class LockedHorizontal(IXAxis xAxis, double xMin, double xMax) : IAxisRule
{
    /// <summary>Gets the horizontal axis constrained by this rule.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public readonly IXAxis XAxis = xAxis;

    /// <summary>Gets or sets the fixed minimum coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double XMin = xMin;

    /// <summary>Gets or sets the fixed maximum coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double XMax = xMax;

    /// <summary>Applies this rule by setting the horizontal axis range.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout) => XAxis.Range.Set(XMin, XMax);
}
