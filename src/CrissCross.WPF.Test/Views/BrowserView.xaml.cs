// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.Test.Views;

/// <summary>
/// Interaction logic for MainView.xaml.
/// </summary>
public partial class BrowserView : IUseHostedNavigation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserView"/> class.
    /// </summary>
    public BrowserView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            ViewModel ??= Locator.Current.GetService<BrowserViewModel>();
            this.Bind(ViewModel, vm => vm.WebUrl, v => v.WebUri.Text).DisposeWith(d);
            this.WhenAnyValue(x => x.ViewModel!.WebUrl)
                .Throttle(TimeSpan.FromSeconds(0.8), RxApp.TaskpoolScheduler)
                .DistinctUntilChanged()
                .Where(query => !string.IsNullOrWhiteSpace(query))
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, vm => vm.browserView.Source)
                .DisposeWith(d);
            browserView.DisposeWith(d);
            this.NavigateToView<MainViewModel>(browserView.Name);
        });
    }
}
