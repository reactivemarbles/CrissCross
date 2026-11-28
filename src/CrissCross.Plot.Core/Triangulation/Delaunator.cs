// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

/*
--- NOTICE BEGIN ---

MIT License

Copyright (c) 2019 Patryk Grech

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

--- NOTICE END ---
*/

namespace CrissCross.Plot.Triangulation;

/// <summary>Creates and queries a Delaunay triangulation.</summary>
public class Delaunator
{
    /// <summary>The number of vertices in each triangle.</summary>
    private const int EdgePointCount = 3;

    /// <summary>The maximum number of pending edge flips.</summary>
    private const int EdgeStackCapacity = 512;

    /// <summary>The number of components in a projected coordinate.</summary>
    private const int CoordinateComponentCount = 2;

    /// <summary>The final vertex offset within a triangle.</summary>
    private const int LastTriangleVertexOffset = 2;

    /// <summary>The point-count factor in the planar triangle bound.</summary>
    private const int PlanarTriangleFactor = 2;

    /// <summary>The offset in the planar triangle bound.</summary>
    private const int PlanarTriangleOffset = 5;

    /// <summary>The upper-half pseudo-angle offset.</summary>
    private const int PseudoAngleUpperOffset = 3;

    /// <summary>The pseudo-angle normalization factor.</summary>
    private const int PseudoAngleScale = 4;

    /// <summary>The maximum partition size sorted by insertion sort.</summary>
    private const int InsertionSortThreshold = 20;

    /// <summary>The initial capacity used for Voronoi vertices.</summary>
    private const int InitialVoronoiVertexCapacity = 10;

    /// <summary>One half, used in circumcenter calculations.</summary>
    private const double Half = 0.5;

    /// <summary>The area below which a polygon is treated as degenerate.</summary>
    private const double MinimumCentroidArea = 1E-7;

    /// <summary>The factor applied to the accumulated polygon area.</summary>
    private const double CentroidAreaScale = 3.0;

    /// <summary>The tolerance used to skip near-duplicate points.</summary>
    private readonly double _epsilon = Math.Pow(2, -52);

    /// <summary>The stack of edges awaiting legalization.</summary>
    private readonly int[] _edgeStack = new int[EdgeStackCapacity];

    /// <summary>The size of the hull hash table.</summary>
    private readonly int _hashSize;

    /// <summary>The packed projected coordinates.</summary>
    private readonly double[] _coordinates;

    /// <summary>The previous point for each hull point.</summary>
    private readonly int[] _hullPrev = [];

    /// <summary>The next point for each hull point.</summary>
    private readonly int[] _hullNext = [];

    /// <summary>The boundary triangle associated with each hull point.</summary>
    private readonly int[] _hullTri = [];

    /// <summary>The hash table used to locate hull points.</summary>
    private readonly int[] _hullHash;

    /// <summary>The circumcenter X coordinate.</summary>
    private readonly double _cx;

    /// <summary>The circumcenter Y coordinate.</summary>
    private readonly double _cy;

    /// <summary>The first point in the hull.</summary>
    private int _hullStart;

    /// <summary>The number of points in the hull.</summary>
    private int _hullSize;

    /// <summary>The number of populated half-edges.</summary>
    private int _trianglesLen;

    /// <summary>Initializes a new instance of the <see cref="Delaunator"/> class.</summary>
    /// <param name="points">The points to triangulate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="points"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Fewer than three points are supplied.</exception>
    public Delaunator(Coordinates3d[] points)
    {
        ValidateNotNull(points, nameof(points));

        if (points.Length < 3)
        {
            throw new ArgumentException("Need at least 3 points", nameof(points));
        }

        Points = points;
        _coordinates = ProjectCoordinates(Points);

        var n = points.Length;
        var maxTriangles = (PlanarTriangleFactor * n) - PlanarTriangleOffset;

        Triangles = new int[maxTriangles * EdgePointCount];
        HalfEdges = new int[maxTriangles * EdgePointCount];
        _hashSize = (int)Math.Ceiling(Math.Sqrt(n));

        _hullPrev = new int[n];
        _hullNext = new int[n];
        _hullTri = new int[n];
        _hullHash = new int[_hashSize];

        var ids = new int[n];

        var (cx, cy) = InitializeIdsAndGetCenter(ids);

        var i0 = FindCenterSeed(cx, cy);
        var i0x = _coordinates[CoordinateComponentCount * i0];
        var i0y = _coordinates[(CoordinateComponentCount * i0) + 1];

        var i1 = FindNearestSeed(i0, i0x, i0y);
        var i1x = _coordinates[CoordinateComponentCount * i1];
        var i1y = _coordinates[(CoordinateComponentCount * i1) + 1];

        var (i2, minRadius) = FindThirdSeed(i0, i1, i0x, i0y, i1x, i1y);

        var i2x = _coordinates[CoordinateComponentCount * i2];
        var i2y = _coordinates[(CoordinateComponentCount * i2) + 1];

        ValidateSeedRadius(minRadius);

        if (Orient(i0x, i0y, i1x, i1y, i2x, i2y))
        {
            var i = i1;
            var x = i1x;
            var y = i1y;
            i1 = i2;
            i1x = i2x;
            i1y = i2y;
            i2 = i;
            i2x = x;
            i2y = y;
        }

        var center = Circumcenter(i0x, i0y, i1x, i1y, i2x, i2y);
        _cx = center.X;
        _cy = center.Y;

        InitializeSeedHull(ids, i0, i1, i2);

        InsertPoints(ids, i0, i1, i2);

        Hull = CreateHullIndices();

        _hullTri = [];
        _hullNext = [];
        _hullPrev = [];

        //// trim typed triangle mesh arrays
        var triangles = new int[_trianglesLen];
        var halfEdges = new int[_trianglesLen];
        Array.Copy(Triangles, triangles, _trianglesLen);
        Array.Copy(HalfEdges, halfEdges, _trianglesLen);
        Triangles = triangles;
        HalfEdges = halfEdges;
    }

