// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Represents a segmented selection surface backed by <see cref="SegmentedSelectionState"/>.</summary>
public class SegmentedControl : HorizontalStackLayout
{
    /// <summary>Bindable property for <see cref="State"/>.</summary>
    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(SegmentedSelectionState),
        typeof(SegmentedControl),
        propertyChanged: static (bindable, _, newValue) => OnStateChanged(bindable, newValue));

    /// <summary>Bindable property for <see cref="SelectedKey"/>.</summary>
    public static readonly BindableProperty SelectedKeyProperty = BindableProperty.Create(
        nameof(SelectedKey),
        typeof(string),
        typeof(SegmentedControl),
        defaultBindingMode: BindingMode.TwoWay);

    /// <summary>Bindable property for <see cref="SelectionCommand"/>.</summary>
    public static readonly BindableProperty SelectionCommandProperty = BindableProperty.Create(
        nameof(SelectionCommand),
        typeof(ICommand),
        typeof(SegmentedControl));

    /// <summary>Gets or sets the segmented selection state.</summary>
    public SegmentedSelectionState? State
    {
        get => (SegmentedSelectionState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Gets or sets the selected segment key.</summary>
    public string? SelectedKey
    {
        get => (string?)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    /// <summary>Gets or sets a command invoked when a segment is selected.</summary>
    public ICommand? SelectionCommand
    {
        get => (ICommand?)GetValue(SelectionCommandProperty);
        set => SetValue(SelectionCommandProperty, value);
    }

    /// <summary>Selects a segment by key and invokes <see cref="SelectionCommand"/> when permitted.</summary>
    /// <param name="key">The segment key.</param>
    /// <returns><c>true</c> when the segment was selected; otherwise, <c>false</c>.</returns>
    public bool SelectSegment(string key)
    {
        var segment = State?.GetItem(key);
        if (segment?.IsEnabled != true)
        {
            return false;
        }

        SelectedKey = segment.Key;
        if (SelectionCommand?.CanExecute(segment.Key) != true)
        {
            return true;
        }

        SelectionCommand.Execute(segment.Key);
        return true;
    }

    /// <summary>Runs the state changed operation.</summary>
    /// <param name="bindable">The bindable object.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnStateChanged(BindableObject bindable, object newValue)
    {
        if (bindable is not SegmentedControl control)
        {
            return;
        }

        control.ApplyState(newValue as SegmentedSelectionState);
    }

    /// <summary>Applies the supplied segmented selection state to the visual children.</summary>
    /// <param name="state">The segmented selection state.</param>
    private void ApplyState(SegmentedSelectionState? state)
    {
        Children.Clear();
        SelectedKey = state?.SelectedKey;

        if (state is null)
        {
            return;
        }

        foreach (var item in state.Items)
        {
            var button = new Button
            {
                Text = item.Text,
                IsEnabled = item.IsEnabled,
                Command = new Command(() => SelectSegment(item.Key))
            };
            Children.Add(button);
        }
    }
}
