// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using CrissCross;
using CrissCross.Avalonia.UI.Appearance;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a theme preference picker over the platform theme service.
/// </summary>
public class ThemeSwitcher : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="SelectedChoice"/>.
    /// </summary>
    public static readonly StyledProperty<ThemeChoice> SelectedChoiceProperty = AvaloniaProperty.Register<ThemeSwitcher, ThemeChoice>(
        nameof(SelectedChoice),
        ThemeChoice.System,
        defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Property for <see cref="SystemChoice"/>.
    /// </summary>
    public static readonly StyledProperty<ThemeChoice> SystemChoiceProperty = AvaloniaProperty.Register<ThemeSwitcher, ThemeChoice>(
        nameof(SystemChoice),
        ThemeChoice.Light);

    /// <summary>
    /// Property for <see cref="SupportsHighContrast"/>.
    /// </summary>
    public static readonly StyledProperty<bool> SupportsHighContrastProperty = AvaloniaProperty.Register<ThemeSwitcher, bool>(
        nameof(SupportsHighContrast),
        true);

    /// <summary>
    /// Property for <see cref="CurrentState"/>.
    /// </summary>
    public static readonly StyledProperty<ThemePreferenceState?> CurrentStateProperty = AvaloniaProperty.Register<ThemeSwitcher, ThemePreferenceState?>(nameof(CurrentState));

    /// <summary>
    /// Property for <see cref="ThemeService"/>.
    /// </summary>
    public static readonly StyledProperty<IThemeService?> ThemeServiceProperty = AvaloniaProperty.Register<ThemeSwitcher, IThemeService?>(nameof(ThemeService));

    /// <summary>
    /// Property for <see cref="ThemeChangedCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> ThemeChangedCommandProperty = AvaloniaProperty.Register<ThemeSwitcher, ICommand?>(nameof(ThemeChangedCommand));

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSwitcher"/> class.
    /// </summary>
    public ThemeSwitcher()
    {
        ApplyThemeCommand = new ThemeSwitcherCommand(ApplyChoice);
        CurrentState = CreateState();
    }

    /// <summary>
    /// Gets or sets the selected theme preference.
    /// </summary>
    public ThemeChoice SelectedChoice
    {
        get => GetValue(SelectedChoiceProperty);
        set => SetValue(SelectedChoiceProperty, value);
    }

    /// <summary>
    /// Gets or sets the current concrete system theme.
    /// </summary>
    public ThemeChoice SystemChoice
    {
        get => GetValue(SystemChoiceProperty);
        set => SetValue(SystemChoiceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether high contrast should be offered.
    /// </summary>
    public bool SupportsHighContrast
    {
        get => GetValue(SupportsHighContrastProperty);
        set => SetValue(SupportsHighContrastProperty, value);
    }

    /// <summary>
    /// Gets or sets the current resolved theme preference state.
    /// </summary>
    public ThemePreferenceState? CurrentState
    {
        get => GetValue(CurrentStateProperty);
        set => SetValue(CurrentStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the optional platform theme service used when applying a choice.
    /// </summary>
    public IThemeService? ThemeService
    {
        get => GetValue(ThemeServiceProperty);
        set => SetValue(ThemeServiceProperty, value);
    }

    /// <summary>
    /// Gets or sets a command invoked with the current state after a theme is applied.
    /// </summary>
    public ICommand? ThemeChangedCommand
    {
        get => GetValue(ThemeChangedCommandProperty);
        set => SetValue(ThemeChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets the command that applies a selected theme choice.
    /// </summary>
    public ICommand ApplyThemeCommand { get; }

    /// <summary>
    /// Creates a platform-neutral theme preference state from the current control inputs.
    /// </summary>
    /// <returns>The resolved preference state.</returns>
    public ThemePreferenceState CreateState() => new(SelectedChoice, SystemChoice, SupportsHighContrast);

    /// <summary>
    /// Applies a theme choice and emits the current state.
    /// </summary>
    /// <param name="choice">The requested theme choice or name.</param>
    public void ApplyChoice(object? choice)
    {
        var selectedChoice = ParseChoice(choice);
        SelectedChoice = selectedChoice;

        var state = CreateState();
        CurrentState = state;

        ApplyTheme(ThemeService, state.EffectiveChoice);

        if (ThemeChangedCommand?.CanExecute(state) == true)
        {
            ThemeChangedCommand.Execute(state);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == SelectedChoiceProperty || change.Property == SystemChoiceProperty || change.Property == SupportsHighContrastProperty)
        {
            CurrentState = CreateState();
        }
    }

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

    private static void ApplyTheme(IThemeService? themeService, ThemeChoice effectiveChoice)
    {
        if (themeService is null)
        {
            return;
        }

        themeService.SetTheme(ToApplicationTheme(effectiveChoice));
    }

    private static ApplicationTheme ToApplicationTheme(ThemeChoice choice) => choice switch
    {
        ThemeChoice.Dark => ApplicationTheme.Dark,
        ThemeChoice.HighContrast => ApplicationTheme.HighContrast,
        _ => ApplicationTheme.Light
    };

    private sealed class ThemeSwitcherCommand : ICommand
    {
        private readonly Action<object?> _execute;

        public ThemeSwitcherCommand(Action<object?> execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute(parameter);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
