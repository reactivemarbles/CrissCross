// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Configures workflow state, validation, and commands for a step descriptor.</summary>
public sealed class StepDescriptorOptions
{
    /// <summary>Gets or sets the workflow status for the step.</summary>
    public StepStatus Status { get; set; } = StepStatus.Pending;

    /// <summary>Gets or sets a value indicating whether the step is optional.</summary>
    public bool IsOptional { get; set; }

    /// <summary>Gets or sets a value indicating whether the step is enabled.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether navigation may enter the step.</summary>
    public bool CanEnter { get; set; } = true;

    /// <summary>Gets or sets a value indicating whether navigation may leave the step.</summary>
    public bool CanLeave { get; set; } = true;

    /// <summary>Gets or sets validation messages associated with the step.</summary>
    public IEnumerable<ValidationMessage>? ValidationMessages { get; set; }

    /// <summary>Gets or sets the command invoked by a platform control when entering the step.</summary>
    public ICommand? EnterCommand { get; set; }

    /// <summary>Gets or sets the command invoked by a platform control when leaving the step.</summary>
    public ICommand? LeaveCommand { get; set; }
}
