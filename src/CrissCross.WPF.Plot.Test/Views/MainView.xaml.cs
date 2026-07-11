// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot.Test.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.Plot.Test.Views
{
    /// <summary>Interaction logic for MainView.xaml.</summary>
    [IViewFor<MainViewModel>]
    public partial class MainView
    {
        /// <summary>Initializes a new instance of the <see cref="MainView"/> class.</summary>
        public MainView()
        {
            InitializeComponent();
            ViewModel = AppLocator.Current.GetService<MainViewModel>()!;
            _ = this.WhenActivated(d =>
            {
                // OneWay bind ViewModel to View
                _ = this.OneWayBind(ViewModel, vm => vm.YAxisNames, v => v.Chart.YAxisName).DisposeWith(d);
                _ = ViewModel.ReactivePlotSources
                    .ObserveOn(RxSchedulers.MainThreadScheduler)
                    .Subscribe(sources => Chart.ReactivePlotSources = sources)
                    .DisposeWith(d);

                // OneWay bind View to ViewModel
                _ = this.WhenAnyValue(x => x.Chart.ViewModel).BindTo(ViewModel, vm => vm.LiveChartViewModel).DisposeWith(d);
            });
        }
    }
}
