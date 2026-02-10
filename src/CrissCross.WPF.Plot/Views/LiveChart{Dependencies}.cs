// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CP.Reactive.Collections;

namespace CrissCross.WPF.Plot;

/// <summary>
/// Represents a live-updating chart control that displays real-time data streams with customizable axes, legends, and
/// visualization options.
/// </summary>
/// <remarks>LiveChart is designed for scenarios where data changes frequently and needs to be visualized in real
/// time, such as monitoring signals, logging data, or displaying observables. The control exposes a variety of
/// dependency properties to configure chart appearance, data sources, and behavior, including support for multiple data
/// series, axis labeling, and legend positioning. It is intended for use in WPF applications and supports binding to
/// reactive data sources for dynamic updates.</remarks>
public partial class LiveChart
{
    /// <summary>
    /// Identifies the UseFixedNumberOfPoints dependency property, which determines whether the chart uses a fixed
    /// number of data points for rendering.
    /// </summary>
    /// <remarks>This field is typically used when interacting with the WPF property system, such as when
    /// calling methods like SetValue or GetValue. The default value is <see langword="false"/>.</remarks>
    public static readonly DependencyProperty UseFixedNumberOfPointsProperty =
        DependencyProperty.Register(nameof(UseFixedNumberOfPoints), typeof(bool), typeof(LiveChart), new PropertyMetadata(false, new(UseFixedNumberOfPointsCallback)));

    /// <summary>
    /// Identifies the NumberPointsPlotted dependency property, which specifies the maximum number of data points to
    /// display in the chart.
    /// </summary>
    /// <remarks>This field is used when referencing the NumberPointsPlotted property in code, such as for
    /// data binding or property metadata operations. The default value is 600. Changing this property affects how many
    /// points are rendered in the chart at any given time.</remarks>
    public static readonly DependencyProperty NumberPointsPlottedProperty =
        DependencyProperty.Register(nameof(NumberPointsPlotted), typeof(int), typeof(LiveChart), new PropertyMetadata(600, new(NumberPointsPlottedCallback)));

