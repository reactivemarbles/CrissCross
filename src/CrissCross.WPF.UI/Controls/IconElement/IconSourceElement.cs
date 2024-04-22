// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Windows.Markup;
using CrissCross.WPF.UI.Converters;

// ReSharper disable once CheckNamespace
namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents an icon that uses an IconSource as its content.
/// </summary>
[ContentProperty(nameof(IconSource))]
public class IconSourceElement : IconElement
{
    /// <summary>
    /// Property for <see cref="IconSource"/>.
    /// </summary>
    public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
        nameof(IconSource),
        typeof(IconSource),
        typeof(IconSourceElement),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets or sets <see cref="IconSource"/>.
    /// </summary>
    public IconSource? IconSource
    {
        get => (IconSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Initializes the children.
    /// </summary>
    /// <returns>
    /// A UIElement.
    /// </returns>
    /// <exception cref="InvalidOperationException">Use {nameof(IconSourceElementConverter)} class.</exception>
    protected override UIElement InitializeChildren() => // TODO come up with an elegant solution
        throw new InvalidOperationException($"Use {nameof(IconSourceElementConverter)} class.");
}
