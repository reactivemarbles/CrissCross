// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a compact single-selection control for mode switching.</summary>
public class SegmentedControl : ItemsControl
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

        if (
            change.Property != SelectionStateProperty
            || change.GetNewValue<SegmentedSelectionState?>() is not { } state)
        {
            return;
        }

        SelectedKey = state.SelectedKey;
        ItemsSource = state.Items;
    }
}
