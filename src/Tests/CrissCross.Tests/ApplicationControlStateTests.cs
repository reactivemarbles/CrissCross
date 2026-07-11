// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross.Tests;

/// <summary>Tests for platform-neutral application control state models shared by UI stacks.</summary>
public class ApplicationControlStateTests
{
    /// <summary>Provides the determinate progress value.</summary>
    private const double DeterminateProgress = 0.42;

    /// <summary>Provides the priority filter threshold.</summary>
    private const int PriorityFilterThreshold = 2;

    /// <summary>Provides the expected active filter count.</summary>
    private const int ExpectedActiveFilterCount = 2;

    /// <summary>Provides the expected selected chip count.</summary>
    private const int ExpectedSelectedChipCount = 2;

    /// <summary>Provides the expected removable chip count.</summary>
    private const int ExpectedRemovableChipCount = 2;

    /// <summary>Provides the expected enabled segment count.</summary>
    private const int ExpectedEnabledSegmentCount = 2;

    /// <summary>Provides the middle page index.</summary>
    private const int MiddlePageIndex = 2;

    /// <summary>Provides the middle page size.</summary>
    private const int MiddlePageSize = 25;

    /// <summary>Provides the middle page total item count.</summary>
    private const int MiddlePageTotalItemCount = 103;

    /// <summary>Provides the expected middle page number.</summary>
    private const int ExpectedMiddlePageNumber = 3;

    /// <summary>Provides the expected middle page total pages.</summary>
    private const int ExpectedMiddlePageTotalPages = 5;

    /// <summary>Provides the expected first item number.</summary>
    private const int ExpectedMiddlePageFirstItemNumber = 51;

    /// <summary>Provides the expected last item number.</summary>
    private const int ExpectedMiddlePageLastItemNumber = 75;

    /// <summary>Provides the page request page size.</summary>
    private const int PageRequestPageSize = 50;

    /// <summary>Provides the reversed range hour offset.</summary>
    private const int ReversedRangeHourOffset = -2;

    /// <summary>Provides the last seven days offset.</summary>
    private const int LastSevenDaysOffset = -7;

    /// <summary>Provides the expected theme choice count.</summary>
    private const int ExpectedThemeChoiceCount = 3;

    /// <summary>Provides the expected descriptor count.</summary>
    private const int ExpectedDescriptorCount = 3;

    /// <summary>Provides the expected visible descriptor count.</summary>
    private const int ExpectedVisibleDescriptorCount = 2;

    /// <summary>Provides the expected active token count.</summary>
    private const int ExpectedActiveTokenCount = 2;

    /// <summary>Provides the modified property value.</summary>
    private const int ModifiedPropertyValue = 5;

    /// <summary>Provides the original property value.</summary>
    private const int OriginalPropertyValue = 3;

    /// <summary>Provides the expected modified descriptor count.</summary>
    private const int ExpectedModifiedDescriptorCount = 2;

    /// <summary>Gets a symbol for email display-name assertions.</summary>
    private static string Email => nameof(Email);

    /// <summary>Gets a symbol for timeout display-name assertions.</summary>
    private static string Timeout => nameof(Timeout);

    /// <summary>Provides the CommandButtonStatus_Executing_IsNotInteractiveAndHasNoError member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CommandButtonStatus_Executing_IsNotInteractiveAndHasNoError()
    {
        var status = CommandButtonStatus.Executing(canExecute: true);

        await Assert.That(status.State).IsEqualTo(CommandButtonState.Executing);
        await Assert.That(status.IsExecuting).IsTrue();
        await Assert.That(status.IsInteractive).IsFalse();
        await Assert.That(status.HasError).IsFalse();
    }

    /// <summary>Provides the CommandButtonStatus_Failed_ExposesErrorAndStaysInteractiveWhenCommandCanExecute member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the BusyOperation_DeterminateProgress_IsActiveDeterminateAndCancellable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BusyOperation_DeterminateProgress_IsActiveDeterminateAndCancellable()
    {
        var cancelCommand = new TestCommand();
        var operation = new BusyOperation("Importing", "42 items", DeterminateProgress, cancelCommand);

        await Assert.That(operation.IsActive).IsTrue();
        await Assert.That(operation.IsDeterminate).IsTrue();
        await Assert.That(operation.IsCancellable).IsTrue();
        await Assert.That(operation.Progress).IsEqualTo(DeterminateProgress);
    }

