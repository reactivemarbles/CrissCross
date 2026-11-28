// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Plottables;

namespace CrissCross.Plot.DataViews;

/// <summary>Renders streamed data as a scrolling plot.</summary>
public class Scroll : IDataStreamerView
{
    /// <summary>The minimum number of samples required to draw a segment.</summary>
    private const int MinimumSampleCount = 2;

    /// <summary>Whether newly added values appear on the right.</summary>
    private readonly bool _newOnRight;

    /// <summary>Initializes a new instance of the <see cref="Scroll"/> class.</summary>
    /// <param name="streamer">The data streamer to render.</param>
    /// <param name="newOnRight">Whether new values appear on the right.</param>
    public Scroll(DataStreamer streamer, bool newOnRight)
    {
        Streamer = streamer;
        _newOnRight = newOnRight;
    }

    /// <summary>Gets the data streamer rendered by this view.</summary>
    public DataStreamer Streamer { get; }

    /// <summary>Renders the visible streamed data.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    public void Render(RenderPack rp)
    {
        foreach (Pixel[] seg in GetSegments(rp))
        {
            Drawing.DrawLines(rp.Canvas, rp.Paint, seg, Streamer.LineStyle);
            Drawing.DrawMarkers(rp.Canvas, rp.Paint, seg, Streamer.MarkerStyle);
        }
    }

    /// <summary>Gets the pixel segments representing the current buffer contents.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <returns>The pixel segments to draw.</returns>
    public IReadOnlyList<Pixel[]> GetSegments(RenderPack rp)
    {
        int capacity = Streamer.Data.Length;
        int sampleCount = Math.Min(Streamer.Data.CountTotal, capacity);
        if (sampleCount < MinimumSampleCount)
        {
            return [];
        }

        double[] values = GetOrderedValues(capacity, sampleCount);
        Pixel[] pixels = CreateSegment(values, capacity, sampleCount);
        return [pixels];
    }

    /// <summary>Copies the circular buffer values into display order.</summary>
    /// <param name="capacity">The buffer capacity.</param>
    /// <param name="sampleCount">The number of available values.</param>
    /// <returns>The values in display order.</returns>
    private double[] GetOrderedValues(int capacity, int sampleCount)
    {
        int nextIndex = Streamer.Data.NextIndex;
        bool wrapped = sampleCount == capacity;
        int wrappedValueCount = wrapped ? capacity - nextIndex : 0;
        double[] values = new double[sampleCount];
        int destinationIndex = 0;

        if (wrapped && wrappedValueCount > 0)
        {
            for (int i = 0; i < wrappedValueCount; i++, destinationIndex++)
            {
                values[destinationIndex] = Streamer.Data.Data[nextIndex + i];
            }
        }

        for (int i = 0; i < nextIndex && destinationIndex < sampleCount; i++, destinationIndex++)
        {
            values[destinationIndex] = Streamer.Data.Data[i];
        }

        return values;
    }

    /// <summary>Converts ordered values to screen coordinates.</summary>
    /// <param name="values">The values in display order.</param>
    /// <param name="capacity">The buffer capacity.</param>
    /// <param name="sampleCount">The number of available values.</param>
    /// <returns>The pixel segment.</returns>
    private Pixel[] CreateSegment(double[] values, int capacity, int sampleCount)
    {
        int horizontalShift = _newOnRight ? capacity - sampleCount : 0;
        Pixel[] pixels = new Pixel[sampleCount];
        for (int screenIndex = 0; screenIndex < sampleCount; screenIndex++)
        {
            int valueIndex = _newOnRight ? screenIndex : sampleCount - 1 - screenIndex;
            int plotIndex = screenIndex + horizontalShift;
            double horizontalCoordinate = (plotIndex * Streamer.Data.SamplePeriod) + Streamer.Data.OffsetX;
            double verticalCoordinate = values[valueIndex] + Streamer.Data.OffsetY;
            pixels[screenIndex] = new(
                Streamer.Axes.GetPixelX(horizontalCoordinate),
                Streamer.Axes.GetPixelY(verticalCoordinate));
        }

        return pixels;
    }
}
