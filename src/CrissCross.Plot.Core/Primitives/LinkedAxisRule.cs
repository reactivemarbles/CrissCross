// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a rule that links one axis to an axis on another plot.</summary>
/// <param name="sourceAxis">The axis whose changes are observed.</param>
/// <param name="targetAxis">The axis updated by the rule.</param>
/// <param name="targetPlot">The plot containing the target axis.</param>
public class LinkedAxisRule(IAxis sourceAxis, IAxis targetAxis, Plot targetPlot)
{
    /// <summary>Gets the source axis.</summary>
    public IAxis SourceAxis { get; } = sourceAxis;

    /// <summary>Gets the target axis.</summary>
    public IAxis TargetAxis { get; } = targetAxis;

    /// <summary>Gets the plot containing the target axis.</summary>
    public Plot TargetPlot { get; } = targetPlot;
}
