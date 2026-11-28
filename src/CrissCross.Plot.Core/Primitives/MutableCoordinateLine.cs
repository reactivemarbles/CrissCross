// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides mutable endpoints and bounds for a coordinate line.</summary>
public class MutableCoordinateLine
{
    /// <summary>Gets or sets the first endpoint's X coordinate.</summary>
    public double X1 { get; set; }

    /// <summary>Gets or sets the second endpoint's X coordinate.</summary>
    public double X2 { get; set; }

    /// <summary>Gets or sets the first endpoint's Y coordinate.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second endpoint's Y coordinate.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets or sets the first endpoint as a coordinate.</summary>
    public Coordinates Point1
    {
        get => new(X1, Y1);
        set
        {
            X1 = value.X;
            Y1 = value.Y;
        }
    }

    /// <summary>Gets or sets the second endpoint as a coordinate.</summary>
    public Coordinates Point2
    {
        get => new(X2, Y2);
        set
        {
            X2 = value.X;
            Y2 = value.Y;
        }
    }

    /// <summary>Gets this line as an immutable coordinate line.</summary>
    public CoordinateLine CoordinateLine => new(X1, Y1, X2, Y2);

    /// <summary>Gets the minimum X coordinate.</summary>
    public double XMin => Math.Min(X1, X2);

    /// <summary>Gets the maximum X coordinate.</summary>
    public double XMax => Math.Max(X1, X2);

    /// <summary>Gets the minimum Y coordinate.</summary>
    public double YMin => Math.Min(Y1, Y2);

    /// <summary>Gets the maximum Y coordinate.</summary>
    public double YMax => Math.Max(Y1, Y2);

    /// <summary>Gets the axis limits covering this line.</summary>
    public AxisLimits AxisLimits => new(XMin, XMax, YMin, YMax);

    /// <summary>Updates the endpoints from an immutable coordinate line.</summary>
    /// <param name="line">The line whose endpoints are copied.</param>
    public void Update(CoordinateLine line)
    {
        X1 = line.X1;
        Y1 = line.Y1;
        X2 = line.X2;
        Y2 = line.Y2;
    }
}
