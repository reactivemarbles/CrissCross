// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ReactiveUI;

namespace CrissCross.Tests;

/// <summary>
/// Tests for the platform-neutral bidirectional navigation registry and resolver.
/// </summary>
public class BidirectionalNavigationRegistryTests
{
    private const string SummaryContract = "summary";
    private const string DetailContract = "detail";

    [Test]
    public async Task Register_ViewModelAndView_AllowsViewModelTypeNavigationToResolveView()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    [Test]
    public async Task Register_ViewModelAndView_AllowsViewModelInstanceNavigationToResolveView()
    {
        var expectedViewModel = new CustomerPageViewModel();
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerPageView>(expectedViewModel)
            .FirstAsync();

        await Assert.That(ReferenceEquals(result.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, expectedViewModel)).IsTrue();
    }

    [Test]
    public async Task Register_InterfaceKeys_AllowsViewModelInterfaceNavigationToResolveConcreteView()
    {
        var registry = new NavigationRegistry();
        registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<ICustomerPageViewModel>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    [Test]
    public async Task Register_ViewAndViewModel_AllowsViewTypeNavigationToResolveViewModel()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateView<CustomerPageViewModel, CustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    [Test]
    public async Task Register_ViewInterfaceKey_AllowsViewInterfaceNavigationToResolveConcreteViewModel()
    {
        var registry = new NavigationRegistry();
        registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateView<ICustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    [Test]
    public async Task Register_SameViewModelWithTwoContracts_ResolvesContractSpecificView()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerSummaryView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerSummaryView(),
            SummaryContract);
        registry.Register<CustomerPageViewModel, CustomerDetailView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerDetailView(),
            DetailContract);

        var summary = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerSummaryView>(SummaryContract)
            .FirstAsync();
        var detail = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerDetailView>(DetailContract)
            .FirstAsync();

