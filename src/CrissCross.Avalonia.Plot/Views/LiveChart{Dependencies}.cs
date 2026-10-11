// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
#if REACTIVE_SHIM
using CP.Reactive.Collections;
#else
using CP.Primitives.Collections;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Defines bindable live chart configuration and source properties.</summary>
public partial class LiveChart
{
    /// <summary>Identifies the UseFixedNumberOfPoints styled property.</summary>
    public static readonly StyledProperty<bool> UseFixedNumberOfPointsProperty =
        AvaloniaProperty.Register<LiveChart, bool>(nameof(UseFixedNumberOfPoints));

    /// <summary>Identifies the NumberPointsPlotted styled property.</summary>
    public static readonly StyledProperty<int> NumberPointsPlottedProperty =
        AvaloniaProperty.Register<LiveChart, int>(nameof(NumberPointsPlotted), 600);

    /// <summary>Identifies the TitleContent styled property.</summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<LiveChart, string>(nameof(TitleContent), " ");

    /// <summary>Identifies the LegendPosition styled property.</summary>
    public static readonly StyledProperty<LegendPosition> LegendPositionProperty =
        AvaloniaProperty.Register<LiveChart, LegendPosition>(nameof(LegendPosition), LegendPosition.Top);

    /// <summary>Identifies the RightWidth styled property.</summary>
    public static readonly StyledProperty<GridLength> RightWidthProperty =
        AvaloniaProperty.Register<LiveChart, GridLength>(nameof(RightWidth), new(0));

    /// <summary>Identifies the LegendWidth styled property.</summary>
    public static readonly StyledProperty<double> LegendWidthProperty =
        AvaloniaProperty.Register<LiveChart, double>(nameof(LegendWidth), double.NaN);

    /// <summary>Identifies the NSamples styled property.</summary>
    public static readonly StyledProperty<int> NSamplesProperty =
        AvaloniaProperty.Register<LiveChart, int>(nameof(NSamples), 2024);

    /// <summary>Identifies the Frequency styled property.</summary>
    public static readonly StyledProperty<int> FrequencyProperty =
        AvaloniaProperty.Register<LiveChart, int>(nameof(Frequency), 32_000);

    /// <summary>Identifies the Observables styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IObservable<(string Name, double Value)>>?> ObservablesProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IObservable<(string Name, double Value)>>?>(nameof(Observables));

    /// <summary>Identifies the ReactivePlotSources styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IReactivePlotSource>?> ReactivePlotSourcesProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IReactivePlotSource>?>(nameof(ReactivePlotSources));

    /// <summary>Identifies the SignalObservablesWithTimeStamp styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>?> SignalObservablesWithTimeStampProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>?>(nameof(SignalObservablesWithTimeStamp));

    /// <summary>Identifies the DataLoggerObservablesWithPoints styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>>?> DataLoggerObservablesWithPointsProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>>?>(nameof(DataLoggerObservablesWithPoints));

