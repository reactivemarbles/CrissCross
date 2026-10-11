// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.Plot;
#else
using CrissCross.Maui.Plot;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Guards MAUI settings editors against redundant and stale native work.</summary>
[NotInParallel]
public sealed class MauiResponsivenessRegressionTests
{
    /// <summary>The test series name.</summary>
    private const string SeriesName = "regression";

    /// <summary>The edited item name.</summary>
    private const string EditedName = "edited regression";

    /// <summary>The edited line width.</summary>
    private const double EditedWidth = 7;

    /// <summary>The series settings collection's empty count.</summary>
    private const int EmptySeriesCount = 0;

    /// <summary>The settings body's layout row.</summary>
    private const int BodyRowIndex = 2;

    /// <summary>The settings panel's layout column.</summary>
    private const int SettingsColumnIndex = 1;

    /// <summary>Verifies ordinary editor changes retain unchanged picker choice sources.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Appearance_EditNameAndWidth_KeepsChoiceSourcesStable()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var view = new RightPropertiesView();
            var model = view.ViewModel!;
            var colors = view.ColorsComboBox.ItemsSource;
            var visibilities = view.VisibilityComboBox.ItemsSource;

            model.ItemName = EditedName;
            model.LineWidth = EditedWidth;

            return (
                ColorsRetained: ReferenceEquals(colors, view.ColorsComboBox.ItemsSource),
                VisibilitiesRetained: ReferenceEquals(visibilities, view.VisibilityComboBox.ItemsSource),
                Name: view.ItemNameTextBox.Text,
                Width: view.LineWidthNumberBox.Text);
        });

        await Assert.That(observed.ColorsRetained).IsTrue();
        await Assert.That(observed.VisibilitiesRetained).IsTrue();
        await Assert.That(observed.Name).IsEqualTo(EditedName);
        await Assert.That(observed.Width).IsEqualTo(EditedWidth.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>Verifies a burst of settings notifications rebuilds the native panel once.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task SettingsRebuild_NotificationBurst_CoalescesNativeRebuild()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart();
            chart.OpenPlotSettings();
            PlotTestApp.Drain();
            var settings = GetSettings(chart);
            var expectedChildren = settings.Children.Count;
            var added = 0;
            settings.ChildAdded += (_, _) => added++;
            SeriesChanged(chart, chart, new(NotifyCollectionChangedAction.Reset));
            SeriesChanged(chart, chart, new(NotifyCollectionChangedAction.Reset));
            SeriesChanged(chart, chart, new(NotifyCollectionChangedAction.Reset));
            PlotTestApp.Drain();
            return (Added: added, Expected: expectedChildren);
        });

        await Assert.That(observed.Expected).IsGreaterThan(0);
        await Assert.That(observed.Added).IsEqualTo(observed.Expected);
    }

    /// <summary>Verifies an already queued settings rebuild is ignored after native unload.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task SettingsRebuild_QueuedBeforeUnload_DoesNotRebuildStaleControls()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            chart.OpenPlotSettings();
            using var series = new ReactivePlotSeriesSettings(new(SeriesName, EmptySeriesCount), PlotType.Line, "Red");
            chart.ViewModel.SeriesSettings.Add(series);
            SeriesChanged(chart, chart.ViewModel.SeriesSettings, new(NotifyCollectionChangedAction.Reset));
            var controlCountBeforeUnload = GetSettings(chart).Children.Count;

            ChartUnloaded(chart, chart, EventArgs.Empty);
            PlotTestApp.Drain();

            var controlCountAfterUnload = GetSettings(chart).Children.Count;
            chart.Dispose();
            return (controlCountBeforeUnload, controlCountAfterUnload);
        });

        await Assert.That(observed.controlCountAfterUnload).IsEqualTo(observed.controlCountBeforeUnload);
    }

    /// <summary>Verifies an already queued settings rebuild is ignored after chart disposal.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task SettingsRebuild_QueuedBeforeDispose_DoesNotRebuildStaleControls()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            chart.OpenPlotSettings();
            using var series = new ReactivePlotSeriesSettings(new(SeriesName, EmptySeriesCount), PlotType.Line, "Red");
            chart.ViewModel.SeriesSettings.Add(series);
            SeriesChanged(chart, chart.ViewModel.SeriesSettings, new(NotifyCollectionChangedAction.Reset));
            var controlCountBeforeDispose = GetSettings(chart).Children.Count;

            chart.Dispose();
            PlotTestApp.Drain();

            return (controlCountBeforeDispose, ControlCountAfterDispose: GetSettings(chart).Children.Count);
        });

        await Assert.That(observed.ControlCountAfterDispose).IsEqualTo(observed.controlCountBeforeDispose);
    }

    /// <summary>Gets the native settings layout.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The settings layout.</returns>
    private static VerticalStackLayout GetSettings(LiveChart chart) => (VerticalStackLayout)((ScrollView)((Grid)((Grid)chart.Content).Children[BodyRowIndex]).Children[SettingsColumnIndex]).Content;

    /// <summary>Queues the settings rebuild directly for deterministic lifecycle verification.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="sender">The collection.</param>
    /// <param name="args">The collection change.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "SeriesChanged")]
    private static extern void SeriesChanged(LiveChart chart, object? sender, NotifyCollectionChangedEventArgs args);

    /// <summary>Invokes the callback wired to native detachment.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The native event arguments.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ChartUnloaded")]
    private static extern void ChartUnloaded(LiveChart chart, object? sender, EventArgs args);
}
