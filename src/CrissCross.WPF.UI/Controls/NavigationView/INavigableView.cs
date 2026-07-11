// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>A component whose ViewModel is separate from the DataContext and can be navigated by <see cref="INavigationView" />.</summary>
/// <typeparam name="T">The T type.</typeparam>
public interface INavigableView<out T>
{
    /// <summary>Gets viewModel used by the view. Optionally, it may implement <see cref="INavigationAware"/> and be navigated by <see cref="INavigationView"/>.</summary>
    T ViewModel { get; }
}
