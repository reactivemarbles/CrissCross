// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Defines common date/time range preset identifiers used by range picker controls.</summary>
public enum DateTimeRangePreset
{
    /// <summary>The range was manually selected by the user.</summary>
    Custom,

    /// <summary>The current day from local midnight through the reference time.</summary>
    Today,

    /// <summary>The previous calendar day.</summary>
    Yesterday,

    /// <summary>The trailing seven-day range ending at the reference time.</summary>
    LastSevenDays,

    /// <summary>The current month from the first day through the reference time.</summary>
    ThisMonth
}
