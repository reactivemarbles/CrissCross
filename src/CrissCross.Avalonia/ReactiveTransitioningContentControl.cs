// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace CrissCross.Avalonia;

/// <summary>Displays <see cref="ContentControl.Content" /> according to an <see cref="IDataTemplate" />.</summary>
/// <seealso cref="ContentControl" />
/// <seealso cref="IDisposable" />
public class ReactiveTransitioningContentControl : ContentControl, IDisposable
{
    /// <summary>The animation timer interval in milliseconds.</summary>
    private const double AnimationIntervalMilliseconds = 10d;

    /// <summary>The opacity increment applied for each animation tick.</summary>
    private const double OpacityIncrement = 0.08d;

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

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
        if (base.RegisterContentPresenter(presenter) ||
            presenter is not ContentPresenter p2 ||
            p2.Name != "PART_ContentPresenter2")
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
                to.Opacity = 0d;
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
        var opacity = 0d;
        _ = Observable.Interval(TimeSpan.FromMilliseconds(AnimationIntervalMilliseconds)).Subscribe(_ =>
        {
            opacity += OpacityIncrement;
            if (opacity > 1d)
            {
                opacity = 1d;
            }

            _opacitySubject.OnNext(opacity);
        }).DisposeWith(_animationDisposable);
        _ = new ActionDisposable(() => RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                to!.Opacity = 1d;
                from!.Opacity = 1d;
                to.IsVisible = true;
                from.IsVisible = false;
                from.Content = null;
                _currentPresenter = current == 1 ? 0 : 1;
                _ = _animationSemaphore.Release();
            })).DisposeWith(_animationDisposable);
        _ = _opacitySubject
            .Where(x => x >= 1d)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (_animationDisposable.IsDisposed)
                {
                    return;
                }

                _animationDisposable.Dispose();
            }).DisposeWith(_animationDisposable);
    }

    /// <summary>Gets the current and next content presenters.</summary>
    /// <returns>The current presenter pair and active index.</returns>
    private (ContentPresenter? from, ContentPresenter? to, int current) GetPresenters()
    {
        var from = _currentPresenter == 1 ? _contentPresenter1 : _contentPresenter2;
        var to = _currentPresenter == 1 ? _contentPresenter2 : _contentPresenter1;
        return (from, to, _currentPresenter);
    }
}
