# Bidirectional Navigation Architecture

Status: discovery/spec for `t_a6b5f732`.
Scope: CrissCross base, WPF, Avalonia, and future MAUI navigation surfaces.

## Problem statement

CrissCross has a functional ViewModel-first navigation path and separate page/view-first navigation services, but they are not one coherent contract:

- `IViewModelRoutedViewHost` exposes ViewModel-first APIs only: `Navigate<TViewModel>()`, `Navigate(IRxObject)`, `NavigateAndReset<TViewModel>()`, and `NavigateBack()` (`src/CrissCross/IViewModelRoutedViewHost.cs`).
- `ViewModelRoutedViewHostMixins.NavigateToView(...)` is named like view navigation, but the generic/type arguments are still `IRxObject` ViewModel keys and the `Type` overload resolves an `IRxObject` service before calling the host (`src/CrissCross/ViewModelRoutedViewHostMixins.cs`).
- WPF and Avalonia `ViewModelRoutedViewHost` implementations duplicate the same ViewModel-first flow: resolve ViewModel from `AppLocator`, resolve View with `IViewLocator.ResolveView(_toViewModel, contract)`, then swap content on `RxSchedulers.MainThreadScheduler` (`src/CrissCross.WPF/ViewModelRoutedViewHost.cs`, `src/CrissCross.Avalonia/ViewModelRoutedViewHost.cs`).
- WPF.UI and Avalonia.UI expose page/view-first services (`INavigationService.Navigate(Type pageType)`), but these use page services/control navigation and do not share the ViewModel-host registration/resolution contract (`src/CrissCross.WPF.UI/INavigationService.cs`, `src/CrissCross.Avalonia.UI/INavigationService.cs`).
- Existing examples register ViewModel and View separately using Splat (`RegisterConstant<TViewModel>` plus `Register<IViewFor<TViewModel>>`) or fluent `RegisterLazySingletonAnd(...).Register<IViewFor<TViewModel>>(...)`, which is enough for ViewModel-first lookup but does not encode reverse View-to-ViewModel registration.

The new architecture must define a single bidirectional registration and resolution model:

1. ViewModel navigation: caller supplies a ViewModel instance/type/interface plus optional contract; registered View is resolved.
2. View navigation: caller supplies a View instance/type/interface plus optional contract; registered ViewModel is resolved.

## Design principles

1. Strongly typed first. Public APIs should expose generic overloads where the caller knows the static type; untyped `Type` overloads are adapter/bridge APIs, not the primary user path.
2. AOT-friendly. Registration stores closed generic factory delegates. Resolution must not depend on assembly scanning, `Activator.CreateInstance`, string type names, or reflection-heavy construction paths.
3. Reactive result flow. Navigation requests, resolution, cancellation, activation, and completion should compose as `IObservable<T>` pipelines and schedule UI mutation through `RxSchedulers.MainThreadScheduler`.
4. Platform-neutral core. Core owns request/result/registration/resolution contracts. WPF, Avalonia, WinForms, and MAUI own only the final view-host adapter that attaches the resolved view to a platform control.
5. Symmetric contracts. The same optional contract string participates in both directions and scopes the pair `(ViewModel key, View key)`.
6. Deterministic failures. Missing or ambiguous registration should throw a typed navigation exception with source kind, source type, requested contract, and known matching contracts.

## Core model

### Navigation key

A navigation key is `(kind, serviceType, contract)`:

- `kind`: `ViewModel` or `View`.
- `serviceType`: the caller-facing type. This may be a concrete class or interface, for example `ICustomerPageViewModel` or `ICustomerPageView`.
- `contract`: optional string. `null` and `""` should be normalized consistently; recommendation: treat both as the default contract.

The key stores `Type` values only as identity tokens. It must not use those types for reflective construction.

### Registration pair

A registration links one ViewModel key and one View key to concrete factories:

```csharp
public sealed record NavigationRegistration<TViewModel, TView>(
    Func<IServiceProvider, TViewModel> CreateViewModel,
    Func<IServiceProvider, TView> CreateView,
    string? Contract = null)
    where TViewModel : class, IRxObject
    where TView : class, IViewFor<TViewModel>;
```

The implementation can store this behind a non-generic internal descriptor with strongly typed delegates captured at registration time:

```csharp
internal sealed record NavigationRegistrationDescriptor(
    Type ViewModelKey,
    Type ViewModelImplementation,
    Type ViewKey,
    Type ViewImplementation,
    string? Contract,
    Func<IServiceProvider, IRxObject> CreateViewModel,
    Func<IServiceProvider, IViewFor> CreateView);
```

