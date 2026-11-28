// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataGenerators;

/// <summary>Generates a one-dimensional random walk.</summary>
/// <param name="seed">An optional random-number seed.</param>
/// <param name="firstValue">The initial value of the walk.</param>
/// <param name="multiplier">The scale applied to each random step.</param>
public class RandomWalker(int? seed = null, double firstValue = 0, double multiplier = 1)
{
    /// <summary>Half of the unit interval used to center random steps.</summary>
    private const double HalfUnitInterval = 0.5;

    /// <summary>The random-number generator used by the walk.</summary>
    private readonly RandomDataGenerator _gen = new(seed);

    /// <summary>The scale applied to each random step.</summary>
    private readonly double _multiplier = multiplier;

    /// <summary>The current value of the walk.</summary>
    private double _lastNumber = firstValue;

    /// <summary>Advances the walk by one random step.</summary>
    /// <returns>The new value.</returns>
    public double Next() => _lastNumber += (_gen.RandomNumber() - HalfUnitInterval) * _multiplier;

    /// <summary>Generates the requested number of successive walk values.</summary>
    /// <param name="count">The number of values to generate.</param>
    /// <returns>The generated values.</returns>
    public IEnumerable<double> Next(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "The number of values cannot be negative.");
        }

        return GenerateValues(count);
    }

    /// <summary>Enumerates values from the walk.</summary>
    /// <param name="count">The number of values to generate.</param>
    /// <returns>The generated values.</returns>
    private IEnumerable<double> GenerateValues(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return Next();
        }
    }
}
