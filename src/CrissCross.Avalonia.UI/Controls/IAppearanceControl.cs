// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Defines a contract for controls that support customizable appearance settings.</summary>
public interface IAppearanceControl
{
    /// <summary>Gets or sets the <see cref="Appearance"/> of the control, if available.</summary>
    ControlAppearance Appearance { get; set; }
}
