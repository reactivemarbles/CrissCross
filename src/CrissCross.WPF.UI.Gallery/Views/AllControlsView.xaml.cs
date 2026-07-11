// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for AllControlsView.xaml.</summary>
[IViewFor<AllControlsViewModel>]
public partial class AllControlsView
{
    /// <summary>Initializes a new instance of the <see cref="AllControlsView"/> class.</summary>
    public AllControlsView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<AllControlsViewModel>()!;
        DataContext = ViewModel;
    }
}
