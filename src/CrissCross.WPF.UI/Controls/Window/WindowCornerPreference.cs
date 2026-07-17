// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Ways you can round windows.</summary>
public enum WindowCornerPreference
{
    /// <summary>Determined by system or application preference.</summary>
    Default,

    /// <summary>Do not round the corners.</summary>
    DoNotRound,

    /// <summary>Round the corners.</summary>
    Round,

    /// <summary>Round the corners slightly.</summary>
    RoundSmall,
}
