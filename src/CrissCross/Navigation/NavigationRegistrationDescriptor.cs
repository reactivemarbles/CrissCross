// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Stores closed factory delegates for one bidirectional navigation pair.
/// </summary>
internal sealed class NavigationRegistrationDescriptor(
    Type viewModelKey,
    Type viewModelImplementation,
    Type viewKey,
    Type viewImplementation,
    string? contract,
    Func<IServiceProvider, IRxObject> createViewModel,
    Func<IServiceProvider, IViewFor> createView)
{
    public Type ViewModelKey { get; } = viewModelKey;

    public Type ViewModelImplementation { get; } = viewModelImplementation;

    public Type ViewKey { get; } = viewKey;

    public Type ViewImplementation { get; } = viewImplementation;

    public string? Contract { get; } = contract;

    public Func<IServiceProvider, IRxObject> CreateViewModel { get; } = createViewModel;

    public Func<IServiceProvider, IViewFor> CreateView { get; } = createView;
}
