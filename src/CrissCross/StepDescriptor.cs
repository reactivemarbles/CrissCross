// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Describes one platform-neutral workflow step for steppers and wizard progress controls.</summary>
public sealed class StepDescriptor
{
    /// <inheritdoc />
    public StepDescriptor(string key, string title)
        : this(key, title, new StepDescriptorOptions()) { }

    /// <summary>Initializes a new instance of the <see cref="StepDescriptor"/> class.</summary>
    /// <param name="key">The stable step key.</param>
    /// <param name="title">The display title.</param>
    /// <param name="options">The workflow state, validation, and command options.</param>
    public StepDescriptor(string key, string title, StepDescriptorOptions options)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ThrowHelper.ThrowIfNull(options, nameof(options));

        Key = key.Trim();
        Title = title ?? string.Empty;
        Status = options.Status;
        IsOptional = options.IsOptional;
        IsEnabled = options.IsEnabled;
        CanEnter = options.CanEnter;
        CanLeave = options.CanLeave;
        ValidationMessages = new ReadOnlyCollection<ValidationMessage>((options.ValidationMessages ?? []).ToList());
        EnterCommand = options.EnterCommand;
        LeaveCommand = options.LeaveCommand;
    }

    /// <summary>Gets the stable step key.</summary>
    public string Key { get; }

    /// <summary>Gets the display title.</summary>
    public string Title { get; }

    /// <summary>Gets the workflow status for the step.</summary>
    public StepStatus Status { get; }

    /// <summary>Gets a value indicating whether the step is optional.</summary>
    public bool IsOptional { get; }

    /// <summary>Gets a value indicating whether the step can be shown as enabled.</summary>
    public bool IsEnabled { get; }

    /// <summary>Gets a value indicating whether navigation may enter the step.</summary>
    public bool CanEnter { get; }

    /// <summary>Gets a value indicating whether navigation may leave the step.</summary>
    public bool CanLeave { get; }

    /// <summary>Gets validation messages associated with the step.</summary>
    public IReadOnlyList<ValidationMessage> ValidationMessages { get; }

    /// <summary>Gets the command invoked by a platform control when entering the step.</summary>
    public System.Windows.Input.ICommand? EnterCommand { get; }

    /// <summary>Gets the command invoked by a platform control when leaving the step.</summary>
    public System.Windows.Input.ICommand? LeaveCommand { get; }

    /// <summary>Gets a value indicating whether validation messages are associated with the step.</summary>
    public bool HasValidationMessages => ValidationMessages.Count > 0;

    /// <summary>Gets a value indicating whether the step blocks forward progress.</summary>
    public bool IsBlocking =>
        Status == StepStatus.Error || ValidationMessages.Any(static message => message.IsBlocking);

    /// <summary>Gets a value indicating whether the step is active/current.</summary>
    public bool IsCurrent => Status == StepStatus.Active;

    /// <summary>Gets a value indicating whether the step is completed or intentionally skipped.</summary>
    public bool IsComplete => Status is StepStatus.Completed or StepStatus.Skipped;

    /// <summary>Gets a value indicating whether navigation may enter and display the step.</summary>
    public bool IsAvailable => IsEnabled && CanEnter && !IsBlocking;

    /// <summary>Gets display text including optional-step hinting.</summary>
    public string DisplayTitle => IsOptional ? Title + " (optional)" : Title;
}
