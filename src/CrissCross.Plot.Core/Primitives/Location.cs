// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes an X/Y location in pixel or coordinate space.</summary>
public sealed class Location
{
    /// <summary>Initializes a new instance of the <see cref="Location"/> class.</summary>
    private Location()
    {
    }

    /// <summary>Gets a location whose units are unspecified.</summary>
    public static Location Unspecified => new() { X = double.NaN, Y = double.NaN, Unit = LocationUnit.Unspecified };

    /// <summary>Gets the location units.</summary>
    public LocationUnit Unit { get; init; }

    /// <summary>Gets or sets the horizontal component.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the vertical component.</summary>
    public double Y { get; set; }

    /// <summary>Creates a location in pixel units.</summary>
    /// <param name="x">The horizontal pixel coordinate.</param>
    /// <param name="y">The vertical pixel coordinate.</param>
    /// <returns>The location in pixel units.</returns>
    public static Location Pixel(float x, float y) => new() { X = x, Y = y, Unit = LocationUnit.Pixel };

    /// <summary>Creates a location in coordinate units.</summary>
    /// <param name="x">The horizontal coordinate.</param>
    /// <param name="y">The vertical coordinate.</param>
    /// <returns>The location in coordinate units.</returns>
    public static Location Coordinates(float x, float y) => new() { X = x, Y = y, Unit = LocationUnit.Coordinates };

    /// <summary>Gets this location as a pixel coordinate.</summary>
    /// <returns>The pixel coordinate.</returns>
    /// <exception cref="InvalidOperationException">The location is not in pixel units.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024", Justification = "This public conversion method is retained for API compatibility.")]
    public Pixel GetPixel()
    {
        if (Unit != LocationUnit.Pixel)
        {
            throw new InvalidOperationException("units are not pixels");
        }

        return new(X, Y);
    }

    /// <summary>Gets this location as a coordinate-space point.</summary>
    /// <returns>The coordinate-space point.</returns>
    /// <exception cref="InvalidOperationException">The location is not in coordinate units.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024", Justification = "This public conversion method is retained for API compatibility.")]
    public Coordinates GetCoordinates()
    {
        if (Unit != LocationUnit.Coordinates)
        {
            throw new InvalidOperationException("units are not pixels");
        }

        return new(X, Y);
    }
}
