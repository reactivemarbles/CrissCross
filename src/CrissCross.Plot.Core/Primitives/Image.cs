// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Runtime.InteropServices;
using CrissCross.Plot.IO;

namespace CrissCross.Plot;

/// <summary>Bitmap representation of an image with helper methods for manipulating the image and enabling IO with other platforms.</summary>
public class Image : IDisposable
{
    /// <summary>Number of color channels in an RGB pixel.</summary>
    private const int RgbChannelCount = 3;

    /// <summary>Bit shift for the green channel in packed pixels.</summary>
    private const int GreenChannelShift = 8;

    /// <summary>Bit shift for the blue channel in packed pixels.</summary>
    private const int BlueChannelShift = 16;

    /// <summary>Bit shift for the alpha channel in packed pixels.</summary>
    private const int AlphaChannelShift = 24;

    /// <summary>Index of the first RGB channel in pixel data.</summary>
    private const int FirstColorChannel = 0;

    /// <summary>Index of the second RGB channel in pixel data.</summary>
    private const int SecondColorChannel = 1;

    /// <summary>Index of the third RGB channel in pixel data.</summary>
    private const int ThirdColorChannel = 2;

    /// <summary>Byte offset of the green component in a pixel.</summary>
    private const int GreenPixelOffset = 1;

    /// <summary>Byte offset of the blue component in a pixel.</summary>
    private const int BluePixelOffset = 2;

    /// <summary>Divisor used to calculate the mean of RGB color channels.</summary>
    private const int RgbChannelDivisor = RgbChannelCount;

