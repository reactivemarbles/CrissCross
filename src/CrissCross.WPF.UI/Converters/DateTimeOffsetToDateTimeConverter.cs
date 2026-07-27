// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Converters;
#else
namespace CrissCross.WPF.UI.Converters;
#endif

/// <summary>Converts offset-aware dates for WPF calendar controls.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class DateTimeOffsetToDateTimeConverter : IValueConverter
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            DateTimeOffset date => date.LocalDateTime,
            null => null,
            _ => Binding.DoNothing,
        };

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            DateTime date => new DateTimeOffset(date),
            null => null,
            _ => Binding.DoNothing,
        };
}
