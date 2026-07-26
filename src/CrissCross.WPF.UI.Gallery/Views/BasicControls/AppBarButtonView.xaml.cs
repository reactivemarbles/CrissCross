// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for AppBarButtonView.xaml.</summary>
[IViewFor<AppBarButtonViewModel>]
public partial class AppBarButtonView
{
    /// <summary>Initializes a new instance of the <see cref="AppBarButtonView"/> class.</summary>
    public AppBarButtonView() => InitializeComponent();
}
