// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.DataGenerators;

namespace CrissCross.Plot;

#nullable enable

/// <summary>Generates sample data for plotting and testing.</summary>
public static class Generate
{
    /// <summary>The shared random-walk generator.</summary>
    public static readonly RandomWalker RandomWalker;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const double TwoPiFactor = 2;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const double SigmoidScale = 10;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const double SigmoidMidpoint = 5;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int HarmonicStep = 2;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int AlphabetLength = 26;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int RandomScaleUpperBound = 5;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int HalvingDivisor = 2;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int QuarterHourMinutes = 15;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int BoxDistributionMaximum = 3;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int QuartileDivisor = 4;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const int UpperQuartileMultiplier = 3;

    /// <summary>Constant used to preserve a generated sample value.</summary>
    private const double NonZeroOffsetRatio = .001;

    /// <summary>Initializes the shared sample data generators in their original order.</summary>
    static Generate()
    {
        RandomData = new(0);
        Financial = new(0);
        RandomWalker = new(0);
    }

    /// <summary>Gets the shared random data generator used by this class.</summary>
    public static RandomDataGenerator RandomData { get; }

    /// <summary>Gets the shared financial data generator used by this class.</summary>
    public static FinancialDataGenerator Financial { get; }

    /// <summary>Returns evenly spaced values beginning at the supplied start value.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="delta">Spacing added between consecutive values.</param>
    /// <param name="first">First value in the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] Consecutive(int count = 51, double delta = 1, double first = 0)
    {
        double[] ys = new double[count];
        for (int i = 0; i < ys.Length; i++)
        {
            ys[i] = (i * delta) + first;
        }

        return ys;
    }

    /// <summary>Returns evenly spaced values beginning at the supplied start value.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="timeSpan">Interval added between consecutive date/time values.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] Consecutive(int count, System.DateTime start, TimeSpan timeSpan)
    {
        System.DateTime[] dates = new System.DateTime[count];

        for (int i = 0; i < count; i++)
        {
            dates[i] = start;
            start += timeSpan;
        }

        return dates;
    }

    /// <summary>Returns evenly spaced values beginning at the supplied start value.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="timeSpan">Interval added between consecutive date/time values.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] Consecutive(int count, DateTimeOffset start, TimeSpan timeSpan)
    {
        DateTimeOffset[] dates = new DateTimeOffset[count];

        for (int i = 0; i < count; i++)
        {
            dates[i] = start;
            start += timeSpan;
        }

        return dates;
    }

    /// <summary>Returns consecutive date/time values separated by the supplied interval.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="timeSpan">Interval added between consecutive date/time values.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveDateTimes(int count, System.DateTime start, TimeSpan timeSpan) => Consecutive(count, start, timeSpan);

    /// <summary>Returns consecutive date/time values separated by the supplied interval.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="timeSpan">Interval added between consecutive date/time values.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveDateTimes(int count, DateTimeOffset start, TimeSpan timeSpan) => Consecutive(count, start, timeSpan);

    /// <summary>Returns consecutive calendar days beginning at the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="year">Year component of the starting date.</param>
    /// <param name="month">Month component of the starting date.</param>
    /// <param name="day">Day component of the starting date.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveDays(int count, int year = 2023, int month = 1, int day = 1)
    {
        System.DateTime start = new(year, month, day, 0, 0, 0, System.DateTimeKind.Unspecified);
        return Consecutive(count, start, TimeSpan.FromDays(1));
    }

    /// <summary>Returns consecutive calendar days beginning at the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveDays(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromDays(1));

    /// <summary>Returns consecutive calendar days beginning at the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveDays(int count, DateTimeOffset start) => Consecutive(count, start, TimeSpan.FromDays(1));

    /// <summary>Returns consecutive weekdays beginning at or after the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="year">Year component of the starting date.</param>
    /// <param name="month">Month component of the starting date.</param>
    /// <param name="day">Day component of the starting date.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveWeekdays(int count, int year = 2023, int month = 1, int day = 1)
    {
        System.DateTime start = new(year, month, day, 0, 0, 0, System.DateTimeKind.Unspecified);
        return ConsecutiveWeekdays(count, start);
    }

    /// <summary>Returns consecutive weekdays beginning at or after the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveWeekdays(int count, System.DateTime start)
    {
        System.DateTime[] dates = new System.DateTime[count];
        TimeSpan step = TimeSpan.FromDays(1);
        int i = 0;
        while (i < count)
        {
            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start += step;
            }

            dates[i] = start;
            i++;
            start += step;
        }

