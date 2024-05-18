// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
