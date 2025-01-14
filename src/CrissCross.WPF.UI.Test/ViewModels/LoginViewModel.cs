// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test;

/// <summary>
/// Login View Model.
/// </summary>
/// <seealso cref="CrissCross.RxObject" />
public class LoginViewModel : RxObject
{
    private string? _password;
    private string? _username;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    public LoginViewModel() =>
        LoginCommand = ReactiveCommand.Create(() =>
        {
            // This is a placeholder for the actual login logic
            if (Password == "1234" && Username == "User")
            {
                Password = string.Empty;
                Username = string.Empty;
            }
        });

    /// <summary>
    /// Gets the login command.
    /// </summary>
    /// <value>
    /// The login command.
    /// </value>
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
}
