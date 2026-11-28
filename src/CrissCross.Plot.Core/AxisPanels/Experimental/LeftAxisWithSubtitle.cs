// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.AxisPanels.Experimental;

/// <summary>
/// A simple custom Y axis which includes a subtitle/sub-label.
/// Note that <see cref="Measure"/> and <see cref="Render"/> are called whenever the plot needs to be rendered
/// so be wary of heavy code that will slow down the rendering if you are using the chart in a user interface
/// where there is zooming, panning, etc.
/// </summary>
public class LeftAxisWithSubtitle : YAxisBase
{
    /// <summary>Rotation used for the vertical subtitle label.</summary>
    private const float SubLabelRotation = -90;

    /// <summary>Gets or sets the tick generator for the axis.</summary>
    public override ITickGenerator TickGenerator { get; set; } = new TickGenerators.NumericAutomatic();

    /// <summary>Gets or sets the subtitle text displayed alongside the axis label.</summary>
    public string SubLabelText { get => SubLabelStyle.Text; set => SubLabelStyle.Text = value; }

    /// <summary>Gets or sets the style of the subtitle label.</summary>
    public LabelStyle SubLabelStyle { get; set; } = new() { Rotation = SubLabelRotation, };

    /// <summary>Gets the edge on which this axis is positioned.</summary>
    public override Edge Edge => Edge.Left;

    /// <summary>Measures the space required to render the axis and its subtitle.</summary>
    /// <param name="paint">The paint used to measure labels.</param>
    /// <returns>The required width in pixels.</returns>
    public override float Measure(Paint paint)
    {
        if (!IsVisible)
        {
            return 0;
        }

        if (!Range.HasBeenSet)
        {
            return SizeWhenNoData;
        }

        float maxTickLabelWidth = 0;
        foreach (Tick tick in TickGenerator.Ticks)
        {
            float tickLabelWidth = TickLabelStyle.Measure(tick.Label, paint).Width;
            maxTickLabelWidth = Math.Max(maxTickLabelWidth, tickLabelWidth);
        }

        // Add the sub-label to the required size of the axis, as well as the main axis label.
        float axisLabelHeight = LabelStyle.Measure(LabelText, paint).LineHeight
                                + SubLabelStyle.Measure(SubLabelText, paint).LineHeight
                                + PaddingBetweenTickAndAxisLabels.Horizontal
                                + PaddingOutsideAxisLabels.Horizontal;

        return maxTickLabelWidth + axisLabelHeight;
    }

    /// <summary>Renders the axis and its subtitle in the space returned by <see cref="Measure"/>.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="size">The axis panel width.</param>
    /// <param name="offset">The distance from the data rectangle.</param>
    public override void Render(RenderPack rp, float size, float offset)
    {
        if (!IsVisible)
        {
            return;
        }

        // Use the properties on this class and the sub-class to determine what to render, and how.
        // You can put whatever you want here! Check out the implementation of the base axis classes for ideas
        // and how to draw shapes, text, etc. 
        PixelRect panelRect = GetPanelRect(rp.DataRect, size, offset, rp.Paint);
        float x = panelRect.Left + PaddingOutsideAxisLabels.Horizontal;
        Pixel labelPoint = new(x, rp.DataRect.VerticalCenter);

        LabelAlignment = Alignment.UpperCenter;
        LabelStyle.Render(rp.Canvas, labelPoint, rp.Paint);

        float labelHeight = LabelStyle.Measure(rp.Paint).LineHeight;
        Pixel subLabelPoint = new(x + labelHeight, rp.DataRect.VerticalCenter);

        SubLabelStyle.Alignment = Alignment.UpperCenter;
        SubLabelStyle.Render(rp.Canvas, subLabelPoint, rp.Paint);

        DrawTicks(rp, TickLabelStyle, panelRect, TickGenerator.Ticks, this, MajorTickStyle, MinorTickStyle);
        DrawFrame(rp, panelRect, Edge, FrameLineStyle);
    }
}
