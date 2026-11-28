// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders the elapsed rendering time and approximate frame rate.</summary>
public class Benchmark : LabelStyleProperties, IPlottable
{
    /// <summary>The number of milliseconds in one second.</summary>
    private const double MillisecondsPerSecond = 1e3;

    /// <summary>The padding around the benchmark label.</summary>
    private const int PixelPadding = 3;

    /// <summary>The horizontal offset of the benchmark label.</summary>
    private const int BenchmarkOffsetX = 10;

    /// <summary>The vertical offset of the benchmark label.</summary>
    private const int BenchmarkOffsetY = -13;

    /// <summary>Gets or sets whether the benchmark text is visible.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Gets or sets the axes used to render this benchmark.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this benchmark.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the style used to render the benchmark text.</summary>
    public override LabelStyle LabelStyle { get; set; } = new()
    {
        FontName = Fonts.Monospace,
        Alignment = Alignment.LowerLeft,
        BackgroundColor = Colors.Yellow,
        PixelPadding = new(PixelPadding, PixelPadding, PixelPadding, PixelPadding),
        BorderWidth = 1,
        BorderColor = Colors.Black,
    };

    /// <summary>Gets the axis limits occupied by this benchmark.</summary>
    /// <returns>No axis limits.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders elapsed-time and frame-rate information.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        LabelStyle.Text = $"Rendered in {rp.Elapsed.TotalMilliseconds:0.000} ms ({MillisecondsPerSecond / rp.Elapsed.TotalMilliseconds:N0} FPS)";

        LabelStyle.Render(rp.Canvas, rp.DataRect.BottomLeft.WithOffset(BenchmarkOffsetX, BenchmarkOffsetY), rp.Paint);
    }
}
