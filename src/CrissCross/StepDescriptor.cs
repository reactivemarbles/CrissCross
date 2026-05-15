// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CrissCross;

/// <summary>
/// Describes one platform-neutral workflow step for steppers and wizard progress controls.
/// </summary>
public sealed class StepDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepDescriptor"/> class.
    /// </summary>
    /// <param name="key">The stable step key.</param>
    /// <param name="title">The display title.</param>
    /// <param name="status">The workflow status for the step.</param>
    /// <param name="isOptional">A value indicating whether the step is optional.</param>
    /// <param name="isEnabled">A value indicating whether the step can be shown as enabled.</param>
    /// <param name="canEnter">A value indicating whether navigation may enter the step.</param>
    /// <param name="canLeave">A value indicating whether navigation may leave the step.</param>
    /// <param name="validationMessages">The validation messages associated with the step.</param>
    /// <param name="enterCommand">The command invoked by a platform control when entering the step.</param>
    /// <param name="leaveCommand">The command invoked by a platform control when leaving the step.</param>
    public StepDescriptor(
        string key,
        string title,
        StepStatus status = StepStatus.Pending,
        bool isOptional = false,
        bool isEnabled = true,
        bool canEnter = true,
        bool canLeave = true,
        IEnumerable<ValidationMessage>? validationMessages = null,
        ICommand? enterCommand = null,
        ICommand? leaveCommand = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("A step key is required.", nameof(key));
        }

        Key = key.Trim();
        Title = title ?? string.Empty;
        Status = status;
        IsOptional = isOptional;
        IsEnabled = isEnabled;
        CanEnter = canEnter;
        CanLeave = canLeave;
        ValidationMessages = new ReadOnlyCollection<ValidationMessage>((validationMessages ?? []).ToList());
        EnterCommand = enterCommand;
        LeaveCommand = leaveCommand;
    }

    /// <summary>
    /// Gets the stable step key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the display title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the workflow status for the step.
    /// </summary>
    public StepStatus Status { get; }

    /// <summary>
    /// Gets a value indicating whether the step is optional.
    /// </summary>
    public bool IsOptional { get; }

    /// <summary>
    /// Gets a value indicating whether the step can be shown as enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether navigation may enter the step.
    /// </summary>
    public bool CanEnter { get; }

    /// <summary>
    /// Gets a value indicating whether navigation may leave the step.
    /// </summary>
    public bool CanLeave { get; }

    /// <summary>
    /// Gets validation messages associated with the step.
    /// </summary>
    public IReadOnlyList<ValidationMessage> ValidationMessages { get; }

    /// <summary>
    /// Gets the command invoked by a platform control when entering the step.
    /// </summary>
    public ICommand? EnterCommand { get; }

    /// <summary>
    /// Gets the command invoked by a platform control when leaving the step.
    /// </summary>
    public ICommand? LeaveCommand { get; }

    /// <summary>
    /// Gets a value indicating whether validation messages are associated with the step.
    /// </summary>
    public bool HasValidationMessages => ValidationMessages.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the step blocks forward progress.
    /// </summary>
    public bool IsBlocking => Status == StepStatus.Error || ValidationMessages.Any(static message => message.IsBlocking);

    /// <summary>
    /// Gets a value indicating whether the step is active/current.
    /// </summary>
    public bool IsCurrent => Status == StepStatus.Active;

    /// <summary>
    /// Gets a value indicating whether the step is completed or intentionally skipped.
    /// </summary>
    public bool IsComplete => Status is StepStatus.Completed or StepStatus.Skipped;

    /// <summary>
    /// Gets a value indicating whether navigation may enter and display the step.
    /// </summary>
    public bool IsAvailable => IsEnabled && CanEnter && !IsBlocking;

    /// <summary>
    /// Gets display text including optional-step hinting.
    /// </summary>
    public string DisplayTitle => IsOptional ? Title + " (optional)" : Title;
}
