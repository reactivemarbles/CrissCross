// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Data;

namespace CrissCross.WPF.UI.Converters;

/// <summary>Converts offset-aware dates for WPF calendar controls.</summary>
public sealed class DateTimeOffsetToDateTimeConverter : IValueConverter
{
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
