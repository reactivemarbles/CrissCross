# Bidirectional Navigation TUnit Red Test Plan

Status: discovery/spec for `t_a6b5f732`.
Purpose: define failing TUnit coverage for the bidirectional navigation contract before production implementation.

These tests are intentionally a RED plan. They should be added as executable tests in `src/Tests/CrissCross.Tests/` during the implementation card after the public API names are finalized. Until then, this document is the test contract and avoids breaking the current build with non-existent API references.

## Test fixture conventions

Recommended fixture names:

- `BidirectionalNavigationRegistryTests`
- `BidirectionalNavigationResolverTests`
- `BidirectionalNavigationLifecycleTests`
- `BidirectionalNavigationCancellationTests`
- `PlatformBidirectionalNavigationAdapterTests` in platform-specific test projects when those projects exist.

Recommended test doubles:

```csharp
private interface ICustomerPageViewModel;
private interface ICustomerPageView;

private sealed class CustomerPageViewModel : RxObject, ICustomerPageViewModel;
private sealed class CustomerPageView : ICustomerPageView, IViewFor<CustomerPageViewModel>
{
    public object? ViewModel { get; set; }
    CustomerPageViewModel? IViewFor<CustomerPageViewModel>.ViewModel
    {
        get => (CustomerPageViewModel?)ViewModel;
        set => ViewModel = value;
    }
}
```

Use unique contracts per test where static service state is involved. Prefer isolated registry instances over global `AppLocator` state for new base tests.

## RED tests

### Registration and symmetric resolution

1. `Register_ViewModelAndView_AllowsViewModelTypeNavigationToResolveView`

Arrange a registry with `CustomerPageViewModel -> CustomerPageView`. Act with `NavigateViewModel<CustomerPageViewModel, CustomerPageView>()`. Assert the result contains a `CustomerPageViewModel`, a `CustomerPageView`, and `view.ViewModel` references the resolved ViewModel.

Initial RED reason: no bidirectional registry/navigator API exists.

2. `Register_ViewModelAndView_AllowsViewModelInstanceNavigationToResolveView`

Arrange a supplied `CustomerPageViewModel` instance. Act with `NavigateViewModel<CustomerPageViewModel, CustomerPageView>(instance)`. Assert the exact instance is used and assigned to the resolved View.

Initial RED reason: existing hosts can navigate `IRxObject` instances, but there is no platform-neutral typed result contract.

3. `Register_InterfaceKeys_AllowsViewModelInterfaceNavigationToResolveConcreteView`

Register `ICustomerPageViewModel -> CustomerPageViewModel` and `ICustomerPageView -> CustomerPageView`. Act with `NavigateViewModel<ICustomerPageViewModel>()`. Assert concrete ViewModel/View instances are resolved.

Initial RED reason: existing ViewModel-first APIs constrain generic navigation to `IRxObject` concrete type keys and do not expose an interface-key pair registration.

4. `Register_ViewAndViewModel_AllowsViewTypeNavigationToResolveViewModel`

Arrange a registered pair. Act with `NavigateView<CustomerPageViewModel, CustomerPageView>()`. Assert the result contains a new `CustomerPageView` and matching `CustomerPageViewModel`, with the ViewModel assigned to the View.

Initial RED reason: current `NavigateToView<T>()` uses ViewModel type semantics; actual View-first navigation does not exist in base.

5. `Register_ViewInterfaceKey_AllowsViewInterfaceNavigationToResolveConcreteViewModel`

Register the pair with `ICustomerPageView` as the view key. Act with `NavigateView<ICustomerPageView>()`. Assert concrete View and ViewModel resolution.

Initial RED reason: there is no reverse View-key registry.

### Contracts

6. `Register_SameViewModelWithTwoContracts_ResolvesContractSpecificView`

Register `CustomerPageViewModel -> SummaryCustomerPageView` with contract `summary` and `CustomerPageViewModel -> DetailCustomerPageView` with contract `detail`. Act through ViewModel-first navigation for both contracts. Assert the chosen View matches the contract.

