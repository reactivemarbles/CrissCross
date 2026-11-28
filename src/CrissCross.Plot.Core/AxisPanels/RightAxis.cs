// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.TickGenerators;

namespace CrissCross.Plot.AxisPanels;

/// <summary>Represents a vertical axis displayed along the right side of a plot.</summary>
public class RightAxis : YAxisBase
{
    /// <summary>The default label rotation for the right vertical axis.</summary>
    private const float DefaultLabelRotation = 90F;

    /// <summary>Initializes a new instance of the <see cref="RightAxis"/> class.</summary>
    public RightAxis()
    {
        LabelRotation = DefaultLabelRotation;
    }

    /// <inheritdoc />
    public override ITickGenerator TickGenerator { get; set; } = new NumericAutomatic();

    /// <inheritdoc />
    public override Edge Edge { get; } = Edge.Right;
}
