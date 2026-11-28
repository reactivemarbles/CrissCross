// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using CrissCross.Plot;
using SkiaSharp;
using Controls = Avalonia.Controls;
using PlotCursor = CrissCross.Plot.Cursor;
using PlotModel = CrissCross.Plot.Plot;
using PlotMultiplot = CrissCross.Plot.Multiplot;
using PlotPixelRect = CrissCross.Plot.PixelRect;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Provides an interactive Avalonia plot control.</summary>
public class AvaPlot : Controls.Control, IPlotControl
{
    /// <summary>The lazily initialized input processor.</summary>
    private Interactivity.UserInputProcessor? _userInputProcessor;

    /// <summary>Indicates whether the menu has been initialized or explicitly assigned.</summary>
    private bool _menuInitialized;

    /// <summary>Initializes a new instance of the <see cref="AvaPlot"/> class.</summary>
    public AvaPlot()
    {
        Plot = new() { PlotControl = this };
        Multiplot = new PlotMultiplot(Plot);
        ClipToBounds = true;
        DisplayScale = DetectDisplayScale();
        Focusable = true;
        Refresh();
    }

    /// <inheritdoc/>
    public PlotModel Plot { get; internal set; }

    /// <inheritdoc/>
    public IMultiplot Multiplot { get; set; }

    /// <inheritdoc/>
    [SuppressMessage("Style", "SST2200:Use the field keyword", Justification = "The initialization flag distinguishes an explicitly disabled menu from an uninitialized menu.")]
    public IPlotMenu? Menu
    {
        get
        {
            if (!_menuInitialized)
            {
                field = new AvaPlotMenu(this);
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
    public GRContext? GRContext => null;

    /// <summary>Gets or sets whether mouse-wheel events are handled instead of bubbling to parent controls.</summary>
    public bool HandleMouseWheelEvent { get; set; } = true;

    /// <inheritdoc/>
    public float DisplayScale { get; set; } = 1;

    /// <inheritdoc/>
    public void Reset() => Reset(new() { PlotControl = this });

    /// <inheritdoc/>
    public void Reset(PlotModel plot)
    {
        ArgumentNullException.ThrowIfNull(plot);
        if (ReferenceEquals(Plot, plot))
        {
            return;
        }

        PlotModel oldPlot = Plot;
        Plot = plot;
        oldPlot.Dispose();
        Plot.PlotControl = this;
        Multiplot.Reset(plot);
    }

    /// <inheritdoc/>
    public void Refresh() => _ = Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel position) => Menu?.ShowContextMenu(position);

    /// <inheritdoc/>
    public float DetectDisplayScale() => 1.0F;

    /// <inheritdoc/>
    public void SetCursor(PlotCursor cursor) => Cursor = cursor.GetCursor();

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        Rect controlBounds = new(Bounds.Size);
        CustomDrawOp customDrawOp = new(controlBounds, Multiplot);
        context.Custom(customDrawOp);
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        Pixel pixel = e.ToPixel(this);
        PointerUpdateKind kind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        UserInputProcessor.ProcessMouseDown(pixel, kind, e.KeyModifiers);
        e.Pointer.Capture(this);
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        Pixel pixel = e.ToPixel(this);
        PointerUpdateKind kind = e.GetCurrentPoint(this).Properties.PointerUpdateKind;
        UserInputProcessor.ProcessMouseUp(pixel, kind, e.KeyModifiers);
        e.Pointer.Capture(null);
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        Pixel pixel = e.ToPixel(this);
        UserInputProcessor.ProcessMouseMove(pixel, e.KeyModifiers);
    }

    /// <inheritdoc/>
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        Pixel pixel = e.ToPixel(this);
        float delta = (float)e.Delta.Y;
        if (delta != 0)
        {
            UserInputProcessor.ProcessMouseWheel(pixel, delta, e.KeyModifiers);
        }

        e.Handled = HandleMouseWheelEvent;
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e) => UserInputProcessor.ProcessKeyDown(e);

    /// <inheritdoc/>
    protected override void OnKeyUp(KeyEventArgs e) => UserInputProcessor.ProcessKeyUp(e);

    /// <inheritdoc/>
    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        UserInputProcessor.ProcessLostFocus();
    }

    /// <summary>Draws the plot using Avalonia's Skia drawing context.</summary>
    private sealed class CustomDrawOp : ICustomDrawOperation
    {
        /// <summary>The multiplot rendered by this operation.</summary>
        private readonly IMultiplot _multiplot;

        /// <summary>Initializes a new instance of the <see cref="CustomDrawOp"/> class.</summary>
        /// <param name="bounds">The bounds of the control.</param>
        /// <param name="multiplot">The plot renderer.</param>
        public CustomDrawOp(in Rect bounds, IMultiplot multiplot)
        {
            ArgumentNullException.ThrowIfNull(multiplot);
            Bounds = bounds;
            _multiplot = multiplot;
        }

        /// <inheritdoc/>
        public Rect Bounds { get; }

        /// <inheritdoc/>
        public bool HitTest(Point p) => true;

        /// <inheritdoc/>
        public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other);

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void Render(ImmediateDrawingContext context)
        {
            ISkiaSharpApiLeaseFeature? leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature is null)
            {
                return;
            }

            using ISkiaSharpApiLease lease = leaseFeature.Lease();
            PlotPixelRect plotRect = new(0, (float)Bounds.Width, (float)Bounds.Height, 0);
            using SKAutoCanvasRestore canvasRestore = new(lease.SkCanvas, false);
            _ = lease.SkCanvas.SaveLayer();
            _multiplot.Render(lease.SkCanvas, plotRect);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
