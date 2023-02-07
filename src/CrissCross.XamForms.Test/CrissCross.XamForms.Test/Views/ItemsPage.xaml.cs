// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.XamForms.Test.Views
{
    /// <summary>
    /// ItemsPage.
    /// </summary>
    /// <seealso cref="ReactiveUI.XamForms.ReactiveContentPage&lt;CrissCross.XamForms.Test.ViewModels.ItemsViewModel&gt;" />
    public partial class ItemsPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsPage"/> class.
        /// </summary>
        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = Locator.Current.GetService<ItemsViewModel>();
            this.WhenActivated(d =>
            {
                ViewModel.OnAppearing();
            });
        }
    }
}
