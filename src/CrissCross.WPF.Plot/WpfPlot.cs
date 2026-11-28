// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Displays an interactive plot using software rendering.</summary>
[System.ComponentModel.ToolboxItem(true)]
[System.ComponentModel.DesignTimeVisible(true)]
[TemplatePart(Name = PartSKElement, Type = typeof(SKElement))]
public class WpfPlot : WpfPlotBase
{
    /// <summary>The name of the rendering element in the control template.</summary>
    private const string PartSKElement = "PART_SKElement";

    /// <summary>The current template's rendering element.</summary>
    private SKElement? _renderElement;

    /// <summary>Initializes static metadata for the control.</summary>
    static WpfPlot() => DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfPlot), new FrameworkPropertyMetadata(typeof(WpfPlot)));

    /// <inheritdoc/>
    public override GRContext? GRContext => null;

    /// <inheritdoc/>
    protected override FrameworkElement PlotFrameworkElement => _renderElement ?? throw new InvalidOperationException("The plot template has not been applied.");

    /// <inheritdoc/>
    public override void OnApplyTemplate()
    {
        if (_renderElement is not null)
        {
            _renderElement.PaintSurface -= OnPaintSurface;
            _renderElement.MouseDown -= SKElement_MouseDown;
            _renderElement.MouseUp -= SKElement_MouseUp;
            _renderElement.MouseMove -= SKElement_MouseMove;
            _renderElement.MouseWheel -= SKElement_MouseWheel;
            _renderElement.KeyDown -= SKElement_KeyDown;
            _renderElement.KeyUp -= SKElement_KeyUp;
            _renderElement.LostMouseCapture -= SKElement_LostFocus;
            _renderElement.LostKeyboardFocus -= SKElement_LostFocus;
        }

        base.OnApplyTemplate();
        _renderElement = GetTemplateChild(PartSKElement) as SKElement;
        if (_renderElement is null)
        {
            return;
        }

        _renderElement.PaintSurface += OnPaintSurface;
        _renderElement.MouseDown += SKElement_MouseDown;
        _renderElement.MouseUp += SKElement_MouseUp;
        _renderElement.MouseMove += SKElement_MouseMove;
        _renderElement.MouseWheel += SKElement_MouseWheel;
        _renderElement.KeyDown += SKElement_KeyDown;
        _renderElement.KeyUp += SKElement_KeyUp;
        _renderElement.LostMouseCapture += SKElement_LostFocus;
        _renderElement.LostKeyboardFocus += SKElement_LostFocus;
    }

    /// <inheritdoc/>
    public override void Refresh()
    {
        if (!CheckAccess())
        {
            _ = Dispatcher.BeginInvoke(Refresh);
            return;
        }

        _renderElement?.InvalidateVisual();
    }

    /// <summary>Renders the current multiplot onto the software surface.</summary>
    /// <param name="sender">The rendering element.</param>
    /// <param name="e">The paint event arguments.</param>
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var bounds = e.Surface.Canvas.LocalClipBounds;
        Multiplot.Render(e.Surface.Canvas, new(0, bounds.Width, bounds.Height, 0));
    }
}
