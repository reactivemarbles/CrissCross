// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Creates and renders contour lines from scattered or gridded data.</summary>
public class ContourLines : IPlottable, IHasLine
{
    /// <summary>Gets or sets the number of contour lines when explicit levels are not supplied.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public int ContourLineCount = 10;

    /// <summary>Gets or sets the explicit contour levels, or <see langword="null"/> to calculate them.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public double[]? ContourLineLevels;

    /// <summary>The number of endpoint-inclusive levels added to the requested contour count.</summary>
    private const int EndpointLevelCount = 2;

    /// <summary>The cached axis limits for the current contour lines.</summary>
    private AxisLimits _axisLimits = AxisLimits.NoLimits;

    /// <summary>Indicates whether the current data came from a rectangular grid.</summary>
    private bool _rectangularGrid;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the contour lines currently displayed.</summary>
    public List<ContourLine> Lines { get; private set; } = [];

    /// <summary>Gets or sets the line style used to render contours.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1, Color = Colors.Black };

    /// <summary>Gets or sets the label style used for rectangular-grid contour labels.</summary>
    public LabelStyle LabelStyle { get; set; } = new();

    /// <summary>Gets or sets the contour line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the contour line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the contour line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the colormap used to color contours by height.</summary>
    public IColormap? Colormap { get; set; }

    /// <summary>Gets the minimum z-value in the source data.</summary>
    public double MinZ { get; private set; }

    /// <summary>Gets the maximum z-value in the source data.</summary>
    public double MaxZ { get; private set; }

    /// <summary>Gets the axis limits occupied by the contour lines.</summary>
    /// <returns>The cached axis limits.</returns>
    public AxisLimits GetAxisLimits() => _axisLimits;

    /// <summary>Updates contour lines from arbitrarily placed data points.</summary>
    /// <param name="coordinates">The scattered three-dimensional coordinates.</param>
    public void Update(IEnumerable<Coordinates3d> coordinates)
    {
        _rectangularGrid = false;
        Triangulation.TriangulatedIrregularNetwork tin = new(coordinates);
        MinZ = tin.MinZ;
        MaxZ = tin.MaxZ;
        double[] levels = ContourLineLevels ?? tin.GetZsByCount(ContourLineCount);
        Lines.Clear();
        Lines.AddRange(tin.GetContourLines(levels));
        UpdateAxisLimits();
    }

    /// <summary>Updates contour lines from a rectangular grid of coordinates.</summary>
    /// <param name="coordinateGrid">The rectangular grid of three-dimensional coordinates.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "A rectangular grid is the natural input and existing public signature for this API.")]
    public void Update(Coordinates3d[, ] coordinateGrid)
    {
        _rectangularGrid = true;
        bool hasCoordinates = false;
        double minZ = 0;
        double maxZ = 0;

        foreach (Coordinates3d coordinate in coordinateGrid)
        {
            if (!hasCoordinates)
            {
                minZ = coordinate.Z;
                maxZ = coordinate.Z;
                hasCoordinates = true;
                continue;
            }

            minZ = Math.Min(minZ, coordinate.Z);
            maxZ = Math.Max(maxZ, coordinate.Z);
        }

        if (!hasCoordinates)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        MinZ = minZ;
        MaxZ = maxZ;
        double[] levels = ContourLineLevels ?? GetAutomaticLevels();
        Lines = Triangulation.RectangularGrid.GetContourLines(coordinateGrid, levels);
        UpdateAxisLimits();
    }

    /// <summary>Renders the current contour lines.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || Lines.Count == 0)
        {
            return;
        }

        for (int i = 0; i < Lines.Count; i++)
        {
            PixelPath path = Axes.GetPixelPath(Lines[i].Path);

            if (Colormap is not null)
            {
                double fraction = (Lines[i].Z - MinZ) / (MaxZ - MinZ);
                LineStyle.Color = Colormap.GetColor(fraction);
            }

            if (_rectangularGrid)
            {
                Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle, Lines[i].Z.ToString("0.###"), LabelStyle);
            }
            else
            {
                Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
            }
        }
    }

    /// <summary>Creates contour levels evenly spaced between the data bounds.</summary>
    /// <returns>The automatically generated contour levels.</returns>
    private double[] GetAutomaticLevels()
    {
        int levelCount = ContourLineCount + EndpointLevelCount;
        double[] levels = new double[levelCount];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = MinZ + ((MaxZ - MinZ) * i / (ContourLineCount + 1));
        }

        return levels;
    }

    /// <summary>Recalculates the axis limits from the current contour paths.</summary>
    private void UpdateAxisLimits()
    {
        ExpandingAxisLimits limits = new();
        foreach (ContourLine line in Lines)
        {
            foreach (Coordinates point in line.Path.Points)
            {
                limits.Expand(point);
            }
        }

        _axisLimits = limits.AxisLimits;
    }
}
