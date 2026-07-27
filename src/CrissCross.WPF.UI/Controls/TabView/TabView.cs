// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>
/// The TabView control is a way to display a set of tabs and their respective content.
/// Tab controls are useful for displaying several pages (or documents) of content while
/// giving a user the capability to rearrange, open, or close new tabs.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TabView : System.Windows.Controls.TabControl
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
