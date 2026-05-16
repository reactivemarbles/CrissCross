// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace CrissCross;

/// <summary>
/// Represents platform-neutral theme preference state for theme picker controls.
/// </summary>
public sealed class ThemePreferenceState
{
    private static readonly IReadOnlyList<ThemeChoice> ChoicesWithHighContrast = Array.AsReadOnly(
        [
            ThemeChoice.System,
            ThemeChoice.Light,
            ThemeChoice.Dark,
            ThemeChoice.HighContrast
        ]);

    private static readonly IReadOnlyList<ThemeChoice> ChoicesWithoutHighContrast = Array.AsReadOnly(
        [
            ThemeChoice.System,
            ThemeChoice.Light,
            ThemeChoice.Dark
        ]);

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemePreferenceState"/> class.
    /// </summary>
    /// <param name="selectedChoice">The user-selected theme preference.</param>
    /// <param name="systemChoice">The current concrete system theme.</param>
    /// <param name="supportsHighContrast">A value indicating whether high contrast is supported by the current platform.</param>
    public ThemePreferenceState(ThemeChoice selectedChoice, ThemeChoice systemChoice = ThemeChoice.Light, bool supportsHighContrast = true)
    {
        SelectedChoice = selectedChoice;
        SystemChoice = NormalizeConcreteChoice(systemChoice, supportsHighContrast);
        SupportsHighContrast = supportsHighContrast;
        AvailableChoices = supportsHighContrast ? ChoicesWithHighContrast : ChoicesWithoutHighContrast;
        EffectiveChoice = ResolveEffectiveChoice(selectedChoice, SystemChoice, supportsHighContrast);
    }

    /// <summary>
    /// Gets the user-selected theme preference.
    /// </summary>
    public ThemeChoice SelectedChoice { get; }

    /// <summary>
    /// Gets the concrete current system theme.
    /// </summary>
    public ThemeChoice SystemChoice { get; }

    /// <summary>
    /// Gets the concrete theme that should be applied for the current preference.
    /// </summary>
    public ThemeChoice EffectiveChoice { get; }

    /// <summary>
    /// Gets a value indicating whether the current platform supports high contrast.
    /// </summary>
    public bool SupportsHighContrast { get; }

    /// <summary>
    /// Gets the theme choices that should be offered by a theme picker.
    /// </summary>
    public IReadOnlyList<ThemeChoice> AvailableChoices { get; }

    /// <summary>
    /// Gets a value indicating whether the selected preference follows the system theme.
    /// </summary>
    public bool IsSystemSelected => SelectedChoice == ThemeChoice.System;

    /// <summary>
    /// Gets a value indicating whether high contrast is the effective concrete theme.
    /// </summary>
    public bool IsHighContrastEffective => EffectiveChoice == ThemeChoice.HighContrast;

    /// <summary>
    /// Gets compact user-facing text for the theme preference.
    /// </summary>
    public string DisplayText
    {
        get
        {
            if (IsSystemSelected)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "System ({0})", FormatChoice(EffectiveChoice));
            }

            if (SelectedChoice == ThemeChoice.HighContrast && !SupportsHighContrast)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "High contrast (using {0})", FormatChoice(EffectiveChoice));
            }

            return FormatChoice(SelectedChoice);
        }
    }

    /// <summary>
    /// Determines whether the specified choice is available on the current platform.
    /// </summary>
    /// <param name="choice">The choice to test.</param>
    /// <returns><c>true</c> when the theme choice is supported; otherwise, <c>false</c>.</returns>
    public bool SupportsChoice(ThemeChoice choice) => choice != ThemeChoice.HighContrast || SupportsHighContrast;

    private static ThemeChoice ResolveEffectiveChoice(ThemeChoice selectedChoice, ThemeChoice systemChoice, bool supportsHighContrast)
    {
        if (selectedChoice == ThemeChoice.System)
        {
            return systemChoice;
        }

        if (selectedChoice == ThemeChoice.HighContrast && !supportsHighContrast)
        {
            return systemChoice;
        }

        return NormalizeConcreteChoice(selectedChoice, supportsHighContrast);
    }

    private static ThemeChoice NormalizeConcreteChoice(ThemeChoice choice, bool supportsHighContrast)
    {
        if (choice == ThemeChoice.Dark || choice == ThemeChoice.Light)
        {
            return choice;
        }

        if (choice == ThemeChoice.HighContrast && supportsHighContrast)
        {
            return ThemeChoice.HighContrast;
        }

        return ThemeChoice.Light;
    }

    private static string FormatChoice(ThemeChoice choice) => choice switch
    {
        ThemeChoice.Dark => "Dark",
        ThemeChoice.Light => "Light",
        ThemeChoice.HighContrast => "High contrast",
        _ => "System"
    };
}
