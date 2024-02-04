// <auto-generated>
//   This file has been borrowed from Wpf-UI.
// </auto-generated>

#nullable enable

// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Extension that converts the text color type enumeration to the name of the resource that represents it.
/// </summary>
public static class TextColorExtensions
{
    /// <summary>
    /// Converts the text color type enumeration to the name of the resource that represents it.
    /// </summary>
    /// <returns>Name of the resource matching the <see cref="TextColor"/>. <see cref="ArgumentOutOfRangeException"/> otherwise.</returns>
    public static string ToResourceValue(this TextColor textColor)
    {
        return textColor switch
        {
            TextColor.Primary => "TextFillColorPrimaryBrush",
            TextColor.Secondary => "TextFillColorSecondaryBrush",
            TextColor.Tertiary => "TextFillColorTertiaryBrush",
            TextColor.Disabled => "TextFillColorDisabledBrush",
            _ => throw new ArgumentOutOfRangeException(nameof(textColor), textColor, null)
        };
    }
}
