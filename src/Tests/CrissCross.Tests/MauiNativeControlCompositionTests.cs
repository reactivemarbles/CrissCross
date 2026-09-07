// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Verifies native MAUI composition for interactive controls that project shared state.</summary>
public sealed class MauiNativeControlCompositionTests
{
    /// <summary>The selected time range label.</summary>
    private const string ShiftWindowLabel = "Shift window";

    /// <summary>Provides the pager page size used by tests.</summary>
    private const int PageSize = 10;

    /// <summary>Provides the pager total item count used by tests.</summary>
    private const int TotalItemCount = 25;

    /// <summary>Provides the middle page index used by tests.</summary>
    private const int MiddlePageIndex = 1;

    /// <summary>Provides the next page index expected after pressing Next.</summary>
    private const int NextPageIndex = 2;

    /// <summary>Provides the expected composed pager button count.</summary>
    private const int PagerButtonCount = 4;

    /// <summary>Provides the expected composed theme button count when high contrast is supported.</summary>
    private const int ThemeButtonCount = 4;

    /// <summary>Provides the expected single element count.</summary>
    private const int ExpectedSingleCount = 1;

    /// <summary>Provides the expected number of endpoint input pairs.</summary>
    private const int EndpointInputCount = 2;

    /// <summary>Provides the fixed sample year used by range tests.</summary>
    private const int RangeYear = 2026;

    /// <summary>Provides the fixed sample month used by range tests.</summary>
    private const int RangeMonth = 9;

    /// <summary>Provides the fixed sample day used by range tests.</summary>
    private const int RangeDay = 6;

    /// <summary>Provides the fixed sample start hour used by range tests.</summary>
    private const int RangeStartHour = 8;

    /// <summary>Provides the fixed sample end hour used by range tests.</summary>
    private const int RangeEndHour = 10;

    /// <summary>Provides the fixed minute/second component used by range tests.</summary>
    private const int EmptyTimeComponent = 0;

    /// <summary>Provides the end offset hour used by the mixed-offset range test.</summary>
    private const int MixedRangeEndOffsetHours = 2;

    /// <summary>Provides a valid range start offset used by tests.</summary>
    private static readonly DateTimeOffset RangeStart = new(
        RangeYear,
        RangeMonth,
        RangeDay,
        RangeStartHour,
        EmptyTimeComponent,
        EmptyTimeComponent,
        TimeSpan.Zero);

    /// <summary>Provides a valid range end offset used by tests.</summary>
    private static readonly DateTimeOffset RangeEnd = new(
        RangeYear,
        RangeMonth,
        RangeDay,
        RangeEndHour,
        EmptyTimeComponent,
        EmptyTimeComponent,
        TimeSpan.Zero);

    /// <summary>Verifies DataPager renders native labels/buttons and emits page requests from button interaction.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataPager_RendersNativeNavigationAndInvokesPageCommand()
    {
        var command = new CaptureCommand();
        var pager = new DataPager { PaginationState = new(MiddlePageIndex, PageSize, TotalItemCount), PageRequestCommand = command, SortKey = "timestamp", SortDescending = true };

        var buttons = FindDescendants<Button>(pager);
        var labels = FindDescendants<Label>(pager);
        FindButton(buttons, "Next").Command?.Execute(null);

        await Assert.That(GetButtonTexts(buttons)).IsEquivalentTo(["First", "Previous", "Next", "Last"]);
        await Assert.That(GetLabelTexts(labels)).IsEquivalentTo(["11-20 of 25", "Page 2 of 3"]);
        await Assert.That(FindButton(buttons, "First").IsEnabled).IsTrue();
        await Assert.That(FindButton(buttons, "Previous").IsEnabled).IsTrue();
        await Assert.That(buttons.Count).IsEqualTo(PagerButtonCount);
        await Assert.That(pager.CurrentRequest?.PageIndex).IsEqualTo(NextPageIndex);
        await Assert.That(pager.CurrentRequest?.SortKey).IsEqualTo("timestamp");
        await Assert.That(pager.CurrentRequest?.SortDescending).IsTrue();
        await Assert.That(command.LastParameter).IsEqualTo(pager.CurrentRequest);
    }

