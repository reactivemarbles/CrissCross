// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using Splat;
using Xamarin.Forms.Xaml;

namespace CrissCross.XamForms.Test.Views
{
    /// <summary>
    /// LoginPage.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = Locator.Current.GetService<LoginViewModel>();
        }
    }
}
