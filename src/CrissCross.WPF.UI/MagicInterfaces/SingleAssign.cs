// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Single Assign Single Assign.</summary>
/// <typeparam name="T">The Type.</typeparam>
public class SingleAssign<T>
{
    /// <summary>Stores the _assigned value.</summary>
    private bool _assigned;

    /// <summary>Gets the value.</summary>
    /// <value>
    /// The value.
    /// </value>
    public T? Value { get; private set; }

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
