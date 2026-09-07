// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Layouts;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays and changes a shared theme preference snapshot.</summary>
public class ThemeSwitcher : ContentView
{
    /// <summary>Bindable property for <see cref="ThemeState"/>.</summary>
    public static readonly BindableProperty ThemeStateProperty = BindableProperty.Create(
        nameof(ThemeState),
        typeof(ThemePreferenceState),
        typeof(ThemeSwitcher),
        propertyChanged: static (bindable, _, newValue) => OnThemeStateChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="ChangeThemeCommand"/>.</summary>
    public static readonly BindableProperty ChangeThemeCommandProperty = BindableProperty.Create(
        nameof(ChangeThemeCommand),
        typeof(ICommand),
        typeof(ThemeSwitcher),
        propertyChanged: static (bindable, _, _) => OnThemeCommandChanged(bindable));

    /// <summary>Provides spacing for the composed theme switcher surface.</summary>
    private const double LayoutSpacing = 6;

    /// <summary>Provides default state when callers have not supplied a theme snapshot.</summary>
    private static readonly ThemePreferenceState DefaultThemeState = new(ThemeChoice.System, ThemeChoice.Light, supportsHighContrast: true);

    /// <summary>Displays the active theme description.</summary>
    private readonly Label _descriptionLabel = CreateLabel(isEmphasized: true);

    /// <summary>Hosts theme choice buttons.</summary>
    private readonly FlexLayout _choicesPanel = new() { Direction = FlexDirection.Row, Wrap = FlexWrap.Wrap, AlignItems = FlexAlignItems.Center };

    /// <summary>Initializes a new instance of the <see cref="ThemeSwitcher"/> class.</summary>
    public ThemeSwitcher()
    {
        Content = CreateLayout();
        ApplyState(DefaultThemeState);
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public ThemePreferenceState? ThemeState
    {
        get => (ThemePreferenceState?)GetValue(ThemeStateProperty);
        set => SetValue(ThemeStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? ChangeThemeCommand
    {
        get => (ICommand?)GetValue(ChangeThemeCommandProperty);
        set => SetValue(ChangeThemeCommandProperty, value);
    }

    /// <summary>Selects a theme choice and invokes <see cref="ChangeThemeCommand"/> with the selected <see cref="ThemeChoice"/>.</summary>
    /// <param name="choice">The selected theme choice.</param>
    /// <returns><c>true</c> when the command was invoked; otherwise, <c>false</c>.</returns>
    public bool SelectTheme(ThemeChoice choice)
    {
        var state = ThemeState ?? DefaultThemeState;
        if (!state.SupportsChoice(choice) || ChangeThemeCommand?.CanExecute(choice) != true)
        {
            return false;
        }

        ChangeThemeCommand.Execute(choice);
        return true;
    }

    /// <summary>Creates a display label.</summary>
    /// <param name="isEmphasized">A value indicating whether the label should use emphasized text.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(bool isEmphasized)
    {
        var label = new Label { FontAttributes = isEmphasized ? FontAttributes.Bold : FontAttributes.None };
        label.SetDynamicResource(Label.TextColorProperty, "CrissCrossTextColor");
        return label;
    }

    /// <summary>Gets display text for a theme choice.</summary>
    /// <param name="choice">The theme choice.</param>
    /// <returns>The display text.</returns>
    private static string GetChoiceText(ThemeChoice choice) => choice switch
    {
        ThemeChoice.Dark => "Dark",
        ThemeChoice.Light => "Light",
        ThemeChoice.HighContrast => "High contrast",
        _ => "System",
    };

    /// <summary>Applies a state change from the bindable property.</summary>
    /// <param name="bindable">The bindable control.</param>
    /// <param name="newValue">The new state value.</param>
    private static void OnThemeStateChanged(BindableObject bindable, object newValue)
    {
        if (bindable is not ThemeSwitcher switcher)
        {
            return;
        }

        switcher.ApplyState(newValue as ThemePreferenceState ?? DefaultThemeState);
    }

    /// <summary>Applies command availability changes to the current presentation.</summary>
    /// <param name="bindable">The bindable control.</param>
    private static void OnThemeCommandChanged(BindableObject bindable)
    {
        if (bindable is not ThemeSwitcher switcher)
        {
            return;
        }

        switcher.ApplyState(switcher.ThemeState ?? DefaultThemeState);
    }

    /// <summary>Creates the native MAUI layout.</summary>
    /// <returns>The composed view.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = LayoutSpacing };
        root.Children.Add(_descriptionLabel);
        root.Children.Add(_choicesPanel);
        return root;
    }

    /// <summary>Applies the supplied theme state to the visual choices.</summary>
    /// <param name="state">The theme state.</param>
    private void ApplyState(ThemePreferenceState state)
    {
        _descriptionLabel.Text = state.DisplayText;
        _choicesPanel.Children.Clear();
        foreach (var choice in state.AvailableChoices)
        {
            var button = new Button
            {
                Text = GetChoiceText(choice),
                Command = ChangeThemeCommand,
                CommandParameter = choice,
                Margin = new(0, 0, LayoutSpacing, LayoutSpacing),
                IsEnabled = state.SupportsChoice(choice) && ChangeThemeCommand is not null,
            };
            button.SetDynamicResource(
                Button.BackgroundColorProperty,
                choice == state.SelectedChoice ? "CrissCrossAccentColor" : "CrissCrossNeutralSurfaceColor");
            button.SetDynamicResource(
                Button.TextColorProperty,
                choice == state.SelectedChoice ? "CrissCrossAccentTextColor" : "CrissCrossTextColor");
            _choicesPanel.Children.Add(button);
        }
    }
}
