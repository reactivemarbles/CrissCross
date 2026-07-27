// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Provides guard helpers for target frameworks with different BCL guard API surfaces.</summary>
internal static class ThrowHelper
{
    /// <summary>Throws when a required value is null.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
#if NET6_0_OR_GREATER
    internal static void ThrowIfNull(object? value, string paramName) =>
        ArgumentNullException.ThrowIfNull(value, paramName);
#else
    internal static void ThrowIfNull(object? value, string paramName)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(paramName);
    }
#endif

    /// <summary>Throws when a required string is null, empty, or whitespace.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
#if NET8_0_OR_GREATER
    internal static void ThrowIfNullOrWhiteSpace(string? value, string paramName) =>
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
#else
    internal static void ThrowIfNullOrWhiteSpace(string? value, string paramName)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        throw new ArgumentException("Value cannot be null or whitespace.", paramName);
    }
#endif

    /// <summary>Throws when a required integer value is negative.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
#if NET6_0_OR_GREATER
    internal static void ThrowIfNegative(int value, string paramName) =>
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
    internal static void ThrowIfNegative(int value, string paramName)
    {
        if (value >= 0)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(paramName);
    }
#endif
}
