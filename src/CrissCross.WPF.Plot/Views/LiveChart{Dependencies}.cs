// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CP.Reactive;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
/// <seealso cref="System.Windows.Controls.UserControl" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class LiveChart
{
    /// <summary>
    /// The use fixed number of points property.
    /// </summary>
    public static readonly DependencyProperty UseFixedNumberOfPointsProperty =
        DependencyProperty.Register(nameof(UseFixedNumberOfPoints), typeof(bool), typeof(LiveChart), new PropertyMetadata(false, new(UseFixedNumberOfPointsCallback)));

    /// <summary>
    /// The number points plotted property.
    /// </summary>
    public static readonly DependencyProperty NumberPointsPlottedProperty =
        DependencyProperty.Register(nameof(NumberPointsPlotted), typeof(int), typeof(LiveChart), new PropertyMetadata(600, new(NumberPointsPlottedCallback)));

    /// <summary>
    /// RightWidth after legend.
    /// </summary>
    public static readonly DependencyProperty RightWidthProperty =
        DependencyProperty.Register(
            nameof(RightWidth),
            typeof(GridLength),
            typeof(LiveChart),
            new PropertyMetadata(
            new GridLength(0),
            new PropertyChangedCallback(RightWidthCallback)));

    /// <summary>
    /// RightWidth after legend.
    /// </summary>
    public static readonly DependencyProperty LegendWidthProperty =
        DependencyProperty.Register(
            nameof(LegendWidth),
            typeof(double),
            typeof(LiveChart),
            new(new PropertyChangedCallback(LegendWidthCallback)));

    /// <summary>
    /// Y Axis Data 2 Property.
    /// </summary>
    public static readonly DependencyProperty ObservablesProperty =
        DependencyProperty.Register(nameof(Observables), typeof(IEnumerable<IObservable<(string, double)>>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty SignalObservablesWithTimeStampProperty =
        DependencyProperty.Register(nameof(SignalObservablesWithTimeStamp), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty DataLoggerObservablesWithPointsProperty =
        DependencyProperty.Register(nameof(DataLoggerObservablesWithPoints), typeof(IEnumerable<IObservable<(string?, IList<double>?, int, int)>>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty DataWithTimeStampProperty =
        DependencyProperty.Register(nameof(DataWithTimeStamp), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty SignalWithPointsProperty =
        DependencyProperty.Register(nameof(SignalWithPoints), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty SignalsWithPointsProperty =
        DependencyProperty.Register(nameof(SignalsWithPoints), typeof(IEnumerable<(string?, IList<double>?, IList<double>, int)>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property. // TODO: Need to be tested and finished.
    /// </summary>
    public static readonly DependencyProperty SignalObservablesWithPointsProperty =
        DependencyProperty.Register(nameof(SignalObservablesWithPoints), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty ScatterObservablesWithTimeStampProperty =
        DependencyProperty.Register(nameof(ScatterObservablesWithTimeStamp), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// The observables with time stamp property.
    /// </summary>
    public static readonly DependencyProperty ScatterWithPointsProperty =
        DependencyProperty.Register(nameof(ScatterWithPoints), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// The axes names and colors.
    /// </summary>
    public static readonly DependencyProperty YAxisNameProperty =
        DependencyProperty.Register(nameof(YAxisName), typeof((IList<string>, IList<string>)), typeof(LiveChart));

    /// <summary>
    /// The axes names and colors.
    /// </summary>
    public static readonly DependencyProperty ControlMenuProperty =
        DependencyProperty.Register(nameof(ControlMenu), typeof(ReactiveList<ChartObjects>), typeof(LiveChart));

    private static void RightWidthCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is GridLength gridLength)
        {
            livechart.RightWidth = gridLength;
        }
    }

    private static void LegendWidthCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is double gridLength)
        {
            livechart.RightLegend.Width = gridLength;
        }
    }

    private static void NumberPointsPlottedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is int numberOfSamples)
        {
            livechart.NumberPointsPlotted = numberOfSamples;
            livechart.ViewModel.NumberPointsPlotted = numberOfSamples;
        }
    }

    private static void UseFixedNumberOfPointsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is bool fixedNumberOfSamples)
        {
            livechart.UseFixedNumberOfPoints = fixedNumberOfSamples;
            livechart.ViewModel.UseFixedNumberOfPoints = fixedNumberOfSamples;
        }
    }
}
