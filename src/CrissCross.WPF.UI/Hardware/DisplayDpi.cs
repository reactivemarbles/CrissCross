// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Hardware;

/// <summary>
/// Stores DPI information from which a <see cref="Visual"/> or <see cref="UIElement"/>
/// is rendered.
/// </summary>
/// <param name="DpiX"> Gets the DPI on the X axis. </param>
/// <param name="DpiY"> Gets the DPI on the Y axis. </param>
/// <param name="DpiScaleX"> Gets the DPI scale on the X axis. </param>
/// <param name="DpiScaleY"> Gets the DPI scale on the Y axis. </param>
public readonly record struct DisplayDpi(int DpiX, int DpiY, double DpiScaleX, double DpiScaleY)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayDpi"/> structure.
    /// </summary>
    /// <param name="dpiScaleX">The DPI scale on the X axis.</param>
    /// <param name="dpiScaleY">The DPI scale on the Y axis.</param>
    public DisplayDpi(double dpiScaleX, double dpiScaleY)
        : this((int)Math.Round(DpiHelper.DefaultDpi * dpiScaleX, MidpointRounding.AwayFromZero), (int)Math.Round(DpiHelper.DefaultDpi * dpiScaleY, MidpointRounding.AwayFromZero), dpiScaleX, dpiScaleY)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayDpi"/> structure.
    /// </summary>
    /// <param name="dpiX">The DPI on the X axis.</param>
    /// <param name="dpiY">The DPI on the Y axis.</param>
    public DisplayDpi(int dpiX, int dpiY)
        : this(dpiX, dpiY, dpiX / (double)DpiHelper.DefaultDpi, dpiY / (double)DpiHelper.DefaultDpi)
    {
    }

    /// <summary>
    /// Gets the DPI on the X axis.
    /// </summary>
    public int DpiX { get; } = DpiX;

    /// <summary>
    /// Gets the DPI on the Y axis.
    /// </summary>
    public int DpiY { get; } = DpiY;

    /// <summary>
    /// Gets the DPI scale on the X axis.
    /// </summary>
    public double DpiScaleX { get; } = DpiScaleX;

    /// <summary>
    /// Gets the DPI scale on the Y axis.
    /// </summary>
    public double DpiScaleY { get; } = DpiScaleY;
}
