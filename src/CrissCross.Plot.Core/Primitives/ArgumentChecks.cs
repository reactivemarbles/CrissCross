// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides argument guards compatible with all supported target frameworks.</summary>
internal static class ArgumentChecks
{
    /// <summary>Throws when a reference argument is null.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the argument.</param>
    internal static void ThrowIfNull(object? value, string parameterName)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
    }

    /// <summary>Throws when an integer argument is negative.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the argument.</param>
    internal static void ThrowIfNegative(int value, string parameterName)
    {
        if (value >= 0)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(parameterName, value, "Value must be non-negative.");
    }

    /// <summary>Throws when an integer argument is greater than or equal to an upper bound.</summary>
    /// <param name="value">The value to check.</param>
    /// <param name="upperBound">The exclusive upper bound.</param>
    /// <param name="parameterName">The name of the argument.</param>
    internal static void ThrowIfGreaterThanOrEqual(int value, int upperBound, string parameterName)
    {
        if (value < upperBound)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be less than {upperBound}.");
    }
}
