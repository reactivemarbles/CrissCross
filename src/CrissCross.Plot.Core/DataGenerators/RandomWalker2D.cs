// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataGenerators;

/// <summary>Generates a two-dimensional random walk.</summary>
/// <param name="seed">An optional random-number seed.</param>
public class RandomWalker2D(int? seed = null)
{
    /// <summary>Half of the unit interval used to center random steps.</summary>
    private const double HalfUnitInterval = 0.5;

    /// <summary>The current horizontal position.</summary>
    private double _x;

    /// <summary>The current vertical position.</summary>
    private double _y;

    /// <summary>The current horizontal velocity.</summary>
    private double _velocityX;

    /// <summary>The current vertical velocity.</summary>
    private double _velocityY;

    /// <summary>Gets the random-number generator used by the walk.</summary>
    private RandomDataGenerator Generator { get; } = new(seed);

    /// <summary>Advances the walk and returns its new position.</summary>
    /// <returns>The new two-dimensional position.</returns>
    public Coordinates Next()
    {
        _velocityX += Generator.RandomNumber() - HalfUnitInterval;
        _velocityY += Generator.RandomNumber() - HalfUnitInterval;

        double absoluteVelocityX = Math.Abs(_velocityX);
        if (absoluteVelocityX > 1)
        {
            _velocityX *= 1 - absoluteVelocityX;
        }

        double absoluteVelocityY = Math.Abs(_velocityY);
        if (absoluteVelocityY > 1)
        {
            _velocityY *= 1 - absoluteVelocityY;
        }

        _x += _velocityX;
        _y += _velocityY;

        return new(_x, _y);
    }

    /// <summary>Generates the requested number of successive positions.</summary>
    /// <param name="count">The number of positions to generate.</param>
    /// <returns>The generated positions.</returns>
    public IEnumerable<Coordinates> Next(int count)
    {
        ArgumentChecks.ThrowIfNegative(count, nameof(count));
        return GeneratePositions(count);
    }

    /// <summary>Enumerates positions from the walk.</summary>
    /// <param name="count">The number of positions to generate.</param>
    /// <returns>The generated positions.</returns>
    private IEnumerable<Coordinates> GeneratePositions(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return Next();
        }
    }
}
