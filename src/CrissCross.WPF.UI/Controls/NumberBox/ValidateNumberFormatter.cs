// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the ValidateNumberFormatter member.</summary>
public class ValidateNumberFormatter : INumberFormatter, INumberParser
{
    /// <summary>The general numeric format specifier.</summary>
    private const string FormatSpecifier = "G";

    /// <inheritdoc />
    public string FormatDouble(double? value) =>
        value?.ToString(FormatSpecifier, GetCurrentCultureConverter()) ?? string.Empty;

    /// <inheritdoc />
    public string FormatInt(int? value) =>
        value?.ToString(FormatSpecifier, GetCurrentCultureConverter()) ?? string.Empty;

    /// <inheritdoc />
    public string FormatUInt(uint? value) =>
        value?.ToString(FormatSpecifier, GetCurrentCultureConverter()) ?? string.Empty;

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

    /// <summary>Provides the GetCurrentCultureConverter member.</summary>
    /// <returns>The result.</returns>
    private static CultureInfo GetCurrentCultureConverter() => CultureInfo.CurrentCulture;
}
