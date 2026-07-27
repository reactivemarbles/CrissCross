// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a compact single-selection control for mode switching.</summary>
public class SegmentedControl : global::Avalonia.Controls.ListBox
{
    /// <summary>Property for <see cref="SelectionState"/>.</summary>
    public static readonly StyledProperty<SegmentedSelectionState?> SelectionStateProperty = AvaloniaProperty.Register<
        SegmentedControl,
        SegmentedSelectionState?
    >(nameof(SelectionState));

    /// <summary>Property for <see cref="SelectedKey"/>.</summary>
    public static readonly StyledProperty<string?> SelectedKeyProperty = AvaloniaProperty.Register<
        SegmentedControl,
        string?
    >(nameof(SelectedKey), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Property for <see cref="SelectionChangedCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> SelectionChangedCommandProperty = AvaloniaProperty.Register<
        SegmentedControl,
        ICommand?
    >(nameof(SelectionChangedCommand));

    /// <summary>Tracks state-driven updates so they do not replay the selection command.</summary>
    private bool _isSynchronizingSelection;

    /// <summary>Gets or sets the shared segmented selection state.</summary>
    public SegmentedSelectionState? SelectionState
    {
        get => GetValue(SelectionStateProperty);
        set => SetValue(SelectionStateProperty, value);
    }

    /// <summary>Gets or sets the selected segment key.</summary>
    public string? SelectedKey
    {
        get => GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    /// <summary>Gets or sets the command invoked when a segment is selected.</summary>
    public ICommand? SelectionChangedCommand
    {
        get => GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == SelectionStateProperty)
        {
            if (change.GetNewValue<SegmentedSelectionState?>() is { } state)
            {
                ItemsSource = state.Items;
                SynchronizeSelection(state.SelectedKey);
            }

            return;
        }

        if (change.Property == SelectedKeyProperty)
        {
            SynchronizeSelection(change.GetNewValue<string?>());
            return;
        }

        if (change.Property != SelectedItemProperty
            || _isSynchronizingSelection
            || change.GetNewValue<object?>() is not SegmentItem { IsEnabled: true } item)
        {
            return;
        }

        SetCurrentValue(SelectedKeyProperty, item.Key);
        ExecuteSelectionChanged(item.Key);
    }

    /// <summary>Executes the reactive selection callback for a user-selected key.</summary>
    /// <param name="selectedKey">The selected segment key.</param>
    private void ExecuteSelectionChanged(string selectedKey)
    {
        var command = SelectionChangedCommand;
        if (command?.CanExecute(selectedKey) != true)
        {
            return;
        }

        command.Execute(selectedKey);
    }

    /// <summary>Selects the item matching a stable segment key.</summary>
    /// <param name="selectedKey">The segment key to select.</param>
    private void SynchronizeSelection(string? selectedKey)
    {
        var state = SelectionState;
        if (state is null)
        {
            return;
        }

        SegmentItem? selectedItem = null;
        foreach (var item in state.Items)
        {
            if (string.Equals(item.Key, selectedKey, StringComparison.Ordinal))
            {
                selectedItem = item;
                break;
            }
        }

        _isSynchronizingSelection = true;
        SelectedItem = selectedItem;
        SetCurrentValue(SelectedKeyProperty, selectedItem?.Key);
        _isSynchronizingSelection = false;
    }
}