This internal descriptor is AOT-safe because the factories are supplied by user code, DI, or generated registration code.

### Registration API contract

Primary user-facing registration should be closed generic and chainable:

```csharp
public interface INavigationRegistry
{
    INavigationRegistry Register<TViewModel, TView>(
        Func<IServiceProvider, TViewModel> createViewModel,
        Func<IServiceProvider, TView> createView,
        string? contract = null)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    INavigationRegistry Register<TViewModelKey, TViewModel, TViewKey, TView>(
        Func<IServiceProvider, TViewModel> createViewModel,
        Func<IServiceProvider, TView> createView,
        string? contract = null)
        where TViewModelKey : class
        where TViewModel : class, TViewModelKey, IRxObject
        where TViewKey : class
        where TView : class, TViewKey, IViewFor<TViewModel>;
}
```

Rules:

- The simple overload uses `typeof(TViewModel)` as the ViewModel key and `typeof(TView)` as the View key.
- The key overload allows interface-based navigation without reflection: `TViewModelKey` and `TViewKey` are lookup keys; `TViewModel` and `TView` are concrete outputs.
- Duplicate `(kind, key, contract)` registrations are invalid unless the API explicitly supports replacement through a named `Replace` method.
- Platform packages may add extension methods to source factories from Splat or Microsoft DI, but the base registry contract should not require a particular container.

### Navigation request/result contract

```csharp
public enum NavigationSourceKind
{
    ViewModel,
    View,
}

public sealed record NavigationRequest(
    NavigationSourceKind SourceKind,
    object? SourceInstance,
    Type SourceKey,
    string? Contract,
    object? Parameter,
    NavigationType NavigationType,
    CancellationToken CancellationToken);

public sealed record NavigationResolution(
    IRxObject ViewModel,
    IViewFor View,
    string? Contract,
    object? Parameter,
    NavigationType NavigationType);
```

Typed wrappers should be provided for compile-time ergonomics:

```csharp
public sealed record NavigationResolution<TViewModel, TView>(
    TViewModel ViewModel,
    TView View,
    string? Contract,
    object? Parameter,
    NavigationType NavigationType)
    where TViewModel : class, IRxObject
    where TView : class, IViewFor<TViewModel>;
```

## Public navigation API

### ViewModel-first navigation

Caller supplies a ViewModel instance, concrete type, or interface key. The registered View is resolved.

```csharp
public interface IBidirectionalNavigator
{
    IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        TViewModel viewModel,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    IObservable<NavigationResolution<TViewModel, TView>> NavigateViewModel<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    IObservable<NavigationResolution> NavigateViewModel(
        Type viewModelKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default);
}
```

For interface-key navigation, the typed registration maps interface key to concrete implementation. The caller can use either a generic convenience overload or the untyped bridge:

```csharp
IObservable<NavigationResolution> NavigateViewModel<TViewModelKey>(
    string? contract = null,
    object? parameter = null,
    CancellationToken cancellationToken = default)
    where TViewModelKey : class;
```

### View-first navigation

Caller supplies a View instance, concrete type, or interface key. The registered ViewModel is resolved.

```csharp
public interface IBidirectionalNavigator
{
    IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        TView view,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    IObservable<NavigationResolution<TViewModel, TView>> NavigateView<TViewModel, TView>(
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IRxObject
        where TView : class, IViewFor<TViewModel>;

    IObservable<NavigationResolution> NavigateView(
        Type viewKey,
        string? contract = null,
        object? parameter = null,
        CancellationToken cancellationToken = default);
}
```

Instance rule: when a supplied view already has a non-null `ViewModel` that is assignable to the registered ViewModel implementation, reuse it. Otherwise resolve/create the ViewModel, assign it to `view.ViewModel`, then navigate.

## Resolution algorithm

For ViewModel-first navigation:

1. Normalize the contract.
2. Resolve registration by `(ViewModel, requested key, contract)`.
3. Use the supplied ViewModel instance when provided; otherwise create one from the registration factory.
4. Create/resolve the registered View.
5. Assign `view.ViewModel = viewModel` if it is not already the same instance.
6. Emit a `NavigationResolution` without mutating UI content yet.

For View-first navigation:

