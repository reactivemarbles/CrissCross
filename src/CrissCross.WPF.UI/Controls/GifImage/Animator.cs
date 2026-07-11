// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CrissCross.WPF.UI.Controls.Decoding;
using CrissCross.WPF.UI.Controls.Decompression;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents Animator.</summary>
/// <seealso cref="System.Windows.DependencyObject" />
/// <seealso cref="System.IDisposable" />
public abstract class Animator : DependencyObject, IDisposable
{
    /// <summary>The BGRA bytes per pixel.</summary>
    private const int BgraBytesPerPixel = 4;

    /// <summary>The bitmap DPI used for decoded GIF frames.</summary>
    private const int BitmapDpi = 96;

    /// <summary>The default GIF frame delay in milliseconds.</summary>
    private const int DefaultFrameDelayMilliseconds = 100;

    /// <summary>The one-based Green channel offset in BGRA pixels.</summary>
    private const int BgraGreenOffset = 1;

    /// <summary>The one-based Red channel offset in BGRA pixels.</summary>
    private const int BgraRedOffset = 2;

    /// <summary>The one-based Alpha channel offset in BGRA pixels.</summary>
    private const int BgraAlphaOffset = 3;

    /// <summary>The GIF interlace first pass start row.</summary>
    private const int InterlaceFirstPassStart = 0;

    /// <summary>The GIF interlace second pass start row.</summary>
    private const int InterlaceSecondPassStart = 4;

    /// <summary>The GIF interlace third pass start row.</summary>
    private const int InterlaceThirdPassStart = 2;

    /// <summary>The GIF interlace fourth pass start row.</summary>
    private const int InterlaceFourthPassStart = 1;

    /// <summary>The GIF interlace coarse row step.</summary>
    private const int InterlaceCoarseStep = 8;

    /// <summary>The GIF interlace medium row step.</summary>
    private const int InterlaceMediumStep = 4;

    /// <summary>The GIF interlace fine row step.</summary>
    private const int InterlaceFineStep = 2;

    /// <summary>The byte padding required for BitReader lookahead.</summary>
    private const int BitReaderLookaheadPaddingBytes = 4;

    /// <summary>The bitmap stride alignment in bits.</summary>
    private const int BitmapStrideAlignmentBits = 32;

    /// <summary>The bitmap stride alignment mask.</summary>
    private const int BitmapStrideAlignmentMask = 31;

    /// <summary>Stores the _bitmap value.</summary>
    private readonly WriteableBitmap _bitmap;

    /// <summary>Stores the _cachedFrameBytes value.</summary>
    private readonly byte[][]? _cachedFrameBytes;

    /// <summary>Stores the _cacheFrameDataInMemory value.</summary>
    private readonly bool _cacheFrameDataInMemory;

    /// <summary>Stores the _indexStreamBuffer value.</summary>
    private readonly byte[] _indexStreamBuffer;

    /// <summary>Stores the _isSourceStreamOwner value.</summary>
    private readonly bool _isSourceStreamOwner;

    /// <summary>Stores the _loadFramesDataTask value.</summary>
    private readonly Task? _loadFramesDataTask;

    /// <summary>Stores the _metadata value.</summary>
    private readonly GifDataStream _metadata;

    /// <summary>Stores the _palettes value.</summary>
    private readonly Dictionary<int, GifPalette> _palettes;

    /// <summary>Stores the _previousBackBuffer value.</summary>
    private readonly byte[] _previousBackBuffer;

    /// <summary>Stores the _sourceStream value.</summary>
    private readonly Stream _sourceStream;

    /// <summary>Stores the _sourceUri value.</summary>
    private readonly Uri? _sourceUri;

    /// <summary>Stores the _stride value.</summary>
    private readonly int _stride;

    /// <summary>Stores the _timingManager value.</summary>
    private readonly TimingManager _timingManager;

    /// <summary>Stores the _cancellationTokenSource value.</summary>
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>Stores the _disposed value.</summary>
    private bool _disposed;

    /// <summary>Stores the _disposing value.</summary>
    private volatile bool _disposing;

    /// <summary>Stores the _isStarted value.</summary>
    private bool _isStarted;

    /// <summary>Stores the _previousFrame value.</summary>
    private GifFrame? _previousFrame;

