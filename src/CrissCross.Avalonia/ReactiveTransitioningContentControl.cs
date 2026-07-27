// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

#if REACTIVE_SHIM
using ReactiveUI.Reactive;
#else
using ReactiveUI;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia;
#else
namespace CrissCross.Avalonia;
#endif

/// <summary>Displays <see cref="ContentControl.Content" /> according to an <see cref="IDataTemplate" />.</summary>
/// <seealso cref="ContentControl" />
/// <seealso cref="IDisposable" />
public class ReactiveTransitioningContentControl : ContentControl, IDisposable
{
    /// <summary>The animation timer interval in milliseconds.</summary>
    private const double AnimationIntervalMilliseconds = 10D;

    /// <summary>Stores the opacity Subject value.</summary>
    private readonly Signal<double> _opacitySubject = new();

    /// <summary>Stores the animation Semaphore value.</summary>
    private readonly SemaphoreSlim _animationSemaphore = new(1);

    /// <summary>Stores the animation Disposable value.</summary>
    private CompositeDisposable _animationDisposable = [];

    /// <summary>Stores the content Presenter2 value.</summary>
    private ContentPresenter? _contentPresenter2;

    /// <summary>Stores the content Presenter1 value.</summary>
    private ContentPresenter? _contentPresenter1;

    /// <summary>Stores the current Presenter value.</summary>
    private int _currentPresenter;

    /// <summary>Gets a value indicating whether gets a value that indicates whether the object is disposed.</summary>
    public bool IsDisposed => _animationDisposable.IsDisposed;

    /// <summary>Releases resources used by this instance.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed || !disposing)
        {
            return;
        }

        _animationDisposable.Dispose();
        _opacitySubject.Dispose();
        _animationSemaphore.Dispose();
    }

    /// <inheritdoc/>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (
            base.RegisterContentPresenter(presenter)
            || presenter is not ContentPresenter p2
            || p2.Name != "PART_ContentPresenter2")
        {
            return false;
        }

        _contentPresenter2 = p2;
        _contentPresenter2.IsVisible = false;
        _contentPresenter1 = Presenter;
        _contentPresenter1!.IsVisible = false;
        return _contentPresenter1 is not null;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change?.Property != ContentProperty)
        {
            base.OnPropertyChanged(change!);
            return;
        }

        UpdateContent(true);
        base.OnPropertyChanged(change!);
    }

    /// <summary>Runs the update Content operation.</summary>
    /// <param name="withTransition">A value indicating whether the content change should animate.</param>
    private void UpdateContent(bool withTransition)
    {
        var (from, to, current) = GetPresenters();
        if (VisualRoot is null || from is null || to is null)
        {
            return;
        }

        try
        {
            _animationSemaphore.Wait();
            to.Content = Content;
            if (withTransition)
            {
                to.Opacity = 0D;
                to.IsVisible = true;
                from!.IsVisible = false;
                AnimateContent();
            }
            else
            {
                _currentPresenter = current == 1 ? 0 : 1;
                to.IsVisible = true;
                from.Content = null;
                from.IsVisible = false;
            }
        }
        catch
        {
            _ = _animationSemaphore.Release();
        }
    }

    /// <summary>Runs the animate Content operation.</summary>
    private void AnimateContent()
    {
        // This should be an animation but there is currently an issue with PageTransitions in Avalonia
        _animationDisposable.Dispose();
        _animationDisposable = [];
        var (from, to, current) = GetPresenters();
        _ = to!.Bind(OpacityProperty, _opacitySubject).DisposeWith(_animationDisposable);
        var opacity = new AnimationOpacityState(_opacitySubject);
        _ = Observable
            .Interval(TimeSpan.FromMilliseconds(AnimationIntervalMilliseconds))
            .Subscribe(opacity.OnNext)
            .DisposeWith(_animationDisposable);
        _ = new ActionDisposable(new AnimationCompletion(this, from!, to!, current).Schedule)
            .DisposeWith(_animationDisposable);
        _ = _opacitySubject
            .Where(static x => x >= 1D)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (_animationDisposable.IsDisposed)
                {
                    return;
                }

                _animationDisposable.Dispose();
            })
            .DisposeWith(_animationDisposable);
    }

    /// <summary>Gets the current and next content presenters.</summary>
    /// <returns>The current presenter pair and active index.</returns>
    private (ContentPresenter? from, ContentPresenter? to, int current) GetPresenters()
    {
        var from = _currentPresenter == 1 ? _contentPresenter1 : _contentPresenter2;
        var to = _currentPresenter == 1 ? _contentPresenter2 : _contentPresenter1;
        return (from, to, _currentPresenter);
    }

    /// <summary>Maintains the opacity for a single active content transition.</summary>
    /// <param name="opacitySubject">The transition opacity signal.</param>
    private sealed class AnimationOpacityState(Signal<double> opacitySubject)
    {
        /// <summary>The opacity increment applied for each animation tick.</summary>
        private const double OpacityIncrement = 0.08D;

        /// <summary>Stores the current transition opacity.</summary>
        private double _opacity;

        /// <summary>Advances the transition opacity for one timer tick.</summary>
        /// <param name="_">The timer tick value.</param>
        public void OnNext(long _)
        {
            _opacity = Math.Min(_opacity + OpacityIncrement, 1D);
            opacitySubject.OnNext(_opacity);
        }
    }

    /// <summary>Schedules the final UI update for a completed content transition.</summary>
    /// <param name="control">The control that owns the transition.</param>
    /// <param name="from">The outgoing content presenter.</param>
    /// <param name="to">The incoming content presenter.</param>
    /// <param name="current">The outgoing presenter index.</param>
    private sealed class AnimationCompletion(
        ReactiveTransitioningContentControl control,
        ContentPresenter from,
        ContentPresenter to,
        int current)
    {
        /// <summary>Schedules completion on the UI scheduler.</summary>
#if REACTIVE_SHIM
        public void Schedule() =>
            _ = RxSchedulers.MainThreadScheduler.Schedule(
                this,
                static (_, completion) =>
                {
                    completion.Complete();
                    return Disposable.Empty;
                });
#else
        public void Schedule() =>
            _ = RxSchedulers.MainThreadScheduler.Schedule(this, static completion => completion.Complete());
#endif

        /// <summary>Completes the transition on the UI thread.</summary>
        private void Complete()
        {
            to.Opacity = 1D;
            from.Opacity = 1D;
            to.IsVisible = true;
            from.IsVisible = false;
            from.Content = null;
            control._currentPresenter = current == 1 ? 0 : 1;
            _ = control._animationSemaphore.Release();
        }
    }
}
