// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using CrissCross.Maui.UI.Resources.Styles;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using CrissCrossStepper = CrissCross.Maui.UI.Controls.Stepper;

namespace CrissCross.Tests;

/// <summary>Tests for MAUI controls that now provide composed native default content.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class MauiFunctionalControlBehaviorTests
{
    /// <summary>Provides the number of default EmptyState layout rows.</summary>
    private const int EmptyStateDefaultRows = 4;

    /// <summary>Provides the number of validation message rows used by the validation summary test.</summary>
    private const int ValidationMessageCount = 2;

    /// <summary>Provides the number of rendered step rows used by the stepper test.</summary>
    private const int RenderedStepRowCount = 4;

    /// <summary>Provides the first step key.</summary>
    private const string DetailsStepKey = "details";

    /// <summary>Provides the current step key.</summary>
    private const string ReviewStepKey = "review";

    /// <summary>Provides the empty state title text.</summary>
    private const string EmptyStateTitle = "No pumps configured";

    /// <summary>Provides the next step key.</summary>
    private const string SubmitStepKey = "submit";

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Verifies the MAUI empty-state default content renders model text and exposes the primary action.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task EmptyState_ModelWithPrimaryAction_RendersDefaultContentAndInvokesCommand()
    {
        var command = new CaptureCommand();
        var model = new EmptyStateModel(
            EmptyStateTitle,
            "Create the first pump before starting acquisition.",
            EmptyStateVariant.NoData,
            "Add pump");
        var target = new EmptyState { Model = model, PrimaryCommand = command };
        var root = (VerticalStackLayout)target.Content!;
        var title = (Label)root.Children[1];
        var message = (Label)root.Children[2];
        var actions = (HorizontalStackLayout)root.Children[3];
        var primary = (Button)actions.Children[0];

        primary.Command!.Execute(null);

        await Assert.That(root.Children.Count).IsEqualTo(EmptyStateDefaultRows);
        await Assert.That(title.Text).IsEqualTo(EmptyStateTitle);
        await Assert.That(message.Text).Contains("Create the first pump");
        await Assert.That(primary.Text).IsEqualTo("Add pump");
        await Assert.That(primary.IsVisible).IsTrue();
        await Assert.That(command.WasExecuted).IsTrue();
        await Assert.That(SemanticProperties.GetDescription(target)).Contains(EmptyStateTitle);
    }

    /// <summary>Verifies the MAUI validation summary renders actual message rows from the shared state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ValidationSummary_State_RendersMessagesWithSeverityText()
    {
        var state = new ValidationSummaryState(
            [
                new ValidationMessage("name", "Name", "is required", ValidationSeverity.Error),
                new ValidationMessage("flow", "Flow", "is above nominal", ValidationSeverity.Warning),
            ]);
        var target = new ValidationSummary { SummaryState = state };
        var root = (VerticalStackLayout)target.Content!;
        var heading = (Label)root.Children[0];
        var messages = (VerticalStackLayout)root.Children[1];
        var firstMessage = (Label)messages.Children[0];
        var secondMessage = (Label)messages.Children[1];

        await Assert.That(heading.Text).IsEqualTo("1 error, 1 warning");
        await Assert.That(messages.Children.Count).IsEqualTo(ValidationMessageCount);
        await Assert.That(firstMessage.Text).IsEqualTo("Error: Name: is required");
        await Assert.That(secondMessage.Text).IsEqualTo("Warning: Flow: is above nominal");
        await Assert.That(SemanticProperties.GetDescription(target)).IsEqualTo("1 error, 1 warning");
    }

    /// <summary>Verifies the MAUI stepper renders steps and emits only allowed navigation requests.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Stepper_State_RendersStepsAndRejectsUnavailableCommands()
    {
        var aggregateCommand = new CaptureCommand();
        var enterCommand = new CaptureCommand();
        var leaveCommand = new CaptureCommand();
        var completed = new StepDescriptor(DetailsStepKey, "Details", new() { Status = StepStatus.Completed });
        var current = new StepDescriptor(ReviewStepKey, "Review", new() { Status = StepStatus.Active, LeaveCommand = leaveCommand });
        var next = new StepDescriptor(SubmitStepKey, "Submit", new StepDescriptorOptions { EnterCommand = enterCommand });
        var unavailable = new StepDescriptor("blocked", "Blocked", new StepDescriptorOptions { CanEnter = false });
        var target = new CrissCrossStepper { StepCommand = aggregateCommand, StepperState = new([completed, current, next, unavailable], ReviewStepKey, StepperOrientation.Vertical) };
        var root = (VerticalStackLayout)target.Content!;
        var steps = (VerticalStackLayout)root.Children[1];
        var actions = (HorizontalStackLayout)root.Children[2];
        var nextButton = (Button)actions.Children[1];

        var renderedStepCount = steps.Children.Count;
        var canMoveNext = nextButton.Command?.CanExecute(null) == true;
        nextButton.Command!.Execute(null);
        target.StepperState = new([current, unavailable], ReviewStepKey, StepperOrientation.Vertical);
        var blockedNextButton = (Button)((HorizontalStackLayout)((VerticalStackLayout)target.Content!).Children[2]).Children[1];
        var blocked = blockedNextButton.Command?.CanExecute(null) == true;

        await Assert.That(renderedStepCount).IsEqualTo(RenderedStepRowCount);
        await Assert.That(canMoveNext).IsTrue();
        await Assert.That(blocked).IsFalse();
        await Assert.That(leaveCommand.LastParameter).IsEqualTo(current);
        await Assert.That(enterCommand.LastParameter).IsEqualTo(next);
        await Assert.That(aggregateCommand.LastParameter).IsEqualTo(next);
    }

    /// <summary>Verifies disabled MAUI Stepper commands guard execution because MAUI Command does not enforce CanExecute in Execute.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Stepper_DisabledCommandExecute_DoesNotEmitBlockedRequests()
    {
        var aggregateCommand = new CaptureCommand();
        var current = new StepDescriptor(ReviewStepKey, "Review", new() { Status = StepStatus.Active, CanLeave = false });
        var next = new StepDescriptor(SubmitStepKey, "Submit");
        var target = new CrissCrossStepper { StepCommand = aggregateCommand, StepperState = new([current, next], ReviewStepKey) };
        var actions = (HorizontalStackLayout)((VerticalStackLayout)target.Content!).Children[2];
        var previousButton = (Button)actions.Children[0];
        var nextButton = (Button)actions.Children[1];
        var finishButton = (Button)actions.Children[2];
        var previousCommand = previousButton.Command!;
        var nextCommand = nextButton.Command!;
        var finishCommand = finishButton.Command!;

        previousCommand.Execute(null);
        nextCommand.Execute(null);
        finishCommand.Execute(null);

        await Assert.That(previousCommand.CanExecute(null)).IsFalse();
        await Assert.That(nextCommand.CanExecute(null)).IsFalse();
        await Assert.That(finishCommand.CanExecute(null)).IsFalse();
        await Assert.That(aggregateCommand.WasExecuted).IsFalse();
    }

    /// <summary>Verifies PersonPicture initials consume the accent text dynamic resource for high-contrast readability.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PersonPicture_Initials_UseAccentTextDynamicResource()
    {
        var target = new PersonPicture { DisplayName = "Grace Hopper" };
        var page = new ContentPage { Content = target };
        page.Resources.MergedDictionaries.Add(new CrissCrossMauiHighContrastTheme());
        var grid = (Grid)target.Content!;
        var initials = (Label)grid.Children[0];

        await Assert.That(initials.Text).IsEqualTo("GH");
        await Assert.That(initials.TextColor).IsEqualTo(Colors.Black);
        await Assert.That(SemanticProperties.GetDescription(target)).IsEqualTo("Grace Hopper");
    }

    /// <summary>Captures command execution and the most recent command parameter.</summary>
    private sealed class CaptureCommand : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets the most recent command parameter.</summary>
        public object? LastParameter { get; private set; }

        /// <summary>Gets a value indicating whether the command has executed.</summary>
        public bool WasExecuted { get; private set; }

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            WasExecuted = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
