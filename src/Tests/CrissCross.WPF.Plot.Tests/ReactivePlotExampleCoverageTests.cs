// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Static coverage tests for reactive WPF plot example and public control binding surface.</summary>
public sealed class ReactivePlotExampleCoverageTests
{
    /// <summary>The WPF plot project name.</summary>
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
        var control = ReadSource(PlotProjectName, ViewsDirectoryName, "LiveChart.xaml.cs");

        await Assert.That(control).Contains("DisposeReactivePlotConnection");
        await Assert.That(control).Contains("new ActionDisposable(DisposeReactivePlotConnection).DisposeWith(d)");
        await Assert.That(control).Contains("UnloadedObservable");
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
        await Assert.That(adapter).Contains("signal.ClearData()");
        await Assert.That(dataLogger).Contains("base.Dispose(disposing)");
        await Assert.That(signal).Contains("public void ClearData()");
    }

    /// <summary>Reads a repository file.</summary>
    /// <param name="relativeSegments">The relative path segments.</param>
    /// <returns>The file content.</returns>
    private static string ReadRepositoryFile(params string[] relativeSegments)
    {
        var path = Path.GetFullPath(Path.Combine(new[] { SourceRoot, ".." }.Concat(relativeSegments).ToArray()));
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
        var path = Path.Combine(new[] { SourceRoot }.Concat(relativeSegments).ToArray());
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected source file was not found: {path}", path);
        }

        return File.ReadAllText(path);
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
}
