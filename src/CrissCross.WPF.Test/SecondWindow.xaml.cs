// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross.WPF.Test
{
    /// <summary>
    /// Interaction logic for SecondWindow.xaml.
    /// </summary>
    public partial class SecondWindow : IUseNavigation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecondWindow"/> class.
        /// </summary>
        public SecondWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.NavigateToView<FirstViewModel>();
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack).DisposeWith(d);
            });
        }
    }
}
