// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Defines native UI Automation TextUnit values.</summary>
public enum WindowsTextUnit
{
    /// <summary>The Character value.</summary>
    Character = 0,

    /// <summary>The Format value.</summary>
    Format = 1,

    /// <summary>The Word value.</summary>
    Word = 2,

    /// <summary>The Line value.</summary>
    Line = 3,

    /// <summary>The Paragraph value.</summary>
    Paragraph = 4,

    /// <summary>The Page value.</summary>
    Page = 5,

    /// <summary>The Document value.</summary>
    Document = 6,
}