    /// <summary>Stores the _previousFrameIndex value.</summary>
    private int _previousFrameIndex;

    /// <summary>Initializes a new instance of the <see cref="Animator"/> class.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="cacheFrameDataInMemory">The cacheFrameDataInMemory value.</param>
    internal Animator(Stream sourceStream, Uri? sourceUri, GifDataStream metadata, RepeatBehavior repeatBehavior, bool cacheFrameDataInMemory)
    {
        _sourceStream = sourceStream;
        _sourceUri = sourceUri;
        _isSourceStreamOwner = sourceUri is not null; // stream opened from URI, should close it
        _metadata = metadata;
        _palettes = CreatePalettes(metadata);
        _bitmap = CreateBitmap(metadata);
        var desc = metadata.Header?.LogicalScreenDescriptor;
        _stride = BgraBytesPerPixel * (((desc!.Width * BitmapStrideAlignmentBits) + BitmapStrideAlignmentMask) / BitmapStrideAlignmentBits);
        _previousBackBuffer = new byte[desc.Height * _stride];
        _indexStreamBuffer = CreateIndexStreamBuffer(metadata, _sourceStream);
        _timingManager = CreateTimingManager(metadata, repeatBehavior);

        _cacheFrameDataInMemory = cacheFrameDataInMemory;

        if (!cacheFrameDataInMemory)
        {
            return;
        }

        _cachedFrameBytes = new byte[_metadata.Frames!.Count][];
        _loadFramesDataTask = Task.Run(LoadFrames);
    }

    /// <summary>Finalizes an instance of the <see cref="Animator"/> class.</summary>
    ~Animator()
    {
        Dispose(false);
    }

    /// <summary>Occurs when [animation completed].</summary>
    public event EventHandler<AnimationCompletedEventArgs>? AnimationCompleted;

    /// <summary>Occurs when [animation started].</summary>
    public event EventHandler<AnimationStartedEventArgs>? AnimationStarted;

    /// <summary>Occurs when [current frame changed].</summary>
    public event EventHandler? CurrentFrameChanged;

    /// <summary>Occurs when [error].</summary>
    public event EventHandler<AnimationErrorEventArgs>? Error;

    /// <summary>Gets the index of the current frame.</summary>
    /// <value>
    /// The index of the current frame.
    /// </value>
    public int CurrentFrameIndex
    {
        get => field;
        private set
        {
            field = value;
            OnCurrentFrameChanged();
        }
    }

    /// <summary>Gets the frame count.</summary>
    /// <value>
    /// The frame count.
    /// </value>
    public int FrameCount => _metadata.Frames!.Count;

    /// <summary>Gets a value indicating whether this instance is complete.</summary>
    /// <value>
    ///   <c>true</c> if this instance is complete; otherwise, <c>false</c>.
    /// </value>
    public bool IsComplete => _isStarted && _timingManager.IsComplete;

    /// <summary>Gets a value indicating whether this instance is paused.</summary>
    /// <value>
    ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    public bool IsPaused => _timingManager.IsPaused;

    /// <summary>Gets the Bitmap value.</summary>
    internal BitmapSource Bitmap => _bitmap;

    /// <summary>Gets the animation source.</summary>
    /// <value>
    /// The animation source.
    /// </value>
    protected abstract object AnimationSource { get; }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Pauses this instance.</summary>
    public void Pause() => _timingManager.Pause();

    /// <summary>Plays this instance.</summary>
    public async void Play()
    {
        try
        {
            if (_timingManager.IsComplete)
            {
                _timingManager.Reset();
                _isStarted = false;
            }

            if (!_isStarted)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new();
                _isStarted = true;
                OnAnimationStarted();
                if (_timingManager.IsPaused)
                {
                    _timingManager.Resume();
                }

                await RunAsync(_cancellationTokenSource.Token);
            }
            else if (_timingManager.IsPaused)
            {
                _timingManager.Resume();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            // ignore errors that might occur during Dispose
            if (!_disposing)
            {
                OnError(ex, AnimationErrorKind.Rendering);
            }
        }
    }

