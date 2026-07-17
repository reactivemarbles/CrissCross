// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides data for the <see cref="AutoSuggestBox.TextChanged"/> event.</summary>
public enum AutoSuggestionBoxTextChangeReason
{
    /// <summary>The user edited the text.</summary>
    UserInput = 0,

    /// <summary>The text was changed via code.</summary>
    ProgrammaticChange = 1,

    /// <summary>The user selected one of the items in the auto-suggestion box.</summary>
    SuggestionChosen = 2,
}
