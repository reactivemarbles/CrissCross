// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using ReactiveUI;

namespace CrissCross.XamForms.Test.Views;

/// <summary>
/// AboutPage.
/// </summary>
public partial class AboutPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutPage"/> class.
    /// </summary>
    public AboutPage()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.NavigateBack, v => v.btnBack).DisposeWith(d);
        });
    }
}
