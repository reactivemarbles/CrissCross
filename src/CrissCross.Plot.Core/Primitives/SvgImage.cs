// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Renders and stores an SVG image using a Skia canvas.</summary>
internal class SvgImage : IDisposable
{
    /// <summary>The stream containing the rendered SVG document.</summary>
    private readonly MemoryStream _stream;

    /// <summary>Indicates whether this image has been disposed.</summary>
    private bool _isDisposed;

    /// <summary>The canvas used to render the SVG image.</summary>
    private SKCanvas? _canvas;

    /// <summary>Initializes a new instance of the <see cref="SvgImage"/> class.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    public SvgImage(int width, int height)
    {
        Width = width;
        Height = height;
        _stream = new();
        SKRect rect = new(0, 0, Width, Height);
        _canvas = SKSvgCanvas.Create(rect, _stream);
    }

    /// <summary>Gets the image width in pixels.</summary>
    public int Width { get; }

    /// <summary>Gets the image height in pixels.</summary>
    public int Height { get; }

    /// <summary>Gets the canvas while the image is being rendered.</summary>
    public SKCanvas Canvas => _canvas ?? throw new InvalidOperationException("Canvas cannot be accessed after rendering.");

    /// <summary>Disposes the SVG canvas and stream.</summary>
    public void Dispose() => Dispose(true);

    /// <summary>Gets the rendered SVG document as XML.</summary>
    /// <returns>The SVG document text.</returns>
    internal string GetXml()
    {
        // The SVG document is not guaranteed to be valid until the canvas is disposed.
        // See: https://learn.microsoft.com/en-us/dotnet/api/skiasharp.sksvgcanvas?view=skiasharp-2.88#remarks
        _canvas?.Dispose();

        _canvas = null;
        return Encoding.UTF8.GetString(_stream.ToArray());
    }

    /// <summary>Releases managed resources when requested and marks this image disposed.</summary>
    /// <param name="disposing"><see langword="true"/> to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _canvas?.Dispose();
            _stream.Dispose();
        }

        _isDisposed = true;
    }
}