Initial RED reason: current VM-first flow passes contract to Splat/IViewLocator, but no bidirectional pair contract validates both directions.

7. `Register_SameViewWithTwoContracts_ResolvesContractSpecificViewModel`

Register `CustomerSummaryView -> CustomerSummaryViewModel` and `CustomerSummaryView -> CustomerReadonlyViewModel` with different contracts. Act through View-first navigation for each contract. Assert the selected ViewModel matches the contract.

Initial RED reason: no View-first reverse contract lookup exists.

8. `Register_DuplicateViewModelKeyAndContract_ThrowsNavigationRegistrationException`

Register the same `(ViewModel key, default contract)` twice. Assert registration throws the typed duplicate-registration exception and reports the duplicate key/contract.

Initial RED reason: current registration is container/global-dictionary based and does not encode pair uniqueness.

### Missing registrations and ambiguity

9. `NavigateViewModel_WhenViewRegistrationMissing_ThrowsNavigationResolutionException`

Register only a ViewModel factory. Act with ViewModel-first navigation. Assert a typed exception includes source kind `ViewModel`, the requested ViewModel key, default contract, and known contracts.

Initial RED reason: current host may produce null View and still publish navigation events; missing view failure is not deterministic.

10. `NavigateView_WhenViewModelRegistrationMissing_ThrowsNavigationResolutionException`

Register only a View factory or use an unregistered View key. Act with View-first navigation. Assert a typed exception includes source kind `View` and requested View key.

Initial RED reason: no reverse lookup exists.

11. `Navigate_WithUnknownContract_ThrowsNavigationResolutionExceptionWithKnownContracts`

Register default and `detail` contracts, then request `summary`. Assert exception lists known contracts.

Initial RED reason: current failures are resolver/container-specific and do not expose contract diagnostics.

### Activation and disposal lifecycle

12. `Navigate_CommittedTransition_DisposesPreviousActiveNavigationDisposables`

Arrange two navigations. The first ViewModel adds a disposable in `WhenNavigatedTo`. Act with a second committed navigation. Assert the first active disposable is disposed exactly once after cancellation checks pass.

Initial RED reason: current `CurrentViewDisposable` is host-keyed global state and should be covered by a platform-neutral lifecycle contract.

13. `Navigate_CancelledTransition_DoesNotDisposeCurrentActiveDisposables`

Arrange an active first navigation and a second navigation whose navigating hook sets `Cancel = true`. Assert current content/history/disposables remain unchanged.

Initial RED reason: cancellation behavior is present in host code but not specified for bidirectional resolution/commit.

14. `Navigate_ViewHooksOptIn_SuppressesDuplicateViewModelLifecycleCallbacks`

Arrange a View implementing `INotifiyNavigation` with `WhenNavigatedTo`/`WhenNavigatedFrom` opt-in flags. Act a committed transition. Assert View hooks fire once and ViewModel hooks do not duplicate them.

Initial RED reason: existing host behavior branches on `INotifiyNavigation`; the new pipeline needs regression coverage.

15. `Navigate_ViewHooksNotOptedIn_InvokesViewModelLifecycleCallbacksOnce`

Arrange Views without navigation hook setup. Act a committed transition. Assert ViewModel `WhenNavigatedFrom` and `WhenNavigatedTo` fire exactly once.

Initial RED reason: required compatibility behavior for existing ViewModel-first navigation.

### Scheduler usage

16. `Navigate_CommitMutatesHostOnRxSchedulersMainThreadScheduler`

Arrange a test scheduler as `RxSchedulers.MainThreadScheduler` if the repository exposes a test override, or use the existing scheduler testing utility. Act navigation. Assert resolution can happen before advancing the main scheduler, but host content/history are not mutated until the main scheduler advances.

Initial RED reason: current WPF/Avalonia hosts call `ObserveOn(RxSchedulers.MainThreadScheduler)` in platform code; the new core/platform split must make this deterministic.

