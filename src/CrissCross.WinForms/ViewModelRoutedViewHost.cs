// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Splat;

namespace CrissCross.WinForms;

/// <summary>
/// ViewModelRoutedViewHost.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="IViewModelRoutedViewHost" />
public partial class ViewModelRoutedViewHost : UserControl, IViewModelRoutedViewHost
{
    private readonly Subject<bool?> _canNavigateBackSubject = new();
    private readonly Subject<INotifiyRoutableViewModel> _currentViewModel = new();
    private IRxObject? __currentViewModel;
    private IViewFor? _currentView;
    private IViewFor? _lastView;
    private bool _navigateBack;
    private bool _resetStack;
    private IRxObject? _toViewModel;
    private Control? _content;

    static ViewModelRoutedViewHost() => _ = RxState.DefaultExceptionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelRoutedViewHost"/> class.
    /// </summary>
    public ViewModelRoutedViewHost()
    {
        InitializeComponent();
        ViewLocator = AppLocator.Current.GetService<IViewLocator>();
        CurrentViewModel.Subscribe(
            vm =>
        {
            if (vm is IRxObject && !_navigateBack)
            {
                __currentViewModel = vm as IRxObject;
                NavigationStack.Add(__currentViewModel?.GetType());
            }

            if (_currentView != null)
            {
                Content = (Control)_currentView;
            }

            CanNavigateBack = NavigationStack?.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
        });
    }

    /// <summary>
    /// Gets or sets the view  AppLocator.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [navigate back is enabled].
    /// </summary>
    /// <value>
    /// <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool? CanNavigateBack { get; set; }

    /// <summary>
    /// Gets the can navigate back observable.
    /// </summary>
    /// <value>
    /// The can navigate back observable.
    /// </value>
    public IObservable<bool?> CanNavigateBackObservable => _canNavigateBackSubject;

    /// <summary>
    /// Gets the current view model.
    /// </summary>
    /// <value>
    /// The current view model.
    /// </value>
    public IObservable<INotifiyRoutableViewModel> CurrentViewModel => _currentViewModel.Publish().RefCount();

    /// <summary>
    /// Gets or sets the name of the host.
    /// </summary>
    /// <value>
    /// The name of the host.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether [navigate back is enabled].
    /// </summary>
    /// <value>
    /// <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public bool? NavigateBackIsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
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

    /// <summary>
    /// Gets the navigation stack.
    /// </summary>
    /// <value>
    /// The navigation stack.
    /// </value>
    public ObservableCollection<Type?> NavigationStack { get; } = [];

    /// <summary>
    /// Gets a value indicating whether [requires setup].
    /// </summary>
    /// <value>
    /// <c>true</c> if [requires setup]; otherwise, <c>false</c>.
    /// </value>
    public bool RequiresSetup => true;

    /// <summary>
    /// Clears the history.
    /// </summary>
    public void ClearHistory() => NavigationStack.Clear();

    /// <summary>
    /// Navigates the ViewModel contract.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void Navigate<T>(string? contract = null, object? parameter = null)
        where T : class, IRxObject => InternalNavigate<T>(contract, parameter);

    /// <summary>
    /// Navigates the ViewModel contract.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null)
        => InternalNavigate(viewModel, contract, parameter);

    /// <summary>
    /// Navigates and resets.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void NavigateAndReset<T>(string? contract = null, object? parameter = null)
        where T : class, IRxObject
    {
        _resetStack = true;
        InternalNavigate<T>(contract, parameter);
    }

    /// <summary>
    /// Navigates the and reset.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="contract">The contract.</param>
    /// <param name="parameter">The parameter.</param>
    public void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null)
    {
        _resetStack = true;
        InternalNavigate(viewModel, contract, parameter);
    }

    /// <summary>
    /// Navigates back.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The target ViewModel.</returns>
    public IRxObject? NavigateBack(object? parameter = null)
    {
        if (NavigateBackIsEnabled == true && CanNavigateBack == true && NavigationStack.Count > 1)
        {
            _navigateBack = true;

            // Get the previous View
            var count = NavigationStack.Count - 2;
            _toViewModel = AppLocator.Current.GetService(NavigationStack[count]) as IRxObject;

            var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, HostName, parameter);
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

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    void IViewModelRoutedViewHost.Refresh()
    {
        // Keep existing view
        if (Content == null && _currentView != null)
        {
            Content = (Control)_currentView;
        }

        if (NavigateBackIsEnabled == false)
        {
            // cleanup while Navigation Back is disabled
            while (NavigationStack.Count > 1)
            {
                NavigationStack.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Setups this instance.
    /// </summary>
    /// <exception cref="ArgumentNullException">Navigation Host Name not set.</exception>
    public void Setup()
    {
        if (string.IsNullOrWhiteSpace(HostName))
        {
            throw new ArgumentNullException(HostName, "Navigation Host Name not set");
        }

        // requested should return result here
        ViewModelRoutedViewHostMixins.ResultNavigating[HostName].DistinctUntilChanged().ObserveOn(RxSchedulers.MainThreadScheduler).Subscribe(e =>
        {
            var fromView = _currentView as INotifiyNavigation;
            if (fromView?.ISetupNavigating == false || fromView?.ISetupNavigating == null)
            {
                // No view is setup for reciving navigation notifications.
                __currentViewModel?.WhenNavigating(e);
            }

            if (!e.Cancel)
            {
                var nea = new ViewModelNavigationEventArgs(__currentViewModel, _toViewModel, _navigateBack ? NavigationType.Back : NavigationType.New, e.View, HostName, e.NavigationParameter);
                var toView = e.View as INotifiyNavigation;
                var callVmNavTo = toView == null || !toView!.ISetupNavigatedTo;
                var callVmNavFrom = fromView == null || !fromView!.ISetupNavigatedTo;
                var cvm = __currentViewModel;
                _toViewModel ??= e.View?.ViewModel as IRxObject;
                var tvm = _toViewModel;

                if (_navigateBack)
                {
                    // Remove the current
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                    if (_toViewModel != null)
                    {
                        _currentView = ViewLocator?.ResolveView(_toViewModel);
                        _currentViewModel.OnNext(_toViewModel);
                        foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost.Where(x => x.Key != HostName).Select(x => x.Key))
                        {
                            ViewModelRoutedViewHostMixins.NavigationHost[host].Refresh();
                        }
                    }
                }
                else if (_toViewModel != null && _resetStack)
                {
                    NavigationStack.Clear();
                    _currentViewModel.OnNext(_toViewModel);
                }
                else if (_toViewModel != null && _currentView != null)
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

    private void InternalNavigate<T>(string? contract, object? parameter)
        where T : class, IRxObject
    {
        _toViewModel = AppLocator.Current.GetService<T>(contract);
        _lastView = _currentView;

        // NOTE: This gets a new instance of the View
        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
        if (_currentView is INotifiyNavigation { ISetupNavigating: true })
        {
            ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
        }
        else
        {
            ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
        }
    }

    private void InternalNavigate(IRxObject viewModel, string? contract, object? parameter)
    {
        _toViewModel = viewModel;
        _lastView = _currentView;

        // NOTE: This gets a new instance of the View
        _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

        var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
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
