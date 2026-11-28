// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System;
using BenchmarkDotNet.Attributes;
using CrissCross.Plot.DataSources;
using SkiaSharp;
using PlotFigure = global::CrissCross.Plot.Plot;

namespace CrissCross.Plot.Core.Benchmarks;

/// <summary>Measures scatter rendering and signal range lookup.</summary>
[MemoryDiagnoser]
public class PlotRenderBenchmark : IDisposable
{
    /// <summary>Width of the raster target.</summary>
    private const int Width = 800;

    /// <summary>Height of the raster target.</summary>
    private const int Height = 600;

    /// <summary>Scale used to create deterministic signal values.</summary>
    private const double SampleAngularScale = 0.01;

    /// <summary>The scatter plot rendered by the benchmark.</summary>
    private PlotFigure _plot = null!;

    /// <summary>The reusable Skia render target.</summary>
    private SKSurface _surface = null!;

    /// <summary>The signal source using a direct range scan.</summary>
    private SignalSourceDouble _signal = null!;

    /// <summary>The signal source using cached range extrema.</summary>
    private FastSignalSourceDouble _fastSignal = null!;

    /// <summary>The positions used by the disabled-marker benchmark.</summary>
    private Pixel[] _markerPixels = null!;

    /// <summary>The paint used by the disabled-marker benchmark.</summary>
    private Paint _markerPaint = null!;

    /// <summary>The built-in None style used by the optimized benchmark.</summary>
    private MarkerStyle _noneMarkerStyle = null!;

    /// <summary>The custom no-op renderer used as the prior-behavior baseline.</summary>
    private MarkerStyle _baselineNoneMarkerStyle = null!;

    /// <summary>Gets or sets the number of source points.</summary>
    [Params(1_000, 100_000)]
    public int PointCount { get; set; }

    /// <summary>Creates deterministic benchmark inputs and render state.</summary>
    [GlobalSetup]
    public void Setup()
    {
        double[] xs = new double[PointCount];
        double[] ys = new double[PointCount];
        for (int i = 0; i < PointCount; i++)
        {
            xs[i] = i;
            ys[i] = Math.Sin(i * SampleAngularScale);
        }

        _plot = new();
        _ = _plot.Add.Scatter(xs, ys);
        _surface = SKSurface.Create(new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul));
        _signal = new(ys, 1);
        _fastSignal = new(ys, 1);
        _markerPixels = new Pixel[PointCount];
        _markerPaint = Paint.NewDisposablePaint();
        _noneMarkerStyle = MarkerStyle.None;
        _baselineNoneMarkerStyle = MarkerStyle.None;
        _baselineNoneMarkerStyle.CustomRenderer = new BaselineNoneMarker();
    }

    /// <summary>Renders the scatter data to a reusable raster target.</summary>
    [Benchmark]
    public void RenderScatter() => _plot.Render(_surface.Canvas, Width, Height);

    /// <summary>Skips marker iteration for the built-in None shape.</summary>
    [Benchmark]
    public void DrawMarkersNoneOptimized() => Drawing.DrawMarkers(_surface.Canvas, _markerPaint, _markerPixels, _noneMarkerStyle);

    /// <summary>Runs a no-op custom renderer once for each marker position.</summary>
    [Benchmark(Baseline = true)]
    public void DrawMarkersNoneBaseline() => Drawing.DrawMarkers(_surface.Canvas, _markerPaint, _markerPixels, _baselineNoneMarkerStyle);

    /// <summary>Scans every sample to find the signal range.</summary>
    /// <returns>The observed minimum and maximum.</returns>
    [Benchmark]
    public SignalRangeY SignalRangeScan() => _signal.GetLimitsY(0, PointCount - 1);

    /// <summary>Retrieves signal extrema from the prebuilt min/max cache.</summary>
    /// <returns>The cached minimum and maximum.</returns>
    [Benchmark]
    public SignalRangeY SignalRangeCached() => _fastSignal.GetLimitsY(0, PointCount - 1);

    /// <summary>Releases per-benchmark render resources.</summary>
    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
        _plot.Dispose();
        _markerPaint.Dispose();
    }

    /// <summary>Releases resources owned by the benchmark instance.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases resources owned by the benchmark instance.</summary>
    /// <param name="disposing">Indicates whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        Cleanup();
    }

    /// <summary>Implements the no-op marker behavior used before the None-shape fast path.</summary>
    private sealed class BaselineNoneMarker : IMarker
    {
        /// <inheritdoc/>
        public void Render(SKCanvas canvas, Paint paint, Pixel center, float size, MarkerStyle markerStyle)
        {
        }
    }
}