    /// <summary>Gets the point index at the start of each half-edge.</summary>
    public int[] Triangles { get; }

    /// <summary>Gets the opposite half-edge in the adjacent triangle, or -1 if there is no adjacent triangle.</summary>
    public int[] HalfEdges { get; }

    /// <summary>Gets the initial points used to construct the triangulation.</summary>
    public Coordinates3d[] Points { get; }

    /// <summary>Gets the point indices traversing the hull.</summary>
    public int[] Hull { get; }

    /// <summary>Creates closed edges from an enumerable of points.</summary>
    /// <param name="points">The points in hull order.</param>
    /// <returns>The closed hull edges.</returns>
    public static IEnumerable<Edge3D> CreateHull(IEnumerable<Coordinates3d> points)
    {
        ValidateNotNull(points, nameof(points));

        return EnumerateHullEdges(points);
    }

    /// <summary>Gets the circumcenter of three points.</summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <param name="c">The third point.</param>
    /// <returns>The circumcenter.</returns>
    public static Coordinates3d GetCircumcenter(Coordinates3d a, Coordinates3d b, Coordinates3d c) => Circumcenter(a.X, a.Y, b.X, b.Y, c.X, c.Y);

    /// <summary>Gets the centroid of a polygon.</summary>
    /// <param name="points">The polygon vertices.</param>
    /// <returns>The polygon centroid, or the default coordinate for a degenerate polygon.</returns>
    public static Coordinates3d GetCentroid(Coordinates3d[] points)
    {
        ValidateNotNull(points, nameof(points));

        double accumulatedArea = 0.0;
        double centerX = 0.0;
        double centerY = 0.0;

        for (int i = 0, j = points.Length - 1; i < points.Length; j = i, i++)
        {
            var temp = (points[i].X * points[j].Y) - (points[j].X * points[i].Y);
            accumulatedArea += temp;
            centerX += (points[i].X + points[j].X) * temp;
            centerY += (points[i].Y + points[j].Y) * temp;
        }

        if (Math.Abs(accumulatedArea) < MinimumCentroidArea)
        {
            return default;
        }

        accumulatedArea *= CentroidAreaScale;
        return new(centerX / accumulatedArea, centerY / accumulatedArea, 0);
    }

    /// <summary>Gets the next half-edge in a triangle.</summary>
    /// <param name="e">The half-edge index.</param>
    /// <returns>The next half-edge index.</returns>
    public static int NextHalfedge(int e) => ((e % EdgePointCount) + 1 == EdgePointCount) ? e - (EdgePointCount - 1) : e + 1;

    /// <summary>Gets the previous half-edge in a triangle.</summary>
    /// <param name="e">The half-edge index.</param>
    /// <returns>The previous half-edge index.</returns>
    public static int PreviousHalfedge(int e) => (e % EdgePointCount == 0) ? e + (EdgePointCount - 1) : e - 1;

    /// <summary>Returns the three half-edges of a given triangle id.</summary>
    /// <param name="t">The triangle id.</param>
    /// <returns>The three half-edges.</returns>
    public static int[] EdgesOfTriangle(int t) => [EdgePointCount * t, (EdgePointCount * t) + 1, (EdgePointCount * t) + LastTriangleVertexOffset];

    /// <summary>Gets the triangle id of a half-edge.</summary>
    /// <param name="e">The half-edge index.</param>
    /// <returns>The triangle id.</returns>
    public static int TriangleOfEdge(int e) => e / EdgePointCount;

    /// <summary>Gets all triangles in the triangulation.</summary>
    /// <returns>The triangulated triangles.</returns>
    public IEnumerable<Triangle3D> GetTriangles()
    {
        for (var t = 0; t < Triangles.Length / EdgePointCount; t++)
        {
            yield return new Triangle3D(t, GetTrianglePoints(t));
        }
    }

    /// <summary>Gets the unique edges of the triangulation.</summary>
    /// <returns>The triangulation edges.</returns>
    public IEnumerable<Edge3D> GetEdges()
    {
        for (var e = 0; e < Triangles.Length; e++)
        {
            if (e > HalfEdges[e])
            {
                var p = Points[Triangles[e]];
                var q = Points[Triangles[NextHalfedge(e)]];
                yield return new Edge3D(e, p, q);
            }
        }
    }

    /// <summary>Gets Voronoi edges using a triangle vertex selector.</summary>
    /// <param name="triangleVerticeSelector">The triangle vertex selector.</param>
    /// <returns>The Voronoi edges.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "SST2309", Justification = "The existing optional parameter and its null default are part of the public API.")]
    public IEnumerable<Edge3D> GetVoronoEdges(Func<int, Coordinates3d>? triangleVerticeSelector = null)
    {
        triangleVerticeSelector ??= GetCentroid;
        for (var e = 0; e < Triangles.Length; e++)
        {
            if (e < HalfEdges[e])
            {
                var p = triangleVerticeSelector(TriangleOfEdge(e));
                var q = triangleVerticeSelector(TriangleOfEdge(HalfEdges[e]));
                yield return new Edge3D(e, p, q);
            }
        }
    }

