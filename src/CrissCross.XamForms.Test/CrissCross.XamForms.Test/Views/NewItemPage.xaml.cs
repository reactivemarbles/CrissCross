// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using CrissCross.XamForms.Test.Models;
using Splat;

namespace CrissCross.XamForms.Test.Views
{
    /// <summary>
    /// NewItemPage.
    /// </summary>
    /// <seealso cref="ReactiveUI.XamForms.ReactiveContentPage&lt;CrissCross.XamForms.Test.ViewModels.NewItemViewModel&gt;" />
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
        public Item Item { get; set; }
    }
}