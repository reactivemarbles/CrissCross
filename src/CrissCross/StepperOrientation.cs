// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Defines the platform-neutral layout orientation for stepper and wizard progress controls.</summary>
public enum StepperOrientation
{
    /// <summary>Steps are arranged from left to right.</summary>
    Horizontal,

    /// <summary>Steps are arranged from top to bottom.</summary>
    Vertical,

    /// <summary>Steps use a compact layout suitable for narrow regions.</summary>
    Compact
}
