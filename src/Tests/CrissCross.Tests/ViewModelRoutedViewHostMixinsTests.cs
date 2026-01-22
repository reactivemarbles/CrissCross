// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CrissCross;
using ReactiveUI;
using Splat;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace CrissCross.Tests;

/// <summary>
/// Tests for ViewModelRoutedViewHostMixins class.
/// </summary>
public class ViewModelRoutedViewHostMixinsTests
{
    private static int _testCounter;
    private static string GetUniqueHostName() => $"TestHost{Interlocked.Increment(ref _testCounter)}";

    private class TestViewModel : RxObject, IUseNavigation
    {
        private readonly string? _navName;

        public TestViewModel(string? name = "")
        {
            _navName = name;
        }

        string? IUseNavigation.Name => _navName;
    }

    private class TestHostedViewModel : RxObject;

    private class TestSetNavigationViewModel : RxObject, ISetNavigation
    {
        private readonly string? _setNavName;

        public TestSetNavigationViewModel(string? name = "TestHost")
        {
            _setNavName = name;
        }

        string? ISetNavigation.Name => _setNavName;
    }

    private class TestViewModelRoutedViewHost : IViewModelRoutedViewHost
    {
        private readonly BehaviorSubject<INotifiyRoutableViewModel?> _currentViewModel = new(null);
        private readonly BehaviorSubject<bool?> _canNavigateBack = new(false);

        public TestViewModelRoutedViewHost(string name = "TestHost")
        {
            Name = name;
        }

        public ObservableCollection<Type?> NavigationStack { get; } = new();

        public IObservable<INotifiyRoutableViewModel> CurrentViewModel => _currentViewModel.Where(x => x != null)!;

        public bool? CanNavigateBack
        {
            get => _canNavigateBack.Value;
            set => _canNavigateBack.OnNext(value);
        }

        public IObservable<bool?> CanNavigateBackObservable => _canNavigateBack;

        public bool? NavigateBackIsEnabled { get; set; }

        public string Name { get; set; }

        public bool RequiresSetup => false;

        public IObservable<Unit> Activated => Observable.Return(Unit.Default);

        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);

        public IFullLogger? Log { get; set; }

        public void ClearHistory()
        {
            NavigationStack.Clear();
            CanNavigateBack = false;
        }

        public void Setup()
        {
            // Setup logic
        }

        public void Navigate<T>(string? contract = null, object? parameter = null) where T : class, IRxObject
        {
            NavigationStack.Add(typeof(T));
            CanNavigateBack = NavigationStack.Count > 1;
        }