    /// <summary>Identifies the DataWithTimeStamp styled property.</summary>
    public static readonly StyledProperty<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> DataWithTimeStampProperty =
        AvaloniaProperty.Register<LiveChart, (string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>(nameof(DataWithTimeStamp));

    /// <summary>Identifies the SignalWithPoints styled property.</summary>
    public static readonly StyledProperty<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)> SignalWithPointsProperty =
        AvaloniaProperty.Register<LiveChart, (string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>(nameof(SignalWithPoints));

    /// <summary>Identifies the SignalsWithPoints styled property.</summary>
    public static readonly StyledProperty<IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>?> SignalsWithPointsProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>?>(nameof(SignalsWithPoints));

    /// <summary>Identifies the SignalObservablesWithPoints styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>>?> SignalObservablesWithPointsProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>>?>(nameof(SignalObservablesWithPoints));

    /// <summary>Identifies the ScatterObservablesWithTimeStamp styled property.</summary>
    public static readonly StyledProperty<IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>?> ScatterObservablesWithTimeStampProperty =
        AvaloniaProperty.Register<LiveChart, IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>?>(nameof(ScatterObservablesWithTimeStamp));

    /// <summary>Identifies the ScatterWithPoints styled property.</summary>
    public static readonly StyledProperty<(string? Name, IList<double>? X, IList<double> Y, int Axis)> ScatterWithPointsProperty =
        AvaloniaProperty.Register<LiveChart, (string? Name, IList<double>? X, IList<double> Y, int Axis)>(nameof(ScatterWithPoints));

    /// <summary>Identifies the YAxisName styled property.</summary>
    public static readonly StyledProperty<(IList<string> yNames, IList<string> hexColors)> YAxisNameProperty =
        AvaloniaProperty.Register<LiveChart, (IList<string> yNames, IList<string> hexColors)>(nameof(YAxisName));

    /// <summary>Identifies the ControlMenu styled property.</summary>
    public static readonly StyledProperty<QuaternaryList<ChartObjects>> ControlMenuProperty =
        AvaloniaProperty.Register<LiveChart, QuaternaryList<ChartObjects>>(nameof(ControlMenu));

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (ViewModel is null)
        {
            return;
        }

        UpdateChartConfiguration(change.Property);
        UpdateChartSources(change.Property);
        UpdateAxisConfiguration(change.Property);
    }

    /// <summary>Applies presentation and display-window property changes.</summary>
    /// <param name="property">The changed styled property.</param>
    private void UpdateChartConfiguration(AvaloniaProperty property)
    {
        if (property == UseFixedNumberOfPointsProperty)
        {
            ViewModel!.UseFixedNumberOfPoints = UseFixedNumberOfPoints;
        }

        if (property == NumberPointsPlottedProperty)
        {
            ViewModel!.NumberPointsPlotted = NumberPointsPlotted;
        }

        if (property == TitleProperty)
        {
            ViewModel!.Title = TitleContent;
        }

        if (property == LegendPositionProperty)
        {
            ViewModel!.LegendPosition = LegendPosition;
        }

        if (property != LegendWidthProperty)
        {
            return;
        }

        RightLegend.Width = LegendWidth;
    }

    /// <summary>Reconnects the affected reactive or legacy data source.</summary>
    /// <param name="property">The changed styled property.</param>
    private void UpdateChartSources(AvaloniaProperty property)
    {
        if (property == ReactivePlotSourcesProperty)
        {
            ChangeReactivePlotSources(ReactivePlotSources);
        }

        if (property == SignalObservablesWithTimeStampProperty)
        {
            ChangeSignalObserver();
        }

        if (property == DataLoggerObservablesWithPointsProperty)
        {
            ChangeDataLoggerObserver();
        }

        if (property == DataWithTimeStampProperty)
        {
            ChangeSignalData();
        }

        if (property == SignalWithPointsProperty)
        {
            ChangeSignalDataWithPoints();
        }

        if (property == SignalsWithPointsProperty)
        {
            ChangeSignalsDataWithPoints();
        }

        if (property == SignalObservablesWithPointsProperty)
        {
            ChangeSignalDataObserverWithPoints();
        }

        if (property == ScatterObservablesWithTimeStampProperty)
        {
            ChangeScatterObserver();
        }

        if (property != ScatterWithPointsProperty)
        {
            return;
        }

        ChangeScatterDataWithPoints();
    }

    /// <summary>Applies changed axis metadata and reconnects current sources.</summary>
    /// <param name="property">The changed styled property.</param>
    private void UpdateAxisConfiguration(AvaloniaProperty property)
    {
        if (property != YAxisNameProperty)
        {
            return;
        }

        YAxisSetup();
        RefreshReactivePlotSources();
    }

    /// <summary>Reconnects sources after axis metadata changes.</summary>
    private void RefreshReactivePlotSources()
    {
        if (ReactivePlotSources is null)
        {
            return;
        }

        ChangeReactivePlotSources(ReactivePlotSources);
    }
}
