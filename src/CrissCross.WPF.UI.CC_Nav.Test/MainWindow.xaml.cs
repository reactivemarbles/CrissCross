// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls;
using ReactiveUI;

namespace CrissCross.WPF.UI.CC_Nav.Test
{
    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    public class MainWindow
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
        public MainWindow()
        {
            Appearance.SystemThemeWatcher.Watch(this);

            _ = InitializeComponent();

            _ = Breadcrumb.SetupNavigation("mainWindow");
            Navigation = Breadcrumb;

            _ = this.WhenActivated(d =>
            {
                NavBack.Command = ReactiveCommand.Create(() => Navigation.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
                Navigation.NavigateTo<MainViewModel>(breadcrumbItemContent: "Main View");
            });
        }

        /// <summary>Gets the navigation.</summary>
        /// <value>
        /// The navigation.
        /// </value>
        public static BreadcrumbBar? Navigation { get; private set; }
    }
}