    /// <summary>Rewinds this instance.</summary>
    public async void Rewind()
    {
        CurrentFrameIndex = 0;
        var isStopped = _timingManager.IsPaused || _timingManager.IsComplete;
        _timingManager.Reset();
        if (!isStopped)
        {
            return;
        }

        _timingManager.Pause();
        _isStarted = false;
        try
        {
            await RenderFrameAsync(0, CancellationToken.None);
        }
        catch (Exception ex)
        {
            OnError(ex, AnimationErrorKind.Rendering);
        }
    }

    /// <summary>Converts to string.</summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var s = _sourceUri?.ToString() ?? _sourceStream.ToString();
        return "GIF: " + s;
    }

    /// <summary>Provides the CreateAsyncCore member.</summary>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="progress">The progress value.</param>
    /// <param name="create">The create value.</param>
    /// <returns>The result.</returns>
    /// <typeparam name="TAnimator">The type.</typeparam>
    internal static async Task<TAnimator> CreateAsyncCore<TAnimator>(
        Uri sourceUri,
        IProgress<int>? progress,
        Func<Stream, GifDataStream, TAnimator> create)
        where TAnimator : Animator
    {
        var stream = await UriLoader.GetStreamFromUriAsync(sourceUri, progress);
        try
        {
            // ReSharper disable once AccessToDisposedClosure
            return await CreateAsyncCore(stream, metadata => create(stream, metadata));
        }
        catch
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            await stream.DisposeAsync().ConfigureAwait(false);
#else
            DisposeStream(stream);
#endif
            throw;
        }
    }

    /// <summary>Provides the CreateAsyncCore member.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="create">The create value.</param>
    /// <returns>The result.</returns>
    /// <typeparam name="TAnimator">The type.</typeparam>
    internal static async Task<TAnimator> CreateAsyncCore<TAnimator>(
        Stream sourceStream,
        Func<GifDataStream, TAnimator> create)
        where TAnimator : Animator
    {
        if (!sourceStream.CanSeek)
        {
            throw new ArgumentException("The stream is not seekable");
        }

        _ = sourceStream.Seek(0, SeekOrigin.Begin);
        var metadata = await GifDataStream.ReadAsync(sourceStream);
        return create(metadata);
    }

    /// <summary>Provides the OnRepeatBehaviorChanged member.</summary>
    internal void OnRepeatBehaviorChanged()
    {
        if (_timingManager is null)
        {
            return;
        }

        var newValue = GetSpecifiedRepeatBehavior();
        var newActualValue = GetActualRepeatBehavior(_metadata, newValue);
        if (_timingManager.RepeatBehavior == newActualValue)
        {
            return;
        }

        _timingManager.RepeatBehavior = newActualValue;
        Rewind();
    }

    /// <summary>Provides the ShowFirstFrameAsync member.</summary>
    /// <returns>The result.</returns>
    internal async Task ShowFirstFrameAsync()
    {
        try
        {
            if (_loadFramesDataTask is not null)
            {
                await _loadFramesDataTask;
            }

            await RenderFrameAsync(0, CancellationToken.None);
            CurrentFrameIndex = 0;
            _timingManager.Pause();
        }
        catch (Exception ex)
        {
            OnError(ex, AnimationErrorKind.Rendering);
        }
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposing = true;
        if (_timingManager is not null)
        {
            _timingManager.Completed -= TimingManagerCompleted;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        if (_isSourceStreamOwner)
        {
            try
            {
                _sourceStream?.Dispose();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        _disposed = true;
    }

    /// <summary>Gets the specified repeat behavior.</summary>
    /// <returns>A RepeatBehavior.</returns>
    protected abstract RepeatBehavior GetSpecifiedRepeatBehavior();

    /// <summary>Called when [animation completed].</summary>
    protected virtual void OnAnimationCompleted() => AnimationCompleted?.Invoke(this, new AnimationCompletedEventArgs(AnimationSource));

    /// <summary>Called when [animation started].</summary>
    protected virtual void OnAnimationStarted() => AnimationStarted?.Invoke(this, new AnimationStartedEventArgs(AnimationSource));

    /// <summary>Called when [current frame changed].</summary>
    protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>Called when [error].</summary>
    /// <param name="ex">The ex.</param>
    /// <param name="kind">The kind.</param>
    protected virtual void OnError(Exception ex, AnimationErrorKind kind) => Error?.Invoke(this, new AnimationErrorEventArgs(AnimationSource, ex, kind));

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
    private static void CopyFromBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) => Marshal.Copy(bitmap.BackBuffer + offset, buffer, 0, length);

    /// <summary>Provides the CopyToBitmap member.</summary>
    /// <param name="buffer">The buffer value.</param>
    /// <param name="bitmap">The bitmap value.</param>
    /// <param name="offset">The offset value.</param>
    /// <param name="length">The length value.</param>
    private static void CopyToBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) => Marshal.Copy(buffer, 0, bitmap.BackBuffer + offset, length);

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
            var sizes = metadata.Frames.Zip(metadata.Frames.Skip(1), (f1, f2) =>
                f2.ImageData!.CompressedDataStartOffset - f1.ImageData!.CompressedDataStartOffset);
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
            globalColorTable =
                metadata.GlobalColorTable?
                    .Select(gc => Color.FromArgb(0xFF, gc.R, gc.G, gc.B))
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
            ? frame.LocalColorTable?
                .Select(gc => Color.FromArgb(0xFF, gc.R, gc.G, gc.B))
                .ToArray()
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
    private static RepeatBehavior GetActualRepeatBehavior(GifDataStream metadata, RepeatBehavior repeatBehavior) => repeatBehavior == default
        ? GetRepeatBehaviorFromGif(metadata)
        : repeatBehavior;

    /// <summary>Provides the GetFrameDelay member.</summary>
    /// <param name="frame">The frame value.</param>
    /// <returns>The result.</returns>
    private static TimeSpan GetFrameDelay(GifFrame frame)
    {
        var gce = frame.GraphicControl;
        return gce is not null && gce.Delay != 0 ? TimeSpan.FromMilliseconds(gce.Delay) : TimeSpan.FromMilliseconds(DefaultFrameDelayMilliseconds);
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
            (Start: InterlaceFourthPassStart, Step: InterlaceFineStep)
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
                GifFrameDisposalMethod.RestoreBackground => () => ClearArea(GetFixedUpFrameRect(_previousFrame?.Descriptor)),
                GifFrameDisposalMethod.RestorePrevious => RestorePreviousFrame,
                _ => static () => { }
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
            var endPosition = _metadata.Frames.Count == frameIndex + 1
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
            await using var indexDecompressedStream = new LzwDecompressStream(indexCompressedBytes, frame.ImageData!.LzwMinimumCodeSize);
#else
            using var indexDecompressedStream = new LzwDecompressStream(indexCompressedBytes, frame.ImageData!.LzwMinimumCodeSize);
#endif
            _cachedFrameBytes![frameIndex] = new byte[frameDesc!.Width * frameDesc.Height];

            await indexDecompressedStream.ReadAllAsync(_cachedFrameBytes[frameIndex], 0, frameDesc.Width * frameDesc.Height);
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
            var rows = desc!.Interlace
                ? InterlacedRows(rect.Height).ToArray()
                : NormalRows(rect.Height).ToArray();
            var indexBuffer = await GetFrameIndexBufferAsync(frameIndex, frame, desc, indexStream, cancellationToken);

            RenderFrameRows(desc, rect, rows, indexBuffer, lineBuffer, palette, transparencyIndex, bufferLength);

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
    private async Task<byte[]> GetFrameIndexBufferAsync(int frameIndex, GifFrame frame, GifImageDescriptor desc, Stream? indexStream, CancellationToken cancellationToken)
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
    /// <param name="bufferLength">The buffer length.</param>
    private void RenderFrameRows(
        GifImageDescriptor desc,
        Int32Rect rect,
        int[] rows,
        byte[] indexBuffer,
        byte[] lineBuffer,
        GifPalette palette,
        int transparencyIndex,
        int bufferLength)
    {
        for (var y = 0; y < rect.Height; y++)
        {
            var offset = ((desc.Top + rows[y]) * _stride) + (desc.Left * BgraBytesPerPixel);

            if (transparencyIndex >= 0)
            {
                CopyFromBitmap(lineBuffer, _bitmap, offset, bufferLength);
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

            CopyToBitmap(lineBuffer, _bitmap, offset, bufferLength);
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
