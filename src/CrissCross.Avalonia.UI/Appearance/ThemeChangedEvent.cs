// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Appearance;

/// <summary>
/// Event delegate for theme changes.
/// </summary>
/// <param name="currentTheme">The current application theme.</param>
/// <param name="accentColor">The current accent color.</param>
public delegate void ThemeChangedEvent(ApplicationTheme currentTheme, Color accentColor);
