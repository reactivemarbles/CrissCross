// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Interface for views that can be navigated to.
/// </summary>
/// <typeparam name="T">The type of the view model.</typeparam>
public interface INavigableView<out T>
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    T ViewModel { get; }
}
