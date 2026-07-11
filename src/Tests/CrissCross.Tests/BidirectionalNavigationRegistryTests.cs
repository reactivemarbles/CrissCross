// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.Tests;

/// <summary>Tests for the platform-neutral bidirectional navigation registry and resolver.</summary>
public class BidirectionalNavigationRegistryTests
{
    /// <summary>Provides the SummaryContract member.</summary>
    private const string SummaryContract = "summary";

    /// <summary>Provides the DetailContract member.</summary>
    private const string DetailContract = "detail";

    /// <summary>Provides the ICustomerPageViewModel member.</summary>
    private interface ICustomerPageViewModel
    {
        /// <summary>Gets the customer scope.</summary>
        string CustomerScope { get; }
    }

    /// <summary>Provides the ICustomerPageView member.</summary>
    private interface ICustomerPageView
    {
        /// <summary>Gets the view scope.</summary>
        string ViewScope { get; }
    }

    /// <summary>Verifies that view-first navigation preserves a supplied view and its compatible view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateView_WithSuppliedView_PreservesViewAndCompatibleViewModel()
    {
        var expectedViewModel = new CustomerPageViewModel();
        var expectedView = new CustomerPageView { ViewModel = expectedViewModel };
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateView<CustomerPageViewModel, CustomerPageView>(expectedView)
            .FirstAsync();

        await Assert.That(ReferenceEquals(result.View, expectedView)).IsTrue();
        await Assert.That(ReferenceEquals(result.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, expectedViewModel)).IsTrue();
    }

