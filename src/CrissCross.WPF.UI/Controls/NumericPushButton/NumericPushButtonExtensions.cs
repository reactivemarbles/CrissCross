// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NumericPushButtonExtensions.</summary>
public static class NumericPushButtonExtensions
{
    /// <summary>Provides extension members.</summary>
    /// <param name="this">The extension value.</param>
    extension(NumericPushButton? @this)
    {
        /// <summary>Updates the content of the spin button.</summary>
        /// <returns>A Value.</returns>
        public NumericPushButton? UpdateSpinButtonContent()
        {
            if (@this is not null)
            {
                var breakLine = @this.UnitsOnNewLine ? "\r\n" : " ";
                @this.Content = $"{Math.Round(@this.Value, @this.DecimalPlaces)}{breakLine}{@this.Units}";
            }

            return @this;
        }
    }
}
