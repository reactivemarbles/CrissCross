// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ReactiveUI;
using Splat;

#if REACTIVE_SHIM
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Maui;
#else
using ReactiveUI.Maui;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.MAUI;
#else
namespace CrissCross.MAUI;
#endif

/// <summary>Hosts MAUI shell navigation for routed view models.</summary>
/// <seealso cref="Shell" />
/// <seealso cref="ISetNavigation" />
/// <seealso cref="IViewModelRoutedViewHost" />
/// <seealso cref="IUseNavigation" />
public partial class NavigationShell
    : Shell,
        ISetNavigation,
        IResolvedViewModelRoutedViewHost,
        IUseNavigation,
        IDisposable
{
    /// <summary>The navigate back is enabled property.</summary>
    public static readonly BindableProperty CanNavigateBackProperty = BindableProperty.Create(
        nameof(CanNavigateBack),
        typeof(bool?),
        typeof(NavigationShell),
        false);

    /// <summary>The host name property.</summary>
    public static readonly BindableProperty NameProperty = BindableProperty.Create(
        nameof(Name),
        typeof(string),
        typeof(NavigationShell),
        string.Empty,
        BindingMode.Default,
        propertyChanged: static (bindable, _, _) => NameChanged(bindable));

    /// <summary>The navigate back is enabled property.</summary>
    public static readonly BindableProperty NavigateBackIsEnabledProperty = BindableProperty.Create(
        nameof(NavigateBackIsEnabled),
        typeof(bool?),
        typeof(NavigationShell),
        true);

    /// <summary>Offset from the current navigation stack count to the previous view model entry.</summary>
    private const int PreviousViewModelStackOffset = 2;

    /// <summary>Stores the can Navigate Back Subject value.</summary>
    private readonly Signal<bool?> _canNavigateBackSubject = new();

    /// <summary>Stores resolved views in the same order as the view model navigation stack.</summary>
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

    /// <summary>Stores the user Instigated value.</summary>
    private bool _userInstigated;

    /// <summary>Stores the disposed value.</summary>
    private bool _disposedValue;

    /// <summary>Initializes a new instance of the <see cref="NavigationShell"/> class.</summary>
    public NavigationShell()
    {
        ViewLocator = AppLocator.Current.GetService<IViewLocator>();
        _ = CurrentViewModel
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(vm =>
            {
                if (vm is IRxObject rxo && _userInstigated)
                {
                    _activeViewModel = rxo;
                    if (!_navigateBack)
                    {
                        NavigationStack.Add(_activeViewModel?.GetType());
                        _navigationViews.Add(_currentView);
                    }
                    else if (NavigationStack?.Count > 1)
                    {
                        _ = NavigationStack.Remove(NavigationStack.Last());
                        if (_navigationViews.Count > 1)
                        {
                            _navigationViews.RemoveAt(_navigationViews.Count - 1);
                        }
                    }
                }

                if (_currentView is not null)
                {
                    _ = GotoPageAsync();
                }

                _navigateBack = false;

                CanNavigateBack = NavigationStack?.Count > 1;
                _canNavigateBackSubject.OnNext(CanNavigateBack);
            });
    }

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
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>Gets or sets the host name used for navigation.</summary>
    /// <value>
    /// The host name used for navigation.
    /// </value>
    public string HostName
    {
        get => Name;
        set => Name = value;
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
    ///   <c>true</c> if [requires setup]; otherwise, <c>false</c>.
    /// </value>
    public bool RequiresSetup => true;

    /// <summary>Gets or sets the view  AppLocator.</summary>
    /// <value>
    /// The view  AppLocator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

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

    /// <summary>Navigates the specified contract.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
#if NET8_0_OR_GREATER
    [RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [RequiresUnreferencedCode(
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
#if NET8_0_OR_GREATER
    [RequiresDynamicCode(
        "Resolving a view from a runtime view model instance requires ReactiveUI runtime type inspection.")]
    [RequiresUnreferencedCode(
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
            _userInstigated = true;
            _navigateBack = true;

            // Get the previous View
            var count = NavigationStack.Count;
            var vm = AppLocator.Current.GetService(NavigationStack[count - PreviousViewModelStackOffset]);
            _toViewModel = vm as IRxObject;

            var ea = new ViewModelNavigatingEventArgs(
                _activeViewModel,
                _toViewModel,
                NavigationType.Back,
                _lastView,
                Name,
                parameter);
            if (_currentView is INotifiyNavigation { ISetupNavigating: true })
            {
                ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
            }
            else
            {
                ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(ea);
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
        if (CurrentPage is null && _currentView is not null)
        {
            _ = GotoPageAsync();
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

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Converts to page.</summary>
    /// <param name="item">The item.</param>
    /// <returns>A Page.</returns>
    protected static Page? ToPage(object item) => item as Page;

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

    /// <summary>Runs the name Changed operation.</summary>
    /// <param name="bindable">The bindable object.</param>
    private static void NameChanged(BindableObject bindable)
    {
        if (bindable is not NavigationShell ns)
        {
            return;
        }

        ns.SetMainNavigationHost(ns);
    }

    /// <summary>Runs the goto Page operation.</summary>
    /// <returns>A task that completes when navigation finishes.</returns>
    private async Task GotoPageAsync()
    {
        var page = ToPage(_currentView!);
        var animated = true;
        var attribute = page!.GetType().GetCustomAttribute<DisableAnimationAttribute>();
        if (attribute is not null)
        {
            animated = false;
        }

        if (NavigationStack.Count == 1)
        {
            await Navigation.PopToRootAsync(animated);
        }
        else if (_navigateBack)
        {
            await Navigation.PopAsync(animated);
        }
        else
        {
            await Navigation.PushAsync(page, animated);
        }

        if (
            CurrentPage is not IViewFor p
            || _activeViewModel is null
            || p.ViewModel?.GetType() != _activeViewModel.GetType())
        {
            return;
        }

        // don't replace view model if vm is null or an incompatible type.
        p.ViewModel = _activeViewModel;
    }

    /// <summary>Runs typed navigation for a supplied view model instance.</summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The optional navigation contract.</param>
    /// <param name="parameter">The optional navigation parameter.</param>
    private void InternalNavigate<T>(T? viewModel, string? contract, object? parameter)
        where T : class, IRxObject
    {
        _userInstigated = true;
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
            Name,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(ea);
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
        _userInstigated = true;
        _toViewModel = viewModel;
        _lastView = _currentView;

        // NOTE: This gets a new instance of the View
        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            Name,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(ea);
        }
    }

    /// <summary>Runs the internal Navigate operation for an already resolved ViewModel/View pair.</summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="view">The resolved view.</param>
    /// <param name="parameter">The navigation parameter.</param>
    private void InternalNavigate(IRxObject viewModel, IViewFor view, object? parameter)
    {
        _userInstigated = true;
        _toViewModel = viewModel;
        _lastView = _currentView;
        _currentView = view;
        _currentView.ViewModel = viewModel;

        var ea = new ViewModelNavigatingEventArgs(
            _activeViewModel,
            _toViewModel,
            NavigationType.New,
            _currentView,
            Name,
            parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(ea);
        }
    }
}
