// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using ReactiveUI;
using Xamarin.Essentials;

namespace CrissCross.XamForms.Test.ViewModels;

/// <summary>
/// AboutViewModel.
/// </summary>
/// <seealso cref="BaseViewModel" />
public class AboutViewModel : BaseViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
    /// </summary>
    public AboutViewModel()
    {
        Title = "About";
        this.WhenSetup().Subscribe(_ =>
        {
            OpenWebCommand = ReactiveCommand.CreateFromTask(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            NavigateBack = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
        });
    }

    /// <summary>
    /// Gets the navigate back.
    /// </summary>
    /// <value>
    /// The navigate back.
    /// </value>
    public ICommand? NavigateBack { get; private set; }

    /// <summary>
    /// Gets the open web command.
    /// </summary>
    /// <value>
    /// The open web command.
    /// </value>
    public ICommand? OpenWebCommand { get; private set; }
}
