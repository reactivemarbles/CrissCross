// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// This interface describes a 1D axis (horizontal or vertical).
/// Responsibilities include: min/max management, unit/pixel conversion, 
/// tick generation (and rendering), axis label rendering, 
/// and self-measurement for layout purposes.
/// </summary>
public interface IAxis : IPanel
{
    /// <summary>Gets the min/max range currently displayed by this axis.</summary>
    CoordinateRangeMutable Range { get; } // TODO: don't expose this

    /// <summary>Gets or sets the minimum coordinate displayed by this axis.</summary>
    double Min { get; set; }

    /// <summary>Gets or sets the maximum coordinate displayed by this axis.</summary>
    double Max { get; set; }

    /// <summary>Gets or sets the logic for determining tick positions and formatting labels.</summary>
    ITickGenerator TickGenerator { get; set; } // TODO: never call TickGenerator.Generate() externally

    /// <summary>Gets the label displayed distal to the ticks.</summary>
    LabelStyle Label { get; }

    /// <summary>Gets or sets the style of major tick marks.</summary>
    TickMarkStyle MajorTickStyle { get; set; }

    /// <summary>Gets or sets the style of minor tick marks.</summary>
    TickMarkStyle MinorTickStyle { get; set; }

    /// <summary>Gets or sets the style used to render tick labels.</summary>
    LabelStyle TickLabelStyle { get; set; }

    /// <summary>Gets the style used to render the axis frame line.</summary>
    LineStyle FrameLineStyle { get; }

    /// <summary>Gets the pixel position of a coordinate given the location and size of the data area.</summary>
    /// <param name="position">The coordinate position.</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped.</param>
    /// <returns>The pixel position corresponding to the coordinate.</returns>
    float GetPixel(double position, PixelRect dataArea);

    /// <summary>Gets the coordinate of a pixel position given the location and size of the data area.</summary>
    /// <param name="pixel">The pixel position.</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped.</param>
    /// <returns>The coordinate corresponding to the pixel.</returns>
    double GetCoordinate(float pixel, PixelRect dataArea);

    /// <summary>Converts a distance in coordinate space to pixel space.</summary>
    /// <param name="coordinateDistance">A distance in coordinate units.</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped.</param>
    /// <returns>The same distance in pixel units.</returns>
    double GetPixelDistance(double coordinateDistance, PixelRect dataArea);

    /// <summary>Converts a distance in pixel space to coordinate space.</summary>
    /// <param name="pixelDistance">A distance in pixel units.</param>
    /// <param name="dataArea">The rectangle onto which the coordinates are mapped.</param>
    /// <returns>The same distance in coordinate units.</returns>
    double GetCoordinateDistance(float pixelDistance, PixelRect dataArea);

    /// <summary>Replaces the <see cref="TickGenerator"/> with a manually configured tick generator.</summary>
    /// <param name="xs">The coordinates for the tick marks.</param>
    /// <param name="labels">The labels displayed at the tick marks.</param>
    void SetTicks(double[] xs, string[] labels);

    /// <summary>Uses the <see cref="TickLabelStyle"/> to generate ticks with ideal spacing.</summary>
    /// <param name="size">The length of the axis in pixels.</param>
    /// <param name="paint">The paint used to measure tick labels.</param>
    void RegenerateTicks(PixelLength size, Paint paint);
}
