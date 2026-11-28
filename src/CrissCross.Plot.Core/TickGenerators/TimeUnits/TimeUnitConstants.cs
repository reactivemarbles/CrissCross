// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.TimeUnits;

/// <summary>Provides named calendar and subsecond duration values.</summary>
internal static class TimeUnitConstants
{
    /// <summary>The number of milliseconds in a centisecond.</summary>
    internal const int MillisecondsPerCentisecond = 10;

    /// <summary>The number of milliseconds in a decisecond.</summary>
    internal const int MillisecondsPerDecisecond = 100;

    /// <summary>The number of days in the tick generator's approximate year.</summary>
    internal const int ApproximateDaysPerYear = 365;

    /// <summary>The number of days used as a lower-bound estimate for a month.</summary>
    internal const int ApproximateDaysPerMonth = 28;

    /// <summary>The first supported year.</summary>
    internal const int MinimumSupportedYear = 100;

    /// <summary>The last supported year.</summary>
    internal const int MaximumSupportedYear = 9_999;

    /// <summary>The first year beyond the supported range.</summary>
    internal const int FirstUnsupportedYear = 10_000;
}
