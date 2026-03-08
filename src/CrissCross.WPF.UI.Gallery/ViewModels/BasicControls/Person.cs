// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Windows.Media.Imaging;
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
        : base(children)
    {
        DisplayName = name;
        Icon = new ImageIcon() { Height = 20, Width = 20, Source = new BitmapImage(new Uri("pack://application:,,,/CrissCross.WPF.UI.Gallery;component/Assets/ControlImages/AnimatedIcon.png")) };
    }

    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>
    /// The view model.
    /// </value>
    public override object ViewModel => this;
}
