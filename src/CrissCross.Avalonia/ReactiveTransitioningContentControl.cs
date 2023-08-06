// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using ReactiveUI;

namespace CrissCross.Avalonia;

/// <summary>
/// Displays <see cref="ContentControl.Content" /> according to an <see cref="IDataTemplate" />.
/// </summary>
/// <seealso cref="ContentControl" />
/// <seealso cref="ICancelable" />
public class ReactiveTransitioningContentControl : ContentControl, ICancelable
{
    private readonly Subject<double> _opacitySubject = new();
    private readonly SemaphoreSlim _animationSemaphore = new(1);
    private CompositeDisposable _animationDisposable = new();
    private ContentPresenter? _contentPresenter2;
    private ContentPresenter? _contentPresenter1;
    private int _currentPresenter;

    /// <summary>
    /// Gets a value indicating whether gets a value that indicates whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _animationDisposable.IsDisposed;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed && disposing)
        {
            _animationDisposable.Dispose();
            _opacitySubject.Dispose();
            _animationSemaphore.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (!base.RegisterContentPresenter(presenter) &&
            presenter is ContentPresenter p2 &&
            p2.Name == "PART_ContentPresenter2")
        {
            _contentPresenter2 = p2;
            _contentPresenter2.IsVisible = false;
            _contentPresenter1 = Presenter;
            _contentPresenter1!.IsVisible = false;
            return _contentPresenter1 != null;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change?.Property == ContentProperty)
        {
            UpdateContent(true);
        }

        base.OnPropertyChanged(change!);
    }

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
            _animationSemaphore.Release();
        }
    }

    private void AnimateContent()
    {
        // This should be an animation but there is currently an issue with PageTransitions in Avalonia
        _animationDisposable.Dispose();
        _animationDisposable = new();
        var (from, to, current) = GetPresenters();
        to!.Bind(ContentPresenter.OpacityProperty, _opacitySubject).DisposeWith(_animationDisposable);
        var opacity = 0d;
        Observable.Interval(TimeSpan.FromMilliseconds(10)).Subscribe(_ =>
        {
            opacity += 0.08;
            if (opacity > 1d)
            {
                opacity = 1d;
            }

            _opacitySubject.OnNext(opacity);
        }).DisposeWith(_animationDisposable);
        Disposable.Create(() => RxApp.MainThreadScheduler.Schedule(() =>
            {
                to!.Opacity = 1d;
                from!.Opacity = 1d;
                to.IsVisible = true;
                from.IsVisible = false;
                from.Content = null;
                _currentPresenter = current == 1 ? 0 : 1;
                _animationSemaphore.Release();
            })).DisposeWith(_animationDisposable);
        _opacitySubject
            .Where(x => x >= 1d)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                if (!_animationDisposable.IsDisposed)
                {
                    _animationDisposable.Dispose();
                }
            }).DisposeWith(_animationDisposable);
    }

    private (ContentPresenter? from, ContentPresenter? to, int current) GetPresenters()
    {
        var from = _currentPresenter == 1 ? _contentPresenter1 : _contentPresenter2;
        var to = _currentPresenter == 1 ? _contentPresenter2 : _contentPresenter1;
        return (from, to, _currentPresenter);
    }
}
