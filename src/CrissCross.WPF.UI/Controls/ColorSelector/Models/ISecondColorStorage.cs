// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// ISecondColorStorage.
/// </summary>
internal interface ISecondColorStorage
{
    /// <summary>
    /// Gets or sets the state of the second color.
    /// </summary>
    /// <value>
    /// The state of the second color.
    /// </value>
    ColorState SecondColorState { get; set; }
}
