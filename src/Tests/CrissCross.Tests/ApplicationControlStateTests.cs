// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross.Tests;

/// <summary>
/// Tests for platform-neutral application control state models shared by UI stacks.
/// </summary>
public class ApplicationControlStateTests
{
    [Test]
    public async Task CommandButtonStatus_Executing_IsNotInteractiveAndHasNoError()
    {
        var status = CommandButtonStatus.Executing(canExecute: true);

        await Assert.That(status.State).IsEqualTo(CommandButtonState.Executing);
        await Assert.That(status.IsExecuting).IsTrue();
        await Assert.That(status.IsInteractive).IsFalse();
        await Assert.That(status.HasError).IsFalse();
    }

    [Test]
    public async Task CommandButtonStatus_Failed_ExposesErrorAndStaysInteractiveWhenCommandCanExecute()
    {
        var exception = new InvalidOperationException("boom");
        var status = CommandButtonStatus.Failed(exception, canExecute: true);

        await Assert.That(status.State).IsEqualTo(CommandButtonState.Failed);
        await Assert.That(status.Error).IsEqualTo(exception);
        await Assert.That(status.HasError).IsTrue();
        await Assert.That(status.IsInteractive).IsTrue();
    }

    [Test]
    public async Task BusyOperation_DeterminateProgress_IsActiveDeterminateAndCancellable()
    {
        var cancelCommand = new TestCommand();
        var operation = new BusyOperation("Importing", "42 items", 0.42, cancelCommand);

        await Assert.That(operation.IsActive).IsTrue();
        await Assert.That(operation.IsDeterminate).IsTrue();
        await Assert.That(operation.IsCancellable).IsTrue();
        await Assert.That(operation.Progress).IsEqualTo(0.42);
    }

    [Test]
    public async Task EmptyStateModel_WithPrimaryAction_ReportsActionAvailability()
    {
        var action = new TestCommand();
        var model = new EmptyStateModel(
            "No matching records",
            "Clear filters or create a new record.",
            EmptyStateVariant.NoResults,
            "Clear filters",
            action);

        await Assert.That(model.HasPrimaryAction).IsTrue();
        await Assert.That(model.HasSecondaryAction).IsFalse();
        await Assert.That(model.Variant).IsEqualTo(EmptyStateVariant.NoResults);
    }

    [Test]
    public async Task SearchQueryState_WithTextAndFilters_ExposesNormalizedQueryAndActiveFilterCount()
    {
        var filters = new[]
        {
            new FilterToken("status", FilterOperator.Equals, "open", "Status: Open"),
            new FilterToken("priority", FilterOperator.GreaterThanOrEqual, 2, "Priority >= 2")
        };

        var state = new SearchQueryState("  pump fault  ", "pump", "pump fault", isSearching: true, resultCount: 12, filters: filters);

        await Assert.That(state.NormalizedText).IsEqualTo("pump fault");
        await Assert.That(state.HasQuery).IsTrue();
        await Assert.That(state.IsFiltered).IsTrue();
        await Assert.That(state.ActiveFilterCount).IsEqualTo(2);
        await Assert.That(state.ResultSummary).IsEqualTo("12 results");
    }

    [Test]
    public async Task FilterToken_RemovableToken_UsesDisplayTextAndStableKey()
    {
        var token = new FilterToken("area", FilterOperator.Contains, "north", "Area contains north", isRemovable: true);

        await Assert.That(token.Key).IsEqualTo("area:Contains:north");
        await Assert.That(token.DisplayText).IsEqualTo("Area contains north");
        await Assert.That(token.IsRemovable).IsTrue();
    }

    [Test]
    public async Task ChipModel_RemovableSelectedChip_ExposesInteractionStateAndStableKey()
    {
        var removeCommand = new TestCommand();
        var chip = new ChipModel(
            "status-open",
            "Open",
            isSelected: true,
            isRemovable: true,
            isEnabled: true,
            icon: "StatusGlyph",
            removeCommand: removeCommand);

        await Assert.That(chip.Key).IsEqualTo("status-open");
        await Assert.That(chip.HasIcon).IsTrue();
        await Assert.That(chip.IsInteractive).IsTrue();
        await Assert.That(chip.CanRemove).IsTrue();
    }

