// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A modern styled Window.
/// </summary>
public class ModernWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModernWindow"/> class.
    /// </summary>
    public ModernWindow()
    {
        // Configure window for modern appearance
        SystemDecorations = SystemDecorations.BorderOnly;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.AcrylicBlur };
    }
}
