// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Enables dragging supported plottables by updating their data offsets.</summary>
public class DraggablePlottableDecorator : HittablePlottableDecorator
{
    /// <summary>The coordinate where the active drag began.</summary>
    private Coordinates _dragFrom;

    /// <summary>The plottable's offset before the active drag began.</summary>
    private Coordinates _beforeDragOffset;

    /// <summary>Initializes a new instance of the <see cref="DraggablePlottableDecorator"/> class.</summary>
    /// <param name="plottable">The supported plottable to wrap.</param>
    public DraggablePlottableDecorator(IPlottable plottable)
        : base(plottable)
    {
        _ = GetOffset();
    }

    /// <summary>Starts dragging at the specified coordinate.</summary>
    /// <param name="start">The initial drag coordinate.</param>
    public void StartDrag(Coordinates start)
    {
        _dragFrom = start;
        _beforeDragOffset = GetOffset();
    }

    /// <summary>Moves the wrapped plottable to the specified coordinate.</summary>
    /// <param name="to">The current drag coordinate.</param>
    public void DragTo(Coordinates to)
    {
        double offsetX = _beforeDragOffset.X + to.X - _dragFrom.X;
        double offsetY = _beforeDragOffset.Y + to.Y - _dragFrom.Y;
        SetOffset(offsetX, offsetY);
    }

    /// <summary>Gets the current offset of the supported plottable.</summary>
    /// <returns>The wrapped plottable's current offset.</returns>
    private Coordinates GetOffset() => Source switch
    {
        Signal signal => new Coordinates(signal.Data.XOffset, signal.Data.YOffset),
        SignalXY signalXY => new Coordinates(signalXY.Data.XOffset, signalXY.Data.YOffset),
        Arrow arrow => new Coordinates(arrow.Base.X, arrow.Base.Y),
        DataLogger dataLogger => new Coordinates(dataLogger.Data.XOffset, dataLogger.Data.YOffset),
        DataStreamer dataStreamer => new Coordinates(dataStreamer.Data.OffsetX, dataStreamer.Data.OffsetY),
        Ellipse ellipse => new Coordinates(ellipse.Center.X, ellipse.Center.Y),
        ImageMarker imageMarker => new Coordinates(imageMarker.Location.X, imageMarker.Location.Y),
        Marker marker => new Coordinates(marker.X, marker.Y),
        Scatter scatter => new Coordinates(scatter.OffsetX, scatter.OffsetY),
        Text text => text.Location,
        _ => throw new ArgumentOutOfRangeException(nameof(Source), Source, $"Unsupported plottable to drag: {Source}")
    };

    /// <summary>Sets the offset of the wrapped plottable.</summary>
    /// <param name="offsetX">The horizontal offset.</param>
    /// <param name="offsetY">The vertical offset.</param>
    private void SetOffset(double offsetX, double offsetY)
    {
        if (TrySetDataOffset(offsetX, offsetY) || TrySetPlottableOffset(offsetX, offsetY))
        {
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(Source), Source, $"Unsupported plottable to drag: {Source}");
    }

    /// <summary>Sets offsets for supported data-backed plottables.</summary>
    /// <param name="offsetX">The horizontal offset.</param>
    /// <param name="offsetY">The vertical offset.</param>
    /// <returns><see langword="true"/> when the source was handled.</returns>
    private bool TrySetDataOffset(double offsetX, double offsetY)
    {
        if (Source is Signal signal)
        {
            OffsetHelpers.SetOffset(signal, offsetX, offsetY);
            return true;
        }

        if (Source is SignalXY signalXY)
        {
            OffsetHelpers.SetOffset(signalXY, offsetX, offsetY);
            return true;
        }

        if (Source is DataLogger dataLogger)
        {
            dataLogger.Data.XOffset = offsetX;
            dataLogger.Data.YOffset = offsetY;
            return true;
        }

        if (Source is not DataStreamer dataStreamer)
        {
            return false;
        }

        dataStreamer.Data.OffsetX = offsetX;
        dataStreamer.Data.OffsetY = offsetY;
        return true;
    }

    /// <summary>Sets offsets for supported annotation and geometry plottables.</summary>
    /// <param name="offsetX">The horizontal offset.</param>
    /// <param name="offsetY">The vertical offset.</param>
    /// <returns><see langword="true"/> when the source was handled.</returns>
    private bool TrySetPlottableOffset(double offsetX, double offsetY)
    {
        if (Source is Arrow arrow)
        {
            OffsetHelpers.SetOffset(arrow, offsetX, offsetY);
            return true;
        }

        if (Source is Ellipse ellipse)
        {
            ellipse.Center.X = offsetX;
            ellipse.Center.Y = offsetY;
            return true;
        }

        if (Source is ImageMarker imageMarker)
        {
            imageMarker.Location = new(offsetX, offsetY);
            return true;
        }

        if (Source is Marker marker)
        {
            marker.X = offsetX;
            marker.Y = offsetY;
            return true;
        }

        if (Source is Scatter scatter)
        {
            scatter.OffsetX = offsetX;
            scatter.OffsetY = offsetY;
            return true;
        }

        if (Source is not Text text)
        {
            return false;
        }

        text.Location = new(offsetX, offsetY);
        return true;
    }

    /// <summary>Contains offset updates for supported plottable data types.</summary>
    private static class OffsetHelpers
    {
        /// <summary>Sets the data offsets for a signal.</summary>
        /// <param name="signal">The signal to update.</param>
        /// <param name="offsetX">The horizontal offset.</param>
        /// <param name="offsetY">The vertical offset.</param>
        public static void SetOffset(Signal signal, double offsetX, double offsetY)
        {
            signal.Data.XOffset = offsetX;
            signal.Data.YOffset = offsetY;
        }

        /// <summary>Sets the data offsets for an XY signal.</summary>
        /// <param name="signalXY">The signal to update.</param>
        /// <param name="offsetX">The horizontal offset.</param>
        /// <param name="offsetY">The vertical offset.</param>
        public static void SetOffset(SignalXY signalXY, double offsetX, double offsetY)
        {
            signalXY.Data.XOffset = offsetX;
            signalXY.Data.YOffset = offsetY;
        }

        /// <summary>Moves an arrow while preserving its length and direction.</summary>
        /// <param name="arrow">The arrow to move.</param>
        /// <param name="offsetX">The new horizontal base coordinate.</param>
        /// <param name="offsetY">The new vertical base coordinate.</param>
        public static void SetOffset(Arrow arrow, double offsetX, double offsetY)
        {
            double tipDeltaX = arrow.Tip.X - arrow.Base.X;
            double tipDeltaY = arrow.Tip.Y - arrow.Base.Y;
            arrow.Base = new(offsetX, offsetY);
            arrow.Tip = new(offsetX + tipDeltaX, offsetY + tipDeltaY);
        }
    }
}
