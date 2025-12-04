// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using CrissCross.WPF.Plot.Test.Views;
using CrissCross.WPF.UI;
using CrissCross.WPF.UI.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.Plot.Test.ViewModels
{
    /// <summary>
    /// MainWindowViewModel.
    /// </summary>
    /// <seealso cref="RxObject" />
    public partial class MainWindowViewModel : RxObject
    {
        private readonly Tracker? _tracker;

        [Reactive]
        private string _applicationTitle = "CrissCross UI Plot Test";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            _tracker = new();
            SetupTracker();
            NavigationModels = [];
            NavigationModels.AddRange(
            [
                new NavigationModel(null, NavigationModels) { IsExpander = true, Icon = new SymbolIcon(SymbolRegular.LineHorizontal320) },
                new NavigationModel(typeof(MainViewModel), NavigationModels) { Name = "Main", Icon = new SymbolIcon(SymbolRegular.Home20), IsSelected = true },
            ]);

            // Register ViewModels and Views
            AppLocator.CurrentMutable.RegisterLazySingletonAnd(static () => new MainViewModel()).Register<IViewFor<MainViewModel>>(static () => new MainView());
            AppLocator.CurrentMutable.SetupComplete();
        }

        /// <summary>
        /// Gets the navigation models.
        /// </summary>
        /// <value>
        /// The navigation models.
        /// </value>
        public List<NavigationModel> NavigationModels { get; }

        private void SetupTracker()
        {
            AppLocator.CurrentMutable.RegisterConstant(_tracker);
            _tracker?.Configure<MainWindow>()
                .Id(w => w.Name, $"[Width={SystemParameters.VirtualScreenWidth},Height{SystemParameters.VirtualScreenHeight}]")
                .Properties(w => new { w.Height, w.Width, w.Left, w.Top, w.WindowState })
                .PersistOn(w => nameof(w.Closing))
                .StopTrackingOn(w => nameof(w.Closing));
        }
    }
}
