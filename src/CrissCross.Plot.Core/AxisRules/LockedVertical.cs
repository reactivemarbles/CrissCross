// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisRules;

/// <summary>Locks the minimum and maximum coordinates of a vertical axis.</summary>
/// <param name="yAxis">The vertical axis to constrain.</param>
/// <param name="yMin">The fixed minimum coordinate.</param>
/// <param name="yMax">The fixed maximum coordinate.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class LockedVertical(IYAxis yAxis, double yMin, double yMax) : IAxisRule
{
    /// <summary>Gets the vertical axis constrained by this rule.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public readonly IYAxis YAxis = yAxis;

    /// <summary>Gets or sets the fixed minimum coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double YMin = yMin;

    /// <summary>Gets or sets the fixed maximum coordinate.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public double Max = yMax;

    /// <summary>Applies this rule by setting the vertical axis range.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout) => YAxis.Range.Set(YMin, Max);
}
