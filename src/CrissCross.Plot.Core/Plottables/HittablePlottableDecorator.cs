// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Threading;
using System.Threading.Tasks;

namespace CrissCross.Plot.Plottables;

/// <summary>Wraps a plottable with a pixel-based hit detection map for mouse interaction.</summary>
public class HittablePlottableDecorator : IPlottable, IDisposable
{
    /// <summary>The monitor protecting the hit bitmap and disposal state.</summary>
    private readonly object _lock = new();

    /// <summary>The bitmap used for pixel-based hit testing.</summary>
    private SKBitmap? _bitmap;

    /// <summary>Indicates whether this decorator has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="HittablePlottableDecorator"/> class.</summary>
    /// <param name="plottable">The plottable to wrap.</param>
    public HittablePlottableDecorator(IPlottable plottable)
    {
        Source = plottable;
    }

    /// <summary>Gets the wrapped plottable.</summary>
    public IPlottable Source { get; }

    /// <summary>Gets or sets whether the wrapped plottable is visible.</summary>
    public bool IsVisible { get => Source.IsVisible; set => Source.IsVisible = value; }

    /// <summary>Gets or sets the axes used by the wrapped plottable.</summary>
    public IAxes Axes { get => Source.Axes; set => Source.Axes = value; }

    /// <summary>Gets the legend items of the wrapped plottable.</summary>
    public IEnumerable<LegendItem> LegendItems => Source.LegendItems;

    /// <summary>Gets the axis limits of the wrapped plottable.</summary>
    /// <returns>The wrapped plottable's axis limits.</returns>
    public AxisLimits GetAxisLimits() => Source.GetAxisLimits();

    /// <summary>Determines whether the rendered plottable is within <paramref name="radius"/> pixels of <paramref name="pixel"/>.</summary>
    /// <param name="pixel">The pixel coordinate to test.</param>
    /// <param name="radius">The radius of the hit-test area in pixels.</param>
    /// <returns><see langword="true"/> when the point intersects rendered content; otherwise, <see langword="false"/>.</returns>
    public bool IsHit(Pixel pixel, float radius)
    {
        Monitor.Enter(_lock);
        try
        {
            if (_bitmap is null)
            {
                return false;
            }

            float radiusHalf = radius / Constants.RadiusDivisor;
            int fromX = Math.Max(0, (int)(pixel.X - radiusHalf));
            int toX = Math.Min(_bitmap.Width, (int)(pixel.X + radiusHalf));
            int fromY = Math.Max(0, (int)(pixel.Y - radiusHalf));
            int toY = Math.Min(_bitmap.Height, (int)(pixel.Y + radiusHalf));
            for (int i = fromX; i < toX; i++)
            {
                for (int j = fromY; j < toY; j++)
                {
                    if (_bitmap.GetPixel(i, j).Alpha != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    /// <summary>Renders the wrapped plottable and updates the hit-test bitmap asynchronously.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        Source.Render(rp);
        _ = Task.Run(() =>
        {
            SKBitmap bitmapBuf = new((int)rp.FigureRect.Width, (int)rp.FigureRect.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using SKCanvas hitCanvas = new(bitmapBuf);
            hitCanvas.Clear(new(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0));
            using RenderPack hitRenderPack = new(rp.Plot, rp.FigureRect, hitCanvas);
            hitRenderPack.CalculateLayout();
            hitRenderPack.CanvasState.Clip(hitRenderPack.DataRect);
            Source.Render(hitRenderPack);
            hitRenderPack.CanvasState.DisableClipping();

            Monitor.Enter(_lock);
            try
            {
                if (_disposed)
                {
                    bitmapBuf.Dispose();
                    return;
                }

                _bitmap?.Dispose();
                _bitmap = bitmapBuf;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        });
    }

    /// <summary>Releases the hit-test bitmap.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases resources owned by this decorator.</summary>
    /// <param name="disposing"><see langword="true"/> to release managed and unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Monitor.Enter(_lock);
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _bitmap?.Dispose();
            _bitmap = null;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    /// <summary>Contains constants used for hit testing.</summary>
    private static class Constants
    {
        /// <summary>The divisor used to calculate half of the hit radius.</summary>
        public const float RadiusDivisor = 2;
    }
}
