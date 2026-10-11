// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !REACTIVELIST_REACTIVE
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CrissCross.Avalonia.Plot.Example.ViewModels;
using CrissCross.Avalonia.Plot.Example.Views;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies example scenarios exercise the complete public plotting surface.</summary>
[NotInParallel]
public sealed class AvaloniaPlotExampleTests
{
    /// <summary>The MinimumStudyCount test value.</summary>
    private const int MinimumStudyCount = 10;

    /// <summary>The LiveSourceCount test value.</summary>
    private const int LiveSourceCount = 5;

    /// <summary>The rendered example width.</summary>
    private const int PreviewWidth = 1200;

    /// <summary>The rendered example height.</summary>
    private const int PreviewHeight = 800;

    /// <summary>The live sample generation wait.</summary>
    private const int SampleWaitMilliseconds = 400;

    /// <summary>Verifies the default gallery covers every supported chart type.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task DefaultScenario_ContainsEveryChartType()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var vm = new MainViewModel();
            HashSet<PlotType> types = [];
            foreach (var source in vm.ActiveSources)
            {
                _ = types.Add(source.PlotType);
            }

            return types;
        });
        foreach (var type in Enum.GetValues<PlotType>())
        {
            await Assert.That(observed.Contains(type)).IsTrue();
        }
    }

    /// <summary>Verifies scenarios change the emitted sources and retain all live chart types.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Commands_SelectHistoricStudiesAndLiveSources()
    {
        var observed = await PlotTestApp.Run(static () =>
        {
            using var vm = new MainViewModel();
            ((ICommand)vm.ShowHistoricCommand).Execute(null);
            var historic = true;
            foreach (var source in vm.ActiveSources)
            {
                historic &= source.XAxisKind == PlotXAxisKind.OADate;
            }

            ((ICommand)vm.ShowIndicatorsCommand).Execute(null);
            var studyCount = vm.ActiveSources.Count;
            ((ICommand)vm.ShowLiveCommand).Execute(null);
            var liveCount = vm.ActiveSources.Count;
            ((ICommand)vm.ShowAllChartTypesCommand).Execute(null);
            return (historic, studyCount, liveCount, RestoredCount: vm.ActiveSources.Count);
        });
        await Assert.That(observed.historic).IsTrue();
        await Assert.That(observed.studyCount).IsGreaterThan(MinimumStudyCount);
        await Assert.That(observed.liveCount).IsEqualTo(LiveSourceCount);
        await Assert.That(observed.RestoredCount).IsGreaterThan(observed.liveCount);
    }

    /// <summary>Loads the actual example visual tree, switches scenarios, and saves its rendered view.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task MainView_ScenariosAndTheme_RenderToPng()
    {
        var observed = await PlotTestApp.RunAsync(static async () =>
        {
            using var vm = new MainViewModel();
            using var view = new MainView { ViewModel = vm };
            var window = new Window { Content = view, Width = PreviewWidth, Height = PreviewHeight };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var chart = view.FindControl<LiveChart>("Chart")!;
            using var chartModel = chart.ViewModel!;
            ((ICommand)vm.ShowHistoricCommand).Execute(null);
            Dispatcher.UIThread.RunJobs();
            var historicCount = chartModel.SeriesSettings.Count;
            ((ICommand)vm.ShowIndicatorsCommand).Execute(null);
            Dispatcher.UIThread.RunJobs();
            var studyCount = chartModel.SeriesSettings.Count;
            ((ICommand)vm.ToggleThemeCommand).Execute(null);
            ((ICommand)vm.ShowAllChartTypesCommand).Execute(null);
            Dispatcher.UIThread.RunJobs();
            await Task.Delay(SampleWaitMilliseconds);
            Dispatcher.UIThread.RunJobs();
            chart.TitleContent = "All chart types";
            chartModel.SelectedSetting = chartModel.PlotLinesCollectionUI[0].ChartSettings;
            chartModel.RightPropertyVisibility = true;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            using var bitmap = new RenderTargetBitmap(new(PreviewWidth, PreviewHeight));
            bitmap.Render(window);
            var outputDirectory = Path.Combine(AppContext.BaseDirectory, "artifacts", "visuals");
            _ = Directory.CreateDirectory(outputDirectory);
            var imagePath = Path.Combine(outputDirectory, "avalonia-plot-example.png");
            bitmap.Save(imagePath, new PngBitmapEncoderOptions());
            var imageLength = new FileInfo(imagePath).Length;
            var theme = vm.IsLightTheme;
            var restoredCount = chartModel.SeriesSettings.Count;
            var result = (historicCount, studyCount, theme, restoredCount, imageLength,
                LiveCount: chartModel.PlotLinesCollectionUI.Count, Title: chart.ChartTitleTextBlock.Text, RightPanelVisible: chart.RightPropertiesPanel.IsVisible);
            window.Close();
            return result;
        });
        await Assert.That(observed.historicCount).IsEqualTo(1);
        await Assert.That(observed.studyCount).IsGreaterThan(MinimumStudyCount);
        await Assert.That(observed.theme).IsTrue();
        await Assert.That(observed.restoredCount).IsGreaterThan(LiveSourceCount);
        await Assert.That(observed.imageLength).IsGreaterThan(PreviewWidth);
        await Assert.That(observed.LiveCount).IsEqualTo(LiveSourceCount);
        await Assert.That(observed.Title).IsEqualTo("All chart types");
        await Assert.That(observed.RightPanelVisible).IsTrue();
    }
}
#endif
