// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Specifies the reason for a text change event in an auto-suggestion box.
/// </summary>
/// <remarks>Use this enumeration to determine whether a text change was triggered by user input, programmatic
/// modification, or by the user selecting a suggestion. This information can be used to adjust application behavior,
/// such as filtering suggestions or handling selection events appropriately.</remarks>
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
