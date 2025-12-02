// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Provides data for the <see cref="AutoSuggestBox.TextChanged"/> event.
/// </summary>
public enum AutoSuggestionBoxTextChangeReason
{
    /// <summary>
    /// The user edited the text.
    /// </summary>
    UserInput = 0,

    /// <summary>
    /// The text was changed via code.
    /// </summary>
    ProgrammaticChange = 1,

    /// <summary>
    /// The user selected one of the items in the auto-suggestion box.
    /// </summary>
    SuggestionChosen = 2,
}
