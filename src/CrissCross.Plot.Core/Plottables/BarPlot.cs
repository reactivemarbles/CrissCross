// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Holds a collection of individually styled bars.</summary>
public class BarPlot : IPlottable, IHasLegendText, IRenderLast
{
    /// <summary>Initializes a new instance of the <see cref="BarPlot"/> class.</summary>
    /// <param name="bars">The bars to plot.</param>
    public BarPlot(List<Bar> bars)
    {
        Bars = bars;
    }

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the bars in this plot.</summary>
    public List<Bar> Bars { get; } // TODO: bar plot data source?

    /// <summary>Gets or sets whether labels are displayed on top of all bars.</summary>
    public bool LabelsOnTop
    {
        get => Bars.Count > 0 && Bars.TrueForAll(static bar => bar.LabelOnTop);
        set => Bars.ForEach(bar => bar.LabelOnTop = value);
    }

    /// <summary>Gets or sets the style used to render bar value labels.</summary>
    public LabelStyle ValueLabelStyle { get; set; } = new() { Alignment = Alignment.LowerCenter, };

    /// <summary>
    /// Gets or sets text displayed above each bar, typically containing string representation of the value.
    /// This label is displayed below the bar for negative bars.
    /// </summary>
    public string ValueLabel
    {
        get => ValueLabelStyle.Text;
        set => ValueLabelStyle.Text = value;
    }

    /// <summary>Gets or sets the fill color for all bars.</summary>
    public Color Color
    {
        get => Bars.Count > 0 ? Bars[0].FillColor : default;
        set
        {
            foreach (Bar bar in Bars)
            {
                bar.FillColor = value;
            }
        }
    }

    /// <summary>Gets or sets whether all bars are horizontal.</summary>
    public bool Horizontal
    {
        get => Bars.Count > 0 && Bars.TrueForAll(static bar => bar.Orientation == Orientation.Horizontal);
        set
        {
            foreach (Bar bar in Bars)
            {
                bar.Orientation = value
                    ? Orientation.Horizontal
                    : Orientation.Vertical;
            }
        }
    }

    /// <summary>Gets the legend items for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            if (Bars.Count == 0)
            {
                return LegendItem.None;
            }

            LegendItem item = new() { Plottable = this, LabelText = LegendText, FillColor = Bars[0].FillColor, };

            return LegendItem.Single(item);
        }
    }

    /// <summary>Gets the axis limits occupied by these bars.</summary>
    /// <returns>The axis limits occupied by these bars.</returns>
    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits limits = new();

        foreach (Bar bar in Bars)
        {
            limits.Expand(bar.AxisLimits);
        }

        return limits.AxisLimits;
    }

    /// <summary>Renders the bars and labels that are not placed on top.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        foreach (Bar bar in Bars)
        {
            bar.RenderBody(rp, Axes);
            if (!bar.LabelOnTop)
            {
                ValueLabelStyle.Text = bar.Label;
                bar.RenderText(rp, Axes, ValueLabelStyle);
            }
        }
    }

    /// <summary>Renders labels that are placed on top of the bars.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void RenderLast(RenderPack rp)
    {
        foreach (Bar bar in Bars)
        {
            if (bar.LabelOnTop)
            {
                ValueLabelStyle.Text = bar.Label;
                bar.RenderText(rp, Axes, ValueLabelStyle);
            }
        }
    }
}
