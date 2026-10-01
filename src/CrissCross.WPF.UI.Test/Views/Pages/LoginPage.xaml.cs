// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Test.Views.Pages;

/// <summary>Interaction logic for LoginView.xaml.</summary>
public partial class LoginPage
{
    /// <summary>Owns subscriptions while the page is loaded.</summary>
    private CompositeDisposable? _bindings;

    /// <summary>Initializes a new instance of the <see cref="LoginPage" /> class.</summary>
    /// <param name="loginViewModel">The login view model.</param>
    public LoginPage(LoginViewModel loginViewModel)
    {
        ArgumentNullException.ThrowIfNull(loginViewModel);
        InitializeComponent();
        ViewModel = loginViewModel;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        LoginButton.Command = ViewModel.LoginCommand;
        _ = UserName.Focus();
    }

    /// <summary>
    /// Gets viewModel used by the view.
    /// Optionally, it may implement <see cref="T:CrissCross.WPF.UI.Controls.INavigationAware" /> and be navigated by
    /// <see cref="T:CrissCross.WPF.UI.Controls.INavigationView" />.
    /// </summary>
    public LoginViewModel ViewModel { get; }

    /// <summary>Registers bindings while the page is loaded.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _bindings?.Dispose();
        var bindings = new CompositeDisposable();
        _bindings = bindings;
        _ = ViewModel.WhenAnyValue(x => x.Password)
            .Subscribe(password => Password.Password = password ?? string.Empty)
            .DisposeWith(bindings);
        _ = ViewModel.WhenAnyValue(x => x.Username)
            .Subscribe(username => UserName.Text = username)
            .DisposeWith(bindings);
        Password.PasswordChanged += OnPasswordChanged;
        UserName.TextChanged += OnUsernameChanged;
    }

    /// <summary>Releases bindings when the page is unloaded.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Password.PasswordChanged -= OnPasswordChanged;
        UserName.TextChanged -= OnUsernameChanged;
        _bindings?.Dispose();
        _bindings = null;
    }

    /// <summary>Copies password edits to the view model.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnPasswordChanged(object? sender, RoutedEventArgs e) => ViewModel.Password = Password.Password;

    /// <summary>Copies username edits to the view model.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnUsernameChanged(object sender, TextChangedEventArgs e) => ViewModel.Username = UserName.Text;
}
