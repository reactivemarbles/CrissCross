// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Represents one contour line crossing a grid cell.</summary>
public class EdgeLine
{
    /// <summary>Initializes a new instance of the <see cref="EdgeLine"/> class.</summary>
    /// <param name="first">The first edge crossing.</param>
    /// <param name="second">The second edge crossing.</param>
    /// <param name="cellId">The grid cell identifier.</param>
    public EdgeLine(IEdge first, IEdge second, int cellId)
    {
        this.first = first;
        this.second = second;
        CellID = cellId;
    }

    /// <summary>Gets the first edge crossing.</summary>
    public IEdge first { get; }

    /// <summary>Gets the second edge crossing.</summary>
    public IEdge second { get; }

    /// <summary>Gets or sets the grid cell identifier.</summary>
    public int CellID { get; set; }
}
