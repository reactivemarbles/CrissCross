// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a line segment with configurable line and marker styles.</summary>
public class LinePlot : IPlottable, IHasLine, IHasMarker, IHasLegendText
{
    /// <summary>Gets or sets the starting coordinate of the line.</summary>
    public Coordinates Start { get; set; }

    /// <summary>Gets or sets the ending coordinate of the line.</summary>
    public Coordinates End { get; set; }

    /// <summary>Gets or sets the coordinates of the line segment.</summary>
    public CoordinateLine Line
    {
        get => new(Start, End);
        set
        {
            Start = value.Start;
            End = value.End;
        }
    }

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { Size = Constants.NoMarkerSize };

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets or sets the legacy legend label.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this line.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item represented by this line.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle, MarkerStyle);

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets whether the line is rendered above the markers.</summary>
    public bool LineOnTop { get; set; } = true;

    /// <summary>Gets or sets whether the markers are rendered above the line.</summary>
    public bool MarkersOnTop { get => !LineOnTop; set => LineOnTop = !value; }

    /// <summary>Gets or sets the line color and marker fill color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineStyle.Color = value;
            MarkerStyle.FillColor = value;
        }
    }

    /// <summary>Gets the axis limits occupied by this line.</summary>
    /// <returns>The axis limits of the line endpoints.</returns>
    public AxisLimits GetAxisLimits()
    {
        CoordinateRect boundingRect = new(Start, End);
        return new(boundingRect);
    }

    /// <summary>Renders the line and markers in the configured order.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (LineOnTop)
        {
            DrawMarkers(rp, rp.Paint);
            DrawLine(rp, rp.Paint);
        }
        else
        {
            DrawLine(rp, rp.Paint);
            DrawMarkers(rp, rp.Paint);
        }
    }

    /// <summary>Renders markers at both line endpoints.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used to render the markers.</param>
    private void DrawMarkers(RenderPack rp, Paint paint)
    {
        Drawing.DrawMarker(rp.Canvas, paint, Axes.GetPixel(Start), MarkerStyle);
        Drawing.DrawMarker(rp.Canvas, paint, Axes.GetPixel(End), MarkerStyle);
    }

    /// <summary>Renders the line between its endpoints.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used to render the line.</param>
    private void DrawLine(RenderPack rp, Paint paint)
    {
        CoordinateLine line = new(Start, End);
        PixelLine pixelLine = Axes.GetPixelLine(line);
        Drawing.DrawLine(rp.Canvas, paint, pixelLine, LineStyle);
    }

    /// <summary>Contains the default marker-size value.</summary>
    private static class Constants
    {
        /// <summary>The default size that disables endpoint markers.</summary>
        public const float NoMarkerSize = 0;
    }
}
