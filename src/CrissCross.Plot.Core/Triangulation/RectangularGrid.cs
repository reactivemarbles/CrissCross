// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Extracts contour lines from values arranged on a rectangular coordinate grid.</summary>
public static class RectangularGrid
{
    /// <summary>The bit assigned to a lower-left vertex above the contour level.</summary>
    private const int BottomLeft = 1;

    /// <summary>The bit assigned to a lower-right vertex above the contour level.</summary>
    private const int BottomRight = 2;

    /// <summary>The bit assigned to an upper-right vertex above the contour level.</summary>
    private const int TopRight = 4;

    /// <summary>The bit assigned to an upper-left vertex above the contour level.</summary>
    private const int TopLeft = 8;

    /// <summary>The marching-squares index when all four vertices are above the contour.</summary>
    private const int AllCornersAbove = BottomLeft | BottomRight | TopRight | TopLeft;

    /// <summary>The first ambiguous marching-squares saddle case.</summary>
    private const int SaddleCaseA = BottomLeft | TopRight;

    /// <summary>The second ambiguous marching-squares saddle case.</summary>
    private const int SaddleCaseB = BottomRight | TopLeft;

    /// <summary>The number of vertices used to compute a cell's average Z value.</summary>
    private const int CornerCount = 4;

    /// <summary>The tolerance used to recognize a contour whose endpoints coincide.</summary>
    private const double ClosedPathTolerance = 1e-10;

