// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace CrissCross.WPF.Plot;

/// <summary>
/// LiveChart.
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
    public static readonly DependencyProperty DataWithTimeStampProperty =
        DependencyProperty.Register(nameof(DataWithTimeStamp), typeof((string?, IList<double>?, IList<double>, int)), typeof(LiveChart));
}
