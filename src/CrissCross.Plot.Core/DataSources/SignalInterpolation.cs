// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Interpolates signal segments that extend beyond the plot data rectangle.</summary>
public static class SignalInterpolation
{
    /// <summary>The minimum number of points required for interpolation.</summary>
    private const int MinimumPixelCount = 3;

    /// <summary>The negative index offset used to select the second-to-last point.</summary>
    private const int SecondToLastIndex = 2;

    /// <summary>The exception message used for unsupported connection styles.</summary>
    private const string UnsupportedConnectionStyleMessage = "Unsupported connection style.";

    /// <summary>
    /// If the point to the left of the graph is extremely far outside the data area,
    /// modify it using interpolation so it is closer to the data area and avoids render artifacts.
    /// </summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="pixels">The rendered points, including outside points at either end.</param>
    /// <param name="connectStyle">The style connecting consecutive points.</param>
    public static void InterpolateBeforeX(RenderPack rp, Pixel[] pixels, ConnectStyle connectStyle)
    {
        if (pixels.Length < MinimumPixelCount)
        {
            return;
        }

        Pixel lastOutsidePoint = pixels[0];
        Pixel firstInsidePoint = pixels[1];
        if (Math.Abs(lastOutsidePoint.X - firstInsidePoint.X) <= float.Epsilon)
        {
            return;
        }

        float boundaryPixel = rp.DataRect.Left - 1;
        float verticalDifference = lastOutsidePoint.Y - firstInsidePoint.Y;
        float horizontalOffset = boundaryPixel - firstInsidePoint.X;
        float segmentWidth = lastOutsidePoint.X - firstInsidePoint.X;
        float boundaryY = connectStyle switch
        {
            ConnectStyle.Straight => firstInsidePoint.Y + (verticalDifference * horizontalOffset / segmentWidth),
            ConnectStyle.StepHorizontal => lastOutsidePoint.Y,
            ConnectStyle.StepVertical => firstInsidePoint.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(connectStyle), connectStyle, UnsupportedConnectionStyleMessage),
        };

        pixels[0] = new(boundaryPixel, boundaryY);
    }

    /// <summary>
    /// If the point below the graph is extremely far outside the data area,
    /// modify it using interpolation so it is closer to the data area and avoids render artifacts.
    /// </summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="pixels">The rendered points, including outside points at either end.</param>
    /// <param name="connectStyle">The style connecting consecutive points.</param>
    public static void InterpolateBeforeY(RenderPack rp, Pixel[] pixels, ConnectStyle connectStyle)
    {
        if (pixels.Length < MinimumPixelCount)
        {
            return;
        }

        Pixel lastOutsidePoint = pixels[0];
        Pixel firstInsidePoint = pixels[1];
        if (Math.Abs(lastOutsidePoint.Y - firstInsidePoint.Y) <= float.Epsilon)
        {
            return;
        }

        float boundaryPixel = rp.DataRect.Bottom + 1;
        float horizontalDifference = lastOutsidePoint.X - firstInsidePoint.X;
        float verticalOffset = boundaryPixel - firstInsidePoint.Y;
        float segmentHeight = lastOutsidePoint.Y - firstInsidePoint.Y;
        float boundaryX = connectStyle switch
        {
            ConnectStyle.Straight => firstInsidePoint.X + (horizontalDifference * verticalOffset / segmentHeight),
            ConnectStyle.StepHorizontal => lastOutsidePoint.X,
            ConnectStyle.StepVertical => firstInsidePoint.X,
            _ => throw new ArgumentOutOfRangeException(nameof(connectStyle), connectStyle, UnsupportedConnectionStyleMessage),
        };

        pixels[0] = new(boundaryX, boundaryPixel);
    }

    /// <summary>
    /// If the point to the right of the graph is extremely far outside the data area,
    /// modify it using interpolation so it is closer to the data area and avoids render artifacts.
    /// </summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="pixels">The rendered points, including outside points at either end.</param>
    /// <param name="connectStyle">The style connecting consecutive points.</param>
    public static void InterpolateAfterX(RenderPack rp, Pixel[] pixels, ConnectStyle connectStyle)
    {
        if (pixels.Length < MinimumPixelCount)
        {
            return;
        }

        Pixel lastInsidePoint = pixels[^SecondToLastIndex];
        Pixel firstOutsidePoint = pixels[^1];
        if (Math.Abs(firstOutsidePoint.X - lastInsidePoint.X) <= float.Epsilon)
        {
            return;
        }

        float boundaryPixel = rp.DataRect.Right + 1;
        float verticalDifference = firstOutsidePoint.Y - lastInsidePoint.Y;
        float horizontalOffset = boundaryPixel - lastInsidePoint.X;
        float segmentWidth = firstOutsidePoint.X - lastInsidePoint.X;
        float boundaryY = connectStyle switch
        {
            ConnectStyle.Straight => lastInsidePoint.Y + (verticalDifference * horizontalOffset / segmentWidth),
            ConnectStyle.StepHorizontal => lastInsidePoint.Y,
            ConnectStyle.StepVertical => firstOutsidePoint.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(connectStyle), connectStyle, UnsupportedConnectionStyleMessage),
        };

        pixels[^1] = new(boundaryPixel, boundaryY);
    }

    /// <summary>
    /// If the point above the graph is extremely far outside the data area,
    /// modify it using interpolation so it is closer to the data area and avoids render artifacts.
    /// </summary>
    /// <param name="rp">The render pack containing the data rectangle.</param>
    /// <param name="pixels">The rendered points, including outside points at either end.</param>
    /// <param name="connectStyle">The style connecting consecutive points.</param>
    public static void InterpolateAfterY(RenderPack rp, Pixel[] pixels, ConnectStyle connectStyle)
    {
        if (pixels.Length < MinimumPixelCount)
        {
            return;
        }

        Pixel lastInsidePoint = pixels[^SecondToLastIndex];
        Pixel firstOutsidePoint = pixels[^1];
        if (Math.Abs(firstOutsidePoint.Y - lastInsidePoint.Y) <= float.Epsilon)
        {
            return;
        }

        float boundaryPixel = rp.DataRect.Top - 1;
        float horizontalDifference = firstOutsidePoint.X - lastInsidePoint.X;
        float verticalOffset = boundaryPixel - lastInsidePoint.Y;
        float segmentHeight = firstOutsidePoint.Y - lastInsidePoint.Y;
        float boundaryX = connectStyle switch
        {
            ConnectStyle.Straight => lastInsidePoint.X + (horizontalDifference * verticalOffset / segmentHeight),
            ConnectStyle.StepHorizontal => firstOutsidePoint.X,
            ConnectStyle.StepVertical => lastInsidePoint.X,
            _ => throw new ArgumentOutOfRangeException(nameof(connectStyle), connectStyle, UnsupportedConnectionStyleMessage),
        };

        pixels[^1] = new(boundaryX, boundaryPixel);
    }
}
