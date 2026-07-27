// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Exercises public edge paths that complete the platform-neutral coverage contract.</summary>
public class CoverageCompletionTests
{
    /// <summary>Provides a non-default page index.</summary>
    private const int RequestedPageIndex = 2;

    /// <summary>Provides a non-default page size.</summary>
    private const int RequestedPageSize = 25;

    /// <summary>Provides the shared status filter key.</summary>
    private const string StatusFilterKey = "status";

    /// <summary>Provides the shared closed-state value.</summary>
    private const string ClosedState = "Closed";

    /// <summary>Provides the expected active-filter count.</summary>
    private const int ExpectedActiveFilterCount = 2;

    /// <summary>Provides the sample query result count.</summary>
    private const int SampleQueryResultCount = 4;

    /// <summary>Exercises every command-status factory and error guard.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CommandButtonStatus_Factories_ProjectEveryStateAndGuardNullErrors()
    {
        var idle = CommandButtonStatus.Idle(false);
        var succeeded = CommandButtonStatus.Succeeded(true);
        var cancelled = CommandButtonStatus.Cancelled(true);
        var failedWithoutError = new CommandButtonStatus(CommandButtonState.Failed, true, false);

        await Assert.That(idle.State).IsEqualTo(CommandButtonState.Idle);
        await Assert.That(idle.IsInteractive).IsFalse();
        await Assert.That(succeeded.State).IsEqualTo(CommandButtonState.Succeeded);
        await Assert.That(succeeded.IsInteractive).IsTrue();
        await Assert.That(cancelled.State).IsEqualTo(CommandButtonState.Cancelled);
        await Assert.That(failedWithoutError.HasError).IsFalse();
        await Assert
            .That(static () => CommandButtonStatus.Failed(null!, true))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Exercises empty, invalid and reset navigation-journal paths.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationJournal_InvalidAndResetPaths_AreDeterministic()
    {
        var journal = new List<string> { "home", "details", };
        var currentIndex = -1;

        var canMoveForward = NavigationJournal.TryMoveForward(
            journal,
            currentIndex,
            out var unchangedIndex,
            out var entryId);
        NavigationJournal.Record(journal, ref currentIndex, "settings");
        NavigationJournal.Clear(journal, ref currentIndex);

        await Assert.That(canMoveForward).IsFalse();
        await Assert.That(unchangedIndex).IsEqualTo(-1);
        await Assert.That(entryId).IsNull();
        await Assert.That(journal).IsEmpty();
        await Assert.That(currentIndex).IsEqualTo(-1);
        await Assert
            .That(static () => NavigationJournal.CanGoBack(null!, 0))
            .Throws<ArgumentNullException>();
        await Assert
            .That(static () => NavigationJournal.CanGoForward(null!, 0))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Exercises all page-request overloads, clamping and empty-query projections.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PageRequest_OverloadsAndClamping_ProjectStableDefaults()
    {
        var basic = new PageRequest(RequestedPageIndex, RequestedPageSize);
        var sorted = new PageRequest(RequestedPageIndex, RequestedPageSize, "name");
        var descending = new PageRequest(RequestedPageIndex, RequestedPageSize, "name", true);
        var clamped = new PageRequest(-1, 0, " ", false, null);

        await Assert.That(basic.Offset).IsEqualTo(RequestedPageIndex * RequestedPageSize);
        await Assert.That(basic.HasSort).IsFalse();
        await Assert.That(basic.HasQuery).IsFalse();
        await Assert.That(basic.DisplayText).IsEqualTo("Page 3, 25 per page");
        await Assert.That(sorted.HasSort).IsTrue();
        await Assert.That(descending.SortDescending).IsTrue();
        await Assert.That(clamped.PageIndex).IsEqualTo(0);
        await Assert.That(clamped.PageSize).IsEqualTo(1);
        await Assert.That(clamped.FilterSnapshotKey).IsEmpty();
    }

    /// <summary>Exercises descriptor overloads, operator labels and value formatting.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterDescriptor_OverloadsAndFormatting_CoverEveryOperatorLabel()
    {
        var explicitOperators = new[] { FilterOperator.NotEquals, };
        var explicitChoices = new object?[] { "Open", };
        var descriptor = new FilterDescriptor(
            StatusFilterKey,
            "Status",
            FilterEditorKind.Enum,
            explicitOperators,
            explicitChoices,
            "Open");
        var required = new FilterDescriptor(
            "created",
            "Created",
            FilterEditorKind.DateTime,
            null,
            null,
            null,
            true);
        var operators = Enum.GetValues<FilterOperator>();
        var displayTexts = new List<string>(operators.Length);

        foreach (var @operator in operators)
        {
            displayTexts.Add(descriptor.CreateDisplayText(@operator, "value"));
        }

        var dateText = required.CreateDisplayText(
            FilterOperator.Equals,
            new DateTime(2026, 7, 26, 12, 30, 0, DateTimeKind.Utc));
        var offsetText = required.CreateDisplayText(
            FilterOperator.Equals,
            new DateTimeOffset(2026, 7, 26, 12, 30, 0, TimeSpan.Zero));
        var nullText = required.CreateDisplayText(FilterOperator.Equals, null);
        var unknownText = descriptor.CreateDisplayText((FilterOperator)int.MaxValue, "value");

        await Assert.That(descriptor.HasChoices).IsTrue();
        await Assert.That(descriptor.DefaultValue).IsEqualTo("Open");
        await Assert.That(descriptor.SupportsOperator(FilterOperator.NotEquals)).IsTrue();
        await Assert.That(descriptor.SupportsOperator(FilterOperator.Equals)).IsFalse();
        await Assert.That(descriptor.CreateToken(ClosedState).IsRemovable).IsTrue();
        await Assert.That(descriptor.CreateToken(ClosedState, FilterOperator.NotEquals).Operator)
            .IsEqualTo(FilterOperator.NotEquals);
        await Assert.That(displayTexts.Count).IsEqualTo(operators.Length);
        await Assert.That(dateText).Contains("2026-07-26 12:30");
        await Assert.That(offsetText).Contains("2026-07-26 12:30");
        await Assert.That(nullText).EndsWith("equals ");
        await Assert.That(unknownText).Contains(int.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>Exercises constructor shims and lookup/query projections for filter-panel state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataFilterPanelState_OverloadsAndLookups_ProjectEmptyAndPluralStates()
    {
        var descriptor = new FilterDescriptor(StatusFilterKey, "Status", FilterEditorKind.Text);
        var expression = new FilterExpression(StatusFilterKey, FilterOperator.Equals, "Open");
        var secondExpression = new FilterExpression(StatusFilterKey, FilterOperator.NotEquals, ClosedState);
        var empty = new DataFilterPanelState();
        var descriptorsOnly = new DataFilterPanelState([descriptor]);
        var withExpressions = new DataFilterPanelState([descriptor], [expression, secondExpression]);
        var dirty = new DataFilterPanelState([descriptor], [expression], true);

        var noTextQuery = withExpressions.ToSearchQueryState();
        var textQuery = withExpressions.ToSearchQueryState("orders");
        var countedQuery = withExpressions.ToSearchQueryState("orders", SampleQueryResultCount);

        await Assert.That(empty.SummaryText).IsEqualTo("No filters");
        await Assert.That(descriptorsOnly.DescriptorCount).IsEqualTo(1);
        await Assert.That(withExpressions.SummaryText).IsEqualTo("2 active filters");
        await Assert.That(withExpressions.GetDescriptor("missing")).IsNull();
        await Assert.That(dirty.CanApply).IsTrue();
        await Assert.That(noTextQuery.ActiveFilterCount).IsEqualTo(ExpectedActiveFilterCount);
        await Assert.That(textQuery.HasQuery).IsTrue();
        await Assert.That(countedQuery.ResultCount).IsEqualTo(SampleQueryResultCount);
    }
}
