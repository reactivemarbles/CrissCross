// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// UI <see cref="Avalonia.Controls.Control"/> with <see cref="ControlAppearance"/> attributes.
/// </summary>
public interface IAppearanceControl
{
    /// <summary>
    /// Gets or sets the <see cref="Appearance"/> of the control, if available.
    /// </summary>
    public ControlAppearance Appearance { get; set; }
}
