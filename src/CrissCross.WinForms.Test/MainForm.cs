// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Splat;

namespace CrissCross.WinForms.Test;

/// <summary>Form1 member.</summary>
public partial class MainForm : NavigationForm<MainWindowViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MainForm"/> class.</summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ViewModel ??= AppLocator.Current.GetService<MainWindowViewModel>() ?? new();
        NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
        this.NavigateToView(new NavigationKeyRequest<MainViewModel>());
    }
}
