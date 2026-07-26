// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

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
