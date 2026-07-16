// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CrissCross.WPF.UI.Controls.Decoding;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents Animator.</summary>
/// <seealso cref="System.Windows.DependencyObject" />
/// <seealso cref="System.IDisposable" />
public abstract partial class Animator : DependencyObject, IDisposable
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
    internal Animator(
        Stream sourceStream,
        Uri? sourceUri,
        GifDataStream metadata,
        RepeatBehavior repeatBehavior,
        bool cacheFrameDataInMemory)
    {
        _sourceStream = sourceStream;
        _sourceUri = sourceUri;
        _isSourceStreamOwner = sourceUri is not null; // stream opened from URI, should close it
        _metadata = metadata;
        _palettes = CreatePalettes(metadata);
        _bitmap = CreateBitmap(metadata);
        var desc = metadata.Header?.LogicalScreenDescriptor;
        _stride =
            BgraBytesPerPixel
            * (((desc!.Width * BitmapStrideAlignmentBits) + BitmapStrideAlignmentMask) / BitmapStrideAlignmentBits);
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

    /// <summary>Provides the Dispose member.</summary>
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
        catch (OperationCanceledException) { }
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
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.</param>
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
    protected virtual void OnAnimationCompleted() =>
        AnimationCompleted?.Invoke(this, new AnimationCompletedEventArgs(AnimationSource));

    /// <summary>Called when [animation started].</summary>
    protected virtual void OnAnimationStarted() =>
        AnimationStarted?.Invoke(this, new AnimationStartedEventArgs(AnimationSource));

    /// <summary>Called when [current frame changed].</summary>
    protected virtual void OnCurrentFrameChanged() => CurrentFrameChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>Called when [error].</summary>
    /// <param name="ex">The ex.</param>
    /// <param name="kind">The kind.</param>
    protected virtual void OnError(Exception ex, AnimationErrorKind kind) =>
        Error?.Invoke(this, new AnimationErrorEventArgs(AnimationSource, ex, kind));
}
