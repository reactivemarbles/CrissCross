// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Extensions;
#else
namespace CrissCross.WPF.UI.Extensions;
#endif

/// <summary>Provides the TextColorExtensions member.</summary>
public static class TextColorExtensions
{
    /// <summary>Provides extension members for text colors.</summary>
    /// <param name="textColor">The textColor value.</param>
    extension(TextColor textColor)
    {
        /// <summary>Converts the text color type enumeration to the name of the resource that represents it.</summary>
        /// <returns>
        /// Name of the resource matching the <see cref="TextColor" />. <see cref="ArgumentOutOfRangeException" />
        /// otherwise.
        /// </returns>
        public string ToResourceValue() =>
            textColor switch
            {
                TextColor.Primary => "TextFillColorPrimaryBrush",
                TextColor.Secondary => "TextFillColorSecondaryBrush",
                TextColor.Tertiary => "TextFillColorTertiaryBrush",
                TextColor.Disabled => "TextFillColorDisabledBrush",
                _ => throw new ArgumentOutOfRangeException(nameof(textColor), textColor, null),
            };
    }
}
