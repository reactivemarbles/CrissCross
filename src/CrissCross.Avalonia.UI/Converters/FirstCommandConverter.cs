// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Input;
using Avalonia.Data.Converters;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Converters;
#else
namespace CrissCross.Avalonia.UI.Converters;
#endif

/// <summary>Selects the first command supplied by a control or its model.</summary>
public sealed class FirstCommandConverter : IMultiValueConverter
{
    /// <summary>Gets the shared converter instance.</summary>
    public static FirstCommandConverter Instance { get; } = new();

    /// <inheritdoc />
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);
        foreach (var value in values)
        {
            if (value is ICommand command)
            {
                return command;
            }
        }

        return null;
    }
}
