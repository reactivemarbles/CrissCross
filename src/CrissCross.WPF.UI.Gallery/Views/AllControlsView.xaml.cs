// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for AllControlsView.xaml.
/// </summary>
[IViewFor<AllControlsViewModel>]
public partial class AllControlsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllControlsView"/> class.
    /// </summary>
    public AllControlsView()
    {
        InitializeComponent();
        DataContext = ViewModel = AppLocator.Current.GetService<AllControlsViewModel>()!;
    }
}
