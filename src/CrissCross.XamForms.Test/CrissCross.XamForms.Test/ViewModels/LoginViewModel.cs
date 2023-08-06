// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive;
using CrissCross;
using ReactiveUI;

namespace CrissCross.XamForms.Test.ViewModels
{
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
}