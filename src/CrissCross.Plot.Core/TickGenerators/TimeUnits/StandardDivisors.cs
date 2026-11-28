// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.TimeUnits;

/// <summary>Provides standard divisors used for choosing time-unit intervals.</summary>
internal static class StandardDivisors
{
    /// <summary>Decimal subdivisions.</summary>
    public static readonly IReadOnlyList<int> Decimal = [1, 5, 10];

    /// <summary>Sexagesimal subdivisions.</summary>
    public static readonly IReadOnlyList<int> Sexagesimal = [1, 5, 10, 15, 20, 30, 60];

    /// <summary>Dozenal subdivisions.</summary>
    public static readonly IReadOnlyList<int> Dozenal = [1, 2, 3, 4, 6, 12];

    /// <summary>Hexadecimal subdivisions.</summary>
    public static readonly IReadOnlyList<int> Hexadecimal = [1, 2, 3, 4, 6, 8, 16];

    /// <summary>Day subdivisions.</summary>
    public static readonly IReadOnlyList<int> Days = [1, 3, 7, 14, 28];

    /// <summary>Month subdivisions.</summary>
    public static readonly IReadOnlyList<int> Months = [1, 3, 6];

    /// <summary>Year subdivisions.</summary>
    public static readonly IReadOnlyList<int> Years = [1, 2, 3, 4, 5, 10];
}
