// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Describes a typed resolved ViewModel/View navigation pair.
/// </summary>
/// <typeparam name="TViewModel">The resolved view model type.</typeparam>
/// <typeparam name="TView">The resolved view type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigationResolution{TViewModel, TView}"/> class.
/// </remarks>
/// <param name="viewModel">The resolved view model.</param>
/// <param name="view">The resolved view.</param>
/// <param name="contract">The normalized navigation contract.</param>
/// <param name="parameter">The optional navigation parameter.</param>
/// <param name="navigationType">The requested navigation operation type.</param>
public sealed class NavigationResolution<TViewModel, TView>(TViewModel viewModel, TView view, string? contract, object? parameter, NavigationType navigationType)
    where TViewModel : class, IRxObject
    where TView : class, IViewFor<TViewModel>
{
    /// <summary>
    /// Gets the resolved view model.
    /// </summary>
    public TViewModel ViewModel { get; } = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

    /// <summary>
    /// Gets the resolved view.
    /// </summary>
    public TView View { get; } = view ?? throw new ArgumentNullException(nameof(view));

    /// <summary>
    /// Gets the normalized navigation contract.
    /// </summary>
    public string? Contract { get; } = NavigationContract.Normalize(contract);

    /// <summary>
    /// Gets the optional navigation parameter.
    /// </summary>
    public object? Parameter { get; } = parameter;

    /// <summary>
    /// Gets the requested navigation operation type.
    /// </summary>
    public NavigationType NavigationType { get; } = navigationType;
}