    [Test]
    public async Task ChipGroupState_MultipleSelection_ReportsSelectedAndRemovableChips()
    {
        var chips = new[]
        {
            new ChipModel("open", "Open", isSelected: true, isRemovable: true),
            new ChipModel("closed", "Closed", isSelected: false, isRemovable: true),
            new ChipModel("locked", "Locked", isSelected: true, isRemovable: false, isEnabled: false)
        };

        var group = new ChipGroupState(chips, ChipGroupSelectionMode.Multiple);

        await Assert.That(group.HasSelection).IsTrue();
        await Assert.That(group.CanSelectMultiple).IsTrue();
        await Assert.That(group.SelectedChips).Count().IsEqualTo(2);
        await Assert.That(group.RemovableChips).Count().IsEqualTo(2);
        await Assert.That(group.GetChip("open")?.Text).IsEqualTo("Open");
    }

    [Test]
    public async Task SegmentedSelectionState_SingleSelection_ExposesSelectedItemAndEnabledItems()
    {
        var items = new[]
        {
            new SegmentItem("list", "List", isEnabled: true, icon: "ListGlyph"),
            new SegmentItem("grid", "Grid", isEnabled: true),
            new SegmentItem("map", "Map", isEnabled: false)
        };

        var state = new SegmentedSelectionState(items, "grid");

        await Assert.That(state.SelectedItem?.Key).IsEqualTo("grid");
        await Assert.That(state.HasSelection).IsTrue();
        await Assert.That(state.EnabledItems).Count().IsEqualTo(2);
        await Assert.That(state.GetItem("list")?.HasIcon).IsTrue();
    }

    [Test]
    public async Task ValidationMessage_Error_IsBlockingAndUsesExplicitDisplayText()
    {
        var action = new TestCommand();
        var message = new ValidationMessage(
            "email",
            "Email address",
            "Enter a valid email address.",
            ValidationSeverity.Error,
            action);

        await Assert.That(message.IsBlocking).IsTrue();
        await Assert.That(message.HasRemediation).IsTrue();
        await Assert.That(message.DisplayText).IsEqualTo("Email address: Enter a valid email address.");
    }

    [Test]
    public async Task ValidationSummaryState_MixedMessages_ReportsValidityCountsAndFieldMessages()
    {
        var messages = new[]
        {
            new ValidationMessage("email", "Email", "Required", ValidationSeverity.Error),
            new ValidationMessage("password", "Password", "Weak", ValidationSeverity.Warning),
            new ValidationMessage("profile", "Profile", "Checking availability", ValidationSeverity.Pending)
        };

        var summary = new ValidationSummaryState(messages);
        var emailMessages = summary.GetMessagesForField("email");

        await Assert.That(summary.IsValid).IsFalse();
        await Assert.That(summary.HasWarnings).IsTrue();
        await Assert.That(summary.IsPending).IsTrue();
        await Assert.That(summary.BlockingCount).IsEqualTo(1);
        await Assert.That(summary.WarningCount).IsEqualTo(1);
        await Assert.That(summary.SummaryText).IsEqualTo("1 error, 1 warning, 1 pending");
        await Assert.That(emailMessages).Count().IsEqualTo(1);
        await Assert.That(emailMessages[0].Message).IsEqualTo("Required");
    }

    [Test]
    public async Task PaginationState_MiddlePage_ReportsRangesAndNavigationAvailability()
    {
        var state = new PaginationState(pageIndex: 2, pageSize: 25, totalItemCount: 103);

        await Assert.That(state.PageNumber).IsEqualTo(3);
        await Assert.That(state.TotalPages).IsEqualTo(5);
        await Assert.That(state.CanGoFirst).IsTrue();
        await Assert.That(state.CanGoPrevious).IsTrue();
        await Assert.That(state.CanGoNext).IsTrue();
        await Assert.That(state.CanGoLast).IsTrue();
        await Assert.That(state.FirstItemNumber).IsEqualTo(51);
        await Assert.That(state.LastItemNumber).IsEqualTo(75);
        await Assert.That(state.SummaryText).IsEqualTo("51-75 of 103");
    }

