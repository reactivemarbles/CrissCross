// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Provides shared behavior for pie and donut plots.</summary>
public abstract class PieBase : IPlottable, IManagesAxisLimits, IHasLine
{
    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the rotation applied to the slices.</summary>
    public Angle Rotation { get; set; } = Angle.FromDegrees(Constants.DefaultRotationDegrees);

    /// <summary>Gets the legend items for the slices.</summary>
    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            List<LegendItem> items = [with(capacity: Slices.Count)];
            foreach (PieSlice slice in Slices)
            {
                items.Add(new LegendItem { LabelText = slice.LegendText, FillStyle = slice.Fill });
            }

            return items;
        }
    }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the padding around the plot.</summary>
    public double Padding { get; set; } = 0.2;

    /// <summary>Gets or sets the distance of slice labels from the center.</summary>
    public double SliceLabelDistance { get; set; } = 1.2;

    /// <summary>Gets or sets the slices displayed by this plot.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2305", Justification = "Retained settable collection property for source and binary compatibility.")]
    public IList<PieSlice> Slices { get; set; } = [];

    /// <summary>Gets or sets whether axis limits are adjusted to keep circles round.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Adds a square-axis rule to the plot when one is not already configured.</summary>
    /// <param name="plot">The plot whose axis rules are updated.</param>
    public virtual void UpdateAxisLimits(Plot plot)
    {
        foreach (IAxisRule rule in plot.Axes.Rules)
        {
            if (rule is AxisRules.SquareZoomOut)
            {
                return;
            }
        }

        AxisRules.SquareZoomOut squareRule = new(Axes.XAxis, Axes.YAxis);
        plot.Axes.Rules.Add(squareRule);
    }

    /// <summary>Gets the axis limits occupied by this plot.</summary>
    /// <returns>The axis limits for this plot.</returns>
    public abstract AxisLimits GetAxisLimits();

    /// <summary>Renders this plot.</summary>
    /// <param name="rp">The render context.</param>
    public abstract void Render(RenderPack rp);

    /// <summary>Contains constants used by pie plots.</summary>
    internal static class Constants
    {
        /// <summary>The default rotation that places the first slice at the top.</summary>
        internal const double DefaultRotationDegrees = -90;
    }
}
