// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>
/// Tests for MAUI UI controls that project shared platform-neutral CrissCross control state.
/// </summary>
public class MauiUiControlBehaviorTests
{
    [Test]
    public async Task CommandButton_SettingIsExecuting_TransitionsToExecutingState()
    {
        var button = new CommandButton
        {
            IsExecuting = true
        };

        await Assert.That(button.State).IsEqualTo(CommandButtonState.Executing);
    }

    [Test]
    public async Task BusyOverlay_ActiveOperation_ProjectsBusyState()
    {
        var overlay = new BusyOverlay
        {
            Operation = new BusyOperation("Saving", "Writing values", progress: 0.25)
        };

        await Assert.That(overlay.IsBusy).IsTrue();
    }

    [Test]
    public async Task DataPager_MoveToPage_ClampsRequestAndInvokesCommand()
    {
        var command = new CaptureCommand();
        var pager = new DataPager
        {
            PaginationState = new PaginationState(pageIndex: 1, pageSize: 25, totalItemCount: 60),
            PageRequestCommand = command,
            SortKey = "name",
            SortDescending = true
        };

        pager.MoveToPage(99);

        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(2);
        await Assert.That(pager.CurrentRequest?.PageSize).IsEqualTo(25);
        await Assert.That(pager.CurrentRequest?.SortKey).IsEqualTo("name");
        await Assert.That(pager.CurrentRequest?.SortDescending).IsTrue();
        await Assert.That(command.LastParameter).IsEqualTo(pager.CurrentRequest);
    }

    [Test]
    public async Task SegmentedControl_State_RendersSegmentsAndInvokesSelectionCommand()
    {
        var command = new CaptureCommand();
        var target = new SegmentedControl
        {
            SelectionCommand = command,
            State = new SegmentedSelectionState(
                [
                    new SegmentItem("open", "Open"),
                    new SegmentItem("closed", "Closed")
                ],
                "open")
        };

        target.SelectSegment("closed");

        await Assert.That(target.Children.Count).IsEqualTo(2);
        await Assert.That(target.SelectedKey).IsEqualTo("closed");
        await Assert.That(target.Children.OfType<Button>().Select(static button => button.Text)).IsEquivalentTo(["Open", "Closed"]);
        await Assert.That(command.LastParameter).IsEqualTo("closed");
    }

    [Test]
    public async Task ChipGroup_State_RendersChipsAndInvokesSelectionCommand()
    {
        var command = new CaptureCommand();
        var target = new ChipGroup
        {
            SelectionCommand = command,
            State = new ChipGroupState(
                [
                    new ChipModel("urgent", "Urgent"),
                    new ChipModel("review", "Needs review", isSelected: true)
                ],
                ChipGroupSelectionMode.Multiple)
        };

        target.SelectChip("urgent");

        await Assert.That(target.Children.Count).IsEqualTo(2);
        await Assert.That(target.SelectionMode).IsEqualTo(ChipGroupSelectionMode.Multiple);
        await Assert.That(target.Children.OfType<Button>().Select(static button => button.Text)).IsEquivalentTo(["Urgent", "Needs review"]);
        await Assert.That(command.LastParameter).IsEqualTo("urgent");
    }

    [Test]
    public async Task SearchBox_SubmitSearch_InvokesSubmitCommandWithNormalizedText()
    {
        var command = new CaptureCommand();
        var target = new SearchBox
        {
            Text = "  pump  ",
            SubmitCommand = command
        };

        target.SubmitSearch();

        await Assert.That(command.LastParameter).IsEqualTo("pump");
    }

    private sealed class CaptureCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public object? LastParameter { get; private set; }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
