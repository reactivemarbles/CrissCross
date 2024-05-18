// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.XamForms.Test.Views;

/// <summary>
/// ItemsPage.
/// </summary>
public partial class ItemsPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsPage"/> class.
    /// </summary>
    public ItemsPage()
    {
        InitializeComponent();

        BindingContext = ViewModel = Locator.Current.GetService<ItemsViewModel>();
        this.WhenActivated(_ => ViewModel?.OnAppearing());
    }
}
