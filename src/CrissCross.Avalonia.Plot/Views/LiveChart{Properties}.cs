// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
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

/// <summary>AICSLiveChart member.</summary>
/// <seealso cref="UserControl" />
public partial class LiveChart
{
    /// <summary>Gets or sets named observable Y-axis values.</summary>
    /// <remarks>Each observable in the collection emits tuples containing a name and a corresponding value,
    /// which can be used for dynamic data binding or real-time updates in charting scenarios. The property is typically
    /// used to supply multiple data series for visualization components.</remarks>
    [Description("Gets or sets the Observables")]
    [Category("AICS Y Axis")]
    public IEnumerable<IObservable<(string Name, double Value)>> Observables
    {
        get => GetValue(ObservablesProperty)!;
        set => _ = SetValue(ObservablesProperty, value);
    }

    /// <summary>Gets or sets normalized observable-first plot sources bound to this chart.</summary>
    public IEnumerable<IReactivePlotSource>? ReactivePlotSources
    {
        get => GetValue(ReactivePlotSourcesProperty)!;
        set => _ = SetValue(ReactivePlotSourcesProperty, value);
    }

    /// <summary>Gets or sets the width of the right column as a <see cref="GridLength"/> value.</summary>
    public GridLength RightWidth
    {
        get => GetValue(RightWidthProperty)!;
        set => _ = SetValue(RightWidthProperty, value);
    }

    /// <summary>Gets or sets the width, in device-independent units (pixels), of the legend area.</summary>
    public double LegendWidth
    {
        get => GetValue(LegendWidthProperty)!;
        set => _ = SetValue(LegendWidthProperty, value);
    }

    /// <summary>Gets or sets the number of samples used in calculations or data processing.</summary>
    public int NSamples
    {
        get => GetValue(NSamplesProperty)!;
        set => _ = SetValue(NSamplesProperty, value);
    }

    /// <summary>Gets or sets the frequency value used by the control.</summary>
    /// <remarks>The meaning and valid range of the frequency value depend on the specific control
    /// implementation. Setting this property updates the control's behavior to reflect the new frequency.</remarks>
    public int Frequency
    {
        get => GetValue(FrequencyProperty)!;
        set => _ = SetValue(FrequencyProperty, value);
    }

