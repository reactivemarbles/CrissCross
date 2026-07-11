// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace CrissCross;

/// <summary>Represents platform-neutral state for steppers and wizard progress controls.</summary>
public sealed class StepperState
{
    /// <summary>Initializes a new instance of the <see cref="StepperState"/> class.</summary>
    /// <param name="steps">The steps projected by the workflow.</param>
    /// <param name="currentKey">The stable key for the current step.</param>
    /// <param name="orientation">The preferred presentation orientation.</param>
    public StepperState(IEnumerable<StepDescriptor> steps, string? currentKey = null, StepperOrientation orientation = StepperOrientation.Horizontal)
    {
        if (steps is null)
        {
            throw new ArgumentNullException(nameof(steps));
        }

        Steps = new ReadOnlyCollection<StepDescriptor>(steps.ToList());
        Orientation = orientation;
        CurrentKey = ResolveCurrentKey(currentKey);
        CurrentIndex = ResolveCurrentIndex(CurrentKey);
        CurrentStep = CurrentIndex >= 0 ? Steps[CurrentIndex] : null;
        CompletedCount = Steps.Count(static step => step.Status == StepStatus.Completed);
        BlockingStepCount = Steps.Count(static step => step.IsBlocking);
    }

    /// <summary>Gets the steps projected by the workflow.</summary>
    public IReadOnlyList<StepDescriptor> Steps { get; }

    /// <summary>Gets the stable key for the current step.</summary>
    public string? CurrentKey { get; }

    /// <summary>Gets the preferred presentation orientation.</summary>
    public StepperOrientation Orientation { get; }

    /// <summary>Gets the current zero-based step index, or -1 when there are no steps.</summary>
    public int CurrentIndex { get; }

    /// <summary>Gets the current step descriptor.</summary>
    public StepDescriptor? CurrentStep { get; }

    /// <summary>Gets the count of completed steps.</summary>
    public int CompletedCount { get; }

    /// <summary>Gets the count of steps that currently block progress.</summary>
    public int BlockingStepCount { get; }

    /// <summary>Gets a value indicating whether the workflow contains steps.</summary>
    public bool HasSteps => Steps.Count > 0;

    /// <summary>Gets a value indicating whether previous-step navigation is currently available.</summary>
    public bool CanGoPrevious => CurrentIndex > 0 && CurrentStep?.CanLeave == true && Steps[CurrentIndex - 1].IsAvailable;

    /// <summary>Gets a value indicating whether next-step navigation is currently available.</summary>
    public bool CanGoNext => CurrentIndex >= 0 && CurrentIndex < Steps.Count - 1 && CurrentStep?.CanLeave == true && Steps[CurrentIndex + 1].IsAvailable;

    /// <summary>Gets a value indicating whether the current workflow state can finish.</summary>
    public bool CanFinish => HasSteps && CurrentIndex == Steps.Count - 1 && BlockingStepCount == 0 && CurrentStep?.CanLeave == true;

    /// <summary>Gets compact progress text for diagnostics and screen-reader labels.</summary>
    public string ProgressText => HasSteps
        ? string.Format(CultureInfo.InvariantCulture, "Step {0} of {1}", CurrentIndex + 1, Steps.Count)
        : "No steps";

    /// <summary>Gets the step with the specified stable key.</summary>
    /// <param name="key">The stable step key.</param>
    /// <returns>The matching step, or <c>null</c> when no step has the key.</returns>
    public StepDescriptor? GetStep(string key) => Steps.FirstOrDefault(step => string.Equals(step.Key, key, StringComparison.Ordinal));

    /// <summary>Resolves the current step key from a requested key or active step.</summary>
    /// <param name="requestedKey">The requested current key.</param>
    /// <returns>The resolved current key.</returns>
    private string? ResolveCurrentKey(string? requestedKey)
    {
        var normalizedKey = (requestedKey ?? string.Empty).Trim();
        if (normalizedKey.Length > 0 && GetStep(normalizedKey) is { } requestedStep)
        {
            return requestedStep.Key;
        }

        var activeStep = Steps.FirstOrDefault(static step => step.IsCurrent);
        return activeStep?.Key ?? (Steps.Count > 0 ? Steps[0].Key : null);
    }

    /// <summary>Resolves the current step index for a key.</summary>
    /// <param name="currentKey">The current step key.</param>
    /// <returns>The zero-based step index, or -1 when not found.</returns>
    private int ResolveCurrentIndex(string? currentKey)
    {
        if (string.IsNullOrWhiteSpace(currentKey))
        {
            return -1;
        }

        for (var index = 0; index < Steps.Count; index++)
        {
            if (string.Equals(Steps[index].Key, currentKey, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }
}
