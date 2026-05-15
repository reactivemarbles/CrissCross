// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a command-aware button that can project execution state, progress, and error content.
/// </summary>
public class CommandButton : Button
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(CommandButtonState),
        typeof(CommandButton),
        new PropertyMetadata(CommandButtonState.Idle));

    /// <summary>
    /// Property for <see cref="IsExecuting"/>.
    /// </summary>
    public static readonly DependencyProperty IsExecutingProperty = DependencyProperty.Register(
        nameof(IsExecuting),
        typeof(bool),
        typeof(CommandButton),
        new PropertyMetadata(false, OnIsExecutingChanged));

    /// <summary>
    /// Property for <see cref="Progress"/>.
    /// </summary>
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress),
        typeof(double?),
        typeof(CommandButton),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="ExecutingContent"/>.
    /// </summary>
    public static readonly DependencyProperty ExecutingContentProperty = DependencyProperty.Register(
        nameof(ExecutingContent),
        typeof(object),
        typeof(CommandButton),
        new PropertyMetadata("Working..."));

    /// <summary>
    /// Property for <see cref="ErrorContent"/>.
    /// </summary>
    public static readonly DependencyProperty ErrorContentProperty = DependencyProperty.Register(
        nameof(ErrorContent),
        typeof(object),
        typeof(CommandButton),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the command execution state displayed by the button.
    /// </summary>
    public CommandButtonState State
    {
        get => (CommandButtonState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command is executing.
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
    /// Gets or sets the content shown while the command is executing.
    /// </summary>
    public object? ExecutingContent
    {
        get => GetValue(ExecutingContentProperty);
        set => SetValue(ExecutingContentProperty, value);
    }

    /// <summary>
    /// Gets or sets optional content shown for a failed command.
    /// </summary>
    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    private static void OnIsExecutingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is CommandButton button && args.NewValue is true)
        {
            button.SetCurrentValue(StateProperty, CommandButtonState.Executing);
        }
    }
}
