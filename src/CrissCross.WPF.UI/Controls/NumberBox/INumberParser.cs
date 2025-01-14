// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// An interface that parses a string representation of a numeric value.
/// </summary>
public interface INumberParser
{
    /// <summary>
    /// Attempts to parse a string representation of a <see cref="double" /> numeric value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A double.</returns>
    double? ParseDouble(string? value);

    /// <summary>
    /// Attempts to parse a string representation of an <see cref="int" /> numeric value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A int.</returns>
    int? ParseInt(string? value);

    /// <summary>
    /// Attempts to parse a string representation of an <see cref="uint" /> numeric value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A uint.</returns>
    uint? ParseUInt(string? value);
}
