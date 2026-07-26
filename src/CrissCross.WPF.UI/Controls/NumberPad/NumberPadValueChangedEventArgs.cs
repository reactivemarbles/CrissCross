// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides data for number pad value changes.</summary>
/// <param name="userChanged">Whether the value was changed by the user.</param>
/// <param name="value">The current value.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class NumberPadValueChangedEventArgs(bool userChanged, double value) : EventArgs
{
    /// <summary>Gets a value indicating whether the value was changed by the user.</summary>
    public bool UserChanged { get; } = userChanged;

    /// <summary>Gets the current value.</summary>
    public double Value { get; } = value;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