    /// <summary>Verifies that generic view navigation rejects a null navigator.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateView_WithNullNavigator_ThrowsArgumentNullException()
    {
        IBidirectionalNavigator? navigator = null;

        await Assert.That(() => navigator!.NavigateView<ICustomerPageView>()).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that generic view model navigation rejects a null navigator.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateViewModel_WithNullNavigator_ThrowsArgumentNullException()
    {
        IBidirectionalNavigator? navigator = null;

        await Assert.That(() => navigator!.NavigateViewModel<ICustomerPageViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the Register_ViewModelAndView_AllowsViewModelTypeNavigationToResolveView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_ViewModelAndView_AllowsViewModelTypeNavigationToResolveView()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    /// <summary>Provides the Register_ViewModelAndView_AllowsViewModelInstanceNavigationToResolveView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_ViewModelAndView_AllowsViewModelInstanceNavigationToResolveView()
    {
        var expectedViewModel = new CustomerPageViewModel();
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<CustomerPageViewModel, CustomerPageView>(expectedViewModel)
            .FirstAsync();

        await Assert.That(ReferenceEquals(result.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, expectedViewModel)).IsTrue();
    }

    /// <summary>Provides the Register_InterfaceKeys_AllowsViewModelInterfaceNavigationToResolveConcreteView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_InterfaceKeys_AllowsViewModelInterfaceNavigationToResolveConcreteView()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateViewModel<ICustomerPageViewModel>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    /// <summary>Provides the Register_InterfaceKeys_PreservesConcreteIdentityContractAndParameter member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_InterfaceKeys_PreservesConcreteIdentityContractAndParameter()
    {
        var expectedViewModel = new CustomerPageViewModel();
        var expectedView = new CustomerPageView();
        var expectedParameter = new NavigationRequestParameter("customer-42");
        var registry = new NavigationRegistry();
        _ = registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            _ => expectedViewModel,
            _ => expectedView,
            DetailContract);

        var result = await registry.CreateNavigator()
            .NavigateViewModel<ICustomerPageViewModel>(DetailContract, expectedParameter)
            .FirstAsync();

        await Assert.That(ReferenceEquals(result.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(result.View, expectedView)).IsTrue();
        await Assert.That(result.View.GetType()).IsEqualTo(typeof(CustomerPageView));
        await Assert.That(ReferenceEquals(expectedView.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(result.Contract).IsEqualTo(DetailContract);
        await Assert.That(ReferenceEquals(result.Parameter, expectedParameter)).IsTrue();
    }

    /// <summary>Provides the Register_ViewAndViewModel_AllowsViewTypeNavigationToResolveViewModel member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_ViewAndViewModel_AllowsViewTypeNavigationToResolveViewModel()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateView<CustomerPageViewModel, CustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    /// <summary>Provides the Register_ViewInterfaceKey_AllowsViewInterfaceNavigationToResolveConcreteViewModel member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_ViewInterfaceKey_AllowsViewInterfaceNavigationToResolveConcreteViewModel()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var result = await registry.CreateNavigator()
            .NavigateView<ICustomerPageView>()
            .FirstAsync();

        await Assert.That(result.ViewModel is CustomerPageViewModel).IsTrue();
        await Assert.That(result.View is CustomerPageView).IsTrue();
        await Assert.That(ReferenceEquals(result.View.ViewModel, result.ViewModel)).IsTrue();
    }

    /// <summary>Provides the NavigateView_RuntimeInterfaceKey_ResolvesConcreteViewAndViewModel member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateView_RuntimeInterfaceKey_ResolvesConcreteViewAndViewModel()
    {
        var expectedViewModel = new CustomerPageViewModel();
        var expectedView = new CustomerPageView();
        var expectedParameter = new NavigationRequestParameter("runtime-view-key");
        var viewKey = typeof(ICustomerPageView);
        var registry = new NavigationRegistry();
        _ = registry.Register<ICustomerPageViewModel, CustomerPageViewModel, ICustomerPageView, CustomerPageView>(
            _ => expectedViewModel,
            _ => expectedView,
            DetailContract);

        var result = await registry.CreateNavigator()
            .NavigateView(viewKey, DetailContract, expectedParameter)
            .FirstAsync();

        await Assert.That(ReferenceEquals(result.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(result.View, expectedView)).IsTrue();
        await Assert.That(result.ViewModel.GetType()).IsEqualTo(typeof(CustomerPageViewModel));
        await Assert.That(result.View.GetType()).IsEqualTo(typeof(CustomerPageView));
        await Assert.That(result.Contract).IsEqualTo(DetailContract);
        await Assert.That(ReferenceEquals(result.Parameter, expectedParameter)).IsTrue();
    }

    /// <summary>Provides the Register_SameViewModelWithTwoContracts_ResolvesContractSpecificView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_SameViewModelWithTwoContracts_ResolvesContractSpecificView()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerSummaryView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerSummaryView(),
            SummaryContract);
        _ = registry.Register<CustomerPageViewModel, CustomerDetailView>(
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

    /// <summary>Provides the Register_SameViewWithTwoContracts_ResolvesContractSpecificViewModel member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_SameViewWithTwoContracts_ResolvesContractSpecificViewModel()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerSummaryViewModel, CustomerSummaryView>(
            static _ => new CustomerSummaryViewModel(),
            static _ => new CustomerSummaryView(),
            SummaryContract);
        _ = registry.Register<CustomerReadOnlyViewModel, CustomerReadOnlyView>(
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

    /// <summary>Provides the Register_DuplicateViewModelKeyAndContract_ThrowsNavigationRegistrationException member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Register_DuplicateViewModelKeyAndContract_ThrowsNavigationRegistrationException()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
            static _ => new CustomerPageViewModel(),
            static _ => new CustomerPageView());

        var exception = await CaptureNavigationRegistrationException(() =>
        {
            _ = registry.Register<CustomerPageViewModel, AlternateCustomerPageView>(
                static _ => new CustomerPageViewModel(),
                static _ => new AlternateCustomerPageView());
        });

        await Assert.That(exception.SourceKind).IsEqualTo(NavigationSourceKind.ViewModel);
        await Assert.That(exception.ServiceType).IsEqualTo(typeof(CustomerPageViewModel));
    }

    /// <summary>Provides the Navigate_WithUnknownContract_ThrowsNavigationResolutionExceptionWithKnownContracts member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_WithUnknownContract_ThrowsNavigationResolutionExceptionWithKnownContracts()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerSummaryView>(
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

    /// <summary>Provides the Navigate_CancellationBeforeResolution_DoesNotInvokeFactories member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_CancellationBeforeResolution_DoesNotInvokeFactories()
    {
        var viewModelFactoryCalls = 0;
        var viewFactoryCalls = 0;
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var registry = new NavigationRegistry();
        _ = registry.Register<CustomerPageViewModel, CustomerPageView>(
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

    /// <summary>Provides the CaptureNavigationRegistrationException member.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>The result.</returns>
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

    /// <summary>Provides the CaptureNavigationResolutionException member.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>The result.</returns>
    private static async Task<NavigationResolutionException> CaptureNavigationResolutionException(Func<Task<NavigationResolution<CustomerPageViewModel, CustomerSummaryView>>> action)
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

    /// <summary>Provides the CaptureOperationCanceledException member.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>The result.</returns>
    private static async Task<OperationCanceledException> CaptureOperationCanceledException(Func<Task<NavigationResolution<CustomerPageViewModel, CustomerPageView>>> action)
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

    /// <summary>Provides the CustomerPageViewModel member.</summary>
    private sealed class CustomerPageViewModel : RxObject, ICustomerPageViewModel
    {
        /// <summary>Gets the customer scope.</summary>
        public string CustomerScope => "Customer";
    }

    /// <summary>Provides the CustomerSummaryViewModel member.</summary>
    private sealed class CustomerSummaryViewModel : RxObject;

    /// <summary>Provides the CustomerReadOnlyViewModel member.</summary>
    private sealed class CustomerReadOnlyViewModel : RxObject;

    /// <summary>Gets or sets the value.</summary>
    private class CustomerPageView : ICustomerPageView, IViewFor<CustomerPageViewModel>
    {
        /// <summary>Gets the view scope.</summary>
        public string ViewScope => "Customer";

        /// <summary>Gets or sets the value.</summary>
        public CustomerPageViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerPageViewModel?)value;
        }
    }

    /// <summary>Provides the AlternateCustomerPageView member.</summary>
    private sealed class AlternateCustomerPageView : CustomerPageView;

    /// <summary>Gets or sets the value.</summary>
    private sealed class CustomerSummaryView : IViewFor<CustomerPageViewModel>, IViewFor<CustomerSummaryViewModel>
    {
        /// <summary>Gets or sets the value.</summary>
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

    /// <summary>Gets or sets the value.</summary>
    private sealed class CustomerDetailView : IViewFor<CustomerPageViewModel>
    {
        /// <summary>Gets or sets the value.</summary>
        public CustomerPageViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerPageViewModel?)value;
        }
    }

    /// <summary>Gets or sets the value.</summary>
    private sealed class CustomerReadOnlyView : IViewFor<CustomerReadOnlyViewModel>
    {
        /// <summary>Gets or sets the value.</summary>
        public CustomerReadOnlyViewModel? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerReadOnlyViewModel?)value;
        }
    }

    /// <summary>Provides the NavigationRequestParameter member.</summary>
    /// <param name="Value">The value.</param>
    private sealed record NavigationRequestParameter(string Value);
}
