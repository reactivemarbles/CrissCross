// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI;
using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public class ViewModelRoutedViewHostMixinsTests
{
    /// <summary>Provides the _testCounter member.</summary>
    private static int _testCounter;

    /// <summary>Provides the Setup member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Before(HookType.Test)]
    public async Task Setup()
    {
        // Note: We don't clear static dictionaries as it causes test isolation issues
        // Each test should use unique host names to avoid conflicts
        await Task.CompletedTask;
    }

    /// <summary>Provides the SetMainNavigationHost_RegistersHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the SetMainNavigationHost_ThrowsWhenSetNavigationIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenSetNavigationIsNull()
    {
        // Arrange
        TestSetNavigationViewModel? setNav = null;
        var viewHost = new TestViewModelRoutedViewHost("TestHost");

        // Act & Assert
        await Assert.That(() => setNav!.SetMainNavigationHost(viewHost)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the SetMainNavigationHost_ThrowsWhenViewHostIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenViewHostIsNull()
    {
        // Arrange
        var setNav = new TestSetNavigationViewModel("TestHost");
        TestViewModelRoutedViewHost? viewHost = null;

        // Act & Assert
        await Assert.That(() => setNav.SetMainNavigationHost(viewHost!)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the SetMainNavigationHost_ReplacesExistingHostForSameKey member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ReplacesExistingHostForSameKey()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var setNav = new TestSetNavigationViewModel(hostName);
        var staleHost = new TestViewModelRoutedViewHost(hostName);
        var replacementHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(staleHost);
        setNav.SetMainNavigationHost(replacementHost);

        // Assert - recreated shells/windows must not keep navigating through stale hosts.
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(replacementHost);
    }

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ClearsHostHistory member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => vm.ClearHistory()).Throws<Exception>();
    }

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.ClearHistory()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ClearsHostHistory member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.ClearHistory(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ClearsSpecifiedHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateBack_WithIUseNavigation_NavigatesBack member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateBack_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateBack()).Throws<Exception>();
    }

    /// <summary>Provides the NavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateBack()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateBack_WithIUseHostedNavigation_NavigatesBack member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateBack_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.NavigateBack(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_Navigates member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var otherHostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(otherHostName);
        var viewHost = new TestViewModelRoutedViewHost(otherHostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateToView<TestViewModel>()).Throws<KeyNotFoundException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToView<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseHostedNavigation_Navigates member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToView<TestViewModel>(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>()).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToViewAndClearHistory<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseHostedNavigation_NavigatesAndClears member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the WhenNavigatedFrom_SetsUpNavigatedFromFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the WhenNavigatedFrom_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedFrom_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedFrom(_ => { })).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenNavigatedTo_SetsUpNavigatedToFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the WhenNavigatedTo_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedTo_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedTo((_, _) => { })).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenNavigating_SetsUpNavigatingFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the WhenNavigating_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigating_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigating(e => e)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenSetup_WithIUseNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenSetup_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = AppLocator.CurrentMutable;
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

    /// <summary>Provides the WhenSetup_WithIUseHostedNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the CanNavigateBack_WithIUseNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = AppLocator.CurrentMutable;
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

    /// <summary>Provides the CanNavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the CanNavigateBack_WithIUseHostedNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the CanNavigateBack_WithIUseHostedNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseHostedNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestHostedViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack("TestHost")).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the GetUniqueHostName member.</summary>
    /// <returns>The result.</returns>
    private static string GetUniqueHostName() => $"TestHost{Interlocked.Increment(ref _testCounter)}";

    /// <summary>Provides the TestViewModel member.</summary>
    private sealed class TestViewModel : RxObject, IUseNavigation
    {
        /// <summary>Provides the documented member.</summary>
        private readonly string? _navName;

        /// <summary>Initializes a new instance of the <see cref="TestViewModel"/> class.</summary>
        /// <param name="name">The name value.</param>
        public TestViewModel(string? name = "")
        {
            _navName = name;
        }

        string? IUseNavigation.Name => _navName;
    }

    /// <summary>Provides the TestHostedViewModel member.</summary>
    private sealed class TestHostedViewModel : RxObject;

    /// <summary>Provides the TestSetNavigationViewModel member.</summary>
    private sealed class TestSetNavigationViewModel : RxObject, ISetNavigation
    {
        /// <summary>Provides the documented member.</summary>
        private readonly string? _setNavName;

        /// <summary>Initializes a new instance of the <see cref="TestSetNavigationViewModel"/> class.</summary>
        /// <param name="name">The name value.</param>
        public TestSetNavigationViewModel(string? name = "TestHost")
        {
            _setNavName = name;
        }

        string? ISetNavigation.Name => _setNavName;
    }

    /// <summary>Provides the TestViewModelRoutedViewHost member.</summary>
    private sealed class TestViewModelRoutedViewHost : IViewModelRoutedViewHost
    {
        /// <summary>Provides the _currentViewModel member.</summary>
        private readonly StateSignal<INotifiyRoutableViewModel?> _currentViewModel = new(null);

        /// <summary>Provides the documented member.</summary>
        private readonly StateSignal<bool?> _canNavigateBack = new(false);

        /// <summary>Initializes a new instance of the <see cref="TestViewModelRoutedViewHost"/> class.</summary>
        /// <param name="name">The name value.</param>
        public TestViewModelRoutedViewHost(string name = "TestHost")
        {
            Name = name;
        }

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
        public string Name { get; set; }

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

        /// <summary>Provides the Navigate member.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            NavigationStack.Add(typeof(T));
            CanNavigateBack = NavigationStack.Count > 1;
        }

        /// <summary>Provides the Navigate member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;

            if (viewModel is not INotifiyRoutableViewModel routableViewModel)
            {
                return;
            }

            _currentViewModel.OnNext(routableViewModel);
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            NavigationStack.Clear();
            NavigationStack.Add(typeof(T));
            CanNavigateBack = false;
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            NavigationStack.Clear();
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = false;
        }

        /// <summary>Provides the NavigateBack member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public IRxObject? NavigateBack(object? parameter = null)
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

    /// <summary>Provides the documented member.</summary>
    private sealed class TestView : INotifiyNavigation, IViewFor
    {
        /// <summary>Initializes a new instance of the <see cref="TestView"/> class.</summary>
        /// <param name="viewModel">The view model.</param>
        public TestView(object? viewModel = null)
        {
            ViewModel = viewModel;
        }

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
        public object? ViewModel { get; set; }

        /// <summary>Provides the Dispose member.</summary>
        public void Dispose()
        {
            CleanUp?.Dispose();
        }
    }
}
