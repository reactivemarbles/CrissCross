// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Exercises MAUI UI control branches not covered by state-projection tests.</summary>
public sealed class MauiUiCoverageEdgeBehaviorTests
{
    /// <summary>Verifies pager navigation commands execute when their matching destinations are available.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataPager_NavigationCommands_ExecuteAvailablePageTransitions()
    {
        const int expectedExecutionCount = 4;
        const int expectedPageIndex = 2;
        const int pageSize = 10;
        const int totalItemCount = 30;
        var command = new TrackingCommand(canExecute: true);
        var pagination = new PaginationState(pageIndex: 1, pageSize, totalItemCount);
        var pager = new DataPager { PaginationState = pagination, PageRequestCommand = command };
        var canExecuteChangedCount = 0;
        pager.NextPageCommand.CanExecuteChanged += (_, _) => canExecuteChangedCount++;

        pager.FirstPageCommand.Execute(null);
        pager.PreviousPageCommand.Execute(null);
        pager.NextPageCommand.Execute(null);
        pager.LastPageCommand.Execute(null);

        await Assert.That(command.ExecutionCount).IsEqualTo(expectedExecutionCount);
        await Assert.That(canExecuteChangedCount).IsEqualTo(1);
        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(expectedPageIndex);
        await Assert.That(pager.CurrentRequest?.PageSize).IsEqualTo(pageSize);
    }

    /// <summary>Verifies pager and segmented controls handle unavailable command and state paths safely.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PagerAndSegmentedControl_UnavailablePaths_RetainSafeState()
    {
        var deniedCommand = new TrackingCommand(canExecute: false);
        var pager = new DataPager { PageRequestCommand = deniedCommand };
        var segment = new SegmentItem("available", "Available");
        var segmented = new SegmentedControl { State = new([segment], segment.Key) };

        pager.MoveToPage(1);
        pager.FirstPageCommand.Execute(null);
        var selected = segmented.SelectSegment(segment.Key);
        segmented.State = null;
        var invalidSelection = segmented.SelectSegment("missing");

        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(1);
        await Assert.That(deniedCommand.ExecutionCount).IsEqualTo(0);
        await Assert.That(selected).IsTrue();
        await Assert.That(segmented.Children).IsEmpty();
        await Assert.That(invalidSelection).IsFalse();
    }

    /// <summary>Verifies chip-group fallback commands and null-state rendering paths.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ChipGroup_FallbackCommandsAndNullState_ExecuteAndClearChildren()
    {
        var selectCommand = new TrackingCommand(canExecute: true);
        var removeCommand = new TrackingCommand(canExecute: true);
        var chip = new ChipModel("review", "Review", new ChipModelOptions { IsRemovable = true });
        var state = new ChipGroupState([chip], ChipGroupSelectionMode.Single);
        var group = new ChipGroup { SelectionCommand = selectCommand, RemoveCommand = removeCommand, State = state };

        var selected = group.SelectChip(chip.Key);
        var removed = group.RemoveChip(chip.Key);
        group.State = null;

        await Assert.That(selected).IsTrue();
        await Assert.That(removed).IsTrue();
        await Assert.That(selectCommand.LastParameter).IsEqualTo(chip.Key);
        await Assert.That(removeCommand.LastParameter).IsEqualTo(chip.Key);
        await Assert.That(group.Children).IsEmpty();
        await Assert.That(group.SelectionMode).IsEqualTo(ChipGroupSelectionMode.None);
    }

    /// <summary>Verifies rating clamping and inactive visual-state projections.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RatingAndVisualControls_ClampAndProjectInactiveBranches()
    {
        const int maxRating = 3;
        const int oversizedRating = 99;
        var rating = new RatingControl { MaxRating = maxRating, Value = oversizedRating };
        var busyOverlay = new BusyOverlay { Operation = new(string.Empty) };
        var imageSource = new FileImageSource { File = "ada.png" };
        var picture = new PersonPicture { DisplayName = "Ada Lovelace", Initials = "AL", Source = imageSource };
        var chip = new Chip { Model = new("id", "Chip") };

        chip.Model = null;

        await Assert.That(rating.Value).IsEqualTo(maxRating);
        await Assert.That(rating.Children.Count).IsEqualTo(maxRating);
        await Assert.That(busyOverlay.IsBusy).IsFalse();
        await Assert.That(SemanticProperties.GetDescription(picture)).IsEqualTo("Ada Lovelace");
        await Assert.That(chip.Text).IsEqualTo("Chip");
    }

    /// <summary>Tracks invocations through an <see cref="ICommand"/> test double.</summary>
    /// <param name="canExecute">Whether the command allows execution.</param>
    private sealed class TrackingCommand(bool canExecute) : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets the last command parameter.</summary>
        public object? LastParameter { get; private set; }

        /// <summary>Gets the number of command executions.</summary>
        public int ExecutionCount { get; private set; }

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => canExecute;

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            ExecutionCount++;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
