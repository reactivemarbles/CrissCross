// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using ReactiveUI.Winforms;
using Splat;

namespace CrissCross.WinForms.Test.Views;

/// <summary>FirstView member.</summary>
public partial class FirstView : ReactiveUserControl<FirstViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="FirstView"/> class.</summary>
    public FirstView()
    {
        InitializeComponent();
    }

    /// <summary>Gets the button that navigates to the first view.</summary>
    public Button GotoFirstButton => GotoFirst;

    /// <summary>Gets the button that navigates to the main view.</summary>
    public Button GotoMainButton => GotoMain;

    /// <inheritdoc />
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _ = this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<FirstViewModel>();
            _ = this.BindCommand(ViewModel, static vm => vm.GotoFirst, static v => v.GotoFirstButton).DisposeWith(d);
            _ = this.BindCommand(ViewModel, static vm => vm.GotoMain, static v => v.GotoMainButton).DisposeWith(d);
        });
    }
}
