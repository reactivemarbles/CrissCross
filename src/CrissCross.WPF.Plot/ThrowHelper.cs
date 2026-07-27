// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Provides guard helpers for target frameworks with different BCL guard API surfaces.</summary>
internal static class ThrowHelper
{
    /// <summary>Throws when a required value is null.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
#if NET6_0_OR_GREATER
    internal static void ThrowIfNull([NotNull] object? value, string paramName) =>
        ArgumentNullException.ThrowIfNull(value, paramName);
#else
    internal static void ThrowIfNull([NotNull] object? value, string paramName)
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(paramName);
    }
#endif

    /// <summary>Throws when a required unsigned integer value is zero.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name.</param>
    /// <param name="message">The exception message to use when the value is zero.</param>
    internal static void ThrowIfZero(uint value, string paramName, string? message = null)
    {
        if (value != 0)
        {
            return;
        }

        throw new ArgumentOutOfRangeException(paramName, message);
    }
}
