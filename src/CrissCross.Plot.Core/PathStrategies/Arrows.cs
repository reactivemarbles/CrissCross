// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PathStrategies;

/// <summary>Creates paths representing arrow vectors.</summary>
public static class Arrows // TODO: use an interface to let users inject custom logic
{
    /// <summary>Creates a path containing arrows for the specified vectors.</summary>
    /// <param name="vectors">The vectors to render as arrows.</param>
    /// <param name="style">The anchor and line style used for the arrows.</param>
    /// <param name="maxBladeWidth">The maximum width of each arrowhead.</param>
    /// <returns>A path containing the arrow geometry.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The optional blade-width default is part of the existing public API.")]
    public static SKPath GetPath(IEnumerable<RootedPixelVector> vectors, ArrowStyle style, float maxBladeWidth = float.PositiveInfinity)
    {
        const float centerAnchorOffset = 0.5F;
        const float bladeLengthFactor = 0.35F; // How long the arrowhead is as a proportion of arrow length
        const float bladeWidthFactor = 0.2F; // How wide the arrowhead is as a proportion of arrow length
        const float cutInFactor = 0.15F; // How much the base of the arrowhead is cut away as a proportion of arrow length (0 for perfect triangle)

        using SKPathBuilder path = new();

        foreach (RootedPixelVector vector in vectors)
        {
            SKPoint start = style.Anchor switch
            {
                ArrowAnchor.Center => new(vector.Point.X - (centerAnchorOffset * vector.Vector.X), vector.Point.Y - (centerAnchorOffset * vector.Vector.Y)),
                ArrowAnchor.Tail => vector.Point.ToSKPoint(),
                ArrowAnchor.Tip => new(vector.Point.X - vector.Vector.X, vector.Point.Y - vector.Vector.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(style), "Unexpected arrow anchor value"),
            };

            path.MoveTo(start);
            path.RLineTo(vector.Vector.X, vector.Vector.Y);
            SKPoint head = start + new SKPoint(vector.Vector.X, vector.Vector.Y);

            var junction = head + new SKPoint(-bladeLengthFactor * vector.Vector.X, -bladeLengthFactor * vector.Vector.Y);

            var perp = new SKPoint(-vector.Vector.Y, vector.Vector.X);
            var bladeTip1 = junction + new SKPoint(Math.Min(bladeWidthFactor * perp.X, maxBladeWidth), Math.Min(bladeWidthFactor * perp.Y, maxBladeWidth));
            var bladeTip2 = junction - new SKPoint(Math.Min(bladeWidthFactor * perp.X, maxBladeWidth), Math.Min(bladeWidthFactor * perp.Y, maxBladeWidth));

            var arrowHeadBegin = junction + new SKPoint(cutInFactor * vector.Vector.X, cutInFactor * vector.Vector.Y);

            path.AddPoly([head, bladeTip1, arrowHeadBegin, bladeTip2]);
        }

        return path.Detach();
    }
}