        await Assert.That(summary.View is CustomerSummaryView).IsTrue();
        await Assert.That(detail.View is CustomerDetailView).IsTrue();
    }

    [Test]
    public async Task Register_SameViewWithTwoContracts_ResolvesContractSpecificViewModel()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerSummaryViewModel, CustomerSummaryView>(
            static _ => new CustomerSummaryViewModel(),
            static _ => new CustomerSummaryView(),
            SummaryContract);
        registry.Register<CustomerReadOnlyViewModel, CustomerReadOnlyView>(
            static _ => new CustomerReadOnlyViewModel(),
            static _ => new CustomerReadOnlyView(),
            DetailContract);

        var summary = await registry.CreateNavigator()
            .NavigateView<CustomerSummaryViewModel, CustomerSummaryView>(SummaryContract)
            .FirstAsync();
        var detail = await registry.CreateNavigator()
            .NavigateView<CustomerReadOnlyViewModel, CustomerReadOnlyView>(DetailContract)
            .FirstAsync();

        await Assert.That(summary.ViewModel is CustomerSummaryViewModel).IsTrue();
        await Assert.That(detail.ViewModel is CustomerReadOnlyViewModel).IsTrue();
    }

    [Test]
    public async Task Register_DuplicateViewModelKeyAndContract_ThrowsNavigationRegistrationException()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var exception = await CaptureNavigationRegistrationException(() =>
        {
            registry.Register<CustomerPageViewModel, AlternateCustomerPageView>(
                static _ => new CustomerPageViewModel(),
                static _ => new AlternateCustomerPageView());
        });

        await Assert.That(exception.SourceKind).IsEqualTo(NavigationSourceKind.ViewModel);
        await Assert.That(exception.ServiceType).IsEqualTo(typeof(CustomerPageViewModel));
    }

    [Test]
    public async Task Navigate_WithUnknownContract_ThrowsNavigationResolutionExceptionWithKnownContracts()
    {
        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerSummaryView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerSummaryView(),
            SummaryContract);

        var exception = await CaptureNavigationResolutionException(() => registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerSummaryView>(DetailContract)
            .FirstAsync());

        await Assert.That(exception.SourceKind).IsEqualTo(NavigationSourceKind.ViewModel);
        await Assert.That(exception.SourceKey).IsEqualTo(typeof(CustomerPageViewModel));
        await Assert.That(exception.Contract).IsEqualTo(DetailContract);
        await Assert.That(exception.KnownContracts.Contains(SummaryContract)).IsTrue();
    }

    [Test]
    public async Task Navigate_CancellationBeforeResolution_DoesNotInvokeFactories()
    {
        var viewModelFactoryCalls = 0;
        var viewFactoryCalls = 0;
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var registry = new NavigationRegistry();
        registry.Register<CustomerPageViewModel, CustomerPageView>(
            _ =>
            {
                viewModelFactoryCalls++;
                return new CustomerPageViewModel();
            },
            _ =>
            {
                viewFactoryCalls++;
                return new CustomerPageView();
            });

        var exception = await CaptureOperationCanceledException(() => registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerPageView>(cancellationToken: cancellation.Token)
            .FirstAsync());

        await Assert.That(exception.CancellationToken).IsEqualTo(cancellation.Token);
        await Assert.That(viewModelFactoryCalls).IsEqualTo(0);
        await Assert.That(viewFactoryCalls).IsEqualTo(0);
    }

    private static Task<NavigationRegistrationException> CaptureNavigationRegistrationException(Action action)
    {
        try
        {
            action();
        }
        catch (NavigationRegistrationException exception)
        {
            return Task.FromResult(exception);
        }

        throw new InvalidOperationException("Expected a navigation registration exception.");
    }

    private static async Task<NavigationResolutionException> CaptureNavigationResolutionException(Func<IObservable<NavigationResolution<CustomerPageViewModel, CustomerSummaryView>>> action)
    {
        try
        {
            _ = await action();
        }
        catch (NavigationResolutionException exception)
        {
            return exception;
        }

        throw new InvalidOperationException("Expected a navigation resolution exception.");
    }

    private static async Task<OperationCanceledException> CaptureOperationCanceledException(Func<IObservable<NavigationResolution<CustomerPageViewModel, CustomerPageView>>> action)
    {
        try
        {
            _ = await action();
        }
        catch (OperationCanceledException exception)
        {
            return exception;
        }

        throw new InvalidOperationException("Expected an operation canceled exception.");
    }

    private interface ICustomerPageViewModel;

    private interface ICustomerPageView;

    private sealed class CustomerPageViewModel : RxObject, ICustomerPageViewModel;

    private sealed class CustomerSummaryViewModel : RxObject;

    private sealed class CustomerReadOnlyViewModel : RxObject;

    private class CustomerPageView : ICustomerPageView, IViewFor<CustomerPageViewModel>
    {
        public CustomerPageViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerPageViewModel?)value;
        }
    }

    private sealed class AlternateCustomerPageView : CustomerPageView;

    private sealed class CustomerSummaryView : IViewFor<CustomerPageViewModel>, IViewFor<CustomerSummaryViewModel>
    {
        public object? ViewModel { get; set; }

        CustomerPageViewModel? IViewFor<CustomerPageViewModel>.ViewModel
        {
            get => (CustomerPageViewModel?)ViewModel;
            set => ViewModel = value;
        }

        CustomerSummaryViewModel? IViewFor<CustomerSummaryViewModel>.ViewModel
        {
            get => (CustomerSummaryViewModel?)ViewModel;
            set => ViewModel = value;
        }
    }

    private sealed class CustomerDetailView : IViewFor<CustomerPageViewModel>
    {
        public CustomerPageViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerPageViewModel?)value;
        }
    }

    private sealed class CustomerReadOnlyView : IViewFor<CustomerReadOnlyViewModel>
    {
        public CustomerReadOnlyViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerReadOnlyViewModel?)value;
        }
    }
}