        return dates;
    }

    /// <summary>Returns consecutive weekdays beginning at or after the supplied date.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveWeekdays(int count, DateTimeOffset start)
    {
        DateTimeOffset[] dates = new DateTimeOffset[count];
        TimeSpan step = TimeSpan.FromDays(1);
        int i = 0;
        while (i < count)
        {
            while (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                start += step;
            }

            dates[i] = start;
            i++;
            start += step;
        }

        return dates;
    }

    /// <summary>Returns consecutive hourly date/time values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveHours(int count)
    {
        var start = new System.DateTime(2023, 01, 01, 0, 0, 0, System.DateTimeKind.Unspecified);
        return ConsecutiveHours(count, start);
    }

    /// <summary>Returns consecutive hourly date/time values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveHours(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromHours(1));

    /// <summary>Returns consecutive hourly date/time values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveHours(int count, DateTimeOffset start) => Consecutive(count, start, TimeSpan.FromHours(1));

    /// <summary>Returns consecutive date/time values at fifteen-minute intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveQuarterHours(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromMinutes(QuarterHourMinutes));

    /// <summary>Returns consecutive date/time values at fifteen-minute intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveQuarterHours(int count, DateTimeOffset start) => Consecutive(count, start, TimeSpan.FromMinutes(QuarterHourMinutes));

    /// <summary>Returns consecutive date/time values at one-minute intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveMinutes(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromMinutes(1));

    /// <summary>Returns consecutive date/time values at one-minute intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveMinutes(int count, DateTimeOffset start) => Consecutive(count, start, TimeSpan.FromMinutes(1));

    /// <summary>Returns consecutive date/time values at one-second intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static System.DateTime[] ConsecutiveSeconds(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromSeconds(1));

    /// <summary>Returns consecutive date/time values at one-second intervals.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <returns>The generated array of values.</returns>
    public static DateTimeOffset[] ConsecutiveSeconds(int count, DateTimeOffset start) => Consecutive(count, start, TimeSpan.FromSeconds(1));

    /// <summary>Returns sampled sine-wave values with the requested amplitude, offset, and phase.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <param name="phase">Horizontal phase shift expressed as a fraction of a cycle.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] Sin(int count = 51, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
    {
        double sinScale = TwoPiFactor * Math.PI * oscillations / (count - 1);
        double[] ys = new double[count];
        for (int i = 0; i < ys.Length; i++)
        {
            ys[i] = (Math.Sin((i * sinScale) + (phase * Math.PI * TwoPiFactor)) * mult) + offset;
        }

        return ys;
    }

    /// <summary>Returns samples of a logistic curve with the requested scale and steepness.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="steepness">Slope applied to the logistic curve.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] Sigmoidal(int count, double mult = 1.0, double steepness = 1.0)
    {
        double[] ys = new double[count];

        for (int i = 0; i < count; i++)
        {
            double x = ((double)i / count * SigmoidScale) - SigmoidMidpoint;
            ys[i] = mult / (1 + Math.Exp(-x * steepness));
        }

        return ys;
    }

    /// <summary>Returns sampled cosine-wave values with the requested amplitude, offset, and phase.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <param name="phase">Horizontal phase shift expressed as a fraction of a cycle.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] Cos(int count = 51, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
    {
        double sinScale = TwoPiFactor * Math.PI * oscillations / (count - 1);
        double[] ys = new double[count];
        for (int i = 0; i < ys.Length; i++)
        {
            ys[i] = (Math.Cos((i * sinScale) + (phase * Math.PI * TwoPiFactor)) * mult) + offset;
        }

        return ys;
    }

    /// <summary>Returns sine-wave samples with random noise added.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="magnitude">Maximum scale of the random perturbation.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [Obsolete("use Generate.Sin() then Generate.AddNoise()")]
    public static double[] NoisySin(int count = 51, double magnitude = 1)
    {
        double[] sin = Sin(count);
        AddNoiseInPlace(sin, magnitude);
        return sin;
    }

    /// <summary>Returns exponentially increasing samples with signal-scaled random noise.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="noise">Magnitude of random noise relative to the signal.</param>
    /// <param name="tau">Rate constant controlling the exponential curve.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] NoisyExponential(int count, double mult = 1000, double noise = .5, double tau = 5)
    {
        double[] values = new double[count];

        // add this to ensure we never actually return 0, so Log10() never returns infinity
        double offset = mult * NonZeroOffsetRatio;

        for (int i = 0; i < values.Length; i++)
        {
            double x = (double)i / (count - 1);
            double y = (Math.Exp(tau * x) - 1) / Math.Exp(tau); // 0 to 1
            y *= mult;
            y += Math.Abs(RandomData.RandomNormalNumber()) * noise * y;
            y += offset;
            values[i] = y;
        }

        return values;
    }

    /// <summary>Returns samples of a square wave with the requested duty cycle and levels.</summary>
    /// <param name="cycles">Number of cycles to generate.</param>
    /// <param name="pointsPerCycle">Number of samples in each cycle.</param>
    /// <param name="duty">Fraction of each cycle assigned to the high level.</param>
    /// <param name="low">Value used for the low portion of the wave.</param>
    /// <param name="high">Value used for the high portion of the wave.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] SquareWave(uint cycles = 20, uint pointsPerCycle = 1_000, double duty = .5, double low = 0, double high = 1)
    {
        if (duty < 0 || duty > 1)
        {
            throw new ArgumentException($"{nameof(duty)} must be in the range [0, 1]");
        }

        uint points = cycles * pointsPerCycle;
        uint cyclePointsHigh = (uint)(pointsPerCycle * duty);
        uint cyclePointsLow = pointsPerCycle - cyclePointsHigh;

        double[] values = new double[points];

        uint i = 0;

        for (int c = 0; c < cycles; c++)
        {
            for (int p = 0; p < cyclePointsLow; p++)
            {
                values[i] = low;
                i++;
            }

            for (int p = 0; p < cyclePointsHigh; p++)
            {
                values[i] = high;
                i++;
            }
        }

        return values;
    }

    /// <summary>Returns a square-wave approximation formed from odd sine harmonics.</summary>
    /// <param name="pointCount">Number of samples in the approximation.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <param name="sineCount">Number of odd harmonics to include.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] SquareWaveFromSines(int pointCount = 1000, double oscillations = 2, int sineCount = 5)
    {
        double[] values = new double[pointCount];
        double deltaX = Math.PI * TwoPiFactor * oscillations / (pointCount - 1);

        for (int j = 0; j < sineCount; j++)
        {
            int harmonic = (j * HarmonicStep) + 1;
            for (int i = 0; i < pointCount; i++)
            {
                values[i] += Math.Sin(i * deltaX * harmonic) / harmonic;
            }
        }

        return values;
    }

    /// <summary>Returns an array filled with zero values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] Zeros(int count) => Repeating(count, 0);

    /// <summary>Returns an array filled with one values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] Ones(int count) => Repeating(count, 1);

    /// <summary>Returns an array filled with the supplied value.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="value">Value repeated in the returned array.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] Repeating(int count, double value)
    {
        double[] values = new double[count];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = value;
        }

        return values;
    }

    /// <summary>Returns an array filled with NaN values.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] NaN(int count) => Repeating(count, double.NaN);

    // TODO: obsolete this
    /// <summary>Returns values from start to stop at the requested step, including the endpoints.</summary>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="stop">Ending value for the generated sequence.</param>
    /// <param name="step">Increment between generated values.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] Range(double start, double stop, double step = 1)
    {
        int n = (int)Math.Round((stop - start) / step) + 1;
        return Range(start, stop, n);
    }

    /// <summary>Returns values from start to stop at the requested step, including the endpoints.</summary>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="stop">Ending value for the generated sequence.</param>
    /// <param name="step">Increment between generated values.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] RangeWithStep(double start, double stop, double step = 1) => Range(start, stop, step);

    /// <summary>Returns the requested number of values spanning start through stop.</summary>
    /// <param name="start">Starting value or date/time for the generated sequence.</param>
    /// <param name="stop">Ending value for the generated sequence.</param>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] RangeWithCount(double start, double stop, int count) => Range(start, stop, count);

    /// <summary>Returns a two-dimensional array with constant row-major spacing.</summary>
    /// <param name="rows">Number of rows to generate.</param>
    /// <param name="columns">Number of columns to generate.</param>
    /// <param name="spacing">Spacing between adjacent values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <returns>The generated double[, ] value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "The multidimensional-array return type is part of the existing public API and data contract.")]
    public static double[, ] Consecutive2D(int rows, int columns, double spacing = 1, double offset = 0)
    {
        double[, ] data = new double[rows, columns];

        var count = offset;
        for (var y = 0; y < data.GetLength(0); y++)
        {
            for (int x = 0; x < data.GetLength(1); x++)
            {
                data[y, x] = count;
                count += spacing;
            }
        }

        return data;
    }

    /// <summary>Returns a two-dimensional sine pattern for the requested dimensions and periods.</summary>
    /// <param name="width">Number of columns or horizontal extent of the generated data.</param>
    /// <param name="height">Number of rows or vertical extent of the generated data.</param>
    /// <param name="xPeriod">Horizontal period factor for the generated pattern.</param>
    /// <param name="yPeriod">Vertical period factor for the generated pattern.</param>
    /// <param name="multiple">Intensity multiplier applied to the generated pattern.</param>
    /// <returns>The generated double[, ] value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "The multidimensional-array return type is part of the existing public API and data contract.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "SST1305",
        Justification = "These coordinate-prefixed parameter names are public named-argument identifiers and cannot be changed compatibly.")]
    public static double[, ] Sin2D(int width, int height, double xPeriod = .2, double yPeriod = .2, double multiple = 100)
    {
        double[, ] intensities = new double[height, width];

        for (int y = 0; y < height; y++)
        {
            double siny = Math.Cos(y * yPeriod) * multiple;
            for (int x = 0; x < width; x++)
            {
                double sinx = Math.Sin(x * xPeriod) * multiple;
                intensities[y, x] = sinx + siny;
            }
        }

        return intensities;
    }

    /// <summary>Returns a two-dimensional diagonal gradient spanning the supplied bounds.</summary>
    /// <param name="width">Number of columns or horizontal extent of the generated data.</param>
    /// <param name="height">Number of rows or vertical extent of the generated data.</param>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated double[, ] value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1020",
        Justification = "The multidimensional-array return type is part of the existing public API and data contract.")]
    public static double[, ] Ramp2D(int width, int height, double min = 0, double max = 1)
    {
        double[, ] intensities = new double[height, width];

        double span = max - min;

        for (int y = 0; y < height; y++)
        {
            double fracY = (double)y / height;
            double valY = (fracY * span) + min;

            for (int x = 0; x < width; x++)
            {
                double fracX = (double)x / width;
                double valX = (fracX * span) + min;

                intensities[y, x] = (valX + valY) / HalvingDivisor;
            }
        }

        return intensities;
    }

    /// <summary>Returns rooted vectors sampled across the requested grid.</summary>
    /// <param name="columns">Number of columns to generate.</param>
    /// <param name="rows">Number of rows to generate.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static RootedCoordinateVector[] SampleVectors(int columns = 10, int rows = 10, double oscillations = 1)
    {
        double[] xs = Consecutive(columns);
        double[] ys = Consecutive(rows);

        List<RootedCoordinateVector> vectors = new();
        for (int i = 0; i < xs.Length; i++)
        {
            for (int j = 0; j < ys.Length; j++)
            {
                double x = (double)i / xs.Length * Math.PI * oscillations;
                double y = (double)j / xs.Length * Math.PI * oscillations;
                double deltaX = Math.Sin(x) + Math.Sin(y);
                double deltaY = Math.Sin(x) - Math.Sin(y);
                System.Numerics.Vector2 v = new((float)deltaX, (float)deltaY);
                Coordinates pt = new(xs[i], ys[j]);
                RootedCoordinateVector vector = new(pt, v);
                vectors.Add(vector);
            }
        }

        return vectors.ToArray();
    }

    /// <summary>Returns a random walk beginning at the supplied offset.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] RandomWalk(int count, double mult = 1, double offset = 0) => RandomData.RandomWalk(count, mult, offset);

    /// <summary>Returns random values in the requested range.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [Obsolete("use RandomSample()")]
    public static double[] Random(int count, double min = 0, double max = 1) => RandomSample(count, min, max);

    /// <summary>Returns random values in the requested range.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] RandomSample(int count, double min = 0, double max = 1)
    {
        ThrowIfNegative(count, nameof(count));
        double[] values = new double[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = RandomData.RandomNumber(min, max);
        }

        return values;
    }

    /// <summary>Returns ascending values with random spacing.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="minDelta">Minimum random increment between values.</param>
    /// <param name="maxDelta">Maximum random increment between values.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] RandomAscending(int count, double minDelta = 0, double maxDelta = 1)
    {
        double[] values = RandomSample(count, minDelta, maxDelta);

        for (int i = 1; i < count; i++)
        {
            values[i] += values[i - 1];
        }

        return values;
    }

    /// <summary>Returns values sampled from the requested normal distribution.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="mean">Mean of the normal distribution.</param>
    /// <param name="stdDev">Standard deviation of the normal distribution.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] RandomNormal(int count, double mean = 0, double stdDev = 1)
    {
        ThrowIfNegative(count, nameof(count));
        double[] values = new double[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = RandomData.RandomNormalNumber(mean, stdDev);
        }

        return values;
    }

    /// <summary>Returns a random integer within the requested exclusive upper bound.</summary>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated integer.</returns>
    public static int RandomInteger(int max) => RandomData.RandomInteger(max);

    /// <summary>Returns a random integer within the requested exclusive upper bound.</summary>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated integer.</returns>
    public static int RandomInteger(int min, int max) => RandomData.RandomInteger(min, max);

    /// <summary>Returns random integers within the requested bounds.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    public static int[] RandomIntegers(int count, int max)
    {
        ThrowIfNegative(count, nameof(count));
        int[] values = new int[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = RandomData.RandomInteger(max);
        }

        return values;
    }

    /// <summary>Returns random integers within the requested bounds.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    public static int[] RandomIntegers(int count, int min, int max)
    {
        ThrowIfNegative(count, nameof(count));
        int[] values = new int[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = RandomData.RandomInteger(min, max);
        }

        return values;
    }

    /// <summary>Returns a random number within the requested bounds.</summary>
    /// <returns>The generated numeric value.</returns>
    public static double RandomNumber() => RandomData.RandomNumber(0, 1);

    /// <summary>Returns a random number within the requested bounds.</summary>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated numeric value.</returns>
    public static double RandomNumber(double max) => RandomData.RandomNumber(0, max);

    /// <summary>Returns a random number within the requested bounds.</summary>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated numeric value.</returns>
    public static double RandomNumber(double min, double max) => RandomData.RandomNumber(min, max);

    /// <summary>Returns a random number from the requested normal distribution.</summary>
    /// <param name="mean">Mean of the normal distribution.</param>
    /// <param name="stdDev">Standard deviation of the normal distribution.</param>
    /// <returns>The generated numeric value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double RandomNormalNumber(double mean = 0, double stdDev = 1) => RandomData.RandomNormalNumber(mean, stdDev);

    /// <summary>Returns random numbers within the requested bounds.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] RandomNumbers(int count, double max) => Generate.RandomSample(count, 0, max);

    /// <summary>Returns random numbers within the requested bounds.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="min">Inclusive lower bound or starting value.</param>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated array of values.</returns>
    public static double[] RandomNumbers(int count, double min, double max) => Generate.RandomSample(count, min, max);

    /// <summary>Returns a copy of the input values with random noise added.</summary>
    /// <param name="input">Source values copied before noise is added.</param>
    /// <param name="magnitude">Maximum scale of the random perturbation.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] AddNoise(double[] input, double magnitude = 1) => RandomData.AddNoise(input, magnitude);

    /// <summary>Adds random noise to each value in the supplied array.</summary>
    /// <param name="values">Array modified by the operation.</param>
    /// <param name="magnitude">Maximum scale of the random perturbation.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static void AddNoiseInPlace(double[] values, double magnitude = 1) => RandomData.AddNoiseInPlace(values, magnitude);

    /// <summary>Adds a sine wave to the supplied array and returns that same array.</summary>
    /// <param name="values">Array modified by the operation.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <param name="phase">Horizontal phase shift expressed as a fraction of a cycle.</param>
    /// <returns>The same array after the sine wave has been added.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] AddSinInPlace(double[] values, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
    {
        double sinScale = TwoPiFactor * Math.PI * oscillations / (values.Length - 1);
        for (int i = 0; i < values.Length; i++)
        {
            values[i] += (Math.Sin((i * sinScale) + (phase * Math.PI * TwoPiFactor)) * mult) + offset;
        }

        return values;
    }

    /// <summary>Returns a copy of the supplied array with a sine wave added.</summary>
    /// <param name="values">Array modified by the operation.</param>
    /// <param name="mult">Amplitude or scale applied to generated values.</param>
    /// <param name="offset">Value added to each generated sample.</param>
    /// <param name="oscillations">Number of wave cycles across the samples.</param>
    /// <param name="phase">Horizontal phase shift expressed as a fraction of a cycle.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    public static double[] AddSin(double[] values, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
    {
        double[] values2 = new double[values.Length];
        Array.Copy(values, values2, values.Length);
        _ = AddSinInPlace(values2, mult, offset, oscillations, phase);
        return values2;
    }

    /// <summary>Returns a random uppercase English letter.</summary>
    /// <returns>The generated char value.</returns>
    public static char RandomChar() => (char)('A' + RandomInteger(AlphabetLength));

    /// <summary>Returns a string of random uppercase English letters.</summary>
    /// <param name="length">Number of characters to generate.</param>
    /// <returns>The generated string value.</returns>
    public static string RandomString(int length)
    {
        char[] chars = new char[length];

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = RandomChar();
        }

        return new(chars);
    }

    /// <summary>Returns a random rectangle within the scaled coordinate range.</summary>
    /// <param name="xMult">Scale applied to horizontal coordinates.</param>
    /// <param name="yMult">Scale applied to vertical coordinates.</param>
    /// <param name="width">Number of columns or horizontal extent of the generated data.</param>
    /// <param name="height">Number of rows or vertical extent of the generated data.</param>
    /// <returns>The generated CoordinateRect value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "SST1305",
        Justification = "These coordinate-prefixed parameter names are public named-argument identifiers and cannot be changed compatibly.")]
    public static CoordinateRect RandomCoordinateRect(double xMult = 1, double yMult = 1, double width = .1, double height = .1)
    {
        Coordinates center = RandomCoordinates(xMult, yMult);
        width *= RandomNumber(1, RandomScaleUpperBound);
        height *= RandomNumber(1, RandomScaleUpperBound);
        double x1 = center.X - (width / HalvingDivisor * xMult);
        double x2 = center.X + (width / HalvingDivisor * xMult);
        double y1 = center.Y - (height / HalvingDivisor * xMult);
        double y2 = center.Y + (height / HalvingDivisor * xMult);
        return new(x1, x2, y1, y2);
    }

    /// <summary>Returns a line between two random coordinates.</summary>
    /// <param name="xMult">Scale applied to horizontal coordinates.</param>
    /// <param name="yMult">Scale applied to vertical coordinates.</param>
    /// <param name="xOffset">Offset added to horizontal coordinates.</param>
    /// <param name="yOffset">Offset added to vertical coordinates.</param>
    /// <returns>The generated CoordinateLine value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "SST1305",
        Justification = "These coordinate-prefixed parameter names are public named-argument identifiers and cannot be changed compatibly.")]
    public static CoordinateLine RandomCoordinateLine(double xMult = 1, double yMult = 1, double xOffset = 0, double yOffset = 0)
    {
        Coordinates pt1 = RandomCoordinates(xMult, yMult, xOffset, yOffset);
        Coordinates pt2 = RandomCoordinates(xMult, yMult, xOffset, yOffset);
        return new(pt1, pt2);
    }

    /// <summary>Returns one or more randomly generated coordinates.</summary>
    /// <param name="xMult">Scale applied to horizontal coordinates.</param>
    /// <param name="yMult">Scale applied to vertical coordinates.</param>
    /// <param name="xOffset">Offset added to horizontal coordinates.</param>
    /// <param name="yOffset">Offset added to vertical coordinates.</param>
    /// <returns>The generated Coordinates value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "SST1305",
        Justification = "These coordinate-prefixed parameter names are public named-argument identifiers and cannot be changed compatibly.")]
    public static Coordinates RandomCoordinates(double xMult = 1, double yMult = 1, double xOffset = 0, double yOffset = 0)
    {
        double x = (RandomData.RandomNumber() * xMult) + xOffset;
        double y = (RandomData.RandomNumber() * yMult) + yOffset;
        return new(x, y);
    }

    /// <summary>Returns one or more randomly generated coordinates.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="xMult">Scale applied to horizontal coordinates.</param>
    /// <param name="yMult">Scale applied to vertical coordinates.</param>
    /// <param name="xOffset">Offset added to horizontal coordinates.</param>
    /// <param name="yOffset">Offset added to vertical coordinates.</param>
    /// <returns>The generated array of values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional defaults are established API; removing them would break source compatibility and change call-site behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Naming",
        "SST1305",
        Justification = "These coordinate-prefixed parameter names are public named-argument identifiers and cannot be changed compatibly.")]
    public static Coordinates[] RandomCoordinates(int count, double xMult = 1, double yMult = 1, double xOffset = 0, double yOffset = 0)
    {
        Coordinates[] cs = new Coordinates[count];

        for (int i = 0; i < count; i++)
        {
            cs[i] = RandomCoordinates(xMult, yMult, xOffset, yOffset);
        }

        return cs;
    }

    /// <summary>Returns a random coordinate within the supplied axis limits.</summary>
    /// <returns>The generated Coordinates value.</returns>
    public static Coordinates RandomLocation()
    {
        AxisLimits limits = new(0, 1, 0, 1);
        return RandomLocation(limits);
    }

    /// <summary>Returns a random coordinate within the supplied axis limits.</summary>
    /// <param name="limits">Axis limits containing the generated coordinate.</param>
    /// <returns>The generated Coordinates value.</returns>
    public static Coordinates RandomLocation(AxisLimits limits)
    {
        double x = (RandomData.RandomNumber() * limits.HorizontalSpan) + limits.Left;
        double y = (RandomData.RandomNumber() * limits.VerticalSpan) + limits.Bottom;
        return new(x, y);
    }

    /// <summary>Returns random coordinates within the unit range or supplied axis limits.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    public static Coordinates[] RandomLocations(int count)
    {
        ThrowIfNegative(count, nameof(count));
        Coordinates[] coordinates = new Coordinates[count];
        for (int i = 0; i < count; i++)
        {
            coordinates[i] = RandomCoordinates();
        }

        return coordinates;
    }

    /// <summary>Returns random coordinates within the unit range or supplied axis limits.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="limits">Axis limits containing the generated coordinate.</param>
    /// <returns>The generated array of values.</returns>
    public static Coordinates[] RandomLocations(int count, AxisLimits limits)
    {
        ThrowIfNegative(count, nameof(count));
        Coordinates[] coordinates = new Coordinates[count];
        for (int i = 0; i < count; i++)
        {
            coordinates[i] = RandomLocation(limits);
        }

        return coordinates;
    }

    /// <summary>Returns a randomly generated OHLC sample, optionally assigned to the supplied date.</summary>
    /// <returns>The generated OHLC value.</returns>
    public static OHLC RandomOHLC()
    {
        List<OHLC> values = RandomData.RandomOHLCs(1);
        return values[0];
    }

    /// <summary>Returns a randomly generated OHLC sample, optionally assigned to the supplied date.</summary>
    /// <param name="date">Date assigned to the generated sample.</param>
    /// <returns>The generated OHLC value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static OHLC RandomOHLC(System.DateTime date)
    {
        OHLC ohlc = RandomOHLC();
        ohlc.DateTime = date;
        return ohlc;
    }

    /// <summary>Returns randomly generated OHLC samples.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <returns>The generated list of samples.</returns>
    public static List<OHLC> RandomOHLCs(int count) => RandomData.RandomOHLCs(count);

    /// <summary>Returns randomly generated OHLC samples.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="startDate">Date of the first generated sample.</param>
    /// <returns>The generated list of samples.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2016",
        Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
    public static List<OHLC> RandomOHLCs(int count, System.DateTime startDate) => RandomData.RandomOHLCs(count, startDate);

    /// <summary>Returns a box plot summary of randomly sampled values at the supplied position.</summary>
    /// <param name="position">Horizontal position assigned to the generated box.</param>
    /// <returns>The generated Box value.</returns>
    public static Box RandomBox(double position)
    {
        const int n = 50;
        double mean = RandomData.RandomNumber(BoxDistributionMaximum);
        double stdDev = RandomData.RandomNumber(BoxDistributionMaximum);

        double[] values = Generate.RandomNormal(n, mean, stdDev);
        Array.Sort(values);
        double min = values[0];
        double q1 = values[n / QuartileDivisor];
        double median = values[n / HalvingDivisor];
        double q3 = values[UpperQuartileMultiplier * n / QuartileDivisor];
        double max = values[n - 1];

        return new Box { Position = position, WhiskerMin = min, BoxMin = q1, BoxMiddle = median, BoxMax = q3, WhiskerMax = max, };
    }

    /// <summary>Returns a randomly selected hue.</summary>
    /// <returns>The generated Color value.</returns>
    public static Color RandomHue() => Color.RandomHue();

    /// <summary>Returns a randomly generated color, optionally limited by a channel maximum.</summary>
    /// <returns>The generated Color value.</returns>
    public static Color RandomColor()
    {
        byte r = RandomData.RandomByte();
        byte g = RandomData.RandomByte();
        byte b = RandomData.RandomByte();
        return new(r, g, b);
    }

    /// <summary>Returns a randomly generated color, optionally limited by a channel maximum.</summary>
    /// <param name="max">Exclusive upper bound or ending value, depending on the operation.</param>
    /// <returns>The generated Color value.</returns>
    public static Color RandomColor(byte max)
    {
        byte r = (byte)RandomData.RandomInteger(max);
        byte g = (byte)RandomData.RandomInteger(max);
        byte b = (byte)RandomData.RandomInteger(max);
        return new(r, g, b);
    }

    /// <summary>Returns a randomly generated color, optionally limited by a channel maximum.</summary>
    /// <param name="colormap">Colormap used to convert a random value to a color.</param>
    /// <returns>The generated Color value.</returns>
    public static Color RandomColor(IColormap colormap) => colormap.GetColor(RandomData.RandomNumber());

    /// <summary>Returns colors sampled from the supplied colormap.</summary>
    /// <param name="count">Number of values to generate.</param>
    /// <param name="colormap">Colormap used to convert a random value to a color.</param>
    /// <returns>The generated array of values.</returns>
    public static Color[] RandomColors(int count, IColormap colormap)
    {
        double[] samples = RandomSample(count);
        Color[] colors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            colors[i] = colormap.GetColor(samples[i]);
        }

        return colors;
    }

    /// <summary>Returns a randomly selected marker shape.</summary>
    /// <returns>The generated MarkerShape value.</returns>
    public static MarkerShape RandomMarkerShape()
    {
        MarkerShape[] markerShapes = GetMarkerShapes();

        int i = RandomInteger(markerShapes.Length);
        return markerShapes[i];
    }

    /// <summary>Returns a randomly selected marker shape that is not line-only.</summary>
    /// <returns>The generated MarkerShape value.</returns>
    public static MarkerShape RandomFilledMarkerShape()
    {
        MarkerShape[] allMarkerShapes = GetMarkerShapes();
        List<MarkerShape> filledMarkerShapes = new();
        foreach (MarkerShape markerShape in allMarkerShapes)
        {
            if (!markerShape.IsLineOnly())
            {
                filledMarkerShapes.Add(markerShape);
            }
        }

        MarkerShape[] markerShapes = filledMarkerShapes.ToArray();

        int i = RandomInteger(markerShapes.Length);
        return markerShapes[i];
    }

    /// <summary>Returns a line between two random locations.</summary>
    /// <returns>The generated CoordinateLine value.</returns>
    public static CoordinateLine RandomLine() => new(RandomLocation(), RandomLocation());

    /// <summary>Returns a randomly selected line pattern.</summary>
    /// <returns>The generated LinePattern value.</returns>
    public static LinePattern RandomLinePattern()
    {
        LinePattern[] linePatterns = LinePattern.GetAllPatterns();
        int i = RandomInteger(linePatterns.Length);
        return linePatterns[i];
    }

    /// <summary>Returns evenly spaced values between the supplied endpoints.</summary>
    /// <param name="start">Starting value of the sequence.</param>
    /// <param name="stop">Ending value of the sequence.</param>
    /// <param name="n">Number of values to generate.</param>
    /// <returns>The generated array of values.</returns>
    private static double[] Range(double start, double stop, int n)
    {
        ThrowIfNegative(n, nameof(n));
        double[] values = new double[n];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = ((stop - start) * i / (n - 1)) + start;
        }

        return values;
    }

    /// <summary>Throws when a value is negative.</summary>
    /// <param name="value">Value that must be non-negative.</param>
    /// <param name="parameterName">Name of the argument to report.</param>
    private static void ThrowIfNegative(int value, string parameterName)
    {
#if NET7_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
#else
        if ((uint)value <= int.MaxValue)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(parameterName);
#endif
    }

    /// <summary>Returns the marker values using the overload available on each target framework.</summary>
    /// <returns>The values defined by <see cref="MarkerShape"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA2263",
        Justification = "The generic overload is unavailable on the net47 target.")]
    private static MarkerShape[] GetMarkerShapes()
    {
#if NET5_0_OR_GREATER
        return Enum.GetValues<MarkerShape>();
#else
        return (MarkerShape[])Enum.GetValues(typeof(MarkerShape));
#endif
    }

    /// <summary>Provides legacy date/time sequence generators.</summary>
    [Obsolete("use ConsecutiveDays(), ConsecutiveHours(), ConsecutiveMinutes(), etc.", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete nested type is retained for public API compatibility.")]
    public static class DateTime
    {
        /// <summary>Gets the fixed example date used by the legacy date helpers.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "Legacy DateTime field retained for API compatibility.")]
        public static readonly System.DateTime ExampleDate = new(2018, 01, 03, 0, 0, 0, System.DateTimeKind.Unspecified);

        /// <summary>Returns evenly spaced values beginning at the supplied start value.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <param name="step">Increment between generated values.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Consecutive(int count, System.DateTime start, TimeSpan step)
        {
            System.DateTime dt = start;
            System.DateTime[] values = new System.DateTime[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = dt;
                dt += step;
            }

            return values;
        }

        /// <summary>Returns consecutive weekdays beginning at or after the supplied date.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Weekdays(int count, System.DateTime start)
        {
            System.DateTime[] dates = new System.DateTime[count];
            int i = 0;
            while (i < count)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    dates[i] = start;
                    i++;
                }

                start = start.AddDays(1);
            }

            return dates;
        }

        /// <summary>Returns consecutive weekdays beginning at or after the supplied date.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Weekdays(int count) => Weekdays(count, ExampleDate);

        /// <summary>Returns consecutive daily date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Days(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromDays(1));

        /// <summary>Returns consecutive daily date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Days(int count) => Days(count, ExampleDate);

        /// <summary>Returns consecutive hourly date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Hours(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromHours(1));

        /// <summary>Returns consecutive hourly date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Hours(int count) => Hours(count, ExampleDate);

        /// <summary>Returns consecutive minute-spaced date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Minutes(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromMinutes(1));

        /// <summary>Returns consecutive minute-spaced date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2318",
            Justification = "These legacy overloads retain their existing generated output for compatibility.")]
        public static System.DateTime[] Minutes(int count) => Hours(count, ExampleDate);

        /// <summary>Returns consecutive second-spaced date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <param name="start">Starting value or date/time for the generated sequence.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        public static System.DateTime[] Seconds(int count, System.DateTime start) => Consecutive(count, start, TimeSpan.FromSeconds(1));

        /// <summary>Returns consecutive second-spaced date/time values.</summary>
        /// <param name="count">Number of values to generate.</param>
        /// <returns>The generated array of values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2016",
            Justification = "DateTime APIs remain for compatibility; DateTimeOffset would break callers and change values.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2318",
            Justification = "These legacy overloads retain their existing generated output for compatibility.")]
        public static System.DateTime[] Seconds(int count) => Hours(count, ExampleDate);
    }
}
