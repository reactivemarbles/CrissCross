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
    /// Gets or sets the collection of observable sequences that provide named double values for the Y axis.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing a name and a corresponding value,
    /// which can be used for dynamic data binding or real-time updates in charting scenarios. The property is typically
    /// used to supply multiple data series for visualization components.</remarks>
    [Description("Gets or sets the Observables")]
    [Category("AICS Y Axis")]
    public IEnumerable<IObservable<(string Name, double Value)>> Observables
    {
        get => (IEnumerable<IObservable<(string Name, double Value)>>)GetValue(ObservablesProperty);
        set => SetValue(ObservablesProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the right column as a <see cref="GridLength"/> value.
    /// </summary>
    public GridLength RightWidth
    {
        get => (GridLength)GetValue(RightWidthProperty);
        set => SetValue(RightWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width, in device-independent units (pixels), of the legend area.
    /// </summary>
    public double LegendWidth
    {
        get => (double)GetValue(LegendWidthProperty);
        set => SetValue(LegendWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of samples used in calculations or data processing.
    /// </summary>
    public int NSamples
    {
        get => (int)GetValue(NSamplesProperty);
        set => SetValue(NSamplesProperty, value);
    }

    /// <summary>
    /// Gets or sets the frequency value used by the control.
    /// </summary>
    /// <remarks>The meaning and valid range of the frequency value depend on the specific control
    /// implementation. Setting this property updates the control's behavior to reflect the new frequency.</remarks>
    public int Frequency
    {
        get => (int)GetValue(FrequencyProperty);
        set => SetValue(FrequencyProperty, value);
    }

    /// <summary>
    /// Gets or sets the text content displayed in the title area of the control.
    /// </summary>
    public string TitleContent
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the position of the legend within the chart.
    /// </summary>
    /// <remarks>Changing this property updates the location where the legend is displayed. The available
    /// positions are defined by the <see cref="LegendPosition"/> enumeration.</remarks>
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of observable sequences that provide scatter plot data points with associated
    /// timestamps and axis information.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing an optional series name, optional X
    /// values, required Y values, and an axis identifier. The property is intended for scenarios where scatter plot
    /// data is streamed or updated over time, such as in real-time charting applications.</remarks>
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
    /// Gets or sets the collection of observables that provide signal data with associated timestamps and axis
    /// information.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing a signal name, its value, a list of
    /// timestamp values, and an axis identifier. The property is intended for scenarios where multiple time-stamped
    /// signal streams are monitored or visualized concurrently.</remarks>
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
    /// Gets or sets the collection of observable data streams for logging, each providing a sequence of named data
    /// points with associated axis information and a maximum number of points to retain.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing the name of the data series, a list
    /// of data values, the axis index, and the maximum number of points to keep. This property is typically used to
    /// bind multiple data sources for real-time logging or visualization scenarios.</remarks>
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
    /// Gets or sets the signal data along with associated time stamps and axis information.
    /// </summary>
    /// <remarks>The tuple contains the signal name, its values, corresponding time stamps, and the axis
    /// index. The <see langword="Value"/> and <see langword="DateTime"/> lists may be null or empty depending on the
    /// data source. Changing this property updates the underlying signal data and may trigger related events or UI
    /// updates.</remarks>
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
    /// Gets or sets the signal data, including its name, Y and X coordinate points, and axis index.
    /// </summary>
    /// <remarks>The tuple contains the signal's name, a list of Y values, a list of X values, and the axis
    /// index to which the signal is assigned. The Y list may be null to indicate missing or unavailable data. Changing
    /// this property updates the associated signal data with the provided points.</remarks>
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
    /// Gets or sets the collection of signal data points, where each item contains a signal name, Y-values, X-values,
    /// and an axis index.
    /// </summary>
    /// <remarks>Each tuple in the collection represents a distinct signal. The signal name may be null if
    /// unnamed. The Y and X lists must be of equal length for each signal. The axis index specifies which axis the
    /// signal is associated with. Setting this property updates the underlying signal data and may trigger related
    /// changes in the consuming component.</remarks>
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
    /// Gets or sets the collection of observables that provide signal data points for plotting or analysis.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing a signal name, Y-values, X-values,
    /// and an axis index. The property is typically used to bind multiple dynamic data sources to a visualization or
    /// processing component. Changing the collection will update the associated signal data observers.</remarks>
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
    /// Gets or sets the scatter plot data, including the series name, X and Y coordinate lists, and the axis index to
    /// which the data is assigned.
    /// </summary>
    /// <remarks>Setting this property updates the scatter plot with the provided points and may trigger a
    /// change in the displayed data. The X and Y lists must have the same number of elements. The axis index should
    /// correspond to a valid axis in the plot configuration.</remarks>
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
    /// Gets or sets the collection of Y-axis names and their associated colors for the chart.
    /// </summary>
    /// <remarks>The Y-axis names are provided as a list of strings, and each name can be associated with a
    /// color specified in hexadecimal format. The order of names and colors in the lists should correspond. If the
    /// number of colors is less than the number of names, default colors may be used for remaining axes.</remarks>
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
    /// Gets or sets the collection of chart objects displayed in the control menu.
    /// </summary>
    public ReactiveList<ChartObjects> ControlMenu
    {
        get => (ReactiveList<ChartObjects>)GetValue(ControlMenuProperty);
        set => SetValue(ControlMenuProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of data points that have been plotted on the chart.
    /// </summary>
    public int NumberPointsPlotted
    {
        get => (int)GetValue(NumberPointsPlottedProperty);
        set => SetValue(NumberPointsPlottedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a fixed number of points should be used for rendering or calculation.
    /// </summary>
    /// <remarks>When set to <see langword="true"/>, the system uses a predetermined number of points, which
    /// may affect performance and accuracy depending on the scenario. When set to <see langword="false"/>, the number
    /// of points may be determined dynamically based on other factors.</remarks>
    public bool UseFixedNumberOfPoints
    {
        get => (bool)GetValue(UseFixedNumberOfPointsProperty);
        set => SetValue(UseFixedNumberOfPointsProperty, value);
    }

    /// <summary>
    /// Assigns live chart data to the appropriate plot type based on the specified source and plot type.
    /// </summary>
    /// <remarks>The method selects the appropriate data assignment strategy according to the specified plot
    /// type. If <paramref name="source"/> is not of a compatible type for the given <paramref name="type"/>, no action
    /// is taken. This method does not throw exceptions for type mismatches.</remarks>
    /// <param name="source">The data source object containing chart data. The expected type depends on the value of <paramref name="type"/>;
    /// must be compatible with the selected plot type.</param>
    /// <param name="type">The plot type that determines how the chart data from <paramref name="source"/> will be assigned.</param>
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
    /// Represents a collection of observable signal data series, each identified by name and associated with X and Y
    /// values and an axis index.
    /// </summary>
    /// <param name="Data">The sequence of observables, where each observable produces tuples containing an optional series name, optional
    /// Y values, X values, and an axis identifier. Each tuple represents a set of signal data points for a specific
    /// series and axis.</param>
    public record SignalEnumObsTicks(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>
    /// Represents a collection of observable data streams, each providing named data points with associated values,
    /// axis information, and a maximum number of points.
    /// </summary>
    /// <param name="Data">An enumerable collection of observables, where each observable emits tuples containing the name of the data
    /// point, a list of double values, the axis index, and the maximum number of points to observe.</param>
    public record DataLoggerEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> Data);

    /// <summary>
    /// Represents a signal with X and Y values, associated timestamps, and axis information.
    /// </summary>
    /// <param name="Data">A tuple containing the signal name, a list of Y values, a list of timestamps as doubles, and the axis index. The
    /// name may be null if unspecified; the value and DateTime lists must not be null and should have matching lengths;
    /// Axis specifies which axis the signal is associated with.</param>
    public record SignalXYTimestamp((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) Data);

    /// <summary>
    /// Represents a set of XY signal data points, including optional metadata such as a name and axis assignment.
    /// </summary>
    /// <remarks>Use this record to encapsulate XY data for plotting or analysis, along with relevant
    /// metadata. The axis index can be used to distinguish between multiple axes in visualization scenarios.</remarks>
    /// <param name="Data">A tuple containing the signal name, the Y values, the X values, and the axis index. The name may be null to
    /// indicate an unnamed signal. The Y and X lists must contain the signal's data points; both lists should be of
    /// equal length. The axis parameter specifies which axis the signal is associated with.</param>
    public record SignalXYPoints((string? Name, IList<double>? Y, IList<double> X, int Axis) Data);

    /// <summary>
    /// Represents a collection of named XY data point series, each associated with an axis identifier.
    /// </summary>
    /// <remarks>This record is typically used to encapsulate multiple XY data series for plotting or
    /// analysis, where each series can be associated with a specific axis. The axis index allows grouping or
    /// distinguishing series by axis in visualization scenarios.</remarks>
    /// <param name="Data">The sequence of tuples containing the series name, Y-values, X-values, and axis index for each data point
    /// series. The name may be null for unnamed series. The Y and X lists must be non-null and of equal length for each
    /// tuple.</param>
    public record SignalXYEnumPoints(IEnumerable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> Data);

    /// <summary>
    /// Represents a collection of observable data streams, each providing named point data for plotting or analysis.
    /// </summary>
    /// <remarks>Each observable in the collection can be used to asynchronously receive updates to point
    /// data, which may be useful for real-time charting or monitoring scenarios. The axis identifier allows grouping or
    /// distinguishing data across multiple axes.</remarks>
    /// <param name="Data">An enumerable sequence of observables, where each observable emits tuples containing a name, Y-values, X-values,
    /// and an axis identifier for the data points.</param>
    public record StreamerEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>
    /// Represents a collection of observable scatter plot data points, where each observation includes a name, X and Y
    /// coordinates, and an axis identifier.
    /// </summary>
    /// <remarks>This record is typically used to model dynamic or real-time scatter plot data in charting or
    /// visualization scenarios. Each observable in the collection can emit multiple updates, allowing the scatter plot
    /// to reflect changes as new data arrives. The axis index can be used to associate points with specific axes in
    /// multi-axis charts.</remarks>
    /// <param name="Data">An enumerable sequence of observables, each emitting a tuple containing the point name, optional X coordinates,
    /// Y coordinates, and the axis index. The observables provide updates to the scatter plot data over time.</param>
    public record ScatterEnumObsPoints(IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> Data);

    /// <summary>
    /// Represents a set of scatter plot points, including their names, coordinates, and associated axis.
    /// </summary>
    /// <remarks>Use this record to encapsulate scatter plot data for visualization or analysis. The axis
    /// index can be used to distinguish between multiple axes in a chart.</remarks>
    /// <param name="Data">A tuple containing the name of the point set, the X and Y coordinate lists, and the axis index to which the
    /// points belong. The coordinate lists must have the same length.</param>
    public record ScatterPoints((string? Name, IList<double> X, IList<double> Y, int Axis) Data);
}
