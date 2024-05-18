// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media;

namespace CrissCross.WPF.UI.Test.Models;

/// <summary>
/// DataColor.
/// </summary>
public record struct DataColor
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public Brush Color { get; set; }
}
