// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Plottables;

namespace CrissCross.Plot.DataViews;

    /// <summary>Renders streamed data with a wiping transition.</summary>
public class Wipe : IDataStreamerView
{
    /// <summary>The maximum number of segments returned by this view.</summary>
    private const int MaximumSegmentCount = 2;

    /// <summary>Whether the wipe proceeds from right to left.</summary>
    private readonly bool _wipeRight;

    /// <summary>Initializes a new instance of the <see cref="Wipe"/> class.</summary>
    /// <param name="streamer">The data streamer to render.</param>
    /// <param name="wipeRight">Whether the wipe proceeds from the right.</param>
    public Wipe(DataStreamer streamer, bool wipeRight)
    {
        Streamer = streamer;
        _wipeRight = wipeRight;
    }

    /// <summary>Gets the data streamer rendered by this view.</summary>
    public DataStreamer Streamer { get; }

    /// <summary>Gets or sets the fraction of the display left blank during the wipe.</summary>
    public double BlankFraction { get; set; }

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
        int newestCount = Streamer.Data.NextIndex;
        int oldestCount = Streamer.Data.Data.Length - newestCount;
        double maximumHorizontalCoordinate = (Streamer.Data.Data.Length * Streamer.Data.SamplePeriod) + Streamer.Data.OffsetX;
        Pixel[] newest = CreateSegment(0, newestCount, maximumHorizontalCoordinate);
        Pixel[] oldest = CreateSegment(newestCount, oldestCount, maximumHorizontalCoordinate);

        ApplyBlankFraction(ref newest, ref oldest);
        if (!_wipeRight)
        {
            System.Array.Reverse(newest);
            System.Array.Reverse(oldest);
        }

        return OrderSegments(newest, oldest);
    }

    /// <summary>Adds a segment when it contains enough points to draw.</summary>
    /// <param name="segments">The ordered segment list.</param>
    /// <param name="pixels">The candidate segment.</param>
    private static void AddVisibleSegment(List<Pixel[]> segments, Pixel[] pixels)
    {
        if (pixels.Length <= 1)
        {
            return;
        }

        segments.Add(pixels);
    }

    /// <summary>Returns the values after the specified number of pixels.</summary>
    /// <param name="pixels">The source pixels.</param>
    /// <param name="count">The number of leading pixels to skip.</param>
    /// <returns>A new array containing the remaining pixels.</returns>
    private static Pixel[] SkipPixels(Pixel[] pixels, int count)
    {
        if (count >= pixels.Length)
        {
            return [];
        }

        Pixel[] result = new Pixel[pixels.Length - count];
        Array.Copy(pixels, count, result, 0, result.Length);
        return result;
    }

    /// <summary>Creates a pixel segment from a contiguous portion of the circular buffer.</summary>
    /// <param name="firstDataIndex">The first data index in the segment.</param>
    /// <param name="count">The number of points in the segment.</param>
    /// <param name="maximumHorizontalCoordinate">The maximum horizontal coordinate in the buffer.</param>
    /// <returns>The pixel segment.</returns>
    private Pixel[] CreateSegment(int firstDataIndex, int count, double maximumHorizontalCoordinate)
    {
        Pixel[] pixels = new Pixel[count];
        for (int i = 0; i < count; i++)
        {
            int dataIndex = firstDataIndex + i;
            double horizontalCoordinate = (dataIndex * Streamer.Data.SamplePeriod) + Streamer.Data.OffsetX;
            double displayedCoordinate = _wipeRight ? horizontalCoordinate : maximumHorizontalCoordinate - horizontalCoordinate;
            pixels[i] = new(
                Streamer.Axes.GetPixelX(displayedCoordinate),
                Streamer.Axes.GetPixelY(Streamer.Data.Data[dataIndex] + Streamer.Data.OffsetY));
        }

        return pixels;
    }

    /// <summary>Removes the leading portion hidden by the blank region.</summary>
    /// <param name="newest">The newer pixel segment.</param>
    /// <param name="oldest">The older pixel segment.</param>
    private void ApplyBlankFraction(ref Pixel[] newest, ref Pixel[] oldest)
    {
        if (BlankFraction <= 0)
        {
            return;
        }

        int blankCount = (int)(BlankFraction * Streamer.Data.Length);
        if (blankCount <= oldest.Length)
        {
            oldest = SkipPixels(oldest, blankCount);
            return;
        }

        newest = SkipPixels(newest, blankCount - oldest.Length);
        oldest = [];
    }

    /// <summary>Orders and filters pixel segments for the configured wipe direction.</summary>
    /// <param name="newest">The newer pixel segment.</param>
    /// <param name="oldest">The older pixel segment.</param>
    /// <returns>The visible segments in drawing order.</returns>
    private List<Pixel[]> OrderSegments(Pixel[] newest, Pixel[] oldest)
    {
        List<Pixel[]> segments = [with(capacity: MaximumSegmentCount)];
        if (_wipeRight)
        {
            AddVisibleSegment(segments, oldest);
            AddVisibleSegment(segments, newest);
        }
        else
        {
            AddVisibleSegment(segments, newest);
            AddVisibleSegment(segments, oldest);
        }

        return segments;
    }
}
