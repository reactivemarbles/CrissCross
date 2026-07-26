// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Converters;
#else
namespace CrissCross.WPF.UI.Converters;
#endif

/// <summary>Provides the PickerTypeToIntConverter member.</summary>
[ValueConversion(typeof(PickerType), typeof(int))]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class PickerTypeToIntConverter : IValueConverter
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Provides the Convert member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value;

    /// <summary>Provides the ConvertBack member.</summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The targetType value.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture value.</param>
    /// <returns>The result.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        (PickerType)value;
}
