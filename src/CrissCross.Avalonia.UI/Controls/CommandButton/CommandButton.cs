// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a command-aware button that can project execution state, progress, and error content.
/// </summary>
public class CommandButton : Button
{
    /// <summary>
    /// Property for <see cref="State"/>.
    /// </summary>
    public static readonly StyledProperty<CommandButtonState> StateProperty = AvaloniaProperty.Register<CommandButton, CommandButtonState>(
        nameof(State), CommandButtonState.Idle);

    /// <summary>
    /// Property for <see cref="IsExecuting"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsExecutingProperty = AvaloniaProperty.Register<CommandButton, bool>(
        nameof(IsExecuting));

    /// <summary>
    /// Property for <see cref="Progress"/>.
    /// </summary>
    public static readonly StyledProperty<double?> ProgressProperty = AvaloniaProperty.Register<CommandButton, double?>(
        nameof(Progress));

    /// <summary>
    /// Property for <see cref="ExecutingContent"/>.
    /// </summary>
    public static readonly StyledProperty<object?> ExecutingContentProperty = AvaloniaProperty.Register<CommandButton, object?>(
        nameof(ExecutingContent), "Working...");

    /// <summary>
    /// Property for <see cref="ErrorContent"/>.
    /// </summary>
    public static readonly StyledProperty<object?> ErrorContentProperty = AvaloniaProperty.Register<CommandButton, object?>(
        nameof(ErrorContent));

    /// <summary>
    /// Gets or sets the command execution state displayed by the button.
    /// </summary>
    public CommandButtonState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command is executing.
    /// </summary>
    public bool IsExecuting
    {
        get => GetValue(IsExecutingProperty);
        set => SetValue(IsExecutingProperty, value);
    }

    /// <summary>
    /// Gets or sets optional normalized progress from 0.0 to 1.0.
    /// </summary>
    public double? Progress
    {
        get => GetValue(ProgressProperty);
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

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == IsExecutingProperty && change.GetNewValue<bool>())
        {
            SetCurrentValue(StateProperty, CommandButtonState.Executing);
        }
    }
}
