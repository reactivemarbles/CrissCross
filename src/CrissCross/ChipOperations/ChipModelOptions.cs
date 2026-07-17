// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross;

/// <summary>Configures the state, presentation, and commands for a <see cref="ChipModel"/>.</summary>
public sealed class ChipModelOptions
{
    /// <summary>Gets or sets a value indicating whether the chip is selected.</summary>
    public bool IsSelected { get; set; }

    /// <summary>Gets or sets a value indicating whether the chip exposes a remove action.</summary>
    public bool IsRemovable { get; set; }

    /// <summary>Gets or sets a value indicating whether the chip can be interacted with.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Gets or sets optional icon content or an icon key used by platform templates.</summary>
    public object? Icon { get; set; }

    /// <summary>Gets or sets an optional command invoked to remove the chip.</summary>
    public ICommand? RemoveCommand { get; set; }

    /// <summary>Gets or sets an optional command invoked to select or toggle the chip.</summary>
    public ICommand? SelectCommand { get; set; }
}