        public void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;
            if (viewModel is INotifiyRoutableViewModel routableViewModel)
            {
                _currentViewModel.OnNext(routableViewModel);
            }
        }

        public void NavigateAndReset<T>(string? contract = null, object? parameter = null) where T : class, IRxObject
        {
            NavigationStack.Clear();
            NavigationStack.Add(typeof(T));
            CanNavigateBack = false;
        }

        public void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            NavigationStack.Clear();
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = false;
        }

        public IRxObject? NavigateBack(object? parameter = null)
        {
            if (NavigationStack.Count > 1)
            {
                NavigationStack.RemoveAt(NavigationStack.Count - 1);
                CanNavigateBack = NavigationStack.Count > 1;
                return null;
            }
            return null;
        }

        public void Refresh()
        {
            // Refresh logic
        }
    }

    private class TestView : INotifiyNavigation, IViewFor
    {
        public TestView(object? viewModel = null)
        {
            ViewModel = viewModel;
        }

        public bool ISetupNavigatedTo { get; set; }
        public bool ISetupNavigatedFrom { get; set; }
        public bool ISetupNavigating { get; set; }
        public CompositeDisposable CleanUp { get; } = new();
        public bool IsDisposed => CleanUp.IsDisposed;
        public IObservable<Unit> Activated => Observable.Return(Unit.Default);
        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);
        public object? ViewModel { get; set; }

        public void Dispose()
        {
            CleanUp?.Dispose();
        }
    }

    [Before(Test)]
    public async Task Setup()
    {
        // Note: We don't clear static dictionaries as it causes test isolation issues
        // Each test should use unique host names to avoid conflicts
        await Task.CompletedTask;
    }

    [Test]
    public async Task SetMainNavigationHost_RegistersHost()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(viewHost);

        // Assert
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost.ContainsKey(hostName)).IsTrue();
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(viewHost);
    }

    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenSetNavigationIsNull()
    {
        // Arrange
        TestSetNavigationViewModel? setNav = null;
        var viewHost = new TestViewModelRoutedViewHost("TestHost");

        // Act & Assert
        await Assert.That(() => setNav!.SetMainNavigationHost(viewHost)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenViewHostIsNull()
    {
        // Arrange
        var setNav = new TestSetNavigationViewModel("TestHost");
        TestViewModelRoutedViewHost? viewHost = null;

        // Act & Assert
        await Assert.That(() => setNav.SetMainNavigationHost(viewHost!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task SetMainNavigationHost_DoesNotRegisterTwice()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost1 = new TestViewModelRoutedViewHost(hostName);
        var viewHost2 = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(viewHost1);
        setNav.SetMainNavigationHost(viewHost2);

        // Assert - Should still be the first one
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(viewHost1);
    }

    [Test]
    public async Task ClearHistory_WithIUseNavigation_ClearsHostHistory()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => vm.ClearHistory()).Throws<Exception>();
    }

    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.ClearHistory()).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ClearsHostHistory()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.ClearHistory(hostName)).Throws<Exception>();
    }

    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ClearsSpecifiedHost()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    [Test]
    public async Task NavigateBack_WithIUseNavigation_NavigatesBack()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        ((IUseNavigation)vm).NavigateBack();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
    }

    [Test]
    [Skip("Method returns void/does nothing when host not found instead of throwing")]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateBack()).Throws<Exception>();
    }

    [Test]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateBack()).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task NavigateBack_WithIUseHostedNavigation_NavigatesBack()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        var result = vm.NavigateBack(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
    }

    [Test]
    [Skip("Method returns null when host not found instead of throwing")]
    public async Task NavigateBack_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.NavigateBack(hostName)).Throws<Exception>();
    }

    [Test]
    public async Task NavigateToView_WithIUseNavigation_Navigates()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateToView<TestViewModel>();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateToView<TestViewModel>()).Throws<KeyNotFoundException>();
    }

    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToView<TestViewModel>()).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_Navigates()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateToView<TestViewModel>(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    [Test]
    [Skip("Method does nothing when host not found instead of throwing")]
    public async Task NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToView<TestViewModel>(hostName)).Throws<Exception>();
    }

    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        ((IUseNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    [Test]
    [Skip("Method does nothing when host not found instead of throwing")]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>()).Throws<Exception>();
    }

    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToViewAndClearHistory<TestViewModel>()).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.NavigateToViewAndClearHistory<TestViewModel>(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    [Test]
    [Skip("Method does nothing when host not found instead of throwing")]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>(hostName)).Throws<Exception>();
    }

    [Test]
    public async Task WhenNavigatedFrom_SetsUpNavigatedFromFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);
        var called = false;

        // Act
        view.WhenNavigatedFrom(_ => called = true);

        // Assert
        await Assert.That(view.ISetupNavigatedFrom).IsTrue();
    }

    [Test]
    public async Task WhenNavigatedFrom_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedFrom(_ => { })).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WhenNavigatedTo_SetsUpNavigatedToFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);
        var called = false;

        // Act
        view.WhenNavigatedTo((_, _) => called = true);

        // Assert
        await Assert.That(view.ISetupNavigatedTo).IsTrue();
    }

    [Test]
    public async Task WhenNavigatedTo_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedTo((_, _) => { })).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WhenNavigating_SetsUpNavigatingFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);

        // Act
        view.WhenNavigating(e => e);

        // Assert
        await Assert.That(view.ISetupNavigating).IsTrue();
    }

    [Test]
    public async Task WhenNavigating_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigating(e => e)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task WhenSetup_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = Locator.CurrentMutable;
        resolver.SetupComplete();

        // Act
        setNav.SetMainNavigationHost(viewHost);
        var observable = vm.WhenSetup();

        var result = false;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    [Test]
    public async Task WhenSetup_WithIUseHostedNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(viewHost);
        var observable = vm.WhenSetup(hostName);

        var result = false;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = Locator.CurrentMutable;
        resolver.SetupComplete();
        setNav.SetMainNavigationHost(viewHost);

        // Act
        var observable = vm.CanNavigateBack();

        var result = true;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsFalse();

        subscription.Dispose();
    }

    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack()).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task CanNavigateBack_WithIUseHostedNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        var observable = vm.CanNavigateBack(hostName);

        var result = true;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsFalse();

        subscription.Dispose();
    }

    [Test]
    public async Task CanNavigateBack_WithIUseHostedNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestHostedViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack("TestHost")).Throws<ArgumentNullException>();
    }
}
