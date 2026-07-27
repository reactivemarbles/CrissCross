// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents GelButton.</summary>
/// <seealso cref="CommonButtonBase" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class GelButton : CommonButtonBase
{
    /// <summary>Initializes a new instance of the <see cref="GelButton"/> class.</summary>
    public GelButton()
        : base(nameof(GelButton)) { }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
