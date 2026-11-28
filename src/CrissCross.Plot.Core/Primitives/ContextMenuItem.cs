// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a single item in a right-click pop-up menu.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "PerformanceSharp",
    "PSH1005",
    Justification = "This public mutable struct remains mutable for API compatibility; callers must not use it as a hash key while mutating its properties.")]
public struct ContextMenuItem
{
    /// <summary>Gets or sets whether this item is a separator.</summary>
    public bool IsSeparator { get; set; }

    /// <summary>Gets or sets the menu item's label.</summary>
    public string Label { get; set; }

    /// <summary>Gets or sets the action invoked when the item is selected.</summary>
    public Action<Plot> OnInvoke { get; set; }
}
