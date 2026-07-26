// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Single Assign Single Assign.</summary>
/// <typeparam name="T">The Type.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SingleAssign<T>
{
    /// <summary>Stores the _assigned value.</summary>
    private bool _assigned;

    /// <summary>Gets the value.</summary>
    /// <value>
    /// The value.
    /// </value>
    public T? Value { get; private set; }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Assigns the specified value.</summary>
    /// <param name="value">The value.</param>
    public void Assign(T? value)
    {
        if (_assigned)
        {
            return;
        }

        Value = value;
        _assigned = true;
    }
}
