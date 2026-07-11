// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Test.Views.Pages;

/// <summary>Interaction logic for LoginView.xaml.</summary>
public partial class LoginPage
{
    /// <summary>Initializes a new instance of the <see cref="LoginPage" /> class.</summary>
    /// <param name="loginViewModel">The login view model.</param>
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        ViewModel = loginViewModel;

        // Bind the password
        _ = ViewModel.WhenAnyValue(x => x.Password).Subscribe(password => Password.Password = password ?? string.Empty);
        _ = EventSignal
            .From<RoutedEventHandler, RoutedEventArgs>(handler => handler.Invoke, handler => Password.PasswordChanged += handler, handler => Password.PasswordChanged -= handler)
            .Select(_ => Password.Password)
            .BindTo(ViewModel, x => x.Password);

        // Bind the username
        _ = ViewModel.WhenAnyValue(x => x.Username).Subscribe(x => UserName.Text = x);
        _ = EventSignal
            .From<TextChangedEventHandler, TextChangedEventArgs>(handler => handler.Invoke, handler => UserName.TextChanged += handler, handler => UserName.TextChanged -= handler)
            .Select(_ => UserName.Text)
            .BindTo(ViewModel, x => x.Username);

        LoginButton.Command = ViewModel.LoginCommand;
        _ = UserName.Focus();
    }

    /// <summary>
    /// Gets viewModel used by the view.
    /// Optionally, it may implement <see cref="T:CrissCross.WPF.UI.Controls.INavigationAware" /> and be navigated by <see cref="T:CrissCross.WPF.UI.Controls.INavigationView" />.
    /// </summary>
    public LoginViewModel ViewModel { get; }
}
