// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>UI <see cref="System.Windows.Controls.Control"/> with <see cref="ControlAppearance"/> attributes.</summary>
public interface IAppearanceControl
{
    /// <summary>Gets or sets the <see cref="Appearance"/> of the control, if available.</summary>
    ControlAppearance Appearance { get; set; }
}
