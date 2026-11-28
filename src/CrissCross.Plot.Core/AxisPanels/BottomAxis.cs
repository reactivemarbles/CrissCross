// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.TickGenerators;

namespace CrissCross.Plot.AxisPanels;

/// <summary>Represents a horizontal axis displayed along the bottom of a plot.</summary>
public class BottomAxis : XAxisBase
{
    /// <inheritdoc />
    public override ITickGenerator TickGenerator { get; set; } = new NumericAutomatic();

    /// <inheritdoc />
    public override Edge Edge => Edge.Bottom;
}
