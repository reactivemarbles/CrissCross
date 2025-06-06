﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// AnimationStartedEventArgs.
/// </summary>
/// <seealso cref="System.Windows.RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="AnimationStartedEventArgs"/> class.
/// </remarks>
/// <param name="source">The source.</param>
public class AnimationStartedEventArgs(object source) : RoutedEventArgs(AnimationBehavior.AnimationStartedEvent, source);