    /// <summary>Verifies ThemeSwitcher renders allowed choices and emits ThemeChoice command parameters.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ThemeSwitcher_RendersChoicesAndInvokesThemeChoiceCommand()
    {
        var command = new CaptureCommand();
        var switcher = new ThemeSwitcher { ThemeState = new(ThemeChoice.System, ThemeChoice.Dark, supportsHighContrast: true), ChangeThemeCommand = command };

        var buttons = FindDescendants<Button>(switcher);
        var description = GetSingle(FindDescendants<Label>(switcher));
        var highContrastButton = FindButton(buttons, "High contrast");
        highContrastButton.Command?.Execute(highContrastButton.CommandParameter);

        await Assert.That(GetButtonTexts(buttons)).IsEquivalentTo(["System", "Light", "Dark", "High contrast"]);
        await Assert.That(AllButtonsUseCommand(buttons, command)).IsTrue();
        await Assert.That(buttons.Count).IsEqualTo(ThemeButtonCount);
        await Assert.That(description.Text).IsEqualTo("System (Dark)");
        await Assert.That(command.LastParameter).IsEqualTo(ThemeChoice.HighContrast);
    }

    /// <summary>Verifies DateTimeRangePicker renders native inputs, blocks invalid ranges, and emits valid ranges.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRangePicker_ValidatesInputsAndInvokesRangeCommand()
    {
        var command = new CaptureCommand();
        var picker = new DateTimeRangePicker { Range = new(RangeStart, RangeEnd, DateTimeRangePreset.Custom, ShiftWindowLabel), ApplyRangeCommand = command };
        var datePickers = FindDescendants<DatePicker>(picker);
        var timePickers = FindDescendants<TimePicker>(picker);
        var applyButton = GetSingle(FindDescendants<Button>(picker));

        datePickers[1].Date = RangeStart.Date.AddDays(-1);
        var invalidApplied = picker.ApplyCurrentRange();
        var invalidCommandParameter = command.LastParameter;

        datePickers[1].Date = RangeEnd.Date;
        timePickers[1].Time = RangeEnd.TimeOfDay;
        var validApplied = picker.ApplyCurrentRange();
        var appliedRange = (DateTimeRange?)command.LastParameter;

        await Assert.That(datePickers.Count).IsEqualTo(EndpointInputCount);
        await Assert.That(timePickers.Count).IsEqualTo(EndpointInputCount);
        await Assert.That(applyButton.Text).IsEqualTo("Apply range");
        await Assert.That(invalidApplied).IsFalse();
        await Assert.That(invalidCommandParameter).IsNull();
        await Assert.That(validApplied).IsTrue();
        await Assert.That(appliedRange?.IsValid).IsTrue();
        await Assert.That(appliedRange?.Start).IsEqualTo(RangeStart);
        await Assert.That(appliedRange?.End).IsEqualTo(RangeEnd);
    }

    /// <summary>Verifies clearing native range inputs preserves null endpoints and prevents invalid apply.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRangePicker_ClearingNativeInputsPreservesNullAndDisablesApply()
    {
        var command = new CaptureCommand();
        var picker = new DateTimeRangePicker { Range = new(RangeStart, RangeEnd, DateTimeRangePreset.Custom, ShiftWindowLabel), ApplyRangeCommand = command };
        var datePickers = FindDescendants<DatePicker>(picker);
        var timePickers = FindDescendants<TimePicker>(picker);
        var applyButton = GetSingle(FindDescendants<Button>(picker));

        datePickers[0].Date = null;
        timePickers[0].Time = null;
        var applied = picker.ApplyCurrentRange();

        await Assert.That(datePickers.Count).IsEqualTo(EndpointInputCount);
        await Assert.That(timePickers.Count).IsEqualTo(EndpointInputCount);
        await Assert.That(applied).IsFalse();
        await Assert.That(picker.Range?.Start).IsNull();
        await Assert.That(picker.Range?.End).IsEqualTo(RangeEnd);
        await Assert.That(picker.Range?.IsValid).IsFalse();
        await Assert.That(applyButton.IsEnabled).IsFalse();
        await Assert.That(command.LastParameter).IsNull();
    }

    /// <summary>Verifies DateTimeRangePicker updates apply availability when the caller command availability changes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRangePicker_TracksApplyCommandCanExecuteChanges()
    {
        var command = new CaptureCommand { CanExecuteResult = false };
        var picker = new DateTimeRangePicker { Range = new(RangeStart, RangeEnd, DateTimeRangePreset.Custom, ShiftWindowLabel), ApplyRangeCommand = command };
        var applyButton = GetSingle(FindDescendants<Button>(picker));

        command.CanExecuteResult = true;
        command.RaiseCanExecuteChanged();
        var enabledAfterCommandChange = applyButton.IsEnabled;

        command.CanExecuteResult = false;
        command.RaiseCanExecuteChanged();

        await Assert.That(enabledAfterCommandChange).IsTrue();
        await Assert.That(applyButton.IsEnabled).IsFalse();
    }

