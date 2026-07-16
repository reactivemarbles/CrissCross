// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF;

/// <summary>View Model Routed View Host.</summary>
/// <seealso cref="RoutedViewHost" />
public class ViewModelRoutedViewHost : TransitioningContentControl, IResolvedViewModelRoutedViewHost, IDisposable
{
    /// <summary>The navigate back is enabled property.</summary>
    public static readonly DependencyProperty CanNavigateBackProperty = DependencyProperty.Register(
        nameof(CanNavigateBack),
        typeof(bool?),
        typeof(ViewModelRoutedViewHost),
        new PropertyMetadata(false));

    /// <summary>The host name property.</summary>
    public static readonly DependencyProperty HostNameProperty = DependencyProperty.Register(
        nameof(HostName),
        typeof(string),
        typeof(ViewModelRoutedViewHost),
        new PropertyMetadata(string.Empty));

    /// <summary>The navigate back is enabled property.</summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(ViewModelRoutedViewHost),
        new PropertyMetadata(true));

    /// <summary>The offset from the end of the stack to the previous entry.</summary>
    private const int PreviousEntryOffset = 2;

    /// <summary>Stores the can Navigate Back Subject value.</summary>
    private readonly Signal<bool?> _canNavigateBackSubject = new();

    /// <summary>Stores the current View Model value.</summary>
    private readonly Signal<INotifiyRoutableViewModel> _currentViewModel = new();

    /// <summary>Stores the active View Model value.</summary>
    private IRxObject? _activeViewModel;

    /// <summary>Stores the current View value.</summary>
    private IViewFor? _currentView;

    /// <summary>Stores the last View value.</summary>
    private IViewFor? _lastView;

    /// <summary>Stores the navigate Back value.</summary>
    private bool _navigateBack;

    /// <summary>Stores the reset Stack value.</summary>
    private bool _resetStack;

    /// <summary>Stores the to View Model value.</summary>
    private IRxObject? _toViewModel;

    /// <summary>Stores the disposed value.</summary>
    private bool _disposedValue;

    /// <summary>Initializes a new instance of the <see cref="ViewModelRoutedViewHost"/> class.</summary>
    public ViewModelRoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        ViewLocator = AppLocator.Current.GetService<IViewLocator>();
        _ = CurrentViewModel.Subscribe(vm =>
        {
            if (vm is IRxObject && !_navigateBack)
            {
                _activeViewModel = vm as IRxObject;
                NavigationStack.Add(_activeViewModel?.GetType());
            }

            if (_currentView is not null)
            {
                Content = _currentView;
            }

            CanNavigateBack = NavigationStack?.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
        });
    }

    /// <summary>Gets or sets the view  AppLocator.</summary>
    /// <value>
    /// The view  AppLocator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value><c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.</value>
    public bool? CanNavigateBack
    {
        get => (bool?)GetValue(CanNavigateBackProperty);
        set => SetValue(CanNavigateBackProperty, value);
    }

    /// <summary>Gets the can navigate back observable.</summary>
    /// <value>
    /// The can navigate back observable.
    /// </value>
    public IObservable<bool?> CanNavigateBackObservable => _canNavigateBackSubject;

    /// <summary>Gets the current view model.</summary>
    /// <value>
    /// The current view model.
    /// </value>
    public IObservable<INotifiyRoutableViewModel> CurrentViewModel => _currentViewModel.Publish().RefCount();

    /// <summary>Gets or sets the name of the host.</summary>
    /// <value>
    /// The name of the host.
    /// </value>
    public string HostName
    {
        get => (string)GetValue(HostNameProperty);
        set => SetValue(HostNameProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool? NavigateBackIsEnabled
    {
        get => (bool?)GetValue(NavigateBackIsEnabledProperty);
        set => SetValue(NavigateBackIsEnabledProperty, value);
    }

    /// <summary>Gets the navigation stack.</summary>
    /// <value>
    /// The navigation stack.
    /// </value>
    public ObservableCollection<Type?> NavigationStack { get; } = [];

    /// <summary>Gets a value indicating whether [requires setup].</summary>
    /// <value>
    /// <c>true</c> if [requires setup]; otherwise, <c>false</c>.
    /// </value>
    public bool RequiresSetup => false;

    /// <summary>Clears the history.</summary>
    public void ClearHistory() => NavigationStack.Clear();

    /// <summary>Navigates the ViewModel contract.</summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="request">The typed navigation request.</param>
    public void Navigate<T>(NavigationKeyRequest<T> request)
        where T : class, IRxObject => InternalNavigate<T>(request.Options.Contract, request.Options.Parameter);

    /// <summary>Navigates a view model instance whose type is known at compile time.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    public void Navigate<T>(T viewModel)
        where T : class, IRxObject => InternalNavigate(viewModel, null, null);

    /// <summary>Navigates a view model instance with request options.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="options">The navigation options.</param>
    public void Navigate<T>(T viewModel, NavigationRequestOptions options)
        where T : class, IRxObject => InternalNavigate(viewModel, options.Contract, options.Parameter);

    /// <inheritdoc />
    public void Navigate<T>(T viewModel, string? contract, object? parameter)
        where T : class, IRxObject => InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the ViewModel contract.</summary>
    /// <param name="viewModel">The view model.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void Navigate(IRxObject viewModel) => InternalNavigate(viewModel, (string?)null, null);

    /// <summary>Navigates the supplied view model with request options.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="options">The navigation options.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void Navigate(IRxObject viewModel, NavigationRequestOptions options) =>
        InternalNavigate(viewModel, options.Contract, options.Parameter);

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void Navigate(IRxObject viewModel, string? contract, object? parameter) =>
        InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the resolved ViewModel/View pair.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    public void Navigate(NavigationResolution resolution)
    {
        ThrowHelper.ThrowIfNull(resolution, nameof(resolution));
        InternalNavigate(resolution.ViewModel, resolution.View, resolution.Parameter);
    }

    /// <summary>Navigates and resets.</summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="request">The typed navigation request.</param>
    public void NavigateAndReset<T>(NavigationKeyRequest<T> request)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate<T>(request.Options.Contract, request.Options.Parameter);
    }

    /// <summary>Navigates a view model instance whose type is known at compile time and resets history.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    public void NavigateAndReset<T>(T viewModel)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate(viewModel, (string?)null, null);
    }

    /// <summary>Navigates a view model instance with request options and resets history.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="options">The navigation options.</param>
    public void NavigateAndReset<T>(T viewModel, NavigationRequestOptions options)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate(viewModel, options.Contract, options.Parameter);
    }

    /// <inheritdoc />
    public void NavigateAndReset<T>(T viewModel, string? contract, object? parameter)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate(viewModel, contract, parameter);
    }

    /// <summary>Navigates the and reset.</summary>
    /// <param name="viewModel">The view model.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void NavigateAndReset(IRxObject viewModel)
    {
        _resetStack = true;
        InternalNavigate(viewModel, (string?)null, null);
    }

    /// <summary>Navigates the supplied view model with request options and resets history.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="options">The navigation options.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void NavigateAndReset(IRxObject viewModel, NavigationRequestOptions options)
    {
        _resetStack = true;
        InternalNavigate(viewModel, options.Contract, options.Parameter);
    }

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void NavigateAndReset(IRxObject viewModel, string? contract, object? parameter)
    {
        _resetStack = true;
        InternalNavigate(viewModel, contract, parameter);
    }

    /// <summary>Navigates the resolved ViewModel/View pair and resets history.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    public void NavigateAndReset(NavigationResolution resolution)
    {
        ThrowHelper.ThrowIfNull(resolution, nameof(resolution));
        _resetStack = true;
        InternalNavigate(resolution.ViewModel, resolution.View, resolution.Parameter);
    }

    /// <summary>Navigates back.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack(object? parameter = null)
    {
        if (NavigateBackIsEnabled == true && CanNavigateBack == true && NavigationStack.Count > 1)
        {
            _navigateBack = true;

            // Get the previous View
            var count = NavigationStack.Count - PreviousEntryOffset;
            _toViewModel = AppLocator.Current.GetService(NavigationStack[count]) as IRxObject;

            var ea = new ViewModelNavigatingEventArgs(
                _activeViewModel,
                _toViewModel,
                NavigationType.Back,
                _lastView,
                HostName,
                parameter);
            if (_currentView is INotifiyNavigation { ISetupNavigating: true })
            {
                ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
            }
            else
            {
                ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
            }
        }

        CanNavigateBack = NavigationStack.Count > 1;
        _canNavigateBackSubject.OnNext(CanNavigateBack);
        return _toViewModel;
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        Setup();
        base.OnApplyTemplate();
    }

    /// <summary>Refreshes this instance.</summary>
    public void Refresh()
    {
        // Keep existing view
        if (Content is null && _currentView is not null)
        {
            Content = _currentView;
        }

        if (NavigateBackIsEnabled != false)
        {
            return;
        }

        // cleanup while Navigation Back is disabled
        while (NavigationStack.Count > 1)
        {
            NavigationStack.RemoveAt(0);
        }
    }

    /// <summary>Setups this instance.</summary>
    /// <exception cref="ArgumentNullException">Navigation Host Name not set.</exception>
    public void Setup()
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(HostName);
#else
        if (string.IsNullOrWhiteSpace(HostName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(HostName));
        }
