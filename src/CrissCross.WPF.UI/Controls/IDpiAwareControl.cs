// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>The control that should react to changes in the screen DPI.</summary>
public interface IDpiAwareControl
{
    /// <summary>Gets the current window display dpi.</summary>
    /// <value>
    /// The current window display dpi.
    /// </value>
    DisplayDpi CurrentWindowDisplayDpi { get; }
}
