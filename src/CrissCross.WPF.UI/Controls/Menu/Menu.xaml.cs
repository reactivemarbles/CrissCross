// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Styles.Controls;
#else
namespace CrissCross.WPF.UI.Styles.Controls;
#endif

/// <summary>Extension to the menu.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class Menu : ResourceDictionary
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
