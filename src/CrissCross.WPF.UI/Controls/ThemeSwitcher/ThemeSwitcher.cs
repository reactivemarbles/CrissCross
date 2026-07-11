// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;
using CrissCross.WPF.UI.Appearance;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents a theme preference picker over the platform theme service.</summary>
public class ThemeSwitcher : Control
{
    /// <summary>Property for <see cref="SelectedChoice"/>.</summary>
    public static readonly DependencyProperty SelectedChoiceProperty = DependencyProperty.Register(
        nameof(SelectedChoice),
        typeof(ThemeChoice),
        typeof(ThemeSwitcher),
        new FrameworkPropertyMetadata(ThemeChoice.System, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnThemeInputChanged));

    /// <summary>Property for <see cref="SystemChoice"/>.</summary>
    public static readonly DependencyProperty SystemChoiceProperty = DependencyProperty.Register(
        nameof(SystemChoice),
        typeof(ThemeChoice),
        typeof(ThemeSwitcher),
        new PropertyMetadata(ThemeChoice.Light, OnThemeInputChanged));

    /// <summary>Property for <see cref="SupportsHighContrast"/>.</summary>
    public static readonly DependencyProperty SupportsHighContrastProperty = DependencyProperty.Register(
        nameof(SupportsHighContrast),
        typeof(bool),
        typeof(ThemeSwitcher),
        new PropertyMetadata(true, OnThemeInputChanged));

    /// <summary>Property for <see cref="CurrentState"/>.</summary>
    public static readonly DependencyProperty CurrentStateProperty = DependencyProperty.Register(
        nameof(CurrentState),
        typeof(ThemePreferenceState),
        typeof(ThemeSwitcher),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ThemeService"/>.</summary>
    public static readonly DependencyProperty ThemeServiceProperty = DependencyProperty.Register(
        nameof(ThemeService),
        typeof(IThemeService),
        typeof(ThemeSwitcher),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="ThemeChangedCommand"/>.</summary>
    public static readonly DependencyProperty ThemeChangedCommandProperty = DependencyProperty.Register(
        nameof(ThemeChangedCommand),
        typeof(ICommand),
        typeof(ThemeSwitcher),
        new PropertyMetadata(null));

    /// <summary>Initializes a new instance of the <see cref="ThemeSwitcher"/> class.</summary>
    public ThemeSwitcher()
    {
        ApplyThemeCommand = new ThemeSwitcherCommand(ApplyChoice);
        CurrentState = CreateState();
    }

    /// <summary>Gets or sets the selected theme preference.</summary>
    public ThemeChoice SelectedChoice
    {
        get => (ThemeChoice)GetValue(SelectedChoiceProperty);
        set => SetValue(SelectedChoiceProperty, value);
    }

    /// <summary>Gets or sets the current concrete system theme.</summary>
    public ThemeChoice SystemChoice
    {
        get => (ThemeChoice)GetValue(SystemChoiceProperty);
        set => SetValue(SystemChoiceProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether high contrast should be offered.</summary>
    public bool SupportsHighContrast
    {
        get => (bool)GetValue(SupportsHighContrastProperty);
        set => SetValue(SupportsHighContrastProperty, value);
    }

    /// <summary>Gets or sets the current resolved theme preference state.</summary>
    public ThemePreferenceState? CurrentState
    {
        get => (ThemePreferenceState?)GetValue(CurrentStateProperty);
        set => SetValue(CurrentStateProperty, value);
    }

    /// <summary>Gets or sets the optional platform theme service used when applying a choice.</summary>
    public IThemeService? ThemeService
    {
        get => (IThemeService?)GetValue(ThemeServiceProperty);
        set => SetValue(ThemeServiceProperty, value);
    }

    /// <summary>Gets or sets a command invoked with the selected theme choice after a theme is applied.</summary>
    public ICommand? ThemeChangedCommand
    {
        get => (ICommand?)GetValue(ThemeChangedCommandProperty);
        set => SetValue(ThemeChangedCommandProperty, value);
    }

    /// <summary>Gets the command that applies a selected theme choice.</summary>
    public ICommand ApplyThemeCommand { get; }

    /// <summary>Creates a platform-neutral theme preference state from the current control inputs.</summary>
    /// <returns>The resolved preference state.</returns>
    public ThemePreferenceState CreateState() => new(SelectedChoice, SystemChoice, SupportsHighContrast);

    /// <summary>Applies a theme choice and emits the current state.</summary>
    /// <param name="choice">The requested theme choice or name.</param>
    public void ApplyChoice(object? choice)
    {
        var themeService = ThemeService ?? new ThemeService();
        var selectedChoice = ParseChoice(choice);
        SetCurrentValue(SystemChoiceProperty, ToThemeChoice(themeService.GetSystemTheme()));
        SetCurrentValue(SelectedChoiceProperty, selectedChoice);

        var state = CreateState();
        SetCurrentValue(CurrentStateProperty, state);

        ApplyTheme(themeService, selectedChoice, state.EffectiveChoice);

        if (ThemeChangedCommand?.CanExecute(selectedChoice) != true)
        {
            return;
        }

        ThemeChangedCommand.Execute(selectedChoice);
    }

    /// <summary>Provides the OnThemeInputChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="_">Unused event arguments required by the dependency property callback.</param>
    private static void OnThemeInputChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs _)
    {
        if (dependencyObject is not ThemeSwitcher switcher)
        {
            return;
        }

        switcher.SetCurrentValue(CurrentStateProperty, switcher.CreateState());
    }

    /// <summary>Provides the ParseChoice member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private static ThemeChoice ParseChoice(object? value)
    {
        if (value is ThemeChoice choice)
        {
            return choice;
        }

        return Enum.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), ignoreCase: true, out ThemeChoice parsed)
            ? parsed
            : ThemeChoice.System;
    }

    /// <summary>Provides the ApplyTheme member.</summary>
    /// <param name="themeService">The themeService value.</param>
    /// <param name="selectedChoice">The selectedChoice value.</param>
    /// <param name="effectiveChoice">The effectiveChoice value.</param>
    private static void ApplyTheme(IThemeService themeService, ThemeChoice selectedChoice, ThemeChoice effectiveChoice) =>
        themeService.SetTheme(
            selectedChoice == ThemeChoice.System
                ? themeService.GetSystemTheme()
                : ToApplicationTheme(effectiveChoice));

    /// <summary>Provides the ToApplicationTheme member.</summary>
    /// <param name="choice">The choice value.</param>
    /// <returns>The result.</returns>
    private static ApplicationTheme ToApplicationTheme(ThemeChoice choice) => choice switch
    {
        ThemeChoice.Dark => ApplicationTheme.Dark,
        ThemeChoice.HighContrast => ApplicationTheme.HighContrast,
        _ => ApplicationTheme.Light
    };

    /// <summary>Provides the ToThemeChoice member.</summary>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result.</returns>
    private static ThemeChoice ToThemeChoice(ApplicationTheme theme) => theme switch
    {
        ApplicationTheme.Dark => ThemeChoice.Dark,
        ApplicationTheme.HighContrast => ThemeChoice.HighContrast,
        _ => ThemeChoice.Light
    };

    /// <summary>Provides the ThemeSwitcherCommand member.</summary>
    private sealed class ThemeSwitcherCommand : ICommand
    {
        /// <summary>Stores the _execute value.</summary>
        private readonly Action<object?> _execute;

        /// <summary>Initializes a new instance of the <see cref="ThemeSwitcherCommand"/> class.</summary>
        /// <param name="execute">The execute value.</param>
        public ThemeSwitcherCommand(Action<object?> execute) => _execute = execute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
