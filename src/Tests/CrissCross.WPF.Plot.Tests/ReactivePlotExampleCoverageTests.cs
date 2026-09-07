// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Windows;
using System.Windows.Threading;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;
#else
using CrissCross.WPF.Plot;
using ReactiveUI;
using ReactiveUI.Builder;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Static coverage tests for reactive WPF plot example and public control binding surface.</summary>
public sealed class ReactivePlotExampleCoverageTests
{
    /// <summary>The source project linked into both WPF plot package variants.</summary>
    private const string PlotProjectName = "CrissCross.WPF.Plot";

    /// <summary>The views directory name.</summary>
    private const string ViewsDirectoryName = "Views";

    /// <summary>Stores the source root.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Verifies the WPF plot example demonstrates observable-first sources for every chart type.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WpfPlotExample_DemonstratesObservableFirstSourcesForEveryChartType()
    {
        var viewModel = ReadSource("CrissCross.WPF.Plot.Test", "ViewModels", "MainViewModel.cs");
        var view = ReadSource("CrissCross.WPF.Plot.Test", ViewsDirectoryName, "MainView.xaml.cs");
        var properties = ReadSource(PlotProjectName, ViewsDirectoryName, "LiveChart{Properties}.cs");
        var dependencies = ReadSource(PlotProjectName, ViewsDirectoryName, "LiveChart{Dependencies}.cs");
        var documentation = ReadRepositoryFile("docs", "reactive-wpf-plot-streams.md");

        await Assert.That(viewModel).Contains("ReactivePlotSource.FromSignalPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromScatterPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromDataLoggerPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromStreamerPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromSignalXyPoints");
        await Assert.That(viewModel).Contains("IObservable<IReadOnlyList<IReactivePlotSource>>");
        await Assert.That(viewModel).Contains("SerialDisposable");
        await Assert.That(view).Contains("ReactivePlotSources");
        await Assert.That(properties).Contains("IEnumerable<IReactivePlotSource>?");
        await Assert.That(dependencies).Contains("ReactivePlotSourcesProperty");
        await Assert.That(documentation).Contains("ReactivePlotSource.FromSignalPoints");
        await Assert.That(documentation).Contains("ReactivePlotSource.FromSignalXyPoints");
        await Assert.That(documentation).Contains("ReactivePlotBindingOptions");
    }

    /// <summary>Verifies LiveChart disposes the reactive plot connection on activation teardown and unload.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task LiveChart_DisposesReactivePlotConnectionOnActivationTeardownAndUnload()
    {
        var snapshot = await RunOnStaThreadAsync(ExerciseLiveChartCrosshairLifecycle);

        await Assert.That(snapshot.SignalCrosshairVisibleAfterEnable).IsTrue();
        await Assert.That(snapshot.SignalCrosshairChangedAfterDisposedToggle).IsFalse();
        await Assert.That(snapshot.ScatterCrosshairChangedAfterDisposedToggle).IsFalse();
        await Assert.That(snapshot.ConnectionActiveBeforeUnload).IsTrue();
        await Assert.That(snapshot.ConnectionClearedAfterUnload).IsTrue();
    }

    /// <summary>Verifies WPF adapters document retention and clear state reset contracts.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WpfAdapters_DocumentRetentionAndClearStateResetContracts()
    {
        var adapter = ReadSource(PlotProjectName, "WpfReactivePlotAdapter.cs");
        var dataLogger = ReadSource(PlotProjectName, "Controls", "DataLoggerUI.cs");
        var signal = ReadSource(PlotProjectName, "Controls", "SignalUI.cs");

        await Assert.That(adapter).Contains("PrepareSnapshotUpdate");
        await Assert.That(adapter).Contains("update.MaxPoints ?? int.MaxValue");
        await Assert.That(adapter).Contains("signal.ClearData");
        await Assert.That(dataLogger).Contains("base.Dispose(disposing)");
        await Assert.That(signal).Contains("public void ClearData()");
    }

    /// <summary>Reads a repository file.</summary>
    /// <param name="relativeSegments">The relative path segments.</param>
    /// <returns>The file content.</returns>
    private static string ReadRepositoryFile(params string[] relativeSegments)
    {
        var path = Path.GetFullPath(Path.Combine(SourceRoot, "..", Path.Combine(relativeSegments)));
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected repository file was not found: {path}", path);
        }

