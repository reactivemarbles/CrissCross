// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides extension operations for <see cref="Alignment"/> values.</summary>
public static class AlignmentExtensions
{
    /// <summary>Maps a row and column index to the corresponding alignment value.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1499",
        Justification = "The public mutable matrix is retained to preserve the existing API and its caller-visible behavior.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "PSH1020",
        Justification = "Changing the public multidimensional matrix to a jagged array would break API compatibility.")]
    public static readonly Alignment[ , ] AlignmentMatrix =
    {
        { Alignment.UpperLeft, Alignment.UpperCenter, Alignment.UpperRight },
        { Alignment.MiddleLeft, Alignment.MiddleCenter, Alignment.MiddleRight },
        { Alignment.LowerLeft, Alignment.LowerCenter, Alignment.LowerRight },
    };

    /// <summary>The fraction used for centered alignment.</summary>
    private const float CenterFraction = 0.5F;

    /// <summary>The error message used for unsupported alignment values.</summary>
    private const string UnsupportedAlignmentMessage = "Unsupported alignment.";

    /// <summary>Provides operations for an alignment value.</summary>
    /// <param name="alignment">The alignment value.</param>
    extension(Alignment alignment)
    {
        /// <summary>Gets whether the alignment is on the upper edge.</summary>
        /// <returns><see langword="true"/> when the alignment is upper-aligned.</returns>
        public bool IsUpperEdge() => alignment is Alignment.UpperLeft or Alignment.UpperCenter or Alignment.UpperRight;

        /// <summary>Gets whether the alignment is on the lower edge.</summary>
        /// <returns><see langword="true"/> when the alignment is lower-aligned.</returns>
        public bool IsLowerEdge() => alignment is Alignment.LowerLeft or Alignment.LowerCenter or Alignment.LowerRight;

        /// <summary>Gets whether the alignment is on the left edge.</summary>
        /// <returns><see langword="true"/> when the alignment is left-aligned.</returns>
        public bool IsLeftEdge() => alignment is Alignment.UpperLeft or Alignment.MiddleLeft or Alignment.LowerLeft;

        /// <summary>Gets whether the alignment is on the right edge.</summary>
        /// <returns><see langword="true"/> when the alignment is right-aligned.</returns>
        public bool IsRightEdge() => alignment is Alignment.UpperRight or Alignment.MiddleRight or Alignment.LowerRight;

        /// <summary>Gets the horizontal fraction represented by the alignment.</summary>
        /// <returns>Zero for the left edge, one half for the center, or one for the right edge.</returns>
        public float HorizontalFraction() => alignment switch
        {
            Alignment.UpperLeft or Alignment.MiddleLeft or Alignment.LowerLeft => 0,
            Alignment.UpperCenter or Alignment.MiddleCenter or Alignment.LowerCenter => CenterFraction,
            Alignment.UpperRight or Alignment.MiddleRight or Alignment.LowerRight => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, UnsupportedAlignmentMessage),
        };

        /// <summary>Gets the vertical fraction represented by the alignment.</summary>
        /// <returns>One for the upper edge, one half for the center, or zero for the lower edge.</returns>
        public float VerticalFraction() => alignment switch
        {
            Alignment.UpperLeft or Alignment.UpperCenter or Alignment.UpperRight => 1,
            Alignment.MiddleLeft or Alignment.MiddleCenter or Alignment.MiddleRight => CenterFraction,
            Alignment.LowerLeft or Alignment.LowerCenter or Alignment.LowerRight => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, UnsupportedAlignmentMessage),
        };

        /// <summary>Converts the alignment to its Skia text alignment.</summary>
        /// <returns>The matching Skia text alignment.</returns>
        public SKTextAlign ToSKTextAlign() => alignment switch
        {
            Alignment.UpperLeft or Alignment.MiddleLeft or Alignment.LowerLeft => SKTextAlign.Left,
            Alignment.UpperCenter or Alignment.MiddleCenter or Alignment.LowerCenter => SKTextAlign.Center,
            Alignment.UpperRight or Alignment.MiddleRight or Alignment.LowerRight => SKTextAlign.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, UnsupportedAlignmentMessage),
        };
    }
}
