// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// This Source Code is partially based on the source code provided by the .NET Foundation.

// ReSharper disable once CheckNamespace
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
