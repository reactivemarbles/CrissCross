// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides allocation-conscious technical studies over normalized plot series.</summary>
public static class TechnicalIndicators
{
    /// <summary>Provides the conventional RSI period.</summary>
    private const int DefaultRelativeStrengthIndexPeriod = 14;

    /// <summary>Provides the conventional fast MACD period.</summary>
    private const int DefaultMacdFastPeriod = 12;

    /// <summary>Provides the conventional slow MACD period.</summary>
    private const int DefaultMacdSlowPeriod = 26;

    /// <summary>Provides the conventional MACD signal period.</summary>
    private const int DefaultMacdSignalPeriod = 9;

    /// <summary>Provides the conventional Bollinger Band period.</summary>
    private const int DefaultBollingerBandPeriod = 20;

    /// <summary>Provides the conventional Bollinger Band deviation multiplier.</summary>
    private const double DefaultBollingerBandDeviations = 2;

    /// <summary>Provides the divisor used for averages of two values.</summary>
    private const double PairDivisor = 2;

    /// <summary>Provides the maximum RSI value.</summary>
    private const double MaximumRelativeStrengthIndex = 100;

    /// <summary>Calculates a simple moving average with an automatically derived key.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The rolling period.</param>
    /// <returns>The moving-average series.</returns>
    public static PlotSeriesData SimpleMovingAverage(PlotSeriesData source, int period) =>
        SimpleMovingAverage(source, period, null);

    /// <summary>Calculates a simple moving average.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The rolling period.</param>
    /// <param name="key">An optional output key.</param>
    /// <returns>The moving-average series.</returns>
    public static PlotSeriesData SimpleMovingAverage(PlotSeriesData source, int period, PlotSeriesKey? key)
    {
        Validate(source, period);
        var output = CreateMissingValues(source.Y.Count);
        var sum = 0D;
        var invalid = 0;
        for (var i = 0; i < source.Y.Count; i++)
        {
            AddRollingValue(source.Y[i], ref sum, ref invalid);
            if (i >= period)
            {
                RemoveRollingValue(source.Y[i - period], ref sum, ref invalid);
            }

            if (i >= period - 1 && invalid == 0)
            {
                output[i] = sum / period;
            }
        }

        return source.Derive(key ?? Suffix(source.Key, $"SMA({period})"), output);
    }

    /// <summary>Calculates an exponential moving average with an automatically derived key.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The smoothing period.</param>
    /// <returns>The exponential moving-average series.</returns>
    public static PlotSeriesData ExponentialMovingAverage(PlotSeriesData source, int period) =>
        ExponentialMovingAverage(source, period, null);

    /// <summary>Calculates an exponential moving average.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The smoothing period.</param>
    /// <param name="key">An optional output key.</param>
    /// <returns>The exponential moving-average series.</returns>
    public static PlotSeriesData ExponentialMovingAverage(PlotSeriesData source, int period, PlotSeriesKey? key)
    {
        Validate(source, period);
        var output = CreateMissingValues(source.Y.Count);
        var multiplier = PairDivisor / (period + 1D);
        var hasValue = false;
        var previous = 0D;
        for (var i = 0; i < source.Y.Count; i++)
        {
            var value = source.Y[i];
            if (!IsFinite(value))
            {
                hasValue = false;
                continue;
            }

            previous = hasValue ? ((value - previous) * multiplier) + previous : value;
            hasValue = true;
            output[i] = previous;
        }

        return source.Derive(key ?? Suffix(source.Key, $"EMA({period})"), output);
    }

    /// <summary>Calculates RSI using its conventional period and an automatically derived key.</summary>
    /// <param name="source">The source series.</param>
    /// <returns>The zero-to-one-hundred RSI series.</returns>
    public static PlotSeriesData RelativeStrengthIndex(PlotSeriesData source) =>
        RelativeStrengthIndex(source, DefaultRelativeStrengthIndexPeriod, null);

    /// <summary>Calculates RSI with an automatically derived key.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The RSI period.</param>
    /// <returns>The zero-to-one-hundred RSI series.</returns>
    public static PlotSeriesData RelativeStrengthIndex(PlotSeriesData source, int period) =>
        RelativeStrengthIndex(source, period, null);

    /// <summary>Calculates the Relative Strength Index using Wilder smoothing.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The RSI period.</param>
    /// <param name="key">An optional output key.</param>
    /// <returns>The zero-to-one-hundred RSI series.</returns>
    public static PlotSeriesData RelativeStrengthIndex(PlotSeriesData source, int period, PlotSeriesKey? key)
    {
        Validate(source, period);
        var output = CreateMissingValues(source.Y.Count);
        if (source.Y.Count <= period)
        {
            return source.Derive(key ?? Suffix(source.Key, $"RSI({period})"), output);
        }

        var gains = 0D;
        var losses = 0D;
        for (var i = 1; i <= period; i++)
        {
            var change = source.Y[i] - source.Y[i - 1];
            gains += Math.Max(0, change);
            losses += Math.Max(0, -change);
        }

        var averageGain = gains / period;
        var averageLoss = losses / period;
        output[period] = CalculateRsi(averageGain, averageLoss);
        for (var i = period + 1; i < source.Y.Count; i++)
        {
            var change = source.Y[i] - source.Y[i - 1];
            averageGain = ((averageGain * (period - 1)) + Math.Max(0, change)) / period;
            averageLoss = ((averageLoss * (period - 1)) + Math.Max(0, -change)) / period;
            output[i] = CalculateRsi(averageGain, averageLoss);
        }

        return source.Derive(key ?? Suffix(source.Key, $"RSI({period})"), output);
    }