    /// <summary>Gets Voronoi edges based on triangle circumcenters.</summary>
    /// <returns>The Voronoi edges.</returns>
    public IEnumerable<Edge3D> GetVoronoEdgesBasedOnCircumCenter() => GetVoronoEdges(GetTriangleCircumcenter);

    /// <summary>Gets Voronoi edges based on triangle centroids.</summary>
    /// <returns>The Voronoi edges.</returns>
    public IEnumerable<Edge3D> GetVoronoEdgesBasedOnCentroids() => GetVoronoEdges(GetCentroid);

    /// <summary>Gets Voronoi cells using a triangle vertex selector.</summary>
    /// <param name="triangleVerticeSelector">The triangle vertex selector.</param>
    /// <returns>The Voronoi cells.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "SST2309", Justification = "The existing optional parameter and its null default are part of the public API.")]
    public IEnumerable<VoronoiCell> GetVoronoiCells(Func<int, Coordinates3d>? triangleVerticeSelector = null)
    {
        triangleVerticeSelector ??= GetCentroid;

        var seen = new HashSet<int>();
        var vertices = new List<Coordinates3d>(InitialVoronoiVertexCapacity); // Keep it outside the loop, reuse capacity, less resizes.

        for (var e = 0; e < Triangles.Length; e++)
        {
            var pointIndex = Triangles[NextHalfedge(e)];

            // True if element was added, If resize the set? O(n) : O(1)
            if (seen.Add(pointIndex))
            {
                foreach (var edge in EdgesAroundPoint(e))
                {
                    // triangleVerticeSelector cant be null, no need to check before invoke (?.).
                    vertices.Add(triangleVerticeSelector.Invoke(TriangleOfEdge(edge)));
                }

                yield return new VoronoiCell(pointIndex, vertices.ToArray());
                vertices.Clear(); // Clear elements, keep capacity
            }
        }
    }

    /// <summary>Gets Voronoi cells based on triangle circumcenters.</summary>
    /// <returns>The Voronoi cells.</returns>
    public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCircumcenters() => GetVoronoiCells(GetTriangleCircumcenter);

    /// <summary>Gets Voronoi cells based on triangle centroids.</summary>
    /// <returns>The Voronoi cells.</returns>
    public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCentroids() => GetVoronoiCells(GetCentroid);

    /// <summary>Gets the edges of the convex hull.</summary>
    /// <returns>The hull edges.</returns>
    public IEnumerable<Edge3D> GetHullEdges() => CreateHull(GetHullPoints());

    /// <summary>Gets the convex hull points in traversal order.</summary>
    /// <returns>The hull points.</returns>
    public Coordinates3d[] GetHullPoints() => Array.ConvertAll(Hull, x => Points[x]);

    /// <summary>Gets the vertices of a triangle.</summary>
    /// <param name="t">The triangle index.</param>
    /// <returns>The triangle vertices.</returns>
    public Coordinates3d[] GetTrianglePoints(int t)
    {
        var points = new List<Coordinates3d>();
        foreach (var p in PointsOfTriangle(t))
        {
            points.Add(Points[p]);
        }

        return points.ToArray();
    }

    /// <summary>Gets relaxed points based on Voronoi circumcenters.</summary>
    /// <returns>The relaxed points.</returns>
    public Coordinates3d[] GetRelaxedPoints()
    {
        var points = new List<Coordinates3d>();
        foreach (var cell in GetVoronoiCellsBasedOnCircumcenters())
        {
            points.Add(GetCentroid(cell.Points));
        }

        return points.ToArray();
    }

    /// <summary>Gets the edges surrounding a triangle.</summary>
    /// <param name="t">The triangle index.</param>
    /// <returns>The triangle edges.</returns>
    public IEnumerable<Edge3D> GetEdgesOfTriangle(int t) => CreateHull(ProjectTrianglePoints(EdgesOfTriangle(t)));

    /// <summary>Gets the circumcenter of a triangle.</summary>
    /// <param name="t">The triangle index.</param>
    /// <returns>The circumcenter.</returns>
    public Coordinates3d GetTriangleCircumcenter(int t)
    {
        var vertices = GetTrianglePoints(t);
        return GetCircumcenter(vertices[0], vertices[1], vertices[2]);
    }

    /// <summary>Gets the centroid of a triangle.</summary>
    /// <param name="t">The triangle index.</param>
    /// <returns>The centroid.</returns>
    public Coordinates3d GetCentroid(int t)
    {
        var vertices = GetTrianglePoints(t);
        return GetCentroid(vertices);
    }

    /// <summary>Returns the half-edges that share a start point with the given half edge, in order.</summary>
    /// <param name="start">The starting half-edge.</param>
    /// <returns>The surrounding half-edges.</returns>
    public IEnumerable<int> EdgesAroundPoint(int start)
    {
        var incoming = start;
        do
        {
            yield return incoming;
            var outgoing = NextHalfedge(incoming);
            incoming = HalfEdges[outgoing];
        } while (incoming != -1 && incoming != start);
    }

    /// <summary>Returns the three point indices of a given triangle id.</summary>
    /// <param name="t">The triangle id.</param>
    /// <returns>The three point indices.</returns>
    public IEnumerable<int> PointsOfTriangle(int t)
    {
        foreach (var edge in EdgesOfTriangle(t))
        {
            yield return Triangles[edge];
        }
    }

