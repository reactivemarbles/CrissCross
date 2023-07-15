// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WinForms.Test;

#pragma warning disable IDE0071 // Simplify interpolation
/// <summary>
/// FirstViewModel.
/// </summary>
public class FirstViewModel : RxObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstViewModel"/> class.
    /// </summary>
    public FirstViewModel() =>
        this.BuildComplete(() =>
            {
                GotoMain = ReactiveCommand.Create(() =>
                {
                    this.NavigateToView<MainViewModel>("MainForm");
                    this.NavigateToView<FirstViewModel>("SecondForm");
                });

                GotoFirst = ReactiveCommand.Create(() =>
                {
                    this.NavigateToView<MainViewModel>("SecondForm");
                    this.NavigateToView<FirstViewModel>("MainForm");
                });
            });

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

        Debug.WriteLine($"{DateTime.Now.ToString()} Navigated To: {e.To?.Name} From: {e.From?.Name} with Host {e.HostName}");
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

        Debug.WriteLine($"{DateTime.Now.ToString()} Navigated From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
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

        Debug.WriteLine($"{DateTime.Now.ToString()} Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
        base.WhenNavigating(e);
    }
}
#pragma warning restore IDE0071 // Simplify interpolation

