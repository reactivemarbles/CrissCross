// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CrissCross.NavigationModes.Example;

/// <summary>Provides a runnable, platform-neutral navigation API example.</summary>
public static class Program
{
    /// <summary>Identifies the summary navigation contract.</summary>
    private const string SummaryContract = "summary";

    /// <summary>Identifies the detail navigation contract.</summary>
    private const string DetailContract = "detail";

    /// <summary>Runs each supported registry and journal navigation mode.</summary>
    /// <returns>A task that represents the asynchronous sample execution.</returns>
    public static async Task Main()
    {
        var registry = CreateRegistry();
        var navigator = registry.CreateNavigator();

        await DemonstrateTypedViewModelFirstNavigation(navigator).ConfigureAwait(false);
        await DemonstrateTypedViewFirstNavigation(navigator).ConfigureAwait(false);
        await DemonstrateInterfaceAndRuntimeKeyNavigation(navigator).ConfigureAwait(false);
        await DemonstrateSuppliedInstances(navigator).ConfigureAwait(false);
        await DemonstrateCancellation(navigator).ConfigureAwait(false);
        DemonstrateJournalNavigation();
    }

    /// <summary>Creates the registrations used by the sample navigation flows.</summary>
    /// <returns>A fully configured navigation registry.</returns>
    private static NavigationRegistry CreateRegistry()
    {
        var registry = new NavigationRegistry();
        _ = registry.Register(
            static _ => new CustomerPageViewModel("factory"),
            static _ => new CustomerPageView());
        _ = registry.Register(
            new NavigationRegistration<
                ICustomerPageViewModel,
                CustomerPageViewModel,
                ICustomerPageView,
                CustomerSummaryView>(
                static _ => new CustomerPageViewModel("summary-factory"),
                static _ => new CustomerSummaryView())
            { Contract = SummaryContract });
        _ = registry.Register(
            new NavigationRegistration<
                ICustomerPageViewModel,
                CustomerPageViewModel,
                ICustomerPageView,
                CustomerDetailView>(
                static _ => new CustomerPageViewModel("detail-factory"),
                static _ => new CustomerDetailView())
            { Contract = DetailContract });
        return registry;
    }

    /// <summary>Demonstrates strongly typed ViewModel-first resolution.</summary>
    /// <param name="navigator">The configured navigator.</param>
    /// <returns>A task that represents the asynchronous resolution.</returns>
    private static async Task DemonstrateTypedViewModelFirstNavigation(IBidirectionalNavigator navigator)
    {
        var resolution = await navigator
            .NavigateViewModel(new ViewModelNavigationRequest<CustomerPageViewModel, CustomerPageView>())
            .FirstAsync()
            .ConfigureAwait(false);

        Trace.WriteLine(
            $"ViewModel-first: {resolution.ViewModel.Source} -> {resolution.View.GetType().Name}; navigation type is {resolution.NavigationType}.");
    }

    /// <summary>Demonstrates strongly typed View-first resolution.</summary>
    /// <param name="navigator">The configured navigator.</param>
    /// <returns>A task that represents the asynchronous resolution.</returns>
    private static async Task DemonstrateTypedViewFirstNavigation(IBidirectionalNavigator navigator)
    {
        var resolution = await navigator
            .NavigateView(new ViewNavigationRequest<CustomerPageViewModel, CustomerPageView>())
            .FirstAsync()
            .ConfigureAwait(false);

        Trace.WriteLine(
            $"View-first: {resolution.View.GetType().Name} -> {resolution.ViewModel.Source}; the view model is assigned to the view.");
    }

    /// <summary>Demonstrates interface and runtime-key lookup with contracts and parameters.</summary>
    /// <param name="navigator">The configured navigator.</param>
    /// <returns>A task that represents the asynchronous resolutions.</returns>
    private static async Task DemonstrateInterfaceAndRuntimeKeyNavigation(IBidirectionalNavigator navigator)
    {
        var summary = await navigator
            .NavigateViewModel(
                new NavigationKeyRequest<ICustomerPageViewModel>
                {
                    Options = new NavigationRequestOptions { Contract = SummaryContract, HostName = "customer-host", Parameter = new CustomerNavigationParameter("customer-42") },
                })
            .FirstAsync()
            .ConfigureAwait(false);
        var detail = await navigator
            .NavigateView(
                typeof(ICustomerPageView),
                new NavigationRequestOptions { Contract = DetailContract })
            .FirstAsync()
            .ConfigureAwait(false);

        Trace.WriteLine(
            $"Interface ViewModel key: {summary.View.GetType().Name}, contract '{summary.Contract}', parameter '{((CustomerNavigationParameter)summary.Parameter!).CustomerId}'.");
        Trace.WriteLine($"Runtime View key: {detail.View.GetType().Name}, contract '{detail.Contract}'.");
    }

    /// <summary>Demonstrates identity-preserving ViewModel and View inputs.</summary>
    /// <param name="navigator">The configured navigator.</param>
    /// <returns>A task that represents the asynchronous resolutions.</returns>
    private static async Task DemonstrateSuppliedInstances(IBidirectionalNavigator navigator)
    {
        var suppliedViewModel = new CustomerPageViewModel("supplied-view-model");
        var viewModelResolution = await navigator
            .NavigateViewModel(
                new ViewModelNavigationRequest<CustomerPageViewModel, CustomerPageView> { ViewModel = suppliedViewModel })
            .FirstAsync()
            .ConfigureAwait(false);
        var suppliedView = new CustomerPageView { ViewModel = suppliedViewModel };
        var viewResolution = await navigator
            .NavigateView(
                new ViewNavigationRequest<CustomerPageViewModel, CustomerPageView> { View = suppliedView })
            .FirstAsync()
            .ConfigureAwait(false);

        Trace.WriteLine($"Supplied ViewModel preserved: {ReferenceEquals(suppliedViewModel, viewModelResolution.ViewModel)}.");
        Trace.WriteLine($"Supplied View and compatible ViewModel preserved: {ReferenceEquals(suppliedView, viewResolution.View)}.");
    }

    /// <summary>Demonstrates observable cancellation before factories can create a navigation pair.</summary>
    /// <param name="navigator">The configured navigator.</param>
    /// <returns>A task that represents the asynchronous resolution.</returns>
    private static async Task DemonstrateCancellation(IBidirectionalNavigator navigator)
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync().ConfigureAwait(false);

        try
        {
            _ = await navigator
                .NavigateViewModel(
                    new ViewModelNavigationRequest<CustomerPageViewModel, CustomerPageView> { Options = new NavigationRequestOptions { CancellationToken = cancellation.Token } })
                .FirstAsync()
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Trace.WriteLine("Cancellation: the observable stopped before resolving a view and view model.");
        }
    }

    /// <summary>Demonstrates new, back, forward, and reset journal state transitions.</summary>
    private static void DemonstrateJournalNavigation()
    {
        var journal = new List<string> { "home" };
        var currentIndex = 0;
        NavigationJournal.Record(journal, ref currentIndex, "orders");
        NavigationJournal.Record(journal, ref currentIndex, "order-42");
        _ = NavigationJournal.TryMoveBack(journal, currentIndex, out currentIndex, out var previous);
        _ = NavigationJournal.TryMoveForward(journal, currentIndex, out currentIndex, out var next);
        NavigationJournal.Clear(journal, ref currentIndex);

        Trace.WriteLine($"Journal: back '{previous}', forward '{next}', then clear (index {currentIndex}).");
    }
}
