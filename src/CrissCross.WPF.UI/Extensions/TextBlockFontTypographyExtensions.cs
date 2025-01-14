// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Extensions;

/// <summary>
/// Extension that converts the typography type enumeration to the name of the resource that represents it.
/// </summary>
public static class TextBlockFontTypographyExtensions
{
    /// <summary>
    /// Converts the typography type enumeration to the name of the resource that represents it.
    /// </summary>
    /// <param name="typography">The typography.</param>
    /// <returns>
    /// Name of the resource matching the <see cref="FontTypography" />. <see cref="ArgumentOutOfRangeException" /> otherwise.
    /// </returns>
    public static string ToResourceValue(this FontTypography typography) => typography switch
    {
        FontTypography.Caption => "CaptionTextBlockStyle",
        FontTypography.Body => "BodyTextBlockStyle",
        FontTypography.BodyStrong => "BodyStrongTextBlockStyle",
        FontTypography.Subtitle => "SubtitleTextBlockStyle",
        FontTypography.Title => "TitleTextBlockStyle",
        FontTypography.TitleLarge => "TitleLargeTextBlockStyle",
        FontTypography.Display => "DisplayTextBlockStyle",
        _ => throw new ArgumentOutOfRangeException(nameof(typography), typography, null)
    };
}
