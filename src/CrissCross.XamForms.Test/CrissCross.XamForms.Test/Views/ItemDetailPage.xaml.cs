// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using Splat;

namespace CrissCross.XamForms.Test.Views;

/// <summary>
/// ItemDetailPage.
/// </summary>
public partial class ItemDetailPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemDetailPage"/> class.
    /// </summary>
    public ItemDetailPage()
    {
        InitializeComponent();
        BindingContext = ViewModel = Locator.Current.GetService<ItemDetailViewModel>();
    }
}
