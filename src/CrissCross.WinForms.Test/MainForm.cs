// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using System.Runtime.Versioning;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;

namespace CrissCross.WinForms.Test;

/// <summary>
/// Form1.
/// </summary>
public partial class MainForm : NavigationForm<MainWindowViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    [RequiresPreviewFeatures]
    public MainForm()
    {
        InitializeComponent();
        this.WhenSetup().Subscribe(_ =>
        {
            this.WhenActivated(_ => ViewModel ??= AppLocator.Current.GetService<MainWindowViewModel>() ?? new());
            NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
            this.NavigateToView<MainViewModel>();
        });
    }
}
