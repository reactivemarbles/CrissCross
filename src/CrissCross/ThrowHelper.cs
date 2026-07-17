// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides guard helpers for target frameworks with different BCL guard API surfaces.</summary>
internal static class ThrowHelper
{
    /// <summary>Throws when a required value is null.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
    public static void ThrowIfNull(object? value, string paramName)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value, paramName);
#else
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(paramName);
#endif
    }

    /// <summary>Throws when a required string is null, empty, or whitespace.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
    public static void ThrowIfNullOrWhiteSpace(string? value, string paramName)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
#else
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        throw new ArgumentException("Value cannot be null or whitespace.", paramName);
#endif
    }
}
