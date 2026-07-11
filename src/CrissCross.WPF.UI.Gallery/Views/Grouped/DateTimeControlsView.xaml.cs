// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using ReactiveUI.SourceGenerators;

namespace CrissCross.WPF.UI.Gallery.Views;

/// <summary>Date and Time grouped page.</summary>
[IViewFor<DateTimeControlsViewModel>]
public partial class DateTimeControlsView
{
    /// <summary>Initializes a new instance of the <see cref="DateTimeControlsView"/> class.</summary>
    public DateTimeControlsView() => InitializeComponent();
}