    /// <summary>Provides the EmptyStateModel_WithPrimaryAction_ReportsActionAvailability member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the SearchQueryState_WithTextAndFilters_ExposesNormalizedQueryAndActiveFilterCount member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SearchQueryState_WithTextAndFilters_ExposesNormalizedQueryAndActiveFilterCount()
    {
        var filters = new[]
        {
            new FilterToken("status", FilterOperator.Equals, "open", "Status: Open"),
            new FilterToken("priority", FilterOperator.GreaterThanOrEqual, PriorityFilterThreshold, "Priority >= 2")
        };

        var state = new SearchQueryState("  pump fault  ", "pump", "pump fault", isSearching: true, resultCount: 12, filters: filters);

        await Assert.That(state.NormalizedText).IsEqualTo("pump fault");
        await Assert.That(state.HasQuery).IsTrue();
        await Assert.That(state.IsFiltered).IsTrue();
        await Assert.That(state.ActiveFilterCount).IsEqualTo(ExpectedActiveFilterCount);
        await Assert.That(state.ResultSummary).IsEqualTo("12 results");
    }

    /// <summary>Provides the FilterToken_RemovableToken_UsesDisplayTextAndStableKey member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterToken_RemovableToken_UsesDisplayTextAndStableKey()
    {
        var token = new FilterToken("area", FilterOperator.Contains, "north", "Area contains north", isRemovable: true);

        await Assert.That(token.Key).IsEqualTo("area:Contains:north");
        await Assert.That(token.DisplayText).IsEqualTo("Area contains north");
        await Assert.That(token.IsRemovable).IsTrue();
    }

