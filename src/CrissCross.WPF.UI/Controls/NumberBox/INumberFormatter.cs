// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// An interface that returns a string representation of a provided value, using distinct format methods to format several data types.
/// </summary>
public interface INumberFormatter
{
    /// <summary>
    /// Returns a string representation of a <see cref="double" /> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    string FormatDouble(double? value);

    /// <summary>
    /// Returns a string representation of an <see cref="int" /> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    string FormatInt(int? value);

    /// <summary>
    /// Returns a string representation of a <see cref="uint" /> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    string FormatUInt(uint? value);
}
