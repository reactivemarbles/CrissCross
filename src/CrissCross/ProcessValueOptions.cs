// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Configures the alarm limits and source quality copied into a process-value snapshot.</summary>
public sealed class ProcessValueOptions
{
    /// <summary>Gets or sets the optional inclusive low alarm limit.</summary>
    public double? LowAlarmLimit { get; set; }

    /// <summary>Gets or sets the optional inclusive high alarm limit.</summary>
    public double? HighAlarmLimit { get; set; }

    /// <summary>Gets or sets a value indicating whether the source reports good data quality.</summary>
    public bool IsGoodQuality { get; set; } = true;
}
