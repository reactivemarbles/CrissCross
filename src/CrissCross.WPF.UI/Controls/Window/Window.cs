﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Window.
/// </summary>
/// <seealso cref="System.Windows.Window" />
/// <seealso cref="ICanShowMessages" />
public class Window : System.Windows.Window, ICanShowMessages
{
    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>
    /// The owner.
    /// </value>
    string ICanShowMessages.Owner => Name;
}
