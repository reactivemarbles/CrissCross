// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Adjusts tick labels to scientific notation and displays their shared multiplier.</summary>
public class TickModifierLabel : IPlottable
{
    /// <summary>The exponent applied to tick values.</summary>
    private int _exponent;

    /// <summary>Initializes a new instance of the <see cref="TickModifierLabel"/> class.</summary>
    /// <param name="axis">The axis whose tick labels are modified.</param>
    public TickModifierLabel(IAxis axis)
    {
        Axis = axis;
        if (axis is AxisPanels.LeftAxis)
        {
            LabelStyle.Alignment = Alignment.LowerLeft;
            GetTextPixel = static (dataRect) => dataRect.TopLeft;
        }
        else if (axis is AxisPanels.BottomAxis)
        {
            LabelStyle.Alignment = Alignment.UpperRight;
            GetTextPixel = static (dataRect) => dataRect.BottomRight.WithOffset(0, Constants.BottomLabelOffset);
        }
        else
        {
            throw new NotSupportedException($"Axis type '{axis.GetType().Name}' is not supported by {nameof(TickModifierLabel)}.");
        }
    }

    /// <summary>Gets or sets whether this tick modifier is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used by this plottable.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this tick modifier.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the style used to render the multiplier label.</summary>
    public LabelStyle LabelStyle { get; } = new();

    /// <summary>Gets the axis whose tick labels are modified.</summary>
    public IAxis Axis { get; }

    /// <summary>Gets or sets the function that positions the multiplier label.</summary>
    public Func<PixelRect, Pixel> GetTextPixel { get; set; }

    /// <summary>Gets or sets the function that formats the exponent label.</summary>
    public Func<int, string> LabelFormatter { get; set; } = static (exponent) => $"1e{exponent}";

    /// <summary>Gets the axis limits occupied by this tick modifier.</summary>
    /// <returns>No axis limits, because this plottable modifies axis labels.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Updates the axis tick labels to use a shared power-of-ten multiplier.</summary>
    public virtual void UpdateTickLabels()
    {
        Tick[] ticks = Axis.TickGenerator.Ticks;
        if (ticks.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        double minimumPosition = ticks[0].Position;
        double maximumPosition = minimumPosition;
        for (int i = 1; i < ticks.Length; i++)
        {
            minimumPosition = Math.Min(minimumPosition, ticks[i].Position);
            maximumPosition = Math.Max(maximumPosition, ticks[i].Position);
        }

        double tickSpan = maximumPosition - minimumPosition;
        _exponent = (int)Math.Log10(tickSpan);
        double multiplier = Math.Pow(Constants.DecimalBase, _exponent);
        for (int i = 0; i < ticks.Length; i++)
        {
            if (!ticks[i].IsMajor)
            {
                continue;
            }

            double position = ticks[i].Position;
            string label = $"{position / multiplier:0.00}";
            ticks[i] = Tick.Major(position, label);
        }
    }

    /// <summary>Renders the formatted multiplier label.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        Pixel pixel = GetTextPixel.Invoke(rp.DataRect);
        string label = LabelFormatter.Invoke(_exponent);
        rp.CanvasState.DisableClipping();
        LabelStyle.Render(rp.Canvas, pixel, rp.Paint, label);
    }

    /// <summary>Contains constants used by tick modifier labels.</summary>
    internal static class Constants
    {
        /// <summary>The pixel offset for a bottom-axis multiplier label.</summary>
        internal const int BottomLabelOffset = 20;

        /// <summary>The base used for scientific notation.</summary>
        internal const double DecimalBase = 10;
    }
}
