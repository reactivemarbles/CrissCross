// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;

namespace CrissCross.XamForms.Test.ViewModels;

/// <summary>
/// LoginViewModel.
/// </summary>
/// <seealso cref="BaseViewModel" />
public class LoginViewModel : BaseViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    public LoginViewModel()
    {
        LoginCommand = ReactiveCommand.Create<object>(o => OnLoginClicked(o));
    }

    /// <summary>
    /// Gets the login command.
    /// </summary>
    /// <value>
    /// The login command.
    /// </value>
    public ReactiveCommand<object, Unit> LoginCommand { get; }

    private void OnLoginClicked(object obj)
    {
        this.NavigateToView<AboutViewModel>();
    }
}