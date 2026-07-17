// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>A modern styled Window.</summary>
public class ModernWindow : Window
{
    /// <summary>Initializes a new instance of the <see cref="ModernWindow"/> class.</summary>
    public ModernWindow()
    {
        // Configure window for modern appearance
        WindowDecorations = global::Avalonia.Controls.WindowDecorations.BorderOnly;
        TransparencyLevelHint = [ WindowTransparencyLevel.AcrylicBlur];
    }
}
