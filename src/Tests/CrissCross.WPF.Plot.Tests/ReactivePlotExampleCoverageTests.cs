// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>
/// Static coverage tests for reactive WPF plot example and public control binding surface.
/// </summary>
public sealed class ReactivePlotExampleCoverageTests
{
    private static readonly string SourceRoot = LocateSourceRoot();

    [Test]
    public async Task WpfPlotExample_DemonstratesObservableFirstSourcesForEveryChartType()
    {
        var viewModel = ReadSource("CrissCross.WPF.Plot.Test", "ViewModels", "MainViewModel.cs");
        var view = ReadSource("CrissCross.WPF.Plot.Test", "Views", "MainView.xaml.cs");
        var properties = ReadSource("CrissCross.WPF.Plot", "Views", "LiveChart{Properties}.cs");
        var dependencies = ReadSource("CrissCross.WPF.Plot", "Views", "LiveChart{Dependencies}.cs");
        var documentation = ReadRepositoryFile("docs", "reactive-wpf-plot-streams.md");

        await Assert.That(viewModel).Contains("ReactivePlotSource.FromSignalTicks");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromScatterPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromDataLoggerPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromStreamerPoints");
        await Assert.That(viewModel).Contains("ReactivePlotSource.FromSignalXyPoints");
        await Assert.That(viewModel).Contains("IObservable<IReadOnlyList<IReactivePlotSource>>");
        await Assert.That(viewModel).Contains("SerialDisposable");
        await Assert.That(view).Contains("ReactivePlotSources");
        await Assert.That(properties).Contains("IEnumerable<IReactivePlotSource>?");
        await Assert.That(dependencies).Contains("ReactivePlotSourcesProperty");
        await Assert.That(documentation).Contains("ReactivePlotSource.FromSignalTicks");
        await Assert.That(documentation).Contains("ReactivePlotSource.FromSignalXyPoints");
        await Assert.That(documentation).Contains("ReactivePlotBindingOptions");
    }

    [Test]
    public async Task LiveChart_DisposesReactivePlotConnectionOnActivationTeardownAndUnload()
    {
        var control = ReadSource("CrissCross.WPF.Plot", "Views", "LiveChart.xaml.cs");

        await Assert.That(control).Contains("DisposeReactivePlotConnection");
        await Assert.That(control).Contains("Disposable.Create(DisposeReactivePlotConnection).DisposeWith(d)");
        await Assert.That(control).Contains("UnloadedObservable");
    }

    [Test]
    public async Task WpfAdapters_DocumentRetentionAndClearStateResetContracts()
    {
        var adapter = ReadSource("CrissCross.WPF.Plot", "WpfReactivePlotAdapter.cs");
        var dataLogger = ReadSource("CrissCross.WPF.Plot", "Controls", "DataLoggerUI.cs");
        var signal = ReadSource("CrissCross.WPF.Plot", "Controls", "SignalUI.cs");

        await Assert.That(adapter).Contains("PrepareSnapshotUpdate");
        await Assert.That(adapter).Contains("update.MaxPoints ?? int.MaxValue");
        await Assert.That(adapter).Contains("signal.ClearData()");
        await Assert.That(dataLogger).Contains("base.Dispose(disposing)");
        await Assert.That(signal).Contains("public void ClearData()");
    }

    private static string ReadRepositoryFile(params string[] relativeSegments)
    {
        var path = Path.GetFullPath(Path.Combine(new[] { SourceRoot, ".." }.Concat(relativeSegments).ToArray()));
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected repository file was not found: {path}", path);
        }

        return File.ReadAllText(path);
    }

    private static string ReadSource(params string[] relativeSegments)
    {
        var path = Path.Combine(new[] { SourceRoot }.Concat(relativeSegments).ToArray());
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected source file was not found: {path}", path);
        }

        return File.ReadAllText(path);
    }

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

        throw new DirectoryNotFoundException("Unable to locate CrissCross.slnx from the current test working directory.");
    }
}
