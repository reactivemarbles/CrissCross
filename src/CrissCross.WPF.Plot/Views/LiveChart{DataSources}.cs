// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
#if !REACTIVE_SHIM
using ReactiveUI;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Interaction logic for WPF Chart AICS.</summary>
public partial class LiveChart
{
    /// <summary>Handles the ChangeReactivePlotSources operation.</summary>
    /// <param name="sources">The sources value.</param>
    private void ChangeReactivePlotSources(IEnumerable<IReactivePlotSource>? sources)
    {
        DisposeReactivePlotConnection();
        if (ViewModel is null || sources is null)
        {
            return;
        }

        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel.ClearContent();
        var sourceArray = (sources as IReactivePlotSource[]) ?? sources.ToArray();
        ConfigureReactivePlotXAxis(sourceArray);
        ViewModel.HideAllYAxis();
        _reactivePlotConnection = new ReactivePlotBinder().Bind(
            ViewModel,
            sourceArray,
            new ReactivePlotBindingOptions
            {
                UiScheduler = RxSchedulers.MainThreadScheduler,
                MaxVisiblePoints = (UseFixedNumberOfPoints ? NumberPointsPlotted : null),
                MaxAxisCount = Math.Max(1, ViewModel.YAxisList.Count),
            });
        ViewModel.InitializeAxisLines();
    }

    /// <summary>Handles the DisposeReactivePlotConnection operation.</summary>
    private void DisposeReactivePlotConnection()
    {
        _reactivePlotConnection?.Dispose();
        _reactivePlotConnection = null;
    }

    /// <summary>Handles the UnloadedObservable operation.</summary>
    /// <returns>The result.</returns>
    private IObservable<EventPattern<RoutedEventArgs>> UnloadedObservable() =>
        Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
            handler => Unloaded += handler,
            handler => Unloaded -= handler);

    /// <summary>Handles the ChangeScatterObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeScatterObserver(ScatterEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeScatterPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterObserver operation.</summary>
    private void ChangeScatterObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeScatterPlotLines(ScatterObservablesWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalObserver(SignalEnumObsTicks input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
        _crosshairDisposable = ViewModel
            .WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(d => ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings.IsCrossHairVisible = d));
    }

    /// <summary>Handles the ChangeSignalObserver operation.</summary>
    private void ChangeSignalObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(SignalObservablesWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
        _crosshairDisposable = ViewModel
            .WhenAnyValue(x => x.CrossHairEnabled)
            .Subscribe(d => ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings.IsCrossHairVisible = d));
    }

    /// <summary>Handles the ChangeDataLoggerObserver operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeDataLoggerObserver(DataLoggerEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeDataLoggerPlotLinesWithPoints(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeDataLoggerObserver operation.</summary>
    private void ChangeDataLoggerObserver()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeDataLoggerPlotLinesWithPoints(DataLoggerObservablesWithPoints);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalData operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalData(SignalXYTimestamp input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(input.Data);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalData operation.</summary>
    private void ChangeSignalData()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeSignalPlotLines(DataWithTimeStamp);
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalDataWithPoints(SignalXYPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataWithPoints operation.</summary>
    private void ChangeSignalDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalPoints(SignalWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalsDataWithPoints(SignalXYEnumPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();

        ViewModel?.InitializeLinesForSignalPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalsDataWithPoints operation.</summary>
    private void ChangeSignalsDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();

        ViewModel?.InitializeLinesForSignalPoints(SignalsWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataObserverWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeSignalDataObserverWithPoints(StreamerEnumObsPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(
            input.Data,
            fs: Frequency,
            sampleCount: Convert.ToUInt32(NSamples));
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeSignalDataObserverWithPoints operation.</summary>
    private void ChangeSignalDataObserverWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForSignalObservablesPoints(
            SignalObservablesWithPoints,
            fs: Frequency,
            sampleCount: Convert.ToUInt32(NSamples));
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterDataWithPoints operation.</summary>
    /// <param name="input">The input value.</param>
    private void ChangeScatterDataWithPoints(ScatterPoints input)
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForScatterPoints(input.Data);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }

    /// <summary>Handles the ChangeScatterDataWithPoints operation.</summary>
    private void ChangeScatterDataWithPoints()
    {
        _needLock = true;
        ExecuteLockUnlock();
        _needAutoScale = true;
        ExecuteManAutoScale();
        _needCrossHairOff = true;
        ExecuteMarkerOnOff();
        ViewModel?.InitializeLinesForScatterPoints(ScatterWithPoints);
        ViewModel?.InitializeControlMenu(ViewModel?.PlotLinesCollectionUI.Select(x => x.ChartSettings).ToList());
        ViewModel?.InitializeAxisLines();
        _crosshairDisposable?.Dispose();
    }
}
