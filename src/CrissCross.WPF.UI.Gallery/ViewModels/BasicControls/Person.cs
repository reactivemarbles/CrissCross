// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using CP.Reactive.Collections;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// Person.
/// </summary>
/// <seealso cref="ReactiveTreeItem" />
public class Person
    : ReactiveTreeItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="children">The children.</param>
    public Person(string? name, IEnumerable<ReactiveTreeItem>? children = null)
        : base(children) => DisplayName = name;

    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>
    /// The view model.
    /// </value>
    public override object ViewModel => this;
}
