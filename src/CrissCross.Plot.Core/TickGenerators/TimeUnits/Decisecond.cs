// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators.TimeUnits;

/// <summary>Represents a decisecond time unit.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2016",
    Justification = "This implementation must retain DateTime signatures defined by the public ITimeUnit API.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2309",
    Justification = "The default increment is part of the public ITimeUnit API contract.")]
public class Decisecond : ITimeUnit
{
    /// <inheritdoc/>
    public IReadOnlyList<int> Divisors => StandardDivisors.Decimal;

    /// <inheritdoc/>
    public TimeSpan MinSize => TimeSpan.FromMilliseconds(TimeUnitConstants.MillisecondsPerDecisecond);

    /// <inheritdoc/>
    public DateTime Snap(DateTime dateTime)
    {
        int snappedMilliseconds = (dateTime.Millisecond / TimeUnitConstants.MillisecondsPerDecisecond) * TimeUnitConstants.MillisecondsPerDecisecond;
        return new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, snappedMilliseconds, DateTimeKind.Unspecified);
    }

    /// <inheritdoc/>
    public string GetDateTimeFormatString()
    {
#if NETFRAMEWORK
        bool clockHas24Hours = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.IndexOf('H') >= 0;
#else
        bool clockHas24Hours = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Contains('H');
#endif
        string hourSpecifier = clockHas24Hours ? "HH" : "hh";
        return $"{CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern}\n{hourSpecifier}:mm:ss.f";
    }

    /// <inheritdoc/>
    public DateTime Next(DateTime dateTime, int increment = 1) => dateTime.AddMilliseconds(increment * TimeUnitConstants.MillisecondsPerDecisecond);
}
