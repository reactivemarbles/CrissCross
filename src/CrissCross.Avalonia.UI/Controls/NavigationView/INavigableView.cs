// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Interface for views that can be navigated to.</summary>
/// <typeparam name="T">The T type.</typeparam>
public interface INavigableView<out T>
{
    /// <summary>Gets the view model.</summary>
    T ViewModel { get; }
}
