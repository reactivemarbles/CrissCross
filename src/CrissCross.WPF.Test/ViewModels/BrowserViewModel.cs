// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.Test;

/// <summary>BrowserViewModel member.</summary>
/// <seealso cref="CrissCross.RxObject" />
public class BrowserViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="BrowserViewModel"/> class.</summary>
    public BrowserViewModel() =>
        this.BuildComplete(() =>
        {
            GotoMain = ReactiveCommand.Create(() =>
            {
                this.NavigateToView<MainViewModel>("mainWindow");
                this.NavigateToView<FirstViewModel>("secondWindow");
            });

            GotoFirst = ReactiveCommand.Create(() =>
            {
                this.NavigateToView<MainViewModel>("secondWindow");
                this.NavigateToView<FirstViewModel>("mainWindow");
            });
            WebUrl = "https://www.aicsolutions.com";
        });

    /// <summary>Gets or sets the web URL.</summary>
    /// <value>
    /// The web URL.
    /// </value>
    public string WebUrl { get => field; set => this.RaiseAndSetIfChanged(ref field, value); }
= string.Empty;

    /// <summary>Gets the goto main.</summary>
    /// <value>
    /// The goto main.
    /// </value>
    public ICommand? GotoMain { get; private set; }

    /// <summary>Gets the goto first.</summary>
    /// <value>
    /// The goto first.
    /// </value>
    public ICommand? GotoFirst { get; private set; }

    /// <summary>WhenNavigatedTo member.</summary>
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

    /// <summary>WhenNavigatedFrom member.</summary>
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

    /// <summary>WhenNavigating member.</summary>
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
