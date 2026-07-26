// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents Window.</summary>
/// <seealso cref="System.Windows.Window" />
/// <seealso cref="ICanShowMessages" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Window : System.Windows.Window, ICanShowMessages
{
    /// <summary>Gets the owner.</summary>
    /// <value>
    /// The owner.
    /// </value>
    string ICanShowMessages.Owner => Name;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
