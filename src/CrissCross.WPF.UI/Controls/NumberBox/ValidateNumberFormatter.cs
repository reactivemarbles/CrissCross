// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// This Source Code is partially based on the source code provided by the .NET Foundation.

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Base nubmer formatter that uses default format specifier and <see cref="CultureInfo"/> that represents the culture used by the current thread.
/// </summary>
public class ValidateNumberFormatter : INumberFormatter, INumberParser
{
    /// <inheritdoc />
    public string FormatDouble(double? value) => value?.ToString(GetFormatSpecifier(), GetCurrentCultureConverter()) ?? string.Empty;

    /// <inheritdoc />
    public string FormatInt(int? value) => value?.ToString(GetFormatSpecifier(), GetCurrentCultureConverter()) ?? string.Empty;

    /// <inheritdoc />
    public string FormatUInt(uint? value) => value?.ToString(GetFormatSpecifier(), GetCurrentCultureConverter()) ?? string.Empty;

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

    private static string GetFormatSpecifier() => "G";

    private static CultureInfo GetCurrentCultureConverter() => CultureInfo.CurrentCulture;
}
