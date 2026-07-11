// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using ReactiveUI;
using Splat;

namespace CrissCross.WinForms;

/// <summary>Hosts routed view model navigation content.</summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IViewModelRoutedViewHost" />
public partial class ViewModelRoutedViewHost : UserControl, IResolvedViewModelRoutedViewHost
{
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

    /// <summary>Stores the content value.</summary>
    private Control? _content;

    /// <summary>Initializes static members of the <see cref="ViewModelRoutedViewHost"/> class.</summary>
    static ViewModelRoutedViewHost() => _ = RxState.DefaultExceptionHandler;

    /// <summary>Initializes a new instance of the <see cref="ViewModelRoutedViewHost"/> class.</summary>
    public ViewModelRoutedViewHost()
    {
        InitializeComponent();
        ViewLocator = AppLocator.Current.GetService<IViewLocator>();
        _ = CurrentViewModel.Subscribe(
            vm =>
        {
            if (vm is IRxObject && !_navigateBack)
            {
                _activeViewModel = vm as IRxObject;
                NavigationStack.Add(_activeViewModel?.GetType());
                _navigationViews.Add(_currentView);
            }

            if (_currentView is not null)
            {
                Content = (Control)_currentView;
            }

            CanNavigateBack = NavigationStack?.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
        });
    }

    /// <summary>Gets or sets the view  AppLocator.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    /// <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool? CanNavigateBack { get; set; }

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
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string HostName { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether [navigate back is enabled].</summary>
    /// <value>
    /// <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool? NavigateBackIsEnabled { get; set; }

    /// <summary>Gets or sets the content.</summary>
    /// <value>
    /// The content.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Control? Content
    {
        get => _content;
        set
        {
            SuspendLayout();
            Controls.Clear();
            _content = value;
            if (_content is not null)
            {
                _content.Dock = DockStyle.Fill;
                Controls.Add(_content);
            }

            ResumeLayout();
        }
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

    /// <summary>Clears the history.</summary>
    public void ClearHistory()
    {
        NavigationStack.Clear();
        _navigationViews.Clear();
    }

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

    /// <summary>Refreshes this instance.</summary>
    void IViewModelRoutedViewHost.Refresh()
    {
        // Keep existing view
        if (Content is null && _currentView is not null)
        {
            Content = (Control)_currentView;
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
                    var currentIndex = NavigationStack.Count - 1;
                    NavigationStack.RemoveAt(currentIndex);
                    if (_navigationViews.Count > currentIndex)
                    {
                        _navigationViews.RemoveAt(currentIndex);
                    }

                    if (_toViewModel is not null)
                    {
                        _currentView = _navigationViews.Count > 0 ? _navigationViews[_navigationViews.Count - 1] : null;
                        if (_currentView is not null)
                        {
                            _currentView.ViewModel = _toViewModel;
                        }

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
                    _navigationViews.Clear();
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

        // NOTE: This gets a new instance of the View
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
