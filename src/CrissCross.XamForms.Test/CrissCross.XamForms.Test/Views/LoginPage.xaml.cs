// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using Splat;
using Xamarin.Forms.Xaml;

namespace CrissCross.XamForms.Test.Views;

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
