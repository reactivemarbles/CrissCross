// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Generates the ticks displayed along an axis.</summary>
public interface ITickGenerator
{
    /// <summary>Gets the ticks to display the next time the axis is rendered.</summary>
    /// <remarks>This array and its contents should not be modified directly. Call <see cref="Regenerate"/> to update it.</remarks>
    Tick[] Ticks { get; }

    /// <summary>Gets or sets the maximum number of ticks to generate.</summary>
    int MaxTickCount { get; set; }

    /// <summary>Generates ticks based on the current settings and stores the result in <see cref="Ticks"/>.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The edge along which the ticks are displayed.</param>
    /// <param name="size">The available pixel length.</param>
    /// <param name="paint">The paint used to measure tick labels.</param>
    /// <param name="labelStyle">The style used for tick labels.</param>
    void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle);
}
