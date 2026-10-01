// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.UI.CC_Nav.Test;

/// <summary>FirstViewModel member.</summary>
/// <seealso cref="RxObject" />
public class FirstViewModel : RxObject
{
    /// <summary>Provides the clock for navigation diagnostics.</summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>Initializes a new instance of the <see cref="FirstViewModel"/> class.</summary>
    public FirstViewModel()
        : this(TimeProvider.System)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FirstViewModel"/> class.</summary>
    /// <param name="timeProvider">The clock for navigation diagnostics.</param>
    public FirstViewModel(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

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

    /// <summary>Registers commands after construction and dependency registration.</summary>
    public void InitializeCommands() =>
        this.BuildComplete(() =>
        {
            DisplayName = "First View";
            GotoMain = ReactiveCommand.Create(static () =>
                MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<MainViewModel>(), "First View"));

            GotoFirst = ReactiveCommand.Create(() =>
                MainWindow.Navigation?.NavigateTo(new NavigationKeyRequest<FirstViewModel>(), DisplayName));
        });

    /// <summary>WhenNavigatedTo member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(e);

        Debug.WriteLine($"{_timeProvider.GetLocalNow()} Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
        base.WhenNavigatedTo(e, disposables);
    }

    /// <summary>WhenNavigatedFrom member.</summary>
    /// <inheritdoc />
    public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Debug.WriteLine($"{_timeProvider.GetLocalNow()} Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigatedFrom(e);
    }

    /// <summary>WhenNavigating member.</summary>
    /// <inheritdoc />
    public override void WhenNavigating(IViewModelNavigatingEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Debug.WriteLine($"{_timeProvider.GetLocalNow()} Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigating(e);
    }
}
