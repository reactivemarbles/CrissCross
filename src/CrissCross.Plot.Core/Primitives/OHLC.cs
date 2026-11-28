// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents open, high, low, and close values for a time interval.</summary>
public struct OHLC : IEquatable<OHLC>
{
    /// <summary>Initializes a new instance of the <see cref="OHLC"/> struct with daily default timing.</summary>
    /// <param name="open">The opening price.</param>
    /// <param name="high">The highest price.</param>
    /// <param name="low">The lowest price.</param>
    /// <param name="close">The closing price.</param>
    public OHLC(double open, double high, double low, double close)
        : this(open, high, low, close, DateTime.MinValue, TimeSpan.FromDays(1))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OHLC"/> struct with timing information.</summary>
    /// <param name="open">The opening price.</param>
    /// <param name="high">The highest price.</param>
    /// <param name="low">The lowest price.</param>
    /// <param name="close">The closing price.</param>
    /// <param name="start">The start date and time.</param>
    /// <param name="span">The time interval duration.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2016", Justification = "DateTime is retained to preserve the established public API and its caller-defined time semantics.")]
    public OHLC(double open, double high, double low, double close, DateTime start, TimeSpan span)
    {
        if (low > high)
        {
            throw new ArgumentException($"{nameof(high)} must be equal to or greater than {nameof(low)}");
        }

        if (open < low || close < low)
        {
            throw new ArgumentException($"{nameof(low)} must be equal to or less than {nameof(open)} and {nameof(close)}");
        }

        if (high < open || high < close)
        {
            throw new ArgumentException($"{nameof(high)} must be equal to or greater than {nameof(open)} and {nameof(close)}");
        }

        Open = open;
        High = high;
        Low = low;
        Close = close;
        DateTime = start;
        TimeSpan = span;
    }

    /// <summary>Gets or sets the opening price.</summary>
    public double Open { get; set; }

    /// <summary>Gets or sets the highest price.</summary>
    public double High { get; set; }

    /// <summary>Gets or sets the lowest price.</summary>
    public double Low { get; set; }

    /// <summary>Gets or sets the closing price.</summary>
    public double Close { get; set; }

    // TODO: deprecate this eventually. OHLC should be price information only.
    // Perhaps create a Candle primitive that contains time information.
    /// <summary>Gets or sets the candle start date and time.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2016", Justification = "Retain DateTime for API and caller-defined time semantics.")]
    public DateTime DateTime { get; set; }

    /// <summary>Gets or sets the time interval duration.</summary>
    public TimeSpan TimeSpan { get; set; }

    /// <summary>Determines whether two OHLC values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the values are equal.</returns>
    public static bool operator ==(OHLC left, OHLC right) => left.Equals(right);

    /// <summary>Determines whether two OHLC values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true"/> when the values differ.</returns>
    public static bool operator !=(OHLC left, OHLC right) => !left.Equals(right);

    /// <summary>Creates a copy of this OHLC value.</summary>
    /// <returns>A copy with the same prices and timing.</returns>
    public readonly OHLC Clone() => new(Open, High, Low, Close, DateTime, TimeSpan);

    /// <summary>Creates a copy with a different opening price.</summary>
    /// <param name="price">The new opening price.</param>
    /// <returns>The updated value.</returns>
    public readonly OHLC WithOpen(double price) => new(price, High, Low, Close, DateTime, TimeSpan);

    /// <summary>Creates a copy with a different high price.</summary>
    /// <param name="price">The new high price.</param>
    /// <returns>The updated value.</returns>
    public readonly OHLC WithHigh(double price) => new(Open, price, Low, Close, DateTime, TimeSpan);

    /// <summary>Creates a copy with a different low price.</summary>
    /// <param name="price">The new low price.</param>
    /// <returns>The updated value.</returns>
    public readonly OHLC WithLow(double price) => new(Open, High, price, Close, DateTime, TimeSpan);

    /// <summary>Creates a copy with a different closing price.</summary>
    /// <param name="price">The new closing price.</param>
    /// <returns>The updated value.</returns>
    public readonly OHLC WithClose(double price) => new(Open, High, Low, price, DateTime, TimeSpan);

    /// <summary>Creates a copy with a different start date and time.</summary>
    /// <param name="dateTime">The new start date and time.</param>
    /// <returns>The updated value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2016", Justification = "DateTime is retained to preserve the established public API and its caller-defined time semantics.")]
    public readonly OHLC WithDate(DateTime dateTime) => new(Open, High, Low, Close, dateTime, TimeSpan);

    /// <summary>Creates a copy with a different interval duration.</summary>
    /// <param name="timeSpan">The new interval duration.</param>
    /// <returns>The updated value.</returns>
    public readonly OHLC WithTimeSpan(TimeSpan timeSpan) => new(Open, High, Low, Close, DateTime, timeSpan);

    /// <summary>Creates a copy shifted by a duration.</summary>
    /// <param name="timeSpan">The duration to add.</param>
    /// <returns>The shifted value.</returns>
    public readonly OHLC ShiftedBy(TimeSpan timeSpan) => new(Open, High, Low, Close, DateTime + timeSpan, TimeSpan);

    /// <summary>Creates a copy with each price shifted by the specified amount.</summary>
    /// <param name="delta">The price change.</param>
    /// <returns>The shifted value.</returns>
    public readonly OHLC ShiftedBy(double delta) => new(Open + delta, High + delta, Low + delta, Close + delta, DateTime, TimeSpan);

    /// <summary>Determines whether this value equals another OHLC value.</summary>
    /// <param name="other">The value to compare.</param>
    /// <returns><see langword="true"/> when all prices and timing values are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1473", Justification = "Exact value equality is required to remain consistent with the hash code.")]
    public readonly bool Equals(OHLC other) =>
        Open.Equals(other.Open)
        && High.Equals(other.High)
        && Low.Equals(other.Low)
        && Close.Equals(other.Close)
        && DateTime.Equals(other.DateTime)
        && TimeSpan.Equals(other.TimeSpan);

    /// <summary>Determines whether this value equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal OHLC value.</returns>
    public override readonly bool Equals(object? obj) => obj is OHLC other && Equals(other);

    /// <summary>Gets a hash code for this OHLC value.</summary>
    /// <returns>A hash code based on prices and timing.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1482",
        Justification = "Mutable properties and value equality are retained for API compatibility.")]
    public override readonly int GetHashCode() =>
        Open.GetHashCode()
        ^ High.GetHashCode()
        ^ Low.GetHashCode()
        ^ Close.GetHashCode()
        ^ DateTime.GetHashCode()
        ^ TimeSpan.GetHashCode();
}
