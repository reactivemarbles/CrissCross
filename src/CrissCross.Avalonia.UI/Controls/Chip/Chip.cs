// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents a compact selectable or removable chip/tag surface.</summary>
public class Chip : TemplatedControl
{
    /// <summary>Property for <see cref="Model"/>.</summary>
    public static readonly StyledProperty<ChipModel?> ModelProperty = AvaloniaProperty.Register<Chip, ChipModel?>(
        nameof(Model));

    /// <summary>Property for <see cref="Text"/>.</summary>
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<Chip, string?>(
        nameof(Text),
        string.Empty);

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<Chip, object?>(
        nameof(Icon));

    /// <summary>Property for <see cref="IsSelected"/>.</summary>
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Chip, bool>(
        nameof(IsSelected));

    /// <summary>Property for <see cref="IsRemovable"/>.</summary>
    public static readonly StyledProperty<bool> IsRemovableProperty = AvaloniaProperty.Register<Chip, bool>(
        nameof(IsRemovable));

    /// <summary>Property for <see cref="SelectCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> SelectCommandProperty = AvaloniaProperty.Register<Chip, ICommand?>(
        nameof(SelectCommand));

    /// <summary>Property for <see cref="RemoveCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> RemoveCommandProperty = AvaloniaProperty.Register<Chip, ICommand?>(
        nameof(RemoveCommand));

    /// <summary>Gets or sets the shared chip model.</summary>
    public ChipModel? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>Gets or sets the chip text.</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the chip icon content.</summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the chip is selected.</summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the chip is removable.</summary>
    public bool IsRemovable
    {
        get => GetValue(IsRemovableProperty);
        set => SetValue(IsRemovableProperty, value);
    }

    /// <summary>Gets or sets the command invoked to select or toggle the chip.</summary>
    public ICommand? SelectCommand
    {
        get => GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked to remove the chip.</summary>
    public ICommand? RemoveCommand
    {
        get => GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property != ModelProperty || change.GetNewValue<ChipModel?>() is not { } model)
        {
            return;
        }

        Text = model.Text;
        Icon = model.Icon;
        IsSelected = model.IsSelected;
        IsEnabled = model.IsEnabled;
        IsRemovable = model.IsRemovable;
        SelectCommand = model.SelectCommand;
        RemoveCommand = model.RemoveCommand;
    }
}