    /// <summary>Gets or sets the text content displayed in the title area of the control.</summary>
    public string TitleContent
    {
        get => GetValue(TitleProperty)!;
        set => _ = SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets the position of the legend within the chart.</summary>
    /// <remarks>Changing this property updates the location where the legend is displayed. The available
    /// positions are defined by the <see cref="LegendPosition"/> enumeration.</remarks>
    public LegendPosition LegendPosition
    {
        get => GetValue(LegendPositionProperty)!;
        set => _ = SetValue(LegendPositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of observable sequences that provide scatter plot data points with associated
    /// timestamps and axis information.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing an optional series name, optional X
    /// values, required Y values, and an axis identifier. The property is intended for scenarios where scatter plot
    /// data is streamed or updated over time, such as in real-time charting applications.</remarks>
    public IEnumerable<
        IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>
    > ScatterObservablesWithTimeStamp
    {
        get => GetValue(ScatterObservablesWithTimeStampProperty)!;
        set => _ = SetValue(ScatterObservablesWithTimeStampProperty, value);
    }

    /// <summary>Gets or sets signal data with timestamps and axis information.</summary>
    /// <remarks>Each observable in the collection emits tuples containing a signal name, its value, a list of
    /// timestamp values, and an axis identifier. The property is intended for scenarios where multiple time-stamped
    /// signal streams are monitored or visualized concurrently.</remarks>
    public IEnumerable<
        IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>
    > SignalObservablesWithTimeStamp
    {
        get => GetValue(SignalObservablesWithTimeStampProperty)!;
        set => _ = SetValue(SignalObservablesWithTimeStampProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of observable data streams for logging, each providing a sequence of named data
    /// points with associated axis information and a maximum number of points to retain.
    /// </summary>
    /// <remarks>Each observable in the collection emits tuples containing the name of the data series, a list
    /// of data values, the axis index, and the maximum number of points to keep. This property is typically used to
    /// bind multiple data sources for real-time logging or visualization scenarios.</remarks>
    public IEnumerable<
        IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>
    > DataLoggerObservablesWithPoints
    {
        get => GetValue(DataLoggerObservablesWithPointsProperty)!;
        set => _ = SetValue(DataLoggerObservablesWithPointsProperty, value);
    }

    /// <summary>Gets or sets the signal data along with associated time stamps and axis information.</summary>
    /// <remarks>The tuple contains the signal name, its values, corresponding time stamps, and the axis
    /// index. The <see langword="Value"/> and <see langword="DateTime"/> lists may be null or empty depending on the
    /// data source. Changing this property updates the underlying signal data and may trigger related events or UI
    /// updates.</remarks>
    public (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) DataWithTimeStamp
    {
        get => GetValue(DataWithTimeStampProperty)!;
        set => _ = SetValue(DataWithTimeStampProperty, value);
    }

    /// <summary>Gets or sets the signal data, including its name, Y and X coordinate points, and axis index.</summary>
    /// <remarks>The tuple contains the signal's name, a list of Y values, a list of X values, and the axis
    /// index to which the signal is assigned. The Y list may be null to indicate missing or unavailable data. Changing
    /// this property updates the associated signal data with the provided points.</remarks>
    public (string? Name, IList<double>? Y, IList<double> X, int Axis) SignalWithPoints
    {
        get => GetValue(SignalWithPointsProperty)!;
        set => _ = SetValue(SignalWithPointsProperty, value);
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
        get => GetValue(SignalsWithPointsProperty)!;
        set => _ = SetValue(SignalsWithPointsProperty, value);
    }

    /// <summary>Gets or sets observable signal points.</summary>
    /// <remarks>Each observable in the collection emits tuples containing a signal name, Y-values, X-values,
    /// and an axis index. The property is typically used to bind multiple dynamic data sources to a visualization or
    /// processing component. Changing the collection will update the associated signal data observers.</remarks>
    public IEnumerable<
        IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>
    > SignalObservablesWithPoints
    {
        get => GetValue(SignalObservablesWithPointsProperty)!;
        set => _ = SetValue(SignalObservablesWithPointsProperty, value);
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
        get => GetValue(ScatterWithPointsProperty)!;
        set => _ = SetValue(ScatterWithPointsProperty, value);
    }

    /// <summary>Gets or sets the collection of Y-axis names and their associated colors for the chart.</summary>
    /// <remarks>The Y-axis names are provided as a list of strings, and each name can be associated with a
    /// color specified in hexadecimal format. The order of names and colors in the lists should correspond. If the
    /// number of colors is less than the number of names, default colors may be used for remaining axes.</remarks>
    public (IList<string> yNames, IList<string> hexColors) YAxisName
    {
        get => GetValue(YAxisNameProperty)!;
        set => _ = SetValue(YAxisNameProperty, value);
    }

#if NET8_0_OR_GREATER
    /// <summary>Gets the collection of chart objects displayed in the control menu.</summary>
    public QuaternaryList<ChartObjects> ControlMenu
    {
        get => GetValue(ControlMenuProperty)!;
        private set => SetValue(ControlMenuProperty, value);
    }
#else
    /// <summary>Gets the collection of chart objects displayed in the control menu.</summary>
    public ReactiveList<ChartObjects> ControlMenu
    {
        get => GetValue(ControlMenuProperty)!;
        private set => SetValue(ControlMenuProperty, value);
    }
#endif

    /// <summary>Gets or sets the number of data points that have been plotted on the chart.</summary>
    public int NumberPointsPlotted
    {
        get => GetValue(NumberPointsPlottedProperty)!;
        set => _ = SetValue(NumberPointsPlottedProperty, value);
    }

    /// <summary>Gets or sets whether rendering uses a fixed point count.</summary>
    /// <remarks>When set to <see langword="true"/>, the system uses a predetermined number of points, which
    /// may affect performance and accuracy depending on the scenario. When set to <see langword="false"/>, the number
    /// of points may be determined dynamically based on other factors.</remarks>
    public bool UseFixedNumberOfPoints
    {
        get => GetValue(UseFixedNumberOfPointsProperty)!;
        set => _ = SetValue(UseFixedNumberOfPointsProperty, value);
    }

    /// <summary>Assigns live data to its selected plot type.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="type">The target plot type.</param>
    public void AssignLiveChartData(object source, UserPlotType type)
    {
        var assignment = ResolveLiveChartDataAssignment(source, type);
        assignment?.Invoke();
    }

    /// <summary>Resolves a live data assignment action for the selected plot type.</summary>
    /// <param name="source">The source value.</param>
    /// <param name="type">The target plot type.</param>
    /// <returns>The assignment action, or <see langword="null"/> when the source does not match the plot type.</returns>
    private Action? ResolveLiveChartDataAssignment(object source, UserPlotType type) =>
        (type, source) switch
        {
            (UserPlotType.SignalEnumObsTicks, SignalEnumObsTicks data) => () => ChangeSignalObserver(data),
            (UserPlotType.DataLoggerEnumObsPoints, DataLoggerEnumObsPoints data) => () => ChangeDataLoggerObserver(data),
            (UserPlotType.SignalXYTimestamp, SignalXYTimestamp data) => () => ChangeSignalData(data),
            (UserPlotType.SignalXYPoints, SignalXYPoints data) => () => ChangeSignalDataWithPoints(data),
            (UserPlotType.SignalXYEnumPoints, SignalXYEnumPoints data) => () => ChangeSignalsDataWithPoints(data),
            (UserPlotType.StreamerEnumObsPoints, StreamerEnumObsPoints data) => () => ChangeSignalDataObserverWithPoints(data),
            (UserPlotType.ScatterEnumObsPoints, ScatterEnumObsPoints data) => () => ChangeScatterObserver(data),
            (UserPlotType.ScatterPoints, ScatterPoints data) => () => ChangeScatterDataWithPoints(data),
            _ => null,
        };

    /// <summary>Provides named observable XY signal series with axis assignments.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record SignalEnumObsTicks(
        IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>Provides named observable data streams with axes and retention limits.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record DataLoggerEnumObsPoints(
        IEnumerable<IObservable<(string? Name, IList<double>? Value, int Axis, int nMaxPoints)>> Data);

    /// <summary>Represents a signal with X and Y values, associated timestamps, and axis information.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record SignalXYTimestamp((string? Name, IList<double>? Value, IList<double> DateTime, int Axis) Data);

    /// <summary>Provides XY signal points with optional names and axis assignments.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record SignalXYPoints((string? Name, IList<double>? Y, IList<double> X, int Axis) Data);

    /// <summary>Provides named XY data-point series with axis assignments.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record SignalXYEnumPoints(
        IEnumerable<(string? Name, IList<double>? Y, IList<double> X, int Axis)> Data);

    /// <summary>Provides named observable point-data streams.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record StreamerEnumObsPoints(
        IEnumerable<IObservable<(string? Name, IList<double>? Y, IList<double> X, int Axis)>> Data);

    /// <summary>Provides named observable scatter points with axis assignments.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record ScatterEnumObsPoints(
        IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>> Data);

    /// <summary>Provides named scatter points with coordinates and axes.</summary>
    /// <param name="Data">The Data value.</param>
    public sealed record ScatterPoints((string? Name, IList<double> X, IList<double> Y, int Axis) Data);
}
