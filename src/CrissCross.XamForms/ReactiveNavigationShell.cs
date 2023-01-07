// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace CrissCross
{
    /// <summary>
    /// Reactive Navigation Shell.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="Xamarin.Forms.Shell" />
    /// <seealso cref="CrissCross.ISetNavigation" />
    /// <seealso cref="CrissCross.IViewModelRoutedViewHost" />
    /// <seealso cref="CrissCross.IUseNavigation" />
    public class ReactiveNavigationShell<TViewModel> : ReactiveShell<TViewModel>, ISetNavigation, IViewModelRoutedViewHost, IUseNavigation
        where TViewModel : class, IRxObject
    {
        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly BindableProperty CanNavigateBackProperty = BindableProperty.Create(
            nameof(CanNavigateBack),
            typeof(bool),
            typeof(ReactiveNavigationShell<TViewModel>),
            false);

        /// <summary>
        /// The host name property.
        /// </summary>
        public static readonly BindableProperty NameProperty = BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(ReactiveNavigationShell<TViewModel>),
            string.Empty,
            BindingMode.Default,
            propertyChanged: NameChanged);

        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly BindableProperty NavigateBackIsEnabledProperty = BindableProperty.Create(
            nameof(NavigateBackIsEnabled),
            typeof(bool),
            typeof(ReactiveNavigationShell<TViewModel>),
            true);

        private readonly ISubject<bool> _canNavigateBackSubject = new Subject<bool>();
        private readonly ISubject<INotifiyRoutableViewModel> _currentViewModel = new Subject<INotifiyRoutableViewModel>();
        private IRxObject? __currentViewModel;
        private IViewFor? _currentView;
        private IViewFor? _lastView;
        private bool _navigateBack;
        private bool _popToRootPending;
        private bool _resetStack;
        private IRxObject? _toViewModel;
        private bool _userInstigated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveNavigationShell.<TViewModel>"/> class.
        /// </summary>
        public ReactiveNavigationShell()
        {
            ViewLocator = Locator.Current.GetService<IViewLocator>();
            CurrentViewModel.ObserveOn(RxApp.MainThreadScheduler).Subscribe(vm =>
            {
                if (vm is IRxObject && !_navigateBack)
                {
                    __currentViewModel = vm as IRxObject;
                    NavigationStack.Add(__currentViewModel?.GetType());
                }

                if (_currentView != null)
                {
                    GotoPage();
                }

                CanNavigateBack = NavigationStack?.Count > 1;
                _canNavigateBackSubject.OnNext(CanNavigateBack);
            });
        }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value><c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.</value>
        public bool CanNavigateBack
        {
            get => (bool)GetValue(CanNavigateBackProperty);
            set => SetValue(CanNavigateBackProperty, value);
        }

        /// <summary>
        /// Gets the can navigate back observable.
        /// </summary>
        /// <value>
        /// The can navigate back observable.
        /// </value>
        public IObservable<bool> CanNavigateBackObservable => _canNavigateBackSubject;

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
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool NavigateBackIsEnabled
        {
            get => (bool)GetValue(NavigateBackIsEnabledProperty);
            set => SetValue(NavigateBackIsEnabledProperty, value);
        }

        /// <summary>
        /// Gets the navigation stack.
        /// </summary>
        /// <value>
        /// The navigation stack.
        /// </value>
        public ObservableCollection<Type?> NavigationStack { get; } = new();

        /// <summary>
        /// Gets a value indicating whether [requires setup].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires setup]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresSetup => true;

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }

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
        /// Navigates back.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void NavigateBack(object? parameter = null)
        {
            if (NavigateBackIsEnabled && CanNavigateBack && NavigationStack.Count > 1)
            {
                _userInstigated = true;
                _navigateBack = true;

                // Get the previous View
                var count = NavigationStack.Count;
                var vm = Locator.Current.GetService(NavigationStack[count - 2]!);
                _toViewModel = vm as IRxObject;

                if ((_currentView as INotifiyNavigation)?.ISetupNavigating == true)
                {
                    ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, Name, parameter));
                }
                else
                {
                    ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, Name, parameter));
                }
            }

            CanNavigateBack = NavigationStack.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            // Keep existing view
            if (CurrentPage == null && _currentView != null)
            {
                GotoPage();
            }

            if (!NavigateBackIsEnabled)
            {
                // cleanup while Navigation Back is disabled
                while (NavigationStack.Count > 1)
                {
                    NavigationStack.RemoveAt(0);
                }
            }
        }

        public void Setup()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentNullException(Name, "NavigationShell Name not set");
            }

            var navigatingEvent = Observable.FromEvent<EventHandler<ShellNavigatingEventArgs>, ShellNavigatingEventArgs>(
                   eventHandler =>
                   {
                       void Handler(object? sender, ShellNavigatingEventArgs e) => eventHandler(e);
                       return Handler;
                   },
                   x => Navigating += x,
                   x => Navigating -= x);

            var navigatedEvent = Observable.FromEvent<EventHandler<ShellNavigatedEventArgs>, ShellNavigatedEventArgs>(
                   eventHandler =>
                   {
                       void Handler(object? sender, ShellNavigatedEventArgs e) => eventHandler(e);
                       return Handler;
                   },
                   x => Navigated += x,
                   x => Navigated -= x);

            navigatingEvent.Subscribe(e =>
            {
                if ((e.Source == ShellNavigationSource.Pop || e.Source == ShellNavigationSource.PopToRoot) && !CanNavigateBack)
                {
                    // Cancel navigate back
                    e.Cancel();
                }

                _canNavigateBackSubject.OnNext(CanNavigateBack);
            });

            navigatedEvent
                .Subscribe(e =>
                {
                    var navigatingForward = false;
                    try
                    {
                        var f = e.Current.Location.OriginalString;
                        Debug.WriteLine($"Current {f}");
                        if (e.Previous != null)
                        {
                            var s = e.Previous.Location.OriginalString;
                            Debug.WriteLine($"Previous {s}");
                        }

                        if ((e.Source == ShellNavigationSource.Pop || e.Source == ShellNavigationSource.PopToRoot) && NavigationStack.Count > 1)
                        {
                            // Navigating back
                            if (!_userInstigated)
                            {
                                NavigationStack.RemoveAt(NavigationStack.Count - 1);
                                Debug.WriteLine($"Navigation Stack: {NavigationStack.Count}");
                            }
                        }

                        if (!_userInstigated && (e.Source == ShellNavigationSource.Push || e.Source == ShellNavigationSource.Insert || e.Source == ShellNavigationSource.ShellItemChanged || e.Source == ShellNavigationSource.ShellSectionChanged))
                        {
                            // navigating forward
                            navigatingForward = true;
                        }

                        if (CurrentPage is IViewFor page && !_userInstigated)
                        {
                            // don't replace view model if vm is null
                            var vm = __currentViewModel;
                            if (vm != null)
                            {
                                page.ViewModel ??= vm;
                            }

                            if (navigatingForward && page.ViewModel is IRxObject)
                            {
                                NavigationStack.Add(page.ViewModel.GetType());
                                Debug.WriteLine($"Navigation Stack: {NavigationStack.Count}");
                            }
                        }
                    }
                    finally
                    {
                        _userInstigated = false;
                    }
                });

            // requested should return result here
            ViewModelRoutedViewHostMixins.ResultNavigating[Name].DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(e =>
            {
                var fromView = _currentView as INotifiyNavigation;
                if (fromView?.ISetupNavigating == false || fromView?.ISetupNavigating == null)
                {
                    // No view is setup for reciving navigation notifications.
                    __currentViewModel?.WhenNavigating(e);
                }

                if (!e.Cancel)
                {
                    ViewModelNavigationEventArgs nea = new(__currentViewModel, _toViewModel, _navigateBack ? NavigationType.Back : NavigationType.New, e.View, Name, e.NavigationParameter);
                    var toView = e.View as INotifiyNavigation;
                    var callVmNavTo = toView == null || !toView!.ISetupNavigatedTo;
                    var callVmNavFrom = fromView == null || !fromView!.ISetupNavigatedTo;
                    var cvm = __currentViewModel;
                    _toViewModel ??= e.View?.ViewModel as IRxObject;
                    var tvm = _toViewModel;

                    if (_navigateBack)
                    {
                        _popToRootPending = NavigationStack.Count == 0;
                        if (_toViewModel != null)
                        {
                            _currentView = ViewLocator?.ResolveView(_toViewModel);
                            _currentViewModel.OnNext(_toViewModel);
                            foreach (var host in ViewModelRoutedViewHostMixins.NavigationHost.Where(x => x.Key != Name).Select(x => x.Key))
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
                        tvm?.WhenNavigatedTo(nea, ViewModelRoutedViewHostMixins.CurrentViewDisposable[Name]);
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
            base.OnAppearing();
        }

        /// <summary>
        /// Converts to page.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected static Page? ToPage(object item)
        {
            return item as Page;
        }

        private static void NameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ReactiveNavigationShell<TViewModel> ns)
            {
                ns.SetMainNavigationHost(ns);
            }
        }

        private async void GotoPage()
        {
            var page = ToPage(_currentView!);
            var animated = true;
            var attribute = page!.GetType().GetCustomAttribute<DisableAnimationAttribute>();
            if (attribute is not null)
            {
                animated = false;
            }

            if (_popToRootPending && Navigation.NavigationStack.Count > 0)
            {
                await Navigation.PopToRootAsync(animated);
            }
            else
            {
                await Navigation.PushAsync(page, animated);
            }

            _popToRootPending = false;
            if (CurrentPage is IViewFor p && __currentViewModel is not null)
            {
                // don't replace view model if vm is null
                p.ViewModel = __currentViewModel;
            }
        }

        private void InternalNavigate<T>(string? contract, object? parameter) where T : class, IRxObject
        {
            _userInstigated = true;
            _toViewModel = Locator.Current.GetService<T>(contract);
            _lastView = _currentView;

            // NOTE: This gets a new instance of the View
            _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

            if ((_currentView as INotifiyNavigation)?.ISetupNavigating == true)
            {
                ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, Name, parameter));
            }
            else
            {
                var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, Name, parameter);
                ViewModelRoutedViewHostMixins.ResultNavigating[Name].OnNext(ea);
            }
        }
    }
}