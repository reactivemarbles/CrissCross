// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls
{
    /// <summary>
    /// NumericPushButtonExtensions.
    /// </summary>
    public static class NumericPushButtonExtensions
    {
        /// <summary>
        /// Updates the content of the spin button.
        /// </summary>
        /// <param name="this">The @this.</param>
        /// <returns>A Value.</returns>
        public static NumericPushButton? UpdateSpinButtonContent(this NumericPushButton? @this)
        {
            if (@this == null)
            {
                return @this;
            }

            var breakLine = @this.UnitsOnNewLine ? "\r\n" : " ";
            @this.Content = $"{Math.Round(@this.Value, @this.DecimalPlaces)}{breakLine}{@this.Units}";
            return @this;
        }
    }
}
