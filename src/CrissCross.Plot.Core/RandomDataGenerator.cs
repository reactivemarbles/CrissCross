// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

#nullable enable

/// <summary>Generates pseudo-random data for plot examples and test scenarios.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Security",
    "CA5394",
    Justification = "This class generates non-security plot/sample data; its seeded System.Random behavior is part of the deterministic data-generation API.")]
public class RandomDataGenerator
{
    /// <summary>The exclusive upper bound for uniformly selected byte values.</summary>
    private const int ByteUpperBoundExclusive = byte.MaxValue + 1;

    /// <summary>The unit upper bound used for default random ranges.</summary>
    private const double UnitIntervalUpperBound = 1.0;

    /// <summary>The first generated random-walk index after the initial value.</summary>
    private const int FirstSampleIndex = 1;

    /// <summary>The offset that makes a byte upper bound inclusive.</summary>
    private const int InclusiveUpperBoundOffset = 1;

    /// <summary>The base used by random sine frequency generation.</summary>
    private const double RandomSinFrequencyBase = 2.0;

    /// <summary>The unit interval offset used to center random values.</summary>
    private const double UnitIntervalOffset = 1.0;

    /// <summary>The factor used by the Box-Muller transform.</summary>
    private const double BoxMullerFactor = -2.0;

    /// <summary>The scale used to generate a symmetric random value.</summary>
    private const double SymmetricRangeScale = 2.0;

    /// <summary>The starting frequency exponent used by random sine generation.</summary>
    private const double RandomSinFrequencyBaseExponent = 1.0;

    /// <summary>The frequency exponent range used by random sine generation.</summary>
    private const double RandomSinFrequencyExponentRange = 10.0;

    /// <summary>The centering offset used by random sine generation.</summary>
    private const double RandomSinCenteringOffset = 0.5;

    /// <summary>The starting oscillation count used by random sine generation.</summary>
    private const double RandomSinOscillationBase = 1.0;

    /// <summary>The oscillation count range used by random sine generation.</summary>
    private const double RandomSinOscillationRange = 5.0;

    /// <summary>The year used by the default OHLC sample start date.</summary>
    private const int DefaultOhlcYear = 2024;

    /// <summary>The month used by the default OHLC sample start date.</summary>
    private const int DefaultOhlcMonth = 1;

    /// <summary>The day used by the default OHLC sample start date.</summary>
    private const int DefaultOhlcDay = 1;

    /// <summary>The minimum opening price used by random OHLC samples.</summary>
    private const int MinimumOhlcOpen = 150;

    /// <summary>The maximum opening price used by random OHLC samples.</summary>
    private const int MaximumOhlcOpen = 250;

    /// <summary>The default start date for generated OHLC samples.</summary>
    private static readonly DateTime DefaultOhlcStartDate = new(DefaultOhlcYear, DefaultOhlcMonth, DefaultOhlcDay, 0, 0, 0, DateTimeKind.Unspecified);

#if !NET5_0_OR_GREATER
    /// <summary>The thread-local random generator used on older target frameworks.</summary>
    private static readonly System.Threading.ThreadLocal<Random> GlobalRandomThread = new(static () => new Random(GetCryptoRandomInt()));
#endif

    /// <summary>The current pseudo-random generator.</summary>
    private Random _rand;

    /// <summary>Initializes a new instance of the <see cref="RandomDataGenerator"/> class.</summary>
    /// <param name="seed">An optional seed for reproducible output.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional seed is part of the established public constructor API.")]
    public RandomDataGenerator(int? seed = null)
    {
        _rand = seed.HasValue
            ? new Random(seed.Value)
#if NET5_0_OR_GREATER
            : Random.Shared;
#else
            : GlobalRandomThread.Value!;
#endif
    }

    /// <summary>Gets the deterministic generator used by static sample-generation helpers.</summary>
    public static RandomDataGenerator Generate { get; } = new(0);

    /// <summary>Resets the pseudo-random generator to the specified seed.</summary>
    /// <param name="seed">The seed used for subsequent generated values.</param>
    public void Seed(int seed) => _rand = new(seed);

    /// <summary>Returns a uniformly distributed value from zero (inclusive) to one (exclusive).</summary>
    /// <returns>A pseudo-random value in the unit interval.</returns>
    public double RandomNumber() => _rand.NextDouble();

