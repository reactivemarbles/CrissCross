// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>
/// DashboardViewModel.
/// </summary>
/// <seealso cref="RxObject" />
public class DashboardViewModel : RxObject, IControlAppBar
{
    private int _counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
    /// </summary>
    public DashboardViewModel() => CounterIncrementCommand = ReactiveCommand.Create(OnCounterIncrement);

    /// <summary>
    /// Gets or sets the counter.
    /// </summary>
    /// <value>
    /// The counter.
    /// </value>
    public int Counter
    {
        get => _counter;
        set => this.RaiseAndSetIfChanged(ref _counter, value);
    }

    /// <summary>
    /// Gets the counter increment.
    /// </summary>
    /// <value>
    /// The counter increment.
    /// </value>
    public ReactiveCommand<Unit, Unit> CounterIncrementCommand { get; }

    private void OnCounterIncrement()
    {
        Counter++;
        this.ShowAppBar();
    }
}
