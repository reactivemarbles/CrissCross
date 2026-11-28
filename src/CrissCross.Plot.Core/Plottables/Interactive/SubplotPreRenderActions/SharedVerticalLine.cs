// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive.SubplotPreRenderActions;

/// <summary>Links multiple vertical lines so they move together.</summary>
public class SharedVerticalLine : IMultiplotPreRenderAction
{
    /// <summary>The vertical lines whose positions are synchronized.</summary>
    private readonly List<InteractiveVerticalLine> _lines;

    /// <summary>The last synchronized positions of the lines.</summary>
    private double[] _positions = [];

    /// <summary>Initializes a new instance of the <see cref="SharedVerticalLine"/> class.</summary>
    /// <param name="lines">The lines to keep synchronized.</param>
    public SharedVerticalLine(params InteractiveVerticalLine[] lines)
    {
        _lines = new(lines);
        UpdateCurrentPositions();
    }

    /// <summary>Synchronizes all lines to the most recently moved line.</summary>
    public void Invoke()
    {
        int wanted = -1;
        for (int i = 0; i < _lines.Count; i++)
        {
            if (Math.Abs(_positions[i] - _lines[i].X) > Constants.PositionTolerance)
            {
                wanted = i;
            }
        }

        if (wanted == -1)
        {
            return;
        }

        double newX = _lines[wanted].X;
        for (int i = 0; i < _lines.Count; i++)
        {
            if (i != wanted)
            {
                _lines[i].X = newX;
            }
        }

        UpdateCurrentPositions();
    }

    /// <summary>Updates the recorded positions from the current line coordinates.</summary>
    private void UpdateCurrentPositions()
    {
        _positions = new double[_lines.Count];
        for (int i = 0; i < _lines.Count; i++)
        {
            _positions[i] = _lines[i].X;
        }
    }

    /// <summary>Contains the tolerance used to detect line movement.</summary>
    private static class Constants
    {
        /// <summary>Changes smaller than this amount are treated as unchanged.</summary>
        public const double PositionTolerance = 1e-10;
    }
}
