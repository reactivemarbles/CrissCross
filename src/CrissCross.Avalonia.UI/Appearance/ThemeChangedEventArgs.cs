// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Appearance;
#else
namespace CrissCross.Avalonia.UI.Appearance;
#endif

/// <summary>Provides data for application theme changes.</summary>
/// <param name="currentTheme">The current application theme.</param>
/// <param name="accentColor">The current accent color.</param>
public sealed class ThemeChangedEventArgs(ApplicationTheme currentTheme, Color accentColor) : EventArgs
{
    /// <summary>Gets the current application theme.</summary>
    public ApplicationTheme CurrentTheme { get; } = currentTheme;

    /// <summary>Gets the current accent color.</summary>
    public Color AccentColor { get; } = accentColor;
}
