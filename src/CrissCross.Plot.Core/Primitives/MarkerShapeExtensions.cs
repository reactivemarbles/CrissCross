// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Markers;

namespace CrissCross.Plot;

/// <summary>Provides helpers for standard marker shapes.</summary>
public static class MarkerShapeExtensions
{
    /// <summary>The marker shapes rendered using only a line.</summary>
    private static readonly HashSet<MarkerShape> LineOnlyShapes = new()
    {
        MarkerShape.OpenCircle,
        MarkerShape.OpenSquare,
        MarkerShape.OpenTriangleUp,
        MarkerShape.OpenTriangleDown,
        MarkerShape.OpenDiamond,
        MarkerShape.Eks,
        MarkerShape.Cross,
        MarkerShape.VerticalBar,
        MarkerShape.HorizontalBar,
        MarkerShape.TriUp,
        MarkerShape.TriDown,
        MarkerShape.Asterisk,
        MarkerShape.HashTag,
        MarkerShape.OpenCircleWithDot,
        MarkerShape.OpenCircleWithCross,
        MarkerShape.OpenCircleWithEks,
    };

    /// <summary>Extension members for <c>MarkerShape</c>.</summary>
    /// <param name="shape">The marker shape used by the extension members.</param>
    extension(MarkerShape shape)
    {
        /// <summary>Gets the marker object for a standard marker shape.</summary>
        /// <returns>The marker corresponding to this shape.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Retained as an extension method for API compatibility and to create a fresh marker instance on each call.")]
        public IMarker GetMarker() => shape switch
        {
            MarkerShape.FilledCircle => new FilledCircle(),
            MarkerShape.OpenCircle => new OpenCircle(),
            MarkerShape.FilledSquare => new FilledSquare(),
            MarkerShape.OpenSquare => new OpenSquare(),
            MarkerShape.FilledTriangleUp => new FilledTriangleUp(),
            MarkerShape.OpenTriangleUp => new OpenTriangleUp(),
            MarkerShape.FilledTriangleDown => new FilledTriangleDown(),
            MarkerShape.OpenTriangleDown => new OpenTriangleDown(),
            MarkerShape.FilledDiamond => new FilledDiamond(),
            MarkerShape.OpenDiamond => new OpenDiamond(),
            MarkerShape.Eks => new Eks(),
            MarkerShape.Cross => new Cross(),
            MarkerShape.VerticalBar => new VerticalBar(),
            MarkerShape.HorizontalBar => new HorizontalBar(),
            MarkerShape.TriUp => new TriUp(),
            MarkerShape.TriDown => new TriDown(),
            MarkerShape.Asterisk => new Asterisk(),
            MarkerShape.HashTag => new HashTag(),
            MarkerShape.OpenCircleWithDot => new OpenCircleWithDot(),
            MarkerShape.OpenCircleWithCross => new OpenCircleWithCross(),
            MarkerShape.OpenCircleWithEks => new OpenCircleWithEks(),
            MarkerShape.CircleWithLineLeft => new CircleWithLineLeft(),
            MarkerShape.CircleWithLineRight => new CircleWithLineRight(),
            MarkerShape.TriangleWithLineLeft => new TriangleWithLineLeft(),
            MarkerShape.TriangleWithLineRight => new TriangleWithLineRight(),
            MarkerShape.None => new None(),
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "The marker shape is not supported."),
        };

        /// <summary>Determines whether this marker uses only a line.</summary>
        /// <returns><see langword="true"/> when the marker uses only a line.</returns>
        public bool IsLineOnly() => LineOnlyShapes.Contains(shape);
    }
}
