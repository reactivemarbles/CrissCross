// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Appearance;

/// <summary>Provides data for application theme changes.</summary>
/// <param name="currentApplicationTheme">Current application <see cref="ApplicationTheme"/>.</param>
/// <param name="systemAccent">Current base system accent <see cref="Color"/>.</param>
public sealed class ThemeChangedEventArgs(ApplicationTheme currentApplicationTheme, Color systemAccent) : EventArgs
{
    /// <summary>Gets the current application theme.</summary>
    public ApplicationTheme CurrentApplicationTheme { get; } = currentApplicationTheme;

    /// <summary>Gets the current base system accent color.</summary>
    public Color SystemAccent { get; } = systemAccent;
}
