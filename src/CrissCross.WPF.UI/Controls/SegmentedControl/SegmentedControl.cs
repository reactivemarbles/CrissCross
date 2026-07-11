// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents a compact single-selection control for mode switching.</summary>
public class SegmentedControl : ItemsControl
{
    /// <summary>Property for <see cref="SelectionState"/>.</summary>
    public static readonly DependencyProperty SelectionStateProperty = DependencyProperty.Register(
        nameof(SelectionState),
        typeof(SegmentedSelectionState),
        typeof(SegmentedControl),
        new PropertyMetadata(null, OnSelectionStateChanged));

    /// <summary>Property for <see cref="SelectedKey"/>.</summary>
    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(SegmentedControl),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>Property for <see cref="SelectionChangedCommand"/>.</summary>
    public static readonly DependencyProperty SelectionChangedCommandProperty = DependencyProperty.Register(
        nameof(SelectionChangedCommand),
        typeof(ICommand),
        typeof(SegmentedControl),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the shared segmented selection state.</summary>
    public SegmentedSelectionState? SelectionState
    {
        get => (SegmentedSelectionState?)GetValue(SelectionStateProperty);
        set => SetValue(SelectionStateProperty, value);
    }

    /// <summary>Gets or sets the selected segment key.</summary>
    public string? SelectedKey
    {
        get => (string?)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    /// <summary>Gets or sets the command invoked when a segment is selected.</summary>
    public ICommand? SelectionChangedCommand
    {
        get => (ICommand?)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    /// <summary>Provides the OnSelectionStateChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnSelectionStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not SegmentedControl segmentedControl || args.NewValue is not SegmentedSelectionState state)
        {
            return;
        }

        segmentedControl.SetCurrentValue(SelectedKeyProperty, state.SelectedKey);
        segmentedControl.SetCurrentValue(ItemsSourceProperty, state.Items);
    }
}
