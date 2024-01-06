// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
