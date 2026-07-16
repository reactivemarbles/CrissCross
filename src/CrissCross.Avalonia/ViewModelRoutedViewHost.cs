// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Layout;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia;

/// <summary>Hosts routed view model navigation content.</summary>
public class ViewModelRoutedViewHost : ReactiveTransitioningContentControl, IResolvedViewModelRoutedViewHost
{
    /// <summary>The can navigate back property.</summary>
    public static readonly StyledProperty<bool?> CanNavigateBackProperty = AvaloniaProperty.Register<
        ViewModelRoutedViewHost,
        bool?
    >(nameof(CanNavigateBack));

    /// <summary>The host name property.</summary>
    public static readonly StyledProperty<string?> HostNameProperty = AvaloniaProperty.Register<
        ViewModelRoutedViewHost,
        string?
    >(nameof(HostName));

    /// <summary>The navigate back is enabled property.</summary>
    public static readonly StyledProperty<bool?> NavigateBackIsEnabledProperty = AvaloniaProperty.Register<
        ViewModelRoutedViewHost,
        bool?
    >(nameof(NavigateBackIsEnabled));

    /// <summary>The offset from the end of the stack to the previous entry.</summary>
    private const int PreviousEntryOffset = 2;

    /// <summary>Stores the can Navigate Back Subject value.</summary>
    private readonly Signal<bool?> _canNavigateBackSubject = new();

