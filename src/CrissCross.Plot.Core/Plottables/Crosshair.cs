// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Displays horizontal and vertical lines intersecting at a position.</summary>
public class Crosshair : IPlottable, IRenderLast, IHasMarker
{
    /// <summary>Gets or sets the obsolete font color field.</summary>
    [Obsolete("Use TextColor and TextBackgroundColor instead", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete field for source and binary compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained obsolete field for source and binary compatibility.")]
    public Color FontColor;

    /// <summary>Gets the horizontal line of the crosshair.</summary>
    public HorizontalLine HorizontalLine { get; } = new();

    /// <summary>Gets the vertical line of the crosshair.</summary>
    public VerticalLine VerticalLine { get; } = new();

    /// <summary>Gets or sets the crosshair position.</summary>
    public Coordinates Position
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>Gets or sets the x-coordinate of the crosshair.</summary>
    public double X { get => VerticalLine.X; set => VerticalLine.X = value; }

    /// <summary>Gets or sets the y-coordinate of the crosshair.</summary>
    public double Y { get => HorizontalLine.Y; set => HorizontalLine.Y = value; }

    /// <summary>Gets or sets the font size for both axis-line labels.</summary>
    public float FontSize
    {
        get => HorizontalLine.LabelFontSize;
        set
        {
            HorizontalLine.LabelFontSize = value;
            VerticalLine.LabelFontSize = value;
        }
    }

    /// <summary>Gets or sets whether both axis-line labels are bold.</summary>
    public bool FontBold
    {
        get => HorizontalLine.LabelBold;
        set
        {
            HorizontalLine.LabelBold = value;
            VerticalLine.LabelBold = value;
        }
    }

    /// <summary>Gets or sets the font name for both axis-line labels.</summary>
    public string FontName
    {
        get => HorizontalLine.LabelFontName;
        set
        {
            HorizontalLine.LabelFontName = value;
            VerticalLine.LabelFontName = value;
        }
    }

    /// <summary>Gets or sets the text color for both axis-line labels.</summary>
    public Color TextColor
    {
        get => HorizontalLine.LabelFontColor;
        set
        {
            HorizontalLine.LabelFontColor = value;
            VerticalLine.LabelFontColor = value;
        }
    }

    /// <summary>Gets or sets the background color for both axis-line labels.</summary>
    public Color TextBackgroundColor
    {
        get => HorizontalLine.LabelBackgroundColor;
        set
        {
            HorizontalLine.LabelBackgroundColor = value;
            VerticalLine.LabelBackgroundColor = value;
        }
    }

    /// <summary>Gets or sets the line color for both crosshair lines.</summary>
    public Color LineColor
    {
        get => HorizontalLine.LineColor;
        set
        {
            HorizontalLine.LineColor = value;
            VerticalLine.LineColor = value;
        }
    }

    /// <summary>Gets or sets the line width for both crosshair lines.</summary>
    public float LineWidth
    {
        get => HorizontalLine.LineWidth;
        set
        {
            HorizontalLine.LineWidth = value;
            VerticalLine.LineWidth = value;
        }
    }

    /// <summary>Gets or sets the line pattern for both crosshair lines.</summary>
    public LinePattern LinePattern
    {
        get => HorizontalLine.LinePattern;
        set
        {
            HorizontalLine.LinePattern = value;
            VerticalLine.LinePattern = value;
        }
    }

    /// <summary>Gets or sets the marker style for the crosshair intersection.</summary>
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

    /// <summary>Gets or sets the obsolete line style property.</summary>
    [Obsolete(
        "Assign values to LineColor, LineWidth, or LinePattern properties to set style for both lines. "
        + "HorizontalLine and VerticalLine have properties that can be set individually as well.",
        true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete property for source and binary compatibility.")]
    public LineStyle LineStyle { get; set; } = new();

    /// <summary>Gets or sets whether the crosshair contributes to autoscaling.</summary>
    public bool EnableAutoscale { get; set; }

    /// <summary>Gets or sets whether the crosshair is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this crosshair.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items from both crosshair lines.</summary>
    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            foreach (LegendItem item in HorizontalLine.LegendItems)
            {
                yield return item;
            }

            foreach (LegendItem item in VerticalLine.LegendItems)
            {
                yield return item;
            }
        }
    }

    /// <summary>Gets the axis limits occupied by the crosshair.</summary>
    /// <returns>The crosshair position limits, or no limits when autoscaling is disabled.</returns>
    public AxisLimits GetAxisLimits() => EnableAutoscale ? new(X, X, Y, Y) : AxisLimits.NoLimits;

    /// <summary>Renders both crosshair lines.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        HorizontalLine.Axes = Axes;
        HorizontalLine.Render(rp);

        VerticalLine.Axes = Axes;
        VerticalLine.Render(rp);
    }

    /// <summary>Renders both lines and the intersection marker in the final render pass.</summary>
    /// <param name="rp">The render context.</param>
    public void RenderLast(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        HorizontalLine.Axes = Axes;
        HorizontalLine.RenderLast(rp);

        VerticalLine.Axes = Axes;
        VerticalLine.RenderLast(rp);

        Pixel pixel = Axes.GetPixel(new(X, Y));
        MarkerStyle.Render(rp.Canvas, pixel, rp.Paint);
    }
}
