// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Displays one or more boxes with shared styling.</summary>
public class BoxPlot : IPlottable, IHasLegendText
{
    /// <summary>Gets the mutable collection of boxes in this plot.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "The public field is retained for source and binary compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2322", Justification = "Callers intentionally mutate this public collection to configure the plot.")]
    public readonly List<Box> Boxes = [];

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets the legend items for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, Boxes[0].FillStyle);

    /// <summary>Gets or sets the fill color for all boxes.</summary>
    public Color FillColor
    {
        get => Boxes.Count > 0 ? Boxes[0].FillColor : default;
        set
        {
            foreach (Box box in Boxes)
            {
                box.FillColor = value;
            }
        }
    }

    /// <summary>Gets or sets the line color for all boxes.</summary>
    public Color LineColor
    {
        get => Boxes.Count > 0 ? Boxes[0].LineColor : default;
        set
        {
            foreach (Box box in Boxes)
            {
                box.LineColor = value;
            }
        }
    }

    /// <summary>Gets or sets the line width for all boxes.</summary>
    public float LineWidth
    {
        get => Boxes.Count > 0 ? Boxes[0].LineWidth : default;
        set
        {
            foreach (Box box in Boxes)
            {
                box.LineWidth = value;
            }
        }
    }

    /// <summary>Gets or sets the obsolete stroke color alias.</summary>
    [Obsolete("use LineColor", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source and binary compatibility.")]
    public Color StrokeColor
    {
        get => LineColor;
        set => LineColor = value;
    }

    /// <summary>Gets or sets the obsolete stroke width alias.</summary>
    [Obsolete("use LineWidth", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source and binary compatibility.")]
    public float StrokeWidth
    {
        get => LineWidth;
        set => LineWidth = value;
    }

    /// <summary>Gets the axis limits occupied by the boxes.</summary>
    /// <returns>The combined axis limits of the boxes.</returns>
    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits limits = new();

        foreach (Box box in Boxes)
        {
            limits.Expand(box.GetAxisLimits());
        }

        return limits.AxisLimits;
    }

    /// <summary>Renders all boxes in this plot.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        foreach (Box box in Boxes)
        {
            box.Render(rp, Axes);
        }
    }
}
