// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Defines native UI Automation SupportedTextSelection values.</summary>
public enum WindowsSupportedTextSelection
{
    /// <summary>The None value.</summary>
    None = 0,

    /// <summary>The Single value.</summary>
    Single = 1,

    /// <summary>The Multiple value.</summary>
    Multiple = 2,
}
