// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows.Controls;

namespace CrissCross.WPF.Plot;

/// <summary>
/// LiveChart.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="System.Windows.Markup.IComponentConnector" />
public partial class LiveChart
{
    /// <summary>
    /// Gets or sets the Y Axis Data 1.
    /// </summary>
    [Description("Gets or sets the Observables")]
    [Category("Y Axis")]
    public IEnumerable<IObservable<(string Name, double Value)>> Observables
    {
        get => (IEnumerable<IObservable<(string Name, double Value)>>)GetValue(ObservablesProperty);
        set => SetValue(ObservablesProperty, value);
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
}
