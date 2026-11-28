// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering;

/// <summary>Coordinates the render actions and events for a plot.</summary>
/// <param name="plot">The plot rendered by this manager.</param>
public class RenderManager(Plot plot)
{
    /// <summary>Gets the ordered actions used to render a plot; callers may add custom actions.</summary>
    public List<IRenderAction> RenderActions { get; } = [
        new RenderActions.PreRenderLock(),
        new RenderActions.ClearCanvas(),
        new RenderActions.ReplaceNullAxesWithDefaults(),
        new RenderActions.AutoScaleUnsetAxes(),
        new RenderActions.ContinuouslyAutoscale(),
        new RenderActions.ExecutePlottableAxisManagers(),
        new RenderActions.ApplyAxisRulesBeforeLayout(),
        new RenderActions.CalculateLayout(),
        new RenderActions.RenderFigureBackground(),
        new RenderActions.ApplyAxisRulesAfterLayout(),
        new RenderActions.RegenerateTicks(),
        new RenderActions.RenderStartingEvent(),
        new RenderActions.RenderDataBackground(),
        new RenderActions.StoreGLState(),
        new RenderActions.RenderGridsBelowPlottables(),
        new RenderActions.RestoreGLState(),
        new RenderActions.RenderPlottables(),
        new RenderActions.RenderGridsAbovePlottables(),
        new RenderActions.RenderLegends(),
        new RenderActions.RenderPanels(),
        new RenderActions.RenderBorders(),
        new RenderActions.RenderZoomRectangle(),
        new RenderActions.SyncGLPlottables(),
        new RenderActions.RenderPlottablesLast(),
        new RenderActions.RenderBenchmark(),
    ];

    /// <summary>Gets information about the previous render.</summary>
    public RenderDetails LastRender { get; private set; }

    /// <summary>Gets or sets the callback invoked before any render action begins.</summary>
    public EventHandler PreRenderLock { get; set; } = static (_, _) => { };

    /// <summary>Gets or sets the callback invoked just before each render.</summary>
    public EventHandler<RenderPack> RenderStarting { get; set; } = static (_, _) => { };

    /// <summary>Gets or sets the callback invoked after each render.</summary>
    public EventHandler<RenderDetails> RenderFinished { get; set; } = static (_, _) => { };

    /// <summary>Gets or sets the callback invoked when the figure size changes.</summary>
    public EventHandler<RenderDetails> SizeChanged { get; set; } = static (_, _) => { };

    /// <summary>Gets or sets the callback invoked when axis limits change.</summary>
    public EventHandler<RenderDetails> AxisLimitsChanged { get; set; } = static (_, _) => { };

    /// <summary>Gets or sets whether to suppress the next axis-limits-changed event.</summary>
    public bool DisableAxisLimitsChangedEventOnNextRender { get; set; }

    /// <summary>Gets whether this plot is in the process of rendering.</summary>
    public bool IsRendering { get; private set; }

    /// <summary>Gets or sets whether calls to <see cref="Render(SKCanvas, PixelRect)"/> are enabled.</summary>
    public bool EnableRendering { get; set; } = true;

    /// <summary>Gets or sets whether render events are invoked.</summary>
    public bool EnableEvents { get; set; } = true;

    /// <summary>Gets or sets whether to clear the canvas before each render.</summary>
    public bool ClearCanvasBeforeEachRender { get; set; } = true;

    /// <summary>Gets the total number of renders completed.</summary>
    public int RenderCount { get; private set; }

    /// <summary>Gets the plot rendered by this manager.</summary>
    private Plot Plot { get; } = plot;

    /// <summary>Removes all render actions of the specified type.</summary>
    /// <typeparam name="T">The type of render action to remove.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2307", Justification = "The explicit type argument selects the render-action type to remove.")]
    public void Remove<T>()
        where T : IRenderAction => _ = RenderActions.RemoveAll(static x => x is T);

    /// <summary>Renders the plot to the specified canvas and rectangle.</summary>
    /// <param name="canvas">The canvas to render to.</param>
    /// <param name="rect">The pixel rectangle to render within.</param>
    public void Render(SKCanvas canvas, PixelRect rect)
    {
        canvas.Scale(Plot.ScaleFactorF);
        const int maxRenderCount = 5;
        for (int i = 0; i < maxRenderCount; i++)
        {
            RenderOnce(canvas, rect);
            if (!LastRender.AxisLimitsChanged)
            {
                return;
            }
        }
    }

    /// <summary>Clears the details recorded for the previous render.</summary>
    public void ForgetLastRender() => LastRender = new();

    /// <summary>Performs one render pass and records its timing and results.</summary>
    /// <param name="canvas">The canvas to render to.</param>
    /// <param name="rect">The pixel rectangle to render within.</param>
    private void RenderOnce(SKCanvas canvas, PixelRect rect)
    {
        if (!EnableRendering)
        {
            return;
        }

        IsRendering = true;

        // If the figure background is semi-transparent, clear early in the render pipeline
        // to compensate for WinUI issues.
        if (ClearCanvasBeforeEachRender && Plot.FigureBackground.Color.A < byte.MaxValue)
        {
            canvas.Clear();
        }

        List<(string, TimeSpan)> actionTimes = [];

        using RenderPack rp = new(Plot, rect, canvas);

        Stopwatch sw = new();
        foreach (IRenderAction action in RenderActions)
        {
            sw.Restart();
            rp.CanvasState.Save();
            action.Render(rp);
            rp.CanvasState.RestoreAll();
            actionTimes.Add((action.ToString() ?? string.Empty, sw.Elapsed));
        }

        LastRender = new(rp, [.. actionTimes], LastRender);
        RenderCount++;
        IsRendering = false;

        if (EnableEvents)
        {
            RenderFinished.Invoke(Plot, LastRender);

            if (LastRender.SizeChanged)
            {
                SizeChanged.Invoke(Plot, LastRender);
            }

            if (!DisableAxisLimitsChangedEventOnNextRender && LastRender.AxisLimitsChanged)
            {
                AxisLimitsChanged.Invoke(Plot, LastRender);
                Plot.Axes.ApplyLinkedAxisRules();
            }
        }

        DisableAxisLimitsChangedEventOnNextRender = false;

        // TODO: event for when layout changes
    }
}
