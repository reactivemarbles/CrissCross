// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Provides math extension members.</summary>
internal static class MathExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="value">The value.</param>
    extension(double value)
    {
        /// <summary>Provides the Clamp member.</summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The result.</returns>
        public double Clamp(double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        /// <summary>Provides the Mod member.</summary>
        /// <param name="m">The m value.</param>
        /// <returns>The result.</returns>
        public double Mod(double m) => ((value % m) + m) % m;
    }
}