    /// <summary>Verifies DateTimeRangePicker preserves distinct start and end offsets.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DateTimeRangePicker_PreservesMixedEndpointOffsets()
    {
        var command = new CaptureCommand();
        var endOffset = TimeSpan.FromHours(MixedRangeEndOffsetHours);
        var mixedEnd = new DateTimeOffset(
            RangeYear,
            RangeMonth,
            RangeDay,
            RangeEndHour,
            EmptyTimeComponent,
            EmptyTimeComponent,
            endOffset);
        var picker = new DateTimeRangePicker { Range = new(RangeStart, mixedEnd, DateTimeRangePreset.Custom, "Mixed offset window"), ApplyRangeCommand = command };

        var applied = picker.ApplyCurrentRange();
        var appliedRange = (DateTimeRange?)command.LastParameter;

        await Assert.That(applied).IsTrue();
        await Assert.That(appliedRange?.Start?.Offset).IsEqualTo(RangeStart.Offset);
        await Assert.That(appliedRange?.End?.Offset).IsEqualTo(endOffset);
    }

    /// <summary>Finds descendants of the requested MAUI element type.</summary>
    /// <typeparam name="T">The descendant element type.</typeparam>
    /// <param name="root">The root element.</param>
    /// <returns>The matching descendants.</returns>
    private static List<T> FindDescendants<T>(Element root)
        where T : Element
    {
        var matches = new List<T>();
        AddDescendants(root, matches);
        return matches;
    }

    /// <summary>Adds descendants of the requested type to the supplied list.</summary>
    /// <typeparam name="T">The descendant element type.</typeparam>
    /// <param name="element">The element to inspect.</param>
    /// <param name="matches">The collected matches.</param>
    private static void AddDescendants<T>(Element element, ICollection<T> matches)
        where T : Element
    {
        if (element is T match)
        {
            matches.Add(match);
        }

        if (element is ContentView { Content: Element content })
        {
            AddDescendants(content, matches);
        }

        if (element is not Layout layout)
        {
            return;
        }

        foreach (var child in layout.Children)
        {
            if (child is not Element childElement)
            {
                continue;
            }

            AddDescendants(childElement, matches);
        }
    }

    /// <summary>Finds a button by text.</summary>
    /// <param name="buttons">The buttons to inspect.</param>
    /// <param name="text">The requested text.</param>
    /// <returns>The matching button.</returns>
    private static Button FindButton(List<Button> buttons, string text) =>
        buttons.Find(button => button.Text == text) ?? throw new InvalidOperationException("Expected a matching button.");

    /// <summary>Gets button text values.</summary>
    /// <param name="buttons">The buttons to inspect.</param>
    /// <returns>The button text values.</returns>
    private static string[] GetButtonTexts(List<Button> buttons)
    {
        var texts = new string[buttons.Count];
        for (var index = 0; index < buttons.Count; index++)
        {
            texts[index] = buttons[index].Text ?? throw new InvalidOperationException("Expected button text.");
        }

        return texts;
    }

    /// <summary>Gets label text values.</summary>
    /// <param name="labels">The labels to inspect.</param>
    /// <returns>The label text values.</returns>
    private static string[] GetLabelTexts(List<Label> labels)
    {
        var texts = new string[labels.Count];
        for (var index = 0; index < labels.Count; index++)
        {
            texts[index] = labels[index].Text ?? throw new InvalidOperationException("Expected label text.");
        }

        return texts;
    }

    /// <summary>Checks that all buttons use the expected command.</summary>
    /// <param name="buttons">The buttons to inspect.</param>
    /// <param name="command">The expected command.</param>
    /// <returns>Whether every button uses the command.</returns>
    private static bool AllButtonsUseCommand(List<Button> buttons, ICommand command)
    {
        if (buttons.Count == 0)
        {
            return false;
        }

        for (var index = 0; index < buttons.Count; index++)
        {
            if (!ReferenceEquals(buttons[index].Command, command))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Gets a single item from a list.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items to inspect.</param>
    /// <returns>The only item.</returns>
    private static T GetSingle<T>(IReadOnlyList<T> items)
    {
        if (items.Count != ExpectedSingleCount)
        {
            throw new InvalidOperationException("Expected a single matching item.");
        }

        return items[0];
    }

    /// <summary>Captures the last command parameter.</summary>
    private sealed class CaptureCommand : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets or sets a value indicating whether the command can execute.</summary>
        public bool CanExecuteResult { get; set; } = true;

        /// <summary>Gets the last command parameter.</summary>
        public object? LastParameter { get; private set; }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => CanExecuteResult;

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Raises command availability changes for tests.</summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
