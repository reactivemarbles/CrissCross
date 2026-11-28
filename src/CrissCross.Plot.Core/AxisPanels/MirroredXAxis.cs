// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisPanels;

/// <summary>Displays an existing X axis on another edge of the plot.</summary>
public sealed class MirroredXAxis : XAxisBase
{
    /// <summary>The edge on which this mirrored axis is displayed.</summary>
    private readonly Edge _edge;

    /// <summary>The axis whose range and ticks are mirrored.</summary>
    private readonly IXAxis _axis;

    /// <summary>Initializes a new instance of the <see cref="MirroredXAxis"/> class.</summary>
    /// <param name="axis">The X axis to mirror.</param>
    /// <param name="edge">The edge to use, or <see langword="null"/> to mirror the source edge.</param>
    public MirroredXAxis(IXAxis axis, Edge? edge)
    {
        _axis = axis;
        _edge = edge ?? axis.Edge;
        TickGenerator = axis.TickGenerator;
    }

    /// <inheritdoc />
    public override Edge Edge => _edge;

    /// <inheritdoc />
    public override CoordinateRangeMutable Range => new(_axis.Min, _axis.Max);
}
