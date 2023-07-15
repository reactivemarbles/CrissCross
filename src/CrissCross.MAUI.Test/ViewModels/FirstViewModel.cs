// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.MAUI.Test
{
    /// <summary>
    /// FirstViewModel.
    /// </summary>
    /// <seealso cref="RxObject" />
    public class FirstViewModel : RxObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstViewModel"/> class.
        /// </summary>
        public FirstViewModel()
        {
            GotoMain = ReactiveCommand.Create(() => this.NavigateToView<MainViewModel>());

            GotoFirst = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
        }

        /// <summary>
        /// Gets the goto main.
        /// </summary>
        /// <value>
        /// The goto main.
        /// </value>
        public ICommand? GotoMain { get; }

        /// <summary>
        /// Gets the goto first.
        /// </summary>
        /// <value>
        /// The goto first.
        /// </value>
        public ICommand? GotoFirst { get; }

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
}
