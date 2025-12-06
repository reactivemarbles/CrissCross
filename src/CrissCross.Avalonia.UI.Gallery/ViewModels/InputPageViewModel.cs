// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>
/// ViewModel for the input controls page.
/// </summary>
public class InputPageViewModel : RxObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputPageViewModel"/> class.
    /// </summary>
    public InputPageViewModel() =>
        this.BuildComplete(() => DisplayName = "Input Controls");

    /// <summary>
    /// Gets the sample suggestions for the AutoSuggestBox.
    /// </summary>
    public IEnumerable<string> SampleSuggestions { get; } =
    [
        "Apple",
        "Apricot",
        "Avocado",
        "Banana",
        "Blackberry",
        "Blueberry",
        "Cherry",
        "Coconut",
        "Cranberry",
        "Date",
        "Dragonfruit",
        "Fig",
        "Grape",
        "Grapefruit",
        "Guava",
        "Kiwi",
        "Lemon",
        "Lime",
        "Mango",
        "Melon",
        "Orange",
        "Papaya",
        "Peach",
        "Pear",
        "Pineapple",
        "Plum",
        "Pomegranate",
        "Raspberry",
        "Strawberry",
        "Watermelon"
    ];
}
