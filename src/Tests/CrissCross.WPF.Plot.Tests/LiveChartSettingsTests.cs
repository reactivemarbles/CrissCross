// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;
using AppBarButton = CrissCross.Reactive.WPF.UI.Controls.AppBarButton;
#else
using CrissCross.WPF.Plot;
using ReactiveUI;
using ReactiveUI.Builder;
using AppBarButton = CrissCross.WPF.UI.Controls.AppBarButton;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies the chart settings popup interaction and action states.</summary>
[NotInParallel]
public sealed class LiveChartSettingsTests
{
    /// <summary>Verifies consecutive settings clicks close and reopen the popup.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task PlotSettings_ConsecutiveClicks_TogglesPopup()
    {
        var observed = await OnSta(static chart =>
        {
            Click(chart.PlotSettings);
            var first = Popup(chart).IsOpen;
            Click(chart.PlotSettings);
            var second = Popup(chart).IsOpen;
            Click(chart.PlotSettings);
            var third = Popup(chart).IsOpen;
            Popup(chart).IsOpen = false;
            Click(chart.PlotSettings);
            return (first, second, third, Reopened: Popup(chart).IsOpen);
        });
        await Assert.That(observed.first).IsTrue();
        await Assert.That(observed.second).IsFalse();
        await Assert.That(observed.third).IsTrue();
        await Assert.That(observed.Reopened).IsTrue();
    }

    /// <summary>Verifies the settings button is the sole toolbar action.</summary>
    /// <param name="name">The removed control name.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments("LiveHistoryBtn")]
    [Arguments("EnableMarkerBtn")]
    [Arguments("AddCrosshairBtn")]
    [Arguments("RemoveLabelBtn")]
    public async Task Toolbar_RedundantButton_IsAbsent(string name)
    {
        var found = await OnSta(chart => chart.FindName(name) is not null);
        await Assert.That(found).IsFalse();
    }

    /// <summary>Verifies annotation actions remain available after toolbar removal.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Actions_AddCrosshairAndClearAnnotations_UpdatePlot()
    {
        var observed = await OnSta(static chart =>
        {
            Click(chart.PlotSettings);
            var plot = chart.ViewModel!.WpfPlot1vm!.Plot;
            var initial = plot.PlottableList.Count;
            Click(FindAction(Popup(chart).Child, "Add crosshair"));
            var added = plot.PlottableList.Count;
            Click(FindAction(Popup(chart).Child, "Clear annotations"));
            return (initial, added, Cleared: plot.PlottableList.Count);
        });
        await Assert.That(observed.added).IsGreaterThan(observed.initial);
        await Assert.That(observed.Cleared).IsEqualTo(observed.initial);
    }

    /// <summary>Verifies toggle icons update in place and persist when the popup is rebuilt.</summary>
    /// <param name="label">The toggle action label.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments("Follow / explore data")]
    [Arguments("Pointer values")]
    [Arguments("Light / dark plot")]
    [Arguments("Plot legend")]
    [Arguments("Grid lines")]
    [Arguments("Series selector")]
    [Arguments("Y-axis visibility")]
    public async Task Action_Toggled_UpdatesIconAndPreservesState(string label)
    {
        var observed = await OnSta(chart =>
        {
            Click(chart.PlotSettings);
            var button = FindAction(Popup(chart).Child, label);
            var initial = button.Icon;
            var initialState = ReadState(chart, label);
            Click(button);
            var toggled = button.Icon;
            var toggledState = ReadState(chart, label);
            Click(chart.PlotSettings);
            Click(chart.PlotSettings);
            button = FindAction(Popup(chart).Child, label);
            var reopened = button.Icon;
            Click(button);
            return (initial, toggled, reopened, Restored: button.Icon, initialState, toggledState, RestoredState: ReadState(chart, label));
        });
        await Assert.That(observed.toggled).IsNotEqualTo(observed.initial);
        await Assert.That(observed.reopened).IsEqualTo(observed.toggled);
        await Assert.That(observed.Restored).IsEqualTo(observed.initial);
        await Assert.That(observed.toggledState).IsNotEqualTo(observed.initialState);
        await Assert.That(observed.RestoredState).IsEqualTo(observed.initialState);
    }

