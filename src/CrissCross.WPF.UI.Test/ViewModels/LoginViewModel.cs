// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Test;

/// <summary>Login View Model.</summary>
/// <seealso cref="CrissCross.RxObject" />
public class LoginViewModel : RxObject
{

    /// <summary>Initializes a new instance of the <see cref="LoginViewModel"/> class.</summary>
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

    /// <summary>Gets the login command.</summary>
    /// <value>
    /// The login command.
    /// </value>
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    /// <summary>Gets or sets the password.</summary>
    /// <value>
    /// The password.
    /// </value>
    public string? Password
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the username.</summary>
    /// <value>
    /// The username.
    /// </value>
    public string? Username
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
