// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

#if REACTIVE_SHIM
using UiScheduler = ReactiveUI.Primitives.Reactive.Concurrency.AvaloniaScheduler;
#else
using UiScheduler = ReactiveUI.Primitives.Concurrency.AvaloniaScheduler;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia;
#else
namespace CrissCross.Avalonia;
#endif

/// <summary>Displays <see cref="ContentControl.Content" /> according to an <see cref="IDataTemplate" />.</summary>
public class ReactiveTransitioningContentControl : ContentControl, IDisposable
{
    /// <summary>The animation timer interval in milliseconds.</summary>
    private const double AnimationIntervalMilliseconds = 10D;

    /// <summary>The number of opacity updates in one transition.</summary>
    private const int AnimationStepCount = 13;

    /// <summary>Owns the current transition and cancels it when newer content arrives.</summary>
    private readonly SerialDisposable _animationSubscription = new();

    /// <summary>The primary template presenter.</summary>
    private ContentPresenter? _contentPresenter1;

    /// <summary>The secondary template presenter.</summary>
    private ContentPresenter? _contentPresenter2;

    /// <summary>Identifies the currently visible presenter.</summary>
    private bool _isPrimaryVisible;

    /// <summary>Gets a value indicating whether this control has been disposed.</summary>
    public bool IsDisposed => _animationSubscription.IsDisposed;

    /// <summary>Releases resources used by this instance.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the active transition subscription.</summary>
    /// <param name="disposing">Whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _animationSubscription.Dispose();
    }

    /// <inheritdoc />
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (base.RegisterContentPresenter(presenter))
        {
            _contentPresenter1 = presenter;
            return true;
        }

        if (presenter.Name != "PART_ContentPresenter2")
        {
            return false;
        }

        _contentPresenter2 = presenter;
        return true;
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateContent(withTransition: false);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateContent(withTransition: false);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _animationSubscription.Disposable = null;
        base.OnDetachedFromVisualTree(e);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != ContentProperty)
        {
            return;
        }

        UpdateContent(withTransition: true);
    }

    /// <summary>Displays the latest content without blocking the UI thread on a preceding animation.</summary>
    /// <param name="withTransition">Whether the incoming content should fade in.</param>
    private void UpdateContent(bool withTransition)
    {
        if (IsDisposed || VisualRoot is null || _contentPresenter1 is null || _contentPresenter2 is null)
        {
            return;
        }

        _animationSubscription.Disposable = null;
        var from = _isPrimaryVisible ? _contentPresenter1 : _contentPresenter2;
        var to = _isPrimaryVisible ? _contentPresenter2 : _contentPresenter1;
        from.Content = null;
        from.IsVisible = false;
        to.Content = null;
        to.Content = Content;
        to.Opacity = 1D;
        to.IsVisible = true;
        _isPrimaryVisible = !_isPrimaryVisible;

        if (!withTransition || Content is null)
        {
            return;
        }

        to.Opacity = 0D;
        var opacity = new AnimationOpacityState(to);
        _animationSubscription.Disposable = Observable
            .Interval(TimeSpan.FromMilliseconds(AnimationIntervalMilliseconds))
            .Take(AnimationStepCount)
            .ObserveOn(UiScheduler.Instance)
            .Subscribe(opacity.OnNext);
    }

    /// <summary>Updates the opacity of one incoming content presenter.</summary>
    /// <param name="presenter">The presenter owned by this transition.</param>
    private sealed class AnimationOpacityState(ContentPresenter presenter)
    {
        /// <summary>Advances opacity until the incoming content is fully visible.</summary>
        /// <param name="tick">The zero-based animation tick.</param>
        public void OnNext(long tick) => presenter.Opacity = Math.Min((tick + 1D) / AnimationStepCount, 1D);
    }
}
