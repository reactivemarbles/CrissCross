// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CrissCross.WPF.UI.Controls.Decoding;
using CrissCross.WPF.UI.Controls.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// AnimationBehavior.
/// </summary>
public static class AnimationBehavior
{
    /// <summary>
    /// The animate in design mode property.
    /// </summary>
    public static readonly DependencyProperty AnimateInDesignModeProperty =
        DependencyProperty.RegisterAttached(
            "AnimateInDesignMode",
            typeof(bool),
            typeof(AnimationBehavior),
            new PropertyMetadata(
                false,
                AnimateInDesignModeChanged));

    /// <summary>
    /// The animation completed event.
    /// </summary>
    public static readonly RoutedEvent AnimationCompletedEvent =
        EventManager.RegisterRoutedEvent(
            "AnimationCompleted",
            RoutingStrategy.Bubble,
            typeof(AnimationCompletedEventHandler),
            typeof(AnimationBehavior));

    /// <summary>
    /// The animation started event.
    /// </summary>
    public static readonly RoutedEvent AnimationStartedEvent =
        EventManager.RegisterRoutedEvent(
            "AnimationStarted",
            RoutingStrategy.Bubble,
            typeof(AnimationStartedEventHandler),
            typeof(AnimationBehavior));

    /// <summary>
    /// The animator property.
    /// </summary>
    public static readonly DependencyProperty AnimatorProperty =
        DependencyProperty.RegisterAttached(
            "Animator",
            typeof(Animator),
            typeof(AnimationBehavior),
            new PropertyMetadata(null));

    /// <summary>
    /// The automatic start property.
    /// </summary>
    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.RegisterAttached(
            "AutoStart",
            typeof(bool),
            typeof(AnimationBehavior),
            new PropertyMetadata(true));

    /// <summary>
    /// The cache frames in memory property.
    /// </summary>
    public static readonly DependencyProperty CacheFramesInMemoryProperty =
        DependencyProperty.RegisterAttached(
        "CacheFramesInMemory",
        typeof(bool),
        typeof(AnimationBehavior),
        new PropertyMetadata(false, SourceChanged));

    /// <summary>
    /// The download progress event.
    /// </summary>
    public static readonly RoutedEvent DownloadProgressEvent =
        EventManager.RegisterRoutedEvent(
            "DownloadProgress",
            RoutingStrategy.Bubble,
            typeof(DownloadProgressEventHandler),
            typeof(AnimationBehavior));

    /// <summary>
    /// The error event.
    /// </summary>
    public static readonly RoutedEvent ErrorEvent =
        EventManager.RegisterRoutedEvent(
            "Error",
            RoutingStrategy.Bubble,
            typeof(AnimationErrorEventHandler),
            typeof(AnimationBehavior));

    /// <summary>
    /// The loaded event.
    /// </summary>
    public static readonly RoutedEvent LoadedEvent =
        EventManager.RegisterRoutedEvent(
            "Loaded",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(AnimationBehavior));

    /// <summary>
    /// The repeat behavior property.
    /// </summary>
    public static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.RegisterAttached(
          "RepeatBehavior",
          typeof(RepeatBehavior),
          typeof(AnimationBehavior),
          new PropertyMetadata(
            default(RepeatBehavior),
            RepeatBehaviorChanged));

    /// <summary>
    /// The source stream property.
    /// </summary>
    public static readonly DependencyProperty SourceStreamProperty =
        DependencyProperty.RegisterAttached(
            "SourceStream",
            typeof(Stream),
            typeof(AnimationBehavior),
            new PropertyMetadata(
                null,
                SourceChanged));

    /// <summary>
    /// The source URI property.
    /// </summary>
    public static readonly DependencyProperty SourceUriProperty =
        DependencyProperty.RegisterAttached(
          "SourceUri",
          typeof(Uri),
          typeof(AnimationBehavior),
          new PropertyMetadata(
            null,
            SourceChanged));

    private static readonly DependencyProperty SeqNumProperty =
        DependencyProperty.RegisterAttached("SeqNum", typeof(int), typeof(AnimationBehavior), new PropertyMetadata(0));

    /// <summary>
    /// Adds the animation completed handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void AddAnimationCompletedHandler(DependencyObject d, AnimationCompletedEventHandler handler) =>
        (d as UIElement)?.AddHandler(AnimationCompletedEvent, handler);

    /// <summary>
    /// Adds the animation started handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void AddAnimationStartedHandler(DependencyObject d, AnimationStartedEventHandler handler) =>
        (d as UIElement)?.AddHandler(AnimationStartedEvent, handler);

