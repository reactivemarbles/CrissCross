// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

// TODO: Consider creating a `LabelFormatter` type or `ILabelFormatter` because we use this pattern in a lot of places.
/* Maybe something like this?

public interface ILabelFormatter
{
    public string GetLabel<T>(T value);
}
*/

/// <summary>A collection of methods which contain logic for choosing how a value is representing with text.</summary>
public static class LabelFormatters
{
    /// <summary>The threshold above which numbers use the numeric format.</summary>
    private const double LargeNumberThreshold = 1000D;

    /// <summary>The number of decimal places used to round general-format labels.</summary>
    private const int GeneralFormatPrecision = 10;

    /// <summary>Formats a numeric axis value using the current culture.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted numeric label.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Exact comparison with the integer conversion identifies integral values and preserves existing formatting behavior.")]
    public static string Numeric(double value)
    {
        // use system culture
        // if the number is round or large, use the numeric format
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-numeric-n-format-specifier
        bool isRoundNumber = (int)value == value;
        bool isLargeNumber = Math.Abs(value) > LargeNumberThreshold;
        if (isRoundNumber || isLargeNumber)
        {
            return value.ToString("N0");
        }

        // otherwise the number is probably small or very precise to use the general format (with slight rounding)
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-general-g-format-specifier
        string label = Math.Round(value, GeneralFormatPrecision).ToString("G");
        return label == "-0" ? "0" : label;
    }
}
