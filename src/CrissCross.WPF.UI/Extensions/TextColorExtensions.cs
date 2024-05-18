// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Extension that converts the text color type enumeration to the name of the resource that represents it.
/// </summary>
public static class TextColorExtensions
{
    /// <summary>
    /// Converts the text color type enumeration to the name of the resource that represents it.
    /// </summary>
    /// <param name="textColor">Color of the text.</param>
    /// <returns>
    /// Name of the resource matching the <see cref="TextColor" />. <see cref="ArgumentOutOfRangeException" /> otherwise.
    /// </returns>
    public static string ToResourceValue(this TextColor textColor) => textColor switch
    {
        TextColor.Primary => "TextFillColorPrimaryBrush",
        TextColor.Secondary => "TextFillColorSecondaryBrush",
        TextColor.Tertiary => "TextFillColorTertiaryBrush",
        TextColor.Disabled => "TextFillColorDisabledBrush",
        _ => throw new ArgumentOutOfRangeException(nameof(textColor), textColor, null)
    };
}
