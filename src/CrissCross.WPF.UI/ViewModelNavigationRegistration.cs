// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Identifies the window and initial view model used by hosted view-model navigation.</summary>
/// <typeparam name="TWindow">The window type.</typeparam>
/// <typeparam name="TViewModel">The initial view-model type.</typeparam>
public sealed class ViewModelNavigationRegistration<TWindow, TViewModel>
{
    /// <summary>Gets the window type.</summary>
    public Type WindowType => typeof(TWindow);

    /// <summary>Gets the initial view-model type.</summary>
    public Type ViewModelType => typeof(TViewModel);
}
