// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
