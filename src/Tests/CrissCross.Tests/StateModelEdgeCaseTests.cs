// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Exercises boundary behavior for platform-neutral state projections.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class StateModelEdgeCaseTests
{
    /// <summary>Provides the number of ticks in the final instant of a day.</summary>
    private const long LastDayTickOffset = 9_999_999;

    /// <summary>Provides the trailing seven day offset.</summary>
    private const int LastSevenDayOffset = -7;

    /// <summary>Provides the expected active filter threshold.</summary>
    private const int PriorityThreshold = 2;

    /// <summary>Provides the expected query result count.</summary>
    private const int ResultCount = 4;

    /// <summary>Provides the number of non-high-contrast theme choices.</summary>
    private const int StandardThemeChoiceCount = 3;

    /// <summary>Provides the navigation request parameter.</summary>
    private const int NavigationParameter = 42;

    /// <summary>Provides a date range maximum in hours.</summary>
    private const int MaximumRangeHours = 1;

    /// <summary>Provides the canonical missing lookup key.</summary>
    private const string MissingKey = "missing";

    /// <summary>Provides a contract that requires normalization.</summary>
    private const string PaddedDetailContract = " detail ";

    /// <summary>Provides the normalized detail contract.</summary>
    private const string DetailContract = "detail";

    /// <summary>Gets the email field symbol used in field-key tests.</summary>
    private static string Email => nameof(Email);

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Verifies each relative date range preset resolves using the supplied offset.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateRangePresets_ResolveAllConcreteAndCustomRanges()
    {
        var reference = new DateTimeOffset(2026, 7, 26, 14, 30, 0, TimeSpan.FromHours(1));
        var today = DateTimeRangePresetDefinition.Today.CreateRange(reference);
        var yesterday = DateTimeRangePresetDefinition.Yesterday.CreateRange(reference);
        var lastSevenDays = DateTimeRangePresetDefinition.LastSevenDays.CreateRange(reference);
        var thisMonth = DateTimeRangePresetDefinition.ThisMonth.CreateRange(reference);
        var custom = DateTimeRangePresetDefinition.Custom.CreateRange(reference);

        await Assert.That(today.Start).IsEqualTo(new DateTimeOffset(2026, 7, 26, 0, 0, 0, TimeSpan.FromHours(1)));
        await Assert.That(today.End).IsEqualTo(reference);
        await Assert.That(yesterday.Start).IsEqualTo(new DateTimeOffset(2026, 7, 25, 0, 0, 0, TimeSpan.FromHours(1)));
        await Assert.That(yesterday.End).IsEqualTo(new DateTimeOffset(2026, 7, 25, 23, 59, 59, TimeSpan.FromHours(1)).AddTicks(LastDayTickOffset));
        await Assert.That(lastSevenDays.Start).IsEqualTo(reference.AddDays(LastSevenDayOffset));
        await Assert.That(lastSevenDays.End).IsEqualTo(reference);
        await Assert.That(thisMonth.Start).IsEqualTo(new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.FromHours(1)));
        await Assert.That(thisMonth.End).IsEqualTo(reference);
        await Assert.That(custom.Start).IsNull();
        await Assert.That(custom.End).IsNull();
        await Assert.That(custom.Label).IsEqualTo(nameof(DateTimeRangePreset.Custom));
    }

    /// <summary>Verifies filter descriptors choose every default operator family and format values predictably.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterDescriptors_ProjectDefaultsChoicesAndAllValueFormats()
    {
        var text = new FilterDescriptor("text", "Text", FilterEditorKind.Text);
        var number = new FilterDescriptor("number", "Number", FilterEditorKind.Number);
        var date = new FilterDescriptor("date", "Date", FilterEditorKind.Date);
        var dateTime = new FilterDescriptor("dateTime", "Date time", FilterEditorKind.DateTime);
        var dateRange = new FilterDescriptor(nameof(Range).ToLowerInvariant(), nameof(Range), FilterEditorKind.DateRange);
        var choice = new FilterDescriptor("choice", "Choice", FilterEditorKind.Enum, [FilterOperator.NotEquals], ["A", "B"], "A", true);

        await Assert.That(text.SupportedOperators).IsEquivalentTo([FilterOperator.Contains, FilterOperator.Equals, FilterOperator.StartsWith, FilterOperator.EndsWith]);
        await Assert.That(number.SupportedOperators).IsEquivalentTo(
            [FilterOperator.Equals, FilterOperator.GreaterThan, FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan, FilterOperator.LessThanOrEqual]);
        await Assert.That(date.SupportedOperators).IsEquivalentTo([FilterOperator.Equals, FilterOperator.GreaterThanOrEqual, FilterOperator.LessThanOrEqual]);
        await Assert.That(dateTime.DefaultOperator).IsEqualTo(FilterOperator.Equals);
        await Assert.That(dateRange.DefaultOperator).IsEqualTo(FilterOperator.Between);
        await Assert.That(choice.HasChoices).IsTrue();
        await Assert.That(choice.IsRequired).IsTrue();
        await Assert.That(choice.SupportsOperator(FilterOperator.NotEquals)).IsTrue();
        await Assert.That(choice.SupportsOperator(FilterOperator.Equals)).IsFalse();
        await Assert.That(text.CreateToken("needle").DisplayText).IsEqualTo("Text contains needle");
        await Assert.That(choice.CreateToken(null, FilterOperator.NotEquals, false).DisplayText).IsEqualTo("Choice does not equal ");
        var localDateTime = new DateTime(2026, 7, 26, 14, 30, 0, DateTimeKind.Utc);
        var offsetDateTime = new DateTimeOffset(2026, 7, 26, 14, 30, 0, TimeSpan.Zero);
        await Assert.That(date.CreateToken(localDateTime, FilterOperator.LessThan).DisplayText)
            .IsEqualTo("Date is less than 2026-07-26 14:30");
        await Assert.That(dateTime.CreateToken(offsetDateTime, FilterOperator.GreaterThan).DisplayText)
            .IsEqualTo("Date time is greater than 2026-07-26 14:30");
    }

    /// <summary>Verifies expression activity and panel summary branches are projected from real filters.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterPanel_ProjectsActiveExpressionsAndQueryState()
    {
        var descriptor = new FilterDescriptor("priority", "Priority", FilterEditorKind.Number);
        var disabled = new FilterExpression("ignored", FilterOperator.Equals, "value", isEnabled: false);
        var blank = new FilterExpression("empty", FilterOperator.Contains, "  ");
        var active = new FilterExpression("priority", FilterOperator.GreaterThan, PriorityThreshold);
        var panel = new DataFilterPanelState([descriptor], [disabled, blank, active], isDirty: true, isApplying: false);
        var applying = new DataFilterPanelState([descriptor], [active], isDirty: true, isApplying: true);
        var empty = new DataFilterPanelState();

        await Assert.That(disabled.IsActive).IsFalse();
        await Assert.That(blank.IsActive).IsFalse();
        await Assert.That(active.IsActive).IsTrue();
        await Assert.That(panel.ActiveFilterCount).IsEqualTo(1);
        await Assert.That(panel.ActiveTokens[0].DisplayText).IsEqualTo("Priority is greater than 2");
        await Assert.That(panel.CanApply).IsTrue();
        await Assert.That(panel.CanClear).IsTrue();
        await Assert.That(panel.SummaryText).IsEqualTo("1 active filter");
        await Assert.That(panel.ToSearchQueryState(" alarm ", ResultCount).NormalizedText).IsEqualTo("alarm");
        await Assert.That(applying.CanApply).IsFalse();
        await Assert.That(applying.CanClear).IsFalse();
        await Assert.That(empty.SummaryText).IsEqualTo("No filters");
        await Assert.That(empty.GetDescriptor(MissingKey)).IsNull();
        await Assert.That(blank.ToToken().DisplayText).IsEqualTo("empty Contains   ");
    }

    /// <summary>Verifies property descriptor formatting, update snapshots, grouping, and every grid summary branch.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PropertyInspector_ProjectsFormattingValidationAndSummaryBranches()
    {
        var reset = new TestCommand();
        var valid = new PropertyDescriptorModel(
            " enabled ",
            " Enabled ",
            new PropertyDescriptorOptions { Category = "  General ", Value = true, OriginalValue = false, ResetCommand = reset, TemplateKey = " editor ", Choices = [true, false] });
        var pending = new PropertyDescriptorModel(
            "when",
            "When",
            new PropertyDescriptorOptions
            {
                Category = "Timing", Value = new DateTimeOffset(2026, 7, 26, 14, 30, 0, TimeSpan.Zero),
                OriginalValue = null, IsReadOnly = true,
                ValidationMessages = [new ValidationMessage("when", "When", "Loading", ValidationSeverity.Pending)],
            });
        var invalid = new PropertyDescriptorModel(
            "name",
            "Name",
            new PropertyDescriptorOptions { Value = string.Empty, OriginalValue = string.Empty, ValidationMessages = [new ValidationMessage("name", "Name", "Required")] });
        var group = new PropertyDescriptorGroup(string.Empty, [valid, invalid]);
        var empty = new PropertyGridState();
        var modified = new PropertyGridState([valid]);
        var errors = new PropertyGridState([valid, invalid], " general ");
        var committing = new PropertyGridState([valid], null, true);

        await Assert.That(valid.Key).IsEqualTo("enabled");
        await Assert.That(valid.DisplayName).IsEqualTo("Enabled");
        await Assert.That(valid.CategoryKey).IsEqualTo("General:enabled");
        await Assert.That(valid.ValueDisplayText).IsEqualTo("True");
        await Assert.That(valid.HasChoices).IsTrue();
        await Assert.That(valid.CanReset).IsTrue();
        await Assert.That(valid.WithValue(false).IsModified).IsFalse();
        await Assert.That(pending.ValueDisplayText).IsEqualTo("2026-07-26 14:30");
        await Assert.That(pending.IsPending).IsTrue();
        await Assert.That(pending.CanEdit).IsFalse();
        await Assert.That(invalid.HasValue).IsFalse();
        await Assert.That(invalid.IsInvalid).IsTrue();
        await Assert.That(group.Name).IsEqualTo("General");
        await Assert.That(group.HasValidationErrors).IsTrue();
        await Assert.That(group.HasModifiedDescriptors).IsTrue();
        await Assert.That(empty.SummaryText).IsEqualTo("No properties");
        await Assert.That(modified.SummaryText).IsEqualTo("1 properties, 1 modified");
        await Assert.That(modified.CanCommit).IsTrue();
        await Assert.That(modified.CanReset).IsTrue();
        await Assert.That(errors.SummaryText).IsEqualTo("2 properties, 1 invalid");
        await Assert.That(errors.VisibleDescriptorCount).IsEqualTo(PriorityThreshold);
        await Assert.That(errors.GetDescriptor(MissingKey)).IsNull();
        await Assert.That(committing.CanCommit).IsFalse();
        await Assert.That(committing.CanReset).IsFalse();
    }

    /// <summary>Verifies step selection fallback and navigation availability at each boundary.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Stepper_ResolvesRequestedActiveAndFallbackSteps()
    {
        var first = new StepDescriptor("first", "First", new StepDescriptorOptions { Status = StepStatus.Active });
        var last = new StepDescriptor("last", "Last", new StepDescriptorOptions { Status = StepStatus.Completed });
        var active = new StepperState([first, last], " missing ", StepperOrientation.Horizontal);
        var final = new StepperState([first, last], "last", StepperOrientation.Vertical);
        var empty = new StepperState([]);

        await Assert.That(active.CurrentKey).IsEqualTo("first");
        await Assert.That(active.CanGoPrevious).IsFalse();
        await Assert.That(active.CanGoNext).IsTrue();
        await Assert.That(active.ProgressText).IsEqualTo("Step 1 of 2");
        await Assert.That(final.CanGoPrevious).IsTrue();
        await Assert.That(final.CanGoNext).IsFalse();
        await Assert.That(final.CanFinish).IsTrue();
        await Assert.That(final.GetStep(MissingKey)).IsNull();
        await Assert.That(empty.HasSteps).IsFalse();
        await Assert.That(empty.CurrentIndex).IsEqualTo(-1);
        await Assert.That(empty.ProgressText).IsEqualTo("No steps");
    }

    /// <summary>Verifies concrete, system, and unsupported high-contrast preferences.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ThemePreference_ResolvesEverySupportedAndFallbackChoice()
    {
        var system = new ThemePreferenceState(ThemeChoice.System, ThemeChoice.Dark, true);
        var highContrast = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Light, true);
        var fallback = new ThemePreferenceState(ThemeChoice.HighContrast, ThemeChoice.Dark, false);
        var normalized = new ThemePreferenceState(ThemeChoice.Dark, ThemeChoice.System, false);

        await Assert.That(system.EffectiveChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(system.DisplayText).IsEqualTo("System (Dark)");
        await Assert.That(highContrast.IsHighContrastEffective).IsTrue();
        await Assert.That(highContrast.DisplayText).IsEqualTo("High contrast");
        await Assert.That(fallback.EffectiveChoice).IsEqualTo(ThemeChoice.Dark);
        await Assert.That(fallback.DisplayText).IsEqualTo("High contrast (using Dark)");
        await Assert.That(fallback.SupportsChoice(ThemeChoice.HighContrast)).IsFalse();
        await Assert.That(fallback.AvailableChoices).Count().IsEqualTo(StandardThemeChoiceCount);
        await Assert.That(normalized.SystemChoice).IsEqualTo(ThemeChoice.Light);
    }

    /// <summary>Verifies incomplete, maximum-duration, exclusive, and preset date range behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateRanges_ProjectIncompleteInvalidAndExclusiveBoundaries()
    {
        var start = new DateTimeOffset(2026, 7, 26, 9, 0, 0, TimeSpan.Zero);
        var incomplete = new DateTimeRange(start, null);
        var tooLong = new DateTimeRange(
            start,
            start.AddHours(MaximumRangeHours + 1),
            DateTimeRangePreset.Today,
            null,
            isEndInclusive: false,
            maximumDuration: TimeSpan.FromHours(MaximumRangeHours));
        var exclusive = new DateTimeRange(start, start.AddHours(MaximumRangeHours), DateTimeRangePreset.ThisMonth, null, isEndInclusive: false);
        var defaultLabel = new DateTimeRange(start, start, DateTimeRangePreset.LastSevenDays);

        await Assert.That(incomplete.IsValid).IsFalse();
        await Assert.That(incomplete.ValidationMessage).IsEqualTo("Start and end are required.");
        await Assert.That(incomplete.DisplayText).IsEqualTo("Custom: invalid range");
        await Assert.That(tooLong.ExceedsMaximumDuration).IsTrue();
        await Assert.That(tooLong.ValidationMessage).IsEqualTo("Range exceeds the maximum allowed duration.");
        await Assert.That(tooLong.Contains(start)).IsFalse();
        await Assert.That(exclusive.IsValid).IsTrue();
        await Assert.That(exclusive.Contains(start)).IsTrue();
        await Assert.That(exclusive.Contains(exclusive.End!.Value)).IsFalse();
        await Assert.That(defaultLabel.Label).IsEqualTo("Last 7 days");
    }

    /// <summary>Verifies empty and plural validation summaries retain their lookup and first-error semantics.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ValidationSummaries_ProjectEmptyPluralAndFieldLookupStates()
    {
        var empty = new ValidationSummaryState(null);
        var error = new ValidationMessage(Email, Email, "Required");
        var summary = new ValidationSummaryState(
            [error, new ValidationMessage("email", Email, "Duplicate"),
                new ValidationMessage("warning", "Warning", "Review", ValidationSeverity.Warning),
                new ValidationMessage("pending", "Pending", "Checking", ValidationSeverity.Pending)]);

        await Assert.That(empty.IsValid).IsTrue();
        await Assert.That(empty.SummaryText).IsEqualTo("No validation messages");
        await Assert.That(empty.FirstError).IsNull();
        await Assert.That(empty.GetMessagesForField(string.Empty)).Count().IsEqualTo(0);
        await Assert.That(summary.SummaryText).IsEqualTo("2 errors, 1 warning, 1 pending");
        await Assert.That(summary.FirstError).IsEqualTo(error);
        await Assert.That(summary.GetMessagesForField(" EMAIL ")).Count().IsEqualTo(PriorityThreshold);
    }

    /// <summary>Verifies navigation data objects preserve normalized contracts and diagnostic exception metadata.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationValueObjects_NormalizeContractsAndPreserveExceptionContext()
    {
        using var cancellation = new CancellationTokenSource();
        var request = new NavigationRequest(
            NavigationSourceKind.View,
            "instance",
            typeof(StateModelEdgeCaseTests),
            PaddedDetailContract,
            NavigationParameter,
            NavigationType.Refresh,
            cancellation.Token);
        var registrationDefault = new NavigationRegistrationException();
        var registrationMessage = new NavigationRegistrationException("duplicate", new InvalidOperationException("inner"));
        var registration = new NavigationRegistrationException(NavigationSourceKind.View, typeof(string), PaddedDetailContract);
        var resolutionDefault = new NavigationResolutionException();
        var resolutionMessage = new NavigationResolutionException("missing", new InvalidOperationException("inner"));
        var resolution = new NavigationResolutionException(
            NavigationSourceKind.ViewModel,
            typeof(int),
            PaddedDetailContract,
            [null, PaddedDetailContract, DetailContract, "other"]);

        await Assert.That(request.SourceKind).IsEqualTo(NavigationSourceKind.View);
        await Assert.That(request.Contract).IsEqualTo(PaddedDetailContract);
        await Assert.That(request.Parameter).IsEqualTo(NavigationParameter);
        await Assert.That(request.CancellationToken).IsEqualTo(cancellation.Token);
        await Assert.That(registrationDefault.ServiceType).IsEqualTo(typeof(object));
        await Assert.That(registrationMessage.Message).IsEqualTo("duplicate");
        await Assert.That(registration.ServiceType).IsEqualTo(typeof(string));
        await Assert.That(registration.Contract).IsEqualTo(PaddedDetailContract);
        await Assert.That(resolutionDefault.KnownContracts).Count().IsEqualTo(0);
        await Assert.That(resolutionMessage.Message).IsEqualTo("missing");
        await Assert.That(resolution.SourceKey).IsEqualTo(typeof(int));
        await Assert.That(resolution.Contract).IsEqualTo(PaddedDetailContract);
        await Assert.That(resolution.KnownContracts).IsEquivalentTo([null, PaddedDetailContract, DetailContract, "other"]);
    }

    /// <summary>Provides a command with the simple always-enabled behavior used by immutable state tests.</summary>
    private sealed class TestCommand : System.Windows.Input.ICommand
    {
        /// <summary>Occurs when command execution availability changes.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Determines whether the command may execute.</summary>
        /// <param name="parameter">The optional command parameter.</param>
        /// <returns><c>true</c>.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Executes the command and publishes its reactive event boundary.</summary>
        /// <param name="parameter">The optional command parameter.</param>
        public void Execute(object? parameter) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
