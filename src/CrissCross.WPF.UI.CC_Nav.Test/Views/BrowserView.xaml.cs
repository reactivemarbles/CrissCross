// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.CC_Nav.Test.Views;

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
            this.NavigateToView<MainViewModel>(browserView.Name);
        });
    }
}
