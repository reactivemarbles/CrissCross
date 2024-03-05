// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross.WPF.UI.CC_Nav.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            Appearance.SystemThemeWatcher.Watch(this);

            InitializeComponent();
            this.WhenActivated(d =>
            {
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
                this.NavigateToView<MainViewModel>();
            });
        }
    }
}
