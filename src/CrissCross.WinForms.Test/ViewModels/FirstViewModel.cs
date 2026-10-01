// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WinForms.Test;

/// <summary>FirstViewModel member.</summary>
public class FirstViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="FirstViewModel"/> class.</summary>
    public FirstViewModel()
    {
        GotoMain = ReactiveCommand.Create(() =>
        {
            this.NavigateToView(new NavigationKeyRequest<MainViewModel> { Options = new() { HostName = nameof(MainForm) } });
            this.NavigateToView(new NavigationKeyRequest<FirstViewModel> { Options = new() { HostName = nameof(SecondForm) } });
        });

        GotoFirst = ReactiveCommand.Create(() =>
        {
            this.NavigateToView(new NavigationKeyRequest<MainViewModel> { Options = new() { HostName = nameof(SecondForm) } });
            this.NavigateToView(new NavigationKeyRequest<FirstViewModel> { Options = new() { HostName = nameof(MainForm) } });
        });
    }

    /// <summary>Gets the goto main.</summary>
    /// <value>
    /// The goto main.
    /// </value>
    public ICommand? GotoMain { get; }

    /// <summary>Gets the goto first.</summary>
    /// <value>
    /// The goto first.
    /// </value>
    public ICommand? GotoFirst { get; }

    /// <summary>WhenNavigatedTo member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(e);
        Debug.WriteLine($"Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
        base.WhenNavigatedTo(e, disposables);
    }

    /// <summary>WhenNavigatedFrom member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        Debug.WriteLine($"Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigatedFrom(e);
    }

    /// <summary>WhenNavigating member.</summary>
    /// <inheritdoc />
    public override void WhenNavigating(IViewModelNavigatingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        Debug.WriteLine($"Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigating(e);
    }
}
