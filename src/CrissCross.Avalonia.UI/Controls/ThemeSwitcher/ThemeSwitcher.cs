// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.UI.Appearance;
#else
using CrissCross.Avalonia.UI.Appearance;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a theme preference picker over the platform theme service.</summary>
public class ThemeSwitcher : TemplatedControl
{
    /// <summary>Property for <see cref="SelectedChoice"/>.</summary>
    public static readonly StyledProperty<ThemeChoice> SelectedChoiceProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        ThemeChoice
    >(nameof(SelectedChoice), ThemeChoice.System, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="SystemChoice"/>.</summary>
    public static readonly StyledProperty<ThemeChoice> SystemChoiceProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        ThemeChoice
    >(nameof(SystemChoice), ThemeChoice.Light);

    /// <summary>Property for <see cref="SupportsHighContrast"/>.</summary>
    public static readonly StyledProperty<bool> SupportsHighContrastProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        bool
    >(nameof(SupportsHighContrast), true);

    /// <summary>Property for <see cref="CurrentState"/>.</summary>
    public static readonly StyledProperty<ThemePreferenceState?> CurrentStateProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        ThemePreferenceState?
    >(nameof(CurrentState));

    /// <summary>Property for <see cref="ThemeService"/>.</summary>
    public static readonly StyledProperty<IThemeService?> ThemeServiceProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        IThemeService?
    >(nameof(ThemeService));

    /// <summary>Property for <see cref="ThemeChangedCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> ThemeChangedCommandProperty = AvaloniaProperty.Register<
        ThemeSwitcher,
        ICommand?
    >(nameof(ThemeChangedCommand));

    /// <summary>Initializes a new instance of the <see cref="ThemeSwitcher"/> class.</summary>
    public ThemeSwitcher()
    {
        ApplyThemeCommand = new ThemeSwitcherCommand(ApplyChoice);
        CurrentState = CreateState();
    }

    /// <summary>Gets or sets the selected theme preference.</summary>
    public ThemeChoice SelectedChoice
    {
        get => GetValue(SelectedChoiceProperty);
        set => SetValue(SelectedChoiceProperty, value);
    }

    /// <summary>Gets or sets the current concrete system theme.</summary>
    public ThemeChoice SystemChoice
    {
        get => GetValue(SystemChoiceProperty);
        set => SetValue(SystemChoiceProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether high contrast should be offered.</summary>
    public bool SupportsHighContrast
    {
        get => GetValue(SupportsHighContrastProperty);
        set => SetValue(SupportsHighContrastProperty, value);
    }

    /// <summary>Gets or sets the current resolved theme preference state.</summary>
    public ThemePreferenceState? CurrentState
    {
        get => GetValue(CurrentStateProperty);
        set => SetValue(CurrentStateProperty, value);
    }

    /// <summary>Gets or sets the optional platform theme service used when applying a choice.</summary>
    public IThemeService? ThemeService
    {
        get => GetValue(ThemeServiceProperty);
        set => SetValue(ThemeServiceProperty, value);
    }

    /// <summary>Gets or sets a command invoked with the selected theme choice after a theme is applied.</summary>
    public ICommand? ThemeChangedCommand
    {
        get => GetValue(ThemeChangedCommandProperty);
        set => SetValue(ThemeChangedCommandProperty, value);
    }

    /// <summary>Gets the command that applies a selected theme choice.</summary>
    public ICommand ApplyThemeCommand { get; }

    /// <summary>Creates a platform-neutral theme preference state from the current control inputs.</summary>
    /// <returns>The resolved preference state.</returns>
    public ThemePreferenceState CreateState() => new(SelectedChoice, SystemChoice, SupportsHighContrast);

    /// <summary>Applies a theme choice and emits the selected choice.</summary>
    /// <param name="choice">The requested theme choice or name.</param>
    public void ApplyChoice(object? choice)
    {
        var themeService = ThemeService ?? new ThemeService();
        var selectedChoice = ParseChoice(choice);
        SystemChoice = ToThemeChoice(themeService.GetSystemTheme());
        SelectedChoice = selectedChoice;

        var state = CreateState();
        CurrentState = state;

        ApplyTheme(themeService, state.EffectiveChoice);

        if (ThemeChangedCommand?.CanExecute(selectedChoice) != true)
        {
            return;
        }

        ThemeChangedCommand.Execute(selectedChoice);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (
            change.Property != SelectedChoiceProperty
            && change.Property != SystemChoiceProperty
            && change.Property != SupportsHighContrastProperty)
        {
            return;
        }

        CurrentState = CreateState();
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

        return Enum.TryParse(
            Convert.ToString(value, CultureInfo.InvariantCulture),
            ignoreCase: true,
            out ThemeChoice parsed)
            ? parsed
            : ThemeChoice.System;
    }

    /// <summary>Provides the ApplyTheme member.</summary>
    /// <param name="themeService">The themeService value.</param>
    /// <param name="effectiveChoice">The effectiveChoice value.</param>
    private static void ApplyTheme(IThemeService? themeService, ThemeChoice effectiveChoice)
    {
        if (themeService is null)
        {
            return;
        }

        _ = themeService.SetTheme(ToApplicationTheme(effectiveChoice));
    }

    /// <summary>Provides the ToApplicationTheme member.</summary>
    /// <param name="choice">The choice value.</param>
    /// <returns>The result.</returns>
    private static ApplicationTheme ToApplicationTheme(ThemeChoice choice) =>
        choice switch
        {
            ThemeChoice.Dark => ApplicationTheme.Dark,
            ThemeChoice.HighContrast => ApplicationTheme.HighContrast,
            _ => ApplicationTheme.Light,
        };

    /// <summary>Provides the ToThemeChoice member.</summary>
    /// <param name="theme">The theme value.</param>
    /// <returns>The result.</returns>
    private static ThemeChoice ToThemeChoice(ApplicationTheme theme) =>
        theme switch
        {
            ApplicationTheme.Dark => ThemeChoice.Dark,
            ApplicationTheme.HighContrast => ThemeChoice.HighContrast,
            _ => ThemeChoice.Light,
        };

    /// <summary>Provides the ThemeSwitcherCommand member.</summary>
    private sealed class ThemeSwitcherCommand : ICommand
    {
        /// <summary>Provides the documented member.</summary>
        private readonly Action<object?> _execute;

        /// <summary>Initializes a new instance of the <see cref="ThemeSwitcherCommand"/> class.</summary>
        /// <param name="execute">The execute value.</param>
        public ThemeSwitcherCommand(Action<object?> execute) => _execute = execute;

        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
