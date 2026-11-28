// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Triangulation;

namespace CrissCross.Plot.Plottables;

/// <summary>Renders a triangulated irregular network and its associated geometric overlays.</summary>
public class TINPlot : IPlottable
{
    /// <summary>The triangulated network rendered by this plot.</summary>
    private TriangulatedIrregularNetwork _triangulatedNetwork = null!;

    /// <summary>Initializes a new instance of the <see cref="TINPlot"/> class.</summary>
    /// <param name="coordinates">The three-dimensional points used to create the network.</param>
    public TINPlot(IEnumerable<Coordinates3d> coordinates)
    {
        Update(coordinates);
    }

    /// <summary>Gets or sets the text displayed for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this network.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, MarkerStyle);

    /// <summary>Gets or sets the contour line interval.</summary>
    public double ContourLineInterval { get; set; } = 0.25;

    /// <summary>Gets or sets the marker style for network points.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new(MarkerShape.FilledCircle, Constants.DefaultMarkerSize, Colors.Red);

    /// <summary>Gets or sets the line style for the network edges.</summary>
    public LineStyle NetworkLineStyle { get; set; } = new(Constants.DefaultLineWidth, Colors.Red);

    /// <summary>Gets or sets the marker style for Voronoi cells.</summary>
    public MarkerStyle VoronoiMarkerStyle { get; set; } = new(MarkerShape.FilledCircle, Constants.DefaultMarkerSize, Colors.Blue);

    /// <summary>Gets or sets the line style for Voronoi cells.</summary>
    public LineStyle VoronoiLineStyle { get; set; } = new(Constants.DefaultLineWidth, Colors.Blue);

    /// <summary>Gets or sets the line style for contour lines.</summary>
    public LineStyle ContourLineStyle { get; set; } = new(Constants.DefaultLineWidth, Colors.Gray, LinePattern.DenselyDashed);

    /// <summary>Gets the axis limits occupied by the triangulated network.</summary>
    /// <returns>The two-dimensional axis limits of the network.</returns>
    public AxisLimits GetAxisLimits() => _triangulatedNetwork.AxisLimits.AxisLimits2d;

    /// <summary>Replaces the current network points and rebuilds the triangulation.</summary>
    /// <param name="coordinates">The three-dimensional points used to create the network.</param>
    public void Update(IEnumerable<Coordinates3d> coordinates) => _triangulatedNetwork = new(coordinates);

    /// <summary>Renders the triangulated network and its overlays.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        DrawNetworkLines(rp, rp.Paint);
        DrawMarkers(rp, rp.Paint);
        DrawVoronoi(rp, rp.Paint);
        DrawContours(rp, rp.Paint);
    }

    /// <summary>Renders the triangulated network edges.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawNetworkLines(RenderPack rp, Paint paint)
    {
        foreach (CoordinateLine line in _triangulatedNetwork.GetLines())
        {
            PixelLine pixelLine = Axes.GetPixelLine(line);
            Drawing.DrawLine(rp.Canvas, paint, pixelLine, NetworkLineStyle);
        }
    }

    /// <summary>Renders the Voronoi cell outlines and markers.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawVoronoi(RenderPack rp, Paint paint)
    {
        foreach (CoordinatePath cell in _triangulatedNetwork.GetCells())
        {
            List<Pixel> pixels = [];
            foreach (Coordinates point in cell.Points)
            {
                pixels.Add(Axes.GetPixel(point));
            }

            Drawing.DrawMarkers(rp.Canvas, paint, pixels, VoronoiMarkerStyle);
            Drawing.DrawLines(rp.Canvas, paint, pixels, VoronoiLineStyle);
        }
    }

    /// <summary>Renders the network's input points.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawMarkers(RenderPack rp, Paint paint)
    {
        List<Pixel> markerPixels = [];
        foreach (Coordinates3d point in _triangulatedNetwork.Points3D)
        {
            markerPixels.Add(Axes.GetPixel(point.Coordinates2d()));
        }

        Drawing.DrawMarkers(rp.Canvas, paint, markerPixels, MarkerStyle);
    }

    /// <summary>Renders the network's contour lines.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void DrawContours(RenderPack rp, Paint paint)
    {
        double[] contourLevels = _triangulatedNetwork.GetZsByInterval(ContourLineInterval);
        foreach (ContourLine line in _triangulatedNetwork.GetContourLines(contourLevels))
        {
            List<Pixel> pixels = [];
            foreach (Coordinates point in line.Path.Points)
            {
                pixels.Add(Axes.GetPixel(point));
            }

            Drawing.DrawLines(rp.Canvas, paint, pixels, ContourLineStyle);
        }
    }

    /// <summary>Contains constants used by triangulated network plots.</summary>
    internal static class Constants
    {
        /// <summary>The default point marker size in pixels.</summary>
        internal const float DefaultMarkerSize = 5;

        /// <summary>The default network line width in pixels.</summary>
        internal const float DefaultLineWidth = 1;
    }
}
