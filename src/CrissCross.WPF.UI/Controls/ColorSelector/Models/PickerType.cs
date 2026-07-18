// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Represents PickerType.</summary>
public enum PickerType
{
    /// <summary>The HSV.</summary>
    HSV = 0,

    /// <summary>The HSL.</summary>
    HSL = 1,
}
