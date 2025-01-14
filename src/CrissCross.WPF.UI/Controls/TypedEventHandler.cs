// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a method that handles general events.
/// </summary>
/// <typeparam name="TSender">The type of the sender.</typeparam>
/// <typeparam name="TArgs">The type of the arguments.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="args">The arguments.</param>
public delegate void TypedEventHandler<in TSender, in TArgs>(TSender sender, TArgs args)
    where TSender : DependencyObject
    where TArgs : RoutedEventArgs;
