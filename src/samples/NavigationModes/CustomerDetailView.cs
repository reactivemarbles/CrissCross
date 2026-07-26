// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.NavigationModes.Example;

/// <summary>Provides the detail-contract customer page view.</summary>
public sealed class CustomerDetailView : ICustomerPageView, IViewFor<CustomerPageViewModel>
{
    /// <inheritdoc />
    public string ViewKind => "detail";

    /// <summary>Gets or sets the strongly typed view model.</summary>
    public CustomerPageViewModel? ViewModel { get; set; }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (CustomerPageViewModel?)value;
    }
}
