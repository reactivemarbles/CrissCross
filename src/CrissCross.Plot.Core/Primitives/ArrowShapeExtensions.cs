// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides operations for <see cref="ArrowShape"/> values.</summary>
public static class ArrowShapeExtensions
{
    /// <summary>Gets the implementation associated with this arrow shape.</summary>
    /// <param name="shape">The arrow shape.</param>
    /// <returns>The arrow shape implementation.</returns>
    extension(ArrowShape shape)
    {
        /// <summary>Gets the implementation associated with this arrow shape.</summary>
        /// <returns>The arrow shape implementation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Retained as an established public extension method for API compatibility.")]
        public IArrowShape GetShape() => shape switch
        {
            ArrowShape.Single => new ArrowShapes.Single(),
            ArrowShape.SingleLine => new ArrowShapes.SingleLine(),
            ArrowShape.Double => new ArrowShapes.Double(),
            ArrowShape.DoubleLine => new ArrowShapes.DoubleLine(),
            ArrowShape.Arrowhead => new ArrowShapes.Arrowhead(),
            ArrowShape.ArrowheadLine => new ArrowShapes.ArrowheadLine(),
            ArrowShape.Pentagon => new ArrowShapes.Pentagon(),
            ArrowShape.Chevron => new ArrowShapes.Chevron(),
            _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, "Unsupported arrow shape."),
        };
    }
}
