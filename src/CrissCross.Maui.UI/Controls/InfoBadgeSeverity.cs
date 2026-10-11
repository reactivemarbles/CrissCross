// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace CrissCross.Reactive.Maui.UI.Controls;
#else
namespace CrissCross.Maui.UI.Controls;
#endif

/// <summary>Defines the semantic importance represented by an <see cref="InfoBadge"/>.</summary>
public enum InfoBadgeSeverity
{
    /// <summary>Displays neutral information.</summary>
    Informational,

    /// <summary>Displays a successful state.</summary>
    Success,

    /// <summary>Displays a caution state.</summary>
    Warning,

    /// <summary>Displays an error state.</summary>
    Error,
}
