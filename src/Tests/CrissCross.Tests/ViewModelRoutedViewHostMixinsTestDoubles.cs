// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI;
using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public partial class ViewModelRoutedViewHostMixinsTests
{
    /// <summary>Provides the default host name for the test doubles.</summary>
    private const string TestDoubleHostName = "TestHost";

    /// <summary>Provides the GetUniqueHostName member.</summary>
    /// <returns>The result.</returns>
    private static string GetUniqueHostName() => $"TestHost{Interlocked.Increment(ref _testCounter)}";

    /// <summary>Provides the RegisterNavigationRegistry member.</summary>
    /// <param name="viewModel">The view model value.</param>
    /// <param name="view">The view value.</param>
    /// <param name="contract">The contract value.</param>
    private static void RegisterNavigationRegistry(
        NavigationTargetViewModel viewModel,
        NavigationTargetView view,
        string? contract)
    {
        var registry = new NavigationRegistry();
        _ = registry.Register(
            new NavigationRegistration<
                INavigationTargetViewModel,
                NavigationTargetViewModel,
                INavigationTargetView,
                NavigationTargetView
            >(_ => viewModel, _ => view)
            {
                Contract = contract,
            });

        RegisterNavigationRegistry(registry);
    }

    /// <summary>Provides the RegisterNavigationRegistry member.</summary>
    /// <param name="registry">The registry value.</param>
    private static void RegisterNavigationRegistry(NavigationRegistry registry)
    {
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        AppLocator.CurrentMutable.RegisterConstant(registry.CreateNavigator());
    }

    /// <summary>Provides the RegisterEmptyNavigationRegistry member.</summary>
    private static void RegisterEmptyNavigationRegistry() => RegisterNavigationRegistry(new NavigationRegistry());

    /// <summary>Provides the RegisterTestViewModel member.</summary>
    /// <param name="viewModel">The view model value.</param>
    /// <param name="contract">The contract value.</param>
    private static void RegisterTestViewModel(TestViewModel viewModel, string? contract = null)
    {
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(contract);
        AppLocator.CurrentMutable.RegisterConstant(viewModel, contract);
    }

    /// <summary>Provides the TestViewModel member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestViewModel(string? name = "") : RxObject, IUseNavigation
    {
        string? IUseNavigation.Name => name;
    }

    /// <summary>Provides the TestHostedViewModel member.</summary>
    private sealed class TestHostedViewModel : RxObject;

    /// <summary>Provides the NavigationTargetViewModel member.</summary>
    private sealed class NavigationTargetViewModel : RxObject, INavigationTargetViewModel
    {
        /// <summary>Gets the navigation scope.</summary>
        public string NavigationScope => "NavigationTarget";
    }

    /// <summary>Provides the NavigationTargetView member.</summary>
    private sealed class NavigationTargetView : INavigationTargetView, IViewFor<NavigationTargetViewModel>
    {
        /// <summary>Gets the view scope.</summary>
        public string ViewScope => "NavigationTarget";

        /// <summary>Gets or sets the value.</summary>
        public NavigationTargetViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the value.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NavigationTargetViewModel?)value;
        }
    }

    /// <summary>Provides the TestSetNavigationViewModel member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestSetNavigationViewModel(string? name = TestDoubleHostName) : RxObject, ISetNavigation
    {
        string? ISetNavigation.Name => name;
    }

    /// <summary>Provides the TestViewModelRoutedViewHost member.</summary>
    /// <param name="name">The name value.</param>
    private class TestViewModelRoutedViewHost(string name = TestDoubleHostName) : IViewModelRoutedViewHost
    {
        /// <summary>Provides the _currentViewModel member.</summary>
        private readonly StateSignal<INotifiyRoutableViewModel?> _currentViewModel = new(null);

        /// <summary>Provides the documented member.</summary>
        private readonly StateSignal<bool?> _canNavigateBack = new(false);

        /// <summary>Gets the value.</summary>
        public ObservableCollection<Type?> NavigationStack { get; } = new();

        /// <summary>Gets the value.</summary>
        public IObservable<INotifiyRoutableViewModel> CurrentViewModel => _currentViewModel.Where(x => x is not null)!;

        /// <summary>Gets or sets the value.</summary>
        public bool? CanNavigateBack
        {
            get => _canNavigateBack.Value;
            set => _canNavigateBack.OnNext(value);
        }

        /// <summary>Gets the value.</summary>
        public IObservable<bool?> CanNavigateBackObservable => _canNavigateBack;

        /// <summary>Gets or sets the value.</summary>
        public bool? NavigateBackIsEnabled { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public string Name { get; set; } = name;

        /// <summary>Gets or sets the value.</summary>
        public string HostName
        {
            get => Name;
            set => Name = value;
        }

        /// <summary>Gets the value.</summary>
        public bool RequiresSetup => false;

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Activated => Observable.Return(Unit.Default);

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public IFullLogger? Log { get; set; }

        /// <summary>Gets the value.</summary>
        public Type? LastNavigationType { get; protected set; }

        /// <summary>Gets the value.</summary>
        public IRxObject? LastViewModel { get; protected set; }

        /// <summary>Gets the value.</summary>
        public string? LastContract { get; protected set; }

        /// <summary>Gets the value.</summary>
        public object? LastParameter { get; protected set; }

        /// <summary>Provides the ClearHistory member.</summary>
        public void ClearHistory()
        {
            NavigationStack.Clear();
            CanNavigateBack = false;
        }

        /// <summary>Provides the Setup member.</summary>
        public void Setup()
        {
            // Setup logic
        }

        /// <summary>Provides typed instance navigation.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate<T>(T viewModel, string? contract, object? parameter)
            where T : class, IRxObject => Navigate((IRxObject)viewModel, contract, parameter);

        /// <summary>Provides the Navigate member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate(IRxObject viewModel, string? contract, object? parameter)
        {
            LastNavigationType = viewModel.GetType();
            LastViewModel = viewModel;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;

            if (viewModel is not INotifiyRoutableViewModel routableViewModel)
            {
                return;
            }

            _currentViewModel.OnNext(routableViewModel);
        }

        /// <summary>Provides typed instance navigation with a history reset.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset<T>(T viewModel, string? contract, object? parameter)
            where T : class, IRxObject => NavigateAndReset((IRxObject)viewModel, contract, parameter);

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset(IRxObject viewModel, string? contract, object? parameter)
        {
            LastNavigationType = viewModel.GetType();
            LastViewModel = viewModel;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Clear();
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = false;
        }

        /// <summary>Provides the NavigateBack member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public IRxObject? NavigateBack(object? parameter)
        {
            if (NavigationStack.Count <= 1)
            {
                return null;
            }

            NavigationStack.RemoveAt(NavigationStack.Count - 1);
            CanNavigateBack = NavigationStack.Count > 1;

            return null;
        }

        /// <summary>Provides the Refresh member.</summary>
        public void Refresh()
        {
            // Refresh logic
        }
    }

    /// <summary>Provides the TestResolvedViewModelRoutedViewHost member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestResolvedViewModelRoutedViewHost(string name = TestDoubleHostName)
        : TestViewModelRoutedViewHost(name),
            IResolvedViewModelRoutedViewHost
    {
        /// <summary>Gets the value.</summary>
        public NavigationResolution? LastResolution { get; private set; }

        /// <summary>Provides the Navigate member.</summary>
        /// <param name="resolution">The resolution value.</param>
        public void Navigate(NavigationResolution resolution)
        {
            LastResolution = resolution;
            LastNavigationType = resolution.ViewModel.GetType();
            LastViewModel = resolution.ViewModel;
            LastContract = resolution.Contract;
            LastParameter = resolution.Parameter;
            NavigationStack.Add(resolution.ViewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <param name="resolution">The resolution value.</param>
        public void NavigateAndReset(NavigationResolution resolution)
        {
            LastResolution = resolution;
            LastNavigationType = resolution.ViewModel.GetType();
            LastViewModel = resolution.ViewModel;
            LastContract = resolution.Contract;
            LastParameter = resolution.Parameter;
            NavigationStack.Clear();
            NavigationStack.Add(resolution.ViewModel.GetType());
            CanNavigateBack = false;
        }
    }

    /// <summary>Provides the documented member.</summary>
    /// <param name="viewModel">The view model.</param>
    private sealed class TestView(object? viewModel = null) : INotifiyNavigation, IViewFor
    {
        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigatedTo { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigatedFrom { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigating { get; set; }

        /// <summary>Gets the value.</summary>
        public CompositeDisposable CleanUp { get; } = new();

        /// <summary>Gets the value.</summary>
        public bool IsDisposed => CleanUp.IsDisposed;

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Activated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public object? ViewModel { get; set; } = viewModel;

        /// <summary>Provides the Dispose member.</summary>
        public void Dispose()
        {
            CleanUp?.Dispose();
        }
    }

    /// <summary>Provides the NavigationParameter member.</summary>
    /// <param name="Value">The value.</param>
    private sealed record NavigationParameter(string Value);
}
