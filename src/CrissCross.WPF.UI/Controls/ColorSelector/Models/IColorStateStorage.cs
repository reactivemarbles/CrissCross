// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// IColorStateStorage.
/// </summary>
public interface IColorStateStorage
{
    /// <summary>
    /// Gets or sets the state of the color.
    /// </summary>
    /// <value>
    /// The state of the color.
    /// </value>
    ColorState ColorState { get; set; }
}
