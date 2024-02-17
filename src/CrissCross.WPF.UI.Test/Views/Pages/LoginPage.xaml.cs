// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.Views.Pages;

/// <summary>
/// Interaction logic for LoginView.xaml.
/// </summary>
public partial class LoginPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage" /> class.
    /// </summary>
    /// <param name="loginViewModel">The login view model.</param>
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        ViewModel = loginViewModel;

        // Bind the password
        ViewModel.WhenAnyValue(x => x.Password).Subscribe(password => Password.Password = password ?? string.Empty);
        Password.Events().PasswordChanged.Select(_ => Password.Password).BindTo(ViewModel, x => x.Password);

        // Bind the username
        ViewModel.WhenAnyValue(x => x.Username).Subscribe(x => UserName.Text = x);
        UserName.Events().TextChanged.Select(_ => UserName.Text).BindTo(ViewModel, x => x.Username);

        LoginButton.Command = ViewModel.LoginCommand;
        UserName.Focus();
    }

    /// <summary>
    /// Gets viewModel used by the view.
    /// Optionally, it may implement <see cref="T:CrissCross.WPF.UI.Controls.INavigationAware" /> and be navigated by <see cref="T:CrissCross.WPF.UI.Controls.INavigationView" />.
    /// </summary>
    public LoginViewModel ViewModel { get; }
}
