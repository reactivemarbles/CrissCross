// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Represents a command-aware MAUI button that projects execution state, progress, and error content.
/// </summary>
public class CommandButton : Button
{
    /// <summary>
    /// Bindable property for <see cref="State"/>.
    /// </summary>
    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(CommandButtonState),
        typeof(CommandButton),
        CommandButtonState.Idle);

    /// <summary>
    /// Bindable property for <see cref="IsExecuting"/>.
    /// </summary>
    public static readonly BindableProperty IsExecutingProperty = BindableProperty.Create(
        nameof(IsExecuting),
        typeof(bool),
        typeof(CommandButton),
        false,
        propertyChanged: OnIsExecutingChanged);

    /// <summary>
    /// Bindable property for <see cref="Progress"/>.
    /// </summary>
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(double?),
        typeof(CommandButton));

    /// <summary>
    /// Bindable property for <see cref="ExecutingText"/>.
    /// </summary>
    public static readonly BindableProperty ExecutingTextProperty = BindableProperty.Create(
        nameof(ExecutingText),
        typeof(string),
        typeof(CommandButton),
        "Working...");

    /// <summary>
    /// Bindable property for <see cref="ErrorText"/>.
    /// </summary>
    public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
        nameof(ErrorText),
        typeof(string),
        typeof(CommandButton));

    /// <summary>
    /// Gets or sets the command execution state displayed by the button.
    /// </summary>
    public CommandButtonState State
    {
        get => (CommandButtonState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command is currently executing.
    /// </summary>
    public bool IsExecuting
    {
        get => (bool)GetValue(IsExecutingProperty);
        set => SetValue(IsExecutingProperty, value);
    }

    /// <summary>
    /// Gets or sets optional normalized progress from 0.0 to 1.0.
    /// </summary>
    public double? Progress
    {
        get => (double?)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Gets or sets the text displayed by templates while a command is executing.
    /// </summary>
    public string? ExecutingText
    {
        get => (string?)GetValue(ExecutingTextProperty);
        set => SetValue(ExecutingTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the text displayed by templates when a command fails.
    /// </summary>
    public string? ErrorText
    {
        get => (string?)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    private static void OnIsExecutingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CommandButton button && newValue is true)
        {
            button.SetValue(StateProperty, CommandButtonState.Executing);
        }
    }
}
