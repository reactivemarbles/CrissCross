// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using CrissCross;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a compact selectable or removable chip/tag surface.
/// </summary>
public class Chip : Control
{
    /// <summary>
    /// Property for <see cref="Model"/>.
    /// </summary>
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model),
        typeof(ChipModel),
        typeof(Chip),
        new PropertyMetadata(null, OnModelChanged));

    /// <summary>
    /// Property for <see cref="Text"/>.
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(Chip),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Property for <see cref="Icon"/>.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(object),
        typeof(Chip),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="IsSelected"/>.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(Chip),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="IsRemovable"/>.
    /// </summary>
    public static readonly DependencyProperty IsRemovableProperty = DependencyProperty.Register(
        nameof(IsRemovable),
        typeof(bool),
        typeof(Chip),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="SelectCommand"/>.
    /// </summary>
    public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(Chip),
        new PropertyMetadata(null));

    /// <summary>
    /// Property for <see cref="RemoveCommand"/>.
    /// </summary>
    public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
        nameof(RemoveCommand),
        typeof(ICommand),
        typeof(Chip),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the shared chip model.
    /// </summary>
    public ChipModel? Model
    {
        get => (ChipModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the chip text.
    /// </summary>
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the chip icon content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chip is selected.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chip is removable.
    /// </summary>
    public bool IsRemovable
    {
        get => (bool)GetValue(IsRemovableProperty);
        set => SetValue(IsRemovableProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked to select or toggle the chip.
    /// </summary>
    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked to remove the chip.
    /// </summary>
    public ICommand? RemoveCommand
    {
        get => (ICommand?)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    private static void OnModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is Chip chip && args.NewValue is ChipModel model)
        {
            chip.SetCurrentValue(TextProperty, model.Text);
            chip.SetCurrentValue(IconProperty, model.Icon);
            chip.SetCurrentValue(IsSelectedProperty, model.IsSelected);
            chip.SetCurrentValue(IsEnabledProperty, model.IsEnabled);
            chip.SetCurrentValue(IsRemovableProperty, model.IsRemovable);
            chip.SetCurrentValue(SelectCommandProperty, model.SelectCommand);
            chip.SetCurrentValue(RemoveCommandProperty, model.RemoveCommand);
        }
    }
}