        return File.ReadAllText(path);
    }

    /// <summary>Reads a source file.</summary>
    /// <param name="relativeSegments">The relative path segments.</param>
    /// <returns>The file content.</returns>
    private static string ReadSource(params string[] relativeSegments)
    {
        var path = Path.Combine(SourceRoot, Path.Combine(relativeSegments));
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected source file was not found: {path}", path);
        }

        return File.ReadAllText(path);
    }

    /// <summary>Exercises LiveChart crosshair and unload lifecycle behavior on a Windows STA thread.</summary>
    /// <returns>The observed lifecycle state.</returns>
    private static LiveChartLifecycleSnapshot ExerciseLiveChartCrosshairLifecycle()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
        var previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
        try
        {
            return ExerciseLiveChartCrosshairLifecycleSynchronously();
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = previousMainThreadScheduler;
        }
    }

    /// <summary>Exercises LiveChart lifecycle behavior with actual source rebinding.</summary>
    /// <returns>The observed lifecycle state.</returns>
    private static LiveChartLifecycleSnapshot ExerciseLiveChartCrosshairLifecycleSynchronously()
    {
        using var reactiveSignalTicks = new Signal<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>();
        using var signalTicks = new Signal<(string? Name, IList<double>? Y, IList<double> X, int Axis)>();
        using var scatterPoints = new Signal<(string? Name, IList<double>? X, IList<double> Y, int Axis)>();
        var chart = new LiveChart();
        var window = new Window { Content = chart };
        var windowClosed = false;

        try
        {
            window.Show();
            DrainDispatcher();

            chart.ReactivePlotSources = [ReactivePlotSource.FromSignalTicks(reactiveSignalTicks)];
            reactiveSignalTicks.OnNext(("ReactiveSignal", [1D], [1D], 0));
            DrainDispatcher();

            chart.AssignLiveChartData(
                new LiveChart.SignalEnumObsTicks([signalTicks]),
                UserPlotType.SignalEnumObsTicks);
            signalTicks.OnNext(("Signal", [1D], [1D], 0));
            DrainDispatcher();
            var signalSetting = chart.ViewModel!.PlotLinesCollectionUI[
                chart.ViewModel.PlotLinesCollectionUI.Count - 1].ChartSettings;
            chart.ViewModel.CrossHairEnabled = true;
            DrainDispatcher();
            var signalCrosshairVisibleAfterEnable = signalSetting.IsCrossHairVisible;

            chart.AssignLiveChartData(
                new LiveChart.ScatterEnumObsPoints([scatterPoints]),
                UserPlotType.ScatterEnumObsPoints);
            scatterPoints.OnNext(("Scatter", [1D], [1D], 0));
            DrainDispatcher();

            var scatterSetting = chart.ViewModel.PlotLinesCollectionUI[
                chart.ViewModel.PlotLinesCollectionUI.Count - 1].ChartSettings;
            var signalCrosshairAfterRebind = signalSetting.IsCrossHairVisible;
            var scatterCrosshairAfterRebind = scatterSetting.IsCrossHairVisible;
            chart.ViewModel.CrossHairEnabled = true;
            DrainDispatcher();

            var connectionActiveBeforeUnload = reactiveSignalTicks.HasObservers;
            window.Content = null;
            window.Close();
            windowClosed = true;
            DrainDispatcher();

            return new(
                signalCrosshairVisibleAfterEnable,
                signalSetting.IsCrossHairVisible != signalCrosshairAfterRebind,
                scatterSetting.IsCrossHairVisible != scatterCrosshairAfterRebind,
                connectionActiveBeforeUnload,
                !reactiveSignalTicks.HasObservers);
        }
        finally
        {
            if (!windowClosed)
            {
                window.Content = null;
                window.Close();
            }

            chart.ViewModel?.Dispose();
        }
    }

    /// <summary>Runs queued reactive notifications on the current Windows dispatcher.</summary>
    private static void DrainDispatcher() =>
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);

    /// <summary>Runs work on a Windows STA thread.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The platform action.</param>
    /// <returns>A task that completes with the action result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(
            () =>
            {
                try
                {
                    completion.SetResult(action());
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Locates the source root.</summary>
    /// <returns>The source root path.</returns>
    private static string LocateSourceRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "CrissCross.slnx");
            if (File.Exists(candidate))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException(
            "Unable to locate CrissCross.slnx from the current test working directory.");
    }

    /// <summary>Captures LiveChart lifecycle observations.</summary>
    /// <param name="SignalCrosshairVisibleAfterEnable">Whether the signal setting responded to crosshair
    /// enable.</param>
    /// <param name="SignalCrosshairChangedAfterDisposedToggle">Whether the old signal setting changed after rebind
    /// disposal.</param>
    /// <param name="ScatterCrosshairChangedAfterDisposedToggle">Whether the scatter setting changed without a
    /// subscription.</param>
    /// <param name="ConnectionActiveBeforeUnload">Whether the source had an active subscription before unload.</param>
    /// <param name="ConnectionClearedAfterUnload">Whether the reactive plot connection was cleared by unload.</param>
    private sealed record LiveChartLifecycleSnapshot(
        bool SignalCrosshairVisibleAfterEnable,
        bool SignalCrosshairChangedAfterDisposedToggle,
        bool ScatterCrosshairChangedAfterDisposedToggle,
        bool ConnectionActiveBeforeUnload,
        bool ConnectionClearedAfterUnload);
}
