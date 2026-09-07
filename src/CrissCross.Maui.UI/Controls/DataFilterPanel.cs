// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays a descriptor-driven filter editing surface.</summary>
public class DataFilterPanel : ContentView
{
    /// <summary>Bindable property for <see cref="FilterPanelState"/>.</summary>
    public static readonly BindableProperty FilterPanelStateProperty = BindableProperty.Create(
        nameof(FilterPanelState),
        typeof(DataFilterPanelState),
        typeof(DataFilterPanel),
        propertyChanged: static (bindable, _, newValue) => OnFilterPanelStateChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="ApplyFiltersCommand"/>.</summary>
    public static readonly BindableProperty ApplyFiltersCommandProperty = BindableProperty.Create(
        nameof(ApplyFiltersCommand),
        typeof(ICommand),
        typeof(DataFilterPanel),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Bindable property for <see cref="ClearFiltersCommand"/>.</summary>
    public static readonly BindableProperty ClearFiltersCommandProperty = BindableProperty.Create(
        nameof(ClearFiltersCommand),
        typeof(ICommand),
        typeof(DataFilterPanel),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Bindable property for <see cref="SearchText"/>.</summary>
    public static readonly BindableProperty SearchTextProperty = BindableProperty.Create(nameof(SearchText), typeof(string), typeof(DataFilterPanel));

    /// <summary>Bindable property for <see cref="ResultCount"/>.</summary>
    public static readonly BindableProperty ResultCountProperty = BindableProperty.Create(nameof(ResultCount), typeof(int?), typeof(DataFilterPanel));

    /// <summary>Bindable property for <see cref="SubmittedQueryState"/>.</summary>
    public static readonly BindableProperty SubmittedQueryStateProperty = BindableProperty.Create(nameof(SubmittedQueryState), typeof(SearchQueryState), typeof(DataFilterPanel));

    /// <summary>Semantic text color resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Semantic accent color resource key.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Semantic accent text color resource key.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Horizontal action padding.</summary>
    private const double ActionPaddingHorizontal = 10D;

    /// <summary>Vertical action padding.</summary>
    private const double ActionPaddingVertical = 6D;

    /// <summary>Minimum action width.</summary>
    private const double ActionMinimumWidth = 112D;

    /// <summary>Root layout spacing.</summary>
    private const double RootSpacing = 12D;

    /// <summary>Root vertical padding.</summary>
    private const double RootPaddingVertical = 4D;

    /// <summary>Action button spacing.</summary>
    private const double ActionSpacing = 8D;

    /// <summary>Descriptor host spacing.</summary>
    private const double DescriptorHostSpacing = 10D;

    /// <summary>Descriptor row spacing.</summary>
    private const double DescriptorRowSpacing = 4D;

    /// <summary>Descriptor row vertical padding.</summary>
    private const double DescriptorRowPaddingVertical = 4D;

    /// <summary>Stores pending editor values by descriptor key.</summary>
    private readonly Dictionary<string, object?> _editedValues = [];

    /// <summary>Stores selected operators by descriptor key.</summary>
    private readonly Dictionary<string, FilterOperator> _selectedOperators = [];

    /// <summary>Displays the current filter summary.</summary>
    private readonly Label _summaryLabel = CreateLabel("No filters", FontAttributes.Bold);

    /// <summary>Hosts descriptor editors.</summary>
    private readonly VerticalStackLayout _descriptorHost = new() { Spacing = DescriptorHostSpacing };

    /// <summary>Applies pending filters.</summary>
    private readonly Button _applyButton = CreateActionButton("Apply filters");

    /// <summary>Clears active filters.</summary>
    private readonly Button _clearButton = CreateActionButton("Clear filters");

    /// <summary>Initializes a new instance of the <see cref="DataFilterPanel"/> class.</summary>
    public DataFilterPanel()
    {
        ApplyCommand = new PanelCommand(ApplyFilters, CanApplyFilters);
        ClearCommand = new PanelCommand(ClearFilters, CanClearFilters);
        _applyButton.Clicked += (_, _) => SubmittedQueryState = CreateQueryState();
        _clearButton.Clicked += (_, _) => SubmittedQueryState = new(SearchText, submittedText: SearchText, resultCount: ResultCount);
        Content = CreateLayout();
        ApplyState(null);
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public DataFilterPanelState? FilterPanelState
    {
        get => (DataFilterPanelState?)GetValue(FilterPanelStateProperty);
        set => SetValue(FilterPanelStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? ApplyFiltersCommand
    {
        get => (ICommand?)GetValue(ApplyFiltersCommandProperty);
        set => SetValue(ApplyFiltersCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when filters are cleared.</summary>
    public ICommand? ClearFiltersCommand
    {
        get => (ICommand?)GetValue(ClearFiltersCommandProperty);
        set => SetValue(ClearFiltersCommandProperty, value);
    }

    /// <summary>Gets or sets query text included in the applied query snapshot.</summary>
    public string? SearchText
    {
        get => (string?)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>Gets or sets result count included in the applied query snapshot.</summary>
    public int? ResultCount
    {
        get => (int?)GetValue(ResultCountProperty);
        set => SetValue(ResultCountProperty, value);
    }

    /// <summary>Gets or sets the most recent query state emitted by <see cref="ApplyFilters"/>.</summary>
    public SearchQueryState? SubmittedQueryState
    {
        get => (SearchQueryState?)GetValue(SubmittedQueryStateProperty);
        set => SetValue(SubmittedQueryStateProperty, value);
    }

    /// <summary>Gets the command that applies current filters.</summary>
    public ICommand ApplyCommand { get; }

    /// <summary>Gets the command that clears current filters.</summary>
    public ICommand ClearCommand { get; }

    /// <summary>Sets a pending filter value for a descriptor.</summary>
    /// <param name="descriptorKey">The descriptor key.</param>
    /// <param name="value">The edited value.</param>
    /// <returns><c>true</c> when the descriptor exists.</returns>
    public bool SetFilterValue(string descriptorKey, object? value)
    {
        var descriptor = FilterPanelState?.GetDescriptor(descriptorKey);
        if (descriptor is null)
        {
            return false;
        }

        _editedValues[descriptor.Key] = value;
        RaiseCommandAvailability();
        return true;
    }

    /// <summary>Applies current descriptor editor values and emits a query-state payload.</summary>
    /// <returns><c>true</c> when the external command was executed.</returns>
    public bool ApplyFilters()
    {
        var queryState = CreateQueryState();
        SubmittedQueryState = queryState;
        if (ApplyFiltersCommand?.CanExecute(queryState) != true)
        {
            return false;
        }

        ApplyFiltersCommand.Execute(queryState);
        return true;
    }

    /// <summary>Clears current filter editor values and invokes the clear command with the existing state.</summary>
    /// <returns><c>true</c> when the clear command was executed.</returns>
    public bool ClearFilters()
    {
        var state = FilterPanelState;
        _editedValues.Clear();
        SubmittedQueryState = new(SearchText, submittedText: SearchText, resultCount: ResultCount);
        ApplyState(state);
        if (ClearFiltersCommand?.CanExecute(state) != true)
        {
            return false;
        }

        ClearFiltersCommand.Execute(state);
        return true;
    }

    /// <summary>Creates a query-state snapshot from current editor values.</summary>
    /// <returns>The query-state snapshot.</returns>
    public SearchQueryState CreateQueryState()
    {
        var state = CreateEditedState();
        return state.ToSearchQueryState(SearchText, ResultCount);
    }

    /// <summary>Creates the current edited filter state.</summary>
    /// <returns>The edited filter state.</returns>
    public DataFilterPanelState CreateEditedState()
    {
        var state = FilterPanelState;
        if (state is null)
        {
            return new();
        }

        var expressions = new List<FilterExpression>();
        foreach (var descriptor in state.Descriptors)
        {
            var value = GetEditorValue(descriptor, state);
            var selectedOperator = _selectedOperators.TryGetValue(descriptor.Key, out var @operator) ? @operator : descriptor.DefaultOperator;
            expressions.Add(new(descriptor.Key, selectedOperator, value, descriptor.DisplayName));
        }

        return new(state.Descriptors, expressions, _editedValues.Count > 0, state.IsApplying);
    }

    /// <summary>Creates a label using semantic theme color resources.</summary>
    /// <param name="text">The label text.</param>
    /// <param name="fontAttributes">The font attributes.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(string text, FontAttributes fontAttributes)
    {
        var label = new Label { Text = text, FontAttributes = fontAttributes };
        label.SetDynamicResource(Label.TextColorProperty, TextColorResourceKey);
        return label;
    }

    /// <summary>Creates an action button using semantic theme color resources.</summary>
    /// <param name="text">The button text.</param>
    /// <returns>The configured button.</returns>
    private static Button CreateActionButton(string text)
    {
        var button = new Button { Text = text, Padding = new(ActionPaddingHorizontal, ActionPaddingVertical), MinimumWidthRequest = ActionMinimumWidth };
        button.SetDynamicResource(Button.BackgroundColorProperty, AccentColorResourceKey);
        button.SetDynamicResource(Button.TextColorProperty, AccentTextColorResourceKey);
        return button;
    }

    /// <summary>Runs when the filter state changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnFilterPanelStateChanged(BindableObject bindable, object? newValue)
    {
        if (bindable is not DataFilterPanel panel)
        {
            return;
        }

        panel.SeedEditors(newValue as DataFilterPanelState);
        panel.ApplyState(newValue as DataFilterPanelState);
    }

    /// <summary>Runs when command availability changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    private static void OnCommandChanged(BindableObject bindable)
    {
        if (bindable is not DataFilterPanel panel)
        {
            return;
        }

        panel.RaiseCommandAvailability();
    }

    /// <summary>Gets an expression value for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="state">The state.</param>
    /// <returns>The expression value.</returns>
    private static object? GetExpressionValue(FilterDescriptor descriptor, DataFilterPanelState? state)
    {
        foreach (var expression in state?.Expressions ?? [])
        {
            if (expression.FieldKey == descriptor.Key)
            {
                return expression.Value;
            }
        }

        return null;
    }

    /// <summary>Gets an expression operator for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="state">The state.</param>
    /// <returns>The expression operator.</returns>
    private static FilterOperator GetExpressionOperator(FilterDescriptor descriptor, DataFilterPanelState? state)
    {
        foreach (var expression in state?.Expressions ?? [])
        {
            if (expression.FieldKey == descriptor.Key && descriptor.SupportsOperator(expression.Operator))
            {
                return expression.Operator;
            }
        }

        return descriptor.DefaultOperator;
    }

    /// <summary>Finds the index of a choice value.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="value">The value.</param>
    /// <returns>The choice index, or -1.</returns>
    private static int FindChoiceIndex(FilterDescriptor descriptor, object? value)
    {
        for (var index = 0; index < (descriptor.Choices?.Count ?? 0); index++)
        {
            if (Equals(descriptor.Choices![index], value))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Finds the index of an operator.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="operator">The operator.</param>
    /// <returns>The operator index, or -1.</returns>
    private static int FindOperatorIndex(FilterDescriptor descriptor, FilterOperator @operator)
    {
        for (var index = 0; index < descriptor.SupportedOperators.Count; index++)
        {
            if (descriptor.SupportedOperators[index] == @operator)
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Creates the root layout.</summary>
    /// <returns>The root layout.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = RootSpacing, Padding = new(0D, RootPaddingVertical) };
        var actions = new HorizontalStackLayout { Spacing = ActionSpacing };
        actions.Children.Add(_applyButton);
        actions.Children.Add(_clearButton);
        root.Children.Add(_summaryLabel);
        root.Children.Add(_descriptorHost);
        root.Children.Add(actions);
        return root;
    }

    /// <summary>Seeds editor values from the supplied state.</summary>
    /// <param name="state">The state.</param>
    private void SeedEditors(DataFilterPanelState? state)
    {
        _editedValues.Clear();
        _selectedOperators.Clear();
        if (state is null)
        {
            return;
        }

        foreach (var descriptor in state.Descriptors)
        {
            _editedValues[descriptor.Key] = GetExpressionValue(descriptor, state) ?? descriptor.DefaultValue;
            _selectedOperators[descriptor.Key] = GetExpressionOperator(descriptor, state);
        }
    }

    /// <summary>Applies current state to native child presenters.</summary>
    /// <param name="state">The state.</param>
    private void ApplyState(DataFilterPanelState? state)
    {
        _descriptorHost.Children.Clear();
        _summaryLabel.Text = state?.SummaryText ?? "No filters";
        foreach (var descriptor in state?.Descriptors ?? [])
        {
            _descriptorHost.Children.Add(CreateDescriptorEditor(descriptor));
        }

        RaiseCommandAvailability();
        SemanticProperties.SetDescription(this, _summaryLabel.Text);
    }

    /// <summary>Creates an editor row for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor row.</returns>
    private VerticalStackLayout CreateDescriptorEditor(FilterDescriptor descriptor)
    {
        var row = new VerticalStackLayout { Spacing = DescriptorRowSpacing, Padding = new(0D, DescriptorRowPaddingVertical) };
        row.Children.Add(CreateLabel(descriptor.DisplayName, FontAttributes.Bold));
        row.Children.Add(CreateOperatorPicker(descriptor));
        row.Children.Add(CreateValueEditor(descriptor));
        SemanticProperties.SetDescription(row, descriptor.DisplayName);
        return row;
    }

    /// <summary>Creates an operator picker for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The operator picker.</returns>
    private Picker CreateOperatorPicker(FilterDescriptor descriptor)
    {
        var picker = new Picker { Title = $"Operator for {descriptor.DisplayName}" };
        picker.SetDynamicResource(Picker.TextColorProperty, TextColorResourceKey);
        foreach (var @operator in descriptor.SupportedOperators)
        {
            picker.Items.Add(@operator.ToString());
        }

        picker.SelectedIndex = Math.Max(0, FindOperatorIndex(descriptor, GetExpressionOperator(descriptor, FilterPanelState)));
        picker.SelectedIndexChanged += (_, _) => ApplySelectedOperator(descriptor, picker);
        return picker;
    }

    /// <summary>Creates a value editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The value editor.</returns>
    private View CreateValueEditor(FilterDescriptor descriptor) =>
        descriptor.EditorKind switch
        {
            FilterEditorKind.Boolean => CreateBooleanEditor(descriptor),
            FilterEditorKind.Enum => CreateChoiceEditor(descriptor),
            FilterEditorKind.Date or FilterEditorKind.DateTime or FilterEditorKind.DateRange => CreateDateEditor(descriptor),
            FilterEditorKind.Number => CreateTextEditor(descriptor, Keyboard.Numeric),
            _ => CreateTextEditor(descriptor, Keyboard.Text),
        };

    /// <summary>Creates a text editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="keyboard">The keyboard type.</param>
    /// <returns>The editor.</returns>
    private Entry CreateTextEditor(FilterDescriptor descriptor, Keyboard keyboard)
    {
        var entry = new Entry { Text = Convert.ToString(GetEditorValue(descriptor, FilterPanelState), CultureInfo.InvariantCulture), Keyboard = keyboard };
        entry.SetDynamicResource(Entry.TextColorProperty, TextColorResourceKey);
        entry.TextChanged += (_, args) => _ = SetFilterValue(descriptor.Key, args.NewTextValue);
        return entry;
    }

    /// <summary>Creates a boolean editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private Switch CreateBooleanEditor(FilterDescriptor descriptor)
    {
        var toggle = new Switch { IsToggled = GetEditorValue(descriptor, FilterPanelState) is true };
        toggle.Toggled += (_, args) => _ = SetFilterValue(descriptor.Key, args.Value);
        return toggle;
    }

    /// <summary>Creates a choice editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private Picker CreateChoiceEditor(FilterDescriptor descriptor)
    {
        var picker = new Picker { Title = descriptor.DisplayName };
        picker.SetDynamicResource(Picker.TextColorProperty, TextColorResourceKey);
        foreach (var choice in descriptor.Choices ?? [])
        {
            picker.Items.Add(Convert.ToString(choice, CultureInfo.InvariantCulture) ?? string.Empty);
        }

        picker.SelectedIndex = FindChoiceIndex(descriptor, GetEditorValue(descriptor, FilterPanelState));
        picker.SelectedIndexChanged += (_, _) => ApplySelectedChoice(descriptor, picker);
        return picker;
    }

    /// <summary>Creates a date editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private DatePicker CreateDateEditor(FilterDescriptor descriptor)
    {
        var value = GetEditorValue(descriptor, FilterPanelState);
        var picker = new DatePicker { Date = value is DateTime date ? date : DateTime.Today };
        picker.SetDynamicResource(DatePicker.TextColorProperty, TextColorResourceKey);
        picker.DateSelected += (_, args) => _ = SetFilterValue(descriptor.Key, args.NewDate);
        return picker;
    }

    /// <summary>Applies a selected operator value.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="picker">The picker.</param>
    private void ApplySelectedOperator(FilterDescriptor descriptor, Picker picker)
    {
        if (picker.SelectedIndex < 0 || picker.SelectedIndex >= descriptor.SupportedOperators.Count)
        {
            return;
        }

        _selectedOperators[descriptor.Key] = descriptor.SupportedOperators[picker.SelectedIndex];
    }

    /// <summary>Applies a selected choice value.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="picker">The picker.</param>
    private void ApplySelectedChoice(FilterDescriptor descriptor, Picker picker)
    {
        if (picker.SelectedIndex < 0 || picker.SelectedIndex >= (descriptor.Choices?.Count ?? 0))
        {
            return;
        }

        _ = SetFilterValue(descriptor.Key, descriptor.Choices![picker.SelectedIndex]);
    }

    /// <summary>Gets the current editor value for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="state">The state.</param>
    /// <returns>The value.</returns>
    private object? GetEditorValue(FilterDescriptor descriptor, DataFilterPanelState? state) =>
        _editedValues.TryGetValue(descriptor.Key, out var value)
            ? value
            : GetExpressionValue(descriptor, state) ?? descriptor.DefaultValue;

    /// <summary>Determines whether filters can be applied.</summary>
    /// <returns><c>true</c> when filters can be applied.</returns>
    private bool CanApplyFilters() =>
        FilterPanelState?.IsApplying != true
        && (_editedValues.Count > 0 || FilterPanelState?.CanApply == true);

    /// <summary>Determines whether filters can be cleared.</summary>
    /// <returns><c>true</c> when filters can be cleared.</returns>
    private bool CanClearFilters() => FilterPanelState?.CanClear == true;

    /// <summary>Refreshes native button command bindings and payloads.</summary>
    private void RefreshCommandTargets()
    {
        var queryState = CreateQueryState();
        _applyButton.Command = ApplyFiltersCommand;
        _applyButton.CommandParameter = queryState;
        _applyButton.IsEnabled = CanApplyFilters() && ApplyFiltersCommand?.CanExecute(queryState) == true;

        var state = FilterPanelState;
        _clearButton.Command = ClearFiltersCommand;
        _clearButton.CommandParameter = state;
        _clearButton.IsEnabled = CanClearFilters() && ClearFiltersCommand?.CanExecute(state) == true;
    }

    /// <summary>Updates button and command availability.</summary>
    private void RaiseCommandAvailability()
    {
        RefreshCommandTargets();
        (ApplyCommand as PanelCommand)?.RaiseCanExecuteChanged();
        (ClearCommand as PanelCommand)?.RaiseCanExecuteChanged();
    }

    /// <summary>Command wrapper for panel actions.</summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">The command guard.</param>
    private sealed class PanelCommand(Func<bool> execute, Func<bool> canExecute) : ICommand
    {
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc />
        public bool CanExecute(object? parameter) => canExecute();

        /// <inheritdoc />
        public void Execute(object? parameter) => _ = execute();

        /// <summary>Raises command availability changes.</summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
