// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Identifies the target type for a tracking configuration.</summary>
/// <typeparam name="T">The tracked type.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class TrackingRequest<T>
{
    /// <summary>Gets the tracked runtime type.</summary>
    public Type TargetType => typeof(T);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
