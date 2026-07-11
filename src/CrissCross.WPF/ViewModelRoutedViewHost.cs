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
    public static readonly DependencyProperty CanNavigateBackProperty = DependencyProperty.Register(nameof(CanNavigateBack), typeof(bool?), typeof(ViewModelRoutedViewHost), new PropertyMetadata(false));

    /// <summary>The host name property.</summary>
    public static readonly DependencyProperty HostNameProperty = DependencyProperty.Register(nameof(HostName), typeof(string), typeof(ViewModelRoutedViewHost), new PropertyMetadata(string.Empty));

    /// <summary>The navigate back is enabled property.</summary>
    public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(nameof(NavigateBackIsEnabled), typeof(bool?), typeof(ViewModelRoutedViewHost), new PropertyMetadata(true));

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
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void Navigate<T>(string? contract = null, object? parameter = null)
        where T : class, IRxObject => InternalNavigate<T>(contract, parameter);

    /// <summary>Navigates a view model instance whose type is known at compile time.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void Navigate<T>(T viewModel, string? contract = null, object? parameter = null)
        where T : class, IRxObject => InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the ViewModel contract.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null)
        => InternalNavigate(viewModel, contract, parameter);

    /// <summary>Navigates the resolved ViewModel/View pair.</summary>
    /// <param name="resolution">The resolved navigation pair.</param>
    public void Navigate(NavigationResolution resolution)
    {
        ThrowHelper.ThrowIfNull(resolution, nameof(resolution));
        InternalNavigate(resolution.ViewModel, resolution.View, resolution.Parameter);
    }

    /// <summary>Navigates and resets.</summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void NavigateAndReset<T>(string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate<T>(contract, parameter);
    }

    /// <summary>Navigates a view model instance whose type is known at compile time and resets history.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void NavigateAndReset<T>(T viewModel, string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate(viewModel, contract, parameter);
    }

    /// <summary>Navigates the and reset.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    public void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null)
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

            var ea = new ViewModelNavigatingEventArgs(_activeViewModel, _toViewModel, NavigationType.Back, _lastView, HostName, parameter);
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
        _ = ViewModelRoutedViewHostMixins.ResultNavigating[HostName].DistinctUntilChanged().ObserveOn(RxSchedulers.MainThreadScheduler).Subscribe(e =>
        {
            var fromView = _currentView as INotifiyNavigation;
            if (fromView?.ISetupNavigating == false || fromView?.ISetupNavigating is null)
            {
                // No view is setup for reciving navigation notifications.
                _activeViewModel?.WhenNavigating(e);
            }

            if (!e.Cancel)
            {
                var nea = new ViewModelNavigationEventArgs(_activeViewModel, _toViewModel, _navigateBack ? NavigationType.Back : NavigationType.New, e.View, HostName, e.NavigationParameter);
                var toView = e.View as INotifiyNavigation;
                var callVmNavTo = toView is null || !toView!.ISetupNavigatedTo;
                var callVmNavFrom = fromView is null || !fromView!.ISetupNavigatedTo;
                var cvm = _activeViewModel;
                _toViewModel ??= e.View?.ViewModel as IRxObject;
                var tvm = _toViewModel;

                if (_navigateBack)
                {
                    // Remove the current
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    if (_toViewModel is not null)
                    {
                        _currentView = ViewLocator?.ResolveView(_toViewModel);
                        _currentViewModel.OnNext(_toViewModel);
                        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost.Where(x => x.Key != HostName).Select(x => x.Key))
                        {
                            ViewModelRoutedViewHostMixins.NavigationHost[host].Refresh();
                        }
                    }
                }
                else if (_toViewModel is not null && _resetStack)
                {
                    NavigationStack.Clear();
                    _currentViewModel.OnNext(_toViewModel);
                }
                else if (_toViewModel is not null && _currentView is not null)
                {
                    _currentViewModel.OnNext(_toViewModel);
                }

                if (toView?.ISetupNavigatedTo == true || fromView?.ISetupNavigatedFrom == true)
                {
                    ViewModelRoutedViewHostMixins.SetWhenNavigated.OnNext(nea);
                }

                if (callVmNavTo)
                {
                    tvm?.WhenNavigatedTo(nea, ViewModelRoutedViewHostMixins.CurrentViewDisposable[HostName]);
                }

                if (callVmNavFrom)
                {
                    cvm?.WhenNavigatedFrom(nea);
                }
            }

            CanNavigateBack = NavigationStack?.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
            _resetStack = false;
            _navigateBack = false;
        });
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
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

        var ea = new ViewModelNavigatingEventArgs(_activeViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
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
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Resolving a view from a runtime view model instance may require members removed by trimming.")]
#endif
    private void InternalNavigate(IRxObject viewModel, string? contract, object? parameter)
    {
        _toViewModel = viewModel;
        _lastView = _currentView;

        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(_activeViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
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

        var ea = new ViewModelNavigatingEventArgs(_activeViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
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
