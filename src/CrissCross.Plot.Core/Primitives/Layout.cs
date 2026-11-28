// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Contains the figure and data-area rectangles and the measured panel layout.</summary>
public readonly struct Layout : IEquatable<Layout>
{
    /// <summary>Initializes a new instance of the <see cref="Layout"/> struct.</summary>
    /// <param name="figureRect">The rectangle occupied by the figure.</param>
    /// <param name="dataRect">The rectangle occupied by the data area.</param>
    /// <param name="sizes">The measured size of each panel.</param>
    /// <param name="offsets">The offset of each panel from the data area.</param>
    public Layout(PixelRect figureRect, PixelRect dataRect, Dictionary<IPanel, float> sizes, Dictionary<IPanel, float> offsets)
    {
        FigureRect = figureRect;
        DataRect = dataRect;
        PanelSizes = sizes;
        PanelOffsets = offsets;
    }

    /// <summary>Gets size of the figure this layout represents.</summary>
    public PixelRect FigureRect { get; }

    /// <summary>Gets final size of the data area.</summary>
    public PixelRect DataRect { get; }

    /// <summary>Gets distance (pixels) each panel is to be placed from the edge of the data rectangle.</summary>
    public Dictionary<IPanel, float> PanelOffsets { get; }

    /// <summary>Gets size (pixels) of each panel in the dimension perpendicular to the data edge it is placed on.</summary>
    public Dictionary<IPanel, float> PanelSizes { get; }

    /// <summary>Determines whether two layouts have equal figure and data-area rectangles.</summary>
    public static bool operator ==(Layout left, Layout right) => left.Equals(right);

    /// <summary>Determines whether two layouts have different figure or data-area rectangles.</summary>
    public static bool operator !=(Layout left, Layout right) => !left.Equals(right);

    /// <inheritdoc />
    public bool Equals(Layout other) => FigureRect.Equals(other.FigureRect) && DataRect.Equals(other.DataRect);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Layout other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCodeCombiner.Combine(FigureRect, DataRect);
}