    [Test]
    public async Task PaginationState_EmptyCollection_ClampsToSingleDisplayPage()
    {
        var state = new PaginationState(pageIndex: 5, pageSize: 20, totalItemCount: 0);

        await Assert.That(state.PageIndex).IsEqualTo(0);
        await Assert.That(state.PageNumber).IsEqualTo(1);
        await Assert.That(state.TotalPages).IsEqualTo(1);
        await Assert.That(state.HasItems).IsFalse();
        await Assert.That(state.CanGoNext).IsFalse();
        await Assert.That(state.SummaryText).IsEqualTo("No items");
    }

    [Test]
    public async Task PageRequest_WithSearchAndFilters_CapturesStableDataQuerySnapshot()
    {
        var filters = new[]
        {
            new FilterToken("status", FilterOperator.Equals, "open", "Status: Open")
        };
        var query = new SearchQueryState(" alarm ", submittedText: "alarm", filters: filters);
        var request = new PageRequest(pageIndex: 1, pageSize: 50, sortKey: "created", sortDescending: true, queryState: query);

        await Assert.That(request.PageIndex).IsEqualTo(1);
        await Assert.That(request.Offset).IsEqualTo(50);
        await Assert.That(request.HasSort).IsTrue();
        await Assert.That(request.HasQuery).IsTrue();
        await Assert.That(request.ActiveFilters).Count().IsEqualTo(1);
        await Assert.That(request.FilterSnapshotKey).IsEqualTo("status:Equals:open");
    }

    [Test]
    public async Task StepDescriptor_OptionalStepWithWarning_ReportsAvailabilityAndValidationState()
    {
        var messages = new[]
        {
            new ValidationMessage("setup", "Setup", "Recommended before import", ValidationSeverity.Warning)
        };
        var step = new StepDescriptor(
            "setup",
            "Setup connection",
            StepStatus.Warning,
            isOptional: true,
            isEnabled: true,
            canEnter: true,
            canLeave: false,
            validationMessages: messages);

        await Assert.That(step.Key).IsEqualTo("setup");
        await Assert.That(step.DisplayTitle).IsEqualTo("Setup connection (optional)");
        await Assert.That(step.IsBlocking).IsFalse();
        await Assert.That(step.HasValidationMessages).IsTrue();
        await Assert.That(step.IsAvailable).IsTrue();
        await Assert.That(step.CanLeave).IsFalse();
    }

    [Test]
    public async Task StepperState_CurrentMiddleStep_ReportsNavigationAndBlockingCounts()
    {
        var steps = new[]
        {
            new StepDescriptor("source", "Source", StepStatus.Completed),
            new StepDescriptor("mapping", "Mapping", StepStatus.Active),
            new StepDescriptor(
                "review",
                "Review",
                StepStatus.Error,
                validationMessages: new[] { new ValidationMessage("review", "Review", "Resolve duplicate columns", ValidationSeverity.Error) }),
            new StepDescriptor("finish", "Finish", StepStatus.Pending, isEnabled: false)
        };
        var state = new StepperState(steps, "mapping", StepperOrientation.Vertical);

        await Assert.That(state.CurrentIndex).IsEqualTo(1);
        await Assert.That(state.CurrentStep?.Key).IsEqualTo("mapping");
        await Assert.That(state.CompletedCount).IsEqualTo(1);
        await Assert.That(state.BlockingStepCount).IsEqualTo(1);
        await Assert.That(state.CanGoPrevious).IsTrue();
        await Assert.That(state.CanGoNext).IsFalse();
        await Assert.That(state.CanFinish).IsFalse();
        await Assert.That(state.GetStep("source")?.IsComplete).IsTrue();
    }

