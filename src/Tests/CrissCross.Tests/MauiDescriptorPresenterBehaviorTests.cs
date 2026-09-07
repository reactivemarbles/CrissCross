// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using CrissCross.Maui.UI.Controls;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Tests for MAUI descriptor-driven native filter and property presenters.</summary>
[System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class MauiDescriptorPresenterBehaviorTests
{
    /// <summary>Provides the active filter count used by filter bar tests.</summary>
    private const int ActiveFilterCount = 2;

    /// <summary>Provides the rendered root child count used by descriptor presenter tests.</summary>
    private const int DescriptorRootChildCount = 3;

    /// <summary>Provides the data filter result count used by filter panel tests.</summary>
    private const int FilterPanelResultCount = 3;

    /// <summary>Provides the rendered filter panel descriptor count.</summary>
    private const int FilterDescriptorCount = 2;

    /// <summary>Provides the rendered property group count.</summary>
    private const int PropertyGroupCount = 2;

    /// <summary>Provides the closed status value used by filter tests.</summary>
    private const string ClosedStatus = "Closed";

    /// <summary>Provides the flow property key used by property tests.</summary>
    private const string FlowKey = "flow";

    /// <summary>Provides the edited flow value used by property tests.</summary>
    private const double FlowEditedValue = 12.5D;

    /// <summary>Provides the original flow value used by property tests.</summary>
    private const double FlowOriginalValue = 10D;

    /// <summary>Provides the serial property key used by property tests.</summary>
    private const string SerialKey = "serial";

    /// <summary>Provides the serial value used by property tests.</summary>
    private const string SerialValue = "A-100";

    /// <summary>Provides the status field key used by filter tests.</summary>
    private const string StatusKey = "status";

    /// <summary>Gets a debugger-safe representation of this test fixture.</summary>
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Verifies the MAUI filter bar renders active chips and emits clear/remove command payloads.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task FilterBar_ActiveFilters_RendersChipsAndCommandPayloads()
    {
        var clearCommand = new CaptureCommand();
        var removeCommand = new CaptureCommand();
        var removable = new FilterToken(StatusKey, FilterOperator.Equals, "Open", "Status equals Open");
        var fixedToken = new FilterToken("owner", FilterOperator.Equals, "Ops", "Owner equals Ops", false);
        var state = new SearchQueryState("pump", submittedText: "pump", resultCount: 7, filters: [removable, fixedToken]);
        var target = new FilterBar { ClearFiltersCommand = clearCommand, RemoveFilterCommand = removeCommand, SearchState = state };
        var root = (VerticalStackLayout)target.Content!;
        var chips = (FlexLayout)root.Children[1];

        var removedActive = target.RemoveFilter(removable);
        var removedFixed = target.RemoveFilter(fixedToken);
        var cleared = target.ClearFilters();

        await Assert.That(root.Children.Count).IsEqualTo(DescriptorRootChildCount);
        await Assert.That(chips.Children.Count).IsEqualTo(ActiveFilterCount);
        await Assert.That(removedActive).IsTrue();
        await Assert.That(removedFixed).IsFalse();
        await Assert.That(cleared).IsTrue();
        await Assert.That(removeCommand.LastParameter).IsEqualTo(removable);
        await Assert.That(clearCommand.LastParameter).IsEqualTo(state);
        await Assert.That(SemanticProperties.GetDescription(target)).Contains("2 active filters");
    }

    /// <summary>Verifies the MAUI data filter panel renders descriptor editors and emits query-state payloads.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DataFilterPanel_Descriptors_EditApplyAndClearCommandPayloads()
    {
        var applyCommand = new CaptureCommand();
        var clearCommand = new CaptureCommand();
        var status = new FilterDescriptor(StatusKey, "Status", FilterEditorKind.Enum, [FilterOperator.Equals], ["Open", ClosedStatus], "Open");
        var priority = new FilterDescriptor("priority", "Priority", FilterEditorKind.Number);
        var state = new DataFilterPanelState([status, priority], [new FilterExpression(StatusKey, FilterOperator.Equals, "Open", "Status")]);
        var target = new DataFilterPanel
        {
            ApplyFiltersCommand = applyCommand,
            ClearFiltersCommand = clearCommand,
            FilterPanelState = state,
            SearchText = "pump",
            ResultCount = FilterPanelResultCount,
        };
        var root = (VerticalStackLayout)target.Content!;
        var descriptors = (VerticalStackLayout)root.Children[1];

        var acceptedEdit = target.SetFilterValue(StatusKey, ClosedStatus);
        var rejectedEdit = target.SetFilterValue("missing", "ignored");
        var applied = target.ApplyFilters();
        var queryState = (SearchQueryState)applyCommand.LastParameter!;
        var submittedBeforeClear = target.SubmittedQueryState;
        var cleared = target.ClearFilters();

        await Assert.That(descriptors.Children.Count).IsEqualTo(FilterDescriptorCount);
        await Assert.That(acceptedEdit).IsTrue();
        await Assert.That(rejectedEdit).IsFalse();
        await Assert.That(applied).IsTrue();
        await Assert.That(cleared).IsTrue();
        await Assert.That(queryState.Text).IsEqualTo("pump");
        await Assert.That(queryState.ResultCount).IsEqualTo(FilterPanelResultCount);
        await Assert.That(queryState.ActiveFilters[0].Value).IsEqualTo(ClosedStatus);
        await Assert.That(submittedBeforeClear).IsEqualTo(queryState);
        await Assert.That(clearCommand.LastParameter).IsEqualTo(state);
    }

    /// <summary>Verifies the MAUI property grid renders grouped editors and commits edited descriptor state.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task PropertyGridLite_Descriptors_EditReadOnlyValidationAndCommitPayloads()
    {
        var setValueCommand = new CaptureCommand();
        var commitCommand = new CaptureCommand();
        var flow = new PropertyDescriptorModel(
            FlowKey,
            "Flow target",
            new PropertyDescriptorOptions
            {
                Category = "Runtime",
                EditorKind = PropertyEditorKind.Number,
                Value = FlowOriginalValue,
                OriginalValue = FlowOriginalValue,
                SetValueCommand = setValueCommand,
                ValidationMessages = [new ValidationMessage("flow", "Flow target", "verify instrument range", ValidationSeverity.Warning)],
            });
        var serial = new PropertyDescriptorModel(
            SerialKey,
            "Serial number",
            new PropertyDescriptorOptions { Category = "Identity", Value = SerialValue, OriginalValue = SerialValue, IsReadOnly = true });
        var state = new PropertyGridState([flow, serial]);
        var target = new PropertyGridLite { CommitChangesCommand = commitCommand, PropertyGridState = state };
        var root = (VerticalStackLayout)target.Content!;
        var groups = (VerticalStackLayout)root.Children[1];

        var edited = target.EditProperty(FlowKey, FlowEditedValue);
        var rejectedReadOnly = target.EditProperty(SerialKey, "B-200");
        var committed = target.CommitChanges();
        var setValueDescriptor = (PropertyDescriptorModel)setValueCommand.LastParameter!;
        var committedState = (PropertyGridState)commitCommand.LastParameter!;

        await Assert.That(groups.Children.Count).IsEqualTo(PropertyGroupCount);
        await Assert.That(GetLabelTexts(target).Exists(static text => text.Contains("verify instrument range", StringComparison.Ordinal))).IsTrue();
        await Assert.That(edited).IsTrue();
        await Assert.That(rejectedReadOnly).IsFalse();
        await Assert.That(committed).IsTrue();
        await Assert.That(setValueDescriptor.Value).IsEqualTo(FlowEditedValue);
        await Assert.That(committedState.GetDescriptor(FlowKey)?.Value).IsEqualTo(FlowEditedValue);
        await Assert.That(committedState.GetDescriptor(SerialKey)?.Value).IsEqualTo(SerialValue);
        await Assert.That(committedState.CanCommit).IsTrue();
        await Assert.That(SemanticProperties.GetDescription(target)).Contains("modified");
    }

    /// <summary>Gets label text values from a MAUI element tree.</summary>
    /// <param name="root">The root view.</param>
    /// <returns>The label text values.</returns>
    private static List<string> GetLabelTexts(View root)
    {
        var labels = new List<string>();
        AddLabelTexts(root, labels);
        return labels;
    }

    /// <summary>Adds label text values from a MAUI element tree.</summary>
    /// <param name="view">The view to inspect.</param>
    /// <param name="labels">The label collection.</param>
    private static void AddLabelTexts(IView view, List<string> labels)
    {
        if (view is Label label)
        {
            labels.Add(label.Text ?? string.Empty);
        }

        if (view is ContentView { Content: not null } contentView)
        {
            AddLabelTexts(contentView.Content, labels);
        }

        if (view is not Layout layout)
        {
            return;
        }

        foreach (var child in layout.Children)
        {
            AddLabelTexts(child, labels);
        }
    }

    /// <summary>Captures command execution and the most recent command parameter.</summary>
    private sealed class CaptureCommand : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <summary>Gets the most recent command parameter.</summary>
        public object? LastParameter { get; private set; }

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => true;

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            LastParameter = parameter;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
