// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataGenerators;

/// <summary>Generates deterministic or random financial price data.</summary>
/// <param name="seed">An optional random-number seed.</param>
public class FinancialDataGenerator(int? seed = null)
{
    /// <summary>Starting lower bound for generated prices.</summary>
    private const double MinimumStartingPrice = 150;

    /// <summary>Starting upper bound for generated prices.</summary>
    private const double MaximumStartingPrice = 250;

    /// <summary>Divisor used to reduce the next opening price movement.</summary>
    private const double OpeningPriceMovementDivisor = 2;

    /// <summary>The random generator used to create price movements.</summary>
    private readonly RandomDataGenerator _randomData = new(seed);

    /// <summary>Generates OHLC values using a random walk.</summary>
    /// <param name="count">The number of OHLC values to generate.</param>
    /// <returns>The generated values.</returns>
    public OHLC[] RandomWalk(int count)
    {
        const double scale = 1.0;
        OHLC[] ohlcs = new OHLC[count];

        double open = _randomData.RandomNumber(MinimumStartingPrice * scale, MaximumStartingPrice * scale);

        for (int i = 0; i < count; i++)
        {
            double close = open + _randomData.RandomNumber(-scale, scale);
            double high = Math.Max(open, close) + _randomData.RandomNumber(0, scale);
            double low = Math.Min(open, close) - _randomData.RandomNumber(0, scale);
            ohlcs[i] = new(open, high, low, close);
            open = close + _randomData.RandomNumber(-scale / OpeningPriceMovementDivisor, scale / OpeningPriceMovementDivisor);
        }

        return ohlcs;
    }

    /// <summary>Generates OHLC values with consecutive one-minute timestamps.</summary>
    /// <param name="count">The number of OHLC values to generate.</param>
    /// <returns>The generated values.</returns>
    public OHLC[] OHLCsByMinute(int count)
    {
        DateTime start = new(2024, 9, 24, 9, 30, 0, DateTimeKind.Unspecified);
        TimeSpan delta = TimeSpan.FromMinutes(1);
        DateTime[] dates = Generate.Consecutive(count, start, delta);
        OHLC[] ohlcs = RandomWalk(count);

        for (int i = 0; i < count; i++)
        {
            ohlcs[i] = ohlcs[i].WithDate(dates[i]).WithTimeSpan(delta);
        }

        return ohlcs;
    }
}
