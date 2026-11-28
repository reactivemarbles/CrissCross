// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using SkiaSharp.Views.WPF;
using TUnit.Core.Executors;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies WPF plot control ownership and template lifecycle.</summary>
[NotInParallel]
[TestExecutor<STAThreadExecutor>]
public sealed class WpfPlotBaseTests
{
    /// <summary>The rendering element's template part name.</summary>
    private const string RenderPartName = "PART_SKElement";

    /// <summary>The available test layout dimension.</summary>
    private const int LayoutSize = 100;

    /// <summary>Verifies resetting to the current plot does not dispose its data.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Reset_CurrentPlot_PreservesPlottables()
    {
        var control = new WpfPlot();
        var plot = control.Plot;
        var signal = plot.Add.Signal([1]);

        control.Reset(plot);

        await Assert.That(control.Plot).IsSameReferenceAs(plot);
        await Assert.That(plot.PlottableList.Count).IsEqualTo(1);
        await Assert.That(plot.PlottableList[0]).IsSameReferenceAs(signal);
        await Assert.That(control.Multiplot.Subplots.GetPlot(0)).IsSameReferenceAs(plot);
        plot.Dispose();
    }

    /// <summary>Verifies reset installs a replacement plot in the control and multiplot.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Reset_ReplacementPlot_UpdatesControlAndMultiplot()
    {
        var control = new WpfPlot();
        var previous = control.Plot;
        _ = previous.Add.Signal([1]);
        using var replacement = new CrissCross.Plot.Plot();

        control.Reset(replacement);

        await Assert.That(control.Plot).IsSameReferenceAs(replacement);
        await Assert.That(replacement.PlotControl).IsSameReferenceAs(control);
        await Assert.That(control.Multiplot.Subplots.GetPlot(0)).IsSameReferenceAs(replacement);
        await Assert.That(previous.PlottableList.Count).IsEqualTo(0);
    }

    /// <summary>Verifies assigning a null context menu disables the default menu.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Menu_ExplicitNull_RemainsDisabled()
    {
        var control = new WpfPlot { Menu = null };
        await Assert.That(control.Menu).IsNull();
        control.Plot.Dispose();
    }

    /// <summary>Verifies the default template provides the software rendering element.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ApplyTemplate_DefaultSoftwareTemplate_ProvidesRenderingElement()
    {
        var control = new WpfPlot();
        control.BeginInit();
        control.EndInit();
        control.Measure(new(LayoutSize, LayoutSize));
        _ = control.ApplyTemplate();
        await Assert.That(control.Template.FindName(RenderPartName, control)).IsTypeOf<SKElement>();
        control.Plot.Dispose();
    }

    /// <summary>Verifies replaced template elements no longer deliver input events.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ApplyTemplate_ReplacesElement_DetachesOldHandlers()
    {
        var control = new WpfPlot { Template = CreateTemplate() };
        _ = control.ApplyTemplate();
        var first = (SKElement)control.Template.FindName(RenderPartName, control);
        var lostFocusCount = 0;
        control.UserInputProcessor.LostFocusAction = _ => lostFocusCount++;

        control.Template = CreateTemplate();
        _ = control.ApplyTemplate();
        var second = (SKElement)control.Template.FindName(RenderPartName, control);
        first.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.LostMouseCaptureEvent });
        await Assert.That(lostFocusCount).IsEqualTo(0);
        second.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.LostMouseCaptureEvent });
        await Assert.That(lostFocusCount).IsEqualTo(1);
        control.Plot.Dispose();
    }

    /// <summary>Creates a template containing a named software rendering element.</summary>
    /// <returns>A control template.</returns>
    private static ControlTemplate CreateTemplate() => new(typeof(WpfPlot)) { VisualTree = new(typeof(SKElement), RenderPartName) };
}
