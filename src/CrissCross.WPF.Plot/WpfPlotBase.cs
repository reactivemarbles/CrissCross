// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SkiaSharp;
using PlotCursor = CrissCross.Plot.Cursor;
using PlotModel = CrissCross.Plot.Plot;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides shared interactive behavior for WPF plot controls.</summary>
public abstract class WpfPlotBase : System.Windows.Controls.Control, IPlotControl
{
    /// <summary>The lazily initialized input processor.</summary>
    private Interactivity.UserInputProcessor? _inputProcessor;

    /// <summary>Indicates whether the menu has been initialized or explicitly assigned.</summary>
    private bool _menuInitialized;

    /// <summary>Initializes static metadata for the control.</summary>
    static WpfPlotBase() => DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfPlotBase), new FrameworkPropertyMetadata(typeof(WpfPlotBase)));

    /// <summary>Initializes a new instance of the <see cref="WpfPlotBase"/> class.</summary>
    [SuppressMessage("Design", "SST1428:Use protected constructors in abstract classes", Justification = "Preserve the existing public constructor's accessibility.")]
    public WpfPlotBase()
    {
        Plot = new() { PlotControl = this };
        Multiplot = new Multiplot(Plot);
        DisplayScale = DetectDisplayScale();
        Focusable = true;
    }

    /// <inheritdoc/>
    public abstract GRContext? GRContext { get; }

    /// <inheritdoc/>
    public PlotModel Plot { get; internal set; }

    /// <inheritdoc/>
    public IMultiplot Multiplot { get; set; }

    /// <inheritdoc/>
    public float DisplayScale { get; set; }

    /// <inheritdoc/>
    [SuppressMessage("Style", "SST2200:Use the field keyword", Justification = "The initialization flag distinguishes an explicitly disabled menu from an uninitialized menu.")]
    public IPlotMenu? Menu
    {
        get
        {
            if (!_menuInitialized)
            {
                field = new WpfPlotMenu(this);
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
    public Interactivity.UserInputProcessor UserInputProcessor => _inputProcessor ??= new(this);

    /// <summary>Gets the element that owns the drawing surface.</summary>
    protected abstract FrameworkElement PlotFrameworkElement { get; }

    /// <inheritdoc/>
    public abstract void Refresh();

    /// <inheritdoc/>
    public void Reset() => Reset(new());

    /// <inheritdoc/>
    [SuppressMessage("Naming", "SST1318:Match base member parameter names", Justification = "Preserve the existing parameter name for named arguments.")]
    public void Reset(PlotModel newPlot)
    {
        ThrowHelper.ThrowIfNull(newPlot, nameof(newPlot));

        if (ReferenceEquals(Plot, newPlot))
        {
            return;
        }

        PlotModel previous = Plot;
        Plot = newPlot;
        previous.Dispose();
        Plot.PlotControl = this;
        Multiplot.Reset(newPlot);
    }

    /// <inheritdoc/>
    public void ShowContextMenu(Pixel position) => Menu?.ShowContextMenu(position);

    /// <inheritdoc/>
    public float DetectDisplayScale() => (float)VisualTreeHelper.GetDpi(this).DpiScaleX;

    /// <summary>Gets the mouse position relative to the plot drawing surface.</summary>
    /// <param name="e">The mouse event arguments.</param>
    /// <returns>The mouse position in plot pixels.</returns>
    public Pixel GetPlotPixelPosition(MouseEventArgs e) => e.ToPixel(PlotFrameworkElement);

    /// <summary>Gets the current mouse position relative to the plot drawing surface.</summary>
    /// <returns>The mouse position in plot pixels.</returns>
    public Pixel GetCurrentPlotPixelPosition() => PlotFrameworkElement.ToPixel(Mouse.GetPosition(PlotFrameworkElement));

    /// <inheritdoc/>
    public void SetCursor(PlotCursor cursor) => Cursor = cursor.GetCursor();

    /// <summary>Handles a mouse button press on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The mouse event arguments.</param>
    internal void SKElement_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        _ = Keyboard.Focus(this);
        UserInputProcessor.ProcessMouseDown(PlotFrameworkElement, e);
        _ = (sender as UIElement)?.CaptureMouse();
    }

    /// <summary>Handles a mouse button release on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The mouse event arguments.</param>
    internal void SKElement_MouseUp(object? sender, MouseButtonEventArgs e)
    {
        UserInputProcessor.ProcessMouseUp(PlotFrameworkElement, e);
        (sender as UIElement)?.ReleaseMouseCapture();
    }

    /// <summary>Handles mouse movement on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The mouse event arguments.</param>
    internal void SKElement_MouseMove(object? sender, MouseEventArgs e) => UserInputProcessor.ProcessMouseMove(PlotFrameworkElement, e);

    /// <summary>Handles mouse wheel input on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The mouse event arguments.</param>
    internal void SKElement_MouseWheel(object? sender, MouseWheelEventArgs e) => UserInputProcessor.ProcessMouseWheel(PlotFrameworkElement, e);

    /// <summary>Handles a key press on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The key event arguments.</param>
    internal void SKElement_KeyDown(object? sender, KeyEventArgs e) => UserInputProcessor.ProcessKeyDown(e);

    /// <summary>Handles a key release on the rendering element.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The key event arguments.</param>
    internal void SKElement_KeyUp(object? sender, KeyEventArgs e) => UserInputProcessor.ProcessKeyUp(e);

    /// <summary>Clears the input state when focus is lost.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The routed event arguments.</param>
    internal void SKElement_LostFocus(object sender, RoutedEventArgs e) => UserInputProcessor.ProcessLostFocus();

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        UserInputProcessor.ProcessKeyDown(e);
        base.OnKeyDown(e);
    }

    /// <inheritdoc/>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        UserInputProcessor.ProcessKeyUp(e);
        base.OnKeyUp(e);
    }
}
