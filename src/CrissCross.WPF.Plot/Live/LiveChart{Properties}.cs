// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
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

    /////// <summary>
    /////// Gets or sets the observables with time stamp.
    /////// </summary>
    /////// <value>
    /////// The observables with time stamp.
    /////// </value>
    ////public IEnumerable<IObservable<(string? Name, double? Position)>> AxisLines
    ////{
    ////    get => (IEnumerable<IObservable<(string? Name, IList<double>? X, IList<double> Y, int Axis)>>)GetValue(ScatterObservablesWithTimeStampProperty);
    ////    set
    ////    {
    ////        SetValue(ScatterObservablesWithTimeStampProperty, value);
    ////        ChangeScatterOberver();
    ////    }
    ////}

    /////// <summary>
    /////// Gets or sets the observables with time stamp.
    /////// </summary>
    /////// <value>
    /////// The observables with time stamp.
    /////// </value>
    ////public IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>> StreamerObservablesWithTimeStamp
    ////{
    ////    get => (IEnumerable<IObservable<(string? Name, IList<double>? Value, IList<double> DateTime, int Axis)>>)GetValue(StreamerObservablesWithTimeStampProperty);
    ////    set
    ////    {
    ////        SetValue(StreamerObservablesWithTimeStampProperty, value);
    ////        ChangeStreamertOberver();
    ////    }
    ////}

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
            ChangeScatterOberver();
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
            ChangeSignalOberver();
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
            ChangeDataLoggerOberver();
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
        ////get => (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) GetValue(DataWithTimeStampProperty);
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
    public (string? Name, IList<double>? X, IList<double> Y, int Axis) ScatterWithPoints
    {
        get => ((string? Name, IList<double>? X, IList<double> Y, int Axis))GetValue(ScatterWithPointsProperty);
        ////get => (string? Name, IList<double>? Value, IList<double> DateTime, int Axis) GetValue(DataWithTimeStampProperty);
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
    public ReactiveList<Settings> ControlMenu
    {
        get => (ReactiveList<Settings>)GetValue(ControlMenuProperty);
        set => SetValue(ControlMenuProperty, value);
    }

    /////// <summary>
    /////// Gets or sets the observables with time stamp.
    /////// </summary>
    /////// <value>
    /////// The observables with time stamp.
    /////// </value>
    ////public IEnumerable<(string? AxisName, double UpperLimit, double LowerLimit)> AxisSettings
    ////{
    ////    get => (IEnumerable<(string?, double, double)>)GetValue(AxisSettingsProperty);
    ////    set => SetValue(AxisSettingsProperty, value);
    ////}
}
