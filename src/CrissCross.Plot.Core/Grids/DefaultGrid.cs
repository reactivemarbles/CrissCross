// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Grids;

/// <summary>Renders major and minor grid lines for the plot axes.</summary>
/// <param name="xAxis">The horizontal axis.</param>
/// <param name="yAxis">The vertical axis.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter names are retained for existing named-argument callers.")]
public class DefaultGrid(IXAxis xAxis, IYAxis yAxis) : IGrid
{
    /// <summary>Gets or sets whether the grid is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets whether the grid is rendered beneath plottables.</summary>
    public bool IsBeneathPlottables { get; set; } = true;

    /// <summary>Gets or sets the horizontal axis.</summary>
    public IXAxis XAxis { get; set; } = xAxis;

    /// <summary>Gets or sets the vertical axis.</summary>
    public IYAxis YAxis { get; set; } = yAxis;

    /// <summary>Gets or sets the horizontal-axis grid style.</summary>
    public GridStyle XAxisStyle { get; set; } = new();

    /// <summary>Gets or sets the vertical-axis grid style.</summary>
    public GridStyle YAxisStyle { get; set; } = new();

    /// <summary>Sets the major grid-line color for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility alias for existing callers.")]
    public Color LineColor { set => MajorLineColor = value; }

    /// <summary>Sets the major grid-line width for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility alias for existing callers.")]
    public float LineWidth { set => MajorLineWidth = value; }

    /// <summary>Sets the major grid-line pattern for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility alias for existing callers.")]
    public LinePattern LinePattern { set => MajorLinePattern = value; }

    /// <summary>Gets or sets the major grid-line color for both axes.</summary>
    public Color MajorLineColor
    {
        get => XAxisStyle.MajorLineStyle.Color;
        set
        {
            XAxisStyle.MajorLineStyle.Color = value;
            YAxisStyle.MajorLineStyle.Color = value;
        }
    }

    /// <summary>Sets the minor grid-line color for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility property for existing callers.")]
    public Color MinorLineColor
    {
        set
        {
            XAxisStyle.MinorLineStyle.Color = value;
            YAxisStyle.MinorLineStyle.Color = value;
        }
    }

    /// <summary>Sets the major grid-line width for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility property for existing callers.")]
    public float MajorLineWidth
    {
        set
        {
            XAxisStyle.MajorLineStyle.Width = value;
            YAxisStyle.MajorLineStyle.Width = value;
        }
    }

    /// <summary>Sets the minor grid-line width for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility property for existing callers.")]
    public float MinorLineWidth
    {
        set
        {
            XAxisStyle.MinorLineStyle.Width = value;
            YAxisStyle.MinorLineStyle.Width = value;
        }
    }

    /// <summary>Sets the major grid-line pattern for both axes.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "Retained as a write-only compatibility property for existing callers.")]
    public LinePattern MajorLinePattern
    {
        set
        {
            XAxisStyle.MajorLineStyle.Pattern = value;
            YAxisStyle.MajorLineStyle.Pattern = value;
        }
    }

    /// <summary>Renders the visible grid lines.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    public void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        if (XAxisStyle.IsVisible)
        {
            double minimumHorizontalCoordinate = Math.Min(XAxis.Min, XAxis.Max);
            double maximumHorizontalCoordinate = Math.Max(XAxis.Min, XAxis.Max);
            List<Tick> horizontalTicks = [];
            foreach (Tick tick in XAxis.TickGenerator.Ticks)
            {
                if (tick.Position >= minimumHorizontalCoordinate && tick.Position <= maximumHorizontalCoordinate)
                {
                    horizontalTicks.Add(tick);
                }
            }

            XAxisStyle.Render(rp, XAxis, horizontalTicks);
        }

        if (YAxisStyle.IsVisible)
        {
            double minimumVerticalCoordinate = Math.Min(YAxis.Min, YAxis.Max);
            double maximumVerticalCoordinate = Math.Max(YAxis.Min, YAxis.Max);
            List<Tick> verticalTicks = [];
            foreach (Tick tick in YAxis.TickGenerator.Ticks)
            {
                if (tick.Position >= minimumVerticalCoordinate && tick.Position <= maximumVerticalCoordinate)
                {
                    verticalTicks.Add(tick);
                }
            }

            YAxisStyle.Render(rp, YAxis, verticalTicks);
        }
    }
}
