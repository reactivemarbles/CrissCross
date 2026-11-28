// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Stores the state captured when an axis span is interacted with.</summary>
public class AxisSpanUnderMouse
{
    /// <summary>Gets or sets the axis span under the pointer.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a required public field to preserve the existing interaction-state API.")]
    public required Plottables.AxisSpan Span;

    /// <summary>Gets or sets the pointer position when the interaction began.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a required public field to preserve the existing interaction-state API.")]
    public required Coordinates MouseStart;

    /// <summary>Gets or sets the original range of the span.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a required public field to preserve the existing interaction-state API.")]
    public required CoordinateRange OriginalRange;

    /// <summary>Gets or sets whether the first edge is being resized.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing interaction-state API.")]
    public bool ResizeEdge1;

    /// <summary>Gets or sets whether the second edge is being resized.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as a public field to preserve the existing interaction-state API.")]
    public bool ResizeEdge2;

    /// <summary>Gets whether either edge is being resized.</summary>
    public bool IsResizing => ResizeEdge1 || ResizeEdge2;

    /// <summary>Gets whether the span is being moved rather than resized.</summary>
    public bool IsMoving => !IsResizing;

    /// <summary>Gets whether a vertical span is being resized.</summary>
    public bool IsResizingVertically => IsResizing && Span is Plottables.VerticalSpan;

    /// <summary>Gets whether a horizontal span is being resized.</summary>
    public bool IsResizingHorizontally => IsResizing && Span is Plottables.HorizontalSpan;

    /// <summary>Applies the current pointer position to the captured span interaction.</summary>
    /// <param name="mouseNow">The current pointer position.</param>
    public void DragTo(Coordinates mouseNow) => Span.DragTo(this, mouseNow);
}
