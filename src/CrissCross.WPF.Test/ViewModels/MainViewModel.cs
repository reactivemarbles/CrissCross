// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.WPF.Test
{
    /// <summary>
    /// MainViewModel.
    /// </summary>
    /// <seealso cref="CrissCross.RxObject" />
    public class MainViewModel : RxObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            GotoFirst = ReactiveCommand.Create(() =>
            {
                this.NavigateToView<MainViewModel>("secondWindow");
                this.NavigateToView<FirstViewModel>("mainWindow");
            });

            GotoMain = ReactiveCommand.Create(() =>
            {
                this.NavigateToView<MainViewModel>("mainWindow");
                this.NavigateToView<FirstViewModel>("secondWindow");
            });
        }

        /// <summary>
        /// Gets the goto first.
        /// </summary>
        /// <value>
        /// The goto first.
        /// </value>
        public ICommand? GotoFirst { get; }

        /// <summary>
        /// Gets the goto main.
        /// </summary>
        /// <value>
        /// The goto main.
        /// </value>
        public ICommand? GotoMain { get; }

        /// <summary>
        /// WhenNavigatedTo.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="disposables"></param>
        /// <inheritdoc />
        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
        {
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
            Debug.WriteLine($"{DateTime.Now} Navigating From: {e.From?.Name} To: {e.To?.Name} with Host {e.HostName}");
            base.WhenNavigating(e);
        }
    }
}