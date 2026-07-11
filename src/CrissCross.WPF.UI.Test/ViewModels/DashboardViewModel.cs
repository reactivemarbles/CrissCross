// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>DashboardViewModel member.</summary>
/// <seealso cref="RxObject" />
public class DashboardViewModel : RxObject, IControlAppBar
{
    /// <summary>Initializes a new instance of the <see cref="DashboardViewModel"/> class.</summary>
    public DashboardViewModel() => CounterIncrementCommand = ReactiveCommand.Create(OnCounterIncrement);

    /// <summary>Gets a value indicating whether the app bar remains visible.</summary>
    public bool AppBarIsSticky => false;

    /// <summary>Gets or sets the counter.</summary>
    /// <value>
    /// The counter.
    /// </value>
    public int Counter
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the counter increment.</summary>
    /// <value>
    /// The counter increment.
    /// </value>
    public ReactiveCommand<Unit, Unit> CounterIncrementCommand { get; }

    /// <summary>Increments the counter and displays the app bar.</summary>
    private void OnCounterIncrement()
    {
        Counter++;
        this.ShowAppBar();
    }
}
