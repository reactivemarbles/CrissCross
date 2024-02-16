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
