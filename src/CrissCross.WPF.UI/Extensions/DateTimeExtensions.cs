// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// A collection of several extensions to the <see cref="DateTime"/> class.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the number of seconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>A long.</returns>
    public static long GetTimestamp(this DateTime dateTime) =>
        (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// Gets the number of milliseconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>A long.</returns>
    public static long GetMillisTimestamp(this DateTime dateTime) => // Should be 10^-3
        (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

    /// <summary>
    /// Gets the number of microseconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>
    /// A long.
    /// </returns>
    public static long GetMicroTimestamp(this DateTime dateTime) => // Should be 10^-6
        (long)dateTime.Subtract(new DateTime(1970, 1, 1)).Ticks / (TimeSpan.TicksPerMillisecond / 1000);
}
