// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Interface for views that can be navigated to.</summary>
/// <typeparam name="T">The T type.</typeparam>
public interface INavigableView<out T>
{
    /// <summary>Gets the view model.</summary>
    T ViewModel { get; }
}
