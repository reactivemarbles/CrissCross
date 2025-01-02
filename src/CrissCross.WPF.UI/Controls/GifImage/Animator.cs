// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CrissCross.WPF.UI.Controls.Decoding;
using CrissCross.WPF.UI.Controls.Decompression;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Animator.
/// </summary>
/// <seealso cref="System.Windows.DependencyObject" />
/// <seealso cref="System.IDisposable" />
public abstract class Animator : DependencyObject, IDisposable
{
    private readonly WriteableBitmap _bitmap;
    private readonly byte[][]? _cachedFrameBytes;
    private readonly bool _cacheFrameDataInMemory;
    private readonly byte[] _indexStreamBuffer;
    private readonly bool _isSourceStreamOwner;
    private readonly Task? _loadFramesDataTask;
    private readonly GifDataStream _metadata;
    private readonly Dictionary<int, GifPalette> _palettes;
    private readonly byte[] _previousBackBuffer;
    private readonly Stream _sourceStream;
    private readonly Uri? _sourceUri;
    private readonly int _stride;
    private readonly TimingManager _timingManager;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed;
    private volatile bool _disposing;
    private int _frameIndex;

    private bool _isStarted;
    private GifFrame? _previousFrame;

    private int _previousFrameIndex;

    internal Animator(Stream sourceStream, Uri? sourceUri, GifDataStream metadata, RepeatBehavior repeatBehavior, bool cacheFrameDataInMemory)
    {
        _sourceStream = sourceStream;
        _sourceUri = sourceUri;
        _isSourceStreamOwner = sourceUri != null; // stream opened from URI, should close it
        _metadata = metadata;
        _palettes = CreatePalettes(metadata);
        _bitmap = CreateBitmap(metadata);
        var desc = metadata.Header?.LogicalScreenDescriptor;
        _stride = 4 * (((desc!.Width * 32) + 31) / 32);
        _previousBackBuffer = new byte[desc.Height * _stride];
        _indexStreamBuffer = CreateIndexStreamBuffer(metadata, _sourceStream);
        _timingManager = CreateTimingManager(metadata, repeatBehavior);

        _cacheFrameDataInMemory = cacheFrameDataInMemory;

        if (cacheFrameDataInMemory)
        {
            _cachedFrameBytes = new byte[_metadata.Frames!.Count][];
            _loadFramesDataTask = Task.Run(LoadFrames);
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Animator"/> class.
    /// </summary>
    ~Animator()
    {
        Dispose(false);
    }

    /// <summary>
    /// Occurs when [animation completed].
    /// </summary>
    public event EventHandler<AnimationCompletedEventArgs>? AnimationCompleted;

    /// <summary>
    /// Occurs when [animation started].
    /// </summary>
    public event EventHandler<AnimationStartedEventArgs>? AnimationStarted;

    /// <summary>
    /// Occurs when [current frame changed].
    /// </summary>
    public event EventHandler? CurrentFrameChanged;

    /// <summary>
    /// Occurs when [error].
    /// </summary>
    public event EventHandler<AnimationErrorEventArgs>? Error;

    /// <summary>
    /// Gets the index of the current frame.
    /// </summary>
    /// <value>
    /// The index of the current frame.
    /// </value>
    public int CurrentFrameIndex
    {
        get => _frameIndex;
        private set
        {
            _frameIndex = value;
            OnCurrentFrameChanged();
        }
    }

    /// <summary>
    /// Gets the frame count.
    /// </summary>
    /// <value>
    /// The frame count.
    /// </value>
    public int FrameCount => _metadata.Frames!.Count;

    /// <summary>
    /// Gets a value indicating whether this instance is complete.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is complete; otherwise, <c>false</c>.
    /// </value>
    public bool IsComplete
    {
        get
        {
            if (_isStarted)
            {
                return _timingManager.IsComplete;
            }

            return false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    public bool IsPaused => _timingManager.IsPaused;

    internal BitmapSource Bitmap => _bitmap;

    /// <summary>
    /// Gets the animation source.
    /// </summary>
    /// <value>
    /// The animation source.
    /// </value>
    protected abstract object AnimationSource { get; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Pauses this instance.
    /// </summary>
    public void Pause() => _timingManager.Pause();

    /// <summary>
    /// Plays this instance.
    /// </summary>
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
                _cancellationTokenSource = new CancellationTokenSource();
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

    /// <summary>
    /// Rewinds this instance.
    /// </summary>
    public async void Rewind()
    {
        CurrentFrameIndex = 0;
        var isStopped = _timingManager.IsPaused || _timingManager.IsComplete;
        _timingManager.Reset();
        if (isStopped)
        {
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
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var s = _sourceUri?.ToString() ?? _sourceStream.ToString();
        return "GIF: " + s;
    }

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
            stream?.Dispose();
            throw;
        }
    }

    internal static async Task<TAnimator> CreateAsyncCore<TAnimator>(
        Stream sourceStream,
        Func<GifDataStream, TAnimator> create)
        where TAnimator : Animator
    {
        if (!sourceStream.CanSeek)
        {
            throw new ArgumentException("The stream is not seekable");
        }

        sourceStream.Seek(0, SeekOrigin.Begin);
        var metadata = await GifDataStream.ReadAsync(sourceStream);
        return create(metadata);
    }

    internal void OnRepeatBehaviorChanged()
    {
        if (_timingManager == null)
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

    internal async Task ShowFirstFrameAsync()
    {
        try
        {
            if (_loadFramesDataTask != null)
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

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposing = true;
            if (_timingManager != null)
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
                catch
                {
                    /* ignored */
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Gets the specified repeat behavior.
    /// </summary>
    /// <returns>A RepeatBehavior.</returns>
    protected abstract RepeatBehavior GetSpecifiedRepeatBehavior();

    /// <summary>
    /// Called when [animation completed].
    /// </summary>
    protected virtual void OnAnimationCompleted() => AnimationCompleted?.Invoke(this, new AnimationCompletedEventArgs(AnimationSource));

    /// <summary>
    /// Called when [animation started].
    /// </summary>
    protected virtual void OnAnimationStarted() => AnimationStarted?.Invoke(this, new AnimationStartedEventArgs(AnimationSource));

    /// <summary>
    /// Called when [current frame changed].
    /// </summary>
    protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Called when [error].
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <param name="kind">The kind.</param>
    protected virtual void OnError(Exception ex, AnimationErrorKind kind) => Error?.Invoke(this, new AnimationErrorEventArgs(AnimationSource, ex, kind));

    private static void CopyFromBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) => Marshal.Copy(bitmap.BackBuffer + offset, buffer, 0, length);

    private static void CopyToBitmap(byte[] buffer, WriteableBitmap bitmap, int offset, int length) => Marshal.Copy(buffer, 0, bitmap.BackBuffer + offset, length);

    private static WriteableBitmap CreateBitmap(GifDataStream metadata)
    {
        var desc = metadata.Header?.LogicalScreenDescriptor;
        return new(desc!.Width, desc.Height, 96, 96, PixelFormats.Bgra32, null);
    }

    private static byte[] CreateIndexStreamBuffer(GifDataStream metadata, Stream stream)
    {
        // Find the size of the largest frame pixel data
        // (ignoring the fact that we include the next frame's header)
        var lastSize = stream.Length - metadata.Frames!.Last().ImageData!.CompressedDataStartOffset;
        var maxSize = lastSize;
        if (metadata.Frames?.Count > 1)
        {
            var sizes = metadata.Frames.Zip(metadata.Frames.Skip(1), (f1, f2) =>
                f2.ImageData!.CompressedDataStartOffset - f1.ImageData!.CompressedDataStartOffset);
            maxSize = Math.Max(sizes.Max(), lastSize);
        }

        // Need 4 extra bytes so that BitReader doesn't need to check the size for every read
        return new byte[maxSize + 4];
    }

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
            var colorTable = globalColorTable;
            if (frame.Descriptor?.HasLocalColorTable == true)
            {
                colorTable =
                    frame.LocalColorTable?
                        .Select(gc => Color.FromArgb(0xFF, gc.R, gc.G, gc.B))
                        .ToArray();
            }

            int? transparencyIndex = null;
            var gce = frame.GraphicControl;
            if (gce is { HasTransparency: true })
            {
                transparencyIndex = gce.TransparencyIndex;
            }

            palettes[i] = new GifPalette(transparencyIndex, colorTable!);
        }

        return palettes;
    }

    private static RepeatBehavior GetActualRepeatBehavior(GifDataStream metadata, RepeatBehavior repeatBehavior) => repeatBehavior == default
                ? GetRepeatBehaviorFromGif(metadata)
                : repeatBehavior;

    private static TimeSpan GetFrameDelay(GifFrame frame)
    {
        var gce = frame.GraphicControl;
        if (gce != null)
        {
            if (gce.Delay != 0)
            {
                return TimeSpan.FromMilliseconds(gce.Delay);
            }
        }

        return TimeSpan.FromMilliseconds(100);
    }

    private static RepeatBehavior GetRepeatBehaviorFromGif(GifDataStream metadata)
    {
        if (metadata.RepeatCount == 0)
        {
            return RepeatBehavior.Forever;
        }

        return new RepeatBehavior(metadata.RepeatCount);
    }

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
            new { Start = 0, Step = 8 },
            new { Start = 4, Step = 8 },
            new { Start = 2, Step = 4 },
            new { Start = 1, Step = 2 }
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

    private static IEnumerable<int> NormalRows(int height) => Enumerable.Range(0, height);

    private static void WriteColor(byte[] lineBuffer, Color color, int startIndex)
    {
        lineBuffer[startIndex] = color.B;
        lineBuffer[startIndex + 1] = color.G;
        lineBuffer[startIndex + 2] = color.R;
        lineBuffer[startIndex + 3] = color.A;
    }

    private void ClearArea(IGifRect? rect) => ClearArea(new Int32Rect(rect!.Left, rect.Top, rect.Width, rect.Height));

    private void ClearArea(Int32Rect rect)
    {
        var bufferLength = 4 * rect.Width;
        var lineBuffer = new byte[bufferLength];
        for (var y = 0; y < rect.Height; y++)
        {
            var offset = ((rect.Y + y) * _stride) + (4 * rect.X);
            CopyToBitmap(lineBuffer, _bitmap, offset, bufferLength);
        }

        _bitmap.AddDirtyRect(new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height));
    }

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

    private void DisposePreviousFrame(GifFrame currentFrame)
    {
        var pgce = _previousFrame?.GraphicControl;
        if (pgce != null)
        {
            switch (pgce.DisposalMethod)
            {
                case GifFrameDisposalMethod.None:
                case GifFrameDisposalMethod.DoNotDispose:
                    {
                        // Leave previous frame in place
                        break;
                    }

                case GifFrameDisposalMethod.RestoreBackground:
                    {
                        ClearArea(GetFixedUpFrameRect(_previousFrame?.Descriptor));
                        break;
                    }

                case GifFrameDisposalMethod.RestorePrevious:
                    {
                        CopyToBitmap(_previousBackBuffer, _bitmap, 0, _previousBackBuffer.Length);
                        var desc = _metadata.Header?.LogicalScreenDescriptor;
                        var rect = new Int32Rect(0, 0, desc!.Width, desc.Height);
                        _bitmap.AddDirtyRect(rect);
                        break;
                    }
            }
        }

        var gce = currentFrame.GraphicControl;
        if (gce is { DisposalMethod: GifFrameDisposalMethod.RestorePrevious })
        {
            CopyFromBitmap(_previousBackBuffer, _bitmap, 0, _previousBackBuffer.Length);
        }
    }

    private Int32Rect GetFixedUpFrameRect(GifImageDescriptor? desc)
    {
        var width = Math.Min(desc!.Width, _bitmap.PixelWidth - desc.Left);
        var height = Math.Min(desc.Height, _bitmap.PixelHeight - desc.Top);
        return new(desc.Left, desc.Top, width, height);
    }

    private async Task GetIndexBytesAsync(int frameIndex, byte[] buffer)
    {
        var startPosition = _metadata.Frames![frameIndex].ImageData!.CompressedDataStartOffset;

        _sourceStream.Seek(startPosition, SeekOrigin.Begin);
        using var memoryStream = new MemoryStream(buffer);
        await GifHelpers.CopyDataBlocksToStreamAsync(_sourceStream, memoryStream).ConfigureAwait(false);
    }

    private async Task<Stream> GetIndexStreamAsync(GifFrame frame, CancellationToken cancellationToken)
    {
        var data = frame.ImageData;
        cancellationToken.ThrowIfCancellationRequested();
        _sourceStream.Seek(data!.CompressedDataStartOffset, SeekOrigin.Begin);
        using (var ms = new MemoryStream(_indexStreamBuffer))
        {
            await GifHelpers.CopyDataBlocksToStreamAsync(_sourceStream, ms, cancellationToken).ConfigureAwait(false);
        }

        return new LzwDecompressStream(_indexStreamBuffer, data.LzwMinimumCodeSize);
    }

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
            using var indexDecompressedStream = new LzwDecompressStream(indexCompressedBytes, frame.ImageData!.LzwMinimumCodeSize);
            _cachedFrameBytes![frameIndex] = new byte[frameDesc!.Width * frameDesc.Height];

            await indexDecompressedStream.ReadAllAsync(_cachedFrameBytes[frameIndex], 0, frameDesc.Width * frameDesc.Height);
        }
    }

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
            if (frameIndex < _previousFrameIndex)
            {
                ClearArea(_metadata.Header?.LogicalScreenDescriptor);
            }
            else
            {
                DisposePreviousFrame(frame);
            }

            var bufferLength = 4 * rect.Width;
            byte[] indexBuffer;
            var lineBuffer = new byte[bufferLength];

            var palette = _palettes[frameIndex];
            var transparencyIndex = palette.TransparencyIndex ?? -1;

            var rows = desc!.Interlace
                ? InterlacedRows(rect.Height).ToArray()
                : NormalRows(rect.Height).ToArray();

            if (!_cacheFrameDataInMemory)
            {
                indexBuffer = new byte[desc.Width * desc.Height];
                await indexStream!.ReadAllAsync(indexBuffer, 0, indexBuffer.Length, cancellationToken);
            }
            else
            {
                indexBuffer = _cachedFrameBytes![frameIndex];
            }

            for (var y = 0; y < rect.Height; y++)
            {
                var offset = ((desc.Top + rows[y]) * _stride) + (desc.Left * 4);

                if (transparencyIndex >= 0)
                {
                    CopyFromBitmap(lineBuffer, _bitmap, offset, bufferLength);
                }

                for (var x = 0; x < rect.Width; x++)
                {
                    var index = indexBuffer[x + (y * desc.Width)];
                    var i = 4 * x;
                    if (index != transparencyIndex)
                    {
                        WriteColor(lineBuffer, palette[index], i);
                    }
                }

                CopyToBitmap(lineBuffer, _bitmap, offset, bufferLength);
            }

            _bitmap.AddDirtyRect(rect);
        }

        _previousFrame = frame;
        _previousFrameIndex = frameIndex;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_loadFramesDataTask != null)
        {
            await _loadFramesDataTask;
        }

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var timing = _timingManager.NextAsync(cancellationToken);
            var rendering = RenderFrameAsync(CurrentFrameIndex, cancellationToken);
            await Task.WhenAll(timing, rendering);
            if (!timing.Result)
            {
                break;
            }

            CurrentFrameIndex = (CurrentFrameIndex + 1) % FrameCount;
        }
    }

    private void TimingManagerCompleted(object? sender, EventArgs e) => OnAnimationCompleted();

    private class GifPalette(int? transparencyIndex, Color[] colors)
    {
        public int? TransparencyIndex { get; } = transparencyIndex;

        public Color this[int i] => colors[i];
    }
}
