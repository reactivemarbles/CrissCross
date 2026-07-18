// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Provides scale-aware comparisons for floating-point values.</summary>
internal static class DoubleComparison
{
    /// <summary>The absolute comparison tolerance.</summary>
    private const double AbsoluteTolerance = 1E-10;

    /// <summary>The relative comparison tolerance.</summary>
    private const double RelativeTolerance = 1E-10;

    /// <summary>Determines whether two values are equal within a scale-aware tolerance.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns><see langword="true" /> when the values are sufficiently close.</returns>
    internal static bool AreClose(double left, double right)
    {
        if (left.Equals(right))
        {
            return true;
        }

        var difference = Math.Abs(left - right);
        var scale = Math.Max(Math.Abs(left), Math.Abs(right));
        return difference <= Math.Max(AbsoluteTolerance, RelativeTolerance * scale);
    }
}
