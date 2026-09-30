// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI;
using Splat;

using static ReactiveUI.Binding.ReactiveUIBindingExtensions;

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
        Loaded += OnLoaded;
    }

    /// <summary>Registers activation when the fully constructed view is loaded.</summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<BrowserViewModel>();
            var viewModel = ViewModel ?? throw new InvalidOperationException("The browser view model must be registered before activation.");
            _ = this.Bind(ViewModel, vm => vm.WebUrl, v => v.WebUri.Text).DisposeWith(d);
            _ = viewModel.WhenAnyValue(x => x.WebUrl)
                .Throttle(TimeSpan.FromSeconds(WebUrlThrottleSeconds), RxSchedulers.TaskpoolScheduler)
                .DistinctUntilChanged()
                .Where(static query => !string.IsNullOrWhiteSpace(query))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .BindTo(this, vm => vm.browserView.Source)
                .DisposeWith(d);
            this.NavigateToView(new NavigationKeyRequest<MainViewModel> { Options = new() { HostName = browserView.Name } });
        });
    }
}
