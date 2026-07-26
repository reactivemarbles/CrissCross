// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls;

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