    [Test]
    public async Task DateTimeRange_ReversedCustomRange_IsInvalidAndReportsDuration()
    {
        var start = new DateTimeOffset(2026, 5, 13, 12, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(-2);
        var range = new DateTimeRange(start, end, DateTimeRangePreset.Custom, "Manual");

        await Assert.That(range.IsValid).IsFalse();
        await Assert.That(range.HasValue).IsTrue();
        await Assert.That(range.Duration).IsEqualTo(TimeSpan.Zero);
        await Assert.That(range.ValidationMessage).IsEqualTo("Start must be before or equal to end.");
        await Assert.That(range.DisplayText).IsEqualTo("Manual: invalid range");
    }

    [Test]
    public async Task DateTimeRangePresetDefinition_LastSevenDays_CreatesInclusiveRangeEndingAtReferenceTime()
    {
        var reference = new DateTimeOffset(2026, 5, 13, 9, 30, 0, TimeSpan.Zero);
        var preset = DateTimeRangePresetDefinition.LastSevenDays;
        var range = preset.CreateRange(reference);

        await Assert.That(range.Preset).IsEqualTo(DateTimeRangePreset.LastSevenDays);
        await Assert.That(range.Start).IsEqualTo(reference.AddDays(-7));
        await Assert.That(range.End).IsEqualTo(reference);
        await Assert.That(range.IsValid).IsTrue();
        await Assert.That(range.DisplayText).IsEqualTo("Last 7 days: 2026-05-06 09:30 - 2026-05-13 09:30");
    }

    [Test]
    public async Task ThemePreferenceState_SystemPreference_UsesSystemChoiceAsEffectiveTheme()
    {
        var state = new ThemePreferenceState(ThemeChoice.System, ThemeChoice.Dark, supportsHighContrast: false);

        await Assert.That(state.SelectedChoice).IsEqualTo(ThemeChoice.System);
        await Assert.That(state.SystemChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(state.EffectiveChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(state.IsSystemSelected).IsTrue();
        await Assert.That(state.SupportsChoice(ThemeChoice.HighContrast)).IsFalse();
        await Assert.That(state.AvailableChoices).Count().IsEqualTo(3);
        await Assert.That(state.DisplayText).IsEqualTo("System (Dark)");
    }

    [Test]
    public async Task ThemePreferenceState_UnsupportedHighContrast_FallsBackToSystemChoice()
    {
        var state = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Light, supportsHighContrast: false);

        await Assert.That(state.SelectedChoice).IsEqualTo(ThemeChoice.HighContrast);
        await Assert.That(state.EffectiveChoice).IsEqualTo(ThemeChoice.Light);
        await Assert.That(state.IsHighContrastEffective).IsFalse();
        await Assert.That(state.DisplayText).IsEqualTo("High contrast (using Light)");
    }

    [Test]
    public async Task FilterDescriptor_EnumDescriptor_CreatesStableDisplayToken()
    {
        var descriptor = new FilterDescriptor(
            "status",
            "Status",
            FilterEditorKind.Enum,
            new[] { FilterOperator.Equals, FilterOperator.NotEquals },
            new[] { "Open", "Closed" });

        var token = descriptor.CreateToken("Open");

        await Assert.That(descriptor.DefaultOperator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(descriptor.HasChoices).IsTrue();
        await Assert.That(descriptor.SupportsOperator(FilterOperator.Contains)).IsFalse();
        await Assert.That(token.Key).IsEqualTo("status:Equals:Open");
        await Assert.That(token.DisplayText).IsEqualTo("Status equals Open");
    }

    [Test]
    public async Task DataFilterPanelState_ActiveExpressions_ProjectsTokensAndSearchState()
    {
        var descriptors = new[]
        {
            new FilterDescriptor("status", "Status", FilterEditorKind.Enum, new[] { FilterOperator.Equals }, new[] { "Open", "Closed" }),
            new FilterDescriptor("created", "Created", FilterEditorKind.DateRange, new[] { FilterOperator.Between }),
            new FilterDescriptor("notes", "Notes", FilterEditorKind.Text, new[] { FilterOperator.Contains })
        };
        var expressions = new[]
        {
            new FilterExpression("status", FilterOperator.Equals, "Open"),
            new FilterExpression("created", FilterOperator.Between, "Today"),
            new FilterExpression("notes", FilterOperator.Contains, string.Empty)
        };

        var state = new DataFilterPanelState(descriptors, expressions, isDirty: true);
        var queryState = state.ToSearchQueryState(" alarm ", resultCount: 4);

        await Assert.That(state.DescriptorCount).IsEqualTo(3);
        await Assert.That(state.ActiveFilterCount).IsEqualTo(2);
        await Assert.That(state.CanApply).IsTrue();
        await Assert.That(state.CanClear).IsTrue();
        await Assert.That(state.ActiveTokens).Count().IsEqualTo(2);
        await Assert.That(state.ActiveTokens[0].DisplayText).IsEqualTo("Status equals Open");
        await Assert.That(state.GetDescriptor("created")?.EditorKind).IsEqualTo(FilterEditorKind.DateRange);
        await Assert.That(queryState.NormalizedText).IsEqualTo("alarm");
        await Assert.That(queryState.ResultSummary).IsEqualTo("4 results");
        await Assert.That(queryState.ActiveFilters).Count().IsEqualTo(2);
    }

    [Test]
    public async Task PropertyDescriptorModel_ModifiedInvalidDescriptor_ReportsEditableState()
    {
        var messages = new[]
        {
            new ValidationMessage("retryCount", "Retry count", "Must be between 1 and 3", ValidationSeverity.Error)
        };
        var resetCommand = new TestCommand();
        var descriptor = new PropertyDescriptorModel(
            "retryCount",
            "Retry count",
            "Connection",
            PropertyEditorKind.Number,
            value: 5,
            originalValue: 3,
            isReadOnly: false,
            resetCommand: resetCommand,
            validationMessages: messages);

        await Assert.That(descriptor.HasValue).IsTrue();
        await Assert.That(descriptor.ValueDisplayText).IsEqualTo("5");
        await Assert.That(descriptor.IsModified).IsTrue();
        await Assert.That(descriptor.IsInvalid).IsTrue();
        await Assert.That(descriptor.CanEdit).IsTrue();
        await Assert.That(descriptor.CanReset).IsTrue();
    }

    [Test]
    public async Task PropertyGridState_SearchAndCategories_ReportsVisibleEditableInvalidDescriptors()
    {
        var descriptors = new[]
        {
            new PropertyDescriptorModel("endpoint", "Endpoint", "Connection", PropertyEditorKind.Text, "opc.tcp://localhost", "opc.tcp://localhost"),
            new PropertyDescriptorModel("timeout", "Timeout", "Connection", PropertyEditorKind.Number, 5, 3),
            new PropertyDescriptorModel(
                "mode",
                "Mode",
                "Advanced",
                PropertyEditorKind.Enum,
                "Manual",
                "Automatic",
                choices: new[] { "Automatic", "Manual" },
                validationMessages: new[] { new ValidationMessage("mode", "Mode", "Manual mode is unavailable", ValidationSeverity.Error) })
        };

        var state = new PropertyGridState(descriptors, "connection");

        await Assert.That(state.DescriptorCount).IsEqualTo(3);
        await Assert.That(state.VisibleDescriptorCount).IsEqualTo(2);
        await Assert.That(state.EditableDescriptorCount).IsEqualTo(3);
        await Assert.That(state.ModifiedDescriptorCount).IsEqualTo(2);
        await Assert.That(state.InvalidDescriptorCount).IsEqualTo(1);
        await Assert.That(state.CanCommit).IsFalse();
        await Assert.That(state.Categories).Count().IsEqualTo(1);
        await Assert.That(state.Categories[0].Name).IsEqualTo("Connection");
        await Assert.That(state.GetDescriptor("timeout")?.IsModified).IsTrue();
    }

    private sealed class TestCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