    /// <summary>Stores the resolved views in the same order as the public navigation stack.</summary>
    private readonly List<IViewFor?> _navigationViews = [];

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
                _navigationViews.Add(_currentView);
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
        get => GetValue(CanNavigateBackProperty)!;
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
        get => GetValue(HostNameProperty) ?? string.Empty;
        set => SetValue(HostNameProperty, value);
    }

    /// <summary>Gets or sets the name of the styled element.</summary>
    /// <remarks>
    /// An element's name is used to uniquely identify an element within the element's name
    /// scope. Once the element is added to a logical tree, its name cannot be changed.
    /// </remarks>
    public new string Name
    {
        get => base.Name!;
        set => base.Name = value;
    }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    public bool? NavigateBackIsEnabled
    {
        get => GetValue(NavigateBackIsEnabledProperty);
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
    public bool RequiresSetup => true;

    /// <summary>Gets the type by which the element is styled.</summary>
    /// <remarks>
    /// Usually controls are styled by their own type, but there are instances where you want
    /// an element to be styled by its base type, e.g. creating SpecialButton that
    /// derives from Button and adds extra functionality but is still styled as a regular
    /// Button. Override this property to change the style for a control class, returning the
    /// type that you wish the elements to be styled as.
    /// </remarks>
    protected override Type StyleKeyOverride => typeof(ReactiveTransitioningContentControl);

    /// <summary>Clears the history.</summary>
    public void ClearHistory()
    {
        NavigationStack.Clear();
        _navigationViews.Clear();
    }

    /// <summary>Navigates a view model instance whose type is known at compile time.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void Navigate<T>(T viewModel, string? contract, object? parameter)
        where T : class, IRxObject => InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the ViewModel contract.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    [RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
    public void Navigate(IRxObject viewModel, string? contract, object? parameter) =>
        InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the resolved ViewModel/View pair.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    public void Navigate(NavigationResolution resolution)
    {
        ThrowHelper.ThrowIfNull(resolution, nameof(resolution));
        InternalNavigate(resolution.ViewModel, resolution.View, resolution.Parameter);
    }

    /// <summary>Navigates a view model instance whose type is known at compile time and resets history.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void NavigateAndReset<T>(T viewModel, string? contract, object? parameter)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate(viewModel, contract, parameter);
    }

    /// <summary>Navigates the and reset.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    [RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
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
            var hostName = ResolveHostName();
            _navigateBack = true;

            // Get the previous View
            var count = NavigationStack.Count - PreviousEntryOffset;
            _toViewModel = AppLocator.Current.GetService(NavigationStack[count]) as IRxObject;

            var ea = new ViewModelNavigatingEventArgs(
                _activeViewModel,
                _toViewModel,
                NavigationType.Back,
                _lastView,
                hostName,
                parameter);
            if (_currentView is INotifiyNavigation { ISetupNavigating: true })
            {
                ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
            }
            else if (ViewModelRoutedViewHostMixins.ResultNavigating.TryGetValue(hostName, out var resultNavigating))
            {
                resultNavigating.OnNext(ea);
            }
        }

        CanNavigateBack = NavigationStack.Count > 1;
        _canNavigateBackSubject.OnNext(CanNavigateBack);
        return _toViewModel;
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
            if (_navigationViews.Count > 1)
            {
                _navigationViews.RemoveAt(0);
            }
        }
    }

    /// <summary>Setups this instance.</summary>
    /// <exception cref="ArgumentNullException">Navigation Host Name not set.</exception>
    public void Setup()
    {
        var hostName = ResolveHostName();

        // requested should return result here
        if (!ViewModelRoutedViewHostMixins.ResultNavigating.TryGetValue(hostName, out var resultNavigating))
        {
            return;
        }

        _ = resultNavigating
            .DistinctUntilChanged()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(e => HandleNavigationResult(e, hostName));
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">Whether managed resources should also be released.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
        {
            return;
        }

        _canNavigateBackSubject.Dispose();
        _currentViewModel.Dispose();
        _navigationViews.Clear();
    }

    /// <summary>Refreshes navigation hosts other than the active host.</summary>
    /// <param name="hostName">The active navigation host name.</param>
    private static void RefreshOtherHosts(string hostName)
    {
        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost.Values.Where(x => x.Name != hostName))
        {
            host.Refresh();
        }
    }

    /// <summary>Completes a navigation result received from the configured host.</summary>
    /// <param name="e">The navigation result.</param>
    /// <param name="hostName">The navigation host name.</param>
    private void HandleNavigationResult(IViewModelNavigatingEventArgs e, string hostName)
    {
        var fromView = _currentView as INotifiyNavigation;
        if (fromView?.ISetupNavigating != true)
        {
            _activeViewModel?.WhenNavigating(e);
        }

        if (!e.Cancel)
        {
            CompleteNavigation(e, fromView, hostName);
        }

        CanNavigateBack = NavigationStack?.Count > 1;
        _canNavigateBackSubject.OnNext(CanNavigateBack);
        _resetStack = false;
        _navigateBack = false;
    }

    /// <summary>Updates the host and sends completed-navigation notifications.</summary>
    /// <param name="e">The navigation result.</param>
    /// <param name="fromView">The view being left.</param>
    /// <param name="hostName">The navigation host name.</param>
    private void CompleteNavigation(IViewModelNavigatingEventArgs e, INotifiyNavigation? fromView, string hostName)
    {
        var navigationEvent = new ViewModelNavigationEventArgs(
            _activeViewModel,
            _toViewModel,
            _navigateBack ? NavigationType.Back : NavigationType.New,
            e.View,
            hostName,
            e.NavigationParameter);
        var toView = e.View as INotifiyNavigation;
        var callViewModelNavigatedTo = toView?.ISetupNavigatedTo != true;
        var callViewModelNavigatedFrom = fromView?.ISetupNavigatedTo != true;
        var previousViewModel = _activeViewModel;
        _toViewModel ??= e.View?.ViewModel as IRxObject;

        UpdateNavigationState(hostName);

        if (toView?.ISetupNavigatedTo == true || fromView?.ISetupNavigatedFrom == true)
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(navigationEvent);
        }

        NotifyViewModels(
            navigationEvent,
            previousViewModel,
            callViewModelNavigatedTo,
            callViewModelNavigatedFrom,
            hostName);
    }

    /// <summary>Updates navigation history and the active view model.</summary>
    /// <param name="hostName">The navigation host name.</param>
    private void UpdateNavigationState(string hostName)
    {
        if (_navigateBack && _toViewModel is not null)
        {
            RemoveCurrentNavigationEntry();
            _currentView = _navigationViews.Count > 0 ? _navigationViews[^1] : null;
            if (_currentView is not null)
            {
                _currentView.ViewModel = _toViewModel;
            }

            _currentViewModel.OnNext(_toViewModel);
            RefreshOtherHosts(hostName);
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

    /// <summary>Removes the active entry from both navigation collections.</summary>
    private void RemoveCurrentNavigationEntry()
    {
        var currentIndex = NavigationStack.Count - 1;
        NavigationStack.RemoveAt(currentIndex);
        if (_navigationViews.Count <= currentIndex)
        {
            return;
        }

        _navigationViews.RemoveAt(currentIndex);
    }

    /// <summary>Sends navigation completion callbacks to view models when views do not handle them.</summary>
    /// <param name="navigationEvent">The completed navigation event.</param>
    /// <param name="previousViewModel">The view model being left.</param>
    /// <param name="callNavigatedTo">Whether the destination view model should be notified.</param>
    /// <param name="callNavigatedFrom">Whether the source view model should be notified.</param>
    /// <param name="hostName">The navigation host name.</param>
    private void NotifyViewModels(
        ViewModelNavigationEventArgs navigationEvent,
        IRxObject? previousViewModel,
        bool callNavigatedTo,
        bool callNavigatedFrom,
        string hostName)
    {
        if (callNavigatedTo)
        {
            if (!ViewModelRoutedViewHostMixins.CurrentViewDisposable.TryGetValue(hostName, out var disposable))
            {
                disposable = [];
                ViewModelRoutedViewHostMixins.CurrentViewDisposable[hostName] = disposable;
            }

            _toViewModel?.WhenNavigatedTo(navigationEvent, disposable);
        }

        if (!callNavigatedFrom)
        {
            return;
        }

        previousViewModel?.WhenNavigatedFrom(navigationEvent);
    }

    /// <summary>Runs typed navigation for a supplied view model instance.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <param name="parameter">The optional navigation parameter.</param>
    private void InternalNavigate<T>(T? viewModel, string? contract, object? parameter)
        where T : class, IRxObject
    {
        _toViewModel = viewModel;
        _lastView = _currentView;
        var hostName = ResolveHostName();

        // The generic locator path is fully AOT-compatible because the view model type is known here.
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
            hostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else if (ViewModelRoutedViewHostMixins.ResultNavigating.TryGetValue(hostName, out var resultNavigating))
        {
            resultNavigating.OnNext(ea);
        }
    }

    /// <summary>Runs the internal Navigate operation.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <param name="parameter">The optional navigation parameter.</param>
    [RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [RequiresUnreferencedCode(
        "Resolving a view from a runtime view model instance may require members removed by trimming.")]
    private void InternalNavigate(IRxObject viewModel, string? contract, object? parameter)
    {
        _toViewModel = viewModel;
        _lastView = _currentView;
        var hostName = ResolveHostName();

        // NOTE: This gets a new instance of the View
        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            hostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else if (ViewModelRoutedViewHostMixins.ResultNavigating.TryGetValue(hostName, out var resultNavigating))
        {
            resultNavigating.OnNext(ea);
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
        var hostName = ResolveHostName();
        _currentView = view;
        _currentView.ViewModel = viewModel;

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            hostName,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else if (ViewModelRoutedViewHostMixins.ResultNavigating.TryGetValue(hostName, out var resultNavigating))
        {
            resultNavigating.OnNext(ea);
        }
    }

    /// <summary>Runs the resolve Host Name operation.</summary>
    /// <returns>The resolved host name.</returns>
    private string ResolveHostName()
    {
        if (string.IsNullOrWhiteSpace(HostName))
        {
            HostName = Name;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(HostName);

        return HostName!;
    }
}
