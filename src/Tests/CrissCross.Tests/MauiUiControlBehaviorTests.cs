// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Tests for MAUI UI controls that project shared platform-neutral CrissCross control state.</summary>
public class MauiUiControlBehaviorTests
{
    /// <summary>Provides the closed segment key.</summary>
    private const string ClosedSegmentKey = "closed";

    /// <summary>Provides the urgent chip key.</summary>
    private const string UrgentChipKey = "urgent";

    /// <summary>Provides the busy operation progress value.</summary>
    private const double BusyOperationProgress = 0.25;

    /// <summary>Provides the requested page index that should be clamped.</summary>
    private const int RequestedPageIndex = 99;

    /// <summary>Provides the expected clamped page index.</summary>
    private const int ExpectedPageIndex = 2;

    /// <summary>Provides the test page size.</summary>
    private const int PageSize = 25;

    /// <summary>Provides the test total item count.</summary>
    private const int TotalItemCount = 60;

    /// <summary>Provides the expected rendered item count.</summary>
    private const int ExpectedRenderedItemCount = 2;

    /// <summary>Provides the CommandButton_SettingIsExecuting_TransitionsToExecutingState member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CommandButton_SettingIsExecuting_TransitionsToExecutingState()
    {
        var button = new CommandButton { IsExecuting = true };

        await Assert.That(button.State).IsEqualTo(CommandButtonState.Executing);
    }

    /// <summary>Provides the BusyOverlay_ActiveOperation_ProjectsBusyState member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task BusyOverlay_ActiveOperation_ProjectsBusyState()
    {
        var overlay = new BusyOverlay { Operation = new("Saving", "Writing values", progress: BusyOperationProgress) };

        await Assert.That(overlay.IsBusy).IsTrue();
    }

    /// <summary>Provides the DataPager_MoveToPage_ClampsRequestAndInvokesCommand member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataPager_MoveToPage_ClampsRequestAndInvokesCommand()
    {
        var command = new CaptureCommand();
        var pager = new DataPager
        {
            PaginationState = new(pageIndex: 1, pageSize: PageSize, totalItemCount: TotalItemCount),
            PageRequestCommand = command,
            SortKey = "name",
            SortDescending = true,
        };

        pager.MoveToPage(RequestedPageIndex);

        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(ExpectedPageIndex);
        await Assert.That(pager.CurrentRequest?.PageSize).IsEqualTo(PageSize);
        await Assert.That(pager.CurrentRequest?.SortKey).IsEqualTo("name");
        await Assert.That(pager.CurrentRequest?.SortDescending).IsTrue();
        await Assert.That(command.LastParameter).IsEqualTo(pager.CurrentRequest);
    }

    /// <summary>Provides the SegmentedControl_State_RendersSegmentsAndInvokesSelectionCommand member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SegmentedControl_State_RendersSegmentsAndInvokesSelectionCommand()
    {
        var command = new CaptureCommand();
        var target = new SegmentedControl
        {
            SelectionCommand = command,
            State = new([new SegmentItem("open", "Open"), new SegmentItem(ClosedSegmentKey, "Closed")], "open"),
        };

        _ = target.SelectSegment(ClosedSegmentKey);

        await Assert.That(target.Children.Count).IsEqualTo(ExpectedRenderedItemCount);
        await Assert.That(target.SelectedKey).IsEqualTo(ClosedSegmentKey);
        await Assert
            .That(target.Children.OfType<Button>().Select(static button => button.Text))
            .IsEquivalentTo(["Open", "Closed"]);
        await Assert.That(command.LastParameter).IsEqualTo(ClosedSegmentKey);
    }

    /// <summary>Provides the ChipGroup_State_RendersChipsAndInvokesSelectionCommand member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ChipGroup_State_RendersChipsAndInvokesSelectionCommand()
    {
        var command = new CaptureCommand();
        var target = new ChipGroup
        {
            SelectionCommand = command,
            State = new(
                [
                    new ChipModel(UrgentChipKey, "Urgent"),
                    new ChipModel("review", "Needs review", new ChipModelOptions { IsSelected = true }),],
                ChipGroupSelectionMode.Multiple),
        };

        _ = target.SelectChip(UrgentChipKey);

        await Assert.That(target.Children.Count).IsEqualTo(ExpectedRenderedItemCount);
        await Assert.That(target.SelectionMode).IsEqualTo(ChipGroupSelectionMode.Multiple);
        await Assert
            .That(target.Children.OfType<Button>().Select(static button => button.Text))
            .IsEquivalentTo(["Urgent", "Needs review"]);
        await Assert.That(command.LastParameter).IsEqualTo(UrgentChipKey);
    }

    /// <summary>Provides the SearchBox_SubmitSearch_InvokesSubmitCommandWithNormalizedText member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SearchBox_SubmitSearch_InvokesSubmitCommandWithNormalizedText()
    {
        var command = new CaptureCommand();
        var target = new SearchBox { Text = "  pump  ", SubmitCommand = command };

        _ = target.SubmitSearch();

        await Assert.That(command.LastParameter).IsEqualTo("pump");
    }

    /// <summary>Provides the CaptureCommand member.</summary>
    private sealed class CaptureCommand : ICommand
    {
        /// <summary>Gets the value.</summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets the value.</summary>
        public object? LastParameter { get; private set; }

        /// <summary>Provides the CanExecute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public bool CanExecute(object? parameter) => true;

        /// <summary>Provides the Execute member.</summary>
        /// <param name="parameter">The parameter value.</param>
        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
