// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;
using CrissCrossStepper = CrissCross.Maui.UI.Controls.Stepper;

namespace CrissCross.Tests;

/// <summary>Tests for MAUI UI controls that project shared platform-neutral CrissCross control state.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
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

    /// <summary>Provides the maximum rating used by the rating-control test.</summary>
    private const int MaximumRating = 5;

    /// <summary>Provides a rating that must be clamped by the rating control.</summary>
    private const int RequestedRating = 99;

    /// <summary>Provides a negative page request used to validate default pager clamping.</summary>
    private const int NegativeRequestedPageIndex = -1;

    /// <summary>Provides the pager's documented fallback page size.</summary>
    private const int DefaultPagerPageSize = 20;

    /// <summary>Provides the key for the active test chip.</summary>
    private const string ActiveChipKey = "active";

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

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
        var pager = new DataPager { PaginationState = new(pageIndex: 1, pageSize: PageSize, totalItemCount: TotalItemCount), PageRequestCommand = command, SortKey = "name", SortDescending = true, };

        pager.MoveToPage(RequestedPageIndex);

        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(ExpectedPageIndex);
        await Assert.That(pager.CurrentRequest?.PageSize).IsEqualTo(PageSize);
        await Assert.That(pager.CurrentRequest?.SortKey).IsEqualTo("name");
        await Assert.That(pager.CurrentRequest?.SortDescending).IsTrue();
        await Assert.That(command.LastParameter).IsEqualTo(pager.CurrentRequest);
    }

    /// <summary>Verifies a pager without state uses its default request and disables navigation commands.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataPager_WithoutState_ClampsRequestsAndDisablesNavigationCommands()
    {
        var pager = new DataPager();

        pager.MoveToPage(NegativeRequestedPageIndex);

        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(0);
        await Assert.That(pager.CurrentRequest?.PageSize).IsEqualTo(DefaultPagerPageSize);
        await Assert.That(pager.FirstPageCommand.CanExecute(null)).IsFalse();
        await Assert.That(pager.PreviousPageCommand.CanExecute(null)).IsFalse();
        await Assert.That(pager.NextPageCommand.CanExecute(null)).IsFalse();
        await Assert.That(pager.LastPageCommand.CanExecute(null)).IsFalse();
    }

    /// <summary>Provides the SegmentedControl_State_RendersSegmentsAndInvokesSelectionCommand member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SegmentedControl_State_RendersSegmentsAndInvokesSelectionCommand()
    {
        var command = new CaptureCommand();
        var target = new SegmentedControl { SelectionCommand = command, State = new([new SegmentItem("open", "Open"), new SegmentItem(ClosedSegmentKey, "Closed")], "open"), };

        _ = target.SelectSegment(ClosedSegmentKey);

        await Assert.That(target.Children.Count).IsEqualTo(ExpectedRenderedItemCount);
        await Assert.That(target.SelectedKey).IsEqualTo(ClosedSegmentKey);
        await Assert.That(GetButtonTexts(target)).IsEquivalentTo(["Open", "Closed"]);
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
        await Assert.That(GetButtonTexts(target)).IsEquivalentTo(["Urgent", "Needs review"]);
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

    /// <summary>Verifies a search box projects its state text and does not submit without an executable command.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SearchBox_StateProjectionAndUnavailableCommand_AreSafe()
    {
        var state = new SearchQueryState(" state text ");
        var target = new SearchBox { SearchState = state, };

        var submitted = target.SubmitSearch();

        await Assert.That(target.Text).IsEqualTo(" state text ");
        await Assert.That(submitted).IsFalse();
    }

    /// <summary>Verifies default async-button presentation values are available without a command.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AsyncCommandButton_DefaultCancelPresentation_IsAvailableWithoutACommand()
    {
        var button = new AsyncCommandButton();

        await Assert.That(button.CancelText).IsEqualTo("Cancel");
        await Assert.That(button.CancelCommand).IsNull();
        await Assert.That(button.State).IsEqualTo(CommandButtonState.Idle);
    }

    /// <summary>Verifies default transient controls expose and execute their close behaviors.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task TransientControls_DefaultCommandsHideTheirVisibleState()
    {
        var alarm = new AlarmBanner { IsActive = true, Message = "Disk space is low", };
        var snackbar = new Snackbar { Title = "Saved", Message = "Changes were stored", };

        snackbar.Show();
        alarm.AcknowledgeCommand!.Execute(null);
        snackbar.CloseCommand!.Execute(null);

        await Assert.That(alarm.IsActive).IsFalse();
        await Assert.That(snackbar.IsShown).IsFalse();
        await Assert.That(SemanticProperties.GetDescription(alarm)).Contains(nameof(InfoBarSeverity.Error));
        await Assert.That(SemanticProperties.GetDescription(snackbar)).Contains("Saved");
    }

    /// <summary>Verifies that a person picture derives a stable initials fallback without an image.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PersonPicture_DisplayName_ProjectsAccessibleInitialsFallback()
    {
        var picture = new PersonPicture { DisplayName = "Ada Lovelace" };

        await Assert.That(SemanticProperties.GetDescription(picture)).IsEqualTo("Ada Lovelace");
        await Assert.That(picture.Content).IsNotNull();
    }

    /// <summary>Verifies that card controls retain the native MAUI content and command composition model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CardControls_ExposeComposableNativeSurfaces()
    {
        var content = new Label { Text = "Details" };
        var card = new Card { Content = content };
        var action = new CardAction { IsChevronVisible = false, Text = "Open" };
        var expander = new CardExpander { Header = new Label { Text = "More" }, ExpandedContent = content, IsExpanded = true };

        await Assert.That(card.Content).IsEqualTo(content);
        await Assert.That(action.IsChevronVisible).IsFalse();
        await Assert.That(expander.IsExpanded).IsTrue();
        await Assert.That(expander.Header).IsNotNull();
        await Assert.That(expander.ExpandedContent).IsEqualTo(content);
    }

    /// <summary>Verifies that an information badge includes its semantic severity in accessible output.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task InfoBadge_Severity_UpdatesAccessibleDescription()
    {
        var badge = new InfoBadge { Text = "Disconnected", Severity = InfoBadgeSeverity.Error };

        await Assert.That(SemanticProperties.GetDescription(badge)).Contains(nameof(InfoBadgeSeverity.Error));
    }

    /// <summary>Verifies that an info bar makes a supplied reactive action available to its native action button.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task InfoBar_Action_ProjectsCommandAndText()
    {
        var command = new CaptureCommand();
        var infoBar = new InfoBar { ActionCommand = command, ActionText = "Retry", Message = "Connection interrupted", Severity = InfoBarSeverity.Warning, Title = "Network", };

        await Assert.That(infoBar.Content).IsNotNull();
        await Assert.That(infoBar.Severity).IsEqualTo(InfoBarSeverity.Warning);
        await Assert.That(SemanticProperties.GetDescription(infoBar)).Contains(nameof(InfoBarSeverity.Warning));
    }

    /// <summary>Verifies that a rating control clamps input and forwards the selected value through its reactive command boundary.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RatingControl_SetRating_ClampsAndInvokesCommand()
    {
        var command = new CaptureCommand();
        var rating = new RatingControl { MaxRating = MaximumRating, ValueChangedCommand = command };

        rating.SetRating(RequestedRating);

        await Assert.That(rating.Value).IsEqualTo(MaximumRating);
        await Assert.That(rating.Children.Count).IsEqualTo(MaximumRating);
        await Assert.That(command.LastParameter).IsEqualTo(MaximumRating);
    }

    /// <summary>Verifies that the inherited display-only state prevents rating command execution.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RatingControl_ReadOnly_DoesNotChangeSelection()
    {
        var command = new CaptureCommand();
        var rating = new RatingControl { IsReadOnly = true, Value = MaximumRating, ValueChangedCommand = command };

        rating.SetRating(0);

        await Assert.That(rating.Value).IsEqualTo(MaximumRating);
        await Assert.That(command.LastParameter).IsNull();
    }

    /// <summary>Verifies each state-only MAUI control retains its projected state and command boundary.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StateControls_ProjectSharedModelsAndCommands()
    {
        var command = new CaptureCommand();
        var filterState = new DataFilterPanelState();
        var range = new DateTimeRange(null, null);
        var emptyModel = new EmptyStateModel("Nothing here");
        var search = new SearchQueryState("filter");
        var propertyState = new PropertyGridState();
        var stepperState = new StepperState([]);
        var themeState = new ThemePreferenceState(ThemeChoice.Dark);
        var validationState = new ValidationSummaryState([]);

        var filterPanel = new DataFilterPanel { FilterPanelState = filterState, ApplyFiltersCommand = command };
        var picker = new DateTimeRangePicker { Range = range, ApplyRangeCommand = command };
        var empty = new EmptyState { Model = emptyModel, PrimaryCommand = command };
        var filterBar = new FilterBar { SearchState = search, ClearFiltersCommand = command };
        var grid = new PropertyGridLite { PropertyGridState = propertyState, UpdatePropertyCommand = command };
        var field = new ReactiveFormField { FieldState = FormFieldState.Warning };
        var stepper = new CrissCrossStepper { StepperState = stepperState, StepCommand = command };
        var theme = new ThemeSwitcher { ThemeState = themeState, ChangeThemeCommand = command };
        var validation = new ValidationSummary { SummaryState = validationState };
        var asyncButton = new AsyncCommandButton { CancelCommand = command, CancelText = "Stop" };

        await Assert.That(filterPanel.FilterPanelState).IsEqualTo(filterState);
        await Assert.That(picker.Range).IsEqualTo(range);
        await Assert.That(empty.Model).IsEqualTo(emptyModel);
        await Assert.That(filterBar.SearchState).IsEqualTo(search);
        await Assert.That(grid.PropertyGridState).IsEqualTo(propertyState);
        await Assert.That(field.FieldState).IsEqualTo(FormFieldState.Warning);
        await Assert.That(stepper.StepperState).IsEqualTo(stepperState);
        await Assert.That(theme.ThemeState).IsEqualTo(themeState);
        await Assert.That(validation.SummaryState).IsEqualTo(validationState);
        await Assert.That(asyncButton.CancelCommand).IsEqualTo(command);
        await Assert.That(asyncButton.CancelText).IsEqualTo("Stop");
    }

    /// <summary>Verifies every public parameterless MAUI UI element constructs and round-trips its declared state surface.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PublicMauiControls_WhenConstructed_RoundTripDeclaredProperties()
    {
        var assembly = typeof(AlarmBanner).Assembly;
        var constructedCount = 0;
        var roundTrippedPropertyCount = 0;

        foreach (var type in assembly.ExportedTypes)
        {
            if (type.IsAbstract || !typeof(BindableObject).IsAssignableFrom(type) || type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            var element = (BindableObject)Activator.CreateInstance(type)!;
            constructedCount++;

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (!IsRoundTrippableControlProperty(property))
                {
                    continue;
                }

                var value = property.GetValue(element);
                property.SetValue(element, value);
                roundTrippedPropertyCount++;
            }
        }

        await Assert.That(constructedCount).IsGreaterThan(0);
        await Assert.That(roundTrippedPropertyCount).IsGreaterThan(0);
    }

    /// <summary>Verifies a chip reacts to its immutable model and chip groups honor selectable and removable branches.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ChipAndChipGroup_ProjectModelsAndGuardInvalidActions()
    {
        var select = new CaptureCommand();
        var remove = new CaptureCommand();
        var active = new ChipModel(
            ActiveChipKey,
            "Active",
            new ChipModelOptions { IsSelected = true, IsRemovable = true, SelectCommand = select, RemoveCommand = remove });
        var disabled = new ChipModel("disabled", "Disabled", new ChipModelOptions { IsEnabled = false });
        var chip = new Chip { Model = active };
        var group = new ChipGroup { State = new([active, disabled], ChipGroupSelectionMode.Single) };

        var selected = group.SelectChip(ActiveChipKey);
        var removed = group.RemoveChip(ActiveChipKey);
        var disabledSelected = group.SelectChip("disabled");
        var missingRemoved = group.RemoveChip("missing");

        await Assert.That(chip.Text).IsEqualTo("Active");
        await Assert.That(chip.IsSelected).IsTrue();
        await Assert.That(chip.IsEnabled).IsTrue();
        await Assert.That(chip.RemoveCommand).IsEqualTo(remove);
        await Assert.That(group.Children.Count).IsEqualTo(ExpectedRenderedItemCount);
        await Assert.That(group.SelectionMode).IsEqualTo(ChipGroupSelectionMode.Single);
        await Assert.That(selected).IsTrue();
        await Assert.That(removed).IsTrue();
        await Assert.That(disabledSelected).IsFalse();
        await Assert.That(missingRemoved).IsFalse();
        await Assert.That(select.LastParameter).IsEqualTo(ActiveChipKey);
        await Assert.That(remove.LastParameter).IsEqualTo(ActiveChipKey);
    }

    /// <summary>Gets the button text values rendered by a layout.</summary>
    /// <param name="layout">The layout to inspect.</param>
    /// <returns>The rendered button text values.</returns>
    private static List<string> GetButtonTexts(Layout layout)
    {
        var buttonTexts = new List<string>(layout.Children.Count);
        foreach (var child in layout.Children)
        {
            if (child is Button button)
            {
                buttonTexts.Add(button.Text ?? string.Empty);
            }
        }

        return buttonTexts;
    }

    /// <summary>Determines whether a public property can be safely round-tripped in a control test.</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><c>true</c> when the property supports the round-trip test.</returns>
    private static bool IsRoundTrippableControlProperty(System.Reflection.PropertyInfo property) =>
        property is { CanRead: true, CanWrite: true }
        && property.GetIndexParameters().Length == 0
        && property.DeclaringType?.Namespace?.StartsWith("CrissCross.", StringComparison.Ordinal) == true;

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