    /// <summary>Provides the ChipModel_RemovableSelectedChip_ExposesInteractionStateAndStableKey member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the ChipGroupState_MultipleSelection_ReportsSelectedAndRemovableChips member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await Assert.That(group.SelectedChips).Count().IsEqualTo(ExpectedSelectedChipCount);
        await Assert.That(group.RemovableChips).Count().IsEqualTo(ExpectedRemovableChipCount);
        await Assert.That(group.GetChip("open")?.Text).IsEqualTo("Open");
    }

    /// <summary>Provides the SegmentedSelectionState_SingleSelection_ExposesSelectedItemAndEnabledItems member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
        await Assert.That(state.EnabledItems).Count().IsEqualTo(ExpectedEnabledSegmentCount);
        await Assert.That(state.GetItem("list")?.HasIcon).IsTrue();
    }

    /// <summary>Provides the ValidationMessage_Error_IsBlockingAndUsesExplicitDisplayText member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the ValidationSummaryState_MixedMessages_ReportsValidityCountsAndFieldMessages member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ValidationSummaryState_MixedMessages_ReportsValidityCountsAndFieldMessages()
    {
        var messages = new[]
        {
            new ValidationMessage("email", nameof(Email), "Required", ValidationSeverity.Error),
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

    /// <summary>Provides the PaginationState_MiddlePage_ReportsRangesAndNavigationAvailability member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PaginationState_MiddlePage_ReportsRangesAndNavigationAvailability()
    {
        var state = new PaginationState(pageIndex: MiddlePageIndex, pageSize: MiddlePageSize, totalItemCount: MiddlePageTotalItemCount);

        await Assert.That(state.PageNumber).IsEqualTo(ExpectedMiddlePageNumber);
        await Assert.That(state.TotalPages).IsEqualTo(ExpectedMiddlePageTotalPages);
        await Assert.That(state.CanGoFirst).IsTrue();
        await Assert.That(state.CanGoPrevious).IsTrue();
        await Assert.That(state.CanGoNext).IsTrue();
        await Assert.That(state.CanGoLast).IsTrue();
        await Assert.That(state.FirstItemNumber).IsEqualTo(ExpectedMiddlePageFirstItemNumber);
        await Assert.That(state.LastItemNumber).IsEqualTo(ExpectedMiddlePageLastItemNumber);
        await Assert.That(state.SummaryText).IsEqualTo("51-75 of 103");
    }

    /// <summary>Provides the PaginationState_EmptyCollection_ClampsToSingleDisplayPage member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the PageRequest_WithSearchAndFilters_CapturesStableDataQuerySnapshot member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PageRequest_WithSearchAndFilters_CapturesStableDataQuerySnapshot()
    {
        var filters = new[]
        {
            new FilterToken("status", FilterOperator.Equals, "open", "Status: Open")
        };
        var query = new SearchQueryState(" alarm ", submittedText: "alarm", filters: filters);
        var request = new PageRequest(pageIndex: 1, pageSize: PageRequestPageSize, sortKey: "created", sortDescending: true, queryState: query);

        await Assert.That(request.PageIndex).IsEqualTo(1);
        await Assert.That(request.Offset).IsEqualTo(PageRequestPageSize);
        await Assert.That(request.HasSort).IsTrue();
        await Assert.That(request.HasQuery).IsTrue();
        await Assert.That(request.ActiveFilters).Count().IsEqualTo(1);
        await Assert.That(request.FilterSnapshotKey).IsEqualTo("status:Equals:open");
    }

    /// <summary>Provides the StepDescriptor_OptionalStepWithWarning_ReportsAvailabilityAndValidationState member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the StepperState_CurrentMiddleStep_ReportsNavigationAndBlockingCounts member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
                validationMessages: [new ValidationMessage("review", "Review", "Resolve duplicate columns", ValidationSeverity.Error)]),
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

    /// <summary>Provides the DateTimeRange_ReversedCustomRange_IsInvalidAndReportsDuration member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRange_ReversedCustomRange_IsInvalidAndReportsDuration()
    {
        var start = new DateTimeOffset(2026, 5, 13, 12, 0, 0, TimeSpan.Zero);
        var end = start.AddHours(ReversedRangeHourOffset);
        var range = new DateTimeRange(start, end, DateTimeRangePreset.Custom, "Manual");

        await Assert.That(range.IsValid).IsFalse();
        await Assert.That(range.HasValue).IsTrue();
        await Assert.That(range.Duration).IsEqualTo(TimeSpan.Zero);
        await Assert.That(range.ValidationMessage).IsEqualTo("Start must be before or equal to end.");
        await Assert.That(range.DisplayText).IsEqualTo("Manual: invalid range");
    }

    /// <summary>Provides the DateTimeRangePresetDefinition_LastSevenDays_CreatesInclusiveRangeEndingAtReferenceTime member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRangePresetDefinition_LastSevenDays_CreatesInclusiveRangeEndingAtReferenceTime()
    {
        var reference = new DateTimeOffset(2026, 5, 13, 9, 30, 0, TimeSpan.Zero);
        var preset = DateTimeRangePresetDefinition.LastSevenDays;
        var range = preset.CreateRange(reference);

        await Assert.That(range.Preset).IsEqualTo(DateTimeRangePreset.LastSevenDays);
        await Assert.That(range.Start).IsEqualTo(reference.AddDays(LastSevenDaysOffset));
        await Assert.That(range.End).IsEqualTo(reference);
        await Assert.That(range.IsValid).IsTrue();
        await Assert.That(range.DisplayText).IsEqualTo("Last 7 days: 2026-05-06 09:30 - 2026-05-13 09:30");
    }

    /// <summary>Provides the ThemePreferenceState_SystemPreference_UsesSystemChoiceAsEffectiveTheme member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ThemePreferenceState_SystemPreference_UsesSystemChoiceAsEffectiveTheme()
    {
        var state = new ThemePreferenceState(ThemeChoice.System, ThemeChoice.Dark, supportsHighContrast: false);

        await Assert.That(state.SelectedChoice).IsEqualTo(ThemeChoice.System);
        await Assert.That(state.SystemChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(state.EffectiveChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(state.IsSystemSelected).IsTrue();
        await Assert.That(state.SupportsChoice(ThemeChoice.HighContrast)).IsFalse();
        await Assert.That(state.AvailableChoices).Count().IsEqualTo(ExpectedThemeChoiceCount);
        await Assert.That(state.DisplayText).IsEqualTo("System (Dark)");
    }

    /// <summary>Provides the ThemePreferenceState_UnsupportedHighContrast_FallsBackToSystemChoice member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ThemePreferenceState_UnsupportedHighContrast_FallsBackToSystemChoice()
    {
        var state = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Light, supportsHighContrast: false);

        await Assert.That(state.SelectedChoice).IsEqualTo(ThemeChoice.HighContrast);
        await Assert.That(state.EffectiveChoice).IsEqualTo(ThemeChoice.Light);
        await Assert.That(state.IsHighContrastEffective).IsFalse();
        await Assert.That(state.DisplayText).IsEqualTo("High contrast (using Light)");
    }

    /// <summary>Provides the FilterDescriptor_EnumDescriptor_CreatesStableDisplayToken member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterDescriptor_EnumDescriptor_CreatesStableDisplayToken()
    {
        var descriptor = new FilterDescriptor(
            "status",
            "Status",
            FilterEditorKind.Enum,
            [FilterOperator.Equals, FilterOperator.NotEquals],
            ["Open", "Closed"]);

        var token = descriptor.CreateToken("Open");

        await Assert.That(descriptor.DefaultOperator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(descriptor.HasChoices).IsTrue();
        await Assert.That(descriptor.SupportsOperator(FilterOperator.Contains)).IsFalse();
        await Assert.That(token.Key).IsEqualTo("status:Equals:Open");
        await Assert.That(token.DisplayText).IsEqualTo("Status equals Open");
    }

    /// <summary>Provides the DataFilterPanelState_ActiveExpressions_ProjectsTokensAndSearchState member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataFilterPanelState_ActiveExpressions_ProjectsTokensAndSearchState()
    {
        var descriptors = new[]
        {
            new FilterDescriptor("status", "Status", FilterEditorKind.Enum, [FilterOperator.Equals], ["Open", "Closed"]),
            new FilterDescriptor("created", "Created", FilterEditorKind.DateRange, [FilterOperator.Between]),
            new FilterDescriptor("notes", "Notes", FilterEditorKind.Text, [FilterOperator.Contains])
        };
        var expressions = new[]
        {
            new FilterExpression("status", FilterOperator.Equals, "Open"),
            new FilterExpression("created", FilterOperator.Between, "Today"),
            new FilterExpression("notes", FilterOperator.Contains, string.Empty)
        };

        var state = new DataFilterPanelState(descriptors, expressions, isDirty: true);
        var queryState = state.ToSearchQueryState(" alarm ", resultCount: 4);

        await Assert.That(state.DescriptorCount).IsEqualTo(ExpectedDescriptorCount);
        await Assert.That(state.ActiveFilterCount).IsEqualTo(ExpectedActiveFilterCount);
        await Assert.That(state.CanApply).IsTrue();
        await Assert.That(state.CanClear).IsTrue();
        await Assert.That(state.ActiveTokens).Count().IsEqualTo(ExpectedActiveTokenCount);
        await Assert.That(state.ActiveTokens[0].DisplayText).IsEqualTo("Status equals Open");
        await Assert.That(state.GetDescriptor("created")?.EditorKind).IsEqualTo(FilterEditorKind.DateRange);
        await Assert.That(queryState.NormalizedText).IsEqualTo("alarm");
        await Assert.That(queryState.ResultSummary).IsEqualTo("4 results");
        await Assert.That(queryState.ActiveFilters).Count().IsEqualTo(ExpectedActiveFilterCount);
    }

    /// <summary>Provides the PropertyDescriptorModel_ModifiedInvalidDescriptor_ReportsEditableState member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
            value: ModifiedPropertyValue,
            originalValue: OriginalPropertyValue,
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

    /// <summary>Provides the PropertyGridState_SearchAndCategories_ReportsVisibleEditableInvalidDescriptors member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PropertyGridState_SearchAndCategories_ReportsVisibleEditableInvalidDescriptors()
    {
        var descriptors = new[]
        {
            new PropertyDescriptorModel("endpoint", "Endpoint", "Connection", PropertyEditorKind.Text, "opc.tcp://localhost", "opc.tcp://localhost"),
            new PropertyDescriptorModel("timeout", nameof(Timeout), "Connection", PropertyEditorKind.Number, ModifiedPropertyValue, OriginalPropertyValue),
            new PropertyDescriptorModel(
                "mode",
                "Mode",
                "Advanced",
                PropertyEditorKind.Enum,
                "Manual",
                "Automatic",
                choices: ["Automatic", "Manual"],
                validationMessages: [new ValidationMessage("mode", "Mode", "Manual mode is unavailable", ValidationSeverity.Error)])
        };

        var state = new PropertyGridState(descriptors, "connection");

        await Assert.That(state.DescriptorCount).IsEqualTo(ExpectedDescriptorCount);
        await Assert.That(state.VisibleDescriptorCount).IsEqualTo(ExpectedVisibleDescriptorCount);
        await Assert.That(state.EditableDescriptorCount).IsEqualTo(ExpectedDescriptorCount);
        await Assert.That(state.ModifiedDescriptorCount).IsEqualTo(ExpectedModifiedDescriptorCount);
        await Assert.That(state.InvalidDescriptorCount).IsEqualTo(1);
        await Assert.That(state.CanCommit).IsFalse();
        await Assert.That(state.Categories).Count().IsEqualTo(1);
        await Assert.That(state.Categories[0].Name).IsEqualTo("Connection");
        await Assert.That(state.GetDescriptor("timeout")?.IsModified).IsTrue();
    }

    /// <summary>Provides the TestCommand member.</summary>
    private sealed class TestCommand : ICommand
    {
        /// <summary>Provides the CanExecuteChanged member.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
