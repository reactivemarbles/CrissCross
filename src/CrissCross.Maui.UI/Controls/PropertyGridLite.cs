// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays categorized editable property descriptors without reflection-heavy discovery.</summary>
public class PropertyGridLite : ContentView
{
    /// <summary>Bindable property for <see cref="PropertyGridState"/>.</summary>
    public static readonly BindableProperty PropertyGridStateProperty = BindableProperty.Create(
        nameof(PropertyGridState),
        typeof(PropertyGridState),
        typeof(PropertyGridLite),
        propertyChanged: static (bindable, _, newValue) => OnPropertyGridStateChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="UpdatePropertyCommand"/>.</summary>
    public static readonly BindableProperty UpdatePropertyCommandProperty = BindableProperty.Create(
        nameof(UpdatePropertyCommand),
        typeof(ICommand),
        typeof(PropertyGridLite),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Bindable property for <see cref="CommitChangesCommand"/>.</summary>
    public static readonly BindableProperty CommitChangesCommandProperty = BindableProperty.Create(
        nameof(CommitChangesCommand),
        typeof(ICommand),
        typeof(PropertyGridLite),
        propertyChanged: static (bindable, _, _) => OnCommandChanged(bindable));

    /// <summary>Bindable property for <see cref="SearchText"/>.</summary>
    public static readonly BindableProperty SearchTextProperty = BindableProperty.Create(
        nameof(SearchText),
        typeof(string),
        typeof(PropertyGridLite),
        propertyChanged: static (bindable, _, _) => OnSearchTextChanged(bindable));

    /// <summary>Semantic text color resource key.</summary>
    private const string TextColorResourceKey = "CrissCrossTextColor";

    /// <summary>Semantic accent color resource key.</summary>
    private const string AccentColorResourceKey = "CrissCrossAccentColor";

    /// <summary>Semantic accent text color resource key.</summary>
    private const string AccentTextColorResourceKey = "CrissCrossAccentTextColor";

    /// <summary>Semantic warning text color resource key.</summary>
    private const string WarningTextColorResourceKey = "CrissCrossWarningTextColor";

    /// <summary>Horizontal action padding.</summary>
    private const double ActionPaddingHorizontal = 10D;

    /// <summary>Vertical action padding.</summary>
    private const double ActionPaddingVertical = 6D;

    /// <summary>Minimum action width.</summary>
    private const double ActionMinimumWidth = 120D;

    /// <summary>Root layout spacing.</summary>
    private const double RootSpacing = 12D;

    /// <summary>Root vertical padding.</summary>
    private const double RootPaddingVertical = 4D;

    /// <summary>Group presenter spacing.</summary>
    private const double GroupSpacing = 8D;

    /// <summary>Group presenter vertical padding.</summary>
    private const double GroupPaddingVertical = 4D;

    /// <summary>Descriptor presenter spacing.</summary>
    private const double DescriptorSpacing = 4D;

    /// <summary>Descriptor presenter vertical padding.</summary>
    private const double DescriptorPaddingVertical = 4D;

    /// <summary>Stores edited descriptors by stable property key.</summary>
    private readonly Dictionary<string, PropertyDescriptorModel> _editedDescriptors = [];

    /// <summary>Displays current property grid summary.</summary>
    private readonly Label _summaryLabel = CreateLabel("No properties", FontAttributes.Bold);

    /// <summary>Hosts category and editor presenters.</summary>
    private readonly VerticalStackLayout _groupHost = new() { Spacing = RootSpacing };

    /// <summary>Commits pending edits.</summary>
    private readonly Button _commitButton = CreateActionButton("Commit changes");

    /// <summary>Initializes a new instance of the <see cref="PropertyGridLite"/> class.</summary>
    public PropertyGridLite()
    {
        CommitCommand = new GridCommand(CommitChanges, CanCommitChanges);
        Content = CreateLayout();
        ApplyState(new());
    }

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public PropertyGridState? PropertyGridState
    {
        get => (PropertyGridState?)GetValue(PropertyGridStateProperty);
        set => SetValue(PropertyGridStateProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? UpdatePropertyCommand
    {
        get => (ICommand?)GetValue(UpdatePropertyCommandProperty);
        set => SetValue(UpdatePropertyCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked when edited descriptors are committed.</summary>
    public ICommand? CommitChangesCommand
    {
        get => (ICommand?)GetValue(CommitChangesCommandProperty);
        set => SetValue(CommitChangesCommandProperty, value);
    }

    /// <summary>Gets or sets search text used to project visible descriptors.</summary>
    public string? SearchText
    {
        get => (string?)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>Gets the command that commits current property edits.</summary>
    public ICommand CommitCommand { get; }

    /// <summary>Edits a property descriptor value using the public descriptor model.</summary>
    /// <param name="descriptorKey">The stable descriptor key.</param>
    /// <param name="value">The edited value.</param>
    /// <returns><c>true</c> when the descriptor exists and accepts edits.</returns>
    public bool EditProperty(string descriptorKey, object? value)
    {
        var descriptor = PropertyGridState?.GetDescriptor(descriptorKey);
        if (descriptor is not { CanEdit: true })
        {
            return false;
        }

        var updatedDescriptor = descriptor.WithValue(value);
        _editedDescriptors[updatedDescriptor.Key] = updatedDescriptor;
        ExecuteSetValueCommand(updatedDescriptor);
        ApplyState(CreateCurrentState());
        return true;
    }

    /// <summary>Commits current property edits using the configured command.</summary>
    /// <returns><c>true</c> when the commit command was executed.</returns>
    public bool CommitChanges()
    {
        var state = CreateCurrentState();
        if (!state.CanCommit)
        {
            return false;
        }

        var command = CommitChangesCommand ?? UpdatePropertyCommand;
        if (command?.CanExecute(state) != true)
        {
            return false;
        }

        command.Execute(state);
        return true;
    }

    /// <summary>Creates a state snapshot from the current property editor values.</summary>
    /// <returns>The projected property grid state.</returns>
    public PropertyGridState CreateCurrentState()
    {
        var state = PropertyGridState;
        if (state is null)
        {
            return new();
        }

        var descriptors = new List<PropertyDescriptorModel>(state.Descriptors.Count);
        foreach (var descriptor in state.Descriptors)
        {
            descriptors.Add(_editedDescriptors.TryGetValue(descriptor.Key, out var editedDescriptor) ? editedDescriptor : descriptor);
        }

        return new(descriptors, SearchText ?? state.SearchText, state.IsCommitting);
    }

    /// <summary>Creates a label using semantic theme color resources.</summary>
    /// <param name="text">The label text.</param>
    /// <param name="fontAttributes">The font attributes.</param>
    /// <returns>The configured label.</returns>
    private static Label CreateLabel(string text, FontAttributes fontAttributes)
    {
        var label = new Label { Text = text, FontAttributes = fontAttributes, VerticalTextAlignment = TextAlignment.Center };
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

    /// <summary>Runs when the grid state changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnPropertyGridStateChanged(BindableObject bindable, object? newValue)
    {
        if (bindable is not PropertyGridLite grid)
        {
            return;
        }

        grid._editedDescriptors.Clear();
        grid.ApplyState(newValue as PropertyGridState ?? new());
    }

    /// <summary>Runs when command availability changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    private static void OnCommandChanged(BindableObject bindable)
    {
        if (bindable is not PropertyGridLite grid)
        {
            return;
        }

        grid.RaiseCommandAvailability();
    }

    /// <summary>Runs when search text changes.</summary>
    /// <param name="bindable">The bindable object.</param>
    private static void OnSearchTextChanged(BindableObject bindable)
    {
        if (bindable is not PropertyGridLite grid)
        {
            return;
        }

        grid.ApplyState(grid.CreateCurrentState());
    }

    /// <summary>Adds validation messages to a descriptor presenter.</summary>
    /// <param name="row">The descriptor row.</param>
    /// <param name="descriptor">The descriptor.</param>
    private static void AddValidationMessages(VerticalStackLayout row, PropertyDescriptorModel descriptor)
    {
        foreach (var message in descriptor.ValidationMessages)
        {
            var label = CreateLabel(message.DisplayText, FontAttributes.Italic);
            label.SetDynamicResource(Label.TextColorProperty, WarningTextColorResourceKey);
            SemanticProperties.SetDescription(label, message.DisplayText);
            row.Children.Add(label);
        }
    }

    /// <summary>Executes a descriptor-level set-value command.</summary>
    /// <param name="descriptor">The updated descriptor.</param>
    private static void ExecuteSetValueCommand(PropertyDescriptorModel descriptor)
    {
        if (descriptor.SetValueCommand?.CanExecute(descriptor) != true)
        {
            return;
        }

        descriptor.SetValueCommand.Execute(descriptor);
    }

    /// <summary>Finds the index of a choice value.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="value">The value.</param>
    /// <returns>The choice index, or -1.</returns>
    private static int FindChoiceIndex(PropertyDescriptorModel descriptor, object? value)
    {
        for (var index = 0; index < descriptor.Choices.Count; index++)
        {
            if (Equals(descriptor.Choices[index], value))
            {
                return index;
            }
        }

        return -1;
    }

    /// <summary>Creates a read-only value presenter.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The value presenter.</returns>
    private static Label CreateReadOnlyPresenter(PropertyDescriptorModel descriptor)
    {
        var label = CreateLabel(descriptor.ValueDisplayText, FontAttributes.Italic);
        SemanticProperties.SetDescription(label, $"{descriptor.DisplayName} read only");
        return label;
    }

    /// <summary>Creates the root layout.</summary>
    /// <returns>The root layout.</returns>
    private VerticalStackLayout CreateLayout()
    {
        var root = new VerticalStackLayout { Spacing = RootSpacing, Padding = new(0D, RootPaddingVertical) };
        root.Children.Add(_summaryLabel);
        root.Children.Add(_groupHost);
        root.Children.Add(_commitButton);
        return root;
    }

    /// <summary>Applies grid state to native child presenters.</summary>
    /// <param name="state">The grid state.</param>
    private void ApplyState(PropertyGridState state)
    {
        _groupHost.Children.Clear();
        _summaryLabel.Text = state.SummaryText;
        foreach (var group in state.Categories)
        {
            _groupHost.Children.Add(CreateGroupPresenter(group));
        }

        RaiseCommandAvailability();
        SemanticProperties.SetDescription(this, _summaryLabel.Text);
    }

    /// <summary>Creates a category presenter.</summary>
    /// <param name="group">The descriptor group.</param>
    /// <returns>The group presenter.</returns>
    private VerticalStackLayout CreateGroupPresenter(PropertyDescriptorGroup group)
    {
        var container = new VerticalStackLayout { Spacing = GroupSpacing, Padding = new(0D, GroupPaddingVertical) };
        container.Children.Add(CreateLabel(group.Name, FontAttributes.Bold));
        foreach (var descriptor in group.Descriptors)
        {
            container.Children.Add(CreateDescriptorPresenter(descriptor));
        }

        SemanticProperties.SetDescription(container, group.Name);
        return container;
    }

    /// <summary>Creates a descriptor presenter.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The descriptor presenter.</returns>
    private VerticalStackLayout CreateDescriptorPresenter(PropertyDescriptorModel descriptor)
    {
        var row = new VerticalStackLayout { Spacing = DescriptorSpacing, Padding = new(0D, DescriptorPaddingVertical) };
        row.Children.Add(CreateLabel(descriptor.DisplayName, FontAttributes.None));
        row.Children.Add(CreateValuePresenter(descriptor));
        AddValidationMessages(row, descriptor);
        SemanticProperties.SetDescription(row, $"{descriptor.DisplayName}: {descriptor.ValueDisplayText}");
        return row;
    }

    /// <summary>Creates the value editor for a descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The value presenter.</returns>
    private View CreateValuePresenter(PropertyDescriptorModel descriptor) => !descriptor.CanEdit
        ? CreateReadOnlyPresenter(descriptor)
        : descriptor.EditorKind switch
        {
            PropertyEditorKind.Boolean => CreateBooleanEditor(descriptor),
            PropertyEditorKind.Enum => CreateChoiceEditor(descriptor),
            PropertyEditorKind.Date or PropertyEditorKind.DateTime => CreateDateEditor(descriptor),
            PropertyEditorKind.Command => CreateCommandPresenter(descriptor),
            PropertyEditorKind.Number => CreateTextEditor(descriptor, Keyboard.Numeric),
            _ => CreateTextEditor(descriptor, Keyboard.Text),
        };

    /// <summary>Creates a text editor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="keyboard">The keyboard type.</param>
    /// <returns>The editor.</returns>
    private Entry CreateTextEditor(PropertyDescriptorModel descriptor, Keyboard keyboard)
    {
        var entry = new Entry { Text = descriptor.ValueDisplayText, Keyboard = keyboard };
        entry.SetDynamicResource(Entry.TextColorProperty, TextColorResourceKey);
        entry.TextChanged += (_, args) => _ = EditProperty(descriptor.Key, args.NewTextValue);
        return entry;
    }

    /// <summary>Creates a boolean editor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private Switch CreateBooleanEditor(PropertyDescriptorModel descriptor)
    {
        var toggle = new Switch { IsToggled = descriptor.Value is true };
        toggle.Toggled += (_, args) => _ = EditProperty(descriptor.Key, args.Value);
        return toggle;
    }

    /// <summary>Creates an explicit choice editor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private Picker CreateChoiceEditor(PropertyDescriptorModel descriptor)
    {
        var picker = new Picker { Title = descriptor.DisplayName };
        picker.SetDynamicResource(Picker.TextColorProperty, TextColorResourceKey);
        foreach (var choice in descriptor.Choices)
        {
            picker.Items.Add(Convert.ToString(choice, CultureInfo.InvariantCulture) ?? string.Empty);
        }

        picker.SelectedIndex = FindChoiceIndex(descriptor, descriptor.Value);
        picker.SelectedIndexChanged += (_, _) => ApplySelectedChoice(descriptor, picker);
        return picker;
    }

    /// <summary>Creates a date editor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The editor.</returns>
    private DatePicker CreateDateEditor(PropertyDescriptorModel descriptor)
    {
        var datePicker = new DatePicker { Date = descriptor.Value is DateTime date ? date : DateTime.Today };
        datePicker.SetDynamicResource(DatePicker.TextColorProperty, TextColorResourceKey);
        datePicker.DateSelected += (_, args) => _ = EditProperty(descriptor.Key, args.NewDate);
        return datePicker;
    }

    /// <summary>Creates a command presenter.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>The command presenter.</returns>
    private Button CreateCommandPresenter(PropertyDescriptorModel descriptor)
    {
        var button = CreateActionButton(descriptor.DisplayName);
        button.Command = descriptor.SetValueCommand;
        button.CommandParameter = descriptor;
        button.IsEnabled = descriptor.SetValueCommand?.CanExecute(descriptor) == true;
        return button;
    }

    /// <summary>Applies a selected choice value.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <param name="picker">The picker.</param>
    private void ApplySelectedChoice(PropertyDescriptorModel descriptor, Picker picker)
    {
        if (picker.SelectedIndex < 0 || picker.SelectedIndex >= descriptor.Choices.Count)
        {
            return;
        }

        _ = EditProperty(descriptor.Key, descriptor.Choices[picker.SelectedIndex]);
    }

    /// <summary>Determines whether changes can be committed.</summary>
    /// <returns><c>true</c> when changes can be committed.</returns>
    private bool CanCommitChanges() => CreateCurrentState().CanCommit;

    /// <summary>Refreshes the native commit button command binding and payload.</summary>
    private void RefreshCommandTarget()
    {
        var state = CreateCurrentState();
        var command = CommitChangesCommand ?? UpdatePropertyCommand;
        _commitButton.Command = command;
        _commitButton.CommandParameter = state;
        _commitButton.IsEnabled = state.CanCommit && command?.CanExecute(state) == true;
    }

    /// <summary>Updates command and button availability.</summary>
    private void RaiseCommandAvailability()
    {
        RefreshCommandTarget();
        (CommitCommand as GridCommand)?.RaiseCanExecuteChanged();
    }

    /// <summary>Command wrapper for grid actions.</summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">The command guard.</param>
    private sealed class GridCommand(Func<bool> execute, Func<bool> canExecute) : ICommand
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
