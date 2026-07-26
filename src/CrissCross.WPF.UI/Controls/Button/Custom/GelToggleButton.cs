// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents GelToggleButton.</summary>
/// <seealso cref="CommonToggleButtonBase" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GelToggleButton : CommonToggleButtonBase
{
    /// <summary>Initializes a new instance of the <see cref="GelToggleButton"/> class.</summary>
    public GelToggleButton()
        : base(nameof(GelToggleButton)) { }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