    /// <summary>Returns a uniformly distributed number from zero (inclusive) to the specified maximum (exclusive).</summary>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>A pseudo-random value in the requested interval.</returns>
    public double RandomNumber(double max) => _rand.NextDouble() * max;

    /// <summary>Returns a uniformly distributed number from the specified minimum (inclusive) to maximum (exclusive).</summary>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>A pseudo-random value in the requested interval.</returns>
    public double RandomNumber(double min, double max) => (_rand.NextDouble() * (max - min)) + min;

    /// <summary>Returns a random number guaranteed not to be zero.</summary>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>A non-zero pseudo-random value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional upper bound is part of the established public API.")]
    public double RandomNonZeroNumber(double max = UnitIntervalUpperBound)
    {
        double randomValue = RandomNumber(max);
        return randomValue != 0
            ? randomValue
            : RandomNonZeroNumber();
    }

    /// <summary>Returns a pseudo-random non-negative integer.</summary>
    /// <returns>A non-negative integer.</returns>
    public int RandomInteger() => _rand.Next();

    /// <summary>Returns a random integer from zero (inclusive) to the specified maximum (exclusive).</summary>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>A non-negative integer below the maximum.</returns>
    public int RandomInteger(int max) => _rand.Next(max);

    /// <summary>Returns a random integer in the specified half-open interval.</summary>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The exclusive upper bound.</param>
    /// <returns>An integer in the specified interval.</returns>
    public int RandomInteger(int min, int max) => _rand.Next(min, max);

    /// <summary>Returns a uniformly distributed random byte.</summary>
    /// <returns>A byte in the range from zero through 255.</returns>
    public byte RandomByte() => (byte)_rand.Next(ByteUpperBoundExclusive);

    /// <summary>Returns a uniformly distributed random byte in the inclusive range.</summary>
    /// <param name="min">The inclusive lower bound.</param>
    /// <param name="max">The inclusive upper bound.</param>
    /// <returns>A byte within the specified range.</returns>
    public byte RandomByte(byte min, byte max) => (byte)_rand.Next(min, max + InclusiveUpperBoundOffset);

    /// <summary>Returns a normally distributed random number around the specified mean.</summary>
    /// <param name="mean">The distribution mean.</param>
    /// <param name="stdDev">The distribution standard deviation.</param>
    /// <returns>A pseudo-random number from the normal distribution.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional distribution parameters are part of the established public API.")]
    public double RandomNormalNumber(double mean = 0, double stdDev = UnitIntervalUpperBound)
    {
        double u1 = RandomNonZeroNumber();
        double u2 = RandomNumber();
        double randStdNormal = Math.Sqrt(BoxMullerFactor * Math.Log(u1)) * Math.Sin(SymmetricRangeScale * Math.PI * u2);
        return mean + (stdDev * randStdNormal);
    }

    /// <summary>Mutates the given array by adding random noise within the specified magnitude.</summary>
    /// <param name="values">The values to mutate.</param>
    /// <param name="magnitude">The maximum absolute noise added to each value.</param>
    public void AddNoiseInPlace(double[] values, double magnitude)
    {
        for (int i = 0; i < values.Length; i++)
        {
            double noise = ((SymmetricRangeScale * RandomNumber()) - UnitIntervalOffset) * magnitude;
            values[i] = values[i] + noise;
        }
    }

    /// <summary>Returns a copy of the input data with random noise added.</summary>
    /// <param name="input">The values to copy and modify.</param>
    /// <param name="magnitude">The maximum absolute noise added to each value.</param>
    /// <returns>A new array containing the noisy values.</returns>
    public double[] AddNoise(double[] input, double magnitude)
    {
        double[] output = new double[input.Length];
        Array.Copy(input, output, input.Length);
        AddNoiseInPlace(output, magnitude);
        return output;
    }

    /// <summary>Returns uniformly distributed values scaled and shifted by the specified parameters.</summary>
    /// <param name="count">The number of values to generate.</param>
    /// <param name="mult">The multiplier applied to each random value.</param>
    /// <param name="offset">The offset added to each scaled value.</param>
    /// <returns>An array of pseudo-random values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional scale and offset are part of the established public API.")]
    public double[] RandomSample(int count, double mult = UnitIntervalUpperBound, double offset = 0)
    {
        double[] values = new double[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = (_rand.NextDouble() * mult) + offset;
        }

        return values;
    }

