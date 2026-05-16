// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>
/// Immutable status snapshot for controls that present a command execution surface.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CommandButtonStatus"/> class.
/// </remarks>
/// <param name="state">The current command execution state.</param>
/// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
/// <param name="isExecuting">A value indicating whether the command is currently executing.</param>
/// <param name="error">The most recent command error, when the state is failed.</param>
public sealed class CommandButtonStatus(CommandButtonState state, bool canExecute, bool isExecuting, Exception? error = null)
{
    /// <summary>
    /// Gets the current command execution state.
    /// </summary>
    public CommandButtonState State { get; } = state;

    /// <summary>
    /// Gets a value indicating whether the underlying command can execute.
    /// </summary>
    public bool CanExecute { get; } = canExecute;

    /// <summary>
    /// Gets a value indicating whether the command is currently executing.
    /// </summary>
    public bool IsExecuting { get; } = isExecuting;

    /// <summary>
    /// Gets the most recent command error, when the state is failed.
    /// </summary>
    public Exception? Error { get; } = error;

    /// <summary>
    /// Gets a value indicating whether the command surface should allow user interaction.
    /// </summary>
    public bool IsInteractive => CanExecute && !IsExecuting;

    /// <summary>
    /// Gets a value indicating whether the state carries a command error.
    /// </summary>
    public bool HasError => State == CommandButtonState.Failed && Error is not null;

    /// <summary>
    /// Creates an idle status snapshot.
    /// </summary>
    /// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
    /// <returns>An idle command status.</returns>
    public static CommandButtonStatus Idle(bool canExecute) => new(CommandButtonState.Idle, canExecute, false);

    /// <summary>
    /// Creates an executing status snapshot.
    /// </summary>
    /// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
    /// <returns>An executing command status.</returns>
    public static CommandButtonStatus Executing(bool canExecute) => new(CommandButtonState.Executing, canExecute, true);

    /// <summary>
    /// Creates a succeeded status snapshot.
    /// </summary>
    /// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
    /// <returns>A succeeded command status.</returns>
    public static CommandButtonStatus Succeeded(bool canExecute) => new(CommandButtonState.Succeeded, canExecute, false);

    /// <summary>
    /// Creates a failed status snapshot.
    /// </summary>
    /// <param name="error">The command failure.</param>
    /// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
    /// <returns>A failed command status.</returns>
    public static CommandButtonStatus Failed(Exception error, bool canExecute)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        return new CommandButtonStatus(CommandButtonState.Failed, canExecute, false, error);
    }

    /// <summary>
    /// Creates a cancelled status snapshot.
    /// </summary>
    /// <param name="canExecute">A value indicating whether the underlying command can execute.</param>
    /// <returns>A cancelled command status.</returns>
    public static CommandButtonStatus Cancelled(bool canExecute) => new(CommandButtonState.Cancelled, canExecute, false);
}
