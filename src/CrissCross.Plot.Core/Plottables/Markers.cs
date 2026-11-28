// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders markers for a scatter data source.</summary>
/// <param name="data">The scatter data source rendered by this plottable.</param>
public class Markers(IScatterSource data) : IPlottable, IHasMarker, IHasLegendText
{
    /// <summary>Gets or sets the legacy legend label.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for these markers in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether these markers are visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render these markers.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { LineWidth = 1 };

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

    /// <summary>Gets the underlying scatter data source.</summary>
    public IScatterSource Data { get; } = data;

    /// <summary>Gets or sets both the marker fill and line colors.</summary>
    public Color Color
    {
        get => MarkerStyle.FillColor;
        set
        {
            MarkerStyle.FillColor = value;
            MarkerStyle.LineColor = value;
        }
    }

    /// <summary>Gets or sets the colormap used to color the markers.</summary>
    public IColormap? Colormap { get; set; }

    /// <summary>Gets the legend item represented by these markers.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, MarkerStyle);

    /// <summary>Gets the axis limits occupied by the scatter data.</summary>
    /// <returns>The axis limits of the data source.</returns>
    public AxisLimits GetAxisLimits() => Data.GetLimits();

    /// <summary>Renders the markers from the scatter data source.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        IReadOnlyList<Coordinates> points = Data.GetScatterPoints();
        if (MarkerStyle == MarkerStyle.None || points.Count == 0)
        {
            return;
        }

        Pixel[] markerPixels = new Pixel[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            markerPixels[i] = Axes.GetPixel(points[i]);
        }

        if (Colormap is not null)
        {
            Drawing.DrawMarkers(rp.Canvas, rp.Paint, markerPixels, MarkerStyle, Colormap);
        }
        else
        {
            Drawing.DrawMarkers(rp.Canvas, rp.Paint, markerPixels, MarkerStyle);
        }
    }
}
