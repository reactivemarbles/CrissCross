// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Appearance;
#else
namespace CrissCross.WPF.UI.Appearance;
#endif

/// <summary>Provides data for application theme changes.</summary>
/// <param name="currentApplicationTheme">Current application <see cref="ApplicationTheme"/>.</param>
/// <param name="systemAccent">Current base system accent <see cref="Color"/>.</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ThemeChangedEventArgs(ApplicationTheme currentApplicationTheme, Color systemAccent) : EventArgs
{
    /// <summary>Gets the current application theme.</summary>
    public ApplicationTheme CurrentApplicationTheme { get; } = currentApplicationTheme;

    /// <summary>Gets the current base system accent color.</summary>
    public Color SystemAccent { get; } = systemAccent;

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
