﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>
/// Interaction logic for ImageView.xaml.
/// </summary>
[IViewFor<ImageViewModel>]
public partial class ImageView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageView"/> class.
    /// </summary>
    public ImageView()
    {
        InitializeComponent();
    }
}