    /// <summary>
    /// Adds the download progress handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void AddDownloadProgressHandler(DependencyObject d, DownloadProgressEventHandler handler) =>
        (d as UIElement)?.AddHandler(DownloadProgressEvent, handler);

    /// <summary>
    /// Adds the error handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void AddErrorHandler(DependencyObject d, AnimationErrorEventHandler handler) =>
        (d as UIElement)?.AddHandler(ErrorEvent, handler);

    /// <summary>
    /// Adds the loaded handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void AddLoadedHandler(DependencyObject d, RoutedEventHandler handler) =>
        (d as UIElement)?.AddHandler(LoadedEvent, handler);

    /// <summary>
    /// Gets the animate in design mode.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>A bool.</returns>
    public static bool GetAnimateInDesignMode(DependencyObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (bool)obj.GetValue(AnimateInDesignModeProperty);
    }

    /// <summary>
    /// Gets the animator.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>Animator.</returns>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static Animator GetAnimator(DependencyObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (Animator)obj.GetValue(AnimatorProperty);
    }

    /// <summary>
    /// Gets the automatic start.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>A bool.</returns>
    [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Image))]
    public static bool GetAutoStart(DependencyObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (bool)obj.GetValue(AutoStartProperty);
    }

    /// <summary>
    /// Gets the cache frames in memory.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>A bool.</returns>
    [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Image))]
    public static bool GetCacheFramesInMemory(DependencyObject element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (bool)element.GetValue(CacheFramesInMemoryProperty);
    }

    /// <summary>
    /// Gets the download cache location.
    /// </summary>
    /// <returns>A string.</returns>
    public static string GetDownloadCacheLocation() => UriLoader.DownloadCacheLocation;

    /// <summary>
    /// Gets the repeat behavior.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>RepeatBehavior.</returns>
    [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Image))]
    public static RepeatBehavior GetRepeatBehavior(DependencyObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (RepeatBehavior)obj.GetValue(RepeatBehaviorProperty);
    }

    /// <summary>
    /// Gets the source stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>Stream.</returns>
    [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Image))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static Stream GetSourceStream(DependencyObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (Stream)obj.GetValue(SourceStreamProperty);
    }

    /// <summary>
    /// Gets the source URI.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <returns>Uri.</returns>
    [AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.Image))]
    public static Uri GetSourceUri(System.Windows.Controls.Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        return (Uri)image.GetValue(SourceUriProperty);
    }

    /// <summary>
    /// Removes the animation completed handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveAnimationCompletedHandler(DependencyObject d, AnimationCompletedEventHandler handler) =>
        (d as UIElement)?.RemoveHandler(AnimationCompletedEvent, handler);

    /// <summary>
    /// Removes the animation started handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveAnimationStartedHandler(DependencyObject d, AnimationStartedEventHandler handler) =>
        (d as UIElement)?.RemoveHandler(AnimationStartedEvent, handler);

    /// <summary>
    /// Removes the download progress handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveDownloadProgressHandler(DependencyObject d, DownloadProgressEventHandler handler) =>
        (d as UIElement)?.RemoveHandler(DownloadProgressEvent, handler);

    /// <summary>
    /// Removes the error handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveErrorHandler(DependencyObject d, AnimationErrorEventHandler handler) =>
        (d as UIElement)?.RemoveHandler(ErrorEvent, handler);

    /// <summary>
    /// Removes the loaded handler.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="handler">The handler.</param>
    public static void RemoveLoadedHandler(DependencyObject d, RoutedEventHandler handler) =>
        (d as UIElement)?.RemoveHandler(LoadedEvent, handler);

    /// <summary>
    /// Sets the animate in design mode.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetAnimateInDesignMode(DependencyObject obj, bool value)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(AnimateInDesignModeProperty, value);
    }

    /// <summary>
    /// Sets the automatic start.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetAutoStart(DependencyObject obj, bool value)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(AutoStartProperty, value);
    }

    /// <summary>
    /// Sets the cache frames in memory.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetCacheFramesInMemory(DependencyObject element, bool value)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValue(CacheFramesInMemoryProperty, value);
    }

    /// <summary>
    /// Sets the download cache location.
    /// </summary>
    /// <param name="value">The value.</param>
    public static void SetDownloadCacheLocation(string value) => UriLoader.DownloadCacheLocation = value;

    /// <summary>
    /// Sets the repeat behavior.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetRepeatBehavior(DependencyObject obj, RepeatBehavior value)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(RepeatBehaviorProperty, value);
    }

    /// <summary>
    /// Sets the source stream.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The value.</param>
    public static void SetSourceStream(DependencyObject obj, Stream value)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(SourceStreamProperty, value);
    }

    /// <summary>
    /// Sets the source URI.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="value">The value.</param>
    public static void SetSourceUri(System.Windows.Controls.Image image, Uri value)
    {
        if (value != null)
        {
            throw new ArgumentException("SourceUri can't be set directly. Use SourceStream instead.");
        }

        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        image.SetValue(SourceUriProperty, value);
    }

    internal static void OnDownloadProgress(System.Windows.Controls.Image image, int downloadPercentage) =>
        image.RaiseEvent(new DownloadProgressEventArgs(image, downloadPercentage));

    internal static void OnError(System.Windows.Controls.Image image, Exception exception, AnimationErrorKind kind) =>
        image.RaiseEvent(new AnimationErrorEventArgs(image, exception, kind));

    private static void AnimateInDesignModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not System.Windows.Controls.Image image)
        {
            return;
        }

        InitAnimation(image);
    }

    private static void AnimatorAnimationCompleted(object? sender, AnimationCompletedEventArgs e) =>
        (e.Source as System.Windows.Controls.Image)?.RaiseEvent(e);

    private static void AnimatorAnimationStarted(object? sender, AnimationStartedEventArgs e) =>
        (e.Source as System.Windows.Controls.Image)?.RaiseEvent(e);

    private static void AnimatorError(object? sender, AnimationErrorEventArgs e)
    {
        var source = e.Source as UIElement;
        source?.RaiseEvent(e);
    }

    private static bool CheckDesignMode(System.Windows.Controls.Image image, Uri? sourceUri, Stream? sourceStream)
    {
        if (IsInDesignMode(image) && !GetAnimateInDesignMode(image))
        {
            try
            {
                if (sourceStream != null)
                {
                    SetStaticImage(image, sourceStream);
                }
                else if (sourceUri != null)
                {
                    image.Source = new BitmapImage
                    {
                        UriSource = sourceUri
                    };
                }
            }
            catch
            {
                image.Source = null!;
            }

            return false;
        }

        return true;
    }

    private static void ClearAnimatorCore(System.Windows.Controls.Image image)
    {
        var animator = GetAnimator(image);
        if (animator == null)
        {
            return;
        }

        animator.AnimationCompleted -= AnimatorAnimationCompleted;
        animator.AnimationStarted -= AnimatorAnimationStarted;
        animator.Error -= AnimatorError;
        animator.Dispose();
        SetAnimator(image, null);
    }

    private static Uri? GetAbsoluteUri(System.Windows.Controls.Image image)
    {
        var uri = GetSourceUri(image);
        if (uri == null)
        {
            return null;
        }

        if (!uri.IsAbsoluteUri)
        {
            var baseUri = ((IUriContext)image).BaseUri;
            if (baseUri != null)
            {
                uri = new Uri(baseUri, uri);
            }
            else
            {
                throw new InvalidOperationException("Relative URI can't be resolved");
            }
        }

        return uri;
    }

    private static int GetSeqNum(DependencyObject obj) => (int)obj.GetValue(SeqNumProperty);

    private static void Image_Loaded(object sender, RoutedEventArgs e)
    {
        var image = (System.Windows.Controls.Image)sender;
        image.Loaded -= Image_Loaded;
        InitAnimation(image);
    }

    private static void Image_Unloaded(object sender, RoutedEventArgs e)
    {
        var image = (System.Windows.Controls.Image)sender;
        image.Unloaded -= Image_Unloaded;
        image.Loaded += Image_Loaded;

        var seqNum = GetSeqNum(image) + 1;
        SetSeqNum(image, seqNum);

        image.Source = null!;
        ClearAnimatorCore(image);
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private static void InitAnimation(System.Windows.Controls.Image image)
    {
        if (IsLoaded(image))
        {
            image.Unloaded += Image_Unloaded;
        }
        else
        {
            image.Loaded += Image_Loaded;
            return;
        }

        var seqNum = GetSeqNum(image) + 1;
        SetSeqNum(image, seqNum);

        image.Source = null!;
        ClearAnimatorCore(image);

        try
        {
            var stream = GetSourceStream(image);
            if (stream != null)
            {
                InitAnimationAsync(image, stream.AsBuffered(), GetRepeatBehavior(image), seqNum, GetCacheFramesInMemory(image));
                return;
            }

            var uri = GetAbsoluteUri(image);
            if (uri != null)
            {
                InitAnimationAsync(image, uri, GetRepeatBehavior(image), seqNum, GetCacheFramesInMemory(image));
            }
        }
        catch (Exception ex)
        {
            OnError(image, ex, AnimationErrorKind.Loading);
        }
    }

    private static async void InitAnimationAsync(System.Windows.Controls.Image image, Uri sourceUri, RepeatBehavior repeatBehavior, int seqNum, bool cacheFrameDataInMemory)
    {
        if (!CheckDesignMode(image, sourceUri, null))
        {
            return;
        }

        try
        {
            var progress = new Progress<int>(percentage => OnDownloadProgress(image, percentage));
            var animator = await ImageAnimator.CreateAsync(sourceUri, repeatBehavior, progress, image, cacheFrameDataInMemory);

            // Check that the source hasn't changed while we were loading the animation
            if (GetSeqNum(image) != seqNum)
            {
                animator.Dispose();
                return;
            }

            SetAnimatorCore(image, animator);
            OnLoaded(image);
            await StartAsync(image, animator);
        }
        catch (InvalidSignatureException)
        {
            await SetStaticImageAsync(image, sourceUri);
            OnLoaded(image);
        }
        catch (Exception ex)
        {
            OnError(image, ex, AnimationErrorKind.Loading);
        }
    }

    private static async void InitAnimationAsync(System.Windows.Controls.Image image, Stream stream, RepeatBehavior repeatBehavior, int seqNum, bool cacheFrameDataInMemory)
    {
        if (!CheckDesignMode(image, null, stream))
        {
            return;
        }

        try
        {
            var animator = await ImageAnimator.CreateAsync(stream, repeatBehavior, image, cacheFrameDataInMemory);

            // Check that the source hasn't changed while we were loading the animation
            if (GetSeqNum(image) != seqNum)
            {
                animator.Dispose();
                return;
            }

            SetAnimatorCore(image, animator);
            OnLoaded(image);
            await StartAsync(image, animator);
        }
        catch (InvalidSignatureException)
        {
            SetStaticImage(image, stream);
            OnLoaded(image);
        }
        catch (Exception ex)
        {
            OnError(image, ex, AnimationErrorKind.Loading);
        }
    }

    // ReSharper disable once UnusedParameter.Local (used in WPF)
    private static bool IsInDesignMode(DependencyObject obj) => DesignerProperties.GetIsInDesignMode(obj);

    private static bool IsLoaded(FrameworkElement element) => element.IsLoaded;

    private static void OnLoaded(System.Windows.Controls.Image sender) => sender.RaiseEvent(new RoutedEventArgs(LoadedEvent, sender));

    private static void RepeatBehaviorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) => GetAnimator(o)?.OnRepeatBehaviorChanged();

    private static void SetAnimator(DependencyObject obj, Animator? value) => obj.SetValue(AnimatorProperty, value);

    private static void SetAnimatorCore(System.Windows.Controls.Image image, Animator animator)
    {
        SetAnimator(image, animator);
        animator.Error += AnimatorError;
        animator.AnimationStarted += AnimatorAnimationStarted;
        animator.AnimationCompleted += AnimatorAnimationCompleted;
        image.Source = animator.Bitmap;
    }

    private static void SetSeqNum(DependencyObject obj, int value) => obj.SetValue(SeqNumProperty, value);

    private static void SetStaticImage(System.Windows.Controls.Image image, Stream stream)
    {
        try
        {
            SetStaticImageCore(image, stream);
        }
        catch (Exception ex)
        {
            OnError(image, ex, AnimationErrorKind.Loading);
        }
    }

    private static async Task SetStaticImageAsync(System.Windows.Controls.Image image, Uri sourceUri)
    {
        try
        {
            var progress = new Progress<int>(percentage => OnDownloadProgress(image, percentage));
            using var stream = await UriLoader.GetStreamFromUriAsync(sourceUri, progress);
            SetStaticImageCore(image, stream);
        }
        catch (Exception ex)
        {
            OnError(image, ex, AnimationErrorKind.Loading);
        }
    }

    private static void SetStaticImageCore(System.Windows.Controls.Image image, Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = stream;
        bmp.EndInit();
        image.Source = bmp;
    }

    private static void SourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is not System.Windows.Controls.Image image)
        {
            return;
        }

        InitAnimation(image);
    }

    private static async Task StartAsync(System.Windows.Controls.Image image, Animator animator)
    {
        if (GetAutoStart(image))
        {
            animator.Play();
        }
        else
        {
            await animator.ShowFirstFrameAsync();
        }
    }
}
