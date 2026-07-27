// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents platform-neutral state for steppers and wizard progress controls.</summary>
public sealed class StepperState
{
    /// <summary>Formats the stepper progress text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat ProgressFormat = System.Text.CompositeFormat.Parse("Step {0} of {1}");
#else
    private const string ProgressFormat = "Step {0} of {1}";
#endif

    /// <inheritdoc />
    public StepperState(IEnumerable<StepDescriptor> steps)
        : this(steps, null, StepperOrientation.Horizontal) { }

    /// <inheritdoc />
    public StepperState(IEnumerable<StepDescriptor> steps, string? currentKey)
        : this(steps, currentKey, StepperOrientation.Horizontal) { }

    /// <summary>Initializes a new instance of the <see cref="StepperState"/> class.</summary>
    /// <param name="steps">The steps projected by the workflow.</param>
    /// <param name="currentKey">The stable key for the current step.</param>
    /// <param name="orientation">The preferred presentation orientation.</param>
    public StepperState(IEnumerable<StepDescriptor> steps, string? currentKey, StepperOrientation orientation)
    {
        ThrowHelper.ThrowIfNull(steps, nameof(steps));

        var stepList = new List<StepDescriptor>(steps);

        Steps = new ReadOnlyCollection<StepDescriptor>(stepList);
        Orientation = orientation;
        (CurrentKey, CurrentIndex) = ResolveCurrentStep(currentKey);
        CurrentStep = CurrentIndex >= 0 ? Steps[CurrentIndex] : null;
        var completedCount = 0;
        var blockingStepCount = 0;
        foreach (var step in Steps)
        {
            if (step.Status == StepStatus.Completed)
            {
                completedCount++;
            }

            if (step.IsBlocking)
            {
                blockingStepCount++;
            }
        }

        CompletedCount = completedCount;
        BlockingStepCount = blockingStepCount;
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
    public bool CanGoPrevious =>
        CurrentIndex > 0 && CurrentStep?.CanLeave == true && Steps[CurrentIndex - 1].IsAvailable;

    /// <summary>Gets a value indicating whether next-step navigation is currently available.</summary>
    public bool CanGoNext =>
        CurrentIndex >= 0
        && CurrentIndex < Steps.Count - 1
        && CurrentStep?.CanLeave == true
        && Steps[CurrentIndex + 1].IsAvailable;

    /// <summary>Gets a value indicating whether the current workflow state can finish.</summary>
    public bool CanFinish =>
        HasSteps && CurrentIndex == Steps.Count - 1 && BlockingStepCount == 0 && CurrentStep?.CanLeave == true;

    /// <summary>Gets compact progress text for diagnostics and screen-reader labels.</summary>
    public string ProgressText =>
        HasSteps
            ? string.Format(CultureInfo.InvariantCulture, ProgressFormat, CurrentIndex + 1, Steps.Count)
            : "No steps";

    /// <summary>Gets the step with the specified stable key.</summary>
    /// <param name="key">The stable step key.</param>
    /// <returns>The matching step, or <c>null</c> when no step has the key.</returns>
    public StepDescriptor? GetStep(string key)
    {
        foreach (var step in Steps)
        {
            if (string.Equals(step.Key, key, StringComparison.Ordinal))
            {
                return step;
            }
        }

        return null;
    }

    /// <summary>Resolves the current step key and index from a requested key or active step.</summary>
    /// <param name="requestedKey">The requested current key.</param>
    /// <returns>The resolved current step key and zero-based index.</returns>
    private (string? Key, int Index) ResolveCurrentStep(string? requestedKey)
    {
        var normalizedKey = (requestedKey ?? string.Empty).Trim();
        if (normalizedKey.Length > 0)
        {
            for (var index = 0; index < Steps.Count; index++)
            {
                var step = Steps[index];
                if (string.Equals(step.Key, normalizedKey, StringComparison.Ordinal))
                {
                    return (step.Key, index);
                }
            }
        }

        for (var index = 0; index < Steps.Count; index++)
        {
            var step = Steps[index];
            if (step.IsCurrent)
            {
                return (step.Key, index);
            }
        }

        return Steps.Count > 0 ? (Steps[0].Key, 0) : (null, -1);
    }
}
