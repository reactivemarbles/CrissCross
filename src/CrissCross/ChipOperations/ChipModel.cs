// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents a compact selectable or removable chip/tag item.</summary>
public sealed class ChipModel
{
    /// <inheritdoc />
    public ChipModel(string key, string text)
        : this(key, text, new ChipModelOptions())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ChipModel"/> class.</summary>
    /// <param name="key">The stable chip key used for selection and reconciliation.</param>
    /// <param name="text">The user-facing chip text.</param>
    /// <param name="options">The chip state, presentation, and command options.</param>
    public ChipModel(string key, string text, ChipModelOptions options)
    {
        ThrowHelper.ThrowIfNull(options, nameof(options));
        Key = key;
        Text = text;
        IsSelected = options.IsSelected;
        IsRemovable = options.IsRemovable;
        IsEnabled = options.IsEnabled;
        Icon = options.Icon;
        RemoveCommand = options.RemoveCommand;
        SelectCommand = options.SelectCommand;
    }

    /// <summary>Gets the stable chip key used for selection and reconciliation.</summary>
    public string Key { get; }

    /// <summary>Gets the user-facing chip text.</summary>
    public string Text { get; }

    /// <summary>Gets a value indicating whether the chip is selected.</summary>
    public bool IsSelected { get; }

    /// <summary>Gets a value indicating whether the chip exposes a remove action.</summary>
    public bool IsRemovable { get; }

    /// <summary>Gets a value indicating whether the chip can be interacted with.</summary>
    public bool IsEnabled { get; }

    /// <summary>Gets optional icon content or icon key used by platform templates.</summary>
    public object? Icon { get; }

    /// <summary>Gets optional command invoked to remove the chip.</summary>
    public System.Windows.Input.ICommand? RemoveCommand { get; }

    /// <summary>Gets optional command invoked to select or toggle the chip.</summary>
    public System.Windows.Input.ICommand? SelectCommand { get; }

    /// <summary>Gets a value indicating whether the chip has icon content.</summary>
    public bool HasIcon => Icon is not null;

    /// <summary>Gets a value indicating whether the chip can be selected or removed.</summary>
    public bool IsInteractive => IsEnabled && (SelectCommand is not null || CanRemove || IsSelected);

    /// <summary>Gets a value indicating whether the chip can be removed by the user.</summary>
    public bool CanRemove => IsEnabled && IsRemovable && RemoveCommand is not null;
}
