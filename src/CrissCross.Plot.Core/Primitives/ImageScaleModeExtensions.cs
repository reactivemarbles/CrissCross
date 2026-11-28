// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides sizing operations for image positions.</summary>
public static class ImageScaleModeExtensions
{
    /// <summary>Gets the divisor used to calculate half of a dimension.</summary>
    private const float HalfDivisor = 2;

    /// <summary>Extension members for <see cref="ImagePosition"/>.</summary>
    /// <param name="position">The image position to extend.</param>
    extension(ImagePosition position)
    {
        /// <summary>Gets the image rectangle to display inside a target rectangle.</summary>
        /// <param name="imageSize">The size of the image.</param>
        /// <param name="targetRect">The rectangle in which to display the image.</param>
        /// <returns>The rectangle occupied by the image.</returns>
        public PixelRect GetRect(PixelSize imageSize, PixelRect targetRect)
        {
            float imgWidth = imageSize.Width;
            float imgHeight = imageSize.Height;

            switch (position)
            {
                case ImagePosition.TopLeft:
                {
                    float left = targetRect.Left;
                    float top = targetRect.Top;
                    float right = left + imgWidth;
                    float bottom = top + imgHeight;
                    return new(left, right, bottom, top);
                }

                case ImagePosition.Center:
                {
                    float left = targetRect.HorizontalCenter - (imgWidth / HalfDivisor);
                    float top = targetRect.VerticalCenter - (imgHeight / HalfDivisor);
                    float right = left + imgWidth;
                    float bottom = top + imgHeight;
                    return new(left, right, bottom, top);
                }

                case ImagePosition.Stretch:
                    return targetRect;

                case ImagePosition.Fill:
                {
                    float ratioX = targetRect.Width / imgWidth;
                    float ratioY = targetRect.Height / imgHeight;
                    float minRatio = Math.Min(ratioX, ratioY);
                    float width = minRatio * imgWidth;
                    float height = minRatio * imgHeight;
                    float left = targetRect.HorizontalCenter - (width / HalfDivisor);
                    float right = targetRect.HorizontalCenter + (width / HalfDivisor);
                    float bottom = targetRect.VerticalCenter + (height / HalfDivisor);
                    float top = targetRect.VerticalCenter - (height / HalfDivisor);
                    return new(left, right, bottom, top);
                }

                case ImagePosition.Zoom:
                {
                    float ratioX = targetRect.Width / imgWidth;
                    float ratioY = targetRect.Height / imgHeight;
                    float maxRatio = Math.Max(ratioX, ratioY);
                    float width = maxRatio * imgWidth;
                    float height = maxRatio * imgHeight;
                    float left = targetRect.HorizontalCenter - (width / HalfDivisor);
                    float right = targetRect.HorizontalCenter + (width / HalfDivisor);
                    float bottom = targetRect.VerticalCenter + (height / HalfDivisor);
                    float top = targetRect.VerticalCenter - (height / HalfDivisor);
                    return new(left, right, bottom, top);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, "The image position is not supported.");
            }
        }
    }
}
