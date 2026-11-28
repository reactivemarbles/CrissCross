// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using PlotMultiplot = CrissCross.Plot.Multiplot;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Provides an interactive native MAUI plot control.</summary>
public class MauiPlot : SKCanvasView, IPlotControl, IDisposable
{
    /// <summary>The number of taps required to autoscale the plot.</summary>
    private const int DoubleTapCount = 2;

    /// <summary>The lazily initialized input processor.</summary>
    private Interactivity.UserInputProcessor? _userInputProcessor;

    /// <summary>Indicates whether the menu was initialized or explicitly assigned.</summary>
    private bool _menuInitialized;

    /// <summary>Indicates whether mobile gesture recognizers have been added.</summary>
    private bool _gestureRecognizersAdded;

    /// <summary>Indicates whether the control has been disposed.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="MauiPlot"/> class.</summary>
    public MauiPlot()
    {
        Plot = new() { PlotControl = this };
        Multiplot = new PlotMultiplot(Plot);
        DisplayScale = DetectDisplayScale();
        IgnorePixelScaling = true;

        if (DeviceInfo.Idiom != DeviceIdiom.Desktop)
        {
            return;
        }

        EnableTouchEvents = true;
        Touch += OnTouch;
    }

    /// <inheritdoc/>
    public PlotModel Plot { get; internal set; }

    /// <inheritdoc/>
    public IMultiplot Multiplot { get; set; }

    /// <inheritdoc/>
    public SkiaSharp.GRContext? GRContext => null;

    /// <inheritdoc/>
    [SuppressMessage("Style", "SST2200:Use the field keyword", Justification = "The initialization flag distinguishes an explicitly disabled menu from an uninitialized menu.")]
    public IPlotMenu? Menu
    {
        get
        {
            if (!_menuInitialized)
            {
                field = new MauiPlotMenu(this);
                _menuInitialized = true;
            }

            return field;
        }

        set
        {
            field = value;
            _menuInitialized = true;
        }
    }

    /// <inheritdoc/>
    public Interactivity.UserInputProcessor UserInputProcessor => _userInputProcessor ??= new(this);

    /// <inheritdoc/>
    public float DisplayScale { get; set; } = 1;

    /// <summary>Gets or sets the most recent pointer position in plot pixels.</summary>
    internal Pixel LastPixel { get; set; }

    /// <summary>Gets or sets the most recent scale-gesture position in plot pixels.</summary>
    internal Pixel LastScalePixel { get; set; }

    /// <inheritdoc/>
    public void Reset() => Reset(new());

    /// <inheritdoc/>
    public void Reset(PlotModel plot)
    {
        ArgumentNullException.ThrowIfNull(plot);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (ReferenceEquals(Plot, plot))
        {
            return;
        }

        PlotModel previous = Plot;
        Plot = plot;
        previous.Dispose();
        Plot.PlotControl = this;
        Multiplot.Reset(plot);
    }

    /// <inheritdoc/>
    public void Refresh() => InvalidateSurface();

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel position) => Menu?.ShowContextMenu(position);

    /// <inheritdoc/>
    public float DetectDisplayScale()
    {
        if (Parent is VisualElement parent)
        {
            Plot.ScaleFactor = parent.Scale;
            DisplayScale = (float)parent.Scale;
        }

        return DisplayScale;
    }

    /// <inheritdoc/>
    public void SetCursor(Cursor cursor) => _ = cursor;

    /// <summary>Disposes the plot owned by this control.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the resources owned by this control.</summary>
    /// <param name="disposing"><see langword="true"/> to release managed and unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Plot.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc/>
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        DisplayScale = DetectDisplayScale();

        if (_gestureRecognizersAdded || DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            return;
        }

        _gestureRecognizersAdded = true;
        PanGestureRecognizer panGestureRecognizer = new();
        PinchGestureRecognizer pinchGestureRecognizer = new();
        TapGestureRecognizer tapGestureRecognizer = new() { NumberOfTapsRequired = DoubleTapCount };
        panGestureRecognizer.PanUpdated += OnPanUpdated;
        pinchGestureRecognizer.PinchUpdated += OnPinchUpdated;
        tapGestureRecognizer.Tapped += OnDoubleTapped;

        GestureRecognizers.Add(pinchGestureRecognizer);
        GestureRecognizers.Add(panGestureRecognizer);
        GestureRecognizers.Add(tapGestureRecognizer);
    }

    /// <inheritdoc/>
    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        e.Surface.Canvas.Clear();
        Multiplot.Render(e.Surface);
    }

    /// <summary>Processes native touch events on desktop MAUI platforms.</summary>
    /// <param name="sender">The control that raised the event.</param>
    /// <param name="e">The touch event data.</param>
    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        if (e.ActionType == SKTouchAction.Pressed)
        {
            UserInputProcessor.ProcessMouseDown(e);
        }
        else if (e.ActionType == SKTouchAction.Moved)
        {
            UserInputProcessor.ProcessMouseMove(e);
        }
        else if (e.ActionType == SKTouchAction.Released)
        {
            UserInputProcessor.ProcessMouseUp(e);
        }
        else if (e.ActionType == SKTouchAction.WheelChanged)
        {
            UserInputProcessor.ProcessWheelChanged(e);
        }
    }

    /// <summary>Processes a pan-gesture update.</summary>
    /// <param name="sender">The gesture recognizer that raised the event.</param>
    /// <param name="e">The pan event data.</param>
    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e) => UserInputProcessor.ProcessPanUpdated(this, e);

    /// <summary>Processes a pinch-gesture update.</summary>
    /// <param name="sender">The gesture recognizer that raised the event.</param>
    /// <param name="e">The pinch event data.</param>
    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e) => UserInputProcessor.ProcessPinchUpdated(this, e, (float)Width, (float)Height);

    /// <summary>Autoscales the plot after a double tap.</summary>
    /// <param name="sender">The gesture recognizer that raised the event.</param>
    /// <param name="e">The tap event data.</param>
    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        Plot.Axes.AutoScale();
        Refresh();
    }
}