    /// <summary>Indicates whether this instance has released its image resource.</summary>
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="surface">The surface to snapshot.</param>
    public Image(SKSurface surface)
    {
        ThrowIfNull(surface, nameof(surface));
        SKImage = surface.Snapshot();
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="filename">The image file path.</param>
    public Image(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException(filename);
        }

        byte[] bytes = File.ReadAllBytes(filename);
        SKImage = SKImage.FromEncodedData(bytes);
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="bytes">The encoded image data.</param>
    public Image(byte[] bytes)
    {
        ThrowIfNull(bytes, nameof(bytes));
        SKImage = SKImage.FromEncodedData(bytes);
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="bmp">The bitmap to copy.</param>
    public Image(SKBitmap bmp)
    {
        ThrowIfNull(bmp, nameof(bmp));
        SKImage = SKImage.FromBitmap(bmp);
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    public Image(int width, int height)
        : this(width, height, Colors.Black)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="size">The image dimensions in pixels.</param>
    public Image(PixelSize size)
        : this((int)size.Width, (int)size.Height, Colors.Black)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="color">The initial fill color.</param>
    public Image(int width, int height, Color color)
    {
        using SKSurface surface = SKSurface.Create(new SKImageInfo(width, height));
        SKCanvas canvas = surface.Canvas;
        canvas.Clear(color.ToSKColor());
        SKImage = surface.Snapshot();
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="pixelArray">The grayscale value for each pixel.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The multidimensional byte-array constructor preserves the public pixel-array API.")]
    public Image(byte[, ] pixelArray)
    {
        ThrowIfNull(pixelArray, nameof(pixelArray));

        int width = pixelArray.GetLength(1);
        int height = pixelArray.GetLength(0);
        uint[] pixelValues = new uint[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte value = pixelArray[y, x];
                byte alpha = byte.MaxValue;
                pixelValues[(y * width) + x] = value
                    | ((uint)value << GreenChannelShift)
                    | ((uint)value << BlueChannelShift)
                    | ((uint)alpha << AlphaChannelShift);
            }
        }

        using SKBitmap bitmap = new();
        SKImageInfo info = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        InstallPixelData(bitmap, info, pixelValues);
        SKImage = SKImage.FromBitmap(bitmap);
    }

    /// <summary>Initializes a new instance of the <see cref="Image"/> class.</summary>
    /// <param name="pixelArray">The color channel values for each pixel.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The multidimensional byte-array constructor preserves the public pixel-array API.")]
    public Image(byte[, , ] pixelArray)
    {
        ThrowIfNull(pixelArray, nameof(pixelArray));

        int width = pixelArray.GetLength(1);
        int height = pixelArray.GetLength(0);

        uint[] pixelValues = new uint[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte red = pixelArray[y, x, FirstColorChannel];
                byte green = pixelArray[y, x, SecondColorChannel];
                byte blue = pixelArray[y, x, ThirdColorChannel];
                byte alpha = byte.MaxValue;

                pixelValues[(y * width) + x] = red
                    | ((uint)green << GreenChannelShift)
                    | ((uint)blue << BlueChannelShift)
                    | ((uint)alpha << AlphaChannelShift);
            }
        }

        using SKBitmap bitmap = new();
        SKImageInfo info = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        InstallPixelData(bitmap, info, pixelValues);
        SKImage = SKImage.FromBitmap(bitmap);
    }

    /// <summary>Gets the width of the image in pixels.</summary>
    public int Width => SKImage.Width;

    /// <summary>Gets the height of the image in pixels.</summary>
    public int Height => SKImage.Height;

    /// <summary>Gets the image dimensions in pixels.</summary>
    public PixelSize Size => new(Width, Height);

    /// <summary>Gets the pixel channel ordering used by the image.</summary>
    public PixelFormat PixelFormat =>
        SKImage.ColorType switch
        {
            // ARGB and BGRA are the same bytes interpreted with different endianness.
            SKColorType.Bgra8888 => PixelFormat.Bgra8888,
            SKColorType.Rgba8888 => PixelFormat.Rgba8888,
            _ => PixelFormat.Unknown
        };

    /// <summary>Gets the image alpha representation.</summary>
    public AlphaType AlphaType => SKImage.ColorType.GetAlphaType() switch
    {
        SKAlphaType.Unknown => AlphaType.Unknown,
        SKAlphaType.Opaque => AlphaType.Opaque,
        SKAlphaType.Premul => AlphaType.Premultiplied,
        SKAlphaType.Unpremul => AlphaType.Unpremultiplied,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>Gets the underlying Skia image.</summary>
    internal SKImage SKImage { get; }

    /// <summary>Encodes the image as a platform-independent byte array.</summary>
    /// <param name="format">The desired encoded image format.</param>
    /// <param name="quality">The quality value used by lossy encoders.</param>
    /// <returns>The encoded image bytes.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "Optional encoding defaults are part of the established public API.")]
    public byte[] GetImageBytes(ImageFormat format = ImageFormat.Png, int quality = 100)
    {
        SKEncodedImageFormat encodedFormat = format.ToSKFormat();

        if (format == ImageFormat.Bmp)
        {
            return GetBitmapBytes();
        }

        using SKData encodedData = SKImage.Encode(encodedFormat, quality);
        return encodedData.ToArray();
    }

    /// <summary>Saves the image as a JPEG file.</summary>
    /// <param name="path">The destination path.</param>
    /// <param name="quality">The JPEG quality from 0 to 100.</param>
    /// <returns>Information about the saved file.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional quality default is part of the established public API.")]
    public SavedImageInfo SaveJpeg(string path, int quality = 85)
    {
        byte[] bytes = GetImageBytes(ImageFormat.Jpeg, quality);
        File.WriteAllBytes(path, bytes);
        return new(path, bytes.Length);
    }

    /// <summary>Saves the image as a PNG file.</summary>
    /// <param name="path">The destination path.</param>
    /// <returns>Information about the saved file.</returns>
    public SavedImageInfo SavePng(string path)
    {
        byte[] bytes = GetImageBytes();
        File.WriteAllBytes(path, bytes);
        return new(path, bytes.Length);
    }

    /// <summary>Saves the image as a BMP file.</summary>
    /// <param name="path">The destination path.</param>
    /// <returns>Information about the saved file.</returns>
    public SavedImageInfo SaveBmp(string path)
    {
        byte[] bytes = GetImageBytes(ImageFormat.Bmp);
        File.WriteAllBytes(path, bytes);
        return new(path, bytes.Length);
    }

    /// <summary>Saves the image as a WebP file.</summary>
    /// <param name="path">The destination path.</param>
    /// <param name="quality">The WebP quality from 0 to 100.</param>
    /// <returns>Information about the saved file.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional quality default is part of the established public API.")]
    public SavedImageInfo SaveWebp(string path, int quality = 85)
    {
        byte[] bytes = GetImageBytes(ImageFormat.Webp, quality);
        File.WriteAllBytes(path, bytes);
        return new(path, bytes.Length);
    }

    /// <summary>Saves the image using the requested format.</summary>
    /// <param name="path">The destination path.</param>
    /// <param name="format">The output image format.</param>
    /// <param name="quality">The quality value used by lossy encoders.</param>
    /// <returns>Information about the saved file.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "Optional output defaults are part of the established public API.")]
    public SavedImageInfo Save(string path, ImageFormat format = ImageFormat.Png, int quality = 85) => format switch
        {
            ImageFormat.Png => SavePng(path),
            ImageFormat.Jpeg => SaveJpeg(path, quality),
            ImageFormat.Webp => SaveWebp(path, quality),
            ImageFormat.Bmp => SaveBmp(path),
            _ => throw new ArgumentException($"Unsupported image format: {format}"),
        };

    /// <summary>Releases the image resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Draws the image onto the specified canvas.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="target">The destination rectangle.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="antiAlias">Whether to use antialiasing while resizing.</param>
    public void Render(SKCanvas canvas, PixelRect target, Paint paint, bool antiAlias)
    {
        paint.Color = Colors.White;
        paint.ResizeFilter = antiAlias ? ResizeFilter.Bicubic : ResizeFilter.NearestNeighbor;
        paint.IsAntialias = antiAlias;
        Drawing.DrawImage(canvas, SKImage, target, paint);
    }

    /// <summary>Returns the image as a two-dimensional grayscale byte array.</summary>
    /// <returns>A grayscale value for each pixel, indexed by row then column.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The multidimensional result preserves the established public pixel-array API.")]
    public byte[, ] GetArrayGrayscale()
    {
        using SKBitmap bmp = SKBitmap.FromImage(SKImage);

        ReadOnlySpan<byte> pixelBytes = bmp.GetPixelSpan();
        byte[, ] grayscaleBytes = new byte[bmp.Height, bmp.Width];
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                int offset = ((y * bmp.Width) + x) * bmp.BytesPerPixel;
                byte red = pixelBytes[offset];
                byte green = pixelBytes[offset + GreenPixelOffset];
                byte blue = pixelBytes[offset + BluePixelOffset];
                grayscaleBytes[y, x] = (byte)((red + green + blue) / RgbChannelDivisor);
            }
        }

        return grayscaleBytes;
    }

    /// <summary>Returns the image as a three-dimensional RGB byte array.</summary>
    /// <returns>Color channel values for each pixel, indexed by row, column, then channel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The multidimensional result preserves the established public pixel-array API.")]
    public byte[, , ] GetArrayRGB()
    {
        using SKBitmap bmp = SKBitmap.FromImage(SKImage);

        ReadOnlySpan<byte> pixelBytes = bmp.GetPixelSpan();
        byte[, , ] rgbBytes = new byte[bmp.Height, bmp.Width, RgbChannelCount];
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                int offset = ((y * bmp.Width) + x) * bmp.BytesPerPixel;
                byte red = pixelBytes[offset];
                byte green = pixelBytes[offset + GreenPixelOffset];
                byte blue = pixelBytes[offset + BluePixelOffset];
                rgbBytes[y, x, ThirdColorChannel] = red;
                rgbBytes[y, x, SecondColorChannel] = green;
                rgbBytes[y, x, FirstColorChannel] = blue;
            }
        }

