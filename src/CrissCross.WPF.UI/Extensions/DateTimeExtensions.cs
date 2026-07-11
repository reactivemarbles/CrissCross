// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>A collection of several extensions to the <see cref="DateTime"/> class.</summary>
public static class DateTimeExtensions
{
    /// <summary>Provides the number of microseconds in a millisecond.</summary>
    private const long MicrosecondsPerMillisecond = 1000;

    /// <summary>Provides the Unix epoch in coordinated universal time.</summary>
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>Provides extension members.</summary>
    /// <param name="dateTime">The dateTime value.</param>
    extension(DateTime dateTime)
    {
        /// <summary>Gets the number of seconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.</summary>
        /// <returns>A long.</returns>
        public long GetTimestamp() =>
            (long)dateTime.Subtract(UnixEpoch).TotalSeconds;

        /// <summary>Gets the number of milliseconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.</summary>
        /// <returns>A long.</returns>
        public long GetMillisTimestamp() => // Should be 10^-3
            (long)dateTime.Subtract(UnixEpoch).TotalMilliseconds;

        /// <summary>Gets the number of microseconds that have elapsed since the Unix epoch, excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970.</summary>
        /// <returns>
        /// A long.
        /// </returns>
        public long GetMicroTimestamp() => // Should be 10^-6
            dateTime.Subtract(UnixEpoch).Ticks / (TimeSpan.TicksPerMillisecond / MicrosecondsPerMillisecond);
    }
}
