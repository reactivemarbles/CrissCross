// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

#if REACTIVE_SHIM
using ReactiveUI.Reactive;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.MAUI;
#else
namespace CrissCross.MAUI;
#endif

/// <summary>Hosts MAUI shell navigation for routed view models.</summary>
public partial class NavigationShell
{
    /// <summary>Sets up shell and routed navigation event handling.</summary>
    /// <exception cref="ArgumentException">The navigation shell name is not set.</exception>
    public void Setup()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        SubscribeToShellNavigationEvents();
        SubscribeToNavigationRequests();
        OnAppearing();
    }

    /// <summary>Subscribes to the MAUI shell navigation events.</summary>
    private void SubscribeToShellNavigationEvents()
    {
        var navigatingEvent = Observable
            .FromEventPattern<ShellNavigatingEventArgs>(
                handler => Navigating += handler,
                handler => Navigating -= handler)
            .Select(pattern => pattern.EventArgs);
        var navigatedEvent = Observable
            .FromEventPattern<ShellNavigatedEventArgs>(handler => Navigated += handler, handler => Navigated -= handler)
            .Select(pattern => pattern.EventArgs);

        _ = navigatingEvent.Subscribe(HandleShellNavigating);
        _ = navigatedEvent.Subscribe(HandleShellNavigated);
    }

    /// <summary>Handles a shell navigating event.</summary>
    /// <param name="eventArgs">The event arguments.</param>
    private void HandleShellNavigating(ShellNavigatingEventArgs eventArgs)
    {
        if (IsBackNavigation(eventArgs.Source) && CanNavigateBack == false)
        {
            _ = eventArgs.Cancel();
        }

        PublishCanNavigateBack();
    }

    /// <summary>Handles a completed shell navigation.</summary>
    /// <param name="eventArgs">The event arguments.</param>
    private void HandleShellNavigated(ShellNavigatedEventArgs eventArgs)
    {
        try
        {
            TraceShellNavigation(eventArgs);
            UpdateHistoryForShellBackNavigation(eventArgs.Source);
            SynchronizeCurrentPage(IsForwardNavigation(eventArgs.Source));
        }
        finally
        {
            _userInstigated = false;
        }
    }

    /// <summary>Updates navigation history after a shell-initiated back navigation.</summary>
    /// <param name="source">The navigation source.</param>
    private void UpdateHistoryForShellBackNavigation(ShellNavigationSource source)
    {
        if (!IsBackNavigation(source) || NavigationStack.Count <= 1 || _userInstigated)
        {
            return;
        }

        NavigationStack.RemoveAt(NavigationStack.Count - 1);
        if (_navigationViews.Count > 1)
        {
            _navigationViews.RemoveAt(_navigationViews.Count - 1);
        }

        PublishCanNavigateBack();
    }

    /// <summary>Synchronizes the routed view model with the current shell page.</summary>
    /// <param name="navigatingForward">Whether the shell is navigating forward.</param>
    private void SynchronizeCurrentPage(bool navigatingForward)
    {
        if (CurrentPage is not IViewFor page || _userInstigated)
        {
            return;
        }

        if (_activeViewModel is not null)
        {
            page.ViewModel ??= _activeViewModel;
        }

        if (!navigatingForward || page.ViewModel is not IRxObject)
        {
            return;
        }

        NavigationStack.Add(page.ViewModel.GetType());
        _navigationViews.Add(page);
    }

    /// <summary>Determines whether a shell navigation source represents forward navigation.</summary>
    /// <param name="source">The navigation source.</param>
    /// <returns><c>true</c> when the source navigates forward; otherwise, <c>false</c>.</returns>
    private bool IsForwardNavigation(ShellNavigationSource source) =>
        !_userInstigated
        && (
            source == ShellNavigationSource.Push
            || source == ShellNavigationSource.Insert
            || source == ShellNavigationSource.ShellItemChanged
            || source == ShellNavigationSource.ShellSectionChanged);

    /// <summary>Subscribes to routed navigation requests for this host.</summary>
    private void SubscribeToNavigationRequests()
    {
        _ = ViewModelRoutedViewHostMixins
            .ResultNavigating[Name]
            .DistinctUntilChanged()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(HandleNavigationRequest);
    }

    /// <summary>Handles a routed navigation request.</summary>
    /// <param name="eventArgs">The navigation event arguments.</param>
    private void HandleNavigationRequest(IViewModelNavigatingEventArgs eventArgs)
    {
        var fromView = _currentView as INotifiyNavigation;
        if (fromView?.ISetupNavigating == false || fromView?.ISetupNavigating is null)
        {
            _activeViewModel?.WhenNavigating(eventArgs);
        }

        if (!eventArgs.Cancel)
        {
            CompleteNavigation(eventArgs, fromView);
        }

        PublishCanNavigateBack();
        _resetStack = false;
    }

    /// <summary>Completes an accepted routed navigation request.</summary>
    /// <param name="eventArgs">The navigation event arguments.</param>
    /// <param name="fromView">The view being navigated from.</param>
    private void CompleteNavigation(IViewModelNavigatingEventArgs eventArgs, INotifiyNavigation? fromView)
    {
        var navigationEvent = new ViewModelNavigationEventArgs(
            _activeViewModel,
            _toViewModel,
            _navigateBack ? NavigationType.Back : NavigationType.New,
            eventArgs.View,
            Name,
            eventArgs.NavigationParameter);
        var toView = eventArgs.View as INotifiyNavigation;
        var callViewModelNavigatedTo = toView?.ISetupNavigatedTo != true;
        var callViewModelNavigatedFrom = fromView?.ISetupNavigatedTo != true;
        var currentViewModel = _activeViewModel;
        _toViewModel ??= eventArgs.View?.ViewModel as IRxObject;
        var targetViewModel = _toViewModel;

        UpdateCurrentNavigationTarget();
        PublishViewNavigationEvent(navigationEvent, toView, fromView);
        NotifyViewModels(
            navigationEvent,
            currentViewModel,
            targetViewModel,
            callViewModelNavigatedTo,
            callViewModelNavigatedFrom);
    }

    /// <summary>Notifies routed view models after navigation completes.</summary>
    /// <param name="navigationEvent">The navigation event.</param>
    /// <param name="currentViewModel">The source view model.</param>
    /// <param name="targetViewModel">The destination view model.</param>
    /// <param name="notifyTarget">Whether to notify the destination view model.</param>
    /// <param name="notifyCurrent">Whether to notify the source view model.</param>
    private void NotifyViewModels(
        IViewModelNavigationEventArgs navigationEvent,
        IRxObject? currentViewModel,
        IRxObject? targetViewModel,
        bool notifyTarget,
        bool notifyCurrent)
    {
        if (notifyTarget)
        {
            targetViewModel?.WhenNavigatedTo(
                navigationEvent,
                ViewModelRoutedViewHostMixins.CurrentViewDisposable[Name]);
        }

        if (!notifyCurrent)
        {
            return;
        }

        currentViewModel?.WhenNavigatedFrom(navigationEvent);
    }

    /// <summary>Updates the current target after an accepted navigation request.</summary>
    private void UpdateCurrentNavigationTarget()
    {
        if (_navigateBack)
        {
            RestorePreviousNavigationTarget();
        }
        else if (_toViewModel is not null && _resetStack)
        {
            NavigationStack.Clear();
            _navigationViews.Clear();
            _currentViewModel.OnNext(_toViewModel);
        }
        else if (_toViewModel is not null && _currentView is not null)
        {
            _currentViewModel.OnNext(_toViewModel);
        }
    }

    /// <summary>Restores the previous routed target during back navigation.</summary>
    private void RestorePreviousNavigationTarget()
    {
        if (_toViewModel is null)
        {
            return;
        }

        var previousIndex = _navigationViews.Count - PreviousViewModelStackOffset;
        _currentView = previousIndex >= 0 ? _navigationViews[previousIndex] : null;
        if (_currentView is not null)
        {
            _currentView.ViewModel = _toViewModel;
        }

        _currentViewModel.OnNext(_toViewModel);
        foreach (
            var hostName in ViewModelRoutedViewHostMixins
                .NavigationHost.Where(pair => pair.Key != Name)
                .Select(pair => pair.Key))
        {
            ViewModelRoutedViewHostMixins.NavigationHost[hostName].Refresh();
        }
    }

    /// <summary>Publishes whether the host can navigate back.</summary>
    private void PublishCanNavigateBack()
    {
        CanNavigateBack = NavigationStack.Count > 1;
        _canNavigateBackSubject.OnNext(CanNavigateBack);
    }
}
