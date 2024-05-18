// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.Models;
using CrissCross.XamForms.Test.ViewModels;
using Splat;

namespace CrissCross.XamForms.Test.Views;

/// <summary>
/// NewItemPage.
/// </summary>
public partial class NewItemPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewItemPage"/> class.
    /// </summary>
    public NewItemPage()
    {
        InitializeComponent();
        BindingContext = ViewModel = Locator.Current.GetService<NewItemViewModel>();
    }

    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    /// <value>
    /// The item.
    /// </value>
    public Item? Item { get; set; }
}
