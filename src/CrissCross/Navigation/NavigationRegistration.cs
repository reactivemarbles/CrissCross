// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>Defines a typed ViewModel/View registration and caller-facing lookup keys.</summary>
/// <typeparam name="TMK">The caller-facing view model lookup key.</typeparam>
/// <typeparam name="TM">The concrete view model type.</typeparam>
/// <typeparam name="TK">The caller-facing view lookup key.</typeparam>
/// <typeparam name="TV">The concrete view type.</typeparam>
public sealed class NavigationRegistration<TMK, TM, TK, TV>
    where TMK : class
    where TM : class, TMK, IRxObject
    where TK : class
    where TV : class, TK, IViewFor<TM>
{
    /// <summary>Initializes a new instance of the <see cref="NavigationRegistration{TMK,TM,TK,TV}"/> class.</summary>
    /// <param name="createViewModel">The view model factory.</param>
    /// <param name="createView">The view factory.</param>
    public NavigationRegistration(
        Func<IServiceProvider, TM> createViewModel,
        Func<IServiceProvider, TV> createView)
    {
        CreateViewModel = createViewModel ?? throw new ArgumentNullException(nameof(createViewModel));
        CreateView = createView ?? throw new ArgumentNullException(nameof(createView));
    }

    /// <summary>Gets the view model factory.</summary>
    public Func<IServiceProvider, TM> CreateViewModel { get; }

    /// <summary>Gets the view factory.</summary>
    public Func<IServiceProvider, TV> CreateView { get; }

    /// <summary>Gets or sets the optional navigation contract.</summary>
    public string? Contract { get; set; }

    /// <summary>Gets the runtime caller-facing view model key.</summary>
    public Type ViewModelKey => typeof(TMK);

    /// <summary>Gets the runtime caller-facing view key.</summary>
    public Type ViewKey => typeof(TK);
}
