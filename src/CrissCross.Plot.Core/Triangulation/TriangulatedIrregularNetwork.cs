// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Provides triangulation and contour operations for irregular coordinate networks.</summary>
public class TriangulatedIrregularNetwork
{
    /// <summary>The triangulation backing this network.</summary>
    private Delaunator _delaunator = null!;

    /// <summary>Initializes a new instance of the <see cref="TriangulatedIrregularNetwork"/> class.</summary>
    /// <param name="coordinates">The three-dimensional points to triangulate.</param>
    public TriangulatedIrregularNetwork(IEnumerable<Coordinates3d> coordinates)
    {
        Update(coordinates);
    }

    /// <summary>Gets the bounds of the network.</summary>
    public AxisLimits3d AxisLimits { get; private set; }

    /// <summary>Gets the points used to construct the triangulation.</summary>
    public Coordinates3d[] Points3D { get; private set; } = [];

    /// <summary>Gets the minimum Z coordinate.</summary>
    public double MinZ { get; private set; }

    /// <summary>Gets the maximum Z coordinate.</summary>
    public double MaxZ { get; private set; }

    /// <summary>Replaces the points and rebuilds the network triangulation.</summary>
    /// <param name="coordinates">The three-dimensional points to triangulate.</param>
    public void Update(IEnumerable<Coordinates3d> coordinates)
    {
        Coordinates3d[] points = new List<Coordinates3d>(coordinates).ToArray();
        AxisLimits = AxisLimits3d.FromPoints(points);
        Points3D = points;

        if (points.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements.");
        }

        double minimumZ = points[0].Z;
        double maximumZ = points[0].Z;
        for (int i = 1; i < points.Length; i++)
        {
            minimumZ = Math.Min(minimumZ, points[i].Z);
            maximumZ = Math.Max(maximumZ, points[i].Z);
        }

        MinZ = minimumZ;
        MaxZ = maximumZ;
        _delaunator = new(Points3D);
    }

    /// <summary>Gets evenly spaced Z coordinates for a requested number of contour intervals.</summary>
    /// <param name="count">The number of intervals between the minimum and maximum Z values.</param>
    /// <returns>The generated Z coordinates.</returns>
    public double[] GetZsByCount(int count)
    {
        double interval = (MaxZ - MinZ) / (count + 1);
        return GetZsByInterval(interval);
    }

    /// <summary>Gets Z coordinates separated by the specified interval.</summary>
    /// <param name="interval">The distance between adjacent Z coordinates.</param>
    /// <returns>The generated Z coordinates.</returns>
    public double[] GetZsByInterval(double interval)
    {
        int count = (int)((MaxZ - MinZ) / interval) + 1;
        ArgumentChecks.ThrowIfNegative(count, nameof(count));

        double[] zs = new double[count];
        for (int i = 0; i < count; i++)
        {
            zs[i] = MinZ + (interval * i);
        }

        return zs;
    }

    /// <summary>Gets the lines connecting the triangulation points.</summary>
    /// <returns>The triangulation edges.</returns>
    public CoordinateLine[] GetLines()
    {
        List<CoordinateLine> lines = [];
        foreach (var edge in _delaunator.GetEdges())
        {
            lines.Add(edge.CoordinateLine);
        }

        return [.. lines];
    }

    /// <summary>Gets the Voronoi cells associated with the triangulation points.</summary>
    /// <returns>The Voronoi cell paths.</returns>
    public CoordinatePath[] GetCells()
    {
        List<CoordinatePath> cells = [];
        foreach (var cell in _delaunator.GetVoronoiCells())
        {
            cells.Add(cell.Path);
        }

        return [.. cells];
    }

    /// <summary>Calculates contour lines at the specified Z coordinates.</summary>
    /// <param name="zs">The Z coordinates at which to calculate contours.</param>
    /// <returns>The generated contour lines.</returns>
    public ContourLine[] GetContourLines(double[] zs)
    {
        List<ContourLine> paths = [];

        foreach (Triangle3D triangle in _delaunator.GetTriangles())
        {
            List<Coordinates3d> trianglePoints = new(triangle.Points);
            foreach (double z in zs)
            {
                ContourLine? contourLine = GetContourLine(triangle, trianglePoints, z);
                if (contourLine.HasValue)
                {
                    paths.Add(contourLine.Value);
                }
            }
        }

        return [.. paths];
    }

    /// <summary>Finds the contour line for a triangle at one Z coordinate.</summary>
    /// <param name="triangle">The triangle to inspect.</param>
    /// <param name="trianglePoints">The triangle vertices.</param>
    /// <param name="z">The contour Z coordinate.</param>
    /// <returns>The contour line, or <see langword="null"/> when the triangle does not cross it.</returns>
    private static ContourLine? GetContourLine(Triangle3D triangle, List<Coordinates3d> trianglePoints, double z)
    {
        if (z < triangle.MinZ || z > triangle.MaxZ)
        {
            return null;
        }

        const int TriangleEdgeCount = 3;
        List<Coordinates> points = [];
        for (int i = 0; i < trianglePoints.Count; i++)
        {
            int nextIndex = i < TriangleEdgeCount - 1 ? i + 1 : 0;
            Coordinates3d? intersection = FindPointAtZ(trianglePoints[i], trianglePoints[nextIndex], z);
            if (intersection is not null)
            {
                points.Add(new(intersection.Value.X, intersection.Value.Y));
            }
        }

        return points.Count == 0 ? null : new ContourLine(CoordinatePath.Open(points), z);
    }

    /// <summary>Finds the intersection of a 3D line segment with a Z plane.</summary>
    /// <param name="startPoint">The start of the line segment.</param>
    /// <param name="endPoint">The end of the line segment.</param>
    /// <param name="z">The Z coordinate of the plane.</param>
    /// <returns>The intersection point, or <see langword="null"/> when both endpoints are on the same side.</returns>
    private static Coordinates3d? FindPointAtZ(Coordinates3d startPoint, Coordinates3d endPoint, double z)
    {
        bool bothValuesOnSameSideOfContour = (startPoint.Z < z && endPoint.Z < z) || (startPoint.Z > z && endPoint.Z > z);
        if (bothValuesOnSameSideOfContour)
        {
            return null;
        }

        double t = (z - startPoint.Z) / (endPoint.Z - startPoint.Z);
        double x = startPoint.X + (t * (endPoint.X - startPoint.X));
        double y = startPoint.Y + (t * (endPoint.Y - startPoint.Y));
        return new Coordinates3d(x, y, z);
    }
}
