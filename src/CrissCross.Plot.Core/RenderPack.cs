// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Pairs a plot with pixel and canvas information used throughout the render system.</summary>
/// <param name="plot">The plot being rendered.</param>
/// <param name="figureRect">The figure rectangle in pixel units.</param>
/// <param name="canvas">The canvas used to render the plot.</param>
public class RenderPack(Plot plot, PixelRect figureRect, SKCanvas canvas) : IDisposable
{
    /// <summary>Indicates whether this render pack has been disposed.</summary>
    private bool _disposed;

    /// <summary>Gets the canvas used to render the plot.</summary>
    public SKCanvas Canvas { get; } = canvas;

    /// <summary>Gets the state tracker for the canvas.</summary>
    public CanvasState CanvasState { get; } = new(canvas);

    /// <summary>Gets the paint used during rendering.</summary>
    public Paint Paint { get; } = Paint.NewDisposablePaint();

    /// <summary>Gets the figure rectangle in pixel units.</summary>
    public PixelRect FigureRect { get; } = figureRect;

    /// <summary>Gets the scaled figure rectangle.</summary>
    public PixelRect ScaledFigureRect { get; private set; }

    /// <summary>Gets the data rectangle.</summary>
    public PixelRect DataRect { get; private set; }

    /// <summary>Gets the layout calculated for the render.</summary>
    public Layout Layout { get; private set; }

    /// <summary>Gets the plot being rendered.</summary>
    public Plot Plot { get; } = plot;

    /// <summary>Gets the elapsed time since rendering began.</summary>
    public TimeSpan Elapsed => Stopwatch.Elapsed;

    /// <summary>Gets the stopwatch used to measure render duration.</summary>
    private Stopwatch Stopwatch { get; } = Stopwatch.StartNew();

    /// <summary>Disposes the resources owned by this render pack.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Uses the layout engine to measure panels and set the layout and data rectangle for this render pack.</summary>
    internal void CalculateLayout()
    {
        if (DataRect.HasArea)
        {
            throw new InvalidOperationException("CalculateLayout() must only be called once per render");
        }

        ScaledFigureRect = new(
            left: FigureRect.Left / Plot.ScaleFactorF,
            right: FigureRect.Right / Plot.ScaleFactorF,
            bottom: FigureRect.Bottom / Plot.ScaleFactorF,
            top: FigureRect.Top / Plot.ScaleFactorF);

        Layout = Plot.Layout.LayoutEngine.GetLayout(ScaledFigureRect, Plot, Paint);
        DataRect = Layout.DataRect;
    }

    /// <summary>Disposes managed resources when requested and marks this render pack disposed.</summary>
    /// <param name="disposing"><see langword="true"/> to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Paint.Dispose();
        }

        _disposed = true;
    }
}
