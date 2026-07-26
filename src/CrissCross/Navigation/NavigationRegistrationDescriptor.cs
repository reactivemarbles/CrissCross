// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Stores closed factory delegates for one bidirectional navigation pair.</summary>
/// <param name="viewModelKey">The view model lookup type.</param>
/// <param name="viewModelImplementation">The concrete view model implementation type.</param>
/// <param name="viewKey">The view lookup type.</param>
/// <param name="viewImplementation">The concrete view implementation type.</param>
/// <param name="contract">The optional navigation contract.</param>
/// <param name="createViewModel">The view model factory.</param>
/// <param name="createView">The view factory.</param>
internal sealed class NavigationRegistrationDescriptor(
    Type viewModelKey,
    Type viewModelImplementation,
    Type viewKey,
    Type viewImplementation,
    string? contract,
    Func<IServiceProvider, IRxObject> createViewModel,
    Func<IServiceProvider, IViewFor> createView)
{
    /// <summary>Gets the view model lookup type.</summary>
    internal Type ViewModelKey { get; } = viewModelKey;

    /// <summary>Gets the concrete view model implementation type.</summary>
    internal Type ViewModelImplementation { get; } = viewModelImplementation;

    /// <summary>Gets the view lookup type.</summary>
    internal Type ViewKey { get; } = viewKey;

    /// <summary>Gets the concrete view implementation type.</summary>
    internal Type ViewImplementation { get; } = viewImplementation;

    /// <summary>Gets the optional navigation contract.</summary>
    internal string? Contract { get; } = contract;

    /// <summary>Gets the view model factory.</summary>
    internal Func<IServiceProvider, IRxObject> CreateViewModel { get; } = createViewModel;

    /// <summary>Gets the view factory.</summary>
    internal Func<IServiceProvider, IViewFor> CreateView { get; } = createView;
}
