// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media;

namespace CrissCross.WPF.UI.Test.Models;

/// <summary>DataColor member.</summary>
public readonly record struct DataColor
{
    /// <summary>Gets or sets the color.</summary>
    /// <value>
    /// The color.
    /// </value>
    public Brush Color { get; init; }
}