    /// <summary>Creates contour lines for the requested Z levels on a rectangular grid.</summary>
    /// <param name="coordinateGrid">The coordinates and Z values arranged by row and column.</param>
    /// <param name="zs">The Z levels at which to extract contours.</param>
    /// <param name="fixContourDirections">Whether to orient closed contours from the top and open contours from the left.</param>
    /// <returns>The contour lines, grouped by the order of the requested Z levels.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "The rectangular multidimensional array is part of the established public API; converting it to a jagged array would change the API and require callers to copy their data.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "SST2309",
        Justification = "The optional default is part of the established public API and must remain for source compatibility.")]
    public static List<ContourLine> GetContourLines(Coordinates3d[, ] coordinateGrid, double[] zs, bool fixContourDirections = true)
    {
        var gridWidth = coordinateGrid.GetLength(1);
        List<ContourLine> result = [];

        foreach (var z in zs)
        {
            Dictionary<int, EdgeLine> edgeLines = [];
            foreach (var edgeLine in MarchingSquares(coordinateGrid, z))
            {
                edgeLines.Add(edgeLine.CellID, edgeLine);
            }

            foreach (var mergedPath in MergeContourParts(edgeLines, gridWidth))
            {
                List<Coordinates> points = [];
                foreach (var edge in mergedPath)
                {
                    points.Add(edge.Interpolate(coordinateGrid, z));
                }

                result.Add(new(CoordinatePath.Open(points), z));
            }
        }

        return fixContourDirections ? FixContourDirections(result) : result;
    }

    /// <summary>Orients closed contours from their highest point and open contours from their leftmost endpoint.</summary>
    /// <param name="contours">The contours to orient.</param>
    /// <returns>The same list with its contour paths reoriented in place.</returns>
    public static List<ContourLine> FixContourDirections(List<ContourLine> contours)
    {
        for (var i = 0; i < contours.Count; i++)
        {
            var points = contours[i].Path.Points;
            if (IsClosed(points))
            {
                FixClosedContourDirection(contours, i, points);
                continue;
            }

            if (points[0].X > points[^1].X)
            {
                ReverseOpenContour(contours, i, points);
            }
        }

        return contours;
    }

    /// <summary>Determines the marching-squares edge lines for every cell in the grid.</summary>
    /// <param name="coordinateGrid">The coordinates and Z values arranged by row and column.</param>
    /// <param name="z">The contour level.</param>
    /// <returns>The edge lines produced by the non-empty grid cells.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "The rectangular multidimensional array is passed through unchanged from the established public API; conversion would require copying the grid.")]
    private static IEnumerable<EdgeLine> MarchingSquares(Coordinates3d[, ] coordinateGrid, double z)
    {
        var gridWidth = coordinateGrid.GetLength(1);
        for (var j = 0; j < coordinateGrid.GetLength(0) - 1; j++)
        {
            for (var i = 0; i < gridWidth - 1; i++)
            {
                var edgeLine = CreateCellEdgeLine(coordinateGrid, z, i, j);
                if (edgeLine is not null)
                {
                    yield return edgeLine;
                }
            }
        }
    }

    /// <summary>Creates edge lines for one cell, or returns <see langword="null"/> when the cell has no contour.</summary>
    /// <param name="coordinateGrid">The coordinates and Z values arranged by row and column.</param>
    /// <param name="z">The contour level.</param>
    /// <param name="i">The column of the cell's lower-left vertex.</param>
    /// <param name="j">The row of the cell's lower-left vertex.</param>
    /// <returns>The cell's edge lines, or <see langword="null"/> for an empty cell.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "Internal helpers preserve the established rectangular-grid API without allocating a jagged copy.")]
    private static EdgeLine? CreateCellEdgeLine(Coordinates3d[, ] coordinateGrid, double z, int i, int j)
    {
        Vertex lowerLeft = new(i, j);
        Vertex upperLeft = new(i, j + 1);
        Vertex lowerRight = new(i + 1, j);
        Vertex upperRight = new(i + 1, j + 1);
        var cellID = (j * coordinateGrid.GetLength(1)) + i;
        var index = GetCellIndex(coordinateGrid, z, lowerLeft, lowerRight, upperRight, upperLeft);

        if (index is 0 or AllCornersAbove)
        {
            return null;
        }

        IEdge left = new VerticalEdge(lowerLeft);
        IEdge right = new VerticalEdge(lowerRight);
        IEdge bottom = new HorizontalEdge(lowerLeft);
        IEdge top = new HorizontalEdge(upperLeft);

        return index switch
        {
            BottomLeft or (AllCornersAbove - BottomLeft) => new EdgeLine(left, bottom, cellID),
            BottomRight or (AllCornersAbove - BottomRight) => new EdgeLine(bottom, right, cellID),
            (BottomLeft | BottomRight) or (AllCornersAbove - (BottomLeft | BottomRight)) => new EdgeLine(left, right, cellID),
            TopRight or (AllCornersAbove - TopRight) => new EdgeLine(top, right, cellID),
            SaddleCaseA => new EdgeLinePair(left, top, bottom, right, cellID),
            (BottomRight | TopRight) or (AllCornersAbove - (BottomRight | TopRight)) => new EdgeLine(bottom, top, cellID),
            (BottomLeft | BottomRight | TopRight) or TopLeft => new EdgeLine(left, top, cellID),
            SaddleCaseB => new EdgeLinePair(left, bottom, top, right, cellID),
            _ => throw new InvalidOperationException("Unexpected marching-squares case."),
        };
    }

    /// <summary>Calculates the four-corner marching-squares case and resolves saddle cases.</summary>
    /// <param name="coordinateGrid">The coordinates and Z values arranged by row and column.</param>
    /// <param name="z">The contour level.</param>
    /// <param name="lowerLeft">The cell's lower-left vertex.</param>
    /// <param name="lowerRight">The cell's lower-right vertex.</param>
    /// <param name="upperRight">The cell's upper-right vertex.</param>
    /// <param name="upperLeft">The cell's upper-left vertex.</param>
    /// <returns>The case index for the cell.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "Internal helpers preserve the established rectangular-grid API without allocating a jagged copy.")]
    private static int GetCellIndex(Coordinates3d[, ] coordinateGrid, double z, Vertex lowerLeft, Vertex lowerRight, Vertex upperRight, Vertex upperLeft)
    {
        var index = 0;
        if (coordinateGrid[lowerLeft.j, lowerLeft.i].Z > z)
        {
            index |= BottomLeft;
        }

        if (coordinateGrid[lowerRight.j, lowerRight.i].Z > z)
        {
            index |= BottomRight;
        }

        if (coordinateGrid[upperRight.j, upperRight.i].Z > z)
        {
            index |= TopRight;
        }

        if (coordinateGrid[upperLeft.j, upperLeft.i].Z > z)
        {
            index |= TopLeft;
        }

        return index is SaddleCaseA or SaddleCaseB
            ? ResolveSaddleCase(coordinateGrid, z, index, lowerLeft, lowerRight, upperRight, upperLeft)
            : index;
    }

    /// <summary>Uses the cell's average Z value to choose a consistent saddle-case connection.</summary>
    /// <param name="coordinateGrid">The coordinates and Z values arranged by row and column.</param>
    /// <param name="z">The contour level.</param>
    /// <param name="index">The saddle-case index.</param>
    /// <param name="lowerLeft">The cell's lower-left vertex.</param>
    /// <param name="lowerRight">The cell's lower-right vertex.</param>
    /// <param name="upperRight">The cell's upper-right vertex.</param>
    /// <param name="upperLeft">The cell's upper-left vertex.</param>
    /// <returns>The resolved saddle-case index.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "Internal helpers preserve the established rectangular-grid API without allocating a jagged copy.")]
    private static int ResolveSaddleCase(Coordinates3d[, ] coordinateGrid, double z, int index, Vertex lowerLeft, Vertex lowerRight, Vertex upperRight, Vertex upperLeft)
    {
        var midpoint = coordinateGrid[lowerLeft.j, lowerLeft.i].Z
                       + coordinateGrid[lowerRight.j, lowerRight.i].Z
                       + coordinateGrid[upperRight.j, upperRight.i].Z
                       + coordinateGrid[upperLeft.j, upperLeft.i].Z;
        midpoint /= CornerCount;
        return midpoint <= z ? AllCornersAbove - index : index;
    }

    /// <summary>Merges adjacent cell edge lines into complete contour paths.</summary>
    /// <param name="edgeLines">The edge lines keyed by their source cell IDs.</param>
    /// <param name="gridWidth">The number of columns in the coordinate grid.</param>
    /// <returns>The merged contour paths.</returns>
    private static IEnumerable<IEnumerable<IEdge>> MergeContourParts(Dictionary<int, EdgeLine> edgeLines, int gridWidth)
    {
        while (edgeLines.Count > 0)
        {
            using var enumerator = edgeLines.GetEnumerator();
            _ = enumerator.MoveNext();
            var startEntry = enumerator.Current;

            var startLine = startEntry.Value;
            _ = edgeLines.Remove(startEntry.Key);

            var rightSearch = FindNeighborsChain(startLine.second, startLine.CellID, edgeLines, gridWidth);
            var leftSearch = FindNeighborsChain(startLine.first, startLine.CellID, edgeLines, gridWidth);
            yield return JoinContourPath(leftSearch, startLine.first, startLine.second, rightSearch);
        }
    }

    /// <summary>Joins the reversed left chain, source edge line, and right chain into one path.</summary>
    /// <param name="leftSearch">The chain extending from the first source edge.</param>
    /// <param name="first">The first source edge.</param>
    /// <param name="second">The second source edge.</param>
    /// <param name="rightSearch">The chain extending from the second source edge.</param>
    /// <returns>The edges in contour path order.</returns>
    private static IEnumerable<IEdge> JoinContourPath(IEnumerable<IEdge> leftSearch, IEdge first, IEdge second, IEnumerable<IEdge> rightSearch)
    {
        List<IEdge> leftEdges = [];
        leftEdges.AddRange(leftSearch);

        for (var i = leftEdges.Count - 1; i >= 0; i--)
        {
            yield return leftEdges[i];
        }

        yield return first;
        yield return second;

        foreach (var edge in rightSearch)
        {
            yield return edge;
        }
    }

    /// <summary>Follows neighboring cells until the edge chain reaches a boundary.</summary>
    /// <param name="startEdge">The edge from which to begin following the chain.</param>
    /// <param name="startCellId">The ID of the cell containing the starting edge.</param>
    /// <param name="edgeLineLookup">The remaining edge lines keyed by cell ID.</param>
    /// <param name="gridWidth">The number of columns in the coordinate grid.</param>
    /// <returns>The edges found along the chain.</returns>
    private static IEnumerable<IEdge> FindNeighborsChain(IEdge startEdge, int startCellId, Dictionary<int, EdgeLine> edgeLineLookup, int gridWidth)
    {
        IEdge current = startEdge;
        var currentCellId = startCellId;
        while (true)
        {
            var key = GetNeighborCell(current, currentCellId, gridWidth);
            if (!edgeLineLookup.TryGetValue(key, out var candidate))
            {
                yield break;
            }

            _ = edgeLineLookup.Remove(key);
            current = GetNextEdge(candidate, current, key, edgeLineLookup);
            yield return current;
            currentCellId = candidate.CellID;
        }
    }

    /// <summary>Chooses the edge continuing the contour and restores the unused half of a saddle cell.</summary>
    /// <param name="candidate">The neighboring cell's edge line.</param>
    /// <param name="current">The edge shared with the current cell.</param>
    /// <param name="key">The neighboring cell ID.</param>
    /// <param name="edgeLineLookup">The remaining edge lines keyed by cell ID.</param>
    /// <returns>The edge continuing the chain.</returns>
    private static IEdge GetNextEdge(EdgeLine candidate, IEdge current, int key, Dictionary<int, EdgeLine> edgeLineLookup)
    {
        if (candidate is not EdgeLinePair pair)
        {
            return candidate.first.Equals(current) ? candidate.second : candidate.first;
        }

        if (pair.first.Equals(current))
        {
            edgeLineLookup.Add(key, new(pair.first1, pair.second1, pair.CellID));
            return pair.second;
        }

        if (pair.second.Equals(current))
        {
            edgeLineLookup.Add(key, new(pair.first1, pair.second1, pair.CellID));
            return pair.first;
        }

        if (pair.first1.Equals(current))
        {
            edgeLineLookup.Add(key, new(pair.first, pair.second, pair.CellID));
            return pair.second1;
        }

        if (pair.second1.Equals(current))
        {
            edgeLineLookup.Add(key, new(pair.first, pair.second, pair.CellID));
            return pair.first1;
        }

        return current;
    }

    /// <summary>Gets the neighboring cell across an edge.</summary>
    /// <param name="edge">The edge shared with the neighboring cell.</param>
    /// <param name="edgeCell">The current cell ID.</param>
    /// <param name="gridWidth">The number of columns in the coordinate grid.</param>
    /// <returns>The neighboring cell ID.</returns>
    private static int GetNeighborCell(IEdge edge, int edgeCell, int gridWidth)
    {
        if (edge is VerticalEdge)
        {
            var edgeInCell = (edge.First.j * gridWidth) + edge.First.i;
            return edgeInCell == edgeCell ? edgeCell - 1 : edgeCell + 1;
        }

        if (edge is HorizontalEdge)
        {
            var edgeInCell = (edge.First.j * gridWidth) + edge.First.i;
            return edgeInCell == edgeCell ? edgeCell - gridWidth : edgeCell + gridWidth;
        }

        return 0;
    }

    /// <summary>Determines whether the first and last points are within the closed-path tolerance.</summary>
    /// <param name="points">The points in the contour path.</param>
    /// <returns><see langword="true"/> when the path endpoints coincide within tolerance.</returns>
    private static bool IsClosed(Coordinates[] points) =>
        Math.Abs(points[0].X - points[^1].X) < ClosedPathTolerance
        && Math.Abs(points[0].Y - points[^1].Y) < ClosedPathTolerance;

    /// <summary>Rotates a closed contour so its highest point is first.</summary>
    /// <param name="contours">The contours to update.</param>
    /// <param name="contourIndex">The index of the contour to rotate.</param>
    /// <param name="points">The points of the contour to rotate.</param>
    private static void FixClosedContourDirection(List<ContourLine> contours, int contourIndex, Coordinates[] points)
    {
        var maximumY = points[0].Y;
        var maximumIndex = 0;
        for (var i = 0; i < points.Length; i++)
        {
            if (points[i].Y > maximumY)
            {
                maximumY = points[i].Y;
                maximumIndex = i;
            }
        }

        var rotatedPoints = new Coordinates[points.Length + (maximumIndex == 0 ? 1 : 0)];
        var destinationIndex = 0;
        for (var i = maximumIndex; i < points.Length; i++)
        {
            rotatedPoints[destinationIndex] = points[i];
            destinationIndex++;
        }

        for (var i = 1; i < maximumIndex; i++)
        {
            rotatedPoints[destinationIndex] = points[i];
            destinationIndex++;
        }

        rotatedPoints[destinationIndex] = points[maximumIndex];
        contours[contourIndex] = new(CoordinatePath.Open(rotatedPoints), contours[contourIndex].Z);
    }

    /// <summary>Reverses an open contour so its leftmost endpoint is first.</summary>
    /// <param name="contours">The contours to update.</param>
    /// <param name="contourIndex">The index of the contour to reverse.</param>
    /// <param name="points">The points of the contour to reverse.</param>
    private static void ReverseOpenContour(List<ContourLine> contours, int contourIndex, Coordinates[] points)
    {
        var reversedPoints = new Coordinates[points.Length];
        for (var i = 0; i < points.Length; i++)
        {
            reversedPoints[i] = points[points.Length - i - 1];
        }

        contours[contourIndex] = new(CoordinatePath.Open(reversedPoints), contours[contourIndex].Z);
    }
}
