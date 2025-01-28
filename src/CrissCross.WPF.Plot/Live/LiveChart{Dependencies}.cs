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
        DependencyProperty.Register(nameof(ControlMenu), typeof(ReactiveList<Settings>), typeof(LiveChart));
}