17. `Navigate_ResolutionDoesNotRequireUiSchedulerUntilCommit`

Arrange factories that run on the caller thread and a host adapter with a test main scheduler. Assert View/ViewModel resolution completes without UI scheduler advancement, then final commit waits for main scheduler.

Initial RED reason: separates platform-neutral resolution from UI mutation.

### Async navigation cancellation

18. `Navigate_CancellationBeforeResolution_DoesNotInvokeFactories`

Create a cancelled token before calling navigation. Assert ViewModel and View factories are not called, no events fire, and the returned observable terminates with cancellation semantics selected by the implementation contract.

Initial RED reason: no cancellation-aware navigation API exists.

19. `Navigate_CancellationAfterResolutionBeforeCommit_DoesNotMutateHost`

Arrange async resolution that completes, cancel before commit scheduler advances. Assert no host content/history mutation and no active disposable changes.

Initial RED reason: existing synchronous APIs cannot express this boundary.

20. `Navigate_SecondRequestCancelsFirstOnlyLatestCommits`

Arrange two async navigation requests through the host pipeline. The first delays; the second completes. Assert only the second resolution commits, and the first produces no content/history/lifecycle effects.

Initial RED reason: needed for reactive `Switch`/latest-wins navigation semantics.

### Cross-platform adapter parity

21. `WpfAdapter_CommittedResolutionAssignsContentToResolvedView`

For WPF tests, feed a completed `NavigationResolution` into the WPF adapter. Assert `Content` is the resolved View and `DataContext/ViewModel` references the resolved ViewModel.

22. `AvaloniaAdapter_CommittedResolutionAssignsContentToResolvedView`

For Avalonia tests, feed a completed `NavigationResolution` into the Avalonia adapter. Assert `Content` is the resolved View and `DataContext/ViewModel` references the resolved ViewModel.

23. `MauiAdapter_CommittedResolutionAssignsCurrentPageToResolvedView`

For future MAUI tests, feed a completed `NavigationResolution` into the MAUI adapter. Assert Shell/Page mutation occurs only through the MAUI adapter and not from core.

Initial RED reason: the current code duplicates host implementations; adapter parity needs explicit tests when the implementation card starts.

## Suggested executable TUnit shape

Example first RED test once APIs are finalized:

```csharp
[Test]
public async Task Register_ViewModelAndView_AllowsViewModelTypeNavigationToResolveView()
{
    var registry = new NavigationRegistry();
    registry.Register<CustomerPageViewModel, CustomerPageView>(
        static _ => new CustomerPageViewModel(),
        static _ => new CustomerPageView());

    var navigator = registry.CreateNavigator();

    var result = await navigator
        .NavigateViewModel<CustomerPageViewModel, CustomerPageView>()
        .FirstAsync();

    await Assert.That(result.ViewModel).IsTypeOf<CustomerPageViewModel>();
    await Assert.That(result.View).IsTypeOf<CustomerPageView>();
    await Assert.That(result.View.ViewModel).IsSameReferenceAs(result.ViewModel);
}
```

This should fail before production implementation because `NavigationRegistry`, `CreateNavigator`, and `NavigateViewModel` do not exist yet. That is the intended RED gate for the implementation card.

## Verification commands for implementation card

Run from `/mnt/c/Projects/GitHub/ReactiveMarbles/CrissCross/src` using Windows dotnet:

```bash
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --treenode-filter "/*/CrissCross.Tests/BidirectionalNavigationRegistryTests/*"
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --treenode-filter "/*/CrissCross.Tests/BidirectionalNavigationResolverTests/*"
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release -- --treenode-filter "/*/CrissCross.Tests/BidirectionalNavigationLifecycleTests/*"
"/mnt/c/Program Files/dotnet/dotnet.exe" test --project Tests/CrissCross.Tests/CrissCross.Tests.csproj -c Release
"/mnt/c/Program Files/dotnet/dotnet.exe" test --solution CrissCross.slnx -c Release
```

Do not use `--no-build`. Keep TUnit/MTP flags after `--`.
