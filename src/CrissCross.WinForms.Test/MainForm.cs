// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;
using Splat;

namespace CrissCross.WinForms.Test;

/// <summary>Form1 member.</summary>
public partial class MainForm : NavigationForm<MainWindowViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MainForm"/> class.</summary>
    [RequiresPreviewFeatures]
    public MainForm()
    {
        InitializeComponent();
        _ = this.WhenSetup()
            .Subscribe(setupComplete =>
            {
                _ = this.WhenActivated(
                    (CompositeDisposable activationDisposables) =>
                        ViewModel ??= AppLocator.Current.GetService<MainWindowViewModel>() ?? new());
                NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
                this.NavigateToView<MainViewModel>();
            });
    }
}
