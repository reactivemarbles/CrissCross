// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI.SourceGenerators;
using Splat;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Interaction logic for the reactive feature playground.</summary>
[IViewFor<FeaturePlaygroundViewModel>]
public partial class FeaturePlaygroundView
{
    /// <summary>Initializes a new instance of the <see cref="FeaturePlaygroundView"/> class.</summary>
    public FeaturePlaygroundView()
    {
        InitializeComponent();
        ViewModel = AppLocator.Current.GetService<FeaturePlaygroundViewModel>()!;
        DataContext = ViewModel;
    }
}
