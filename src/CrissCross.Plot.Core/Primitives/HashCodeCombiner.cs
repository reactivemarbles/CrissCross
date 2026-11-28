// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Combines value hashes without depending on framework-specific hash-code APIs.</summary>
internal static class HashCodeCombiner
{
    /// <summary>The multiplier used to mix component hash codes.</summary>
    private const int HashMultiplier = 31;

    /// <summary>Combines two values into one hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The combined hash code.</returns>
    internal static int Combine<T1, T2>(T1 value1, T2 value2) => Mix(GetHashCode(value1), GetHashCode(value2));

    /// <summary>Combines three values into one hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <returns>The combined hash code.</returns>
    internal static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3) => Mix(Combine(value1, value2), GetHashCode(value3));

    /// <summary>Combines five values into one hash code.</summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="value3">The third value.</param>
    /// <param name="value4">The fourth value.</param>
    /// <param name="value5">The fifth value.</param>
    /// <returns>The combined hash code.</returns>
    internal static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) =>
        Mix(Combine(value1, value2, value3), Combine(value4, value5));

    /// <summary>Gets the hash code of a possibly null value.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to hash.</param>
    /// <returns>The value's hash code or zero when it is null.</returns>
    private static int GetHashCode<T>(T value) => value is null ? 0 : EqualityComparer<T>.Default.GetHashCode(value);

    /// <summary>Combines two hash codes.</summary>
    /// <param name="first">The first hash code.</param>
    /// <param name="second">The second hash code.</param>
    /// <returns>The combined hash code.</returns>
    private static int Mix(int first, int second) => unchecked((first * HashMultiplier) + second);
}
