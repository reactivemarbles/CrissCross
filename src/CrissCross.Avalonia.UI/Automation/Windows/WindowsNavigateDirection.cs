// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Defines native UI Automation NavigateDirection values.</summary>
public enum WindowsNavigateDirection
{
    /// <summary>The Parent value.</summary>
    Parent = 0,

    /// <summary>The NextSibling value.</summary>
    NextSibling = 1,

    /// <summary>The PreviousSibling value.</summary>
    PreviousSibling = 2,

    /// <summary>The FirstChild value.</summary>
    FirstChild = 3,

    /// <summary>The LastChild value.</summary>
    LastChild = 4,
}