1. Normalize the contract.
2. Resolve registration by `(View, requested key, contract)`.
3. Use the supplied View instance when provided; otherwise create one from the registration factory.
4. Reuse `view.ViewModel` if it is assignable to the registered ViewModel implementation; otherwise create/resolve the ViewModel and assign it.
5. Emit a `NavigationResolution` without mutating UI content yet.

For both directions, cancellation must be observed before factory invocation, after asynchronous resolution, before lifecycle callbacks, and before final host commit.

## Host commit pipeline

Core resolution should be independent from host mutation. Hosts should consume a stream of resolutions:

```csharp
IObservable<NavigationResolution> requests = ...;

requests
    .Select(resolution => CommitAsync(resolution).ToObservable())
    .Switch()
    .ObserveOn(RxSchedulers.MainThreadScheduler)
    .Subscribe();
```

Recommended commit sequence:

1. Build `ViewModelNavigatingEventArgs`.
2. Invoke View/VM navigating hooks. If cancelled, do not mutate content, history, or active disposables.
3. Dispose the previous active navigation `CompositeDisposable` for this host only after cancellation has been ruled out.
4. Assign platform content (`Content`, `CurrentPage`, `INavigationView.Navigate`, etc.) on `RxSchedulers.MainThreadScheduler`.
5. Update navigation stack with a pair identity, not ViewModel type alone. Recommended stack entry: `(ViewModelKey, ViewKey, Contract, ViewModelInstance?)`.
6. Invoke `WhenNavigatedFrom` and `WhenNavigatedTo` exactly once per committed transition. Prefer view hooks when the view opted in; otherwise use ViewModel hooks, matching current behavior.
7. Publish `CurrentViewModel`, `CanNavigateBack`, and final navigation event observables.

## Platform split

### Base (`CrissCross`)

Owns:

- `INavigationRegistry`.
- `IBidirectionalNavigator`.
- request/result/exception types.
- navigation lifecycle event normalization.
- TUnit coverage for registration, contracts, missing registrations, cancellation, scheduler selection, and activation/disposal independent of WPF/Avalonia/MAUI.

Does not own:

- WPF `FrameworkElement` or `Content` assignment.
- Avalonia `Control` assignment.
- MAUI `Page`/`Shell` details.

### WPF / Avalonia / WinForms / MAUI packages

Own:

- Adapter from `NavigationResolution` to the platform host/control.
- Platform-specific view type constraints, if necessary.
- Existing `IViewModelRoutedViewHost` compatibility shims.
- UI-library extension methods that register factories from Splat/MS.DI without reflection-based construction.

## Compatibility plan

1. Keep existing `IViewModelRoutedViewHost.Navigate<TViewModel>()` and `Navigate(IRxObject)` as compatibility wrappers over ViewModel-first navigation.
2. Mark existing `NavigateToView<T>()` naming as legacy ambiguous in documentation; route it to `NavigateViewModel<TViewModel>()` semantics until a breaking-change window.
3. Introduce new `NavigateView<TView>()` APIs for actual View-first navigation.
4. Preserve `IViewFor<TViewModel>` registrations as the default View lookup shape.
5. Add reverse registration where current apps register views, for example:

```csharp
registry.Register<MainViewModel, MainView>(
    services => services.GetRequiredService<MainViewModel>(),
    services => services.GetRequiredService<MainView>());
```

For Splat-based apps, extension methods can bridge to `AppLocator.CurrentMutable` with explicit factories.

## Failure modes to specify

- Missing ViewModel registration from View-first request: throw `NavigationResolutionException` with `SourceKind = View`.
- Missing View registration from ViewModel-first request: throw `NavigationResolutionException` with `SourceKind = ViewModel`.
- Contract registered for one direction but not the other: invalid registration, fail at registration time.
- Duplicate key/contract: fail at registration time unless explicitly replacing.
- View has an incompatible existing `ViewModel`: default behavior is to replace it with the registered ViewModel; optionally expose strict mode that throws.
- Cancellation before commit: no content/history/disposable/event mutation.

## Open implementation decisions

- Exact namespace and names for the new base abstractions.
- Whether the default contract normalizes `null` and `""` to `null` or a sentinel value.
- Whether navigation factories should use `IServiceProvider`, Splat `IReadonlyDependencyResolver`, or an adapter abstraction in the core contract. Recommendation: core accepts `IServiceProvider` and package extensions bridge other containers.
- Whether history stores ViewModel instances for back navigation or resolves new instances from registration. Recommendation: store entry identity plus optional instance; back navigation can preserve an existing instance when the prior navigation supplied one.
