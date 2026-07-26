// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Identifies the window and initial view model used by hosted view-model navigation.</summary>
/// <typeparam name="TWindow">The window type.</typeparam>
/// <typeparam name="TViewModel">The initial view-model type.</typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ViewModelNavigationRegistration<TWindow, TViewModel>
{
    /// <summary>Gets the window type.</summary>
    public Type WindowType => typeof(TWindow);

    /// <summary>Gets the initial view-model type.</summary>
    public Type ViewModelType => typeof(TViewModel);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
