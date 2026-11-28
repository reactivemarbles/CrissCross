// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines the appearance and rendering behavior of axis tick marks.</summary>
public class TickMarkStyle
{
    /// <summary>Gets or sets the tick mark length.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Length;

    /// <summary>Gets or sets the tick mark stroke width.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public float Width;

    /// <summary>Gets or sets a value indicating whether to use a hairline stroke.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public bool Hairline;

    /// <summary>Gets or sets the tick mark color.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public Color Color;

    /// <summary>Gets or sets a value indicating whether anti-aliasing is enabled.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public bool AntiAlias;

    /// <summary>Gets or sets the tick mark line pattern.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public LinePattern Pattern = LinePattern.Solid;

    /// <summary>Renders a tick mark using the configured style.</summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="paint">The paint to configure and use.</param>
    /// <param name="pxLine">The line representing the tick mark.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "The parameter name is retained for named-argument compatibility.")]
    public void Render(SKCanvas canvas, Paint paint, PixelLine pxLine)
    {
        ApplyToPaint(paint);
        if (Hairline)
        {
            paint.StrokeWidth = 1F / canvas.TotalMatrix.ScaleX;
        }

        Drawing.DrawLine(canvas, paint, pxLine);
    }

    /// <summary>Applies this style's settings to a paint object.</summary>
    /// <param name="paint">The paint to configure.</param>
    public void ApplyToPaint(Paint paint)
    {
        paint.IsAntialias = AntiAlias;
        paint.IsStroke = true;
        paint.Color = Color;
        paint.StrokeWidth = Hairline ? 1 : Width;
        paint.SKPathEffect = Pattern.GetPathEffect();
    }
}
