// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Describes a resolved platform-neutral ViewModel/View navigation pair.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigationResolution"/> class.
/// </remarks>
/// <param name="viewModel">The resolved view model.</param>
/// <param name="view">The resolved view.</param>
/// <param name="contract">The normalized navigation contract.</param>
/// <param name="parameter">The optional navigation parameter.</param>
/// <param name="navigationType">The requested navigation operation type.</param>
public sealed class NavigationResolution(IRxObject viewModel, IViewFor view, string? contract, object? parameter, NavigationType navigationType)
{
    /// <summary>
    /// Gets the resolved view model.
    /// </summary>
    public IRxObject ViewModel { get; } = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

    /// <summary>
    /// Gets the resolved view.
    /// </summary>
    public IViewFor View { get; } = view ?? throw new ArgumentNullException(nameof(view));

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