    /// <summary>
    /// Identifies the TitleContent dependency property, which represents the title text displayed by the LiveChart
    /// control.
    /// </summary>
    /// <remarks>This field is typically used when interacting with the LiveChart control's title in XAML or
    /// when calling methods such as SetValue or GetValue. The default value is a single space character.</remarks>
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(TitleContent), typeof(string), typeof(LiveChart), new PropertyMetadata(" ", new(TitleCallback)));

    /// <summary>
    /// Identifies the LegendPosition dependency property, which determines the position of the legend in a LiveChart
    /// control.
    /// </summary>
    /// <remarks>This field is used when referencing the LegendPosition property in property system
    /// operations, such as data binding or styling. The default value is LegendPosition.Top.</remarks>
    public static readonly DependencyProperty LegendPositionProperty =
        DependencyProperty.Register(nameof(LegendPosition), typeof(LegendPosition), typeof(LiveChart), new PropertyMetadata(LegendPosition.Top, new(LegendPositionCallback)));

    /// <summary>
    /// Identifies the RightWidth dependency property, which specifies the width of the right panel in the LiveChart
    /// control.
    /// </summary>
    /// <remarks>This property can be used in XAML or code to set or retrieve the width of the right panel.
    /// Changes to this property will trigger layout updates for the control. The default value is a GridLength of
    /// 0.</remarks>
    public static readonly DependencyProperty RightWidthProperty =
        DependencyProperty.Register(
            nameof(RightWidth),
            typeof(GridLength),
            typeof(LiveChart),
            new PropertyMetadata(
            new GridLength(0),
            new PropertyChangedCallback(RightWidthCallback)));

    /// <summary>
    /// Identifies the LegendWidth dependency property.
    /// </summary>
    /// <remarks>This field is used to reference the LegendWidth property in property system operations, such
    /// as data binding or styling within XAML. It is typically used when calling methods that require a
    /// DependencyProperty identifier.</remarks>
    public static readonly DependencyProperty LegendWidthProperty =
        DependencyProperty.Register(
            nameof(LegendWidth),
            typeof(double),
            typeof(LiveChart),
            new(new PropertyChangedCallback(LegendWidthCallback)));

    /// <summary>
    /// Identifies the NSamples dependency property, which specifies the number of samples to display in the LiveChart
    /// control.
    /// </summary>
    /// <remarks>This field is used when referencing the NSamples property in property system operations, such
    /// as data binding or property metadata configuration. The default value is 2024.</remarks>
    public static readonly DependencyProperty NSamplesProperty =
        DependencyProperty.Register(nameof(NSamples), typeof(int), typeof(LiveChart), new PropertyMetadata(2024, new(NSamplesCallback)));

    /// <summary>
    /// Identifies the Frequency dependency property, which specifies the data sampling frequency for the LiveChart
    /// control.
    /// </summary>
    /// <remarks>This field is used when referencing the Frequency property in property system operations,
    /// such as data binding or property metadata overrides. The default value is 32,000. Changing the Frequency
    /// property affects how often the LiveChart control samples or updates its data.</remarks>
    public static readonly DependencyProperty FrequencyProperty =
        DependencyProperty.Register(nameof(Frequency), typeof(int), typeof(LiveChart), new PropertyMetadata(32000, new(FrequencyCallback)));

    /// <summary>
    /// Identifies the Observables dependency property, which enables data binding for a collection of observable data
    /// series in a LiveChart control.
    /// </summary>
    /// <remarks>This dependency property allows the LiveChart to bind to an enumerable collection of
    /// IObservable tuples, where each tuple represents a data series with a string key and a double value. Use this
    /// property to provide dynamic, real-time data updates to the chart through data binding in XAML or code.</remarks>
    public static readonly DependencyProperty ObservablesProperty =
        DependencyProperty.Register(nameof(Observables), typeof(IEnumerable<IObservable<(string, double)>>), typeof(LiveChart));

    /// <summary>
    /// Identifies the SignalObservablesWithTimeStamp dependency property, which enables binding to a collection of
    /// observables that provide time-stamped signal data for the chart.
    /// </summary>
    /// <remarks>Each observable in the collection emits a tuple containing an optional signal name, optional
    /// X values, Y values, and a time stamp index. This property is typically used to display real-time or historical
    /// signal data in the chart. The property is intended for use with data binding in XAML or code-behind.</remarks>
    public static readonly DependencyProperty SignalObservablesWithTimeStampProperty =
        DependencyProperty.Register(nameof(SignalObservablesWithTimeStamp), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// Identifies the DataLoggerObservablesWithPoints dependency property, which enables binding a collection of
    /// observables containing data point information to the LiveChart control.
    /// </summary>
    /// <remarks>Each observable in the collection provides a tuple containing a series name, a list of data
    /// point values, and two integer values representing additional metadata. This property is typically used to supply
    /// dynamic or real-time data to the chart through data binding.</remarks>
    public static readonly DependencyProperty DataLoggerObservablesWithPointsProperty =
        DependencyProperty.Register(nameof(DataLoggerObservablesWithPoints), typeof(IEnumerable<IObservable<(string?, IList<double>?, int, int)>>), typeof(LiveChart));

    /// <summary>
    /// Identifies the DataWithTimeStamp dependency property, which stores a tuple containing optional string data,
    /// optional and required lists of double values, and an integer value for the LiveChart control.
    /// </summary>
    /// <remarks>This property enables data binding, styling, and animation for the associated
    /// DataWithTimeStamp property in LiveChart. The tuple consists of four elements: a nullable string, a nullable list
    /// of doubles, a non-nullable list of doubles, and an integer. The property is typically used to represent chart
    /// data along with associated timestamps and metadata.</remarks>
    public static readonly DependencyProperty DataWithTimeStampProperty =
        DependencyProperty.Register(nameof(DataWithTimeStamp), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// Identifies the SignalWithPoints dependency property, which stores a tuple containing a signal name, optional X
    /// and Y coordinate lists, and a point count for use in LiveChart visualizations.
    /// </summary>
    /// <remarks>This property enables data binding and styling for chart signals that include both metadata
    /// and point data. The tuple consists of a signal name (string or null), an optional list of X coordinates, a
    /// required list of Y coordinates, and an integer representing the number of points. The property is intended for
    /// scenarios where both the signal's identity and its associated data points need to be provided to the chart
    /// control.</remarks>
    public static readonly DependencyProperty SignalWithPointsProperty =
        DependencyProperty.Register(nameof(SignalWithPoints), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// Identifies the SignalsWithPoints dependency property, which represents a collection of signal data points to be
    /// displayed in the chart.
    /// </summary>
    /// <remarks>Each item in the collection is a tuple containing a signal name, optional X values, Y values,
    /// and a color index. This property enables data binding for multiple signals with associated points in a LiveChart
    /// control.</remarks>
    public static readonly DependencyProperty SignalsWithPointsProperty =
        DependencyProperty.Register(nameof(SignalsWithPoints), typeof(IEnumerable<(string?, IList<double>?, IList<double>, int)>), typeof(LiveChart));

    /// <summary>
    /// Identifies the SignalObservablesWithPoints dependency property, which enables data binding for a collection of
    /// observable data series with associated point values.
    /// </summary>
    /// <remarks>This property is intended for scenarios where the chart should react to changes in multiple
    /// observable data sources, each providing a series name, optional X and Y values, and a count. The property can be
    /// used to bind dynamic data streams to the chart for real-time updates. The expected value is an enumerable of
    /// observables, where each observable emits a tuple containing a series name, optional X values, Y values, and a
    /// point count.</remarks>
    public static readonly DependencyProperty SignalObservablesWithPointsProperty =
        DependencyProperty.Register(nameof(SignalObservablesWithPoints), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// Identifies the ScatterObservablesWithTimeStamp dependency property, which enables data binding for a collection
    /// of observable scatter data points with associated timestamps.
    /// </summary>
    /// <remarks>This property is intended for use in scenarios where scatter plot data is provided as a
    /// sequence of observables, each emitting tuples containing an optional series name, optional X values, required Y
    /// values, and a timestamp. The property supports dynamic updates to the chart as new data arrives. The expected
    /// value is an enumerable of IObservable sequences, each producing a tuple of (string?, IList{double}?,
    /// IList{double}, int).</remarks>
    public static readonly DependencyProperty ScatterObservablesWithTimeStampProperty =
        DependencyProperty.Register(nameof(ScatterObservablesWithTimeStamp), typeof(IEnumerable<IObservable<(string?, IList<double>?, IList<double>, int)>>), typeof(LiveChart));

    /// <summary>
    /// Identifies the ScatterWithPoints dependency property, which stores the configuration for a scatter plot with
    /// associated data points.
    /// </summary>
    /// <remarks>The value of this property is a tuple containing an optional series name, two lists of double
    /// values representing the X and Y coordinates of the points, and an integer specifying the series index. This
    /// property is typically used to bind scatter plot data to a LiveChart control in XAML or code-behind.</remarks>
    public static readonly DependencyProperty ScatterWithPointsProperty =
        DependencyProperty.Register(nameof(ScatterWithPoints), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));

    /// <summary>
    /// Identifies the YAxisName dependency property, which stores the names of the Y axes for the chart.
    /// </summary>
    /// <remarks>This dependency property is used to associate a pair of string lists representing Y axis
    /// names with a LiveChart instance. It enables data binding and styling for Y axis labels in XAML-based
    /// applications.</remarks>
    public static readonly DependencyProperty YAxisNameProperty =
        DependencyProperty.Register(nameof(YAxisName), typeof((IList<string>, IList<string>)), typeof(LiveChart));

    /// <summary>
    /// Identifies the ControlMenu dependency property, which represents a collection of chart objects associated with
    /// the control.
    /// </summary>
    /// <remarks>This field is typically used when interacting with the property system, such as for data
    /// binding, styling, or animation in XAML. To get or set the value of the ControlMenu property, use the
    /// corresponding CLR property on the LiveChart class.</remarks>
    #if NET8_0_OR_GREATER
    public static readonly DependencyProperty ControlMenuProperty =
        DependencyProperty.Register(nameof(ControlMenu), typeof(QuaternaryList<ChartObjects>), typeof(LiveChart));
#else
    public static readonly DependencyProperty ControlMenuProperty =
        DependencyProperty.Register(nameof(ControlMenu), typeof(ReactiveList<ChartObjects>), typeof(LiveChart));
#endif

    /// <summary>
    /// Handles changes to the right width property of a LiveChart control.
    /// </summary>
    /// <remarks>This callback is typically used as a property changed handler for a DependencyProperty
    /// representing the right width of a LiveChart. If the provided dependency object is not a LiveChart or the new
    /// value is not a GridLength, the callback does nothing.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Expected to be a LiveChart instance.</param>
    /// <param name="e">The event data containing information about the property change, including the new value.</param>
    private static void RightWidthCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is GridLength gridLength)
        {
            livechart.RightWidth = gridLength;
        }
    }

    /// <summary>
    /// Handles changes to the legend width property by updating the width of the right legend in a LiveChart control.
    /// </summary>
    /// <remarks>This callback is typically used as a property changed handler for a dependency property
    /// representing the legend width. The method only updates the legend width if the dependency object is a LiveChart
    /// and the new value is a double.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Must be a LiveChart instance for the callback to
    /// have an effect.</param>
    /// <param name="e">The event data containing information about the property change, including the new value to apply as the legend
    /// width.</param>
    private static void LegendWidthCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is double gridLength)
        {
            livechart.RightLegend.Width = gridLength;
        }
    }

    /// <summary>
    /// Handles changes to the NSamples dependency property by updating the corresponding value on the associated
    /// LiveChart instance.
    /// </summary>
    /// <remarks>This callback is typically used in property metadata to synchronize the NSamples property
    /// value between the dependency property system and the LiveChart control. If the dependency object is not a
    /// LiveChart or the new value is not an integer, no action is taken.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Must be a LiveChart instance to apply the update.</param>
    /// <param name="e">The event data containing information about the property change, including the new value for NSamples.</param>
    private static void NSamplesCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is int nSamples)
        {
            livechart.NSamples = nSamples;
        }
    }

    /// <summary>
    /// Handles changes to the frequency dependency property and updates the associated LiveChart instance.
    /// </summary>
    /// <remarks>This callback is typically used in property metadata to synchronize the LiveChart's Frequency
    /// property with the value of the dependency property. If the dependency object is not a LiveChart or the new value
    /// is not an integer, no action is taken.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Expected to be a LiveChart instance.</param>
    /// <param name="e">The event data containing information about the property change, including the new value.</param>
    private static void FrequencyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is int fs)
        {
            livechart.Frequency = fs;
        }
    }

    /// <summary>
    /// Handles changes to the NumberPointsPlotted dependency property by updating the corresponding values on the
    /// LiveChart instance and its view model.
    /// </summary>
    /// <remarks>This callback is typically used in WPF to synchronize the NumberPointsPlotted property
    /// between the LiveChart control and its associated view model when the dependency property value
    /// changes.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Must be a LiveChart instance for the callback to
    /// have an effect.</param>
    /// <param name="e">The event data containing information about the property change, including the new value to be applied.</param>
    private static void NumberPointsPlottedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is int numberOfSamples)
        {
            livechart.NumberPointsPlotted = numberOfSamples;
            livechart.ViewModel?.NumberPointsPlotted = numberOfSamples;
        }
    }

    /// <summary>
    /// Handles changes to the Title property by updating the associated LiveChart's ViewModel title.
    /// </summary>
    /// <remarks>This callback is typically used as a property changed handler for the Title dependency
    /// property on LiveChart controls. If the dependency object is not a LiveChart or the new value is not a string, no
    /// action is taken.</remarks>
    /// <param name="d">The dependency object whose Title property has changed. Must be a LiveChart instance.</param>
    /// <param name="e">The event data containing information about the property change, including the new value.</param>
    private static void TitleCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is string title)
        {
            livechart.ViewModel?.Title = title;
        }
    }

    /// <summary>
    /// Handles changes to the legend position dependency property for a LiveChart control.
    /// </summary>
    /// <remarks>This callback updates the legend position in the associated LiveChart's view model when the
    /// dependency property value changes.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Expected to be a LiveChart instance.</param>
    /// <param name="e">The event data containing information about the property change, including the new value for the legend
    /// position.</param>
    private static void LegendPositionCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is LegendPosition position)
        {
            livechart.ViewModel?.LegendPosition = position;
        }
    }

    /// <summary>
    /// Handles changes to the UseFixedNumberOfPoints dependency property by updating the corresponding property on the
    /// LiveChart and its ViewModel.
    /// </summary>
    /// <remarks>This callback is typically used in property metadata to synchronize the
    /// UseFixedNumberOfPoints setting between the LiveChart control and its associated ViewModel. The method expects
    /// the new value to be of type Boolean.</remarks>
    /// <param name="d">The dependency object on which the property change occurred. Must be a LiveChart instance.</param>
    /// <param name="e">The event data containing information about the property change, including the new value.</param>
    private static void UseFixedNumberOfPointsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LiveChart livechart && e.NewValue is bool fixedNumberOfSamples)
        {
            livechart.UseFixedNumberOfPoints = fixedNumberOfSamples;
            livechart.ViewModel?.UseFixedNumberOfPoints = fixedNumberOfSamples;
        }
    }
}
