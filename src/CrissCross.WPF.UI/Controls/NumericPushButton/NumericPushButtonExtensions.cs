// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

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
