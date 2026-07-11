// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.WPF.UI.CC_Nav.Test.Views;

/// <summary>Interaction logic for MainView.xaml.</summary>
public partial class BrowserView : IUseHostedNavigation
{
    /// <summary>The delay before navigating to the entered URL.</summary>
    private const double WebUrlThrottleSeconds = 0.8;

    /// <summary>Initializes a new instance of the <see cref="BrowserView"/> class.</summary>
    public BrowserView()
    {
        InitializeComponent();
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<BrowserViewModel>();
            _ = this.Bind(ViewModel, vm => vm.WebUrl, v => v.WebUri.Text).DisposeWith(d);
            _ = this.WhenAnyValue(x => x.ViewModel!.WebUrl)
                .Throttle(TimeSpan.FromSeconds(WebUrlThrottleSeconds), RxSchedulers.TaskpoolScheduler)
                .DistinctUntilChanged()
                .Where(query => !string.IsNullOrWhiteSpace(query))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .BindTo(this, vm => vm.browserView.Source)
                .DisposeWith(d);
            this.NavigateToView<MainViewModel>(browserView.Name);
        });
    }
}
