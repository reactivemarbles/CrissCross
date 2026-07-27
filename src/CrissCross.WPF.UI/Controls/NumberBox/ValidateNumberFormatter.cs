// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the ValidateNumberFormatter member.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ValidateNumberFormatter : INumberFormatter, INumberParser
{
    /// <summary>The general numeric format specifier.</summary>
    private const string FormatSpecifier = "G";

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc />
    public string FormatDouble(double? value) => Format(value);

    /// <inheritdoc />
    public string FormatInt(int? value) => FormatDouble(value);

    /// <inheritdoc />
    public string FormatUInt(uint? value) =>
        value is { } number ? Format<uint>(number) : string.Empty;

    /// <inheritdoc />
    public double? ParseDouble(string? value)
    {
        _ = double.TryParse(value, out var d);

        return d;
    }

    /// <inheritdoc />
    public int? ParseInt(string? value)
    {
        _ = int.TryParse(value, out var i);

        return i;
    }

    /// <inheritdoc />
    public uint? ParseUInt(string? value)
    {
        _ = uint.TryParse(value, out var ui);

        return ui;
    }

    /// <summary>Formats a nullable numeric value using the current UI culture.</summary>
    /// <typeparam name="T">The numeric value type.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted value, or an empty string.</returns>
    private static string Format<T>(T? value)
        where T : struct, IFormattable =>
        value?.ToString(FormatSpecifier, GetCurrentCultureConverter()) ?? string.Empty;

    /// <summary>Provides the GetCurrentCultureConverter member.</summary>
    /// <returns>The result.</returns>
    private static CultureInfo GetCurrentCultureConverter() => CultureInfo.CurrentCulture;
}
