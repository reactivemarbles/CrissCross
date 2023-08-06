// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia
{
    /// <summary>
    /// ViewModelRoutedViewHost.
    /// </summary>
    public class ViewModelRoutedViewHost : ReactiveTransitioningContentControl, IViewModelRoutedViewHost
    {
        /// <summary>
        /// The can navigate back property.
        /// </summary>
        public static readonly StyledProperty<bool?> CanNavigateBackProperty =
            AvaloniaProperty.Register<ViewModelRoutedViewHost, bool?>(nameof(CanNavigateBack));

        /// <summary>
        /// The host name property.
        /// </summary>
        public static readonly StyledProperty<string?> HostNameProperty =
            AvaloniaProperty.Register<ViewModelRoutedViewHost, string?>(nameof(HostName));

        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly StyledProperty<bool?> NavigateBackIsEnabledProperty =
            AvaloniaProperty.Register<ViewModelRoutedViewHost, bool?>(nameof(NavigateBackIsEnabled));

        private readonly ISubject<bool?> _canNavigateBackSubject = new Subject<bool?>();
        private readonly ISubject<INotifiyRoutableViewModel> _currentViewModel = new Subject<INotifiyRoutableViewModel>();
        private IRxObject? __currentViewModel;
        private IViewFor? _currentView;
        private IViewFor? _lastView;
        private bool _navigateBack;
        private bool _resetStack;
        private IRxObject? _toViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelRoutedViewHost"/> class.
        /// </summary>
        public ViewModelRoutedViewHost()
        {
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            ViewLocator = Locator.Current.GetService<IViewLocator>();
            CurrentViewModel.Subscribe(vm =>
            {
                if (vm is IRxObject && !_navigateBack)
                {
                    __currentViewModel = vm as IRxObject;
                    NavigationStack.Add(__currentViewModel?.GetType());
                }

                if (_currentView != null)
                {
                    Content = _currentView;
                }

                CanNavigateBack = NavigationStack?.Count > 1;
                _canNavigateBackSubject.OnNext(CanNavigateBack);
            });
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator? ViewLocator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value><c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.</value>
        public bool? CanNavigateBack
        {
            get => GetValue(CanNavigateBackProperty)!;
            set => SetValue(CanNavigateBackProperty, value);
        }

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
        public string? HostName
        {
            get => GetValue(HostNameProperty);
            set => SetValue(HostNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the name of the styled element.
        /// </summary>
        /// <remarks>
        /// An element's name is used to uniquely identify an element within the element's name
        /// scope. Once the element is added to a logical tree, its name cannot be changed.
        /// </remarks>
        public new string Name
        {
            get => base.Name!;
            set => base.Name = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [navigate back is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [navigate back is enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool? NavigateBackIsEnabled
        {
            get => GetValue(NavigateBackIsEnabledProperty);
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
        /// <c>true</c> if [requires setup]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresSetup => false;

        /// <summary>
        /// Gets the type by which the element is styled.
        /// </summary>
        /// <remarks>
        /// Usually controls are styled by their own type, but there are instances where you want
        /// an element to be styled by its base type, e.g. creating SpecialButton that
        /// derives from Button and adds extra functionality but is still styled as a regular
        /// Button. Override this property to change the style for a control class, returning the
        /// type that you wish the elements to be styled as.
        /// </remarks>
        protected override Type StyleKeyOverride => typeof(TransitioningContentControl);

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
        public void NavigateBack(object? parameter = null)
        {
            if (NavigateBackIsEnabled == true && CanNavigateBack == true && NavigationStack.Count > 1)
            {
                _navigateBack = true;

                // Get the previous View
                var count = NavigationStack.Count - 2;
                _toViewModel = Locator.Current.GetService(NavigationStack[count]) as IRxObject;

                var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, HostName, parameter);
                if (_currentView is INotifiyNavigation { ISetupNavigating: true })
                {
                    ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(ea);
                }
                else
                {
                    ViewModelRoutedViewHostMixins.ResultNavigating[HostName!].OnNext(ea);
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
            if (Content == null && _currentView != null)
            {
                Content = _currentView;
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
            ViewModelRoutedViewHostMixins.ResultNavigating[HostName].DistinctUntilChanged().ObserveOn(RxApp.MainThreadScheduler).Subscribe(e =>
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
            _toViewModel = Locator.Current.GetService<T>(contract);
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
                ViewModelRoutedViewHostMixins.ResultNavigating[HostName!].OnNext(ea);
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
                ViewModelRoutedViewHostMixins.ResultNavigating[HostName!].OnNext(ea);
            }
        }
    }
}