#endif

        // requested should return result here
        _ = ViewModelRoutedViewHostMixins
            .ResultNavigating[HostName]
            .DistinctUntilChanged()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(HandleNavigating);
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes the specified disposing.</summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _canNavigateBackSubject.Dispose();
            _currentViewModel.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>Handles a navigation result.</summary>
    /// <param name="args">The navigation arguments.</param>
    private void HandleNavigating(IViewModelNavigatingEventArgs args)
    {
        var fromView = _currentView as INotifiyNavigation;
        if (fromView?.ISetupNavigating == false || fromView?.ISetupNavigating is null)
        {
            _activeViewModel?.WhenNavigating(args);
        }

        if (!args.Cancel)
        {
            CompleteNavigation(args, fromView);
        }

        CanNavigateBack = NavigationStack.Count > 1;
        _canNavigateBackSubject.OnNext(CanNavigateBack);
        _resetStack = false;
        _navigateBack = false;
    }

    /// <summary>Completes a navigation that was not canceled.</summary>
    /// <param name="args">The navigation arguments.</param>
    /// <param name="fromView">The view being navigated from.</param>
    private void CompleteNavigation(IViewModelNavigatingEventArgs args, INotifiyNavigation? fromView)
    {
        var eventArgs = new ViewModelNavigationEventArgs(
            _activeViewModel,
            _toViewModel,
            _navigateBack ? NavigationType.Back : NavigationType.New,
            args.View,
            HostName,
            args.NavigationParameter);
        var toView = args.View as INotifiyNavigation;
        var currentViewModel = _activeViewModel;
        _toViewModel ??= args.View?.ViewModel as IRxObject;
        var targetViewModel = _toViewModel;

        UpdateNavigationStack();
        NotifyNavigation(eventArgs, toView, fromView, currentViewModel, targetViewModel);
    }

    /// <summary>Notifies views and view models about completed navigation.</summary>
    /// <param name="eventArgs">The completed navigation event.</param>
    /// <param name="toView">The target view.</param>
    /// <param name="fromView">The source view.</param>
    /// <param name="currentViewModel">The source view model.</param>
    /// <param name="targetViewModel">The target view model.</param>
    private void NotifyNavigation(
        ViewModelNavigationEventArgs eventArgs,
        INotifiyNavigation? toView,
        INotifiyNavigation? fromView,
        IRxObject? currentViewModel,
        IRxObject? targetViewModel)
    {
        NavigationNotification.NotifyViews(eventArgs, toView, fromView);
        NotifyNavigatedTo(eventArgs, toView, targetViewModel);
        NavigationNotification.NotifyFrom(eventArgs, fromView, currentViewModel);
    }

    /// <summary>Notifies the target view model when the target view does not handle navigation.</summary>
    /// <param name="eventArgs">The completed navigation event.</param>
    /// <param name="toView">The target view.</param>
    /// <param name="targetViewModel">The target view model.</param>
    private void NotifyNavigatedTo(
        ViewModelNavigationEventArgs eventArgs,
        INotifiyNavigation? toView,
        IRxObject? targetViewModel)
    {
        if (toView?.ISetupNavigatedTo == true)
        {
            return;
        }

        targetViewModel?.WhenNavigatedTo(eventArgs, ViewModelRoutedViewHostMixins.CurrentViewDisposable[HostName]);
    }

    /// <summary>Updates the navigation stack and active view model.</summary>
    private void UpdateNavigationStack()
    {
        if (_navigateBack)
        {
            NavigateBackInStack();
            return;
        }

        if (_toViewModel is not null && _resetStack)
        {
            NavigationStack.Clear();
            _currentViewModel.OnNext(_toViewModel);
            return;
        }

        if (_toViewModel is null || _currentView is null)
        {
            return;
        }

        _currentViewModel.OnNext(_toViewModel);
    }

    /// <summary>Updates the navigation stack after backward navigation.</summary>
    private void NavigateBackInStack()
    {
        NavigationStack.RemoveAt(NavigationStack.Count - 1);
        if (_toViewModel is null)
        {
            return;
        }

        _currentView = ViewLocator?.ResolveView(_toViewModel);
        _currentViewModel.OnNext(_toViewModel);
        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost.Keys.Where(x => x != HostName))
        {
            ViewModelRoutedViewHostMixins.NavigationHost[host].Refresh();
        }
    }

    /// <summary>Runs the internal Navigate operation.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    private void InternalNavigate<T>(string? contract, object? parameter)
        where T : class, IRxObject => InternalNavigate(AppLocator.Current.GetService<T>(contract), contract, parameter);

    /// <summary>Runs typed navigation for a supplied view model instance.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
    private void InternalNavigate<T>(T? viewModel, string? contract, object? parameter)
        where T : class, IRxObject
    {
        _toViewModel = viewModel;
        _lastView = _currentView;

        _currentView = ViewLocator?.ResolveView<T>(contract);
        if (_currentView is not null)
        {
            _currentView.ViewModel = _toViewModel;
        }

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            HostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
        }
    }

    /// <summary>Runs the internal Navigate operation.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The navigation contract.</param>
    /// <param name="parameter">The navigation parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    private void InternalNavigate(IRxObject viewModel, string? contract, object? parameter)
    {
        _toViewModel = viewModel;
        _lastView = _currentView;

        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            HostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
        }
    }

    /// <summary>Runs the internal Navigate operation for an already resolved ViewModel/View pair.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="view">The resolved view.</param>
    /// <param name="parameter">The navigation parameter.</param>
    private void InternalNavigate(IRxObject viewModel, IViewFor view, object? parameter)
    {
        _toViewModel = viewModel;
        _lastView = _currentView;
        _currentView = view;
        _currentView.ViewModel = viewModel;

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            HostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
        }
    }
}
