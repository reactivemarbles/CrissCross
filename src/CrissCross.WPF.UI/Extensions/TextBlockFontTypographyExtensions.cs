// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Extensions;
#else
namespace CrissCross.WPF.UI.Extensions;
#endif

/// <summary>Provides the TextBlockFontTypographyExtensions member.</summary>
public static class TextBlockFontTypographyExtensions
{
    /// <summary>Provides extension members for font typography values.</summary>
    /// <param name="typography">The typography value.</param>
    extension(FontTypography typography)
    {
        /// <summary>Converts the typography type enumeration to the name of the resource that represents it.</summary>
        /// <returns>
        /// Name of the resource matching the <see cref="FontTypography" />. <see cref="ArgumentOutOfRangeException" />
        /// otherwise.
        /// </returns>
        public string ToResourceValue() =>
            typography switch
            {
                FontTypography.Caption => "CaptionTextBlockStyle",
                FontTypography.Body => "BodyTextBlockStyle",
                FontTypography.BodyStrong => "BodyStrongTextBlockStyle",
                FontTypography.Subtitle => "SubtitleTextBlockStyle",
                FontTypography.Title => "TitleTextBlockStyle",
                FontTypography.TitleLarge => "TitleLargeTextBlockStyle",
                FontTypography.Display => "DisplayTextBlockStyle",
                _ => throw new ArgumentOutOfRangeException(nameof(typography), typography, null),
            };
    }
}
