// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a marker at a data-coordinate location.</summary>
public class Marker : IPlottable, IHasMarker, IHasLegendText
{
    /// <summary>Gets or sets the x-coordinate of the marker.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the y-coordinate of the marker.</summary>
    public double Y { get; set; }

    /// <summary>Gets or sets the marker location.</summary>
    public Coordinates Location
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>Gets or sets the marker position.</summary>
    public Coordinates Position
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>Gets or sets the marker coordinates.</summary>
    public Coordinates Coordinates
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>Gets or sets the legacy legend label.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    [Obsolete("use LegendText")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this marker in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this marker is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new();

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor
    {
        get => MarkerStyle.LineColor;
        set
        {
            MarkerStyle.LineColor = value;
            MarkerStyle.OutlineColor = value;
        }
    }

    /// <summary>Gets or sets both the marker fill and line colors.</summary>
    public Color MarkerColor
    {
        get => MarkerFillColor;
        set
        {
            MarkerFillColor = value;
            MarkerLineColor = value;
        }
    }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets or sets the marker size through the legacy size property.</summary>
    public float Size { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets both the marker line and outline widths.</summary>
    public float LineWidth
    {
        get => MarkerStyle.LineWidth;
        set
        {
            MarkerStyle.LineWidth = value;
            MarkerStyle.OutlineWidth = value;
        }
    }

    /// <summary>Gets or sets the marker shape through the legacy shape property.</summary>
    public MarkerShape Shape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets both the marker fill and line colors.</summary>
    public Color Color
    {
        get => MarkerFillColor;
        set
        {
            MarkerFillColor = value;
            MarkerLineColor = value;
        }
    }

    /// <summary>Gets or sets the axes used to position and render this marker.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item represented by this marker.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, MarkerStyle);

    /// <summary>Gets the axis limits occupied by this marker.</summary>
    /// <returns>The axis limits of the marker location.</returns>
    public AxisLimits GetAxisLimits() => new(Location);

    /// <summary>Renders this marker.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp) => Drawing.DrawMarker(rp.Canvas, rp.Paint, Axes.GetPixel(Location), MarkerStyle);
}