    /// <summary>Calculates MACD using its conventional periods.</summary>
    /// <param name="source">The source series.</param>
    /// <returns>The three MACD output series.</returns>
    public static MacdResult MovingAverageConvergenceDivergence(PlotSeriesData source) =>
        MovingAverageConvergenceDivergence(
            source,
            DefaultMacdFastPeriod,
            DefaultMacdSlowPeriod,
            DefaultMacdSignalPeriod);

    /// <summary>Calculates Moving Average Convergence Divergence, its signal line, and histogram.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="fastPeriod">The fast EMA period.</param>
    /// <param name="slowPeriod">The slow EMA period.</param>
    /// <param name="signalPeriod">The signal EMA period.</param>
    /// <returns>The three MACD output series.</returns>
    public static MacdResult MovingAverageConvergenceDivergence(
        PlotSeriesData source,
        int fastPeriod,
        int slowPeriod,
        int signalPeriod)
    {
        Validate(source, fastPeriod);
        Validate(source, slowPeriod);
        Validate(source, signalPeriod);
        if (fastPeriod >= slowPeriod)
        {
            throw new ArgumentException("The MACD fast period must be less than the slow period.", nameof(fastPeriod));
        }

        var fast = ExponentialMovingAverage(source, fastPeriod);
        var slow = ExponentialMovingAverage(source, slowPeriod);
        var macdValues = new double[source.Y.Count];
        for (var i = 0; i < macdValues.Length; i++)
        {
            macdValues[i] = fast.Y[i] - slow.Y[i];
        }

        var macd = source.Derive(Suffix(source.Key, "MACD"), macdValues);
        var signal = ExponentialMovingAverage(macd, signalPeriod, Suffix(source.Key, "MACD Signal"));
        var histogram = new double[source.Y.Count];
        for (var i = 0; i < histogram.Length; i++)
        {
            histogram[i] = macd.Y[i] - signal.Y[i];
        }

        return new(macd, signal, source.Derive(Suffix(source.Key, "MACD Histogram"), histogram));
    }

    /// <summary>Calculates Bollinger Bands using the conventional period and deviation multiplier.</summary>
    /// <param name="source">The source series.</param>
    /// <returns>The middle, upper, and lower bands.</returns>
    public static BollingerBandsResult BollingerBands(PlotSeriesData source) =>
        BollingerBands(source, DefaultBollingerBandPeriod, DefaultBollingerBandDeviations);

    /// <summary>Calculates Bollinger Bands using the conventional deviation multiplier.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The rolling period.</param>
    /// <returns>The middle, upper, and lower bands.</returns>
    public static BollingerBandsResult BollingerBands(PlotSeriesData source, int period) =>
        BollingerBands(source, period, DefaultBollingerBandDeviations);

    /// <summary>Calculates Bollinger Bands around a simple moving average.</summary>
    /// <param name="source">The source series.</param>
    /// <param name="period">The rolling period.</param>
    /// <param name="standardDeviations">The standard-deviation multiplier.</param>
    /// <returns>The middle, upper, and lower bands.</returns>
    public static BollingerBandsResult BollingerBands(PlotSeriesData source, int period, double standardDeviations)
    {
        Validate(source, period);
        if (!IsFinite(standardDeviations) || standardDeviations <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(standardDeviations),
                standardDeviations,
                "The standard-deviation multiplier must be finite and positive.");
        }

        var middle = SimpleMovingAverage(source, period, Suffix(source.Key, "Bollinger Middle"));
        var upper = CreateMissingValues(source.Y.Count);
        var lower = CreateMissingValues(source.Y.Count);
        var sum = 0D;
        var sumSquares = 0D;
        for (var i = 0; i < source.Y.Count; i++)
        {
            var value = source.Y[i];
            sum += value;
            sumSquares += value * value;
            if (i >= period)
            {
                var removed = source.Y[i - period];
                sum -= removed;
                sumSquares -= removed * removed;
            }

            if (i < period - 1)
            {
                continue;
            }

            var mean = sum / period;
            var variance = Math.Max(0, (sumSquares / period) - (mean * mean));
            var offset = Math.Sqrt(variance) * standardDeviations;
            upper[i] = mean + offset;
            lower[i] = mean - offset;
        }