    /// <summary>Reads the actual plot state represented by a toggle action.</summary>
    /// <param name="chart">The chart.</param>
    /// <param name="label">The toggle action name.</param>
    /// <returns>The represented enabled state.</returns>
    private static bool ReadState(LiveChart chart, string label) => label switch
    {
        "Follow / explore data" => chart.ViewModel!.WpfPlot1vm!.Plot.Axes.ContinuouslyAutoscale,
        "Pointer values" => chart.ViewModel!.CrossHairEnabled,
        "Light / dark plot" => chart.ViewModel!.CurrentTheme == ReactivePlotTheme.Light,
        "Plot legend" => chart.ViewModel!.WpfPlot1vm!.Plot.Legend.IsVisible,
        "Grid lines" => chart.ViewModel!.WpfPlot1vm!.Plot.Grid.XAxisStyle.MajorLineStyle.Width != 0,
        "Series selector" => chart.TopLegendViewer.Visibility == Visibility.Visible || chart.RightLegendViewer.Visibility == Visibility.Visible,
        "Y-axis visibility" => chart.ViewModel!.YAxisList.Exists(static axis => axis.IsVisible),
        _ => throw new ArgumentOutOfRangeException(nameof(label)),
    };

    /// <summary>Finds an action by its accessible name in the popup content.</summary>
    /// <param name="root">The content root.</param>
    /// <param name="label">The action name.</param>
    /// <returns>The matching action.</returns>
    private static AppBarButton FindAction(DependencyObject root, string label)
    {
        if (root is AppBarButton button && AutomationProperties.GetName(button) == label)
        {
            return button;
        }

        foreach (var child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is DependencyObject dependency)
            {
                var result = FindActionOrNull(dependency, label);
                if (result is not null)
                {
                    return result;
                }
            }
        }

        throw new InvalidOperationException($"Action '{label}' was not found.");
    }

    /// <summary>Finds an action within a logical subtree.</summary>
    /// <param name="root">The subtree root.</param>
    /// <param name="label">The accessible name.</param>
    /// <returns>The matching action, if present.</returns>
    private static AppBarButton? FindActionOrNull(DependencyObject root, string label)
    {
        if (root is AppBarButton button && AutomationProperties.GetName(button) == label)
        {
            return button;
        }

        foreach (var child in LogicalTreeHelper.GetChildren(root))
        {
            if (child is DependencyObject dependency && FindActionOrNull(dependency, label) is { } result)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>Reads the popup without extending the public chart API.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The settings popup.</returns>
    private static Popup Popup(LiveChart chart) =>
        SettingsPopup(chart) ?? throw new InvalidOperationException("Settings popup has not been opened.");

    /// <summary>Reads the popup field without runtime reflection.</summary>
    /// <param name="chart">The chart.</param>
    /// <returns>The popup field reference.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_settingsPopup")]
    private static extern ref Popup? SettingsPopup(LiveChart chart);

    /// <summary>Raises a real button click and drains queued UI work.</summary>
    /// <param name="button">The button.</param>
    private static void Click(Button button)
    {
        button.RaiseEvent(new(ButtonBase.ClickEvent));
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);
    }

    /// <summary>Runs chart interactions on a Windows STA dispatcher.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The chart interaction.</param>
    /// <returns>The interaction result.</returns>
    private static Task<T> OnSta<T>(Func<LiveChart, T> action)
    {
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            T result = default!;
            Exception? failure = null;
            _ = Dispatcher.CurrentDispatcher;
            _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
            var previous = RxSchedulers.MainThreadScheduler;
            RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
            try
            {
                var chart = new LiveChart { LegendPosition = LegendPosition.Right };
                chart.ViewModel!.YAxesSetup((["Test"], ["#FFFFFF"]));
                var window = new Window { Content = chart, ShowInTaskbar = false };
                try
                {
                    window.Show();
                    Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);
                    result = action(chart);
                }
                finally
                {
                    window.Content = null;
                    window.Close();
                    chart.ViewModel?.Dispose();
                }
            }
            catch (Exception exception)
            {
                failure = exception;
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previous;
            }

            if (failure is null)
            {
                _ = completion.TrySetResult(result);
            }
            else
            {
                _ = completion.TrySetException(failure);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }
}
