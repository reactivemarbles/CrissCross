// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.MAUI.Test;

/// <summary>FirstView member.</summary>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FirstView : ReactiveUI.Maui.ReactiveContentPage<FirstViewModel>
{
    /// <summary>Provides the activation registration.</summary>
    private IDisposable? _activation;

    /// <summary>Initializes a new instance of the <see cref="FirstView"/> class.</summary>
    public FirstView()
    {
        InitializeComponent();
    }

    /// <summary>Gets the GotoMain navigation button.</summary>
    public Button GotoMainButton => GotoMain;

    /// <summary>Gets the GotoFirst navigation button.</summary>
    public Button GotoFirstButton => GotoFirst;

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _activation ??= this.WhenActivated(d =>
        {
            ViewModel ??= AppLocator.Current.GetService<FirstViewModel>();
            _ = this.BindCommand(ViewModel, vm => vm.GotoMain, v => v.GotoMainButton).DisposeWith(d);
            _ = this.BindCommand(ViewModel, vm => vm.GotoFirst, v => v.GotoFirstButton).DisposeWith(d);
        });
    }
}