        return new(
            middle,
            source.Derive(Suffix(source.Key, "Bollinger Upper"), upper),
            source.Derive(Suffix(source.Key, "Bollinger Lower"), lower));
    }

    /// <summary>Calculates the standard 9/26/52 Ichimoku study with 26-period displacement.</summary>
    /// <param name="source">The source close-value series.</param>
    /// <returns>The five Ichimoku output series.</returns>
    public static IchimokuResult Ichimoku(PlotSeriesData source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        const int conversionPeriod = 9;
        const int basePeriod = 26;
        const int spanPeriod = 52;
        const int displacement = 26;
        var conversion = RollingMidpoint(source.Y, conversionPeriod);
        var baseLine = RollingMidpoint(source.Y, basePeriod);
        var spanBValues = RollingMidpoint(source.Y, spanPeriod);
        var leadingA = CreateMissingValues(source.Y.Count);
        var leadingB = CreateMissingValues(source.Y.Count);
        var lagging = CreateMissingValues(source.Y.Count);
        for (var i = 0; i < source.Y.Count; i++)
        {
            var forwardIndex = i + displacement;
            if (forwardIndex < source.Y.Count && IsFinite(conversion[i]) && IsFinite(baseLine[i]))
            {
                leadingA[forwardIndex] = (conversion[i] + baseLine[i]) / PairDivisor;
            }

            if (forwardIndex < source.Y.Count && IsFinite(spanBValues[i]))
            {
                leadingB[forwardIndex] = spanBValues[i];
            }

            var backwardIndex = i - displacement;
            if (backwardIndex >= 0)
            {
                lagging[backwardIndex] = source.Y[i];
            }
        }

        return new(
            source.Derive(Suffix(source.Key, "Ichimoku Conversion"), conversion),
            source.Derive(Suffix(source.Key, "Ichimoku Base"), baseLine),
            source.Derive(Suffix(source.Key, "Ichimoku Span A"), leadingA),
            source.Derive(Suffix(source.Key, "Ichimoku Span B"), leadingB),
            source.Derive(Suffix(source.Key, "Ichimoku Lagging"), lagging));
    }

    /// <summary>Validates a study input.</summary>
    /// <param name="source">The input series.</param>
    /// <param name="period">The requested period.</param>
    private static void Validate(PlotSeriesData source, int period)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (period > 0)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(period), period, "Indicator periods must be positive.");
    }

    /// <summary>Creates a result key from a source key and suffix.</summary>
    /// <param name="key">The source key.</param>
    /// <param name="suffix">The display suffix.</param>
    /// <returns>The result key.</returns>
    private static PlotSeriesKey Suffix(PlotSeriesKey key, string suffix) => new($"{key.Name} {suffix}", key.Axis);

    /// <summary>Creates an array initialized with missing values.</summary>
    /// <param name="count">The array size.</param>
    /// <returns>The initialized array.</returns>
    private static double[] CreateMissingValues(int count)
    {
        var values = new double[count];
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = double.NaN;
        }

        return values;
    }

    /// <summary>Adds a value to rolling finite-value state.</summary>
    /// <param name="value">The value to add.</param>
    /// <param name="sum">The rolling sum.</param>
    /// <param name="invalid">The rolling invalid-value count.</param>
    private static void AddRollingValue(double value, ref double sum, ref int invalid)
    {
        if (IsFinite(value))
        {
            sum += value;
            return;
        }

        invalid++;
    }

    /// <summary>Removes a value from rolling finite-value state.</summary>
    /// <param name="value">The value to remove.</param>
    /// <param name="sum">The rolling sum.</param>
    /// <param name="invalid">The rolling invalid-value count.</param>
    private static void RemoveRollingValue(double value, ref double sum, ref int invalid)
    {
        if (IsFinite(value))
        {
            sum -= value;
            return;
        }

        invalid--;
    }

    /// <summary>Converts average gains and losses to an RSI value.</summary>
    /// <param name="averageGain">The smoothed average gain.</param>
    /// <param name="averageLoss">The smoothed average loss.</param>
    /// <returns>The RSI value.</returns>
    private static double CalculateRsi(double averageGain, double averageLoss) =>
        averageLoss == 0
            ? MaximumRelativeStrengthIndex
            : MaximumRelativeStrengthIndex - (MaximumRelativeStrengthIndex / (1 + (averageGain / averageLoss)));

    /// <summary>Determines whether a value is neither NaN nor infinite.</summary>
    /// <param name="value">The value to inspect.</param>
    /// <returns><see langword="true"/> when the value is finite; otherwise, <see langword="false"/>.</returns>
    private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    /// <summary>Calculates rolling midpoint values over a close-value series.</summary>
    /// <param name="values">The input values.</param>
    /// <param name="period">The rolling period.</param>
    /// <returns>The rolling midpoint values.</returns>
    private static double[] RollingMidpoint(IReadOnlyList<double> values, int period)
    {
        var output = CreateMissingValues(values.Count);
        for (var i = period - 1; i < values.Count; i++)
        {
            var minimum = double.PositiveInfinity;
            var maximum = double.NegativeInfinity;
            for (var j = i - period + 1; j <= i; j++)
            {
                minimum = Math.Min(minimum, values[j]);
                maximum = Math.Max(maximum, values[j]);
            }

            output[i] = (minimum + maximum) / PairDivisor;
        }

        return output;
    }
}
