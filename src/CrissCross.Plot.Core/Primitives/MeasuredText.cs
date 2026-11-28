// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Contains measured dimensions and layout data for multiline text.</summary>
public readonly struct MeasuredText : IEquatable<MeasuredText>
{
    /// <summary>
    /// Gets size of the entire multiline label in pixels.
    /// Width is the largest value returned by paint.MeasureText().
    /// Height is the line height multiplied by the number of lines.
    /// </summary>
    public PixelSize Size { get; init; }

    /// <summary>Gets vertical spacing between each line. This is the value returned by GetFontMetrics().</summary>
    public float LineHeight { get; init; }

    /// <summary>Gets width of each line of text in pixel units.</summary>
    public float[] LineWidths { get; init; }

    /// <summary>Gets recommended vertical offset when calling SKCanvas.DrawText().</summary>
    public required float VerticalOffset { get; init; }

    /// <summary>Gets distance below the baseline the rendered font may occupy.</summary>
    public required float Bottom { get; init; }

    /// <summary>Gets width of the entire text. Equals the length of the widest line.</summary>
    public float Width => Size.Width;

    /// <summary>Gets height of the entire text.</summary>
    public float Height => Size.Height;

    /// <summary>Determines whether two measured-text values are equal.</summary>
    public static bool operator ==(MeasuredText left, MeasuredText right) => left.Equals(right);

    /// <summary>Determines whether two measured-text values are not equal.</summary>
    public static bool operator !=(MeasuredText left, MeasuredText right) => !left.Equals(right);

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1473",
        Justification = "Value equality must compare exact measurements to remain transitive and consistent with GetHashCode; a tolerance would violate the equality contract.")]
    public bool Equals(MeasuredText other) =>
        Size.Equals(other.Size)
        && LineHeight.Equals(other.LineHeight)
        && object.Equals(LineWidths, other.LineWidths)
        && VerticalOffset.Equals(other.VerticalOffset)
        && Bottom.Equals(other.Bottom);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is MeasuredText other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() =>
        Size.GetHashCode()
        ^ LineHeight.GetHashCode()
        ^ (LineWidths?.GetHashCode() ?? 0)
        ^ VerticalOffset.GetHashCode()
        ^ Bottom.GetHashCode();

    /// <summary>Returns the bounding rectangle of the text relative to its alignment point.</summary>
    /// <param name="alignment">The alignment of the text.</param>
    /// <returns>The bounding rectangle centered on the alignment point.</returns>
    public PixelRect Rect(Alignment alignment)
    {
        float horizontalOffset = Width * alignment.HorizontalFraction();
        float verticalOffset = Height * alignment.VerticalFraction();
        return new PixelRect(Size).WithDelta(-horizontalOffset, verticalOffset - Height);
    }
}
