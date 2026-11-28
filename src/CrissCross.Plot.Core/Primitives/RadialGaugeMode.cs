// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Specifies how multiple radial gauges are arranged.</summary>
public enum RadialGaugeMode
{
    /// <summary>Successive gauges start outward from the center but start at the same angle.</summary>
    Stacked,

    /// <summary>Successive gauges start outward from the center and start at sequential angles.</summary>
    Sequential,

    /// <summary>Gauges are all the same distance from the center but start at sequential angles.</summary>
    SingleGauge
}
