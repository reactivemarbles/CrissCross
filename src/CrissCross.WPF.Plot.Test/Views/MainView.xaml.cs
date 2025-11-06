// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using CrissCross.WPF.Plot.Test.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.Plot.Test.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    [IViewFor<MainViewModel>]
    public partial class MainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<MainViewModel>()!;
            this.WhenActivated(d =>
            {
                // OneWay bind ViewModel to View
                this.OneWayBind(ViewModel, vm => vm.LiveChartSubject, v => v.Chart.SignalObservablesWithTimeStamp).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.YAxisNames, v => v.Chart.YAxisName).DisposeWith(d);

                // OneWay bind View to ViewModel
                this.WhenAnyValue(x => x.Chart.ViewModel).BindTo(ViewModel, vm => vm.LiveChartViewModel).DisposeWith(d);
            });
        }
    }
}