    /// <summary>Returns the triangle ids adjacent to the given triangle id. Will return up to three values.</summary>
    /// <param name="t">The triangle id.</param>
    /// <returns>The adjacent triangle ids.</returns>
    public IEnumerable<int> TrianglesAdjacentToTriangle(int t)
    {
        var adjacentTriangles = new List<int>();
        foreach (var e in EdgesOfTriangle(t))
        {
            var opposite = HalfEdges[e];
            if (opposite >= 0)
            {
                adjacentTriangles.Add(TriangleOfEdge(opposite));
            }
        }

        return adjacentTriangles;
    }

    /// <summary>Stores the planar coordinates of the input points.</summary>
    /// <param name="points">The input points.</param>
    /// <returns>The packed X and Y coordinates.</returns>
    private static double[] ProjectCoordinates(Coordinates3d[] points)
    {
        var coordinates = new double[points.Length * CoordinateComponentCount];
        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];
            coordinates[CoordinateComponentCount * i] = point.X;
            coordinates[(CoordinateComponentCount * i) + 1] = point.Y;
        }

        return coordinates;
    }

    /// <summary>Tests whether a point lies inside a triangle circumcircle.</summary>
    /// <param name="a">The first triangle vertex.</param>
    /// <param name="b">The second triangle vertex.</param>
    /// <param name="c">The third triangle vertex.</param>
    /// <param name="point">The point being tested.</param>
    /// <returns>Whether the point is inside the circumcircle.</returns>
    private static bool InCircle((double X, double Y) a, (double X, double Y) b, (double X, double Y) c, (double X, double Y) point)
    {
        var dx = a.X - point.X;
        var dy = a.Y - point.Y;
        var ex = b.X - point.X;
        var ey = b.Y - point.Y;
        var fx = c.X - point.X;
        var fy = c.Y - point.Y;

        var ap = (dx * dx) + (dy * dy);
        var bp = (ex * ex) + (ey * ey);
        var cp = (fx * fx) + (fy * fy);

        return (dx * ((ey * cp) - (bp * fy)))
               - (dy * ((ex * cp) - (bp * fx)))
               + (ap * ((ex * fy) - (ey * fx))) < 0;
    }

    /// <summary>Calculates a normalized pseudo-angle for a vector.</summary>
    /// <param name="dx">The vector X component.</param>
    /// <param name="dy">The vector Y component.</param>
    /// <returns>The normalized pseudo-angle.</returns>
    private static double PseudoAngle(double dx, double dy)
    {
        var p = dx / (Math.Abs(dx) + Math.Abs(dy));
        return (dy > 0 ? PseudoAngleUpperOffset - p : 1 + p) / PseudoAngleScale; // [0..1]
    }

    /// <summary>Sorts point indices by their associated distances.</summary>
    /// <param name="ids">The point indices to sort.</param>
    /// <param name="dists">The distance for each point.</param>
    /// <param name="left">The first index to sort.</param>
    /// <param name="right">The last index to sort.</param>
    private static void Quicksort(int[] ids, double[] dists, int left, int right)
    {
        if (right - left <= InsertionSortThreshold)
        {
            InsertionSort(ids, dists, left, right);
            return;
        }

        var i = PreparePivot(ids, dists, left, right);
        var j = right;

        var temp = ids[i];
        var tempDist = dists[temp];
        while (true)
        {
            do
            {
                i++;
            }
            while (dists[ids[i]] < tempDist);
            do
            {
                j--;
            }
            while (dists[ids[j]] > tempDist);
            if (j < i)
            {
                break;
            }

            Swap(ids, i, j);
        }

        ids[left + 1] = ids[j];
        ids[j] = temp;

        if (right - i + 1 >= j - left)
        {
            Quicksort(ids, dists, i, right);
            Quicksort(ids, dists, left, j - 1);
        }
        else
        {
            Quicksort(ids, dists, left, j - 1);
            Quicksort(ids, dists, i, right);
        }
    }

    /// <summary>Places the median pivot between the partition endpoints.</summary>
    /// <param name="ids">The point indices to sort.</param>
    /// <param name="dists">The distance for each point.</param>
    /// <param name="left">The first index to sort.</param>
    /// <param name="right">The last index to sort.</param>
    /// <returns>The pivot index.</returns>
    private static int PreparePivot(int[] ids, double[] dists, int left, int right)
    {
        var median = (left + right) >> 1;
        var i = left + 1;
        Swap(ids, median, i);
        if (dists[ids[left]] > dists[ids[right]])
        {
            Swap(ids, left, right);
        }

        if (dists[ids[i]] > dists[ids[right]])
        {
            Swap(ids, i, right);
        }

        if (dists[ids[left]] > dists[ids[i]])
        {
            Swap(ids, left, i);
        }

        return i;
    }

    /// <summary>Sorts a small partition by insertion.</summary>
    /// <param name="ids">The point indices to sort.</param>
    /// <param name="dists">The distance for each point.</param>
    /// <param name="left">The first index to sort.</param>
    /// <param name="right">The last index to sort.</param>
    private static void InsertionSort(int[] ids, double[] dists, int left, int right)
    {
        for (var i = left + 1; i <= right; i++)
        {
            var temp = ids[i];
            var tempDist = dists[temp];
            var j = i - 1;
            while (j >= left && dists[ids[j]] > tempDist)
            {
                ids[j + 1] = ids[j];
                j--;
            }

            ids[j + 1] = temp;
        }
    }

    /// <summary>Swaps two point indices.</summary>
    /// <param name="arr">The array to update.</param>
    /// <param name="i">The first array index.</param>
    /// <param name="j">The second array index.</param>
    private static void Swap(int[] arr, int i, int j)
    {
        var tmp = arr[i];
        arr[i] = arr[j];
        arr[j] = tmp;
    }

    /// <summary>Tests the orientation of three points.</summary>
    /// <param name="px">The first point X coordinate.</param>
    /// <param name="py">The first point Y coordinate.</param>
    /// <param name="qx">The second point X coordinate.</param>
    /// <param name="qy">The second point Y coordinate.</param>
    /// <param name="rx">The third point X coordinate.</param>
    /// <param name="ry">The third point Y coordinate.</param>
    /// <returns>Whether the points have the tested orientation.</returns>
    private static bool Orient(double px, double py, double qx, double qy, double rx, double ry) => (qy - py) * (rx - qx) < (qx - px) * (ry - qy);

    /// <summary>Calculates the squared circumradius of three points.</summary>
    /// <param name="ax">The first point X coordinate.</param>
    /// <param name="ay">The first point Y coordinate.</param>
    /// <param name="bx">The second point X coordinate.</param>
    /// <param name="by">The second point Y coordinate.</param>
    /// <param name="cx">The third point X coordinate.</param>
    /// <param name="cy">The third point Y coordinate.</param>
    /// <returns>The squared circumradius.</returns>
    private static double Circumradius(double ax, double ay, double bx, double by, double cx, double cy)
    {
        var dx = bx - ax;
        var dy = by - ay;
        var ex = cx - ax;
        var ey = cy - ay;
        var bl = (dx * dx) + (dy * dy);
        var cl = (ex * ex) + (ey * ey);
        var d = Half / ((dx * ey) - (dy * ex));
        var x = ((ey * bl) - (dy * cl)) * d;
        var y = ((dx * cl) - (ex * bl)) * d;
        return (x * x) + (y * y);
    }

    /// <summary>Calculates the circumcenter of three points.</summary>
    /// <param name="ax">The first point X coordinate.</param>
    /// <param name="ay">The first point Y coordinate.</param>
    /// <param name="bx">The second point X coordinate.</param>
    /// <param name="by">The second point Y coordinate.</param>
    /// <param name="cx">The third point X coordinate.</param>
    /// <param name="cy">The third point Y coordinate.</param>
    /// <returns>The circumcenter.</returns>
    private static Coordinates3d Circumcenter(double ax, double ay, double bx, double by, double cx, double cy)
    {
        var dx = bx - ax;
        var dy = by - ay;
        var ex = cx - ax;
        var ey = cy - ay;
        var bl = (dx * dx) + (dy * dy);
        var cl = (ex * ex) + (ey * ey);
        var d = Half / ((dx * ey) - (dy * ex));
        var x = ax + (((ey * bl) - (dy * cl)) * d);
        var y = ay + (((dx * cl) - (ex * bl)) * d);

        return new(x, y, 0);
    }

    /// <summary>Calculates the squared distance between two points.</summary>
    /// <param name="ax">The first point X coordinate.</param>
    /// <param name="ay">The first point Y coordinate.</param>
    /// <param name="bx">The second point X coordinate.</param>
    /// <param name="by">The second point Y coordinate.</param>
    /// <returns>The squared distance.</returns>
    private static double Dist(double ax, double ay, double bx, double by)
    {
        var dx = ax - bx;
        var dy = ay - by;
        return (dx * dx) + (dy * dy);
    }

    /// <summary>Rejects a positive-infinite seed circumradius.</summary>
    /// <param name="radius">The smallest squared circumradius.</param>
    /// <exception cref="Exception">No Delaunay triangulation exists for the input.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA2201",
        Justification = "The public constructor historically throws exactly System.Exception for this input; changing its type changes observable behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2409",
        Justification = "The public constructor historically throws exactly System.Exception for this input; changing its type changes observable behavior.")]
    private static void ValidateSeedRadius(double radius)
    {
        if (radius != double.PositiveInfinity)
        {
            return;
        }

        throw new Exception("No Delaunay triangulation exists for this input.");
    }

    /// <summary>Enumerates adjacent pairs, closing the hull with a fresh first-point enumeration.</summary>
    /// <param name="points">The points in hull order.</param>
    /// <returns>The lazily evaluated closed hull edges.</returns>
    private static IEnumerable<Edge3D> EnumerateHullEdges(IEnumerable<Coordinates3d> points)
    {
        using var left = points.GetEnumerator();
        var hasLeft = left.MoveNext();
        if (!hasLeft)
        {
            yield break;
        }

        using (var tail = points.GetEnumerator())
        {
            var skipped = tail.MoveNext();
            while (hasLeft && skipped && tail.MoveNext())
            {
                yield return new Edge3D(0, left.Current, tail.Current);
                hasLeft = left.MoveNext();
            }
        }

        if (!hasLeft)
        {
            yield break;
        }

        using var first = points.GetEnumerator();
        if (!first.MoveNext())
        {
            yield break;
        }

        yield return new Edge3D(0, left.Current, first.Current);
        _ = left.MoveNext();
    }

    /// <summary>Throws when a public method receives a null reference.</summary>
    /// <typeparam name="T">The validated reference type.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The public parameter name.</param>
    private static void ValidateNotNull<T>(T value, string parameterName)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
    }

    /// <summary>Copies the linked hull into traversal order.</summary>
    /// <returns>The hull point indices.</returns>
    private int[] CreateHullIndices()
    {
        var hull = new int[_hullSize];
        var current = _hullStart;
        for (var i = 0; i < _hullSize; i++)
        {
            hull[i] = current;
            current = _hullNext[current];
        }

        return hull;
    }

    /// <summary>Sorts the points and initializes the seed triangle hull.</summary>
    /// <param name="ids">The point indices to sort.</param>
    /// <param name="i0">The first seed index.</param>
    /// <param name="i1">The second seed index.</param>
    /// <param name="i2">The third seed index.</param>
    private void InitializeSeedHull(int[] ids, int i0, int i1, int i2)
    {
        var dists = new double[ids.Length];
        for (var i = 0; i < ids.Length; i++)
        {
            dists[i] = Dist(_coordinates[CoordinateComponentCount * i], _coordinates[(CoordinateComponentCount * i) + 1], _cx, _cy);
        }

        // sort the points by distance from the seed triangle circumcenter
        Quicksort(ids, dists, 0, ids.Length - 1);

        // set up the seed triangle as the starting hull
        _hullStart = i0;
        _hullSize = EdgePointCount;

        _hullPrev[i2] = i1;
        _hullNext[i0] = i1;
        _hullPrev[i0] = i2;
        _hullNext[i1] = i2;
        _hullPrev[i1] = i0;
        _hullNext[i2] = i0;

        _hullTri[i0] = 0;
        _hullTri[i1] = 1;
        _hullTri[i2] = LastTriangleVertexOffset;

        _hullHash[HashKey(_coordinates[CoordinateComponentCount * i0], _coordinates[(CoordinateComponentCount * i0) + 1])] = i0;
        _hullHash[HashKey(_coordinates[CoordinateComponentCount * i1], _coordinates[(CoordinateComponentCount * i1) + 1])] = i1;
        _hullHash[HashKey(_coordinates[CoordinateComponentCount * i2], _coordinates[(CoordinateComponentCount * i2) + 1])] = i2;

        _trianglesLen = 0;
        _ = AddTriangle(i0, i1, i2, -1, -1, -1);
    }

    /// <summary>Inserts the non-seed points into the hull.</summary>
    /// <param name="ids">The point indices in distance order.</param>
    /// <param name="i0">The first seed index.</param>
    /// <param name="i1">The second seed index.</param>
    /// <param name="i2">The third seed index.</param>
    private void InsertPoints(int[] ids, int i0, int i1, int i2)
    {
        double xp = 0;
        double yp = 0;

        for (var k = 0; k < ids.Length; k++)
        {
            var i = ids[k];
            var x = _coordinates[CoordinateComponentCount * i];
            var y = _coordinates[(CoordinateComponentCount * i) + 1];

            // skip near-duplicate points
            if (k > 0 && Math.Abs(x - xp) <= _epsilon && Math.Abs(y - yp) <= _epsilon)
            {
                continue;
            }

            xp = x;
            yp = y;

            // skip seed triangle points
            if (i == i0 || i == i1 || i == i2)
            {
                continue;
            }

            InsertPointIntoHull(i, x, y);
        }
    }

    /// <summary>Inserts a point into the visible hull edges.</summary>
    /// <param name="i">The point index.</param>
    /// <param name="x">The point X coordinate.</param>
    /// <param name="y">The point Y coordinate.</param>
    private void InsertPointIntoHull(int i, double x, double y)
    {
        // find a visible edge on the convex hull using edge hash
        var start = 0;
        for (var j = 0; j < _hashSize; j++)
        {
            var key = HashKey(x, y);
            start = _hullHash[(key + j) % _hashSize];
            if (start != -1 && start != _hullNext[start])
            {
                break;
            }
        }

        start = _hullPrev[start];
        var e = start;
        var q = _hullNext[e];

        while (!Orient(
            x,
            y,
            _coordinates[CoordinateComponentCount * e],
            _coordinates[(CoordinateComponentCount * e) + 1],
            _coordinates[CoordinateComponentCount * q],
            _coordinates[(CoordinateComponentCount * q) + 1]))
        {
            e = q;
            if (e == start)
            {
                e = int.MaxValue;
                break;
            }

            q = _hullNext[e];
        }

        if (e == int.MaxValue)
        {
            return; // likely a near-duplicate point; skip it
        }

        // add the first triangle from the point
        var t = AddTriangle(e, i, _hullNext[e], -1, -1, _hullTri[e]);

        // recursively flip triangles from the point until they satisfy the Delaunay condition
        _hullTri[i] = Legalize(t + LastTriangleVertexOffset);
        _hullTri[e] = t; // keep track of boundary triangles on the hull
        _hullSize++;

        var next = WalkForward(i, x, y, e);
        if (e == start)
        {
            e = WalkBackward(i, x, y, e);
        }

        // update the hull indices
        _hullPrev[i] = e;
        _hullStart = e;
        _hullPrev[next] = i;
        _hullNext[e] = i;
        _hullNext[i] = next;

        // save the two new edges in the hash table
        _hullHash[HashKey(x, y)] = i;
        _hullHash[HashKey(_coordinates[CoordinateComponentCount * e], _coordinates[(CoordinateComponentCount * e) + 1])] = e;
    }

    /// <summary>Walks forward along the hull while inserting triangles.</summary>
    /// <param name="i">The point index.</param>
    /// <param name="x">The point X coordinate.</param>
    /// <param name="y">The point Y coordinate.</param>
    /// <param name="e">The starting hull edge.</param>
    /// <returns>The next surviving hull point.</returns>
    private int WalkForward(int i, double x, double y, int e)
    {
        var next = _hullNext[e];
        var q = _hullNext[next];

        while (Orient(
            x,
            y,
            _coordinates[CoordinateComponentCount * next],
            _coordinates[(CoordinateComponentCount * next) + 1],
            _coordinates[CoordinateComponentCount * q],
            _coordinates[(CoordinateComponentCount * q) + 1]))
        {
            var t = AddTriangle(next, i, q, _hullTri[i], -1, _hullTri[next]);
            _hullTri[i] = Legalize(t + LastTriangleVertexOffset);
            _hullNext[next] = next; // mark as removed
            _hullSize--;
            next = q;

            q = _hullNext[next];
        }

        return next;
    }

    /// <summary>Walks backward along the hull while inserting triangles.</summary>
    /// <param name="i">The point index.</param>
    /// <param name="x">The point X coordinate.</param>
    /// <param name="y">The point Y coordinate.</param>
    /// <param name="e">The starting hull edge.</param>
    /// <returns>The previous surviving hull point.</returns>
    private int WalkBackward(int i, double x, double y, int e)
    {
        var q = _hullPrev[e];

        while (Orient(
            x,
            y,
            _coordinates[CoordinateComponentCount * q],
            _coordinates[(CoordinateComponentCount * q) + 1],
            _coordinates[CoordinateComponentCount * e],
            _coordinates[(CoordinateComponentCount * e) + 1]))
        {
            var t = AddTriangle(q, i, e, -1, _hullTri[e], _hullTri[q]);
            _ = Legalize(t + LastTriangleVertexOffset);
            _hullTri[q] = t;
            _hullNext[e] = e; // mark as removed
            _hullSize--;
            e = q;

            q = _hullPrev[e];
        }

        return e;
    }

    /// <summary>Finds the point nearest the center of the input bounds.</summary>
    /// <param name="cx">The center X coordinate.</param>
    /// <param name="cy">The center Y coordinate.</param>
    /// <returns>The seed point index.</returns>
    private int FindCenterSeed(double cx, double cy)
    {
        var minDist = double.PositiveInfinity;
        var i0 = 0;

        // pick a seed point close to the center
        for (int i = 0; i < Points.Length; i++)
        {
            var d = Dist(cx, cy, _coordinates[CoordinateComponentCount * i], _coordinates[(CoordinateComponentCount * i) + 1]);
            if (d < minDist)
            {
                i0 = i;
                minDist = d;
            }
        }

        return i0;
    }

    /// <summary>Finds the nearest distinct point to the first seed.</summary>
    /// <param name="i0">The first seed index.</param>
    /// <param name="i0x">The first seed X coordinate.</param>
    /// <param name="i0y">The first seed Y coordinate.</param>
    /// <returns>The second seed index.</returns>
    private int FindNearestSeed(int i0, double i0x, double i0y)
    {
        var minDist = double.PositiveInfinity;

        var i1 = 0;

        // find the point closest to the seed
        for (int i = 0; i < Points.Length; i++)
        {
            if (i == i0)
            {
                continue;
            }

            var d = Dist(i0x, i0y, _coordinates[CoordinateComponentCount * i], _coordinates[(CoordinateComponentCount * i) + 1]);
            if (d < minDist && d > 0)
            {
                i1 = i;
                minDist = d;
            }
        }

        return i1;
    }

    /// <summary>Finds the third seed with the smallest circumcircle.</summary>
    /// <param name="i0">The first seed index.</param>
    /// <param name="i1">The second seed index.</param>
    /// <param name="i0x">The first seed X coordinate.</param>
    /// <param name="i0y">The first seed Y coordinate.</param>
    /// <param name="i1x">The second seed X coordinate.</param>
    /// <param name="i1y">The second seed Y coordinate.</param>
    /// <returns>The third seed index and squared radius.</returns>
    private (int Index, double Radius) FindThirdSeed(int i0, int i1, double i0x, double i0y, double i1x, double i1y)
    {
        var minRadius = double.PositiveInfinity;

        var i2 = 0;

        // find the third point which forms the smallest circumcircle with the first two
        for (int i = 0; i < Points.Length; i++)
        {
            if (i == i0 || i == i1)
            {
                continue;
            }

            var r = Circumradius(i0x, i0y, i1x, i1y, _coordinates[CoordinateComponentCount * i], _coordinates[(CoordinateComponentCount * i) + 1]);
            if (r < minRadius)
            {
                i2 = i;
                minRadius = r;
            }
        }

        return (i2, minRadius);
    }

    /// <summary>Initializes the point indices and calculates the center of their bounds.</summary>
    /// <param name="ids">The point indices to initialize.</param>
    /// <returns>The bounding box center.</returns>
    private (double X, double Y) InitializeIdsAndGetCenter(int[] ids)
    {
        var minX = double.PositiveInfinity;
        var minY = double.PositiveInfinity;
        var maxX = double.NegativeInfinity;
        var maxY = double.NegativeInfinity;

        for (var i = 0; i < ids.Length; i++)
        {
            var x = _coordinates[CoordinateComponentCount * i];
            var y = _coordinates[(CoordinateComponentCount * i) + 1];
            if (x < minX)
            {
                minX = x;
            }

            if (y < minY)
            {
                minY = y;
            }

            if (x > maxX)
            {
                maxX = x;
            }

            if (y > maxY)
            {
                maxY = y;
            }

            ids[i] = i;
        }

        return ((minX + maxX) / CoordinateComponentCount, (minY + maxY) / CoordinateComponentCount);
    }

    /// <summary>Projects triangle edge indices onto the point array.</summary>
    /// <param name="edges">The triangle edge indices.</param>
    /// <returns>The corresponding points.</returns>
    private IEnumerable<Coordinates3d> ProjectTrianglePoints(int[] edges)
    {
        foreach (var edge in edges)
        {
            yield return Points[edge];
        }
    }

    /// <summary>Flips edges until the Delaunay condition is satisfied.</summary>
    /// <param name="a">The half-edge to legalize.</param>
    /// <returns>The last opposite half-edge within the triangle.</returns>
    private int Legalize(int a)
    {
        var i = 0;
        int ar;

        // recursion eliminated with a fixed-size stack
        while (true)
        {
            var b = HalfEdges[a];

            /* if the pair of triangles doesn't satisfy the Delaunay condition
             * (p1 is inside the circumcircle of [p0, pl, pr]), flip them,
             * then do the same check/flip recursively for the new pair of triangles
             *
             *           pl                    pl
             *          /||\                  /  \
             *       al/ || \bl            al/    \a
             *        /  ||  \              /      \
             *       /  a||b  \    flip    /___ar___\
             *     p0\   ||   /p1   =>   p0\---bl---/p1
             *        \  ||  /              \      /
             *       ar\ || /br             b\    /br
             *          \||/                  \  /
             *           pr                    pr
             */
            int a0 = a - (a % EdgePointCount);
            ar = a0 + ((a + (EdgePointCount - 1)) % EdgePointCount);

            if (b == -1)
            { // convex hull edge
                if (i == 0)
                {
                    break;
                }

                i--;
                a = _edgeStack[i];
                continue;
            }

            var b0 = b - (b % EdgePointCount);
            var al = a0 + ((a + 1) % EdgePointCount);
            var bl = b0 + ((b + (EdgePointCount - 1)) % EdgePointCount);

            var p0 = Triangles[ar];
            var pr = Triangles[a];
            var pl = Triangles[al];
            var p1 = Triangles[bl];

            var illegal = InCircle(
                (_coordinates[CoordinateComponentCount * p0], _coordinates[(CoordinateComponentCount * p0) + 1]),
                (_coordinates[CoordinateComponentCount * pr], _coordinates[(CoordinateComponentCount * pr) + 1]),
                (_coordinates[CoordinateComponentCount * pl], _coordinates[(CoordinateComponentCount * pl) + 1]),
                (_coordinates[CoordinateComponentCount * p1], _coordinates[(CoordinateComponentCount * p1) + 1]));

            if (illegal)
            {
                FlipEdge(a, b, ar, bl, p0, p1);

                var br = b0 + ((b + 1) % EdgePointCount);

                // don't worry about hitting the cap: it can only happen on extremely degenerate input
                if (i < _edgeStack.Length)
                {
                    _edgeStack[i] = br;
                    i++;
                }
            }
            else
            {
                if (i == 0)
                {
                    break;
                }

                i--;
                a = _edgeStack[i];
            }
        }

        return ar;
    }

    /// <summary>Flips an illegal edge and repairs its half-edge links.</summary>
    /// <param name="a">The first half-edge.</param>
    /// <param name="b">The opposite half-edge.</param>
    /// <param name="ar">The right edge of the first triangle.</param>
    /// <param name="bl">The left edge of the opposite triangle.</param>
    /// <param name="p0">The point opposite the first half-edge.</param>
    /// <param name="p1">The point opposite the second half-edge.</param>
    private void FlipEdge(int a, int b, int ar, int bl, int p0, int p1)
    {
        Triangles[a] = p1;
        Triangles[b] = p0;

        var hbl = HalfEdges[bl];

        // edge swapped on the other side of the hull (rare); fix the halfedge reference
        if (hbl == -1)
        {
            var e = _hullStart;
            do
            {
                if (_hullTri[e] == bl)
                {
                    _hullTri[e] = a;
                    break;
                }

                e = _hullPrev[e];
            } while (e != _hullStart);
        }

        Link(a, hbl);
        Link(b, HalfEdges[ar]);
        Link(ar, bl);
    }

    /// <summary>Adds a triangle and links its adjacent half-edges.</summary>
    /// <param name="i0">The first point index.</param>
    /// <param name="i1">The second point index.</param>
    /// <param name="i2">The third point index.</param>
    /// <param name="a">The first opposite half-edge.</param>
    /// <param name="b">The second opposite half-edge.</param>
    /// <param name="c">The third opposite half-edge.</param>
    /// <returns>The first new half-edge index.</returns>
    private int AddTriangle(int i0, int i1, int i2, int a, int b, int c)
    {
        var t = _trianglesLen;

        Triangles[t] = i0;
        Triangles[t + 1] = i1;
        Triangles[t + LastTriangleVertexOffset] = i2;

        Link(t, a);
        Link(t + 1, b);
        Link(t + LastTriangleVertexOffset, c);

        _trianglesLen += EdgePointCount;
        return t;
    }

    /// <summary>Links a pair of opposite half-edges.</summary>
    /// <param name="a">The first half-edge.</param>
    /// <param name="b">The opposite half-edge, or -1 at the hull.</param>
    private void Link(int a, int b)
    {
        HalfEdges[a] = b;
        if (b == -1)
        {
            return;
        }

        HalfEdges[b] = a;
    }

    /// <summary>Calculates a hull hash key for a point.</summary>
    /// <param name="x">The point X coordinate.</param>
    /// <param name="y">The point Y coordinate.</param>
    /// <returns>The hash table index.</returns>
    private int HashKey(double x, double y) => (int)(Math.Floor(PseudoAngle(x - _cx, y - _cy) * _hashSize) % _hashSize);
}
