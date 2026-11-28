// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Testing;

/// <summary>Compares two raster images to identify and quantify differences.</summary>
public class ImageDiff
{
    /// <summary>Initializes a new instance of the <see cref="ImageDiff"/> class.</summary>
    /// <param name="img1">The first image.</param>
    /// <param name="img2">The second image.</param>
    /// <param name="saveDiffImage">Whether to save an image visualizing the pixel differences.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional default is retained for source and binary compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "This comparison retains the established rectangular pixel-array API exposed by Image.")]
    public ImageDiff(Image img1, Image img2, bool saveDiffImage = true)
    {
        const int PercentageScale = 100;

        if (img1.Size != img2.Size)
        {
            throw new InvalidOperationException("images must be the same size");
        }

        byte[, ] diffValues = new byte[img1.Height, img2.Width];
        byte[, ] img1values = img1.GetArrayGrayscale();
        byte[, ] img2values = img2.GetArrayGrayscale();

        NumberOfDifferentPixels = 0;
        TotalDifference = 0;
        MaxDifference = 0;
        for (int y = 0; y < img1.Height; y++)
        {
            for (int x = 0; x < img2.Width; x++)
            {
                int diffValue = Math.Abs(img1values[y, x] - img2values[y, x]);
                diffValues[y, x] = (byte)diffValue;
                NumberOfDifferentPixels += diffValue > 0 ? 1 : 0;
                TotalDifference += diffValue;
                if (diffValue > MaxDifference)
                {
                    MaxDifference = diffValue;
                }
            }
        }

        PercentOfDifferencePixels = NumberOfDifferentPixels / (img1.Height * img2.Width) * PercentageScale;

        if (!saveDiffImage)
        {
            return;
        }

        DifferenceImage = new(diffValues);
    }

    /// <summary>Gets the number of pixels with different values.</summary>
    public double NumberOfDifferentPixels { get; }

    /// <summary>Gets the percentage of pixels with different values.</summary>
    public double PercentOfDifferencePixels { get; }

    /// <summary>Gets the sum of all absolute pixel-value differences.</summary>
    public double TotalDifference { get; }

    /// <summary>Gets the largest absolute pixel-value difference.</summary>
    public double MaxDifference { get; }

    /// <summary>Gets the image visualizing pixel differences, if it was requested.</summary>
    public Image? DifferenceImage { get; }
}