        return rgbBytes;
    }

    /// <summary>Returns a new image with brightness and contrast scaled to the data range.</summary>
    /// <returns>An image with its brightest color channel mapped to the maximum byte value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1020", Justification = "The multidimensional image representation preserves the existing public pixel-array API.")]
    public Image GetAutoscaledImage()
    {
        byte[, , ] values = GetArrayRGB();

        byte maxValue = 0;
        for (int y = 0; y < values.GetLength(0); y++)
        {
            for (int x = 0; x < values.GetLength(1); x++)
            {
                for (int c = 0; c < values.GetLength(RgbChannelCount - 1); c++)
                {
                    maxValue = Math.Max(maxValue, values[y, x, c]);
                }
            }
        }

        if (maxValue == 0)
        {
            return new(values);
        }

        for (int y = 0; y < values.GetLength(0); y++)
        {
            for (int x = 0; x < values.GetLength(1); x++)
            {
                for (int c = 0; c < values.GetLength(RgbChannelCount - 1); c++)
                {
                    values[y, x, c] = (byte)((double)values[y, x, c] / maxValue * byte.MaxValue);
                }
            }
        }

        return new(values);
    }

    /// <summary>Returns an image scaled uniformly by the specified factor.</summary>
    /// <param name="scale">The factor applied to both dimensions.</param>
    /// <param name="antiAlias">Whether to use antialiasing while resizing.</param>
    /// <returns>The scaled image.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional antialiasing default is part of the established public API.")]
    public Image Scaled(double scale, bool antiAlias = true) => Scaled(scale, scale, antiAlias);

    /// <summary>Returns an image scaled by the specified horizontal and vertical factors.</summary>
    /// <param name="scaleX">The factor applied to the width.</param>
    /// <param name="scaleY">The factor applied to the height.</param>
    /// <param name="antiAlias">Whether to use antialiasing while resizing.</param>
    /// <returns>The scaled image.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional antialiasing default is part of the established public API.")]
    public Image Scaled(double scaleX, double scaleY, bool antiAlias = true)
    {
        int newWidth = (int)(scaleX * Width);
        int newHeight = (int)(scaleY * Height);
        return Resized(newWidth, newHeight, antiAlias);
    }

    /// <summary>Returns an image resized to the specified dimensions.</summary>
    /// <param name="newWidth">The output width in pixels.</param>
    /// <param name="newHeight">The output height in pixels.</param>
    /// <param name="antiAlias">Whether to use antialiasing while resizing.</param>
    /// <returns>The resized image.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "SST2309", Justification = "The optional antialiasing default is part of the established public API.")]
    public Image Resized(int newWidth, int newHeight, bool antiAlias = true)
    {
        PixelRect newRect = new(0, newWidth, 0, newHeight);

        using Paint paint = Paint.NewDisposablePaint();
        paint.ResizeFilter = antiAlias ? ResizeFilter.Bicubic : ResizeFilter.NearestNeighbor;
        using SKBitmap targetBitmap = new(newWidth, newHeight);
        using SKCanvas targetCanvas = new(targetBitmap);
        using SKBitmap sourceBitmap = SKBitmap.FromImage(SKImage);
        Drawing.DrawImage(targetCanvas, sourceBitmap, newRect, paint);

        using SKImage newImage = SKImage.FromBitmap(targetBitmap);
        byte[] bytes = GetBitmapBytes(newImage);
        return new(bytes);
    }

    /// <summary>Releases managed image resources.</summary>
    /// <param name="disposing"><see langword="true"/> when called by <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed || !disposing)
        {
            return;
        }

        SKImage.Dispose();
        _isDisposed = true;
    }

    /// <summary>Throws when a required reference parameter is null.</summary>
    /// <typeparam name="T">The reference type to validate.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "PSH1409", Justification = "ArgumentNullException.ThrowIfNull is unavailable for the supported .NET Framework targets.")]
    private static void ThrowIfNull<T>(T value, string parameterName)
        where T : class
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(value, parameterName);
#else
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(parameterName);
#endif
    }

    /// <summary>Creates bitmap-encoded bytes from a Skia image.</summary>
    /// <param name="image">The image to encode.</param>
    /// <returns>The BMP file bytes.</returns>
    private static byte[] GetBitmapBytes(SKImage image)
    {
        using SKBitmap bitmap = SKBitmap.FromImage(image);
        BitmapHeader header = new(image.Width, image.Height);
        byte[] bitmapBytes = new byte[bitmap.Bytes.Length + BitmapHeader.FileHeaderSize];
        IntPtr headerPointer = IntPtr.Zero;

        try
        {
            headerPointer = Marshal.AllocHGlobal(BitmapHeader.FileHeaderSize);
            Marshal.StructureToPtr(header, headerPointer, false);
            Marshal.Copy(headerPointer, bitmapBytes, 0, BitmapHeader.FileHeaderSize);
            Array.Copy(bitmap.Bytes, 0, bitmapBytes, BitmapHeader.FileHeaderSize, bitmap.Bytes.Length);
        }
        finally
        {
            Marshal.FreeHGlobal(headerPointer);
        }

        return bitmapBytes;
    }

    /// <summary>Installs pixel data backed by a pinned array.</summary>
    /// <param name="bitmap">The bitmap receiving the pixel data.</param>
    /// <param name="info">The bitmap dimensions and color information.</param>
    /// <param name="pixelValues">The packed pixel data.</param>
    /// <exception cref="InvalidOperationException">SkiaSharp rejects the pixel data.</exception>
    private static void InstallPixelData(SKBitmap bitmap, SKImageInfo info, uint[] pixelValues)
    {
        GCHandle pixelHandle = GCHandle.Alloc(pixelValues, GCHandleType.Pinned);

        try
        {
            IntPtr pixelPointer = pixelHandle.AddrOfPinnedObject();
            if (!bitmap.InstallPixels(
                info,
                pixelPointer,
                info.RowBytes,
                static (_, state) => ((GCHandle)state).Free(),
                pixelHandle))
            {
                throw new InvalidOperationException("SkiaSharp could not install the pixel data.");
            }
        }
        catch
        {
            if (pixelHandle.IsAllocated)
            {
                pixelHandle.Free();
            }

            throw;
        }
    }

    /// <summary>Returns the image as BMP-encoded bytes.</summary>
    /// <returns>The BMP file bytes.</returns>
    private byte[] GetBitmapBytes() => GetBitmapBytes(SKImage);
}
