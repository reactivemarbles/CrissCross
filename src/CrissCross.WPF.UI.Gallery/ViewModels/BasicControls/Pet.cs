// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using CP.Reactive.Collections;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Pet member.</summary>
/// <seealso cref="ReactiveTreeItem" />
public class Pet : ReactiveTreeItem
{
    /// <summary>Initializes a new instance of the <see cref="Pet"/> class.</summary>
    /// <param name="name">The name.</param>
    public Pet(string? name) => DisplayName = name;

    /// <summary>Gets the view model.</summary>
    /// <value>
    /// The view model.
    /// </value>
    public override object ViewModel => this;
}
