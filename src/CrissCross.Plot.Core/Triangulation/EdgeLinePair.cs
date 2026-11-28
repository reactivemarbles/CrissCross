// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Represents a saddle cell containing two contour lines.</summary>
public class EdgeLinePair : EdgeLine
{
    /// <summary>Initializes a new instance of the <see cref="EdgeLinePair"/> class.</summary>
    /// <param name="first">The first edge of the first contour line.</param>
    /// <param name="second">The second edge of the first contour line.</param>
    /// <param name="first1">The first edge of the second contour line.</param>
    /// <param name="second1">The second edge of the second contour line.</param>
    /// <param name="cellId">The grid cell identifier.</param>
    public EdgeLinePair(IEdge first, IEdge second, IEdge first1, IEdge second1, int cellId)
        : base(first, second, cellId)
    {
        this.first1 = first1;
        this.second1 = second1;
    }

    /// <summary>Gets the first edge crossing of the second contour line.</summary>
    public IEdge first1 { get; }

    /// <summary>Gets the second edge crossing of the second contour line.</summary>
    public IEdge second1 { get; }
}
