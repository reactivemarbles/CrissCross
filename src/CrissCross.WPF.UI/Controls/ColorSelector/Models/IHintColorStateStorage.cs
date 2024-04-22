// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// IHintColorStateStorage.
/// </summary>
public interface IHintColorStateStorage
{
    /// <summary>
    /// Gets or sets the state of the hint color.
    /// </summary>
    /// <value>
    /// The state of the hint color.
    /// </value>
    ColorState HintColorState { get; set; }
}
