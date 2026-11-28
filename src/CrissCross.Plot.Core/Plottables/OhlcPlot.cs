// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders open-high-low-close data as financial bars.</summary>
/// <param name="data">The OHLC data source.</param>
public class OhlcPlot(IOHLCSource data) : IPlottable
{
    /// <summary>Gets or sets the fractional width of an OHLC symbol relative to its time span.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained as a public field to preserve the existing binary API.")]
    public double SymbolWidth = Constants.DefaultSymbolWidth;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets whether symbols use sequential integer x positions rather than their dates.</summary>
    public bool Sequential { get; set; }

    /// <summary>Gets the line style for rising OHLC values.</summary>
    public LineStyle RisingStyle { get; } = new() { Color = Color.FromHex("#089981"), Width = Constants.DefaultLineWidth };

    /// <summary>Gets the line style for falling OHLC values.</summary>
    public LineStyle FallingStyle { get; } = new() { Color = Color.FromHex("#f23645"), Width = Constants.DefaultLineWidth };

    /// <summary>Gets the legend items for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => [];

    /// <summary>Gets the axis limits occupied by the OHLC data.</summary>
    /// <returns>The axis limits of the data.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (Sequential)
        {
            CoordinateRange verticalRange = data.GetLimitsY();
            CoordinateRange horizontalRange = new(0, data.GetOHLCs().Count - 1);
            return new(horizontalRange, verticalRange);
        }

        var limits = data.GetLimits();
        var ohlcs = data.GetOHLCs();
        if (ohlcs.Count == 0)
        {
            return limits;
        }

        double left = ohlcs[0].DateTime.ToOADate() - (ohlcs[0].TimeSpan.TotalDays / Constants.HalfWidthDivisor);
        double right = ohlcs[ohlcs.Count - 1].DateTime.ToOADate() + (ohlcs[ohlcs.Count - 1].TimeSpan.TotalDays / Constants.HalfWidthDivisor);
        return new(left, right, limits.Bottom, limits.Top);
    }

    /// <summary>Renders the OHLC bars.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        using SKPathBuilder risingBuilder = new();
        using SKPathBuilder fallingBuilder = new();
        var ohlcs = data.GetOHLCs();
        for (int i = 0; i < ohlcs.Count; i++)
        {
            OHLC ohlc = ohlcs[i];
            bool isRising = ohlc.Close >= ohlc.Open;
            SKPathBuilder path = isRising ? risingBuilder : fallingBuilder;
            float top = Axes.GetPixelY(ohlc.High);
            float bottom = Axes.GetPixelY(ohlc.Low);
            float center;
            float left;
            float right;

            if (!Sequential)
            {
                double centerNumber = NumericConversion.ToNumber(ohlc.DateTime);
                center = Axes.GetPixelX(centerNumber);
                double halfWidthNumber = ohlc.TimeSpan.TotalDays / Constants.HalfWidthDivisor * SymbolWidth;
                left = Axes.GetPixelX(centerNumber - halfWidthNumber);
                right = Axes.GetPixelX(centerNumber + halfWidthNumber);
            }
            else
            {
                center = Axes.GetPixelX(i);
                left = Axes.GetPixelX(i - ((float)SymbolWidth / Constants.HalfWidthDivisor));
                right = Axes.GetPixelX(i + ((float)SymbolWidth / Constants.HalfWidthDivisor));
            }

            float open = Axes.GetPixelY(ohlc.Open);
            float close = Axes.GetPixelY(ohlc.Close);
            if (right < rp.DataRect.Left || left > rp.DataRect.Right)
            {
                continue;
            }

            path.MoveTo(center, top);
            path.LineTo(center, bottom);
            path.MoveTo(left, open);
            path.LineTo(center, open);
            path.MoveTo(center, close);
            path.LineTo(right, close);
        }

        using SKPath risingPath = risingBuilder.Detach();
        using SKPath fallingPath = fallingBuilder.Detach();
        Drawing.DrawPath(rp.Canvas, rp.Paint, risingPath, RisingStyle);
        Drawing.DrawPath(rp.Canvas, rp.Paint, fallingPath, FallingStyle);
    }

    /// <summary>Contains internal constants used when rendering OHLC bars.</summary>
    internal static class Constants
    {
        /// <summary>The default fractional width of an OHLC symbol.</summary>
        internal const double DefaultSymbolWidth = 0.8;

        /// <summary>The divisor used to calculate half-widths.</summary>
        internal const double HalfWidthDivisor = 2;

        /// <summary>The default line width of rising and falling bars.</summary>
        internal const float DefaultLineWidth = 2;
    }
}
