// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Wraps a Skia canvas and tracks its saved state across complex render systems.</summary>
/// <param name="canvas">The canvas to wrap.</param>
public class CanvasState(SKCanvas canvas)
{
    /// <summary>The wrapped Skia canvas.</summary>
    private readonly SKCanvas _canvas = canvas;

    /// <summary>Gets or sets number of times <see cref="Save"/> was called without <see cref="Restore"/></summary>
    public int SaveLevels { get; set; }

    /// <summary>Save the current state of the canvas. This state can be recalled by calling <see cref="Restore"/>.</summary>
    public void Save()
    {
        _ = _canvas.Save();
        SaveLevels++;
    }

    /// <summary>
    /// Restore the canvas to the state the last time <see cref="Save"/> was called.
    /// This method will throw if a canvas is restored more times than it was saved.
    /// Use the <see cref="RestoreAll"/> method to restore a canvas to its
    /// original state if the number of saves is unknown.
    /// </summary>
    public void Restore()
    {
        if (SaveLevels == 0)
        {
            throw new InvalidOperationException("Restore() was called more than Save()");
        }

        _canvas.Restore();
        SaveLevels--;
    }

    /// <summary>Restore the canvas to its original state, regardless of how many times <see cref="Save"/> was called.</summary>
    public void RestoreAll()
    {
        while (SaveLevels > 0)
        {
            Restore();
        }
    }

    /// <summary>Restore the canvas to its original state. Disables all clipping and transformations.</summary>
    public void DisableClipping() => RestoreAll();

    /// <summary>Clips the canvas so drawing occurs only within the given rectangle.</summary>
    /// <param name="rect">The clipping rectangle.</param>
    public void Clip(PixelRect rect) => _canvas.ClipRect(rect.ToSKRect());

    /// <summary>Translates the canvas by the specified horizontal and vertical offsets.</summary>
    /// <param name="dX">The horizontal offset.</param>
    /// <param name="dY">The vertical offset.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1305",
        Justification = "Parameter names are retained for named-argument compatibility.")]
    public void Translate(float dX, float dY) => _canvas.Translate(dX, dY);

    /// <summary>Translates the canvas by a pixel offset.</summary>
    /// <param name="delta">The pixel offset.</param>
    public void Translate(Pixel delta) => _canvas.Translate(delta.X, delta.Y);

    /// <summary>Rotates the canvas around the origin by the specified angle in degrees.</summary>
    /// <param name="degrees">The rotation angle in degrees.</param>
    public void RotateDegrees(double degrees) => _canvas.RotateDegrees((float)degrees, 0, 0);

    /// <summary>Rotates the canvas around a pixel position by the specified angle in degrees.</summary>
    /// <param name="degrees">The rotation angle in degrees.</param>
    /// <param name="px">The rotation origin.</param>
    public void RotateDegrees(double degrees, Pixel px) => _canvas.RotateDegrees((float)degrees, px.X, px.Y);

    /// <summary>Rotates the canvas around the origin by the specified angle in radians.</summary>
    /// <param name="radians">The rotation angle in radians.</param>
    public void RotateRadians(double radians) => _canvas.RotateRadians((float)radians, 0, 0);

    /// <summary>Rotates the canvas around a pixel position by the specified angle in radians.</summary>
    /// <param name="radians">The rotation angle in radians.</param>
    /// <param name="px">The rotation origin.</param>
    public void RotateRadians(double radians, Pixel px) => _canvas.RotateRadians((float)radians, px.X, px.Y);
}
