// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
using CrissCross.WPF.UI.Controls.Decoding;
#endif
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Decompression;
#else
using CrissCross.WPF.UI.Controls.Decompression;
#endif
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Extensions;
#else
using CrissCross.WPF.UI.Controls.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Contains GIF frame decoding and rendering operations.</summary>
public abstract partial class Animator
{
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)

    /// <summary>Disposes a stream synchronously for targets without async disposal.</summary>
    /// <param name="stream">The stream.</param>
    private static void DisposeStream(Stream stream) => stream.Dispose();
#endif

    /// <summary>Provides the CopyFromBitmap member.</summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="length">The length value.</param>
    private static void CopyFromBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) =>
        Marshal.Copy(bitmap.BackBuffer + offset, buffer, 0, length);

    /// <summary>Provides the CopyToBitmap member.</summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="length">The length value.</param>
    private static void CopyToBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) =>
        Marshal.Copy(buffer, 0, bitmap.BackBuffer + offset, length);

    /// <summary>Provides the CreateBitmap member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <returns>The result.</returns>
    private static WriteableBitmap CreateBitmap(GifDataStream metadata)
    {
        var desc = metadata.Header?.LogicalScreenDescriptor;
        return new(desc!.Width, desc.Height, BitmapDpi, BitmapDpi, PixelFormats.Bgra32, null);
    }

    /// <summary>Provides the CreateIndexStreamBuffer member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="stream">The stream value.</param>
    /// <returns>The result.</returns>
    private static byte[] CreateIndexStreamBuffer(GifDataStream metadata, Stream stream)
    {
        // Find the size of the largest frame pixel data
        // (ignoring the fact that we include the next frame's header)
        var lastSize = stream.Length - metadata.Frames![^1].ImageData!.CompressedDataStartOffset;
        var maxSize = lastSize;
        if (metadata.Frames?.Count > 1)
        {
            var sizes = metadata.Frames.Zip(
                metadata.Frames.Skip(1),
                (f1, f2) => f2.ImageData!.CompressedDataStartOffset - f1.ImageData!.CompressedDataStartOffset);
            maxSize = Math.Max(sizes.Max(), lastSize);
        }

        return new byte[maxSize + BitReaderLookaheadPaddingBytes];
    }

    /// <summary>Provides the CreatePalettes member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <returns>The result.</returns>
    private static Dictionary<int, GifPalette> CreatePalettes(GifDataStream metadata)
    {
        var palettes = new Dictionary<int, GifPalette>();
        Color[]? globalColorTable = null;
        if (metadata.Header?.LogicalScreenDescriptor?.HasGlobalColorTable == true)
        {
            globalColorTable = metadata
                .GlobalColorTable?.Select(gc => Color.FromArgb(0xFF, gc.R, gc.G, gc.B))
                .ToArray();
        }

        for (var i = 0; i < metadata.Frames?.Count; i++)
        {
            var frame = metadata.Frames[i];
            palettes[i] = new(GetTransparencyIndex(frame), GetFrameColorTable(frame, globalColorTable)!);
        }

        return palettes;
    }

    /// <summary>Gets the color table for a GIF frame.</summary>
    /// <param name="frame">The GIF frame.</param>
    /// <param name="globalColorTable">The global color table.</param>
    /// <returns>The frame color table.</returns>
    private static Color[]? GetFrameColorTable(GifFrame frame, Color[]? globalColorTable)
    {
        return frame.Descriptor?.HasLocalColorTable == true
            ? frame.LocalColorTable?.Select(gc => Color.FromArgb(0xFF, gc.R, gc.G, gc.B)).ToArray()
            : globalColorTable;
    }

    /// <summary>Gets the transparency index for a GIF frame.</summary>
    /// <param name="frame">The GIF frame.</param>
    /// <returns>The transparency index, or <see langword="null"/>.</returns>
    private static int? GetTransparencyIndex(GifFrame frame) =>
        frame.GraphicControl is { HasTransparency: true } gce ? gce.TransparencyIndex : null;

    /// <summary>Provides the GetActualRepeatBehavior member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <returns>The result.</returns>
    private static RepeatBehavior GetActualRepeatBehavior(GifDataStream metadata, RepeatBehavior repeatBehavior) =>
        repeatBehavior == default ? GetRepeatBehaviorFromGif(metadata) : repeatBehavior;

    /// <summary>Provides the GetFrameDelay member.</summary>
    /// <param name="frame">The frame value.</param>
    /// <returns>The result.</returns>
    private static TimeSpan GetFrameDelay(GifFrame frame)
    {
        var gce = frame.GraphicControl;
        return gce is not null && gce.Delay != 0
            ? TimeSpan.FromMilliseconds(gce.Delay)
            : TimeSpan.FromMilliseconds(DefaultFrameDelayMilliseconds);
    }

    /// <summary>Provides the GetRepeatBehaviorFromGif member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <returns>The result.</returns>
    private static RepeatBehavior GetRepeatBehaviorFromGif(GifDataStream metadata)
    {
        return metadata.RepeatCount == 0 ? RepeatBehavior.Forever : new RepeatBehavior(metadata.RepeatCount);
    }

    /// <summary>Provides the InterlacedRows member.</summary>
    /// <param name="height">The height value.</param>
    /// <returns>The result.</returns>
    private static IEnumerable<int> InterlacedRows(int height)
    {
        /*
         * 4 passes:
         * Pass 1: rows 0, 8, 16, 24...
         * Pass 2: rows 4, 12, 20, 28...
         * Pass 3: rows 2, 6, 10, 14...
         * Pass 4: rows 1, 3, 5, 7...
         * */
        var passes = new[]
        {
            (Start: InterlaceFirstPassStart, Step: InterlaceCoarseStep),
            (Start: InterlaceSecondPassStart, Step: InterlaceCoarseStep),
            (Start: InterlaceThirdPassStart, Step: InterlaceMediumStep),
            (Start: InterlaceFourthPassStart, Step: InterlaceFineStep),
        };
        foreach (var pass in passes)
        {
            var y = pass.Start;
            while (y < height)
            {
                yield return y;
                y += pass.Step;
            }
        }
    }

    /// <summary>Provides the NormalRows member.</summary>
    /// <param name="height">The height value.</param>
    /// <returns>The result.</returns>
    private static IEnumerable<int> NormalRows(int height) => Enumerable.Range(0, height);

    /// <summary>Provides the WriteColor member.</summary>
    /// <param name="lineBuffer">The lineBuffer value.</param>
    /// <param name="color">The color value.</param>
    /// <param name="startIndex">The startIndex value.</param>
    private static void WriteColor(byte[] lineBuffer, Color color, int startIndex)
    {
        lineBuffer[startIndex] = color.B;
        lineBuffer[startIndex + BgraGreenOffset] = color.G;
        lineBuffer[startIndex + BgraRedOffset] = color.R;
        lineBuffer[startIndex + BgraAlphaOffset] = color.A;
    }

    /// <summary>Provides the ClearArea member.</summary>
    /// <param name="rect">The rect value.</param>
    private void ClearArea(IGifRect? rect) => ClearArea(new Int32Rect(rect!.Left, rect.Top, rect.Width, rect.Height));

    /// <summary>Provides the ClearArea member.</summary>
    /// <param name="rect">The rect value.</param>
    private void ClearArea(Int32Rect rect)
    {
        var bufferLength = BgraBytesPerPixel * rect.Width;
        var lineBuffer = new byte[bufferLength];
        for (var y = 0; y < rect.Height; y++)
        {
            var offset = ((rect.Y + y) * _stride) + (BgraBytesPerPixel * rect.X);
            CopyToBitmap(lineBuffer, _bitmap, offset, bufferLength);
        }

        _bitmap.AddDirtyRect(new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height));
    }

    /// <summary>Provides the CreateTimingManager member.</summary>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <returns>The result.</returns>
    private TimingManager CreateTimingManager(GifDataStream metadata, RepeatBehavior repeatBehavior)
    {
        var actualRepeatBehavior = GetActualRepeatBehavior(metadata, repeatBehavior);

        var manager = new TimingManager(actualRepeatBehavior);
        foreach (var frame in metadata.Frames!)
        {
            manager.Add(GetFrameDelay(frame));
        }

        manager.Completed += TimingManagerCompleted;
        return manager;
    }

    /// <summary>Provides the DisposePreviousFrame member.</summary>
    /// <param name="currentFrame">The currentFrame value.</param>
    private void DisposePreviousFrame(GifFrame currentFrame)
    {
        var pgce = _previousFrame?.GraphicControl;
        if (pgce is not null)
        {
            Action disposePreviousFrame = pgce.DisposalMethod switch
            {
                GifFrameDisposalMethod.None or GifFrameDisposalMethod.DoNotDispose => static () => { },
                GifFrameDisposalMethod.RestoreBackground => () =>
                    ClearArea(GetFixedUpFrameRect(_previousFrame?.Descriptor)),
                GifFrameDisposalMethod.RestorePrevious => RestorePreviousFrame,
                _ => static () => { },
            };

            disposePreviousFrame();
        }

        var gce = currentFrame.GraphicControl;
        if (gce is not { DisposalMethod: GifFrameDisposalMethod.RestorePrevious })
        {
            return;
        }

        CopyFromBitmap(_previousBackBuffer, _bitmap, 0, _previousBackBuffer.Length);
    }

    /// <summary>Provides the GetFixedUpFrameRect member.</summary>
    /// <param name="desc">The desc value.</param>
    /// <returns>The result.</returns>
    private Int32Rect GetFixedUpFrameRect(GifImageDescriptor? desc)
    {
        var width = Math.Min(desc!.Width, _bitmap.PixelWidth - desc.Left);
        var height = Math.Min(desc.Height, _bitmap.PixelHeight - desc.Top);
        return new(desc.Left, desc.Top, width, height);
    }

    /// <summary>Provides the GetIndexBytesAsync member.</summary>
    /// <param name="frameIndex">The frameIndex value.</param>
    /// <param name="buffer">The buffer value.</param>
    /// <returns>The result.</returns>
    private async Task GetIndexBytesAsync(int frameIndex, byte[] buffer)
    {
        var startPosition = _metadata.Frames![frameIndex].ImageData!.CompressedDataStartOffset;

        _ = _sourceStream.Seek(startPosition, SeekOrigin.Begin);
#if NET8_0_OR_GREATER
        await using var memoryStream = new MemoryStream(buffer);
#else
        using var memoryStream = new MemoryStream(buffer);
#endif
        await GifHelpers.CopyDataBlocksToStreamAsync(_sourceStream, memoryStream).ConfigureAwait(false);
    }

    /// <summary>Provides the GetIndexStreamAsync member.</summary>
    /// <param name="frame">The frame value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    private async Task<Stream> GetIndexStreamAsync(GifFrame frame, CancellationToken cancellationToken)
    {
        var data = frame.ImageData;
        cancellationToken.ThrowIfCancellationRequested();
        _ = _sourceStream.Seek(data!.CompressedDataStartOffset, SeekOrigin.Begin);
#if NET8_0_OR_GREATER
        await using (var ms = new MemoryStream(_indexStreamBuffer))
#else
        using (var ms = new MemoryStream(_indexStreamBuffer))
#endif
        {
            await GifHelpers.CopyDataBlocksToStreamAsync(_sourceStream, ms, cancellationToken).ConfigureAwait(false);
        }

        return new LzwDecompressStream(_indexStreamBuffer, data.LzwMinimumCodeSize);
    }

    /// <summary>Provides the LoadFrames member.</summary>
    /// <returns>The result.</returns>
    private async Task LoadFrames()
    {
        var biggestFrameSize = 0L;
        for (var frameIndex = 0; frameIndex < _metadata.Frames!.Count; frameIndex++)
        {
            var startPosition = _metadata.Frames![frameIndex].ImageData!.CompressedDataStartOffset;
            var endPosition =
                _metadata.Frames.Count == frameIndex + 1
                    ? _sourceStream.Length
                    : _metadata.Frames![frameIndex + 1].ImageData!.CompressedDataStartOffset - 1;
            var size = endPosition - startPosition;
            biggestFrameSize = Math.Max(size, biggestFrameSize);
        }

        var indexCompressedBytes = new byte[biggestFrameSize];
        for (var frameIndex = 0; frameIndex < _metadata.Frames.Count; frameIndex++)
        {
            var frame = _metadata.Frames[frameIndex];
            var frameDesc = _metadata.Frames[frameIndex].Descriptor;
            await GetIndexBytesAsync(frameIndex, indexCompressedBytes);
#if NET8_0_OR_GREATER
            await using var indexDecompressedStream = new LzwDecompressStream(
                indexCompressedBytes,
                frame.ImageData!.LzwMinimumCodeSize);
#else
            using var indexDecompressedStream = new LzwDecompressStream(
                indexCompressedBytes,
                frame.ImageData!.LzwMinimumCodeSize);
#endif
            _cachedFrameBytes![frameIndex] = new byte[frameDesc!.Width * frameDesc.Height];

            await indexDecompressedStream.ReadAllAsync(
                _cachedFrameBytes[frameIndex],
                0,
                frameDesc.Width * frameDesc.Height);
        }
    }

    /// <summary>Provides the RenderFrameAsync member.</summary>
    /// <param name="frameIndex">The frameIndex value.</param>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    private async Task RenderFrameAsync(int frameIndex, CancellationToken cancellationToken)
    {
        if (frameIndex < 0)
        {
            return;
        }

        var frame = _metadata.Frames![frameIndex];
        var desc = frame.Descriptor;
        var rect = GetFixedUpFrameRect(desc);

        Stream? indexStream = null;
        if (!_cacheFrameDataInMemory)
        {
            indexStream = await GetIndexStreamAsync(frame, cancellationToken);
        }

        using (indexStream)
        using (_bitmap.LockInScope())
        {
            UpdatePreviousFrame(frameIndex, frame);

            var bufferLength = BgraBytesPerPixel * rect.Width;
            var lineBuffer = new byte[bufferLength];

            var palette = _palettes[frameIndex];
            var transparencyIndex = palette.TransparencyIndex ?? -1;
            var rows = desc!.Interlace ? InterlacedRows(rect.Height).ToArray() : NormalRows(rect.Height).ToArray();
            var indexBuffer = await GetFrameIndexBufferAsync(frameIndex, frame, desc, indexStream, cancellationToken);

            RenderFrameRows(desc, rect, rows, indexBuffer, lineBuffer, palette, transparencyIndex);

            _bitmap.AddDirtyRect(rect);
        }

        _previousFrame = frame;
        _previousFrameIndex = frameIndex;
    }

    /// <summary>Gets the frame index buffer.</summary>
    /// <param name="frameIndex">The frame index.</param>
    /// <param name="frame">The frame.</param>
    /// <param name="desc">The frame descriptor.</param>
    /// <param name="indexStream">The index stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The frame index buffer.</returns>
    private async Task<byte[]> GetFrameIndexBufferAsync(
        int frameIndex,
        GifFrame frame,
        GifImageDescriptor desc,
        Stream? indexStream,
        CancellationToken cancellationToken)
    {
        _ = frame;
        if (_cacheFrameDataInMemory)
        {
            return _cachedFrameBytes![frameIndex];
        }

        var indexBuffer = new byte[desc.Width * desc.Height];
        await indexStream!.ReadAllAsync(indexBuffer, 0, indexBuffer.Length, cancellationToken);
        return indexBuffer;
    }

    /// <summary>Renders frame rows into the bitmap.</summary>
    /// <param name="desc">The frame descriptor.</param>
    /// <param name="rect">The frame rectangle.</param>
    /// <param name="rows">The rows to render.</param>
    /// <param name="indexBuffer">The index buffer.</param>
    /// <param name="lineBuffer">The line buffer.</param>
    /// <param name="palette">The GIF palette.</param>
    /// <param name="transparencyIndex">The transparency index.</param>
    private void RenderFrameRows(
        GifImageDescriptor desc,
        Int32Rect rect,
        int[] rows,
        byte[] indexBuffer,
        byte[] lineBuffer,
        GifPalette palette,
        int transparencyIndex)
    {
        for (var y = 0; y < rect.Height; y++)
        {
            var offset = ((desc.Top + rows[y]) * _stride) + (desc.Left * BgraBytesPerPixel);

            if (transparencyIndex >= 0)
            {
                CopyFromBitmap(lineBuffer, _bitmap, offset, lineBuffer.Length);
            }

            for (var x = 0; x < rect.Width; x++)
            {
                var index = indexBuffer[x + (y * desc.Width)];
                var i = BgraBytesPerPixel * x;
                if (index != transparencyIndex)
                {
                    WriteColor(lineBuffer, palette[index], i);
                }
            }

            CopyToBitmap(lineBuffer, _bitmap, offset, lineBuffer.Length);
        }
    }

    /// <summary>Updates the previous frame state before rendering the next frame.</summary>
    /// <param name="frameIndex">The frame index.</param>
    /// <param name="frame">The frame.</param>
    private void UpdatePreviousFrame(int frameIndex, GifFrame frame)
    {
        if (frameIndex < _previousFrameIndex)
        {
            ClearArea(_metadata.Header?.LogicalScreenDescriptor);
            return;
        }

        DisposePreviousFrame(frame);
    }

    /// <summary>Restores the previous full bitmap buffer.</summary>
    private void RestorePreviousFrame()
    {
        CopyToBitmap(_previousBackBuffer, _bitmap, 0, _previousBackBuffer.Length);
        var desc = _metadata.Header?.LogicalScreenDescriptor;
        var rect = new Int32Rect(0, 0, desc!.Width, desc.Height);
        _bitmap.AddDirtyRect(rect);
    }

    /// <summary>Provides the RunAsync member.</summary>
    /// <param name="cancellationToken">The cancellationToken value.</param>
    /// <returns>The result.</returns>
    private async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_loadFramesDataTask is not null)
        {
            await _loadFramesDataTask;
        }

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var timing = _timingManager.NextAsync(cancellationToken);
            var rendering = RenderFrameAsync(CurrentFrameIndex, cancellationToken);
            await Task.WhenAll(timing, rendering);
            if (!await timing)
            {
                break;
            }

            CurrentFrameIndex = (CurrentFrameIndex + 1) % FrameCount;
        }
    }

    /// <summary>Provides the TimingManagerCompleted member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void TimingManagerCompleted(object? sender, EventArgs e) => OnAnimationCompleted();

    /// <summary>Provides the GifPalette member.</summary>
    /// <param name="transparencyIndex">The transparencyIndex value.</param>
    /// <param name="colors">The colors value.</param>
    private sealed class GifPalette(int? transparencyIndex, Color[] colors)
    {
        /// <summary>Gets the TransparencyIndex value.</summary>
        public int? TransparencyIndex { get; } = transparencyIndex;

        public Color this[int i] => colors[i];
    }
}
