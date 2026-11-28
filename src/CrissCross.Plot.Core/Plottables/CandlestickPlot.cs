// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Plots OHLC data as candlestick symbols.</summary>
/// <param name="data">The OHLC data source.</param>
public class CandlestickPlot(IOHLCSource data) : IPlottable
{
    /// <summary>Gets or sets the candle width as a fraction of its time span.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public double SymbolWidth = .8;

    /// <summary>The multiplier used to calculate half-widths.</summary>
    private const double Half = 0.5;

    /// <summary>The default candle line width in pixels.</summary>
    private const float DefaultLineWidth = 2;

    /// <summary>The tolerance for comparing candle positions in pixels.</summary>
    private const float PixelComparisonTolerance = 0.001F;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the OHLC data source.</summary>
    public IOHLCSource Data { get; } = data;

    /// <summary>
    /// Gets or sets whether candle positions use sequential integer indices instead of the OHLC date and time.
    /// When enabled, positions ascend from zero without gaps.
    /// </summary>
    public bool Sequential { get; set; }

    /// <summary>Gets the line style for rising candles.</summary>
    public LineStyle RisingLineStyle { get; } = new() { Color = Color.FromHex("#089981"), Width = DefaultLineWidth, };

    /// <summary>Gets the line style for falling candles.</summary>
    public LineStyle FallingLineStyle { get; } = new() { Color = Color.FromHex("#f23645"), Width = DefaultLineWidth, };

    /// <summary>Gets the fill style for rising candles.</summary>
    public FillStyle RisingFillStyle { get; } = new() { Color = Color.FromHex("#089981"), };

    /// <summary>Gets the fill style for falling candles.</summary>
    public FillStyle FallingFillStyle { get; } = new() { Color = Color.FromHex("#f23645"), };

    /// <summary>Gets or sets the color used for rising candles.</summary>
    public Color RisingColor
    {
        get => RisingLineStyle.Color;
        set
        {
            RisingLineStyle.Color = value;
            RisingFillStyle.Color = value;
        }
    }

    /// <summary>Gets or sets the color used for falling candles.</summary>
    public Color FallingColor
    {
        get => FallingLineStyle.Color;
        set
        {
            FallingLineStyle.Color = value;
            FallingFillStyle.Color = value;
        }
    }

    /// <summary>Gets the legend items for this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => [];

    /// <summary>Gets the axis limits occupied by the candle data.</summary>
    /// <returns>The axis limits containing the candle data.</returns>
    public AxisLimits GetAxisLimits()
    {
        AxisLimits limits = Data.GetLimits(); // TODO: Data.GetSequentialLimits()

        if (Sequential)
        {
            return new(
                left: -Half,
                right: Data.GetOHLCs().Count - 1 + Half,
                bottom: limits.Bottom,
                top: limits.Top);
        }

        var ohlcs = Data.GetOHLCs();
        if (ohlcs.Count == 0)
        {
            return limits;
        }

        double left = ohlcs[0].DateTime.ToOADate() - (ohlcs[0].TimeSpan.TotalDays * Half);
        double right = ohlcs[ohlcs.Count - 1].DateTime.ToOADate() + (ohlcs[ohlcs.Count - 1].TimeSpan.TotalDays * Half);

        return new(left, right, limits.Bottom, limits.Top);
    }

    /// <summary>Gets the price range visible on the x-axis.</summary>
    /// <returns>The visible price range, or no limits when there is no OHLC data.</returns>
    public CoordinateRange GetPriceRangeInView()
    {
        var ohlcs = Data.GetOHLCs();
        if (ohlcs.Count == 0)
        {
            return CoordinateRange.NoLimits;
        }

        int minIndexInView = (int)NumericConversion.Clamp(Axes.XAxis.Min, 0, ohlcs.Count - 1);
        int maxIndexInView = (int)NumericConversion.Clamp(Axes.XAxis.Max, 0, ohlcs.Count - 1);
        return Data.GetPriceRange(minIndexInView, maxIndexInView);
    }

    /// <summary>Gets the OHLC value nearest to the specified x-coordinate.</summary>
    /// <param name="x">The x-coordinate to inspect.</param>
    /// <returns>The matching index and OHLC value, or <see langword="null"/> when the index is outside the data.</returns>
    public (int index, OHLC ohlc)? GetOhlcNearX(double x)
    {
        int ohlcIndex = (int)Math.Round(x);
        return (ohlcIndex >= 0 && ohlcIndex < Data.Count)
            ? (ohlcIndex, Data.GetOHLCs()[ohlcIndex])
            : null;
    }

    /// <summary>Renders the candle data.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        var ohlcs = Data.GetOHLCs();
        for (int i = 0; i < ohlcs.Count; i++)
        {
            RenderCandle(rp, ohlcs[i], i);
        }
    }

    /// <summary>Renders one OHLC candle when it intersects the visible plot area.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="ohlc">The OHLC candle to render.</param>
    /// <param name="index">The sequential index of the candle.</param>
    private void RenderCandle(RenderPack rp, OHLC ohlc, int index)
    {
        bool isRising = ohlc.Close >= ohlc.Open;
        LineStyle lineStyle = isRising ? RisingLineStyle : FallingLineStyle;
        FillStyle fillStyle = isRising ? RisingFillStyle : FallingFillStyle;
        float top = Axes.GetPixelY(ohlc.High);
        float bottom = Axes.GetPixelY(ohlc.Low);

        (float center, float left, float right) = GetCandlePixelBounds(ohlc, index);
        if (right < rp.DataRect.Left || left > rp.DataRect.Right)
        {
            return;
        }

        float openY = Axes.GetPixelY(ohlc.Open);
        float closeY = Axes.GetPixelY(ohlc.Close);
        PixelLine verticalLine = new(center, top, center, bottom);
        Drawing.DrawLine(rp.Canvas, rp.Paint, verticalLine, lineStyle);

        if (right - left <= 1)
        {
            return;
        }

        PixelRangeX horizontalRange = new(left, right);
        PixelRangeY verticalRange = new(Math.Min(openY, closeY), Math.Max(openY, closeY));
        PixelRect rect = new(horizontalRange, verticalRange);
        if (Math.Abs(openY - closeY) > PixelComparisonTolerance)
        {
            fillStyle.Render(rp.Canvas, rect, rp.Paint);
            return;
        }

        lineStyle.Render(rp.Canvas, rect.BottomLine, rp.Paint);
    }

    /// <summary>Calculates the center and horizontal pixel bounds for a candle.</summary>
    /// <param name="ohlc">The OHLC candle to measure.</param>
    /// <param name="index">The sequential index of the candle.</param>
    /// <returns>The center, left edge, and right edge in pixels.</returns>
    private (float Center, float Left, float Right) GetCandlePixelBounds(OHLC ohlc, int index)
    {
        if (!Sequential)
        {
            double centerNumber = NumericConversion.ToNumber(ohlc.DateTime);
            double halfWidthNumber = ohlc.TimeSpan.TotalDays * Half * SymbolWidth;
            return (
                Axes.GetPixelX(centerNumber),
                Axes.GetPixelX(centerNumber - halfWidthNumber),
                Axes.GetPixelX(centerNumber + halfWidthNumber));
        }

        float halfSymbolWidth = (float)SymbolWidth * (float)Half;
        return (
            Axes.GetPixelX(index),
            Axes.GetPixelX(index - halfSymbolWidth),
            Axes.GetPixelX(index + halfSymbolWidth));
    }
}
