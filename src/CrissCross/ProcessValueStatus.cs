// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Describes the displayed condition of a process measurement.</summary>
public enum ProcessValueStatus
{
    /// <summary>The measurement is valid and within its alarm limits.</summary>
    Normal,

    /// <summary>The measurement is at or below its low alarm limit.</summary>
    LowAlarm,

    /// <summary>The measurement is at or above its high alarm limit.</summary>
    HighAlarm,

    /// <summary>The measurement is unavailable, non-finite, or reported with bad quality.</summary>
    BadQuality,

    /// <summary>The display range or alarm limits are invalid.</summary>
    InvalidConfiguration,
}
