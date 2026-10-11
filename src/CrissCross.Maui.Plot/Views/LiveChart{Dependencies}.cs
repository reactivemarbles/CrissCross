// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using Microsoft.Maui.Controls;
#if REACTIVE_SHIM
using CP.Reactive.Collections;
#else
using CP.Primitives.Collections;
#endif
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif
/// <summary>Bindable native chart inputs.</summary>
public partial class LiveChart
{
    /// <summary>Identifies UseFixedNumberOfPoints.</summary>
    public static readonly BindableProperty UseFixedNumberOfPointsProperty =
        BindableProperty.Create(
            nameof(UseFixedNumberOfPoints),
            typeof(bool),
            typeof(LiveChart),
            default(bool),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(UseFixedNumberOfPoints)));

    /// <summary>Identifies NumberPointsPlotted.</summary>
    public static readonly BindableProperty NumberPointsPlottedProperty =
        BindableProperty.Create(
            nameof(NumberPointsPlotted),
            typeof(int),
            typeof(LiveChart),
            600,
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(NumberPointsPlotted)));

    /// <summary>Identifies TitleContent.</summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(TitleContent),
            typeof(string),
            typeof(LiveChart),
            " ",
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(TitleContent)));

    /// <summary>Identifies LegendPosition.</summary>
    public static readonly BindableProperty LegendPositionProperty =
        BindableProperty.Create(
            nameof(LegendPosition),
            typeof(LegendPosition),
            typeof(LiveChart),
            LegendPosition.Top,
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(LegendPosition)));

    /// <summary>Identifies RightWidth.</summary>
    public static readonly BindableProperty RightWidthProperty =
        BindableProperty.Create(
            nameof(RightWidth),
            typeof(GridLength),
            typeof(LiveChart),
            new GridLength(0),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(RightWidth)));

    /// <summary>Identifies LegendWidth.</summary>
    public static readonly BindableProperty LegendWidthProperty =
        BindableProperty.Create(
            nameof(LegendWidth),
            typeof(double),
            typeof(LiveChart),
            double.NaN,
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(LegendWidth)));

    /// <summary>Identifies NSamples.</summary>
    public static readonly BindableProperty NSamplesProperty =
        BindableProperty.Create(
            nameof(NSamples),
            typeof(int),
            typeof(LiveChart),
            2024,
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(NSamples)));

    /// <summary>Identifies Frequency.</summary>
    public static readonly BindableProperty FrequencyProperty =
        BindableProperty.Create(
            nameof(Frequency),
            typeof(int),
            typeof(LiveChart),
            32_000,
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(Frequency)));

    /// <summary>Identifies Observables.</summary>
    public static readonly BindableProperty ObservablesProperty =
        BindableProperty.Create(
            nameof(Observables),
            typeof(IEnumerable<IObservable<(string Name, double Value)>>),
            typeof(LiveChart),
            default(IEnumerable<IObservable<(string Name, double Value)>>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(Observables)));

    /// <summary>Identifies ReactivePlotSources.</summary>
    public static readonly BindableProperty ReactivePlotSourcesProperty =
        BindableProperty.Create(
            nameof(ReactivePlotSources),
            typeof(IEnumerable<IReactivePlotSource>),
            typeof(LiveChart),
            default(IEnumerable<IReactivePlotSource>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(ReactivePlotSources)));

    /// <summary>Identifies SignalObservablesWithTimeStamp.</summary>
    public static readonly BindableProperty SignalObservablesWithTimeStampProperty =
        BindableProperty.Create(
            nameof(SignalObservablesWithTimeStamp),
            typeof(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>),
            typeof(LiveChart),
            default(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(SignalObservablesWithTimeStamp)));

    /// <summary>Identifies DataLoggerObservablesWithPoints.</summary>
    public static readonly BindableProperty DataLoggerObservablesWithPointsProperty =
        BindableProperty.Create(
            nameof(DataLoggerObservablesWithPoints),
            typeof(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>>),
            typeof(LiveChart),
            default(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(DataLoggerObservablesWithPoints)));

    /// <summary>Identifies DataWithTimeStamp.</summary>
    public static readonly BindableProperty DataWithTimeStampProperty =
        BindableProperty.Create(
            nameof(DataWithTimeStamp),
            typeof((string? Name, IList<double>? Value, IList<double> DateTime, int Axis)),
            typeof(LiveChart),
            default((string? Name, IList<double>? Value, IList<double> DateTime, int Axis)),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(DataWithTimeStamp)));

    /// <summary>Identifies SignalWithPoints.</summary>
    public static readonly BindableProperty SignalWithPointsProperty =
        BindableProperty.Create(
            nameof(SignalWithPoints),
            typeof((string? Name, IList<double>? Value, IList<double> DateTime, int Axis)),
            typeof(LiveChart),
            default((string? Name, IList<double>? Value, IList<double> DateTime, int Axis)),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(SignalWithPoints)));

    /// <summary>Identifies SignalsWithPoints.</summary>
    public static readonly BindableProperty SignalsWithPointsProperty =
        BindableProperty.Create(
            nameof(SignalsWithPoints),
            typeof(IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>),
            typeof(LiveChart),
            default(IEnumerable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(SignalsWithPoints)));

    /// <summary>Identifies SignalObservablesWithPoints.</summary>
    public static readonly BindableProperty SignalObservablesWithPointsProperty =
        BindableProperty.Create(
            nameof(SignalObservablesWithPoints),
            typeof(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>>),
            typeof(LiveChart),
            default(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(SignalObservablesWithPoints)));

    /// <summary>Identifies ScatterObservablesWithTimeStamp.</summary>
    public static readonly BindableProperty ScatterObservablesWithTimeStampProperty =
        BindableProperty.Create(
            nameof(ScatterObservablesWithTimeStamp),
            typeof(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>),
            typeof(LiveChart),
            default(IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(ScatterObservablesWithTimeStamp)));

    /// <summary>Identifies ScatterWithPoints.</summary>
    public static readonly BindableProperty ScatterWithPointsProperty =
        BindableProperty.Create(
            nameof(ScatterWithPoints),
            typeof((string? Name, IList<double>? X, IList<double> Y, int Axis)),
            typeof(LiveChart),
            default((string? Name, IList<double>? X, IList<double> Y, int Axis)),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(ScatterWithPoints)));

    /// <summary>Identifies YAxisName.</summary>
    public static readonly BindableProperty YAxisNameProperty =
        BindableProperty.Create(
            nameof(YAxisName),
            typeof((IList<string> yNames, IList<string> hexColors)),
            typeof(LiveChart),
            default((IList<string> yNames, IList<string> hexColors)),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(YAxisName)));

    /// <summary>Identifies ControlMenu.</summary>
    public static readonly BindableProperty ControlMenuProperty =
        BindableProperty.Create(
            nameof(ControlMenu),
            typeof(QuaternaryList<ChartObjects>),
            typeof(LiveChart),
            default(QuaternaryList<ChartObjects>),
            propertyChanged: static (source, oldValue, newValue) => ((LiveChart)source).InputChanged(nameof(ControlMenu)));
}
