// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Identifies the window and initial page used by hosted page navigation.</summary>
/// <typeparam name="TWindow">The window type.</typeparam>
/// <typeparam name="TPage">The initial page type.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class PageNavigationRegistration<TWindow, TPage>
{
    /// <summary>Gets the window type.</summary>
    public Type WindowType => typeof(TWindow);

    /// <summary>Gets the initial page type.</summary>
    public Type PageType => typeof(TPage);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
