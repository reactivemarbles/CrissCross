﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.UI.CC_Nav.Test;

/// <summary>
/// BrowserViewModel.
/// </summary>
/// <seealso cref="CrissCross.RxObject" />
public class BrowserViewModel : RxObject
{
    private string _WebUrl = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserViewModel"/> class.
    /// </summary>
    public BrowserViewModel() =>
        this.BuildComplete(() =>
        {
            GotoMain = ReactiveCommand.Create(() =>
            {
                MainWindow.Navigation?.NavigateToView<MainViewModel>("mainWindow");
                ////this.NavigateToView<FirstViewModel>("secondWindow");
            });

            GotoFirst = ReactiveCommand.Create(() =>
            {
                ////this.NavigateToView<MainViewModel>("secondWindow");
                MainWindow.Navigation?.NavigateToView<FirstViewModel>("mainWindow");
            });
            WebUrl = "https://www.aicsolutions.com";
        });

    /// <summary>
    /// Gets or sets the web URL.
    /// </summary>
    /// <value>
    /// The web URL.
    /// </value>
    public string WebUrl { get => _WebUrl; set => this.RaiseAndSetIfChanged(ref _WebUrl, value); }

    /// <summary>
    /// Gets the goto main.
    /// </summary>
    /// <value>
    /// The goto main.
    /// </value>
    public ICommand? GotoMain { get; private set; }

    /// <summary>
    /// Gets the goto first.
    /// </summary>
    /// <value>
    /// The goto first.
    /// </value>
    public ICommand? GotoFirst { get; private set; }

    /// <summary>
    /// WhenNavigatedTo.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="disposables"></param>
    /// <inheritdoc />
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
        base.WhenNavigatedTo(e, disposables);
    }

    /// <summary>
    /// WhenNavigatedFrom.
    /// </summary>
    /// <param name="e"></param>
    /// <inheritdoc />
    public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigatedFrom(e);
    }

    /// <summary>
    /// WhenNavigating.
    /// </summary>
    /// <param name="e"></param>
    /// <inheritdoc />
    public override void WhenNavigating(IViewModelNavigatingEventArgs e)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        Debug.WriteLine($"{DateTime.Now} Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigating(e);
    }
}
