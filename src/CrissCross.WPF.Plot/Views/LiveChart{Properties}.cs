// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CP.Reactive;

namespace CrissCross.WPF.Plot;

/// <summary>
/// AICSLiveChart.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class LiveChart
{
    /// <summary>
    /// Gets or sets the Y Axis Data 1.
    /// </summary>
    [Description("Gets or sets the Observables")]
    [Category("AICS Y Axis")]
    public IEnumerable<IObservable<(string Name, double Value)>> Observables
    {
        get => (IEnumerable<IObservable<(string Name, double Value)>>)GetValue(ObservablesProperty);
        set => SetValue(ObservablesProperty, value);
    }

    /// <summary>
    /// Gets or sets the width ater the legend.
    /// </summary>
    public GridLength RightWidth
    {
        get => (GridLength)GetValue(RightWidthProperty);
        set => SetValue(RightWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width ater the legend.
    /// </summary>
    public double LegendWidth
    {
        get => (double)GetValue(LegendWidthProperty);
        set => SetValue(LegendWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width ater the legend.
    /// </summary>
    public int NSamples
    {
        get => (int)GetValue(NSamplesProperty);
        set => SetValue(NSamplesProperty, value);
    }

    /// <summary>
    /// Gets or sets the width ater the legend.
    /// </summary>
    public int Frequency
    {
        get => (int)GetValue(FrequencyProperty);
        set => SetValue(FrequencyProperty, value);
    }

    /// <summary>
    /// Gets or sets the observables with time stamp.
    /// </summary>
    /// <value>
    /// The observables with time stamp.
    /// </value>
    public string TitleContent
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the observables with time stamp.
    /// </summary>
    /// <value>
    /// The observables with time stamp.
    /// </value>
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the observables with time stamp.
    /// </summary>
    /// <value>
    /// The observables with time stamp.
    /// </value>
    public IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> ScatterObservablesWithTimeStamp
    {
        get => (IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>>)GetValue(ScatterObservablesWithTimeStampProperty);
        set
        {
            SetValue(ScatterObservablesWithTimeStampProperty, value);
            ChangeScatterObserver();
        }
    }

    /// <summary>
    /// Gets or sets the observables with time stamp.
    /// </summary>
    /// <value>
    /// The observables with time stamp.
    /// </value>
    public IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> SignalObservablesWithTimeStamp
    {
        get => (IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>)GetValue(SignalObservablesWithTimeStampProperty);
        set
        {
            SetValue(SignalObservablesWithTimeStampProperty, value);
            ChangeSignalObserver();
        }
    }

    /// <summary>
    /// Gets or sets the observables with time stamp.
    /// </summary>
    /// <value>
    /// The observables with time stamp.
    /// </value>
    public IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> DataLoggerObservablesWithPoints
    {
        get => (IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>>)GetValue(DataLoggerObservablesWithPointsProperty);
        set
        {
            SetValue(DataLoggerObservablesWithPointsProperty, value);
            ChangeDataLoggerObserver();
        }
    }

    /// <summary>
    /// Gets or sets the data with time stamp.
    /// </summary>
    /// <value>
    /// The data with time stamp.
    /// </value>
    public (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) DataWithTimeStamp
    {
        get => ((string? Name, IList<double>? Value, IList<double> DateTime, int Axis))GetValue(DataWithTimeStampProperty);
        set
        {
            SetValue(DataWithTimeStampProperty, value);
            ChangeSignalData();
        }
    }

    /// <summary>
    /// Gets or sets the data with time stamp.
    /// </summary>
    /// <value>
    /// The data with time stamp.
    /// </value>
    public (string? Name, IList<double>? Y, IList<double> X, int Axis) SignalWithPoints
    {
        get => ((string? Name, IList<double>? Y, IList<double> X, int Axis))GetValue(SignalWithPointsProperty);
        set
        {
            SetValue(SignalWithPointsProperty, value);
            ChangeSignalDataWithPoints();
        }
    }

    /// <summary>
    /// Gets or sets the data with time stamp.
    /// </summary>
    /// <value>
    /// The data with time stamp.
    /// </value>
    public IEnumerable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> SignalsWithPoints
    {
        get => (IEnumerable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>)GetValue(SignalsWithPointsProperty);
        set
        {
            SetValue(SignalsWithPointsProperty, value);
            ChangeSignalsDataWithPoints();
        }
    }

    /// <summary>
    /// Gets or sets the data with time stamp. // TODO: Need to be tested and finished.
    /// </summary>
    /// <value>
    /// The data with time stamp. // TODO: Need to be tested and finished.
    /// </value>
    public IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> SignalObservablesWithPoints
    {
        get => (IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>>)GetValue(SignalObservablesWithPointsProperty);
        set
        {
            SetValue(SignalObservablesWithPointsProperty, value);
            ChangeSignalDataObserverWithPoints();
        }
    }

    /// <summary>
    /// Gets or sets the data with time stamp.
    /// </summary>
    /// <value>
    /// The data with time stamp.
    /// </value>
    public (string? Name, IList<double> X, IList<double> Y, int Axis) ScatterWithPoints
    {
        get => ((string? Name, IList<double> X, IList<double> Y, int Axis))GetValue(ScatterWithPointsProperty);
        set
        {
            SetValue(ScatterWithPointsProperty, value);
            ChangeScatterDataWithPoints();
        }
    }

    /// <summary>
    /// Gets or sets the axes.
    /// </summary>
    /// <value>
    /// The name of axes.
    /// </value>
    public (IList<string> yNames, IList<string> hexColors) YAxisName
    {
        get => ((IList<string> yNames, IList<string> hexColors))GetValue(YAxisNameProperty);
        set
        {
            SetValue(YAxisNameProperty, value);
            YAxisSetup();
        }
    }

    /// <summary>
    /// Gets or sets the axes.
    /// </summary>
    /// <value>
    /// The name of axes.
    /// </value>
    public ReactiveList<ChartObjects> ControlMenu
    {
        get => (ReactiveList<ChartObjects>)GetValue(ControlMenuProperty);
        set => SetValue(ControlMenuProperty, value);
    }

    /// <summary>
    /// Gets or sets the number points plotted.
    /// </summary>
    /// <value>
    /// The number points plotted.
    /// </value>
    public int NumberPointsPlotted
    {
        get => (int)GetValue(NumberPointsPlottedProperty);
        set => SetValue(NumberPointsPlottedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [use fixed number of points].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use fixed number of points]; otherwise, <c>false</c>.
    /// </value>
    public bool UseFixedNumberOfPoints
    {
        get => (bool)GetValue(UseFixedNumberOfPointsProperty);
        set => SetValue(UseFixedNumberOfPointsProperty, value);
    }

    /// <summary>
    /// Assigns the live chart data.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="type">The type.</param>
    public void AssignLiveChartData(object source, UserPlotType type)
    {
        switch (type)
        {
            case UserPlotType.SignalEnumObsTicks:
                if (source is SignalEnumObsTicks data1)
                {
                    ChangeSignalObserver(data1);
                }

                break;

            case UserPlotType.DataLoggerEnumObsPoints:
                if (source is DataLoggerEnumObsPoints data2)
                {
                    ChangeDataLoggerObserver(data2);
                }

                break;

            case UserPlotType.SignalXYTimestamp:
                if (source is SignalXYTimestamp data3)
                {
                    ChangeSignalData(data3);
                }

                break;

            case UserPlotType.SignalXYPoints:
                if (source is SignalXYPoints data4)
                {
                    ChangeSignalDataWithPoints(data4);
                }

                break;

            case UserPlotType.SignalXYEnumPoints:
                if (source is SignalXYEnumPoints data5)
                {
                    ChangeSignalsDataWithPoints(data5);
                }

                break;

            case UserPlotType.StreamerEnumObsPoints:
                if (source is StreamerEnumObsPoints data6)
                {
                    ChangeSignalDataObserverWithPoints(data6);
                }

                break;

            case UserPlotType.ScatterEnumObsPoints:
                if (source is ScatterEnumObsPoints data7)
                {
                    ChangeScatterObserver(data7);
                }

                break;

            case UserPlotType.ScatterPoints:
                if (source is ScatterPoints data8)
                {
                    ChangeScatterDataWithPoints(data8);
                }

                break;
        }
    }

    /// <summary>
    /// SignalEnumObsTicks.
    /// </summary>
    public record SignalEnumObsTicks(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>
    /// DataLoggerEnumObsPoints.
    /// </summary>
    public record DataLoggerEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> Data);

    /// <summary>
    /// SignalXYTimestamp.
    /// </summary>
    public record SignalXYTimestamp((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) Data);

    /// <summary>
    /// SignalXYPoints.
    /// </summary>
    public record SignalXYPoints((string? Name, IList<double>? Y, IList<double> X, int Axis) Data);

    /// <summary>
    /// SignalXYEnumPoints.
    /// </summary>
    public record SignalXYEnumPoints(IEnumerable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> Data);

    /// <summary>
    /// StreamerEnumObsPoints.
    /// </summary>
    public record StreamerEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>
    /// ScatterEnumObsPoints.
    /// </summary>
    public record ScatterEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> Data);

    /// <summary>
    /// ScatterPoints.
    /// </summary>
    public record ScatterPoints((string? Name, IList<double> X, IList<double> Y, int Axis) Data);
}
