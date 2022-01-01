using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Splat;
using System.Linq;

namespace CrissCross.WPF
{
    /// <summary>
    /// View Model Routed View Host.
    /// </summary>
    /// <seealso cref="RoutedViewHost" />
    public class ViewModelRoutedViewHost : RoutedViewHost, IViewModelRoutedViewHost
    {
        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly DependencyProperty CanNavigateBackProperty = DependencyProperty.Register(nameof(CanNavigateBack), typeof(bool), typeof(ViewModelRoutedViewHost), new PropertyMetadata(false));

        /// <summary>
        /// The host name property.
        /// </summary>
        public static readonly DependencyProperty HostNameProperty = DependencyProperty.Register(nameof(HostName), typeof(string), typeof(ViewModelRoutedViewHost), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The navigate back is enabled property.
        /// </summary>
        public static readonly DependencyProperty NavigateBackIsEnabledProperty = DependencyProperty.Register(nameof(NavigateBackIsEnabled), typeof(bool), typeof(ViewModelRoutedViewHost), new PropertyMetadata(true));

        private readonly ISubject<bool> _canNavigateBackSubject = new Subject<bool>();
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
        public string HostName
        {
            get => (string)GetValue(HostNameProperty);
            set => SetValue(HostNameProperty, value);
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
        /// <c>true</c> if [requires setup]; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresSetup => false;

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
                _navigateBack = true;

                // Get the previous View
                var count = NavigationStack.Count - 2;
                _toViewModel = Locator.Current.GetService(NavigationStack[count]!) as IRxObject;

                if ((_currentView as INotifiyNavigation)?.ISetupNavigating == true)
                {
                    ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, HostName, parameter));
                }
                else
                {
                    ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.Back, _lastView, HostName, parameter));
                }
            }

            CanNavigateBack = NavigationStack.Count > 1;
            _canNavigateBackSubject.OnNext(CanNavigateBack);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            Setup();
            base.OnApplyTemplate();
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

            if (!NavigateBackIsEnabled)
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
        /// <exception cref="System.ArgumentNullException">Navigation Host Name not set</exception>
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
                    ViewModelNavigationEventArgs nea = new(__currentViewModel, _toViewModel, _navigateBack ? NavigationType.Back : NavigationType.New, e.View, HostName, e.NavigationParameter);
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

        private void InternalNavigate<T>(string? contract, object? parameter) where T : class, IRxObject
        {
            _toViewModel = Locator.Current.GetService<T>(contract);
            _lastView = _currentView;

            // NOTE: This gets a new instance of the View
            _currentView = ViewLocator?.ResolveView(_toViewModel, contract);

            if ((_currentView as INotifiyNavigation)?.ISetupNavigating == true)
            {
                ViewModelRoutedViewHostMixins.SetWhenNavigating.OnNext(new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter));
            }
            else
            {
                var ea = new ViewModelNavigatingEventArgs(__currentViewModel, _toViewModel, NavigationType.New, _currentView, HostName, parameter);
                ViewModelRoutedViewHostMixins.ResultNavigating[HostName].OnNext(ea);
            }
        }
    }
}