// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross.XamForms.Test.Views
{
    public partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.NavigateBack, v => v.btnBack).DisposeWith(d);
            });
        }
    }
}