    /// <summary>Returns normally distributed random values around the specified mean.</summary>
    /// <param name="count">The number of values to generate.</param>
    /// <param name="mean">The distribution mean.</param>
    /// <param name="stdDev">The distribution standard deviation.</param>
    /// <returns>An array of normally distributed pseudo-random values.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional distribution parameters are part of the established public API.")]
    public double[] RandomNormalSample(int count, double mean = 0, double stdDev = UnitIntervalUpperBound)
    {
        double[] values = new double[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = RandomNormalNumber(mean, stdDev);
        }

        return values;
    }

    /// <summary>Returns a sine wave with random frequency, amplitude, and phase.</summary>
    /// <param name="count">The number of values in the generated wave.</param>
    /// <returns>An array containing the generated sine wave.</returns>
    public double[] RandomSin(int count)
    {
        double mult = Math.Pow(RandomSinFrequencyBase, RandomSinFrequencyBaseExponent + (_rand.NextDouble() * RandomSinFrequencyExponentRange));
        double offset = mult * (_rand.NextDouble() - RandomSinCenteringOffset);
        double oscillations = RandomSinOscillationBase + (_rand.NextDouble() * RandomSinOscillationRange);
        double phase = _rand.NextDouble() * Math.PI * SymmetricRangeScale;
        return CrissCross.Plot.Generate.Sin(count, mult, offset, oscillations, phase);
    }

    /// <summary>Returns a random walk beginning at the specified offset.</summary>
    /// <param name="count">The number of values to generate.</param>
    /// <param name="mult">The scale applied to each random step.</param>
    /// <param name="offset">The starting value.</param>
    /// <param name="slope">The approximate slope between values.</param>
    /// <returns>An array containing the generated random walk.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional scale, offset, and slope are part of the established public API.")]
    public double[] RandomWalk(int count, double mult = UnitIntervalUpperBound, double offset = 0, double slope = 0)
    {
        double[] data = new double[count];
        data[0] = offset;
        for (int i = FirstSampleIndex; i < data.Length; i++)
        {
            double randomStep = (_rand.NextDouble() * SymmetricRangeScale) - UnitIntervalOffset;

            // Using linear equation y_2 = m * x + y_1 where x = 1,
            // then adding a scaled random step simplifies to:
            data[i] = slope + data[i - 1] + (randomStep * mult);
        }

        return data;
    }

    /// <summary>Returns random OHLC samples starting from the default date.</summary>
    /// <param name="count">The number of OHLC samples to generate.</param>
    /// <returns>The generated OHLC samples.</returns>
    public List<OHLC> RandomOHLCs(int count) => RandomOHLCs(count, DefaultOhlcStartDate);

    /// <summary>Returns random OHLC samples starting from the specified date.</summary>
    /// <param name="count">The number of OHLC samples to generate.</param>
    /// <param name="start">The date of the first sample.</param>
    /// <returns>The generated OHLC samples.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "SST2016", Justification = "DateTime is the established public plotting coordinate type; changing this signature would break callers.")]
    public List<OHLC> RandomOHLCs(int count, DateTime start)
    {
        DateTime[] dates = CrissCross.Plot.Generate.ConsecutiveWeekdays(count, start);
        TimeSpan span = TimeSpan.FromDays(1);

        const double mult = UnitIntervalUpperBound;

        List<OHLC> ohlcs = new();
        double open = RandomNumber(MinimumOhlcOpen, MaximumOhlcOpen);
        for (int i = 0; i < count; i++)
        {
            double close = open + RandomNumber(-mult, mult);
            double high = Math.Max(open, close) + RandomNumber(0, mult);
            double low = Math.Min(open, close) - RandomNumber(0, mult);
            OHLC ohlc = new(open, high, low, close, dates[i], span);
            ohlcs.Add(ohlc);
            open = close + RandomNumber(-mult / SymmetricRangeScale, mult / SymmetricRangeScale);
        }

        return ohlcs;
    }

#if !NET5_0_OR_GREATER
    /// <summary>Creates a non-negative seed for the thread-local pseudo-random generator.</summary>
    /// <returns>A non-negative seed value.</returns>
    private static int GetCryptoRandomInt()
    {
        using System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        byte[] data = new byte[sizeof(int)];
        rng.GetBytes(data);
        return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
    }
#endif
}
