// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.Plot;
#else
using CrissCross.Maui.Plot;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Exercises real MAUI controls and the lifecycle callbacks wired to their native events.</summary>
[NotInParallel]
public sealed class MauiNativeControlTests
{
    /// <summary>The second sample.</summary>
    private const double SecondValue = 2;

    /// <summary>The third sample.</summary>
    private const double ThirdValue = 3;

    /// <summary>The edited appearance width.</summary>
    private const double EditedWidth = 7;

    /// <summary>The image width.</summary>
    private const int ImageWidth = 640;

    /// <summary>The image height.</summary>
    private const int ImageHeight = 360;

    /// <summary>The pixel conversion tolerance.</summary>
    private const double PixelTolerance = 0.001;

    /// <summary>Verifies source replacement, clearing, and disposal release live subscriptions.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Sources_ReplaceClearAndDispose_ReleaseObservers()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var chart = new LiveChart();
            using var first = new Signal<ReactivePlotUpdate>();
            using var second = new Signal<ReactivePlotUpdate>();
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(new(nameof(first), 0), PlotType.Line, first)];
            PlotTestApp.Drain();
            first.OnNext(new(new(nameof(first), 0), PlotType.Line, ReactivePlotUpdateKind.Replace, [1D], [SecondValue], PlotXAxisKind.Numeric, 1));
            PlotTestApp.Drain();
            var initial = chart.ViewModel.SeriesSettings.Count;
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(new(nameof(second), 0), PlotType.Points, second)];
            PlotTestApp.Drain();
            second.OnNext(new(new(nameof(second), 0), PlotType.Points, ReactivePlotUpdateKind.Replace, [1D], [ThirdValue], PlotXAxisKind.Numeric, 1));
            PlotTestApp.Drain();
            var oldReleased = !first.HasObservers;
            var name = chart.ViewModel.SeriesSettings[0].Key.Name;
            chart.ReactivePlotSources = null;
            PlotTestApp.Drain();
            var cleared = chart.ViewModel.SeriesSettings.Count;
            var secondReleased = !second.HasObservers;
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(new(nameof(first), 0), PlotType.Line, first)];
            PlotTestApp.Drain();
            chart.Dispose();
            return (initial, oldReleased, name, cleared, secondReleased, Disposed: !first.HasObservers, Connection: chart.ReactivePlotConnection);
        });
        await Assert.That(observed.initial).IsEqualTo(1);
        await Assert.That(observed.oldReleased).IsTrue();
        await Assert.That(observed.name).IsEqualTo("second");
        await Assert.That(observed.cleared).IsEqualTo(0);
        await Assert.That(observed.secondReleased).IsTrue();
        await Assert.That(observed.Disposed).IsTrue();
        await Assert.That(observed.Connection).IsNull();
    }

    /// <summary>Verifies the callbacks used by native attachment replay sources and detach releases them.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task NativeLifecycle_DetachAndReload_ReplaysStaticSource()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart { ReactivePlotSources = [ReactivePlotSource.FromStatic(PlotSeriesData.Numeric("replay", 0, [1D], [SecondValue]), PlotType.Line)] };
            PlotTestApp.Drain();
            var initial = chart.ViewModel.SeriesSettings.Count;
            ChartUnloaded(chart, chart, EventArgs.Empty);
            var detached = chart.ViewModel.SeriesSettings.Count;
            var disconnected = chart.ReactivePlotConnection is null;
            chart.BindReactivePlotSources();
            var stillDetached = chart.ViewModel.SeriesSettings.Count;
            ChartLoaded(chart, chart, EventArgs.Empty);
            PlotTestApp.Drain();
            return (initial, detached, disconnected, stillDetached, Restored: chart.ViewModel.SeriesSettings.Count);
        });
        await Assert.That(observed.initial).IsEqualTo(1);
        await Assert.That(observed.detached).IsEqualTo(0);
        await Assert.That(observed.disconnected).IsTrue();
        await Assert.That(observed.stillDetached).IsEqualTo(0);
        await Assert.That(observed.Restored).IsEqualTo(1);
    }

    /// <summary>Verifies the settings panel builds native enum pickers and edits retained presentation.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Settings_OpenAndEdit_UpdatesNativeControlsAndSeries()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart { ReactivePlotSources = [ReactivePlotSource.FromStatic(PlotSeriesData.Numeric("line", 0, [1D, SecondValue], [SecondValue, ThirdValue]), PlotType.Line)] };
            PlotTestApp.Drain();
            chart.OpenPlotSettings();
            var settings = GetSettings(chart);
            var legend = GetPicker(settings, "Legend");
            var modes = GetPicker(settings, "Line mode");
            var allChoicesPresent = AllPickersHaveChoices(settings);
            legend.SelectedItem = LegendPosition.Left;
            modes.SelectedItem = PlotLineMode.MarkersOnly;
            GetEntry(settings, "Chart title").Text = "Edited title";
            GetEntry(settings, "Color").Text = "Blue";
            GetEntry(settings, "Line width").Text = "7";
            GetEntry(settings, "Color").Text = "invalid color";
            GetEntry(settings, "Line width").Text = "NaN";
            var series = chart.ViewModel.SeriesSettings[0];
            chart.OpenPlotSettings();
            return (allChoicesPresent, chart.LegendPosition, Title: chart.ChartTitleTextBlock.Text, series.Color, series.LineWidth, series.LineMode, Hidden: !GetSettingsScroll(chart).IsVisible);
        });
        await Assert.That(observed.allChoicesPresent).IsTrue();
        await Assert.That(observed.LegendPosition).IsEqualTo(LegendPosition.Left);
        await Assert.That(observed.Title).IsEqualTo("Edited title");
        await Assert.That(observed.Color).IsEqualTo("Blue");
        await Assert.That(observed.LineWidth).IsEqualTo(EditedWidth);
        await Assert.That(observed.LineMode).IsEqualTo(PlotLineMode.MarkersOnly);
        await Assert.That(observed.Hidden).IsTrue();
    }

    /// <summary>Verifies new series rebuild an open settings panel with a native editor.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Settings_OpenBeforeData_AddsSeriesEditors()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart();
            using var updates = new Signal<ReactivePlotUpdate>();
            chart.ReactivePlotSources = [ReactivePlotSource.FromUpdates(new("live", 0), PlotType.Line, updates)];
            PlotTestApp.Drain();
            chart.OpenPlotSettings();
            var before = GetSettings(chart).Children.Count;
            updates.OnNext(new(new("live", 0), PlotType.Line, ReactivePlotUpdateKind.Replace, [1D], [SecondValue], PlotXAxisKind.Numeric, 1));
            PlotTestApp.WaitUntil(() => HasLabel(GetSettings(chart), "Name"));
            return (before, After: GetSettings(chart).Children.Count, HasName: HasLabel(GetSettings(chart), "Name"));
        });
        await Assert.That(observed.After).IsGreaterThan(observed.before);
        await Assert.That(observed.HasName).IsTrue();
    }

    /// <summary>Verifies native appearance entries and pickers save to the selected chart object.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Appearance_EditAndSave_PersistsSelection()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var view = new RightPropertiesView();
            using var selected = new ChartObjects { ItemName = "original", LineWidth = (float)SecondValue, Color = "Red", Visibility = "Visible" };
            view.ViewModel!.SelectedSetting = selected;
            view.ItemNameTextBox.Text = "edited";
            view.LineWidthNumberBox.Text = "7";
            view.ColorsComboBox.SelectedItem = "Blue";
            view.VisibilityComboBox.SelectedItem = "Invisible";
            ((ICommand)view.ViewModel.SaveConfiguration).Execute(null);
            view.LineWidthNumberBox.Text = "NaN";
            return (selected.ItemName, selected.LineWidth, selected.Color, selected.Visibility, Width: view.ViewModel.LineWidth);
        });
        await Assert.That(observed.ItemName).IsEqualTo("edited");
        await Assert.That(observed.LineWidth).IsEqualTo((float)EditedWidth);
        await Assert.That(observed.Color).IsEqualTo("Blue");
        await Assert.That(observed.Visibility).IsEqualTo("Invisible");
        await Assert.That(observed.Width).IsEqualTo(EditedWidth);
    }

    /// <summary>Verifies replacing the appearance model stops obsolete model and choice notifications.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Appearance_ReplaceAndDispose_StopsOldNotifications()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            var view = new RightPropertiesView();
            using var previous = view.ViewModel!;
            using var next = new RightPropertiesViewModel { ItemName = "next", LineWidth = ThirdValue };
            view.ViewModel = next;
            previous.ItemName = "obsolete";
            next.LineColors.Add("Cyan");
            PlotTestApp.WaitUntil(() => view.ColorsComboBox.Items.Contains("Cyan"));
            var updated = view.ItemNameTextBox.Text;
            var hasNewChoice = view.ColorsComboBox.Items.Contains("Cyan");
            view.Dispose();
            next.ItemName = "after disposal";
            return (updated, hasNewChoice, Final: view.ItemNameTextBox.Text);
        });
        await Assert.That(observed.updated).IsEqualTo("next");
        await Assert.That(observed.hasNewChoice).IsTrue();
        await Assert.That(observed.Final).IsEqualTo("next");
    }

    /// <summary>Verifies native touch events move an axis marker and restore the navigation lock.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Touch_DragCrosshair_ChangesPositionAndReleasesLock()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart();
            var vm = chart.ViewModel;
            var plot = vm.MauiPlot1vm!.Plot;
            vm.AddCrosshair();
            var line = vm.CrosshairCollection[0].PlotLine!.VerticalLine;
            line.Position = SecondValue;
            vm.CrosshairCollection[0].PlotLine!.HorizontalLine.Position = ThirdValue;
            plot.Axes.ContinuouslyAutoscale = false;
            plot.Axes.SetLimits(0, EditedWidth, 0, EditedWidth);
            _ = plot.GetImage(ImageWidth, ImageHeight);
            var pressedPixel = plot.GetPixel(new(SecondValue, 1), vm.XAxis1, vm.YAxisList[0]);
            var movePixel = plot.GetPixel(new(ThirdValue, 1), vm.XAxis1, vm.YAxisList[0]);
            var pressed = new SKTouchEventArgs(1, SKTouchAction.Pressed, new(pressedPixel.X, pressedPixel.Y), true);
            ((ISKCanvasView)vm.MauiPlot1vm).OnTouch(pressed);
            var moved = new SKTouchEventArgs(1, SKTouchAction.Moved, new(movePixel.X, movePixel.Y), true);
            ((ISKCanvasView)vm.MauiPlot1vm).OnTouch(moved);
            var position = line.Position;
            ((ISKCanvasView)vm.MauiPlot1vm).OnTouch(new(1, SKTouchAction.Released, new(movePixel.X, movePixel.Y), false));
            return (pressed.Handled, Moved: moved.Handled, position, Locked: !vm.MauiPlot1vm.UserInputProcessor.IsEnabled);
        });
        await Assert.That(observed.Handled).IsTrue();
        await Assert.That(observed.Moved).IsTrue();
        await Assert.That(observed.position).IsEqualTo(ThirdValue).Within(PixelTolerance);
        await Assert.That(observed.Locked).IsTrue();
    }

    /// <summary>Verifies native toolbar actions toggle pointer state, navigation, and crosshairs.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Toolbar_ClickActions_UpdatesNativePlotState()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var chart = new LiveChart();
            ClickToolbar(chart, "Lock");
            var unlocked = chart.ViewModel.MauiPlot1vm!.UserInputProcessor.IsEnabled;
            ClickToolbar(chart, "Pointer");
            var pointer = chart.ViewModel.CrossHairEnabled;
            ClickToolbar(chart, "Crosshair");
            var count = chart.ViewModel.CrosshairCollection.Count;
            ClickToolbar(chart, "Clear crosshairs");
            chart.ApplyTheme(ReactivePlotTheme.Light);
            return (unlocked, pointer, count, Cleared: chart.ViewModel.CrosshairCollection.Count, Background: chart.BackgroundColor.ToArgbHex());
        });
        await Assert.That(observed.unlocked).IsTrue();
        await Assert.That(observed.pointer).IsTrue();
        await Assert.That(observed.count).IsEqualTo(1);
        await Assert.That(observed.Cleared).IsEqualTo(0);
        await Assert.That(observed.Background).IsEqualTo(Microsoft.Maui.Graphics.Color.FromArgb(ReactivePlotTheme.Light.FigureBackground).ToArgbHex());
    }

    /// <summary>Clicks a real MAUI toolbar button through its controller.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="caption">The caption.</param>
    private static void ClickToolbar(LiveChart chart, string caption)
    {
        var toolbar = (HorizontalStackLayout)((ScrollView)((Grid)chart.Content).Children[1]).Content;
        foreach (var item in toolbar.Children)
        {
            if (item is Button button && button.Text == caption)
            {
                ((IButtonController)button).SendClicked();
                return;
            }
        }

        throw new InvalidOperationException("Missing toolbar button.");
    }

    /// <summary>Finds a native picker by caption.</summary>
    /// <param name="settings">The settings.</param>
    /// <param name="caption">The caption.</param>
    /// <returns>The picker.</returns>
    private static Picker GetPicker(VerticalStackLayout settings, string caption)
    {
        foreach (var item in settings.Children)
        {
            if (item is Picker picker && picker.Title == caption)
            {
                return picker;
            }
        }

        throw new InvalidOperationException("Missing native picker.");
    }

    /// <summary>Checks the native choice lists.</summary>
    /// <param name="settings">The settings.</param>
    /// <returns>Whether every picker has choices.</returns>
    private static bool AllPickersHaveChoices(VerticalStackLayout settings)
    {
        foreach (var item in settings.Children)
        {
            if (item is Picker picker && picker.Items.Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Finds a native label.</summary>
    /// <param name="settings">The settings.</param>
    /// <param name="caption">The caption.</param>
    /// <returns>Whether the label is present.</returns>
    private static bool HasLabel(VerticalStackLayout settings, string caption)
    {
        foreach (var item in settings.Children)
        {
            if (item is Label label && label.Text == caption)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets the public native settings scroll container.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The scroll container.</returns>
    private static ScrollView GetSettingsScroll(LiveChart chart) => (ScrollView)((Grid)((Grid)chart.Content).Children[2]).Children[1];

    /// <summary>Gets the native settings layout.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The settings layout.</returns>
    private static VerticalStackLayout GetSettings(LiveChart chart) => (VerticalStackLayout)GetSettingsScroll(chart).Content;

    /// <summary>Gets the entry immediately following its caption.</summary>
    /// <param name="settings">The settings layout.</param>
    /// <param name="caption">The caption.</param>
    /// <returns>The native entry.</returns>
    private static Entry GetEntry(VerticalStackLayout settings, string caption)
    {
        for (var index = 0; index < settings.Children.Count - 1; index++)
        {
            if (settings.Children[index] is Label label && label.Text == caption)
            {
                return (Entry)settings.Children[index + 1];
            }
        }

        throw new InvalidOperationException("Missing native entry.");
    }

    /// <summary>Invokes the callback wired to native detachment.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The native event arguments.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ChartUnloaded")]
    private static extern void ChartUnloaded(LiveChart chart, object? sender, EventArgs args);

    /// <summary>Invokes the callback wired to native attachment.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The native event arguments.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "ChartLoaded")]
    private static extern void ChartLoaded(LiveChart chart, object? sender, EventArgs args);
}
