// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>
/// Main view model for the gallery application.
/// </summary>
public class MainViewModel : RxObject, IUseHostedNavigation
{
    private const string HostName = "mainNavHost";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() =>
        this.BuildComplete(() =>
        {
            DisplayName = "Gallery";

            // Navigation commands for each control category - use IUseHostedNavigation with explicit host name
            GotoButtons = ReactiveCommand.Create(() => this.NavigateToView<ButtonsPageViewModel>());
            GotoInput = ReactiveCommand.Create(() => this.NavigateToView<InputPageViewModel>());
            GotoProgress = ReactiveCommand.Create(() => this.NavigateToView<ProgressPageViewModel>());
            GotoHome = ReactiveCommand.Create(() => this.NavigateToView<HomePageViewModel>());
        });

    /// <summary>
    /// Gets the goto buttons command.
    /// </summary>
    public ICommand? GotoButtons { get; private set; }

    /// <summary>
    /// Gets the goto input command.
    /// </summary>
    public ICommand? GotoInput { get; private set; }

    /// <summary>
    /// Gets the goto progress command.
    /// </summary>
    public ICommand? GotoProgress { get; private set; }

    /// <summary>
    /// Gets the goto home command.
    /// </summary>
    public ICommand? GotoHome { get; private set; }
}
